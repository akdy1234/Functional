namespace SWU_FP2019
(* You are allowed to remove this namespace if you submit a .fsx-file *)


module Peano =

    (* Question 1.1 *)

    // Peano numbers: a unary representation of the natural numbers.
    //   O         = zero
    //   S(O)      = one  (S = successor)
    //   S(S(O))   = two
    //   S(S(S(O))) = three
    // Every natural number n is represented by a chain of n nested S constructors.
    type Peano =
    | O
    | S of Peano

    (* Question 1.2 *)

    // toInt — convert a Peano number to an ordinary int.
    //
    // Count the number of S constructors by recursion.
    // O → 0 (base case). S p → 1 + toInt p (peel one S, add 1, recurse).
    //
    // NOT tail-recursive: the `1 + toInt p` has a pending addition after the call.
    let rec toInt : Peano -> int = function
        | O   -> 0
        | S p -> 1 + toInt p

    // fromInt — convert a non-negative int to a Peano number.
    //
    // 0 → O.  n → S(fromInt(n-1)).
    // Wraps n in n nested S constructors.
    let rec fromInt : int -> Peano = function
        | 0 -> O
        | n -> S (fromInt (n - 1))

    (* Question 1.3 *)

    // add a b — Peano addition: a + b.
    //
    // IDEA: strip S constructors off `a` one by one, adding each to `b`.
    //   O    + b = b
    //   S a' + b = S (a' + b)  — peel one S off a, wrap result in S
    //
    // WHY NOT TAIL-RECURSIVE: `S (add a' b)` has a pending S-wrapping after the call.
    let rec add (a : Peano) (b : Peano) : Peano =
        match a with
        | O    -> b
        | S a' -> S (add a' b)

    // mult a b — Peano multiplication: a * b.
    //
    // IDEA: a * b = b + b + ... + b  (a times).
    //   O    * b = O
    //   S a' * b = b + (a' * b)   — one copy of b plus a'-1 more copies
    let rec mult (a : Peano) (b : Peano) : Peano =
        match a with
        | O    -> O
        | S a' -> add b (mult a' b)

    // pow a b — Peano exponentiation: a^b.
    //
    // IDEA: a^0 = 1,  a^(S b') = a * a^b'
    //   We use S O as the Peano representation of 1.
    let rec pow (a : Peano) (b : Peano) : Peano =
        match b with
        | O    -> S O        // a^0 = 1
        | S b' -> mult a (pow a b')

    (* Question 1.4 *)

    // tailAdd — tail-recursive addition using b as accumulator.
    //
    // INSIGHT: a + b = strip a S-constructors, add each to b.
    // Instead of building up S on the return path (pending S), we put each S
    // directly onto the accumulator BEFORE the recursive call.
    //   tailAdd O b     = b
    //   tailAdd (S a') b = tailAdd a' (S b)   ← tail call, no pending S
    //
    // Trace: tailAdd S(S(O)) S(O)
    //   = tailAdd S(O) S(S(O))
    //   = tailAdd O S(S(S(O)))
    //   = S(S(S(O)))  =  3 ✓
    let tailAdd (a : Peano) (b : Peano) : Peano =
        let rec aux a acc =
            match a with
            | O    -> acc
            | S a' -> aux a' (S acc)
        aux a b

    // tailMult — tail-recursive multiplication via accumulator.
    //
    // a * b = add b to accumulator a times.
    // Start with acc = O; each step: acc ← tailAdd b acc; a ← pred a.
    let tailMult (a : Peano) (b : Peano) : Peano =
        let rec aux a acc =
            match a with
            | O    -> acc
            | S a' -> aux a' (tailAdd b acc)
        aux a O

    // tailPow — tail-recursive exponentiation via CPS.
    //
    // Naive pow has pending tailMult after each recursive call.
    // CPS: instead of multiplying on the way back, capture the multiplication
    // in a continuation closure.
    //
    // Trace: tailPow a S(S(O)) = a^2
    //   aux S(S(O)) id
    //   = aux S(O) (fun r -> id (tailMult a r))
    //   = aux O    (fun r -> (fun r -> id (tailMult a r)) (tailMult a r))
    //   = outerCont (S O)               ← base case: a^0 = 1 = S O
    //   = ... unfolds to tailMult a (tailMult a (S O)) = a*a*1 = a^2 ✓
    let tailPow (a : Peano) (b : Peano) : Peano =
        let rec aux b cont =
            match b with
            | O    -> cont (S O)
            | S b' -> aux b' (fun result -> cont (tailMult a result))
        aux b id

    (* Question 1.5 *)

    // loop n f x — apply f to x exactly n times.
    //
    // This is function iteration: f^n(x).
    //   loop O     f x = x         (apply 0 times = identity)
    //   loop (S n) f x = loop n f (f x)   (apply once, then loop n more times)
    //
    // TAIL-RECURSIVE: the recursive call `loop n f (f x)` is the last operation.
    // f x is computed, and its result is passed as the new starting value.
    let loop (n : Peano) (f : 'a -> 'a) (x : 'a) : 'a =
        let rec aux n x =
            match n with
            | O    -> x
            | S n' -> aux n' (f x)
        aux n x

    (* Question 1.6 *)

    // loopAdd, loopMult, loopPow — re-implement the arithmetic operations using loop.
    //
    // loopAdd a b:
    //   Apply S (successor) to b, a times.
    //   Each application increments the result by 1: loop a S b = a + b ✓
    //
    // loopMult a b:
    //   Apply (tailAdd b) to O, a times.
    //   Each application adds b once: after a steps, acc = a * b ✓
    //
    // loopPow a b:
    //   Apply (tailMult a) to S O (=1), b times.
    //   Each application multiplies by a: after b steps, acc = a^b ✓
    let loopAdd (a : Peano) (b : Peano) : Peano = loop a S b
    let loopMult (a : Peano) (b : Peano) : Peano = loop a (tailAdd b) O
    let loopPow (a : Peano) (b : Peano) : Peano = loop b (tailMult a) (S O)


module CodeComprehension =

    let rec f x =
        function
        | []                -> None
        | y::ys when x = y  -> Some ys
        | y::ys when x <> y ->
            match f x ys with
            | Some ys' -> Some (y::ys')
            | None     -> None

    let rec g xs =
        function
        | []    -> xs = []
        | y::ys ->
            match f y xs with
            | Some xs' -> g xs' ys
            | None     -> false

    (* Question 2.1 *)

    (*

    Q: What are the types of functions f and g?

    A: f : 'a -> 'a list -> 'a list option   (requires 'a : equality)
         Takes an element x and a list. Returns Some(remaining) if x is found,
         None if x is not in the list.

       g : 'a list -> 'a list -> bool         (requires 'a : equality)
         Takes two lists. Returns true if they have the same multiset of elements.


    Q: What do functions f and g do?
       Focus on what they do rather than how they do it.

    A: f x lst — removes the FIRST occurrence of x from lst.
         If x is present, returns Some(lst without the first x).
         If x is absent, returns None.
         Example: f 2 [1;2;3;2] = Some [1;3;2]
                  f 5 [1;2;3]   = None

       g xs ys — checks whether xs and ys are PERMUTATIONS of each other,
         i.e., they contain exactly the same elements with the same multiplicities
         (same multiset).
         Algorithm: for each element y in ys, try to remove y from xs.
         If all removals succeed AND xs is empty when ys is exhausted → true.
         Example: g [1;2;3] [3;1;2] = true
                  g [1;2;3] [1;2]   = false  (xs not empty when ys runs out)
                  g [1;2]   [1;2;3] = false  (3 not in xs)


    Q: What would be appropriate names for functions f and g?

    A: f → removeFirst / removeFirstOccurrence
       g → isPermutationOf / sameMultiset / arePermutations

    *)

    (* Question 2.2 *)

    (*
    Q: The function f generates a warning during compilation: "Incomplete pattern match".
       Why and where? Fix it.

    A: The match in f has two guarded arms on `y::ys`:
         | y::ys when x = y  → ...
         | y::ys when x <> y → ...
       Both use `when` guards.  Even though `x = y OR x <> y` is always true,
       the compiler cannot prove this statically — it sees two possibly-failing
       guarded patterns and warns that some input might reach a missing case.

       FIX: Remove the `when x <> y` guard from the second arm.
       By the time we reach the second arm, the first arm already failed
       (x ≠ y), so the guard is redundant.  A plain `y::ys` is a catch-all
       and satisfies the compiler.
    *)

    // f2 — fixed version of f that eliminates the incomplete-match warning.
    // The second `y::ys` arm has no guard — it matches everything not caught earlier.
    let rec f2 (x : 'a) : 'a list -> 'a list option = function
        | []               -> None
        | y::ys when x = y -> Some ys
        | y::ys            ->
            match f2 x ys with
            | Some ys' -> Some (y::ys')
            | None     -> None

    (* Question 2.3 *)

    // fOpt — version of f using Option.map instead of explicit match.
    //
    // The `match f x ys with Some ys' -> ... | None -> None` pattern
    // is exactly what Option.map does: apply a function to the Some value,
    // propagate None unchanged.
    //
    // Option.map (fun ys' -> y :: ys') (fOpt x ys)
    //   = Some (y :: ys') if fOpt x ys = Some ys'
    //   = None            if fOpt x ys = None
    let rec fOpt (x : 'a) : 'a list -> 'a list option = function
        | []               -> None
        | y::ys when x = y -> Some ys
        | y::ys            ->
            fOpt x ys |> Option.map (fun ys' -> y :: ys')

    // gOpt — version of g using Option.bind instead of explicit match.
    //
    // `f y xs >>= fun xs' -> gOpt xs' ys` means:
    //   if f y xs = Some xs', continue with gOpt xs' ys
    //   if f y xs = None, short-circuit to None
    //
    // Returns Some() when xs and ys are permutations, None otherwise.
    // (Wrapping the bool result in Option to use the option-monad chain.)
    let rec gOpt (xs : 'a list) : 'a list -> 'a list option = function
        | []    -> if xs = [] then Some [] else None
        | y::ys ->
            fOpt y xs |> Option.bind (fun xs' -> gOpt xs' ys)

    (* Question 2.4 *)

    (*
    Q: Neither f nor g is tail recursive. Pick one and explain why.
       Only implement the tail-recursive version for the one that is NOT tail-recursive.

    A: f is NOT tail recursive.

       Evaluate f 1 [2;1]:

         f 1 [2;1]
         (* x=1, y=2, x≠y, recurse *)
         match f 1 [1] with           ← RECURSIVE CALL
         | Some ys' -> Some (2::ys')  ← PENDING: wrap result in Some(2::...)
         | None     -> None

         f 1 [1]
         (* x=1, y=1, x=y *)
         = Some []

         So:  match Some [] with
              | Some ys' -> Some (2::[]) = Some [2]

       The recursive call `f 1 [1]` is INSIDE a match, and after it returns,
       we still need to compute `Some (2::ys')`.  That pending work keeps the
       current stack frame alive.  For a list of length n, up to n frames pile up.

       g IS tail recursive:
         `g xs' ys` is the LAST thing returned in the `Some xs'` branch —
         nothing waits on the stack after it returns.  The compiler can reuse
         the current frame.

       Therefore we only implement fTail.
    *)

    // fTail — tail-recursive version of f using an accumulator.
    //
    // Elements before x are collected in `acc` (reversed).
    // When x is found: combine List.rev acc @ ys (elements after x) → Some result.
    // When [] is reached: x was not found → None.
    //
    // The call `aux (y::acc) ys` is the last operation — tail position. ✓
    let fTail (x : 'a) (lst : 'a list) : 'a list option =
        let rec aux acc = function
            | []               -> None
            | y::ys when x = y -> Some (List.rev acc @ ys)
            | y::ys            -> aux (y::acc) ys
        aux [] lst

    // gTail is not needed — g is already tail-recursive (see analysis above).
    let gTail = g


module Pi =

    (* Question 3.1 *)

    // calculatePi n — approximate π using the first n terms of the Leibniz series.
    //
    // LEIBNIZ FORMULA:
    //   π/4 = 1 - 1/3 + 1/5 - 1/7 + ...
    //       = Σ(j=0..∞) (-1)^j / (2j+1)
    //   π   = 4 * Σ(j=0..n-1) (-1)^j / (2j+1)
    //
    // SIGN: term j is positive when j is even, negative when j is odd.
    //       (-1)^j is encoded as `if j % 2 = 0 then 1.0 else -1.0`.
    //
    // CONVERGENCE: very slow — the Leibniz series converges at O(1/n) rate.
    //   You need millions of terms to get 6 decimal places of accuracy.
    let calculatePi (n : int) : float =
        let term j = (if j % 2 = 0 then 1.0 else -1.0) / float (2*j + 1)
        4.0 * List.sumBy term [0..n-1]

    (* Question 3.2 *)

    // piSeq — infinite sequence of π approximations using increasing term counts.
    //
    // The n-th element of the sequence (0-indexed) is calculatePi(n+1),
    // so successive elements use more terms and converge toward π.
    //
    // Seq.initInfinite is appropriate here because the n-th element can be
    // computed INDEPENDENTLY from index n without needing prior elements.
    // (Unlike look-and-say sequences where each element depends on the previous.)
    let piSeq () : float seq =
        Seq.initInfinite (fun n -> calculatePi (n + 1))

    (* Question 3.3 *)

    // circleArea r pi — area of a circle with radius r, using pi as π.
    //   A = π * r²
    let circleArea (r : float) (pi : float) : float = pi * r * r

    // sphereVolume r pi — volume of a sphere with radius r, using pi as π.
    //   V = (4/3) * π * r³
    let sphereVolume (r : float) (pi : float) : float = (4.0 / 3.0) * pi * r * r * r

    (* Question 3.4 *)

    // circleSphere r — infinite sequence of (area, volume) pairs.
    //
    // As the index increases, the π approximation used improves,
    // so both the area and volume converge to their true values.
    //
    // Each step: take the next π approximation from piSeq, compute area and volume.
    let circleSphere (r : float) : (float * float) seq =
        piSeq () |> Seq.map (fun pi -> (circleArea r pi, sphereVolume r pi))

    (* Question 3.5 *)

    // parallelPi n chunks — compute π using n Leibniz terms in parallel, split
    // into `chunks` groups.
    //
    // STRATEGY:
    //   Divide the n terms into `chunks` contiguous ranges.
    //   Each async task computes the partial sum for its range using GLOBAL indices
    //   (so the sign of each term is correct regardless of which chunk it's in).
    //   Async.Parallel runs all tasks concurrently; we sum results and multiply by 4.
    let parallelPi (n : int) (chunks : int) : float =
        let chunkSize = n / chunks
        [0..chunks-1]
        |> List.map (fun i ->
            async {
                let startIdx = i * chunkSize
                let endIdx   = min (startIdx + chunkSize - 1) (n - 1)
                return
                    [ startIdx..endIdx ]
                    |> List.sumBy (fun j ->
                        (if j % 2 = 0 then 1.0 else -1.0) / float (2*j + 1))
            })
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Array.sum
        |> fun x -> 4.0 * x

    (*
    Q: Comment on the correctness and efficiency of your parallel implementation.

    A: CORRECTNESS: The implementation is correct.
       Each task uses the GLOBAL index j (not a local index within the chunk)
       to determine the sign: `if j % 2 = 0 then +1 else -1`.
       Since the Leibniz sum is a finite sum of independent terms, splitting it
       into chunks and summing the partial results is mathematically equivalent
       to computing all n terms in sequence.  The final `Array.sum` collects the
       chunk sums into the correct total.

       EFFICIENCY: The speedup is limited by the number of CPU cores.
       With k physical cores and `chunks` = k tasks, the ideal speedup is ~k.
       In practice, thread creation and synchronisation overhead reduce this.
       Parallelism is beneficial only when each chunk does meaningful work —
       i.e., when n/chunks is large enough to amortise the overhead.
    *)


module Typewriter =

    type Dir = Left | Right

    (* Question 4.1 *)

    // Tape<'a> — a two-list zipper modelling an infinite Turing-machine tape.
    //
    // Tape(leftRev, current, right):
    //   leftRev  = cells to the LEFT of the head, in REVERSE order
    //              (head of leftRev = nearest left neighbour)
    //   current  = cell under the read/write head (None = blank)
    //   right    = cells to the RIGHT of the head, in order
    //              (head of right = nearest right neighbour)
    //
    // Moving left/right is O(1); reading/writing the current cell is O(1).
    type Tape<'a> = Tape of 'a option list * 'a option * 'a option list

    (* Question 4.2 *)

    // tapeFromList — initialise a tape from a list; head positioned at first element.
    //
    // The list becomes the initial tape content.  No left context yet (leftRev = []).
    let tapeFromList (lst : 'a option list) : Tape<'a> =
        match lst with
        | []      -> Tape([], None, [])
        | x :: xs -> Tape([], x, xs)

    // tapeToList — flatten the tape back to a list in left-to-right order.
    //
    // leftRev is reversed to recover left-to-right order, then
    // the current cell and right cells are appended.
    let tapeToList (Tape(l, c, r) : Tape<'a>) : 'a option list =
        List.rev l @ [c] @ r

    (* Question 4.3 optional *)

    // moveHead dir tape — move the read/write head one cell left or right.
    //
    // Moving RIGHT: current cell joins leftRev; head of right becomes current.
    //   If right is empty, we move into blank territory (current = None).
    // Moving LEFT: current cell joins right; head of leftRev becomes current.
    //   If leftRev is empty, we move into blank territory on the left.
    let moveHead (dir : Dir) (Tape(l, c, r) : Tape<'a>) : Tape<'a> =
        match dir with
        | Right ->
            match r with
            | []      -> Tape(c :: l, None, [])
            | x :: xs -> Tape(c :: l, x, xs)
        | Left ->
            match l with
            | []      -> Tape([], None, c :: r)
            | x :: xs -> Tape(xs, x, c :: r)

    // readtape — return the symbol currently under the head (None = blank).
    let readtape (Tape(_, c, _) : Tape<'a>) : 'a option = c

    // writeTape v tape — overwrite the current cell with value v.
    // Left and right contexts are unaffected; only the current cell changes.
    let writeTape (v : 'a option) (Tape(l, _, r) : Tape<'a>) : Tape<'a> = Tape(l, v, r)

    (* Question 4.4 *)

    type Action<'a when 'a : comparison> =
       Act of Map<'a option, string * 'a option * Dir>
    type Program<'a when 'a : comparison> =
       Prog of Map<string, Action<'a>>

    // evalAction action tape — apply one Turing-machine action to the tape.
    //
    // LOOKUP: read the current cell symbol and look it up in the action map.
    //   Not found → None (this action has no rule for the current symbol).
    //   Found (nextState, writeVal, dir) →
    //     1. Write writeVal to the current cell.
    //     2. Move head in direction dir.
    //     3. Return Some (nextState, updatedTape).
    let evalAction (Act m : Action<'a>) : Tape<'a> -> (string * Tape<'a>) option =
        fun tape ->
            match Map.tryFind (readtape tape) m with
            | None                             -> None
            | Some (nextState, writeVal, dir)  ->
                Some (nextState, tape |> writeTape writeVal |> moveHead dir)

    // evalProgram prog — run a full Turing machine program until it halts.
    //
    // EXECUTION MODEL:
    //   - Look up the current state in the program map.
    //   - If the state is NOT in the map → halt; return Some(state, tape).
    //   - If found → apply the action; if the action has no rule for the
    //     current symbol → return None (undefined transition).
    //   - Otherwise step to the next state and continue.
    //
    // Returns: Some(finalState, finalTape) on successful halt, None on error.
    let evalProgram (Prog prog : Program<'a>) : string -> Tape<'a> -> (string * Tape<'a>) option =
        let rec run state tape =
            match Map.tryFind state prog with
            | None        -> Some (state, tape)  // halt: state not in program
            | Some action ->
                match evalAction action tape with
                | None                      -> None
                | Some (nextState, tape')   -> run nextState tape'
        run

    (* Question 4.5 *)

    (* You may use *either* railway-oriented programming or computation expressions.
       You do not have to use both *)

    (* railway-oriented programming *)
    let ret = Some
    let bind f =
        function
        | None   -> None
        | Some x -> f x
    let (>>=) x f = bind f x

    (* Computation expressions *)
    type opt<'a> = 'a option
    type OptBuilderClass() =
        member t.Return (x : 'a) : opt<'a> = ret x
        member t.ReturnFrom (x : opt<'a>) = x
        member t.Bind (o : opt<'a>, f : 'a -> opt<'b>) : opt<'b> = bind f o
    let opt = new OptBuilderClass()

    // |>>| — sequential composition of two program evaluators.
    //
    // (eval1 |>>| eval2) state tape:
    //   1. Run eval1 on (state, tape).
    //   2. If it returns None → fail (propagate None).
    //   3. If it returns Some(state', tape') → feed (state', tape') into eval2.
    //
    // This is exactly monadic bind for the `string -> Tape -> option` type:
    // run the first evaluator, and if it succeeds, chain the second one.
    //
    // Uses the `>>=` (railway bind) defined above.
    let (|>>|) (eval1 : string -> Tape<'a> -> (string * Tape<'a>) option)
               (eval2 : string -> Tape<'a> -> (string * Tape<'a>) option)
               : string -> Tape<'a> -> (string * Tape<'a>) option =
        fun state tape ->
            eval1 state tape >>= fun (state', tape') -> eval2 state' tape'

    (* Question 4.6 *)

    // combineProgramEvaluations evals — fold a list of evaluators into one.
    //
    // Applies |>>| left-associatively:
    //   [e1; e2; e3] → (e1 |>>| e2) |>>| e3
    //
    // Empty list: the identity evaluator (does nothing, returns (state, tape) unchanged).
    let combineProgramEvaluations
        (evals : (string -> Tape<'a> -> (string * Tape<'a>) option) list)
        : string -> Tape<'a> -> (string * Tape<'a>) option =
        match evals with
        | []        -> fun state tape -> Some (state, tape)
        | e :: rest -> List.fold (|>>|) e rest

    (* Testing your typewriter (EXAM ENDS HERE)
       You do not have to write any code below this point, but you may comment out
       the function addThreeUnaryNumbers if you wish to test your |>>| function.

       You may change or remove the code below, but you are not even expected to read it.
       *)

    let unaryAddition startL midL1 midL2 endL =

        let move c dir label = Map.add c (label, c, dir)
        let write old n dir label = Map.add old (label, n, dir)

        let moveToEnd startLabel endLabel =
            Map.empty |>
            move (Some true) Right startLabel |>
            write (Some false) (Some true) Left endLabel |>
            Act

        let moveToStart startLabel endLabel =
            Map.empty |>
            move (Some true) Left startLabel |>
            move None Right endLabel |>
            Act

        let start nextLabel endLabel =
            Map.empty |>
            write (Some false) None Right endLabel |>
            write (Some true) None Right nextLabel |>
            Act

        Map.empty |>
        Map.add startL (start midL1 endL) |>
        Map.add midL1 (moveToEnd midL1 midL2) |>
        Map.add midL2 (moveToStart midL2 endL) |>
        Prog

    let testUnary =
        let mkUnary x = [for i in 1..x do yield Some true
                         yield Some false]

        function
        | [] -> []
        | xs -> List.collect mkUnary xs

    let addUnaryNumbers eval numbers =
        let input = testUnary numbers
        let output = testUnary [List.sum numbers]

        eval "start" (tapeFromList input) |>
        Option.map snd |>
        Option.map tapeToList |>
        (fun lst -> printfn "Adding numbers %A successful : %b" numbers (lst = Some output))

    let addTwoUnaryNumbers a b =
        addUnaryNumbers
            (evalProgram (unaryAddition "start" "mid1" "mid2" "end"))
            [a; b]
(*
Remove this comment to test your |>>| function
    let addThreeUnaryNumbers a b c =
        addUnaryNumbers
            (evalProgram (unaryAddition "start" "mid1" "mid2" "end") |>>|
             evalProgram (unaryAddition "end" "mid1" "mid2" "start"))
            [a; b; c]
*)
    let addSeveralUnaryNumbers numbers =
        let eval1 = evalProgram (unaryAddition "start" "mid1" "mid2" "end")
        let eval2 = evalProgram (unaryAddition "end" "mid1" "mid2" "start")

        let evals =
            [for i in 2..List.length numbers
                do yield if i % 2 = 0 then eval1 else eval2] |>
            combineProgramEvaluations

        addUnaryNumbers evals numbers
