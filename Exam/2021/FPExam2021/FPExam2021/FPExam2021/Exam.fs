module Exam2021
(* If you are importing this into F# interactive then comment out
   the line above and remove the comment for the line bellow.

   Do note that the project will not compile if you do this, but
   it does allow you to work in interactive mode and you can just remove the '='
   to make the project compile again.

   You will also need to load JParsec.fs. Do this by typing
   #load "JParsec.fs"
   in the interactive environment. You may need the entire path.

   Do not remove the module declaration (even though that does work) because you may inadvertantly
   introduce indentation errors in your code that may be hard to find if you want
   to switch back to project mode.

   Alternative, keep the module declaration as is, but load ExamInteractive.fsx into the interactive environment
   *)
(*
 module Exam2021 =
 *)

(* 1: Dungeon crawler *)

    type direction = North | East | South | West
    type coord     = C of int * int

(* Question 1.1 *)

    // move d c — take one step in direction d from coordinate c.
    //
    // We pattern-match on the direction and adjust x or y by 1.
    //   North → y + 1 (up)   South → y - 1 (down)
    //   East  → x + 1 (right) West  → x - 1 (left)
    let move (d : direction) (C(x, y) : coord) : coord =
        match d with
        | North -> C(x, y + 1)
        | South -> C(x, y - 1)
        | East  -> C(x + 1, y)
        | West  -> C(x - 1, y)

    // turnRight — rotate 90° clockwise: N→E→S→W→N
    let turnRight = function
        | North -> East
        | East  -> South
        | South -> West
        | West  -> North

    // turnLeft — rotate 90° counterclockwise: N→W→S→E→N
    let turnLeft = function
        | North -> West
        | West  -> South
        | South -> East
        | East  -> North

(* Question 1.2 *)

    type position = P of (coord * direction)
    type move     = TurnLeft | TurnRight | Forward of int

    // step mv pos — apply one move to a position.
    //
    //   TurnLeft / TurnRight  → update direction only, coord unchanged
    //   Forward n             → move n steps in current direction via go n c
    //     go is a private tail-recursive helper that applies `move d` n times.
    let step (mv : move) (P(c, d) : position) : position =
        match mv with
        | TurnLeft  -> P(c, turnLeft d)
        | TurnRight -> P(c, turnRight d)
        | Forward n ->
            let rec go i cur =
                if i = 0 then cur
                else go (i - 1) (move d cur)
            P(go n c, d)

(* Question 1.3 *)

    // walk — apply a list of moves to a starting position, return final position.
    //
    // List.fold threads the position through each move instruction left-to-right.
    // No explicit recursion needed — `step` handles each move, fold chains them.
    let walk (moves : move list) (pos : position) : position =
        List.fold (fun p m -> step m p) pos moves

(* Question 1.4 *)

    // path — return the list of coordinates visited, one entry per move instruction.
    //
    // CONVENTION: each iteration records the coord BEFORE applying the move.
    // The final coord (after all moves) is added at the base case [].
    //
    // Example: path [Forward 2; TurnRight] (P(C(0,0), East))
    //   c=C(0,0) :: path [TurnRight] (P(C(2,0), East))
    //   c=C(2,0) :: path []          (P(C(2,0), South))
    //   = [C(0,0); C(2,0); C(2,0)]
    //
    // WHY NOT TAIL-RECURSIVE (see Q1.6):
    //   The recursive call `path rest (step mv pos)` is nested inside `c :: (...)`.
    //   The cons operator `::` cannot complete until the recursive call returns,
    //   so each call frame stays alive on the stack. Stack depth = list length.
    let rec path (moves : move list) (pos : position) : coord list =
        let (P(c, _)) = pos
        match moves with
        | []         -> [c]
        | mv :: rest -> c :: path rest (step mv pos)

