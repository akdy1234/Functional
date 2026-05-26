module Exam2023
(* If you are importing this into F# interactive then comment out
   the line above and remove the comment for the line bellow.

   Do note that the project will not compile if you do this, but
   it does allow you to work in interactive mode and you can just remove the '='
   to make the project compile again.

   You will also need to load JParsec.fs. Do this by typing
   #load "JParsec.fs"
   in the interactive environment. You may need the entire path.

   Do not remove the module declaration (even though that does work) because you may inadvertently
   introduce indentation errors in your code that may be hard to find if you want
   to switch back to project mode.

   Alternative, keep the module declaration as is, but load ExamInteractive.fsx into the interactive environment
   *)
(*
 module Exam2023 =
 *)

(* ═══════════════════════════════════════════════════════════════
   1: Logic (25%)
   ═══════════════════════════════════════════════════════════════ *)

    type prop =
    | TT
    | FF
    | And of prop * prop
    | Or of prop * prop

    let p1 = And(TT, FF)
    let p2 = Or(TT, FF)
    let p3 = And(Or(TT, And(TT, FF)), TT)
    let p4 = And(Or(TT, And(TT, FF)), Or(FF, And(TT, FF)))

(* ── Question 1.1 ────────────────────────────────────────────── *)

    (* eval : prop -> bool
       Evaluates a proposition to a Boolean value by structural recursion.
       The four rules directly mirror the constructors:
         [[T]]       = true
         [[⊥]]       = false
         [[P ∧ Q]]   = [[P]] && [[Q]]
         [[P ∨ Q]]   = [[P]] || [[Q]]

       WHY NOT TAIL-RECURSIVE?
       `eval p && eval q` is not tail-recursive — after the recursive call
       `eval p` returns, we still need to evaluate `eval q` and apply &&.
       The exam explicitly hints: "Do not try tail recursion. It's significantly
       more complicated." Simple structural recursion is the right approach here. *)
    let rec eval =
        function
        | TT        -> true
        | FF        -> false
        | And(p, q) -> eval p && eval q
        | Or(p, q)  -> eval p || eval q


(* ── Question 1.2 ────────────────────────────────────────────── *)

    (* negate : prop -> prop
       Negates a proposition using De Morgan's laws:
         ¬⊤       = ⊥
         ¬⊥       = ⊤
         ¬(P ∧ Q) = (¬P) ∨ (¬Q)      (De Morgan 1)
         ¬(P ∨ Q) = (¬P) ∧ (¬Q)      (De Morgan 2)

       Note that negate returns a NEW proposition, not a Boolean.
       It does NOT evaluate the proposition — it just restructures it.

       Check negate p1:
         p1 = And(TT, FF)
         negate p1 = Or(negate TT, negate FF) = Or(FF, TT)
         eval (Or(FF, TT)) = false || true = true ✓  (since eval(And(TT,FF)) = false) *)
    let rec negate =
        function
        | TT        -> FF
        | FF        -> TT
        | And(p, q) -> Or(negate p, negate q)
        | Or(p, q)  -> And(negate p, negate q)

    (* implies : prop -> prop -> prop
       Logical implication P → Q is classically defined as (¬P) ∨ Q.
       We encode it here structurally: negate P, then Or with Q.

       Truth table check:
         T → T = (¬T) ∨ T = F ∨ T = T ✓
         T → F = (¬T) ∨ F = F ∨ F = F ✓  (implication is only false T→F)
         F → T = (¬F) ∨ T = T ∨ T = T ✓
         F → F = (¬F) ∨ F = T ∨ F = T ✓ *)
    let implies p q = Or(negate p, q)


(* ── Question 1.3 ────────────────────────────────────────────── *)

    (* forall : ('a -> prop) -> 'a list -> prop
       TAIL-RECURSIVE using an accumulator.

       The spec says:
         ∀[]. f       = ⊤              (empty list → top, identity for And)
         ∀[x1,…,xn]. f = f x1 ∧ … ∧ f xn

       We only require the result to evaluate to the same Boolean value, so
       the order of conjuncts is allowed to vary.

       ACCUMULATOR PATTERN:
       We start acc = TT (identity for And) and fold left:
         aux TT [x1;x2;x3]
       = aux (And(TT, f x1)) [x2;x3]
       = aux (And(And(TT, f x1), f x2)) [x3]
       = aux (And(And(And(TT, f x1), f x2), f x3)) []
       = And(And(And(TT, f x1), f x2), f x3)

       eval of this: true && (eval(f x1)) && (eval(f x2)) && (eval(f x3))
       = eval(f x1) && eval(f x2) && eval(f x3) ✓

       WHY TAIL-RECURSIVE:
       The recursive call `aux (...) xs` is the LAST thing computed in each
       step — there is no pending operation after it. *)
    let forall f lst =
        let rec aux acc = function
            | []      -> acc
            | x :: xs -> aux (And(acc, f x)) xs
        aux TT lst


(* ── Question 1.4 ────────────────────────────────────────────── *)

    (* exists : ('a -> prop) -> 'a list -> prop
       NON-RECURSIVE — uses List.fold from the standard library.

       The spec says:
         ∃[]. f       = ⊥              (empty list → bottom, identity for Or)
         ∃[x1,…,xn]. f = f x1 ∨ … ∨ f xn

       List.fold (fun acc x -> Or(acc, f x)) FF [x1;x2;x3]
       = Or(Or(Or(FF, f x1), f x2), f x3)

       eval: false || (eval(f x1)) || (eval(f x2)) || (eval(f x3))
       = eval(f x1) || eval(f x2) || eval(f x3) ✓

       The exam requires a NON-recursive implementation — List.fold handles
       the iteration internally, so we write no explicit `let rec`. *)
    let exists f lst =
        List.fold (fun acc x -> Or(acc, f x)) FF lst


(* ── Question 1.5 ────────────────────────────────────────────── *)

    (* existsOne : ('a -> prop) -> 'a list -> prop
       Creates a proposition true iff exactly ONE element x in lst satisfies f.

       The spec:
         ∃₁[]. f = ⊥
         ∃₁[x1,…,xn]. f = (f x1 ∧ ¬(f x2) ∧ … ∧ ¬(f xn)) ∨
                           (f x2 ∧ ¬(f x1) ∧ ¬(f x3) ∧ … ∧ ¬(f xn)) ∨ …

       STRATEGY:
       For each index i, produce the conjunction:
         "f(lst[i]) is true AND for every other index j, f(lst[j]) is false"
       Then join all n conjunctions with Or.

       We use List.mapi to get both the value and its index.
       For element at index i, all elements at index j≠i must satisfy ¬(f xj).

       EXAMPLE with [1;2;3] and f = (fun x -> if x % 2 = 0 then TT else FF):
       Index 0 (x=1): f(1)=FF ∧ ¬f(2) ∧ ¬f(3) = FF ∧ FF ∧ TT → evaluates false
       Index 1 (x=2): f(2)=TT ∧ ¬f(1) ∧ ¬f(3) = TT ∧ TT ∧ TT → evaluates true
       Index 2 (x=3): f(3)=FF ∧ ¬f(1) ∧ ¬f(2) = FF ∧ TT ∧ FF → evaluates false
       Overall Or: false ∨ true ∨ false = true ✓ (only 2 is even) *)
    let existsOne f lst =
        let oneAtIndex i =
            List.mapi (fun j xj -> if i = j then f xj else negate (f xj)) lst
            |> List.fold (fun acc p -> And(acc, p)) TT
        lst
        |> List.mapi (fun i _ -> oneAtIndex i)
        |> List.fold (fun acc p -> Or(acc, p)) FF


(* ═══════════════════════════════════════════════════════════════
   2: Code Comprehension (25%)
   ═══════════════════════════════════════════════════════════════ *)

    let rec foo xs ys =
        match xs, ys with
        | _       , []                  -> Some xs
        | x :: xs', y :: ys' when x = y -> foo xs' ys'
        | _       , _                   -> None

    let rec bar xs ys =
        match foo xs ys with
        | Some zs -> bar zs ys
        | None -> match xs with
                  | [] -> []
                  | x :: xs' -> x :: (bar xs' ys)

    let baz (a : string) (b : string) =
        bar [for c in a -> c] [for c in b -> c] |>
        List.fold (fun acc c -> acc + string c) ""

(* ── Question 2.1 ────────────────────────────────────────────── *)

    (*

    Q: What are the types of functions foo, bar, and baz?

    A:
      foo : 'a list -> 'a list -> 'a list option
        Takes two lists of the same element type and returns an option list.
        The type variable 'a has an equality constraint (used in `when x = y`),
        so the full type is 'a list -> 'a list -> 'a list option when 'a : equality.

      bar : 'a list -> 'a list -> 'a list
        Takes two lists and returns a list of the same element type.
        Same equality constraint applies.

      baz : string -> string -> string
        Takes two strings (explicitly annotated) and returns a string.

    Q: What do the functions foo, bar, and baz do.
       Focus on what they do rather than how they do it.

    A:
      foo xs ys:
      Checks if ys is a PREFIX of xs.
      - If ys is empty, it is always a prefix — return Some xs (the remainder).
      - If the heads match (x = y), recurse on the tails.
      - Otherwise ys is NOT a prefix — return None.
      Result: Some(remainder of xs after stripping prefix ys), or None if ys
      is not a prefix of xs.
      Example: foo ['a';'b';'c'] ['a';'b'] = Some ['c']
               foo ['a';'x';'c'] ['a';'b'] = None

      bar xs ys:
      Removes ALL non-overlapping occurrences of the subsequence ys from xs.
      The algorithm scans xs left to right:
      - At the current position, try to strip ys (using foo).
        If successful, skip ys and continue from the remainder.
        If not, keep the current head element and advance by one.
      Note: after a successful match, bar immediately retries at the same
      position (bar zs ys), which handles the case where after removing ys,
      the new prefix also starts a match.
      Example: bar ['a';'b';'a';'b'] ['a';'b'] = []
               bar ['a';'b';'c'] ['a';'b'] = ['c']

      baz a b:
      Removes all occurrences of string b from string a, returning the result
      as a string.
      It converts both strings to char lists, applies bar, then converts back
      to a string using List.fold with string concatenation.

    Q: What would be appropriate names for functions
       foo, bar, and baz?

    A:
      foo → stripPrefix   (checks and strips a prefix from a list)
      bar → removeAll     (removes all occurrences of a sublist)
      baz → removeString  (removes all occurrences of one string from another)

    *)


(* ── Question 2.2 ────────────────────────────────────────────── *)

    (*

    The function baz contains the following three code snippets.

    * A: `[for c in a -> c]`
    * B: `[for c in b -> c]`
    * C: `List.fold (fun acc c -> acc + string c) ""`

    Q: In the context of the baz function, i.e. assuming that `a` and `b` are strings,
       what are the types of snippets A, B, and C and what are they --
       focus on what they do rather than how they do it.

    A:
      A: char list
         A list comprehension that converts string `a` to a list of its characters.
         Iterating over a string in F# yields chars left to right.
         e.g. "hello" → ['h';'e';'l';'l';'o']

      B: char list
         Identical to A but converts string `b` to a char list.

      C: char list -> string
         A partially-applied List.fold that, when given a char list, converts
         it back to a string by accumulating each char with string concatenation.
         The initial accumulator is "" (empty string).
         List.fold applies (fun acc c -> acc + string c) to each element.
         e.g. ['h';'e'] → "" + "h" + "e" = "he"
         Type: char list -> string (the |> makes the list argument implicit)

    Q: Explain the use of the `|>`-operator in the baz function.

    A:
      |> is the PIPELINE operator: `x |> f` = `f x`.
      In baz, it threads the intermediate result through:
        bar [for c in a -> c] [for c in b -> c]
        |> List.fold (fun acc c -> acc + string c) ""
      means:
        List.fold (fun acc c -> acc + string c) "" (bar [..a..] [..b..])
      The result of bar (a char list) is piped as the final argument to
      List.fold, which returns a string.
      Using |> avoids deep nesting of parentheses and makes the data flow
      left-to-right (list → string), which is more readable.

    *)


(* ── Question 2.3 ────────────────────────────────────────────── *)

    (* foo2: non-recursive version of foo, using List.splitAt.

       STRATEGY:
       foo xs ys checks whether ys is a prefix of xs.
       List.splitAt n xs splits xs at position n into (prefix, suffix).
       If the prefix equals ys, ys IS a prefix → return Some suffix.
       Otherwise → None.

       EDGE CASE: if xs is shorter than ys, List.splitAt would throw.
       We guard with `List.length xs < List.length ys`.

       WHY List.splitAt?  The exam hint says to use it.  It gives us the
       first n elements and the rest in one call, exactly what we need. *)
    let foo2 xs ys =
        let n = List.length ys
        if List.length xs < n then None
        else
            let prefix, suffix = List.splitAt n xs
            if prefix = ys then Some suffix else None


(* ── Question 2.4 ────────────────────────────────────────────── *)

    (*

    Q: The function `bar` is not tail recursive. Demonstrate why.
       You do not have to step through the foo-function.

    A:
      Rule: a call is tail-recursive only if it is the VERY LAST operation.
      In bar, the None branch of the inner match is:
          x :: (bar xs' ys)
      The recursive call `bar xs' ys` is NOT the last operation — its result
      is used as the tail of the cons `x :: (...)`.  After `bar xs' ys`
      returns, the runtime still needs to prepend x.  That pending `x :: ...`
      is held on the call stack.

      We evaluate `bar ['a';'c'] ['b']` step by step
      (evaluating foo immediately as permitted):

        bar ['a';'c'] ['b']
      = (* foo ['a';'c'] ['b'] = None, since 'a' ≠ 'b' *)
        'a' :: (bar ['c'] ['b'])
                                        ← PENDING: prepend 'a'
      = 'a' :: ('c' :: (bar [] ['b']))
                                        ← PENDING: prepend 'c'
      = (* foo [] ['b'] = None; [] match *)
        'a' :: ('c' :: [])
      = 'a' :: ['c']
      = ['a';'c']

      In the step `'a' :: (bar ['c'] ['b'])`, the recursive call is INSIDE
      the cons constructor — the cons (::) is still waiting for bar to return.
      This is exactly what it means to be non-tail-recursive.
      For a list of length n, there can be n pending cons operations on the
      call stack simultaneously.

    *)


(* ── Question 2.5 ────────────────────────────────────────────── *)

    (* barTail: tail-recursive version of bar using Continuation-Passing Style.

       WHAT IS A CONTINUATION?
       A continuation `c` represents "the rest of the computation": a function
       that takes the result computed so far and produces the final answer.
       By passing it deeper into the recursion instead of building the result
       on the way back, all recursive calls become tail calls.

       Initially `c = id` (just return the result as-is).

       CASE 1 — foo xs ys = Some zs (ys was stripped from xs):
       We found an occurrence of ys.  Skip it and continue scanning from zs,
       using the SAME continuation (no new pending work).
       This is a tail call: `aux zs c`.

       CASE 2 — foo xs ys = None, xs = x :: xs' (no match at this position):
       Keep x and recurse on xs', but wrap the continuation:
       `fun result -> c (x :: result)` says "when you finish the rest,
       prepend x to it and then call the outer continuation c."
       The recursive call `aux xs' (fun result -> c (x :: result))` is the
       last thing done — tail position. ✓

       CASE 3 — foo xs ys = None, xs = [] (end of list):
       Call the continuation with [] — `c []`.

       TRACE of barTail ['a';'b';'c'] ['a';'b']:
         aux ['a';'b';'c'] id
       = (* foo ['a';'b';'c'] ['a';'b'] = Some ['c'] *)
         aux ['c'] id
       = (* foo ['c'] ['a';'b'] = None, xs = 'c'::[] *)
         aux [] (fun r -> id ('c'::r))
       = (* foo [] ['a';'b'] = None, xs = [] *)
         (fun r -> id ('c'::r)) []
       = id ['c']
       = ['c'] ✓ *)
    let barTail xs ys =
        let rec aux xs c =
            match foo xs ys with
            | Some zs -> aux zs c
            | None ->
                match xs with
                | []       -> c []
                | x :: xs' -> aux xs' (fun result -> c (x :: result))
        aux xs id


(* ═══════════════════════════════════════════════════════════════
   3: Collatz Conjecture (25%)
   ═══════════════════════════════════════════════════════════════ *)

(* ── Question 3.1 ────────────────────────────────────────────── *)

    (* collatz : int -> int list
       Returns the Collatz sequence of x (down to 1).
       Fails with "Non positive number: <x>" if x ≤ 0 at any point.

       THE COLLATZ FUNCTION:
         f(1)  = 1           (terminate)
         f(x)  = f(x/2)      if x is even
         f(x)  = f(3x+1)     if x is odd

       EFFICIENCY REQUIREMENT: the exam says "do NOT append single elements
       to the END of the list" (i.e. xs @ [x] is O(n)).
       Instead we build the list in REVERSE using an accumulator (prepend is
       O(1)) and reverse once at the end (List.rev is O(n) total).

       OVERFLOW DETECTION: for very large inputs, 3x+1 can overflow int and
       produce a negative number. The `if x <= 0 then failwith` guard catches
       this — the exam example shows "collatz 1048574 fails with
       'Non positive number: −1970444364'". *)
    let collatz x =
        let rec aux x acc =
            if x <= 0 then failwith (sprintf "Non positive number: %d" x)
            elif x = 1 then List.rev (1 :: acc)
            elif x % 2 = 0 then aux (x / 2) (x :: acc)
            else aux (3 * x + 1) (x :: acc)
        aux x []


(* ── Question 3.2 ────────────────────────────────────────────── *)

    (* evenOddCollatz : int -> int * int
       Returns (number of even elements, number of odd elements) in collatz(x).

       STRATEGY:
       1. Generate the Collatz sequence with `collatz x`.
       2. Partition into even/odd using List.partition.
       3. Return the two lengths.

       NOTE: 1 is odd (1 % 2 = 1), so collatz 1 = [1] → (0, 1) ✓
       And collatz 2 = [2; 1] → (1, 1) ✓ *)
    let evenOddCollatz x =
        let seq = collatz x
        let evens, odds = List.partition (fun n -> n % 2 = 0) seq
        (List.length evens, List.length odds)


(* ── Question 3.3 ────────────────────────────────────────────── *)

    (* maxCollatz : int -> int -> int * int
       Given a range [x..y], returns (a, len) where a is the number in [x..y]
       with the LONGEST Collatz sequence, and len is that length.
       (Assumes x ≤ y; we can ignore x > y.)

       STRATEGY:
       1. Generate all numbers in [x..y].
       2. Map each to (n, length of collatz(n)).
       3. Find the maximum by the length (snd).
       `List.maxBy snd` returns the entire pair whose second element is maximum. *)
    let maxCollatz x y =
        [x..y]
        |> List.map (fun n -> (n, List.length (collatz n)))
        |> List.maxBy snd


(* ── Question 3.4 ────────────────────────────────────────────── *)

    (* collect : int -> int -> Map<int, Set<int>>
       Groups numbers in [x..y] by the length of their Collatz sequence.
       Returns a Map from sequence-length to the set of numbers with that length.

       STRATEGY (fold over [x..y]):
       For each n:
       - Compute len = length of collatz(n).
       - Find the current set for key len (or empty set if not present).
       - Add n to that set.
       - Update the map.

       EXAMPLE: collect 20 30 →
         [(8, {20;21}); (11, {24;26}); (16, {22;23}); ...]

       WHY Map<int, Set<int>> rather than Map<int, int list>?
       Set gives O(log n) membership, automatic deduplication, and the exam
       output format shows `set [...]` which is the F# Set display. *)
    let collect x y =
        [x..y]
        |> List.fold (fun acc n ->
            let len = List.length (collatz n)
            let s = Map.tryFind len acc |> Option.defaultValue Set.empty
            Map.add len (Set.add n s) acc
        ) Map.empty


(* ── Question 3.5 ────────────────────────────────────────────── *)

    (* parallelMaxCollatz : int -> int -> int -> int
       Finds the number in [x..y] with the longest Collatz sequence, using n
       parallel threads each handling (y-x)/n numbers.

       PARALLEL PATTERN:
       1. Split [x..y] into n equal chunks (each of size (y-x)/n).
          The last chunk absorbs any remainder to handle the "divides evenly"
          assumption.
       2. For each chunk, create an async computation that runs maxCollatz on it.
       3. Run all async computations in parallel with Async.Parallel.
       4. Wait for all results with Async.RunSynchronously (returns an array).
       5. Compare the results from all threads to find the overall maximum.

       Note: `fst (maxCollatz lo hi)` gives us the NUMBER with the max length
       from that chunk.  We then re-compute lengths to find the global max.
       Alternatively, keep track of (number, length) pairs across threads. *)
    let parallelMaxCollatz x y n =
        let chunkSize = (y - x) / n
        [0..n-1]
        |> List.map (fun i ->
            let lo = x + i * chunkSize
            let hi = if i = n - 1 then y else lo + chunkSize - 1
            async { return maxCollatz lo hi }
        )
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Array.maxBy snd
        |> fst


(* ═══════════════════════════════════════════════════════════════
   4: Memory Machine (25%)
   ═══════════════════════════════════════════════════════════════ *)

    type expr =
    | Num    of int              // Integer literal
    | Lookup of expr             // Memory lookup
    | Plus   of expr * expr      // Addition
    | Minus  of expr * expr      // Subtraction

    type stmnt =
    | Assign of expr * expr      // Assign value to memory location
    | While  of expr * prog      // While loop

    and prog = stmnt list        // Programs are sequences of statements

    let (.+.) e1 e2 = Plus(e1, e2)
    let (.-.) e1 e2 = Minus(e1, e2)
    let (.<-.) e1 e2 = Assign (e1, e2)

    // Starting from memory {0, 0, 2, 0}
    let fibProg x =
        [Num 0 .<-. Num x       // {x, 0, 2, 0}
         Num 1 .<-. Num 1       // {x, 1, 2, 0}
         Num 2 .<-. Num 0       // {x, 1, 0, 0}
         While (Lookup (Num 0),
                [Num 0 .<-. Lookup (Num 0) .-. Num 1
                 Num 3 .<-. Lookup (Num 1)
                 Num 1 .<-. Lookup (Num 1) .+. Lookup (Num 2)
                 Num 2 .<-. Lookup (Num 3)
                ]) // after loop {0, fib (x + 1), fib x, fib x}
         ]

(* ── Question 4.1 ────────────────────────────────────────────── *)

    (* DESIGN DECISIONS FOR mem:

       The exam requires:
       - lookup: at most O(log n) time (constant time with mutable structure)
       - assign: at most O(log n) time (constant time with mutable)
       - Memory addresses are non-negative integers; values are integers.

       We use a mutable int array (int[]).
       - Array indexing mem.[i] is O(1) — constant time, well within O(log n).
       - Array.zeroCreate allocates an array of n zeros — memory starts at 0.
       - The exam explicitly allows using imperative mutation for assign:
         "You may use imperative features that update m, and return the same,
          but updated, m to make it run in constant time."

       FUNCTIONAL ASSIGN:
       If we wanted a PURELY functional version, we'd use Array.copy + mutate
       the copy.  But that would be O(n) in time and space.  The exam says we
       CAN mutate for constant time, so we mutate in-place and return m. *)
    type mem = int array

    let emptyMem (n : int) : mem = Array.zeroCreate n

    let lookup (m : mem) (i : int) : int = m.[i]

    let assign (m : mem) (i : int) (v : int) : mem =
        m.[i] <- v
        m


(* ── Question 4.2 ────────────────────────────────────────────── *)

    (* evalExpr, evalStmnt, evalProg are MUTUALLY RECURSIVE because:
       - stmnt includes While of expr * prog (contains a prog)
       - prog is stmnt list
       - evalStmnt needs to call evalProg for the While body
       - evalProg calls evalStmnt for each statement
       → They reference each other → must be declared with `and`.

       evalExpr : mem -> expr -> int
         Evaluates an expression in a given memory state.
         - Num x      → x
         - Lookup e   → evaluate e to get address i, then lookup m[i]
         - Plus(e1,e2)  → eval e1 + eval e2
         - Minus(e1,e2) → eval e1 - eval e2

       evalStmnt : mem -> stmnt -> mem
         Evaluates a statement and returns the (possibly updated) memory.
         - Assign(e1, e2): evaluate e1 to get address i, e2 to get value v,
                           assign m[i] = v.
         - While(e, p): if e evaluates to 0, loop terminates (return m).
                        Otherwise, evaluate the body p AND the While itself
                        again: evalProg m (p @ [While(e, p)]).
                        This is the standard loop unrolling trick.

       evalProg : mem -> prog -> mem
         Evaluates a sequence of statements left-to-right, threading memory
         through each step. *)
    let rec evalExpr (m : mem) =
        function
        | Num x        -> x
        | Lookup e     -> lookup m (evalExpr m e)
        | Plus(e1, e2) -> evalExpr m e1 + evalExpr m e2
        | Minus(e1, e2)-> evalExpr m e1 - evalExpr m e2

    and evalStmnt (m : mem) =
        function
        | Assign(e1, e2) -> assign m (evalExpr m e1) (evalExpr m e2)
        | While(e, p)    ->
            if evalExpr m e = 0 then m
            else evalProg m (p @ [While(e, p)])

    and evalProg (m : mem) =
        function
        | []      -> m
        | s :: rest -> evalProg (evalStmnt m s) rest


(* ── Question 4.3 ────────────────────────────────────────────── *)

    (* THE STATE MONAD FOR MEMORY:
       type StateMonad<'a> = SM of (mem -> ('a * mem) option)

       This monad threads a mem through computations AND can fail.
       The Option wrapper allows lookup/assign to fail on invalid addresses.

       `fail` is provided — it always returns None (monadic failure).
       `bind f (SM a)` runs `a` on the current memory; if it succeeds with
       (x, s'), applies f to x and runs the result on s'.
       If it fails (None), propagates failure.

       lookup2 : int -> StateMonad<int>
         Returns the value at address i if 0 ≤ i < size of memory, else fails.
         We read i from the current mem `s` and return (s.[i], s) unchanged.

       assign2 : int -> int -> StateMonad<unit>
         Updates address i to v if valid, else fails.
         We return ((), assign s i v). *)
    type StateMonad<'a> = SM of (mem -> ('a * mem) option)

    let ret x = SM (fun s -> Some (x, s))
    let fail  = SM (fun _ -> None)
    let bind f (SM a) : StateMonad<'b> =
        SM (fun s ->
            match a s with
            | Some (x, s') ->
                let (SM g) = f x
                g s'
            | None -> None)

    let (>>=) x f = bind f x
    let (>>>=) x y = x >>= (fun _ -> y)

    let evalSM m (SM f) = f m

    let lookup2 (i : int) : StateMonad<int> =
        SM (fun s ->
            if 0 <= i && i < s.Length then Some (s.[i], s)
            else None)

    let assign2 (i : int) (v : int) : StateMonad<unit> =
        SM (fun s ->
            if 0 <= i && i < s.Length then Some ((), assign s i v)
            else None)


(* ── Question 4.4 ────────────────────────────────────────────── *)

    (* evalExpr2, evalStmnt2, evalProg2: monadic versions.
       These replicate the logic of evalExpr/evalStmnt/evalProg but use the
       state monad to abstract the memory block and handle failures.

       The exam says: "you may not break the abstraction of the state monad
       (you may not pattern match on StateMonad nor construct with SM directly)."
       So we only use ret, fail, >>=, >>>= and the monadic lookup2/assign2.

       evalExpr2 : expr -> SM<int>
         Thread the memory through expression evaluation.
         Lookup evaluates the sub-expression to get an address i, then
         calls lookup2 i (which may fail if i is out of range).

       evalStmnt2 : stmnt -> SM<unit>
         Assign: evaluate both expressions to get address and value,
                 then assign2 address value.
         While: evaluate guard; if 0, done (ret ()); else unfold the loop. *)

    type StateBuilder() =

        member this.Bind(f, x)    = bind x f
        member this.Return(x)     = ret x
        member this.ReturnFrom(x) = x
        member this.Combine(a, b) = a >>= (fun _ -> b)

    let state = StateBuilder()

    let rec evalExpr2 =
        function
        | Num x        -> ret x
        | Lookup e     ->
            evalExpr2 e >>= fun i ->
            lookup2 i
        | Plus(e1, e2) ->
            evalExpr2 e1 >>= fun v1 ->
            evalExpr2 e2 >>= fun v2 ->
            ret (v1 + v2)
        | Minus(e1, e2) ->
            evalExpr2 e1 >>= fun v1 ->
            evalExpr2 e2 >>= fun v2 ->
            ret (v1 - v2)

    and evalStmnt2 =
        function
        | Assign(e1, e2) ->
            evalExpr2 e1 >>= fun i ->
            evalExpr2 e2 >>= fun v ->
            assign2 i v
        | While(e, p) ->
            evalExpr2 e >>= fun v ->
            if v = 0 then ret ()
            else evalProg2 (p @ [While(e, p)])

    and evalProg2 =
        function
        | []        -> ret ()
        | s :: rest -> evalStmnt2 s >>>= evalProg2 rest


(* ── Question 4.5 ────────────────────────────────────────────── *)

    open JParsec.TextParser

    let ParseExpr, eref = createParserForwardedToRef<expr>()
    let ParseAtom, aref = createParserForwardedToRef<expr>()

    (* GRAMMAR:
       e, e1, e2 ::=
         x      → Num x      (integer literal)
         [e]    → Lookup e   (memory lookup of address e)
         e1+e2  → Plus(e1, e2)
         e1−e2  → Minus(e1, e2)

       There are no whitespace characters in expressions ("5+3" valid, "5 + 3" not).

       PARSING HIERARCHY:
       - parseAtom handles the two atomic forms: integers and [expr].
         Lookups contain a full sub-expression, so they reference ParseExpr.
       - parseExpr handles binary operators (+, -) between atoms, OR just an atom.

       WHY THIS ORDER?
       parseExpr tries atom OP atom first, then falls back to just atom.
       If we tried atom alone first, we'd never see the operator.

       `pint32 |>> Num`
         Parses an integer and wraps it in Num.
       `pchar '[' >>. ParseExpr .>> pchar ']' |>> Lookup`
         Parses '[', then the inner expression, then ']', wraps in Lookup.
       `ParseAtom .>>. (pchar '+' >>. ParseAtom) |>> (fun (e1,e2) -> Plus(e1,e2))`
         Parses e1, '+', e2, builds Plus.
       `<|>` tries the left parser first; if it fails, tries the right. *)
    let parseAtom =
        pint32 |>> Num
        <|> (pchar '[' >>. ParseExpr .>> pchar ']' |>> Lookup)

    let parseExpr =
        (ParseAtom .>>. (pchar '+' >>. ParseAtom) |>> (fun (e1, e2) -> Plus(e1, e2)))
        <|> (ParseAtom .>>. (pchar '-' >>. ParseAtom) |>> (fun (e1, e2) -> Minus(e1, e2)))
        <|> ParseAtom

    do aref := parseAtom
    do eref := parseExpr
