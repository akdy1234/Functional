module Exam2022_2
(* If you are importing this into F# interactive then comment out
   the line above and remove the comment for the line bellow.

   Do note that the project will not compile if you do this, but
   it does allow you to work in interactive mode and you can just remove the '='
   to make the project compile again.

   Do not remove the module declaration (even though that does work) because you may inadvertently
   introduce indentation errors in your code that may be hard to find if you want
   to switch back to project mode.

   Alternative, keep the module declaration as is, but load ExamInteractive.fsx into the interactive environment
   *)
(*
 module Exam2022_2 =
 *)

(* ═══════════════════════════════════════════════════════════════
   1: Grayscale images (25%)
   ═══════════════════════════════════════════════════════════════ *)

    // A grayscale image is either:
    //   Square v  — a solid block with colour v (0 = black, 255 = white)
    //   Quad(q1, q2, q3, q4) — four equally-sized sub-quadrants, clockwise from top-left:
    //     q1 top-left | q2 top-right
    //     q4 bot-left | q3 bot-right
    type grayscale =
    | Square of uint8
    | Quad of grayscale * grayscale * grayscale * grayscale

    let img =
      Quad (Square 255uy,
            Square 128uy,
            Quad(Square 255uy,
                 Square 128uy,
                 Square 192uy,
                 Square 64uy),
            Square 0uy)

(* ── Question 1.1 ────────────────────────────────────────────── *)

    // maxDepth — returns the maximum nesting depth of Quad nodes in the image.
    //
    // TYPE: grayscale -> int
    //
    // DEFINITION:
    //   Square _       → depth 0  (a leaf, no nesting)
    //   Quad(...)      → 1 + maximum depth among all four sub-quadrants
    //
    // WHY NOT TAIL-RECURSIVE (as the question requires):
    //   The Quad case computes `1 + List.max [maxDepth q1; ...]`.
    //   Each recursive call to maxDepth is INSIDE the list construction —
    //   we cannot return until all four sub-depths are computed.
    //   Multiple pending additions and list operations sit on the stack.
    //
    // Trace: maxDepth img
    //   img = Quad(Square 255, Square 128, Quad(255,128,192,64), Square 0)
    //   = 1 + max [maxDepth(Square 255), maxDepth(Square 128),
    //              maxDepth(Quad(255,128,192,64)), maxDepth(Square 0)]
    //   = 1 + max [0, 0, 1+max[0,0,0,0], 0]
    //   = 1 + max [0, 0, 1, 0]
    //   = 1 + 1
    //   = 2  ✓
    let rec maxDepth =
        function
        | Square _                   -> 0
        | Quad(q1, q2, q3, q4)       ->
            1 + List.max [maxDepth q1; maxDepth q2; maxDepth q3; maxDepth q4]

(* ── Question 1.2 ────────────────────────────────────────────── *)

    // mirror — mirrors a grayscale image around the Y-axis (vertical mirror).
    //
    // TYPE: grayscale -> grayscale
    //
    // A Square has no sub-structure, so mirroring leaves it unchanged.
    //
    // Quadrant layout:
    //   q1 (top-left)  | q2 (top-right)
    //   q4 (bot-left)  | q3 (bot-right)
    //
    // Mirroring around the Y-axis swaps LEFT ↔ RIGHT within each row:
    //   q1 ↔ q2  (top-left and top-right swap)
    //   q4 ↔ q3  (bot-left and bot-right swap)
    //
    // So: Quad(q1, q2, q3, q4) → Quad(mirror q2, mirror q1, mirror q4, mirror q3)
    // The mirroring is applied recursively (hint from the exam).
    //
    // Verify with example:
    //   mirror (Quad(Square 0uy, Square 85uy, Square 170uy, Square 255uy))
    //   = Quad(mirror(Square 85uy), mirror(Square 0uy),
    //          mirror(Square 255uy), mirror(Square 170uy))
    //   = Quad(Square 85uy, Square 0uy, Square 255uy, Square 170uy)  ✓
    let rec mirror =
        function
        | Square _ as sq             -> sq
        | Quad(q1, q2, q3, q4)       ->
            Quad(mirror q2, mirror q1, mirror q4, mirror q3)

