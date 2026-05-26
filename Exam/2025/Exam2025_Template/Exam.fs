module Exam2025_Template.Exam

    open JParsec.TextParser

    (* ═══════════════════════════════════════════════════════════════
       Question 1: Coordinates and rectangles (25%)
       ═══════════════════════════════════════════════════════════════ *)

    type coord     = C of int * int
    type rectangle = R of coord * int * int


    (* ── Question 1.1 ────────────────────────────────────────────── *)

    (* valid: a rectangle is valid when both its width AND its height are
       strictly positive (> 0).  A zero or negative dimension makes no
       geometric sense.

       PATTERN SYNTAX:
       `R(_, w, h)` destructures the rectangle and gives names to the width
       and height components, discarding the corner coordinate with `_`.
       We don't need the corner to check validity — only the dimensions matter.

       `w > 0 && h > 0` uses `&&` (logical AND): both conditions must hold. *)
    let valid (R(_, w, h)) = w > 0 && h > 0


    (* ── Question 1.2 ────────────────────────────────────────────── *)

    (* coords: returns the set of ALL integer grid points (coords) that lie
       inside or on the border of a rectangle.

       HOW IT WORKS:
       The rectangle R(C(x,y), w, h) has:
         - bottom-left corner at (x, y)
         - width w  → x-coordinates run from x to x+w-1
         - height h → y-coordinates run from y to y+h-1
       We iterate every (cx, cy) pair in that range and add C(cx,cy) to a Set.

       STRUCTURE — two nested recursive helpers:
         auxX cx: builds the set for all columns from cx to x+w-1.
                  For each column cx, it uses auxY to collect all rows,
                  then unions that column's set with the rest (auxX (cx+1)).
         auxY cy: builds the set for one column cx, rows cy to y+h-1.
                  Adds C(cx, cy) and recurses on cy+1.

       WHY NOT TAIL-RECURSIVE:
       In auxX: `Set.union (auxY y) (auxX (cx + 1))`
         — both calls must complete before Set.union can run.
         auxX (cx+1) is NOT in tail position (Set.union waits for it).
       In auxY: `Set.add (C(cx, cy)) (auxY (cy + 1))`
         — Set.add cannot run until auxY (cy+1) returns.
         auxY (cy+1) is NOT in tail position. *)
    let coords (R(C(x, y), w, h)) =
        let rec auxX cx =
            if cx >= x + w then Set.empty
            else
                let rec auxY cy =
                    if cy >= y + h then Set.empty
                    else Set.add (C(cx, cy)) (auxY (cy + 1))
                Set.union (auxY y) (auxX (cx + 1))
        auxX x


    (* ── Question 1.3 ────────────────────────────────────────────── *)

    (* coordsAcc: tail-recursive version of coords using an accumulator.

       ACCUMULATOR PATTERN:
       A single helper `aux cx cy acc` replaces the two nested helpers.
       Instead of building sets bottom-up (Set.add waits for recursion),
       we carry `acc` and add to it BEFORE recursing.

       CONTROL FLOW:
       - If cx is past the right edge (cx >= x+w): we've visited all columns → return acc.
       - If cy is past the top edge (cy >= y+h): this column is done → move to next
         column by calling aux (cx+1) y acc  (reset cy back to y).
       - Otherwise: add C(cx,cy) to acc and advance to the next row in this column.

       Every recursive call `aux ... acc` is in tail position — the function
       does nothing after the call, so no stack frames accumulate. *)
    let coordsAcc (R(C(x, y), w, h)) =
        let rec aux cx cy acc =
            if cx >= x + w    then acc
            elif cy >= y + h  then aux (cx + 1) y acc
            else aux cx (cy + 1) (Set.add (C(cx, cy)) acc)
        aux x y Set.empty


    (* ── Question 1.4 ────────────────────────────────────────────── *)

    (* merge: given a list of rectangles, return the set of ALL coordinates
       covered by the valid rectangles.

       PIPELINE BREAKDOWN (|> threads the value left-to-right):

       List.filter valid rs
         Keep only rectangles where valid returns true.  Discards any with
         zero or negative dimensions.

       List.map coords
         Convert each valid rectangle into its Set<coord> using coords from Q1.2.

       List.fold Set.union Set.empty
         `List.fold f init xs` folds f over xs with initial accumulator `init`.
         Here f = Set.union, init = Set.empty.
         It computes: Set.union (... (Set.union Set.empty s1) ...) sn
         i.e. the union of all the coordinate sets.
         Set.union returns a new set containing all elements from both sets. *)
    let merge rs =
        rs
        |> List.filter valid
        |> List.map coords
        |> List.fold Set.union Set.empty


    (* ── Question 1.5 ────────────────────────────────────────────── *)

    (* foldRect: fold a function f over every coordinate in rectangle r.

       WHAT THIS DOES:
       It's like List.fold but for a rectangle — instead of folding over a list,
       we fold over the grid of (cx, cy) pairs that make up the rectangle.

       PARAMETER EXPLANATION:
       - f : 'a -> coord -> 'a   the combining function (takes acc + current coord)
       - acc : 'a                initial accumulator
       - r : rectangle           the rectangle to iterate over

       STRUCTURE:
       loopX cx accX: iterates columns left to right.
         - Base: cx past right edge → return accX.
         - Step: run loopY for the current column cx, then advance column.
       loopY cy accY: iterates rows bottom to top within column cx.
         - Base: cy past top edge → return accY (hand off to loopX).
         - Step: apply f to the current coord, advance row.

       TAIL RECURSION:
       Both loopX and loopY are tail-recursive:
         loopX calls loopX (cx+1) (loopY y accX) — loopY runs first, then
           the result is passed directly to loopX; no pending work after.
         loopY calls loopY (cy+1) (f accY ...) — f runs first, then tail call.

       coords2 shows usage: use Set.add as f and Set.empty as initial acc,
       which is exactly what coords does but now via foldRect. *)
    let foldRect f acc (R(C(x, y), w, h)) =
        let rec loopX cx accX =
            if cx >= x + w then accX
            else
                let rec loopY cy accY =
                    if cy >= y + h then accY
                    else loopY (cy + 1) (f accY (C(cx, cy)))
                loopX (cx + 1) (loopY y accX)
        loopX x acc

    let coords2 r = foldRect (fun acc c -> Set.add c acc) Set.empty r


    (* ═══════════════════════════════════════════════════════════════
       Question 2: Code Comprehension (25%)
       ═══════════════════════════════════════════════════════════════ *)

    let rec foo a b =
        match a, b with
        | a :: c, b :: d when a < b -> a :: foo c (b :: d)
        | a :: c, b :: d            -> b :: foo (a :: c) d
        | [], _ -> b
        | _, [] -> a

    let rec bar a =
        match List.length a with
        | 0 -> []                                              (* 1 *)
        | 1 -> a                                               (* 2 *)
        | l -> foo (bar a.[0..l / 2 - 1]) (bar a.[l / 2 .. l - 1]) (* 3 *)


    (* ── Question 2.1 ────────────────────────────────────────────── *)

    (*
      ── WHAT DOES foo DO? ────────────────────────────────────────────

      foo takes two lists `a` and `b` and processes them with four cases.
      Let's trace an example to figure it out.  Try foo [1;3] [2;4]:

        foo [1;3] [2;4]
        a=1, b=2: case 1 fires (1 < 2) → 1 :: foo [3] [2;4]
          foo [3] [2;4]
          a=3, b=2: case 2 fires (3 ≥ 2) → 2 :: foo [3] [4]
            foo [3] [4]
            a=3, b=4: case 1 fires (3 < 4) → 3 :: foo [] [4]
              foo [] [4]: case 3 fires (a=[]) → [4]
            → 3 :: [4] = [3;4]
          → 2 :: [3;4] = [2;3;4]
        → 1 :: [2;3;4] = [1;2;3;4]

      The result [1;2;3;4] is both lists MERGED and SORTED.
      foo picks the smaller of the two heads each time, so it interleaves
      two already-sorted lists into one sorted list.

      This ONLY works correctly if both input lists are already sorted.
      If either list is unsorted, foo produces a wrong result.

      APPROPRIATE NAME: foo → merge  (or mergeSorted)

      ── WHAT DOES bar DO? ────────────────────────────────────────────

      bar matches on List.length a:
        Case 0: empty list → return [].
        Case 1: singleton  → return as-is (already sorted).
        Case l: split in half, RECURSIVELY sort each half, then merge.

      This is MERGE SORT.
      `a.[0..l/2-1]` takes the first half (using F# list slice syntax —
      0-indexed, inclusive on both ends).
      `a.[l/2..l-1]` takes the second half.
      foo merges the two sorted halves.

      APPROPRIATE NAME: bar → mergeSort  (or sort)

      ── CONSTRAINTS ON foo's ARGUMENTS ──────────────────────────────

      Both lists a and b must already be SORTED IN ASCENDING ORDER.
      If they are not, foo's logic ("put the smaller head first") produces
      an incorrect result.  foo does NOT sort — it only merges.
    *)


    (* ── Question 2.2 ────────────────────────────────────────────── *)

    (*
      Q: What happens when you swap lines?

      ── Swap lines (1) and (2): `| 0 -> []` and `| 1 -> a` ───────────

      These are two base cases for specific integer values: length 0 and
      length 1.  Since 0 ≠ 1, only one can ever match a given input.
      Swapping them has NO EFFECT — the function behaves identically.

      ── Swap lines (1) and (3): `| 0 -> []` and `| l -> foo ...` ─────

      The wildcard `| l -> ...` moves ABOVE `| 0 -> []`.
      Wildcards match EVERY integer, including 0.
      Now an empty list hits the wildcard case:
        l = 0 → foo (bar a.[0..-1]) (bar a.[0..-1])
      In F#, a.[0..-1] is an empty slice = [].
      So bar [] → foo (bar []) (bar []) → foo (bar []) (bar []) → ...
      This recurses FOREVER → infinite recursion / stack overflow.

      ── Swap lines (2) and (3): `| 1 -> a` and `| l -> foo ...` ──────

      The wildcard `| l -> ...` moves above `| 1 -> a`.
      For a singleton list, l = 1, which hits the wildcard:
        l = 1 → foo (bar a.[0..0]) (bar a.[1..0])
      a.[0..0] = [x] (singleton), a.[1..0] = [] (empty).
      foo (bar [x]) (bar []) calls bar [x] again with the wildcard (l=1)
      → infinite recursion / stack overflow.
    *)


    (* ── Question 2.3 ────────────────────────────────────────────── *)

    (*
      Q: Why is `match List.length a with` not as performant as it could be?

      List.length a traverses the ENTIRE list from head to tail to count
      its elements.  This takes O(n) time — proportional to list length.

      bar calls List.length at the START of EVERY recursive invocation.
      This includes the base cases: for an empty list or a singleton, we
      pay O(n) just to discover that n=0 or n=1, when a simple pattern
      match on the list structure (`| [] -> ...` or `| [_] -> ...`) would
      tell us the same thing in O(1) time.

      FIX: use structural pattern matching for the base cases.  Only call
      List.length in the recursive case where we genuinely need the length
      to split the list:

      bar2 below handles [] and [_] with O(1) structural patterns, and only
      calls List.length in the `| _ ->` catch-all for lists of length ≥ 2. *)
    let rec bar2 a =
        match a with
        | []  -> []
        | [_] -> a
        | _ ->
            let l = List.length a
            foo (bar2 a.[0..l / 2 - 1]) (bar2 a.[l / 2 .. l - 1])


    (* ── Question 2.4 ────────────────────────────────────────────── *)

    (*
      Q: Prove that foo is NOT tail-recursive.

      DEFINITION: a function is tail-recursive if every recursive call is
      in "tail position" — i.e., it is the VERY LAST operation before the
      function returns.  Nothing may wait for the result of the recursive call.

      Evaluate foo [1;3] [2]:

        foo [1;3] [2]
        Case 1: a=1 < b=2 → 1 :: foo [3] [2]
                             ↑ THE PENDING CONS OPERATION
        1 :: foo [3] [2]
          foo [3] [2]
          Case 2: a=3 ≥ b=2 → 2 :: foo [3] []
          2 :: foo [3] []
            foo [3] []
            Case 4: b=[] → a = [3]
          → 2 :: [3] = [2;3]
        → 1 :: [2;3] = [1;2;3]

      In the step `1 :: foo [3] [2]`:
      The runtime computes `foo [3] [2]` recursively.  But AFTER that call
      returns, the operation `1 :: (result)` still has to happen.
      The `1 ::` cons is pending on the call stack.

      This means the recursive call `foo [3] [2]` is NOT in tail position.
      There are as many pending cons operations as output elements, so the
      stack depth grows proportionally — not tail-recursive.
    *)


    (* ── Question 2.5 ────────────────────────────────────────────── *)

    (* footail: tail-recursive merge using Continuation-Passing Style (CPS).

       THE IDEA:
       In foo, the pending work is the cons operation `a :: (recursive result)`.
       In CPS, instead of leaving that `a ::` on the call stack, we capture it
       in a continuation function `c` that is passed deeper into the recursion.

       PARAMETER `c`:
       `c` is a function of type `'a list -> 'a list`.
       It represents "everything that needs to be prepended to the final result."
       Initially `c = id` (identity): the result needs nothing prepended.

       CASES:
       Case 1 (x < y): instead of `x :: foo rest (y::rb)`, we recurse with a
         new continuation `fun res -> c (x :: res)`.  This continuation says:
         "when you eventually have the merged result `res`, prepend x to it and
         pass to the outer continuation c."
         The call `aux ra (y::rb) (...)` is in tail position. ✓

       Case 2 (x ≥ y): symmetrically prepend y via the continuation.

       Base cases: one list is empty → the result is the other list.
         We call c immediately: `c b` or `c a`, which runs all the pending
         prepend operations as a chain of function calls (in tail position). *)
    let footail a b =
        let rec aux a b c =
            match a, b with
            | x :: ra, y :: rb when x < y -> aux ra (y :: rb) (fun res -> c (x :: res))
            | x :: ra, y :: rb            -> aux (x :: ra) rb  (fun res -> c (y :: res))
            | [], _  -> c b
            | _,  [] -> c a
        aux a b id


    (* ═══════════════════════════════════════════════════════════════
       Question 3: Dining philosophers (25%)
       ═══════════════════════════════════════════════════════════════ *)

    (* ── Question 3.1 ────────────────────────────────────────────── *)

    (* type table = T of bool[]
       A table is a mutable array of booleans, one slot per philosopher.
       Slot i = true means philosopher i's LEFT fork is currently in use.
       Slot (i+1) % n = philosopher i's RIGHT fork (shared with philosopher i+1).

       setTable n: creates an array of n falses — all forks on the table.

       getLeftFork / getRightFork:
       - Check if the fork is already taken (arr.[idx] = true).
       - If taken: print an error message (shouldn't happen in correct usage).
       - If free: set arr.[idx] <- true (pick it up).
       The `<-` operator mutates the array in place.

       putLeftFork / putRightFork: reverse — check if fork is down (false = error),
       otherwise set to false (put it back).

       Index for right fork: (p + 1) % Array.length arr
       Philosopher p's right fork is the same as philosopher (p+1)'s left fork.
       The modulo wraps the last philosopher back to fork 0. *)
    type table = T of bool []

    let setTable size = T (Array.create size false)

    let getLeftFork (T arr) p =
        if arr.[p] then
            printfn "Philosopher %d tried to pick up their left fork, but it is already taken" p
        else
            arr.[p] <- true

    let getRightFork (T arr) p =
        let idx = (p + 1) % Array.length arr
        if arr.[idx] then
            printfn "Philosopher %d tried to pick up their right fork, but it is already taken" p
        else
            arr.[idx] <- true

    let putLeftFork (T arr) p =
        if not arr.[p] then
            printfn "Philosopher %d tried to put down their left fork, but it is already on the table" p
        else
            arr.[p] <- false

    let putRightFork (T arr) p =
        let idx = (p + 1) % Array.length arr
        if not arr.[idx] then
            printfn "Philosopher %d tried to put down their right fork, but it is already on the table" p
        else
            arr.[idx] <- false


    (* ── Question 3.2 ────────────────────────────────────────────── *)

    (* eat t p:   philosopher p picks up their LEFT fork, then RIGHT fork.
       think t p: philosopher p puts down LEFT fork, then RIGHT fork.

       canEat (T arr) p: returns true only if BOTH of p's forks are free.
       - arr.[p]            = p's left fork (free = false)
       - arr.[(p+1)%n]      = p's right fork (free = false)
       Both must be false (not taken) for p to be able to eat. *)
    let eat t p =
        getLeftFork t p
        getRightFork t p

    let think t p =
        putLeftFork t p
        putRightFork t p

    let canEat (T arr) p =
        let idx = (p + 1) % Array.length arr
        not arr.[p] && not arr.[idx]


    (* ── Question 3.3 ────────────────────────────────────────────── *)

    (* MAILBOXPROCESSOR — THE ACTOR MODEL:
       A MailboxProcessor is a background agent that processes messages
       one at a time from a queue (its "mailbox").  This serializes all
       table access: even if multiple philosophers request forks simultaneously,
       the mailbox processes one message at a time, preventing race conditions.

       type message:
       - Eat(p, rc): philosopher p wants to eat.  `rc` is a "reply channel"
         (AsyncReplyChannel<unit>).  The agent will call rc.Reply(()) when p
         is allowed to eat, unblocking the philosopher's thread.
       - Think(p): philosopher p is done eating (puts forks down).

       type philosopherTable = Phil of MailboxProcessor<message>
       Wraps the mailbox so callers don't touch it directly.

       inbox size mbox:
       This is the MESSAGE LOOP function.  It runs forever, processing one
       message at a time.

       `let! msg = mbox.Receive()` — asynchronously wait for the next message.
       `async { ... }` + `return!` — the loop is an async computation that
       keeps calling itself (tail-recursive via `return! messageLoop`).

       PENDING QUEUE:
       When a philosopher wants to eat but both forks aren't free, we add
       them to `pending` (a list of (philosopher, reply_channel) pairs).
       When someone calls Think(p), we free p's forks and then scan `pending`
       to see if any waiting philosopher can now eat.

       allowPending walks the pending list:
       - For each (pid, rc2) still waiting, check canEat.
       - If yes: grab forks, reply (unblock), remove from pending.
       - If no:  keep in pending.
       The result is the REMAINING pending list (those still unable to eat). *)
    type message =
        | Eat of int * AsyncReplyChannel<unit>
        | Think of int

    type philosopherTable = Phil of MailboxProcessor<message>

    let inbox size (mbox : MailboxProcessor<message>) =
        let t = setTable size

        let rec messageLoop (pending : (int * AsyncReplyChannel<unit>) list) =
            async {
                let! msg = mbox.Receive()
                match msg with
                | Eat(p, rc) ->
                    if canEat t p then
                        eat t p
                        rc.Reply(())
                        printfn "philosopher %d is eating" p
                        return! messageLoop pending
                    else
                        return! messageLoop ((p, rc) :: pending)
                | Think(p) ->
                    think t p
                    printfn "philosopher %d is thinking" p
                    let rec allowPending (ps : (int * AsyncReplyChannel<unit>) list) acc =
                        match ps with
                        | [] -> List.rev acc
                        | (pid, rc2) :: rest ->
                            if canEat t pid then
                                eat t pid
                                rc2.Reply(())
                                printfn "philosopher %d is eating" pid
                                allowPending rest acc
                            else
                                allowPending rest ((pid, rc2) :: acc)
                    return! messageLoop (allowPending pending [])
            }

        messageLoop []


    (* ── Question 3.4 ────────────────────────────────────────────── *)

    (* newTable size: starts the MailboxProcessor background agent.
       MailboxProcessor.Start(inbox size) creates the agent and begins
       running the message loop immediately.

       philEat (Phil mbox) p:
       Uses PostAndReply — posts an Eat message to the mailbox AND blocks
       the calling thread until the agent replies.  The reply is sent when
       p successfully picks up both forks.  The `fun rc -> Eat(p, rc)` lambda
       creates the message with the reply channel provided by PostAndReply.

       philThink (Phil mbox) p:
       Uses Post (fire-and-forget) — sends a Think message without waiting.
       The philosopher doesn't need confirmation that forks were put down. *)
    let newTable size =
        Phil (MailboxProcessor.Start(inbox size))

    let philEat (Phil mbox) p =
        mbox.PostAndReply(fun rc -> Eat(p, rc))

    let philThink (Phil mbox) p =
        mbox.Post(Think p)


    (* ── Question 3.5 ────────────────────────────────────────────── *)

    (* philosopher p meals time pt:
       An async computation that models one philosopher eating `meals` times.
       Each iteration: eat (blocks until forks available), sleep random time,
       think (put forks down), sleep random time.
       `Async.Sleep ms` suspends for ms milliseconds WITHOUT blocking the thread.
       `random.Next(time + 1)` gives a random int in [0, time].

       diningPhilosophers phils meals time:
       - Creates the shared table.
       - Creates an array of async computations, one per philosopher.
         `[| for p in 0..phils-1 -> ... |]` is an array comprehension.
       - `Async.Parallel` runs all philosophers concurrently.
       - `Async.RunSynchronously` blocks until ALL philosophers finish all meals.
       - `|> ignore` discards the unit[] result array. *)
    let random = System.Random(42)

    let philosopher p meals time pt =
        let rec loop remaining =
            async {
                if remaining > 0 then
                    philEat pt p
                    do! Async.Sleep(random.Next(time + 1))
                    philThink pt p
                    do! Async.Sleep(random.Next(time + 1))
                    return! loop (remaining - 1)
            }
        loop meals

    let diningPhilosophers phils meals time =
        let pt = newTable phils
        [| for p in 0..phils-1 -> philosopher p meals time pt |]
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
        printfn "Everyone is done eating"


    (* ═══════════════════════════════════════════════════════════════
       Question 4: The Towers of Hanoi (25%)
       ═══════════════════════════════════════════════════════════════ *)

    (* ── Question 4.1 ────────────────────────────────────────────── *)

    (* TYPE DEFINITIONS:
       peg   = Start | Middle | Goal   — the three pegs.
       disc  = D of int                — a disc with a size number.
       hanoi = H of Map<peg, disc list>  — the full game state.

       The Map maps each peg to its stack of discs (head = top of stack).
       So `Map.find Start m` gives the list of discs currently on Start,
       with the topmost disc first.

       newGame n: creates the initial configuration.
       - Start holds discs [D 1; D 2; …; D n] (1 on top, n on bottom).
       - Middle and Goal are empty.
       `[ for i in 1..numDiscs -> D i ]` is a list comprehension.

       isFinished: the game is done when Goal holds discs 1,2,…,n in order.
       - The first disc must be D 1 (smallest on top).
       - `List.pairwise ds` returns pairs of adjacent elements: [(d1,d2),(d2,d3),...].
       - For each pair (d1,d2), size d2 - size d1 = 1 means consecutive sizes. *)
    type peg   = Start | Middle | Goal
    type disc  = D of int
    type hanoi = H of Map<peg, disc list>

    let start  = Start
    let middle = Middle
    let goal   = Goal

    let size (D n) = n

    let newGame numDiscs =
        H (Map.ofList [
            (Start,  [ for i in 1..numDiscs -> D i ])
            (Middle, [])
            (Goal,   [])
        ])

    let isFinished (H m) =
        match Map.find Goal m with
        | [] -> false
        | ds ->
            List.head ds = D 1 &&
            List.pairwise ds |> List.forall (fun (d1, d2) -> size d2 - size d1 = 1)


    (* ── Question 4.2 ────────────────────────────────────────────── *)

    (* type error = Empty of peg | Invalid of peg * disc * disc
       Describes what can go wrong:
       - Empty p: tried to take a disc from peg p but it was empty.
       - Invalid(p, d, top): tried to place disc d on peg p where top disc
         `top` is already there and size d > size top (larger on smaller).

       take p (H m): remove the top disc from peg p.
       - `Map.find p m` looks up peg p in the map.
       - If the list is empty: return Error (Empty p).
       - If there is a disc d on top: return Ok (d, updated_hanoi).
         `Map.add p rest m` creates a new map with p's list set to `rest`.
         The overall type is `Result<disc * hanoi, error>`.

       place p d (H m): put disc d on top of peg p.
       - If the peg is non-empty and the top disc is SMALLER than d:
         placing d would violate the rules → return Error (Invalid ...).
       - Otherwise (peg is empty OR d fits): put d on top → return Ok new_hanoi.
         `Map.add p (d :: discs) m` prepends d to the peg's disc list. *)
    type error =
        | Empty of peg
        | Invalid of peg * disc * disc

    let take p (H m) =
        match Map.find p m with
        | []         -> Error (Empty p)
        | d :: rest  -> Ok (d, H (Map.add p rest m))

    let place p d (H m) =
        match Map.find p m with
        | top :: _ when size d > size top -> Error (Invalid (p, d, top))
        | discs                           -> Ok (H (Map.add p (d :: discs) m))


    (* ── Question 4.3 ────────────────────────────────────────────── *)

    (* THE HANOI STATE MONAD:

       type hanoiMonad<'a> = HM of (hanoi -> Result<'a * hanoi, error>)

       An HM is a RECIPE: given an input game state (hanoi), it either:
         - Succeeds: returns Ok (value, new_game_state)
         - Fails:    returns Error (some error)
       This lets us sequence game moves and automatically propagate errors.

       ret x:
         A recipe that succeeds without changing the game state, returning x.
         HM (fun h -> Ok (x, h))

       fail err:
         A recipe that immediately fails with `err`, ignoring the state.
         HM (fun _ -> Error err)

       bind f (HM a):
         Sequences two recipes:
         1. Run recipe `a` on the game state h.
         2. If it fails: propagate the error (short-circuit).
         3. If it succeeds with (x, h'): apply f to x to get a second recipe,
            then run that recipe on the updated state h'.
         This is "railway-oriented programming" — success stays on the track,
         errors jump off.

       >>= is infix bind.  >>>= sequences, ignoring the left result.
       evalHM (HM f) l = f (newGame l) — runs on a fresh l-disc game.

       take2 p / place2 p d:
       These BREAK the HM abstraction (use HM directly) to call take/place
       and convert their Result into the HM format.  We can't build them
       from ret/bind because those don't interact with take/place. *)
    type hanoiMonad<'a> = HM of (hanoi -> Result<'a * hanoi, error>)

    let ret x = HM (fun h -> (Ok (x, h)))
    let fail err = HM (fun _ -> Error err)
    let bind f (HM a)  =
        HM (fun h ->
            match a h with
            | Ok (x, h') ->
                let (HM g) = f x
                g h'
            | Error err -> Error err)

    let (>>=) a f = bind f a
    let (>>>=) a b = a >>= (fun _ -> b)

    let evalHM (HM f) l = f (newGame l)

    let take2 p =
        HM (fun h ->
            match take p h with
            | Ok (d, h') -> Ok (d, h')
            | Error e    -> Error e)

    let place2 p d =
        HM (fun h ->
            match place p d h with
            | Ok h'   -> Ok ((), h')
            | Error e -> Error e)


    (* ── Question 4.4 ────────────────────────────────────────────── *)

    (* HanoiBuilder: maps computation expression keywords to monadic operations.

       HOW COMPUTATION EXPRESSIONS DESUGAR:
       Inside `han { ... }`:
         `let! x = expr`  → this.Bind(expr, fun x -> <rest>)
         `do! expr`        → this.Bind(expr, fun () -> <rest>)
         `return x`        → this.Return(x)
         `return! expr`    → this.ReturnFrom(expr)
         `a; b`            → this.Combine(a, b)  (via Combine = >>>= )

       move src dst:
         Takes the top disc from peg `src` and places it on peg `dst`.
         `take2 src >>= place2 dst` — take gives a disc d, bind passes d to
         `place2 dst`, which places it.  If take or place fails, the error
         propagates automatically via bind.

       doMoves moves:
         `List.fold (fun acc (src,dst) -> acc >>>= move src dst) (ret ()) moves`
         Starts with `ret ()` (do nothing), and folds over the move list.
         Each fold step appends the next move with `>>>=` (sequence, ignore result).
         Result: one big recipe that performs all moves in order.

       solveHanoi size from via dest:
         Classic recursive Tower of Hanoi algorithm:
         - Move the top (size-1) discs from `from` to `via` (using `dest`).
         - Move the largest disc from `from` to `dest`.
         - Move the (size-1) discs from `via` to `dest` (using `from`).
         Returns a list of (src, dst) move pairs. *)
    type HanoiBuilder() =

        member this.Bind(f, x)    = bind x f
        member this.Return(x)     = ret x
        member this.ReturnFrom(x) = x
        member this.Combine(a, b) = a >>= (fun _ -> b)

    let han = new HanoiBuilder()

    let move src dst =
        take2 src >>= place2 dst

    let doMoves moves =
        List.fold (fun acc (src, dst) -> acc >>>= move src dst) (ret ()) moves

    let rec solveHanoi size from via dest =
        match size with
        | 0 -> []
        | x -> solveHanoi (x - 1) from dest via @
               (from, dest) ::
               solveHanoi (x - 1) via from dest


    (* ── Question 4.5 ────────────────────────────────────────────── *)

    (* JParsec parser combinators used here:

       pstring s : Parser<string>
         Matches exactly the string `s` in the input; fails otherwise.
         Returns the matched string.

       |>> f : Parser<'b>
         "Parser map" — if the parser succeeds with value x, transform it
         with f.  Here `fun _ -> Start` ignores the string and returns the
         peg value.  We use this to convert "start"/"middle"/"goal" strings
         into the corresponding peg discriminated union cases.

       <|> : Parser<'a>
         "Parser OR" — try the left parser first; if it fails, try the right.
         `pstart <|> pmiddle <|> pgoal` succeeds on any of the three peg names.

       .>>. : Parser<'a * 'b>
         "Tuple parser" — run both parsers in sequence, return a pair of results.

       >>. : Parser<'b>
         Run both in sequence, DISCARD the left result, return the right.
         `pstring "->" >>. parsePeg` consumes the "->" and returns the peg.

       .>> : Parser<'a>
         Run both in sequence, return the left result, DISCARD the right.
         `parseMove .>> pchar ';'` parses a move and then consumes the semicolon.

       pchar c : Parser<char>
         Matches exactly the single character c.

       many p : Parser<'a list>
         Runs p zero or more times, collecting results in a list.

       parseMoves parses e.g. "start->goal;middle->start;" into
       [(Start, Goal); (Middle, Start)]. *)
    let pstart  = pstring "start"  |>> fun _ -> Start
    let pmiddle = pstring "middle" |>> fun _ -> Middle
    let pgoal   = pstring "goal"   |>> fun _ -> Goal

    let parsePeg   = pstart <|> pmiddle <|> pgoal
    let parseMove  = parsePeg .>>. (pstring "->" >>. parsePeg) .>> pchar ';'
    let parseMoves = many parseMove