(* Question 1.5 *)

    // path2 — like path but records EVERY individual step within a Forward n move.
    //
    // path  records one coord per move instruction (coarse granularity).
    // path2 records every footstep — for Forward n, all n intermediate coords are listed.
    //
    // IMPLEMENTATION:
    //   TurnLeft / TurnRight → record current coord, recurse with new direction
    //   Forward 0            → zero steps, skip (no new coord)
    //   Forward n            → record c, then recurse with Forward (n-1) after advancing one step
    //
    // Trace: path2 [Forward 2; TurnRight] (P(C(0,0), East))
    //   c=C(0,0) :: path2 [Forward 1] (P(C(1,0), East))
    //   C(0,0) :: C(1,0) :: path2 [Forward 0] (P(C(2,0), East))
    //   C(0,0) :: C(1,0) :: path2 [TurnRight] (P(C(2,0), East))
    //   C(0,0) :: C(1,0) :: C(2,0) :: path2 [] (P(C(2,0), South))
    //   = [C(0,0); C(1,0); C(2,0); C(2,0)]
    let rec path2 (moves : move list) (pos : position) : coord list =
        let (P(c, d)) = pos
        match moves with
        | []                -> [c]
        | TurnLeft  :: rest -> c :: path2 rest (P(c, turnLeft d))
        | TurnRight :: rest -> c :: path2 rest (P(c, turnRight d))
        | Forward 0 :: rest -> path2 rest pos
        | Forward n :: rest ->
            let c' = move d c
            c :: path2 (Forward (n - 1) :: rest) (P(c', d))

(* Question 1.6 *)

(* Q: Your solution for `path` is not tail recursive. Why? To make a compelling
      argument you should evaluate a function call of the function, similarly to
      what is done in Chapter 1.4 of HR, and reason about that evaluation.
      You need to make clear what aspects of the evaluation tell you that the
      function is not tail recursive. Keep in mind that all steps in an evaluation
      chain must evaluate to the same value
      (```(5 + 4) * 3 --> 9 * 3 --> 27```, for instance).

   A: Evaluate path [Forward 1; TurnRight] (P(C(0,0), East)):

        path [Forward 1; TurnRight] (P(C(0,0), East))
      = C(0,0) :: path [TurnRight] (P(C(1,0), East))
                  ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                  recursive call is INSIDE the cons operator
      = C(0,0) :: (C(1,0) :: path [] (P(C(1,0), South)))
      = C(0,0) :: (C(1,0) :: [C(1,0)])
      = C(0,0) :: [C(1,0); C(1,0)]
      = [C(0,0); C(1,0); C(1,0)]

      In the first step, the recursive call `path [TurnRight] (P(C(1,0), East))`
      is the ARGUMENT to `::`.  After it returns, we still need to prepend C(0,0).
      That pending prepend is held as a stack frame.  For a list of n moves, n
      frames pile up simultaneously — this is the definition of non-tail-recursive.
      A tail call would be one where the recursive call IS the final result,
      with nothing else to compute afterwards.
*)

    // path3 — tail-recursive version of path using an accumulator.
    //
    // We reverse the problem: collect coords in `acc` (reversed order),
    // then List.rev at the base case to restore left-to-right order.
    //
    // The call `aux rest (step mv pos) (c :: acc)` is the LAST thing done
    // in the recursive branch — tail position. ✓
    let path3 (moves : move list) (pos : position) : coord list =
        let rec aux moves pos acc =
            let (P(c, _)) = pos
            match moves with
            | []         -> List.rev (c :: acc)
            | mv :: rest -> aux rest (step mv pos) (c :: acc)
        aux moves pos []


(* 2: Code Comprehension *)

    let foo f =
        let mutable m = Map.empty
        let aux x =
            match Map.tryFind x m with
            | Some y when Map.containsKey x m -> y
            | None   ->
            m <- Map.add x (f x) m; f x
        aux

    let rec bar x =
      match x with
      | 0 -> 0
      | 1 -> 1
      | y -> baz (y - 1) + baz (y - 2)

    and baz = foo bar

(* Question 2.1 *)

    (*

    Q: What are the types of functions foo, bar, and baz?

    A: foo : ('a -> 'b) -> 'a -> 'b
         foo takes a function f and returns a new function (aux) of the same type.
         The mutable map m : Map<'a,'b> is hidden in the closure.

       bar : int -> int
         bar pattern-matches on int (0, 1, y) and returns int (0, 1, or sum).

       baz : int -> int
         baz = foo bar, so baz has the same type as bar: int -> int.


    Q: What do functions foo and baz do (skip bar)?
       Focus on what they do rather than how they do it.

    A: foo — MEMOIZES a function.
         Given a function f : 'a -> 'b, foo returns a new function that behaves
         identically to f but caches results in a persistent Map.
         The first time aux is called with x, it computes f x and stores it.
         Subsequent calls with the same x return the cached result immediately.

       baz — Memoized Fibonacci numbers.
         baz n returns the n-th Fibonacci number.
         bar is the "raw" recursive Fibonacci, and baz = foo bar wraps it in
         the memo cache.  This makes repeated calls to baz fast because computed
         values are never recalculated.


    The function foo uses a mutable variable.

    Q: What function does it serve (why is it there)?

    A: The mutable Map `m` is the memo cache / lookup table.
       It persists across multiple calls to `aux` because it is captured in
       `aux`'s closure.  Without mutation, we could not update the cache after
       each computation.


    Q: What would happen if you removed the mutable keyword from the line
       let mutable m = Map.empty? Would the function foo still work?
       If yes, why; if no, why not?

    A: No, it would NOT compile.
       The expression `m <- Map.add x (f x) m` uses `<-`, the mutation operator.
       In F# you can only use `<-` on a `mutable` binding.
       Without the `mutable` keyword, `m` is immutable and `<-` is a type error.
       The compiler would report: "This value is not mutable."


    Q: What would be appropriate names for functions foo, bar, and baz?

    A: foo  → memoize       (wraps a function with a memo cache)
       bar  → fibRaw        (raw / uncached Fibonacci)
       baz  → fib           (memoized Fibonacci)

    *)

(* Question 2.2 *)

    (*
    The code includes the keyword "and".

    Q: What function does this keyword serve in general
       (why would you use "and" when writing any program)?

    A: `and` allows MUTUAL RECURSION between two or more `let` bindings.
       Normally, each `let` can only refer to things defined before it.
       `let rec f = ... and g = ...` allows f to call g AND g to call f —
       both names are in scope for both bodies simultaneously.


    Q: What would happen if you removed it from this particular program and
       replaced it with a standard "let"
       (change the line "and baz = foo bar" to "let baz = foo bar")?

    A: Changing `and baz = foo bar` to `let baz = foo bar` would cause a
       COMPILE ERROR.
       The reason: `bar` is defined with `let rec`, and `bar`'s body references
       `baz` (via `baz (y-1) + baz (y-2)`).  If `baz` is defined separately
       with a plain `let baz = foo bar`, the compiler processes `bar` first.
       At that point `baz` does not yet exist in scope, so `bar`'s reference to
       `baz` is unresolved — type error / unbound identifier.
       The `and` keyword solves this by bringing both `bar` and `baz` into scope
       at the same time.

    *)

(* Question 2.3 *)

    (*
    The function foo generates a warning during compilation:
    "Warning: Incomplete pattern matches on this expression.".

    Q: Why does this happen, and where?

    A: Inside `aux`, the match has two arms:
         | Some y when Map.containsKey x m -> y
         | None   -> ...

       The first arm uses a `when` guard.  Even though `Map.tryFind x m`
       returns either `Some y` or `None`, the guard `when Map.containsKey x m`
       on the `Some y` arm might theoretically fail (the compiler doesn't know
       it's logically impossible for tryFind to return `Some y` when containsKey
       is false).  The compiler sees a potentially-failing guarded arm followed by
       a `None` arm — and notices that `Some y` when NOT containsKey is missing.
       It warns about this incomplete coverage.


    Q: For these particular three functions will this incomplete pattern match
       ever cause problems for any possible execution of baz? If yes, why;
       if no, why not.

    A: NO, it will never cause problems.
       Map.tryFind returns `Some y` if and only if the key exists in the map.
       Therefore `Map.tryFind x m = Some y` IMPLIES `Map.containsKey x m = true`.
       The "missing" case (Some y when containsKey is false) is logically
       impossible.  The warning is a false alarm from the compiler's perspective,
       but the runtime will never reach the missing branch.


    Q: The function foo has two redundant computations and is hence not as
       efficient as it could be. What are these two computations and why
       are they redundant?

    A: 1. `Map.containsKey x m` is redundant.
          `Map.tryFind x m` already tells us whether x is in the map:
          Some y means YES, None means NO.  Calling containsKey a second time
          duplicates work that tryFind already did.

       2. In the None branch: `m <- Map.add x (f x) m; f x`
          `f x` is called TWICE — once to store in the map, once to return.
          This double-evaluation wastes time (and may have side effects if f is impure).

    *)

    // foo2 — fixed memoizer that eliminates both redundant computations.
    //
    // Changes:
    //   - Removed `when Map.containsKey x m` guard (tryFind already covers it).
    //   - Compute f x ONCE, store in `v`, then store AND return `v`.
    let foo2 f =
        let mutable m = Map.empty
        let aux x =
            match Map.tryFind x m with
            | Some y -> y
            | None   ->
                let v = f x
                m <- Map.add x v m
                v
        aux

(* Question 2.4 *)

    let rec barbaz x =
        let baz = foo barbaz
        match x with
        | 0 -> 0
        | 1 -> 1
        | y -> baz (y - 1) + baz (y - 2)

    (*

    Q: Without explicitly timing the execution times, compare the execution
       times of baz and barbaz. One is slower than the other.
       Why? You do not have to give exact times, just spot which one is
       slower and explain why.

    A: barbaz is MUCH SLOWER than baz.

       In barbaz, the line `let baz = foo barbaz` is executed on EVERY call to
       barbaz.  Each call creates a BRAND NEW memoization closure with an EMPTY
       cache.  So when barbaz 10 calls baz 9, that creates a new cache; baz 9
       calls baz 8 with yet another empty cache, and so on.  There is NO
       sharing of cached results between different recursive calls — effectively
       barbaz behaves like naive exponential-time recursion.

       In contrast, `baz = foo bar` is evaluated ONCE at module load time.
       The same memoization map `m` is reused across ALL calls to baz.
       Once baz 5 is computed, any future call to baz 5 returns instantly from cache.
       baz runs in O(n) for computing fib(n) the first time; subsequent calls O(1).

    *)

(* Question 2.5 *)

    // bazSeq — infinite sequence of Fibonacci numbers starting from index 0.
    //
    // Seq.initInfinite f generates [f 0; f 1; f 2; ...] lazily.
    // Since baz n = the n-th Fibonacci number, Seq.initInfinite baz gives
    // the Fibonacci sequence.
    // Thanks to baz's memoization, each element is computed in amortised O(1).
    let bazSeq () : int seq = Seq.initInfinite baz


(* 3: Guess the next sequence element *)

    // This section implements the LOOK-AND-SAY sequence:
    //   1 → 11 → 21 → 1211 → 111221 → 312211 → ...
    // Rule: describe what you see in the previous element.
    //   "1"    → "one 1"            = "11"
    //   "11"   → "two 1s"           = "21"
    //   "21"   → "one 2, one 1"     = "1211"
    //   "1211" → "one 1, one 2, two 1s" = "111221"

(* Question 3.1 *)

    // An element is a list of single-digit integers.
    // e.g., "1211" is represented as [1; 2; 1; 1].
    type element = int list

(* Question 3.2 *)

    // elToString — convert element to its digit string representation.
    // [1;2;1;1] → "1211"
    let elToString (e : element) : string =
        e |> List.map string |> String.concat ""

    // elFromString — parse a digit string into an element.
    // "1211" → [1;2;1;1]
    // Each character is converted via `int c - int '0'` (ASCII arithmetic).
    let elFromString (s : string) : element =
        [ for c in s -> int c - int '0' ]

(* Question 3.3 *)

    // compress — run-length encode an element.
    //
    // Groups consecutive equal digits and returns (count, digit) pairs.
    // e.g., [1;1;2;1] → [(2,1); (1,2); (1,1)]
    //
    // IMPLEMENTATION:
    //   For the head digit x, count how many following digits also equal x
    //   (via List.span), then recurse on the remainder.
    //   Count = 1 (for x itself) + length of the equal-prefix.
    let compress (e : element) : (int * int) list =
        let rec aux = function
            | [] -> []
            | x :: xs ->
                let same = xs |> List.takeWhile (fun y -> y = x)
                let rest = xs |> List.skipWhile (fun y -> y = x)
                (List.length same + 1, x) :: aux rest
        aux e

    // nextElement — produce the next element in the look-and-say sequence.
    //
    // STEPS:
    //   1. compress e  → [(count1, digit1); (count2, digit2); ...]
    //   2. Flatten each pair to [count; digit]
    //   3. Concatenate all pairs into one list
    //
    // Example: nextElement [1;2;1;1]
    //   compress = [(1,1);(1,2);(2,1)]
    //   flatten  = [1;1; 1;2; 2;1] = [1;1;1;2;2;1] = "111221" ✓
    let nextElement (e : element) : element =
        compress e |> List.collect (fun (count, digit) -> [count; digit])

(* Question 3.4 *)

    // elSeq start — infinite sequence of look-and-say elements starting from `start`.
    //
    // Seq.unfold carries the current element as state and yields it,
    // then produces the next element for the following iteration.
    // This avoids recomputing from scratch for each element (unlike Seq.initInfinite).
    let elSeq (start : element) : element seq =
        Seq.unfold (fun e -> Some (e, nextElement e)) start

    // elSeq2 — alternative using a recursive sequence expression.
    // Functionally equivalent to elSeq; uses `seq { yield; yield! }` syntax.
    let rec elSeq2 (start : element) : element seq =
        seq { yield start; yield! elSeq2 (nextElement start) }

    (*

    Q: Why would Seq.initInfinite not be an appropriate choice to
       write a function like elSeq?

    A: Seq.initInfinite f generates the sequence [f 0; f 1; f 2; ...] where
       `f` takes the INDEX and returns the element at that position.

       To compute the n-th look-and-say element, you would need a function
       `nthElement n` that starts from the initial element and applies
       nextElement n times.  Computing nthElement n requires computing
       nthElement (n-1), which requires (n-2), all the way down to the start.
       If each element is computed from scratch independently, the total
       cost is O(n²) just to generate the first n elements.

       Seq.unfold is appropriate because it PASSES STATE FORWARD: each step
       receives the previous element and only computes one nextElement call.
       The total cost for the first n elements is O(n * |element|), which is
       far more efficient.

    *)

(* Question 3.5 *)

    // (compress is already defined above in Q3.3)
    // Repeated here as a note: compress performs run-length encoding.
    // elSeq calls nextElement which calls compress internally.

(* Question 3.6 *)

    open JParsec.TextParser

    // elParse — a JParsec parser that reads a sequence of digit characters
    // and returns them as an element (list of ints).
    //
    // COMBINATORS:
    //   digit   : Parser<char>  — matches a single character '0'–'9'
    //   |>> f   : transforms the parse result: char → int via ASCII arithmetic
    //   many    : runs the digit parser repeatedly, collecting into a list
    //
    // many never fails — if the first character is not a digit, returns [].
    let elParse : Parser<element> =
        many (digit |>> fun c -> int c - int '0')

    // elFromString2 — parse a string into an element using the JParsec parser.
    //
    // run elParse s  : runs the parser on string s, returns a ParserResult<element>
    // getSuccess     : extracts the successful value (throws if parse failed)
    let elFromString2 (s : string) : element =
        run elParse s |> getSuccess


(* 4: Rings *)

(* Question 4.1 *)

    // A ring is a circular data structure supporting O(1) rotation in both directions.
    //
    // REPRESENTATION: Ring(l, r) — a two-list zipper.
    //   r = elements at and clockwise from current (head of r = current element)
    //   l = elements counterclockwise of current, in REVERSE order
    //       (head of l = nearest counterclockwise neighbour)
    //
    // Example: ring [1;2;3;4] with current=2:
    //   Ring([1], [2;3;4])   — 1 is counterclockwise, 2 is current, 3;4 are clockwise
    //
    // This representation gives O(1) CW rotation (move head of r to head of l)
    // and amortised O(1) CCW rotation.
    type 'a ring = Ring of 'a list * 'a list

(* Question 4.2 *)

    // length — total number of elements in the ring.
    let length (Ring(l, r) : 'a ring) : int = List.length l + List.length r

    // ringFromList — create a ring from a list; first element becomes current.
    let ringFromList (xs : 'a list) : 'a ring = Ring([], xs)

    // ringToList — return all elements starting from current, going clockwise.
    // r is already in CW order; l is reversed, so we reverse it to get CCW elements
    // and append them after r.
    let ringToList (Ring(l, r) : 'a ring) : 'a list = r @ List.rev l

(* Question 4.3 *)

    // empty — create an empty ring.
    let empty () : 'a ring = Ring([], [])

    // push — add element x at the current position (in front of existing current).
    let push (x : 'a) (Ring(l, r) : 'a ring) : 'a ring = Ring(l, x :: r)

    // peek — return the current element without removing it, or None if empty.
    let peek : 'a ring -> 'a option = function
        | Ring(_, x :: _) -> Some x
        | _               -> None

    // pop — remove and return the current element; None if ring is empty.
    let pop : 'a ring -> ('a * 'a ring) option = function
        | Ring(l, x :: r) -> Some (x, Ring(l, r))
        | _               -> None

    // cw — rotate ONE step clockwise (current advances to next CW element).
    //
    // Normal case: Ring(l, x::r) → Ring(x::l, r)  current was x, now it's head of r.
    // Wrap case: Ring(l, [x]) → when x was last, reverse everything to wrap around.
    //   List.rev (x :: l) gives us all elements in the ring starting from the
    //   "first" element (which came after x in the original circular order).
    //
    // Ring([], []) case: empty ring, nothing to do.
    let cw : 'a ring -> 'a ring = function
        | Ring(_, [])     -> Ring([], [])
        | Ring(l, [x])    -> Ring([], List.rev (x :: l))
        | Ring(l, x :: r) -> Ring(x :: l, r)

    // ccw — rotate ONE step counterclockwise (current goes back to previous element).
    //
    // Normal case: Ring(x::l, r) → Ring(l, x::r)  x was nearest CCW, now it's current.
    // Wrap case: Ring([], r) → the CCW neighbour is the LAST element of r.
    //   Reverse r, take head as new current, rest become the new l.
    let ccw : 'a ring -> 'a ring = function
        | Ring([], [])     -> Ring([], [])
        | Ring([], r)      ->
            match List.rev r with
            | []      -> Ring([], [])
            | x :: xs -> Ring(xs, [x])
        | Ring(x :: l, r) -> Ring(l, x :: r)

(* Question 4.4 *)

    type StateMonad<'a, 'b> = SM of ('b ring -> ('a * 'b ring) option)
    let ret x = SM (fun st -> Some (x, st))
    let bind (SM m) f =
        SM (fun st ->
            match m st with
            | None -> None
            | Some (x, st') ->
                let (SM g) = f x
                g st')

    let (>>=) m f = bind m f
    let (>>>=) m n = m >>= (fun () -> n)
    let evalSM (SM f) s = f s

    // smLength — monadic: return the current ring length without changing state.
    let smLength : StateMonad<int, 'b> =
        SM (fun r -> Some (length r, r))

    // smPush x — monadic: push element x at the current ring position.
    let smPush (x : 'b) : StateMonad<unit, 'b> =
        SM (fun r -> Some ((), push x r))

    // smPop — monadic: pop the current element; None if ring is empty.
    let smPop : StateMonad<'b, 'b> =
        SM (fun r ->
            match pop r with
            | None           -> None
            | Some (x, r')   -> Some (x, r'))

    // smCW / smCCW — monadic ring rotations.
    let smCW : StateMonad<unit, 'b> =
        SM (fun r -> Some ((), cw r))

    let smCCW : StateMonad<unit, 'b> =
        SM (fun r -> Some ((), ccw r))

(* Question 4.5 *)

    (* You may solve this exercise either using monadic operators or
        using computational expressions. *)

    type StateBuilder() =
        member this.Bind(x, f)    = bind x f
        member this.Zero ()       = ret ()
        member this.Return(x)     = ret x
        member this.ReturnFrom(x) = x
        member this.Combine(a, b) = a >>= (fun _ -> b)

    let state = new StateBuilder()

    // ringStep — one step of the "remove-evens" sweep:
    //   Pop current element. If it is EVEN, remove it (don't put back) and return its value.
    //   If it is ODD, put it back and rotate CW (skip it), return 0.
    //
    // This lets iterRemoveSumEven step through each original position exactly once.
    let ringStep () : StateMonad<int, int> =
        smPop >>= fun x ->
            if x % 2 = 0 then
                ret x
            else
                smPush x >>= fun () ->
                smCW     >>= fun () ->
                ret 0

    // iterRemoveSumEven n — process exactly n elements, summing any even ones removed.
    //
    // Processes one element per step using ringStep. After n steps:
    //   - all even elements encountered have been removed from the ring
    //   - the returned value is their total sum
    //   - odd elements remain (they were pushed back and skipped)
    let iterRemoveSumEven (n : int) : StateMonad<int, int> =
        let rec aux i =
            if i = 0 then ret 0
            else
                state {
                    let! v    = ringStep ()
                    let! rest = aux (i - 1)
                    return v + rest
                }
        aux n
