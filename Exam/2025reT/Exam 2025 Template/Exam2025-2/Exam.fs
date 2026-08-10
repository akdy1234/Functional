module Exam2025_Template.Exam

    open JParsec.TextParser

    (* Question 1: Triangle Numbers (25%) *)

    (* Question 1.1
       Recursive but NOT tail-recursive, at most linear complexity.
       The trick to staying linear while still being non-tail-recursive (and without List.last/@,
       which are O(n) each and would make this O(n^2)) is: the inner helper returns a PAIR
       (current sum, list built by CONS so far). Consing is O(1), but because we're consing the
       newest triangle number onto the front each time, we end up with the list in DECREASING
       order - hence the hinted List.rev at the very end to flip it back to increasing order.
       This is recursive-but-not-tail-recursive because the tuple `(curSum, curSum :: prevList)`
       is built AFTER `aux (n - 1)` returns - there's work left to do once the recursive call
       comes back, so it isn't a tail call. *)
    let triangleNumber (n : int) : int list =
        let rec aux n =
            if n = 0 then (0, [])
            else
                let prevSum, prevList = aux (n - 1)
                let curSum = prevSum + n
                (curSum, curSum :: prevList)
        let _, decreasing = aux n
        List.rev decreasing

    (* Question 1.2
       Tail-recursive with an accumulator (not a continuation), linear complexity.
       We walk i upward from 1 to n, keeping a running sum and consing each new triangle number
       onto an accumulator (again in decreasing order), then reverse once at the end. The call
       `aux (i + 1) ...` is the entire result of its branch, so this compiles to a loop. *)
    let triangleNumberAcc (n : int) : int list =
        let rec aux i sum acc =
            if i > n then List.rev acc
            else
                let sum' = sum + i
                aux (i + 1) sum' (sum' :: acc)
        aux 1 0 []

    (* Question 1.3
       Non-recursive, using only List.sum and (nested) list comprehension, at most quadratic
       complexity. For each i from 1 to n we build the list [1..i] and sum it directly - no
       recursion needed, just two nested comprehensions, giving O(n^2) overall (n outer steps,
       each doing an O(n) sum), which satisfies "at most quadratic". *)
    let triangleNumberList (n : int) : int list =
        [ for i in 1 .. n -> List.sum [ for j in 1 .. i -> j ] ]

    (* Question 1.4
       sequence f i n builds [S1; ...; Sn] where S1 = i and S(k) = f k S(k-1) for k = 2..n.
       Implemented tail-recursively (accumulator carrying the running previous value and the
       list-so-far in reverse) to guarantee linear complexity, mirroring the pattern from 1.2. *)
    let sequence (f : int -> 'a -> 'a) (i : 'a) (n : int) : 'a list =
        let rec aux k prev acc =
            if k > n then List.rev acc
            else
                let next = f k prev
                aux (k + 1) next (next :: acc)
        if n = 0 then []
        else aux 2 i [ i ]

    (* Question 1.5
       Non-recursive: alternatingTriangle is just `sequence` fed the right step function. Where
       triangleNumber uses `f k acc = acc + k` for every k, alternatingTriangle instead subtracts
       when k is even and adds when k is odd - exactly the rule "even numbers are subtracted". *)
    let alternatingTriangle (n : int) : int list =
        let f k acc = if k % 2 = 0 then acc - k else acc + k
        sequence f 1 n

    (* Question 2: Code comprehension (25%) *)

    (* Question 2.1

     Q: What do the functions `foo`, `bar` and `baz` do? Focus on what they do rather than how they do it.
     A: `foo` turns a list into the list of all its overlapping consecutive pairs, e.g.
        foo [1;2;3;4] = [(1,2); (2,3); (3,4)] ("sliding window of size 2").
        `bar f pairs` checks whether the predicate `f` holds for every pair in a list of pairs (returning
        `true` vacuously on an empty list) - i.e. a "for-all" check specialised to a list of pairs.
        `baz` composes the two: it turns a list into its consecutive pairs and then checks that every
        consecutive pair sums to an odd number. Since a sum of two integers is odd exactly when one is
        even and the other is odd, `baz` checks whether the list strictly ALTERNATES in parity from one
        element to the next (odd, even, odd, even, ... or even, odd, even, odd, ...).

     Q: What would be appropriate names for functions `foo`, `bar` and `baz`?
     A: foo -> consecutivePairs (or slidingPairs / pairwise - this is exactly F#'s built-in List.pairwise)
        bar -> forallPairs (or allPairs - this is List.forall specialised to a list of pairs)
        baz -> alternatesInParity (or isAlternatingParity)

     Q: In the foo function, what would happen if you replaced the match [x] with [_]?
     A: Nothing observable would change. In the `[x]` branch the bound value `x` is never used (the branch
        just returns `[]` regardless of what the single element is), so renaming that pattern to the
        wildcard `_` has exactly the same behaviour for every input - the only difference is that the
        compiler no longer emits an "unused value x" warning, since `_` never introduces a binding at all.

     Q: What would happen if you swap lines (* 1 *) and (* 2 *) in the baz function?
     A: This changes behaviour, because the two patterns being swapped OVERLAP: `_ :: xs -> false` matches
        ANY non-empty list, with no guard, so once it sits above `(x, y) :: xs when f x y -> ...`, it
        intercepts every non-empty input before the guarded case ever gets a chance to run (F# tries match
        rules top to bottom). The guarded rule becomes dead/unreachable code (most compilers will warn
        "this rule will never be matched"), and `bar` degenerates into "return true only for []; false for
        anything else" - i.e. `baz` would only ever be true for lists of length 0 or 1 (which produce no
        pairs) and false for everything else, regardless of the actual parities involved.

     Q: What would happen if you swap lines (* 2 *) and (* 3 *) in the baz function?
     A: Nothing would change. `_ :: xs -> false` and `[] -> true` match completely DISJOINT inputs (one
        requires a non-empty list, the other requires an empty list) - they never compete for the same
        input, so their relative order is irrelevant. Unlike the (*1*)/(*2*) swap above, order only matters
        between rules whose patterns can overlap on the same input.
    *)

    (* Question 2.2
       A single, simple function using the standard library: List.pairwise already does exactly what `foo`
       does, and List.forall already does exactly what `bar` does, so baz2 is just their composition with
       the same predicate baz used. This is "clean and simple" precisely because it needs no recursion of
       its own at all. *)
    let baz2 (xs : int list) : bool =
        xs |> List.pairwise |> List.forall (fun (x, y) -> (x + y) % 2 <> 0)

    (* Question 2.3
       (See baz2 above - the question numbers the "create baz2" task as 2.3 and the "explain non-tail-
       recursion" task as 2.4; baz2 is implemented just above so it can use the same helper layout as the
       rest of the file.)

       Two lists of length > 5 for which baz gives different results:
         baz [1;2;3;4;5;6]  = true   (parities strictly alternate: odd,even,odd,even,odd,even)
         baz [1;2;3;4;5;7]  = false  (the last two elements, 5 and 7, are both odd, so that pair sums to
                                       an even number, breaking the alternation)
    *)
    let bazExampleTrue  = [1;2;3;4;5;6]   // baz bazExampleTrue  = true
    let bazExampleFalse = [1;2;3;4;5;7]   // baz bazExampleFalse = false

    (* Question 2.4

      Q: The function foo from Question 2.1 is not tail recursive. Explain why. To make a compelling
      argument you must evaluate a function call of the function, similarly to what is done in Chapter 1.4
      of HR, and reason about that evaluation. You need to make clear what aspects of the evaluation tell
      you that the function is not tail recursive.

      A: Consider evaluating `foo [1;2;3;4]`:

        foo [1;2;3;4]
        --> (x=1, y=2, xs=[3;4], second branch)
            (1, 2) :: foo [2;3;4]
        --> (1, 2) :: ((2, 3) :: foo [3;4])
        --> (1, 2) :: ((2, 3) :: ((3, 4) :: foo [4]))
        --> (1, 2) :: ((2, 3) :: ((3, 4) :: []))          (foo [4] matches the [x] base case -> [])
        --> (1, 2) :: ((2, 3) :: [(3, 4)])
        --> (1, 2) :: [(2, 3); (3, 4)]
        --> [(1, 2); (2, 3); (3, 4)]

      At every step the pending operation is "(x, y) :: (foo ...)" - the `::` wraps AROUND the recursive
      call rather than being the entire result of the branch. The evaluator has to keep track of every one
      of those pending cons operations ("still owe a (1,2)::", "still owe a (2,3)::", "still owe a
      (3,4)::") while it dives deeper into the recursive calls, and can only actually perform them, from
      the inside out, once it hits the base case. That growing chain of deferred operations is exactly what
      a call stack tracks, and it is exactly what a tail call does NOT need, since a tail call's result IS
      the function's result with nothing left pending. Because foo's recursive call is always used as an
      argument to `::` rather than being returned directly, foo is not tail recursive.
    *)

    (* Question 2.5
       Continuation-passing version of foo. Instead of doing "(x,y) :: (...)" after the recursive call
       returns, we pass along a continuation `c` describing everything still owed to the final answer, and
       grow that continuation on each step instead of growing the call stack - so the recursive call becomes
       the very last thing done in every branch. *)
    let fooTail (a : 'a list) : ('a * 'a) list =
        let rec aux a c =
            match a with
            | [] -> c []
            | [ _ ] -> c []
            | x :: y :: xs -> aux (y :: xs) (fun acc -> c ((x, y) :: acc))
        aux a id

    (* Question 3: Stores and locks (25%) *)

    (* Question 3.1 *)
    type 'a store = {
        data : 'a
        owner : int option
    }

    let newStore (v : 'a) : 'a store = { data = v; owner = None }

    let lock (pid : int) (st : 'a store) : 'a store =
        match st.owner with
        | None -> { st with owner = Some pid }
        | Some pid2 ->
            printfn "process %d tried to lock but the lock is held by %d" pid pid2
            st

    let unlock (pid : int) (st : 'a store) : 'a store =
        match st.owner with
        | Some pid2 when pid2 = pid -> { st with owner = None }
        | Some pid2 ->
            printfn "process %d tried to unlock but the lock is held by %d" pid pid2
            st
        | None ->
            printfn "process %d tried to unlock but no one holds the lock" pid
            st

    (* Question 3.2 *)
    let read (st : 'a store) : 'a = st.data

    let write (pid : int) (value : 'a) (st : 'a store) : 'a store =
        match st.owner with
        | Some pid2 when pid2 <> pid ->
            printfn "process %d tried to write %A but the lock is held by %d" pid value pid2
            st
        | _ -> { st with data = value }

    let isLocked (st : 'a store) : bool = Option.isSome st.owner

    (* Question 3.3
       The mailbox loop. It only ever touches the store through lock/unlock/read/write/isLocked from Q3.1/
       Q3.2 (never through the { data = ...; owner = ... } record fields directly), so it doesn't break the
       store's abstraction.
       On Unlock, we only try to hand the lock to the next pending process if the store is ACTUALLY unlocked
       afterwards (i.e. `unlock` succeeded) - if some other pid mistakenly calls Unlock while it doesn't hold
       the lock, `unlock` already prints its own error and returns the store untouched, so nothing should be
       handed off in that case. *)
    type 'a message =
        | Lock of int * AsyncReplyChannel<unit>
        | Unlock of int
        | Read of AsyncReplyChannel<'a>
        | Write of int * 'a

    type 'a storeServer = Store of MailboxProcessor<'a message>

    let inbox x (mbox : MailboxProcessor<'a message>) =

        let rec messageLoop (st : 'a store) (pending : (int * AsyncReplyChannel<unit>) list) =
            async {
                let! msg = mbox.Receive()
                match msg with
                | Lock(pid, rc) ->
                    if isLocked st then
                        return! messageLoop st (pending @ [ (pid, rc) ])
                    else
                        let st' = lock pid st
                        rc.Reply(())
                        return! messageLoop st' pending
                | Unlock pid ->
                    let st' = unlock pid st
                    if isLocked st' then
                        // unlock did not actually succeed (wrong pid) - nothing to hand off
                        return! messageLoop st' pending
                    else
                        match pending with
                        | (pid2, rc2) :: rest ->
                            let st'' = lock pid2 st'
                            rc2.Reply(())
                            return! messageLoop st'' rest
                        | [] ->
                            return! messageLoop st' pending
                | Read rc ->
                    rc.Reply(read st)
                    return! messageLoop st pending
                | Write(pid, value) ->
                    let st' = write pid value st
                    return! messageLoop st' pending
            }

        messageLoop (newStore x) []

    (* Question 3.4 *)
    let createStore (v : 'a) : 'a storeServer =
        Store (MailboxProcessor.Start(inbox v))

    let storeLock (pid : int) (Store mbox : 'a storeServer) : unit =
        mbox.PostAndReply(fun rc -> Lock(pid, rc))

    let storeUnlock (pid : int) (Store mbox : 'a storeServer) : unit =
        mbox.Post(Unlock pid)

    let storeRead (Store mbox : 'a storeServer) : 'a =
        mbox.PostAndReply(fun rc -> Read rc)

    let storeWrite (pid : int) (value : 'a) (Store mbox : 'a storeServer) : unit =
        mbox.Post(Write(pid, value))

    (* Question 3.5
       inc: lock, read, write (current+1), unlock - in that order, so the read-modify-write is protected by
       the lock for the whole duration.
       countTo: spawn `size` async workers (one per pid, running `inc`), run them all in parallel via
       Async.Parallel, wait for all of them with Async.RunSynchronously, then read the final value. Because
       every `inc` fully holds the lock across its read+write, the increments cannot interleave, so the
       final count should always be exactly `size`. *)
    let inc (pid : int) (st : int storeServer) : unit =
        storeLock pid st
        let current = storeRead st
        storeWrite pid (current + 1) st
        storeUnlock pid st

    let countTo (size : int) : int =
        let st = createStore 0
        [ 1 .. size ]
        |> List.map (fun pid -> async { inc pid st })
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
        storeRead st

    (* Question 4: Tic-tac-toe (25%) *)

    (* Question 4.1
       row/col are simple single-case wrapper types around an int, which gives us distinct, nameable values
       (topRow, midRow, ...) while still being comparable/usable as Map keys.
       The board itself is represented as a Map from (row * col) to player: only squares that actually have
       a tile on them appear in the map at all, which makes "is this square occupied, and by whom" a single
       Map.tryFind, and makes newGame trivially Map.empty. *)
    type row = Row of int
    type col = Col of int

    let topRow = Row 0
    let midRow = Row 1
    let botRow = Row 2

    let leftCol = Col 0
    let midCol = Col 1
    let rightCol = Col 2

    type player = X | O

    type board = Map<row * col, player>

    let newGame : board = Map.empty

    (* Question 4.2
       NOTE: the exam text names this function `doNextMove` in the prose but every single worked example
       calls it `doMove` - almost certainly a wording slip in the exam sheet rather than two different
       functions. I've implemented it as `doMove` to match the examples; rename to `doNextMove` if your
       actual downloaded template stub uses that name instead - the body is identical either way. *)
    type error =
        | PlayerTurn of player
        | SquareTaken of row * col * player

    type state =
        | Running of player * board
        | Win of player * board
        | Draw of board

    let private otherPlayer p = match p with | X -> O | O -> X

    let private winningLines : (row * col) list list =
        [ // the three rows
          [ (topRow, leftCol); (topRow, midCol); (topRow, rightCol) ]
          [ (midRow, leftCol); (midRow, midCol); (midRow, rightCol) ]
          [ (botRow, leftCol); (botRow, midCol); (botRow, rightCol) ]
          // the three columns
          [ (topRow, leftCol); (midRow, leftCol); (botRow, leftCol) ]
          [ (topRow, midCol); (midRow, midCol); (botRow, midCol) ]
          [ (topRow, rightCol); (midRow, rightCol); (botRow, rightCol) ]
          // the two diagonals
          [ (topRow, leftCol); (midRow, midCol); (botRow, rightCol) ]
          [ (topRow, rightCol); (midRow, midCol); (botRow, leftCol) ] ]

    let private isWinFor (p : player) (b : board) : bool =
        winningLines
        |> List.exists (fun line -> line |> List.forall (fun sq -> Map.tryFind sq b = Some p))

    let private isFull (b : board) : bool = Map.count b = 9

    let doMove (p : player) (r : row) (c : col) (st : state) : Result<state, error> =
        match st with
        | Win _ | Draw _ ->
            // Game is already over - the spec says calling doMove again just hands the finished state back.
            Ok st
        | Running(p', b) ->
            if p <> p' then
                Error (PlayerTurn p')
            else
                match Map.tryFind (r, c) b with
                | Some p'' -> Error (SquareTaken(r, c, p''))
                | None ->
                    let b' = Map.add (r, c) p b
                    if isWinFor p b' then Ok (Win(p, b'))
                    elif isFull b' then Ok (Draw b')
                    else Ok (Running(otherPlayer p, b'))

    (* Question 4.3
       Same shape as the Q4.3 chess-monad exercise from the other exam: wrap doMove/gameOver/getBoard so
       they fit the ticTacToeMonad's function-from-state shape. This is exactly the place the question says
       we're allowed (and need) to break the monad's abstraction, since something has to bridge the plain
       Result<state,error> world of Q4.2 into the monad's Result<'a*state,error> shape.
       NOTE on evalTTT: the exam sheet's snippet is `let evalTTT (TTT f) l = f (Running(empty, X))`, but (a)
       every single usage example in the exam calls `evalTTT` with exactly ONE argument (e.g. `evalTTT
       gameOver`), and (b) `Running(empty, X)` has its arguments in the wrong order for `state`'s declared
       shape `Running of player * board` (every Q4.2 example consistently writes `Running(X, empty)`, player
       first). Both are most likely PDF/OCR artefacts (or, for the stray `l`, possibly copy-pasted from the
       N-Queens exam's very similar `evalCM (CM f) N = f (empty N)`, where N meaningfully mattered as a
       board size - there's no equivalent size parameter for a fixed 3x3 tic-tac-toe board). I've implemented
       it the way it's actually used everywhere: one argument, starting from `Running(X, newGame)`. Check
       your real template stub and adjust the parameter list/argument order if it differs. *)
    type ticTacToeMonad<'a> = TTT of (state -> Result<'a * state, error>)

    let ret x = TTT (fun h -> (Ok (x, h)))
    let fail err = TTT (fun _ -> Error err)
    let bind f (TTT a) =
        TTT (fun h ->
            match a h with
            | Ok (x, h') ->
                let (TTT g) = f x
                g h'
            | Error err -> Error err)

    let (>>=) a f = bind f a
    let (>>>=) a b = a >>= (fun _ -> b)

    let evalTTT (TTT f) = f (Running(X, newGame))

    let doMove2 (p : player) (r : row) (c : col) : ticTacToeMonad<unit> =
        TTT (fun st ->
            match doMove p r c st with
            | Ok st' -> Ok ((), st')
            | Error err -> Error err)

    let gameOver : ticTacToeMonad<bool> =
        TTT (fun st ->
            match st with
            | Running _ -> Ok (false, st)
            | Win _ | Draw _ -> Ok (true, st))

    let getBoard : ticTacToeMonad<board> =
        TTT (fun st ->
            match st with
            | Running(_, b) -> Ok (b, st)
            | Win(_, b) -> Ok (b, st)
            | Draw b -> Ok (b, st))

    (* Question 4.4
       Uses the given `ttt { }` computation expression. For each move: if the game is already over, stop and
       return the board as-is (this is what makes the trailing "will never be played" move in the example
       get skipped); otherwise play the move and recurse on the rest. Never pattern-matches on
       ticTacToeMonad itself - only ever goes through doMove2/gameOver/getBoard and the computation
       expression, respecting the abstraction boundary as required. *)
    type TicTacToeBuilder() =
        member this.Bind(f, x) = bind x f
        member this.Return(x) = ret x
        member this.ReturnFrom(x) = x
        member this.Combine(a, b) = a >>= (fun _ -> b)

    let ttt = new TicTacToeBuilder()

    let rec playGame (moves : (player * row * col) list) : ticTacToeMonad<board> =
        ttt {
            match moves with
            | [] -> return! getBoard
            | (p, r, c) :: rest ->
                let! over = gameOver
                if over then
                    return! getBoard
                else
                    do! doMove2 p r c
                    return! playGame rest
        }

    (* Question 4.5
       NOTE: as with the previous exam's parser question, the exact combinator names below (pstring, choice,
       (.>>.), (.>>), (>>.), (|>>), many, spaces) are my best-guess mapping onto standard parser-combinator
       naming conventions - confirm the precise names/signatures in your actual JParsec module before the
       real exam, the shape of the solution (parse a keyword, map it to a value, glue pieces together with
       sequencing combinators) is what matters and should transfer directly once the names are right. *)
    let parsePlayer : Parser<player> =
        (pstring "X" |>> fun _ -> X) <|> (pstring "O" |>> fun _ -> O)

    let parseRow : Parser<row> =
        choice [ pstring "topRow" |>> fun _ -> topRow
                 pstring "midRow" |>> fun _ -> midRow
                 pstring "botRow" |>> fun _ -> botRow ]

    let parseCol : Parser<col> =
        choice [ pstring "leftCol" |>> fun _ -> leftCol
                 pstring "midCol" |>> fun _ -> midCol
                 pstring "rightCol" |>> fun _ -> rightCol ]

    let parseMove : Parser<player * row * col> =
        pstring "Player " >>. parsePlayer
        .>> pstring " places a tile on row " .>>. parseRow
        .>> pstring " and column " .>>. parseCol
        |>> fun ((p, r), c) -> (p, r, c)

    //let parseMoves : Parser<(player * row * col) list> =
    //    many (parseMove .>> spaces)

    