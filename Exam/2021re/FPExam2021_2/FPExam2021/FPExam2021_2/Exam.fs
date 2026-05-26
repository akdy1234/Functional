module Exam2021_2
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
 module Exam2021_2 =
 *)

(* 1: Binary lists *)

(* Question 1.1 *)

    // binList<'a,'b> — a linked list whose elements alternate between two types.
    //   Nil              — empty list
    //   Cons1 of 'a * _  — a node holding an 'a value
    //   Cons2 of 'b * _  — a node holding a 'b value
    // Both constructors carry the tail, enabling heterogeneous sequences.
    type binList<'a, 'b> =
    | Nil
    | Cons1 of 'a * binList<'a, 'b>
    | Cons2 of 'b * binList<'a, 'b>

    // length — counts ALL nodes regardless of which constructor they use.
    //
    // Both Cons1 and Cons2 contribute 1 to the length; Nil contributes 0.
    // Simple structural recursion — matches the shape of the type.
    let rec length : binList<'a,'b> -> int = function
        | Nil            -> 0
        | Cons1(_, rest) -> 1 + length rest
        | Cons2(_, rest) -> 1 + length rest

(* Question 1.2 *)

    // split — partition a binList into two ordinary lists: one for each type.
    //
    // Returns (aList, bList) where aList contains all 'a values (in order)
    // and bList contains all 'b values (in order).
    //
    // We recurse to the end (Nil), then add each element to the appropriate list
    // as the call stack unwinds.
    let rec split : binList<'a,'b> -> 'a list * 'b list = function
        | Nil            -> ([], [])
        | Cons1(x, rest) ->
            let (xs, ys) = split rest
            (x :: xs, ys)
        | Cons2(y, rest) ->
            let (xs, ys) = split rest
            (xs, y :: ys)

    // length2 — compute length using split, then sum the two list lengths.
    //
    // This is a higher-level (but less efficient) alternative to `length`:
    // it traverses the list TWICE (once to split, once per List.length).
    // Useful to demonstrate that split captures all elements.
    let length2 (bl : binList<'a,'b>) : int =
        let (xs, ys) = split bl
        List.length xs + List.length ys

(* Question 1.3 *)

    // map f g bl — apply f to every 'a element and g to every 'b element.
    //
    // Preserves the shape of the list: Cons1 stays Cons1, Cons2 stays Cons2.
    // The result type is binList<'c,'d> where 'c = return type of f, 'd = return type of g.
    let rec map (f : 'a -> 'c) (g : 'b -> 'd) : binList<'a,'b> -> binList<'c,'d> = function
        | Nil            -> Nil
        | Cons1(x, rest) -> Cons1(f x, map f g rest)
        | Cons2(y, rest) -> Cons2(g y, map f g rest)

(* Question 1.4 *)

    // filter p1 p2 bl — keep Cons1 nodes where p1 holds and Cons2 nodes where p2 holds.
    //
    // Nodes failing their predicate are skipped (the tail is still processed).
    // The relative order of remaining nodes is preserved.
    let rec filter (p1 : 'a -> bool) (p2 : 'b -> bool) : binList<'a,'b> -> binList<'a,'b> = function
        | Nil            -> Nil
        | Cons1(x, rest) ->
            if p1 x then Cons1(x, filter p1 p2 rest)
            else filter p1 p2 rest
        | Cons2(y, rest) ->
            if p2 y then Cons2(y, filter p1 p2 rest)
            else filter p1 p2 rest