(* ── Question 1.3 ────────────────────────────────────────────── *)

    // operate — applies a combining function f bottom-up across a grayscale tree.
    //
    // TYPE: (grayscale -> grayscale -> grayscale -> grayscale -> grayscale)
    //       -> grayscale -> grayscale
    //
    // BEHAVIOUR:
    //   Square v          → Square v  (leaves pass through unchanged)
    //   Quad(a, b, c, d)  → f (operate f a) (operate f b) (operate f c) (operate f d)
    //
    // The recursion descends to the leaves first (bottom-up), then applies f
    // to combine the four recursively-processed sub-quadrants.
    //
    // Example — averaging function:
    //   let average a b c d = match a,b,c,d with
    //     | Square v1, Square v2, Square v3, Square v4 ->
    //         Square (((float v1+float v2+float v3+float v4)/4.0) |> uint8)
    //     | _ -> Quad(a,b,c,d)
    //
    //   operate average img
    //   The inner Quad(255,128,192,64) has four Square leaves, so average produces
    //   Square((255+128+192+64)/4) = Square(159uy).
    //   The outer Quad then has all Square leaves: average(255,128,159,0)
    //   = Square((255+128+159+0)/4) = Square(135uy)  ✓
    let rec operate f =
        function
        | Square _ as sq             -> sq
        | Quad(q1, q2, q3, q4)       ->
            f (operate f q1) (operate f q2) (operate f q3) (operate f q4)

    // mirror2 — non-recursive mirror using operate.
    //
    // TYPE: grayscale -> grayscale
    //
    // operate descends recursively; the combining function just swaps
    // the left and right quadrants (q1↔q2, q3↔q4) at each level.
    // This is identical to the recursive mirror from Q1.2.
    let mirror2 img =
        operate (fun a b c d -> Quad(b, a, d, c)) img

(* ── Question 1.4 ────────────────────────────────────────────── *)

    // compress — bottom-up compression: collapses a Quad whose four sub-quadrants
    // are all the SAME solid colour into a single Square of that colour.
    //
    // TYPE: grayscale -> grayscale
    //
    // ALGORITHM:
    //   1. Recursively compress each sub-quadrant first (bottom-up).
    //   2. If ALL four compressed results are Square with the SAME value → collapse to Square v.
    //   3. Otherwise, keep as Quad with the (possibly compressed) sub-quadrants.
    //
    // Square leaves are already as compressed as possible — no change needed.
    //
    // Examples:
    //   compress (Square 123uy)                        = Square 123uy  (leaf unchanged)
    //   compress (Quad(Sq 123, Sq 123, Sq 123, Sq 123)) = Square 123uy  (all same → collapse)
    //   compress (Quad(Sq 123, Sq 123, Sq 123, Sq 0))   = Quad(...)     (not all same → keep)
    //   compress (Quad(Sq 123, Sq 123, Quad(Sq 123,...,Sq 123), Sq 123))
    //     inner Quad compresses to Square 123uy → outer becomes all-same → Square 123uy  ✓
    let rec compress =
        function
        | Square _ as sq             -> sq
        | Quad(q1, q2, q3, q4)       ->
            let a = compress q1
            let b = compress q2
            let c = compress q3
            let d = compress q4
            match a, b, c, d with
            | Square v1, Square v2, Square v3, Square v4
                when v1 = v2 && v2 = v3 && v3 = v4 -> Square v1
            | _                                      -> Quad(a, b, c, d)


(* ═══════════════════════════════════════════════════════════════
   2: Code Comprehension (25%)
   ═══════════════════════════════════════════════════════════════ *)

    let rec foo f =
        function
        | []               -> []
        | x :: xs when f x -> x :: (foo f xs)
        | _ :: xs          -> foo f xs

    let rec bar fs xs =
        match fs with
        | []       -> xs
        | f :: fs' -> bar fs' (foo f xs)