(* Question 1.5 *)

    // fold f g acc bl — left fold over a binList with two combining functions.
    //
    // f is applied when a Cons1 node is visited, g when a Cons2 node is visited.
    // The accumulator is threaded left-to-right through all nodes.
    //
    // This generalises List.fold to heterogeneous lists.
    // Example (compute length):
    //   fold (fun n _ -> n+1) (fun n _ -> n+1) 0 bl = length bl
    let rec fold (f : 'acc -> 'a -> 'acc) (g : 'acc -> 'b -> 'acc)
                 (acc : 'acc) : binList<'a,'b> -> 'acc = function
        | Nil            -> acc
        | Cons1(x, rest) -> fold f g (f acc x) rest
        | Cons2(y, rest) -> fold f g (g acc y) rest


(* 2: Code Comprehension *)

    let rec foo xs ys =
      match xs, ys with
      | [], ys -> ys
      | xs, [] -> xs
      | x :: xs, y :: ys when x < y ->
        x :: (foo xs (y :: ys))
      | x :: xs, y :: ys ->
        y :: (foo (x :: xs) ys)

    and bar =
      function
      | [] -> []
      | [x] -> [x]
      | xs ->
        let (a, b) = List.splitAt (List.length xs / 2) xs
        foo (bar a) (bar b)

(* Question 2.1 *)

    (*

    Q: What are the types of functions foo and bar?

    A: foo : 'a list -> 'a list -> 'a list   (requires 'a : comparison)
         foo takes two lists and returns one list of the same element type.
         The `when x < y` guard requires elements to be comparable.

       bar : 'a list -> 'a list              (requires 'a : comparison)
         bar takes a list and returns a list of the same type.


    Q: What does the function bar do.
       Focus on what it does rather than how it does it.

    A: bar SORTS a list using MERGE SORT.

       How it works at a high level:
       - Base cases: [] and [x] are already sorted — return as-is.
       - Recursive case: split the list in half, recursively sort each half
         with bar, then merge the two sorted halves with foo (which is the
         merge step).

       foo is the MERGE function: it merges two sorted lists into one sorted list
       by repeatedly taking the smaller head from whichever list has it.


    Q: What would be appropriate names for functions foo and bar?

    A: foo → merge        (merges two sorted lists)
       bar → mergeSort    (sorts a list via merge sort)


    Q: What would be appropriate names of the values a and b in bar.

    A: a → leftHalf  (or firstHalf)  — the first half of the input list
       b → rightHalf (or secondHalf) — the second half of the input list

    *)

(* Question 2.2 *)

    (*
    The code includes the keyword "and".

    Q: What function does this keyword serve in general
       (why would you use "and" when writing any program)?

    A: `and` enables MUTUAL RECURSION between two or more definitions.
       Without it, a `let` binding can only refer to names defined before it.
       `let rec foo = ... and bar = ...` places both names in scope for both
       bodies, allowing foo to call bar and bar to call foo simultaneously.


    Q: What would happen if you removed it from this particular program and
       replaced it with a standard "let"
       (change the line "and bar = " to "let rec bar = ")?
       Explain why the program either does or does not work.

    A: Changing `and bar = function` to `let rec bar = function` would WORK.

       Here is why: foo is defined first with `let rec foo`.
       bar calls foo in its body (`foo (bar a) (bar b)`), and foo is already
       in scope when bar is processed.
       bar also calls itself recursively — that's handled by `let rec bar`.
       foo does NOT call bar, so there is no mutual dependency in that direction.

       Therefore, the `and` keyword is technically unnecessary here.
       It is used by convention (pairing a sort function with its merge helper)
       but replacing it with `let rec bar = function` would compile and run
       identically.

    *)

(* Question 2.3 *)

    // foo2 — re-implementation of foo using List.unfold.
    //
    // List.unfold f state repeatedly applies f to the state, producing elements
    // until f returns None.
    //
    // STATE: (remaining_xs, remaining_ys)
    //
    // At each step we look at the heads of both lists:
    //   ([], [])         → both exhausted → None (stop)
    //   (x::xs, [])      → ys empty → emit x, advance xs
    //   ([], y::ys)      → xs empty → emit y, advance ys
    //   (x::xs, y::ys) when x < y → emit x, advance xs (keep y in ys)
    //   (x::xs, y::ys)             → emit y, advance ys (keep x in xs)
    //
    // This mirrors the recursive structure of foo exactly, but driven by unfold.
    let foo2 xs ys =
        List.unfold (fun (xs, ys) ->
            match xs, ys with
            | [], []                         -> None
            | x :: xs, []                    -> Some (x, (xs, []))
            | [], y :: ys                    -> Some (y, ([], ys))
            | x :: xs, y :: ys when x < y   -> Some (x, (xs, y :: ys))
            | x :: xs, y :: ys               -> Some (y, (x :: xs, ys))
        ) (xs, ys)

(* Question 2.4 *)

    (*

    Q: Neither foo nor bar is tail recursive. Pick one (not both) of them and explain why.

    A: We pick FOO.

       A recursive call is tail-recursive only if it is the VERY LAST operation —
       nothing waits on the stack after it returns.

       Evaluate foo [1;3] [2;4]:

         foo [1;3] [2;4]
         = 1 :: (foo [3] [2;4])         (* 1 < 2, take 1 *)
                 ^^^^^^^^^^^^^^
                 recursive call is the ARGUMENT to `::`, not the final result.
                 After `foo [3] [2;4]` returns, we still need to prepend 1.

         = 1 :: (2 :: (foo [3] [4]))    (* 3 > 2, take 2 *)
         = 1 :: (2 :: (3 :: (foo [] [4])))  (* 3 < 4, take 3 *)
         = 1 :: (2 :: (3 :: [4]))       (* xs=[], return ys=[4] *)
         = [1;2;3;4]

       At each step, `foo ...` is nested inside ` :: (...)`.
       The `::` operator is PENDING — it cannot execute until the recursive
       call returns.  This keeps the current frame on the stack.  For lists
       of total length n, up to n frames accumulate simultaneously.
       That is the definition of non-tail-recursive.

    *)

(* Question 2.5 *)

    // fooTail — tail-recursive merge using Continuation-Passing Style (CPS).
    //
    // PROBLEM WITH foo: `x :: (foo ...)` leaves a pending `::` on the stack.
    //
    // CPS FIX: instead of returning a value upward, we PASS the "what to do next"
    // as a continuation function `cont`.  Pending `::` operations become closures
    // on the HEAP rather than stack frames.
    //
    // BASE CASES: one list exhausted → apply continuation to the remaining list.
    // RECURSIVE CASE: emit the smaller head into the continuation, recurse.
    //
    // TRACE: fooTail [1] [2]
    //   aux id [1] [2]
    //   = aux (fun r -> id (1::r)) [] [2]      (1 < 2, take 1)
    //   = (fun r -> id (1::r)) [2]             (xs=[], return ys=[2])
    //   = id [1;2] = [1;2] ✓
    let fooTail xs ys =
        let rec aux cont xs ys =
            match xs, ys with
            | [], ys -> cont ys
            | xs, [] -> cont xs
            | x :: xs, y :: ys when x < y ->
                aux (fun result -> cont (x :: result)) xs (y :: ys)
            | x :: xs, y :: ys ->
                aux (fun result -> cont (y :: result)) (x :: xs) ys
        aux id xs ys

(* Question 2.5 — barTail *)

    // barTail — tail-recursive merge sort using CPS.
    //
    // PROBLEM WITH bar: `foo (bar a) (bar b)` has two recursive calls to bar.
    // Neither is in tail position because their results must be merged with foo.
    //
    // CPS FIX: nest continuations.
    //   - Sort left half a, then IN THE CONTINUATION sort right half b,
    //     then IN THAT CONTINUATION merge with fooTail and pass to outer cont.
    //   This way each recursive call is the last operation in its scope. ✓
    let barTail lst =
        let rec aux cont = function
            | []  -> cont []
            | [x] -> cont [x]
            | xs  ->
                let (a, b) = List.splitAt (List.length xs / 2) xs
                aux (fun sortedA ->
                    aux (fun sortedB ->
                        cont (fooTail sortedA sortedB))
                    b)
                a
        aux id lst


(* 3: Approximating square roots *)

(* Question 3.1 *)

    // approxSquare n e — approximate √n to within tolerance e using Newton's method.
    //
    // NEWTON'S METHOD for square roots:
    //   Start with an initial guess (n/2 is a common choice).
    //   Repeatedly refine: new_guess = (guess + n/guess) / 2
    //   Stop when |guess² - n| < e  (the guess is close enough).
    //
    // CONVERGENCE: Newton's method converges quadratically — the number of
    // correct digits roughly doubles with each iteration.
    //
    // INITIAL GUESS: n/2 is used; for small n this may be slow to converge,
    // but it always converges for n ≥ 0.
    let approxSquare (n : float) (e : float) : float =
        let rec aux guess =
            if abs (guess * guess - n) < e then guess
            else aux ((guess + n / guess) / 2.0)
        aux (n / 2.0)

(* Question 3.2 *)

    // quadratic a b c e — find the real roots of ax² + bx + c = 0
    // using the quadratic formula, with approxSquare for the square root.
    //
    // QUADRATIC FORMULA:  x = (-b ± √(b²-4ac)) / (2a)
    //
    // DISCRIMINANT:
    //   d = b² - 4ac
    //   d > 0  → two distinct real roots
    //   d = 0  → one repeated real root
    //   d < 0  → no real roots → return None
    //
    // We use `approxSquare d e` to compute √d to tolerance e.
    let quadratic (a : float) (b : float) (c : float) (e : float) : (float * float) option =
        let d = b * b - 4.0 * a * c
        if d < 0.0 then None
        else
            let sqrtD = approxSquare d e
            Some ((-b + sqrtD) / (2.0 * a), (-b - sqrtD) / (2.0 * a))

(* Question 3.3 *)

    // parQuadratic — solve multiple quadratics in parallel using async workflows.
    //
    // Each quadratic is wrapped in `async { return ... }` creating a lazy task.
    // Async.Parallel runs all tasks concurrently and waits for all to finish.
    // Async.RunSynchronously blocks until done, returning an array of results.
    //
    // TYPE: (float * float * float * float) list -> (float * float) option list
    //   Each tuple is (a, b, c, epsilon) for one quadratic equation.
    let parQuadratic (polys : (float * float * float * float) list) : (float * float) option list =
        polys
        |> List.map (fun (a, b, c, e) -> async { return quadratic a b c e })
        |> Async.Parallel
        |> Async.RunSynchronously
        |> List.ofArray

(* Question 3.4 *)

    // solveQuadratic — solve a list of quadratics and collect all real roots.
    //
    // List.choose filters out None results and unwraps Some.
    // List.collect flattens the pair of roots into one flat list.
    let solveQuadratic (polys : (float * float * float * float) list) : float list =
        polys
        |> List.choose (fun (a, b, c, e) -> quadratic a b c e)
        |> List.collect (fun (r1, r2) -> [r1; r2])


(* 4: Rational numbers *)

(* Question 4.1 *)

    // A rational number is represented as a fraction in LOWEST TERMS with
    // a positive denominator.
    //   Rat(n, d): numerator=n, denominator=d
    //   Invariants: d > 0,  gcd(|n|, d) = 1
    //
    // Storing in lowest terms means Rat(1,2) = Rat(1,2) (not Rat(2,4)),
    // which makes equality comparisons work with structural equality.
    type rat = Rat of int * int

(* Question 4.2 *)

    // gcd — Euclidean GCD, always returns a positive value.
    let private gcd a b =
        let rec aux a b = if b = 0 then a else aux b (a % b)
        aux (abs a) (abs b)

    // mkRat n d — construct a rational in lowest terms.
    //
    // NORMALISATION STEPS:
    //   1. Reject d = 0 (undefined).
    //   2. Make denominator positive: if d < 0, negate both n and d.
    //   3. Divide both by gcd to reduce to lowest terms.
    let mkRat (n : int) (d : int) : rat =
        if d = 0 then failwith "denominator cannot be zero"
        else
            let sign = if d < 0 then -1 else 1
            let g    = gcd (abs n) (abs d)
            Rat(sign * n / g, sign * d / g)

    // ratToString — display as "n/d" or just "n" when denominator is 1.
    let ratToString (Rat(n, d) : rat) : string =
        if d = 1 then string n
        else sprintf "%d/%d" n d

(* Question 4.3 *)

    // Arithmetic operations — all normalise via mkRat.
    //
    // FORMULAS (fractions a/b and c/d):
    //   a/b + c/d = (a*d + c*b) / (b*d)
    //   a/b - c/d = (a*d - c*b) / (b*d)
    //   a/b * c/d = (a*c) / (b*d)
    //   a/b / c/d = (a*d) / (b*c)   [division flips second fraction]
    let plus  (Rat(n1,d1)) (Rat(n2,d2)) = mkRat (n1*d2 + n2*d1) (d1*d2)
    let minus (Rat(n1,d1)) (Rat(n2,d2)) = mkRat (n1*d2 - n2*d1) (d1*d2)
    let mult  (Rat(n1,d1)) (Rat(n2,d2)) = mkRat (n1*n2) (d1*d2)
    let div   (Rat(n1,d1)) (Rat(n2,d2)) =
        if n2 = 0 then failwith "division by zero rational"
        else mkRat (n1*d2) (d1*n2)

(* Question 4.4 *)

    // State monad where the STATE IS the current rational number.
    //
    // SM of (rat -> ('a * rat) option)
    //   Given the current rat, produce (result, newRat) on success, or None on failure.
    //
    // smPlus/smMinus/smMult r  — update the state by combining it with r.
    // smDiv r                   — fails (None) if r = 0, otherwise divides.
    //
    // These let you chain arithmetic operations monadically, starting from
    // an initial rational number via evalSM.
    type SM<'a> = SM of (rat -> ('a * rat) option)
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

    // smPlus r — add r to the current state rat.
    let smPlus (r : rat) : SM<unit> =
        SM (fun st -> Some ((), plus st r))

    // smMinus r — subtract r from the current state rat.
    let smMinus (r : rat) : SM<unit> =
        SM (fun st -> Some ((), minus st r))

    // smMult r — multiply the current state rat by r.
    let smMult (r : rat) : SM<unit> =
        SM (fun st -> Some ((), mult st r))

    // smDiv r — divide the current state rat by r; fails if r = 0.
    let smDiv (r : rat) : SM<unit> =
        SM (fun st ->
            let (Rat(n, _)) = r
            if n = 0 then None
            else Some ((), div st r))

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

    // calculate ops start — run a sequence of monadic arithmetic operations
    // starting from rational number `start`.
    //
    // Returns Some(finalRat) if all operations succeed, or None if any fails
    // (e.g. a division by zero encountered via smDiv).
    //
    // Uses the StateBuilder CE: each `do! op` runs one operation, threading
    // the state (current rat) forward automatically.
    let calculate (ops : SM<unit> list) (start : rat) : rat option =
        let rec runAll = function
            | [] -> ret ()
            | op :: rest ->
                state {
                    do! op
                    return! runAll rest
                }
        evalSM (runAll ops) start |> Option.map snd