(* ── Question 2.1 ────────────────────────────────────────────── *)

    (*

    Q: What are the types of functions foo and bar?

    A: foo : ('a -> bool) -> 'a list -> 'a list
       bar : ('a -> bool) list -> 'a list -> 'a list

       Derivation:
         foo takes a predicate f, pattern-matches a list, and returns a list of
         the same element type.  f is applied to each element (f x : bool), and
         the results are collected.  So foo : ('a -> bool) -> 'a list -> 'a list.

         bar takes a list of predicates `fs` and a list `xs`, recurses by applying
         foo to xs with each predicate in turn.  The element type of both lists
         must match.  So bar : ('a -> bool) list -> 'a list -> 'a list.


    Q: What do the functions foo and bar do?
       Focus on what they do rather than how they do it.

    A: foo — FILTERS a list, keeping only elements for which the predicate f returns true.
             This is exactly List.filter.
             e.g. foo (fun x -> x > 2) [1;3;5;2;4] = [3;5;4]

       bar — applies a LIST OF FILTERS sequentially.
             Each predicate in fs is used to filter the list; the result of each
             filtering step is passed as input to the next.
             Only elements that pass ALL predicates survive.
             e.g. bar [(fun x -> x > 1); (fun x -> x < 5)] [1;2;3;4;5]
               step 1: filter by (>1) → [2;3;4;5]
               step 2: filter by (<5) → [2;3;4]


    Q: What would be appropriate names for functions foo and bar?

    A: foo → filter  (it IS List.filter)
       bar → filterAll / applyFilters / multiFilter


    Q: The function foo uses an underscore _ in its third case.
       Is this good coding practice, if so why, and if not why not?

    A: YES, using _ is GOOD coding practice here.

       REASON: By the time we reach the third case `| _ :: xs -> foo f xs`,
       we KNOW the element does NOT satisfy f (otherwise the second case would
       have matched).  We don't need the element's value for anything — we just
       skip it.  Using _ explicitly signals to the reader:
         "I am intentionally ignoring this value."
       Naming it (e.g. `| y :: xs`) would imply we might use y, which we don't.
       The underscore makes the intent clearer and avoids a compiler warning
       about an unused binding.

    *)


(* ── Question 2.2 ────────────────────────────────────────────── *)

    // bar2 — non-recursive version of bar using higher-order functions from List.
    //
    // TYPE: ('a -> bool) list -> 'a list -> 'a list
    //
    // bar applies each predicate in fs sequentially, filtering xs at each step.
    // This is a LEFT FOLD over the predicate list, starting with xs as the accumulator:
    //   List.fold (fun acc f -> List.filter f acc) xs fs
    //
    // We may NOT use foo (the question forbids it) and may NOT write recursive helpers.
    // List.fold and List.filter are both higher-order functions from the List library.
    //
    // Why it works:
    //   fold starts with acc = xs.
    //   For each predicate f in fs, it replaces acc with List.filter f acc.
    //   After all predicates have been applied, acc contains only elements
    //   that passed every filter — identical to bar fs xs.
    let bar2 fs xs = List.fold (fun acc f -> List.filter f acc) xs fs

(* ── Question 2.3 ────────────────────────────────────────────── *)

    // baz — creates a COMBINED PREDICATE that holds iff ALL individual predicates hold.
    //
    // TYPE: ('a -> bool) list -> 'a -> bool
    //
    // REQUIREMENT: bar fs xs = foo (baz fs) xs for all fs and xs.
    //
    // foo (baz fs) xs keeps element x if (baz fs) x is true.
    // bar fs xs keeps element x if x passes every predicate in fs.
    // Therefore (baz fs) x must be true iff x passes every predicate in fs.
    //
    // List.forall f [p1;p2;...] = p1(x) && p2(x) && ... — returns true iff ALL hold.
    //
    // Verify: foo (baz [(>1);(<5)]) [1;2;3;4;5]
    //   baz [(>1);(<5)] = (fun x -> List.forall (fun f -> f x) [(>1);(<5)])
    //   Keeps x where x>1 AND x<5 → [2;3;4]  ✓
    //   bar  [(>1);(<5)] [1;2;3;4;5] = [2;3;4]  ✓ (same result)
    let baz fs x = List.forall (fun f -> f x) fs

(* ── Question 2.4 ────────────────────────────────────────────── *)

    (*

    Q: Only one of foo and bar is tail recursive. Which one?
       Demonstrate why the other is NOT tail recursive.

    A: BAR is tail recursive; FOO is NOT.

       bar: the recursive call `bar fs' (foo f xs)` is the LAST expression in
       the `f :: fs'` case — nothing happens after it returns.  This IS a tail call.
       The F# compiler can reuse the stack frame (tail-call optimisation). ✓

       FOO is NOT tail recursive.  Proof by evaluation:

       Evaluate foo (fun x -> x > 0) [1; -1; 2]:

         foo f [1; -1; 2]
         = 1 :: (foo f [-1; 2])           (* f 1 = true → keep, recurse *)
               ↑ PENDING CONS OPERATION
               The result of `foo f [-1; 2]` must come back before
               we can cons 1 onto it.  The current frame cannot be
               discarded — it holds the pending `1 ::`.

         = 1 :: (foo f [2])               (* f (-1) = false → skip *)
         = 1 :: (2 :: (foo f []))         (* f 2 = true → keep, recurse *)
                   ↑ another pending cons
         = 1 :: (2 :: [])
         = [1; 2]

       At the step `1 :: (foo f [-1; 2])`:
       The recursive call `foo f [-1; 2]` is the RIGHT OPERAND of `::`.
       After it returns, we still need to perform the `1 ::` operation.
       That is pending work — NOT a tail call.
       For a list of length n, up to n pending cons operations accumulate on the stack.

    *)

(* ── Question 2.5 ────────────────────────────────────────────── *)

    // fooTail — tail-recursive version of foo using Continuation-Passing Style (CPS).
    //
    // (barTail is NOT needed because bar is already tail recursive — see Q2.4.)
    //
    // THE PROBLEM WITH foo: `x :: (foo f xs)` leaves a pending cons on the stack.
    //
    // CPS SOLUTION: pass a CONTINUATION `cont` representing "what to do with the
    // final filtered list."  Instead of prepending x AFTER the recursive call,
    // we capture the prepend in a new closure passed to the next recursive call.
    //
    // BASE CASE  ([] → cont []):
    //   Empty list → result is [].  Apply the continuation to [].
    //
    // RECURSIVE CASE when f x is TRUE  (x :: xs → keep x):
    //   Recurse on xs, passing a new continuation that prepends x to the result.
    //   The call `aux xs (fun result -> cont (x :: result))` is in tail position. ✓
    //
    // RECURSIVE CASE when f x is FALSE  (skip x):
    //   Recurse on xs with the SAME continuation (nothing new to prepend).
    //   The call `aux xs cont` is in tail position. ✓
    //
    // TRACE: fooTail (fun x -> x > 0) [1; -1; 2]
    //   aux id [1; -1; 2]
    //   = aux (fun r -> id (1::r)) [-1; 2]           (* f 1 = true, keep *)
    //   = aux (fun r -> id (1::r)) [2]               (* f(-1) = false, skip; cont unchanged *)
    //   = aux (fun r -> (fun r -> id(1::r)) (2::r)) []  (* f 2 = true, keep *)
    //   = (outermost cont) []
    //   = (fun r -> id (1::r)) (2::[])
    //   = id [1; 2]
    //   = [1; 2]  ✓
    let fooTail f lst =
        let rec aux cont =
            function
            | []               -> cont []
            | x :: xs when f x -> aux (fun result -> cont (x :: result)) xs
            | _ :: xs          -> aux cont xs
        aux id lst

    // barTail is already tail recursive (see Q2.4 explanation).
    // Only implementing the one that is NOT already tail recursive, as per the question.
    let barTail fs xs = bar fs xs


(* ═══════════════════════════════════════════════════════════════
   3: Guess a number (25%)
   ═══════════════════════════════════════════════════════════════ *)

    type guessResult = Lower | Higher | Equal
    type oracle =
        { max : int
          f : int -> guessResult }

(* ── Question 3.1 ────────────────────────────────────────────── *)

    // validOracle — returns true iff the oracle is valid according to the specification.
    //
    // TYPE: oracle -> bool
    //
    // A VALID oracle has a target number t in [1..max] such that:
    //   1. f t = Equal                                     (exactly one Equal)
    //   2. f x = Higher for all x in [1..t-1]             (too low → Higher)
    //   3. f x = Lower  for all x in [t+1..max]           (too high → Lower)
    //
    // STRATEGY:
    //   Find all x in [1..max] where f x = Equal.
    //   There must be EXACTLY ONE such x (the target).
    //   Then check that all numbers below it return Higher and all above return Lower.
    //
    // Example: validOracle {max=10; f = fun _ -> Lower} = false
    //   (no Equal found — fails immediately)
    // Example: validOracle {max=10; f = fun x -> if x=5 then Equal elif x<5 then Higher else Lower}
    //   = true  ✓
    let validOracle (o : oracle) : bool =
        let equalNums = List.filter (fun x -> o.f x = Equal) [1..o.max]
        match equalNums with
        | [target] ->
            List.forall (fun x -> o.f x = Higher) [1..target - 1] &&
            List.forall (fun x -> o.f x = Lower)  [target + 1..o.max]
        | _ -> false

(* ── Question 3.2 ────────────────────────────────────────────── *)

    // randomOracle — creates an oracle for a random target number in [1..m].
    //
    // TYPE: int -> int option -> oracle
    //
    // The optional seed `oseed`:
    //   None      → use System.Random() with a random seed (different each time)
    //   Some seed → use System.Random(seed) for reproducible results
    //
    // CRITICAL: generate the random number r BEFORE constructing the oracle record.
    // If r were generated inside the oracle function `f`, a new number would be
    // drawn on EVERY call to f — the target would change between guesses!
    //
    // The oracle returns:
    //   Equal  when x = r
    //   Higher when x < r  (the guess is too low — go higher)
    //   Lower  when x > r  (the guess is too high — go lower)
    let randomOracle (m : int) (oseed : int option) : oracle =
        let rng =
            match oseed with
            | None      -> System.Random()
            | Some seed -> System.Random(seed)
        let r = rng.Next(1, m + 1)
        { max = m
          f = fun x ->
                  if   x = r then Equal
                  elif x < r then Higher
                  else            Lower }

(* ── Question 3.3 ────────────────────────────────────────────── *)

    // findNumber — finds the target number using binary search.
    //
    // TYPE: oracle -> int list
    //   Returns the LIST OF ALL GUESSES made, in order (including the final correct guess).
    //   The list length must not exceed ⌈log₂(max)⌉.
    //
    // BINARY SEARCH ALGORITHM:
    //   Given range [a, b], guess g = a + (b-a)/2  (midpoint, integer division).
    //   f g = Equal  → done, g is the answer.
    //   f g = Higher → target is in [g+1, b]; recurse with tighter lower bound.
    //   f g = Lower  → target is in [a, g-1]; recurse with tighter upper bound.
    //
    // We build the list of guesses in reverse (accumulator pattern) and reverse at the end.
    //
    // Correctness: binary search halves the search space each step.
    // For max = 10 → at most 4 guesses; for max = 1 000 000 → at most 20.  ✓
    let findNumber (o : oracle) : int list =
        let rec search a b acc =
            let g = a + (b - a) / 2
            match o.f g with
            | Equal  -> List.rev (g :: acc)
            | Higher -> search (g + 1) b (g :: acc)
            | Lower  -> search a (g - 1) (g :: acc)
        search 1 o.max []

(* ── Question 3.4 ────────────────────────────────────────────── *)

    // evilOracle — an adversarial oracle that maximises the user's search space.
    //
    // TYPE: int -> int option -> oracle
    //
    // STRATEGY:
    //   The evil oracle does NOT pick a target number in advance.
    //   Instead, it maintains a MUTABLE range [lo, hi] of still-possible target values.
    //   For each guess x:
    //     - If hi-x > x-lo (more numbers above x): return Higher, narrow to [x+1, hi]
    //     - If x-lo > hi-x (more numbers below x): return Lower,  narrow to [lo, x-1]
    //     - If equal: randomly return Higher or Lower (using the RNG), narrow accordingly
    //     - If lo = hi: return Equal (forced — cannot lie any further)
    //
    // The oracle NEVER lies (it always narrows consistently), so validOracle returns true.
    // But it always maximises the remaining search space, forcing the maximum number of guesses.
    //
    // IMPORTANT: initialise the RNG and mutable variables OUTSIDE the oracle function `f`.
    // If they were inside `f`, they would reset on every call to `f`.
    let evilOracle (m : int) (oseed : int option) : oracle =
        let rng =
            match oseed with
            | None      -> System.Random()
            | Some seed -> System.Random(seed)
        let mutable lo = 1
        let mutable hi = m
        { max = m
          f = fun x ->
                  if lo = hi then Equal
                  else
                      let higherCount = hi - x   // numbers strictly above x still possible
                      let lowerCount  = x - lo   // numbers strictly below x still possible
                      if higherCount > lowerCount then
                          lo <- x + 1
                          Higher
                      elif lowerCount > higherCount then
                          hi <- x - 1
                          Lower
                      else
                          if rng.Next(2) = 0 then
                              lo <- x + 1
                              Higher
                          else
                              hi <- x - 1
                              Lower }

(* ── Question 3.5 ────────────────────────────────────────────── *)

    // parFindNumber — runs findNumber on multiple oracles IN PARALLEL.
    //
    // TYPE: oracle list -> int list list
    //   Each oracle gets its own findNumber call; results are collected as a list of lists.
    //
    // PARALLEL PATTERN (standard F# async):
    //   1. List.map each oracle to an async workflow  `async { return findNumber o }`.
    //   2. Async.Parallel bundles all workflows into one that runs them concurrently.
    //   3. Async.RunSynchronously blocks until all finish, returns results as an array.
    //   4. Array.toList converts the result array to a list.
    let parFindNumber (os : oracle list) : int list list =
        os
        |> List.map (fun o -> async { return findNumber o })
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Array.toList


(* ═══════════════════════════════════════════════════════════════
   4: Assembly (25%)
   ═══════════════════════════════════════════════════════════════ *)

    type register = R1 | R2 | R3
    type address = uint

    type assembly =
    | MOVI of register * int
    | MULT of register * register * register
    | SUB of register * register * register
    | JGTZ of register * address

    let factorial x =           // Address
        [MOVI (R1, 1)           // 0
         MOVI (R2, x)           // 1
         MOVI (R3, 1)           // 2
         MULT (R1, R1, R2)      // 3 (Loop starts here)
         SUB  (R2, R2, R3)      // 4
         JGTZ (R2, 3u)]         // 5 (Loop ends here)

(* ── Question 4.1 ────────────────────────────────────────────── *)

    // DESIGN CHOICE: Map<uint, assembly>
    //
    // The exam requires O(log n) lookup time from address → command.
    //
    // A list gives O(n) lookup (must traverse from the head).
    // A Map (balanced binary search tree) gives O(log n) lookup. ✓
    //
    // The address is the 0-based index in the original assembly list.
    // `List.mapi` pairs each command with its index; `Map.ofList` builds the Map.
    type program = Map<uint, assembly>

    // assemblyToProgram — converts a list of assembly commands to a program Map.
    //
    // TYPE: assembly list -> program
    //
    // List.mapi f xs pairs each element with its index: [(0,cmd0);(1,cmd1);...].
    // We convert each index i to uint (the address type) and build a Map.
    //
    // Example: assemblyToProgram (factorial 10)
    //   produces Map {0u→MOVI(R1,1); 1u→MOVI(R2,10); ...; 5u→JGTZ(R2,3u)}
    let assemblyToProgram (cmds : assembly list) : program =
        cmds
        |> List.mapi (fun i cmd -> (uint i, cmd))
        |> Map.ofList

(* ── Question 4.2 ────────────────────────────────────────────── *)

    // type state — the full execution state of the assembly machine.
    //
    // Contains the three pieces of information required by the exam:
    //   pc   : uint     — program counter (index of the current instruction)
    //   r1/r2/r3 : int  — the three registers (initial value 0)
    //   prog : program  — the compiled program (Map from address to command)
    //
    // Using a record type gives named field access and clean `with`-update syntax.
    type state =
        { pc   : uint
          r1   : int
          r2   : int
          r3   : int
          prog : program }

    // emptyState — creates an initial execution state from a list of assembly commands.
    //
    // TYPE: assembly list -> state
    //
    // The program counter starts at address 0u (the first instruction).
    // All three registers are initialised to 0.
    // The program is built by assemblyToProgram.
    let emptyState (cmds : assembly list) : state =
        { pc   = 0u
          r1   = 0
          r2   = 0
          r3   = 0
          prog = assemblyToProgram cmds }

(* ── Question 4.3 ────────────────────────────────────────────── *)

    // Pure (non-monadic) helper functions that read or update the raw state record.
    // These are used by Q4.4's monadic wrappers.

    // setRegister — returns a new state with register reg set to value v.
    // TYPE: register -> int -> state -> state
    let setRegister (reg : register) (v : int) (s : state) : state =
        match reg with
        | R1 -> { s with r1 = v }
        | R2 -> { s with r2 = v }
        | R3 -> { s with r3 = v }

    // getRegister — reads the current value of a register.
    // TYPE: register -> state -> int
    let getRegister (reg : register) (s : state) : int =
        match reg with
        | R1 -> s.r1
        | R2 -> s.r2
        | R3 -> s.r3

    // setProgramCounter — returns a new state with the PC set to addr.
    // TYPE: uint -> state -> state
    let setProgramCounter (addr : uint) (s : state) : state = { s with pc = addr }

    // getProgramCounter — reads the current program counter value.
    // TYPE: state -> uint
    let getProgramCounter (s : state) : uint = s.pc

    // getProgram — reads the program Map from the state.
    // TYPE: state -> program
    let getProgram (s : state) : program = s.prog

(* ── Question 4.4 ────────────────────────────────────────────── *)

    // StateMonad<'a> = SM of (state -> 'a * state)
    //
    // This state monad does NOT use option — every computation succeeds.
    // A computation takes the current state and produces (result, newState).
    type StateMonad<'a> = SM of (state -> 'a * state)

    let ret x = SM (fun s -> x, s)
    let bind f (SM a) : StateMonad<'b> =
        SM (fun s ->
            let x, s' = a s
            let (SM g) = f x
            g s')

    let (>>=) x f = bind f x
    let (>>>=) x y = x >>= (fun _ -> y)

    // evalSM prog m — initialises the state from the assembly list and runs m.
    // Returns (result, finalState).
    let evalSM prog (SM f) = f (emptyState prog)

    // setReg — monadic: set register reg to value v.
    // TYPE: register -> int -> StateMonad<unit>
    let setReg (reg : register) (v : int) : StateMonad<unit> =
        SM (fun s -> ((), setRegister reg v s))

    // getReg — monadic: read the value stored in register reg.
    // TYPE: register -> StateMonad<int>
    let getReg (reg : register) : StateMonad<int> =
        SM (fun s -> (getRegister reg s, s))

    // setPC — monadic: set the program counter to addr.
    // TYPE: uint -> StateMonad<unit>
    let setPC (addr : uint) : StateMonad<unit> =
        SM (fun s -> ((), setProgramCounter addr s))

    // incPC — monadic: advance the program counter by 1.
    // TYPE: StateMonad<unit>
    // Used after every non-jump instruction to move to the next command.
    let incPC : StateMonad<unit> =
        SM (fun s -> ((), setProgramCounter (s.pc + 1u) s))

    // lookupCmd — monadic: fetch the assembly command at the current PC.
    // TYPE: StateMonad<assembly>
    // Uses Map.find — will throw if the PC is out of range (handled in runProgram).
    let lookupCmd : StateMonad<assembly> =
        SM (fun s -> (Map.find s.pc s.prog, s))

(* ── Question 4.5 ────────────────────────────────────────────── *)

    // StateBuilder — maps F# CE keywords to monadic functions.
    //   let! x = m   → Bind(m, fun x -> ...)   run m, bind result to x
    //   do! m        → Bind(m, fun () -> ...)   run m for its side-effect
    //   return x     → Return(x) = ret x         wrap x in the monad
    //   return! m    → ReturnFrom(m) = m          return an already-monadic value
    //
    // NOTE on Bind parameter order: F# passes Bind(computation, continuation),
    // but the template writes Bind(f, x). Here f IS the computation and x IS
    // the continuation, so the body calls `bind x f`.
    type StateBuilder() =

        member this.Bind(f, x)    = bind x f
        member this.Return(x)     = ret x
        member this.ReturnFrom(x) = x
        member this.Combine(a, b) = a >>= (fun _ -> b)

    let state = new StateBuilder()

    // runProgram — executes an assembly program and returns the final machine state.
    //
    // TYPE: assembly list -> state
    //
    // EXECUTION LOOP:
    //   Each iteration reads the current PC, looks up the command in the program map,
    //   executes it (updating registers and PC), then recurses.
    //   The loop terminates when the PC falls outside the program map (past the end).
    //
    // TERMINATION DETECTION:
    //   We use `safeGetCmd` which calls Map.tryFind (instead of lookupCmd's Map.find).
    //   When tryFind returns None, the PC is past the last instruction — halt.
    //   This happens naturally after the last non-jumping instruction increments PC.
    //
    // INSTRUCTION SEMANTICS:
    //   MOVI(r, v)       → r := v, PC++
    //   MULT(r1, r2, r3) → r1 := r2 * r3, PC++
    //   SUB(r1, r2, r3)  → r1 := r2 - r3, PC++
    //   JGTZ(r, addr)    → if r > 0 then PC := addr else PC++
    //
    // WHY >>= INSTEAD OF STATE CE:
    //   The StateBuilder does not define a `Delay` method.  Without Delay, F# cannot
    //   compile CE blocks that contain `if/match` where BOTH branches use CE keywords
    //   (do!, return!, etc.).  We use explicit >>= operators instead — they are
    //   semantically identical but don't require Delay.
    //
    // evalSM cmds (loop ()) returns (unit, finalState); snd extracts the final state.
    let runProgram (cmds : assembly list) : state =
        let safeGetCmd : StateMonad<assembly option> =
            SM (fun s -> (Map.tryFind s.pc s.prog, s))
        let rec loop () : StateMonad<unit> =
            safeGetCmd >>= fun cmd ->
                match cmd with
                | None -> ret ()
                | Some (MOVI (r, v)) ->
                    setReg r v >>>= incPC >>>= loop ()
                | Some (MULT (r1, r2, r3)) ->
                    getReg r2 >>= fun v2 ->
                    getReg r3 >>= fun v3 ->
                    setReg r1 (v2 * v3) >>>= incPC >>>= loop ()
                | Some (SUB (r1, r2, r3)) ->
                    getReg r2 >>= fun v2 ->
                    getReg r3 >>= fun v3 ->
                    setReg r1 (v2 - v3) >>>= incPC >>>= loop ()
                | Some (JGTZ (r, addr)) ->
                    getReg r >>= fun v ->
                    (if v > 0 then setPC addr else incPC) >>>= loop ()
        evalSM cmds (loop ()) |> snd
