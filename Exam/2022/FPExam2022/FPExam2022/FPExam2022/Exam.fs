module Exam2022
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
 module Exam2022 =
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

    // countWhite — returns the number of white (255uy) leaf cells in the image.
    //
    // TYPE: grayscale -> int
    //
    // CASES:
    //   Square 255uy        → 1  (this IS a white cell)
    //   Square _            → 0  (any other solid colour)
    //   Quad(q1,q2,q3,q4)  → sum countWhite over all four sub-quadrants
    //
    // WHY NOT TAIL-RECURSIVE (as the question requires):
    //   The Quad case is  countWhite q1 + countWhite q2 + countWhite q3 + countWhite q4.
    //   After each recursive call, the result must still be added to the others.
    //   There are three pending additions per Quad node — the stack frame cannot be
    //   discarded until those additions complete.
    //
    // Trace: countWhite img
    //   = countWhite(Square 255) + countWhite(Square 128)
    //     + countWhite(Quad(255,128,192,64)) + countWhite(Square 0)
    //   = 1 + 0 + (1 + 0 + 0 + 0) + 0
    //   = 2  ✓
    let rec countWhite =
        function
        | Square 255uy               -> 1
        | Square _                   -> 0
        | Quad(q1, q2, q3, q4)       ->
            countWhite q1 + countWhite q2 + countWhite q3 + countWhite q4

(* ── Question 1.2 ────────────────────────────────────────────── *)

    // rotateRight — rotates a grayscale image 90 degrees clockwise.
    //
    // TYPE: grayscale -> grayscale
    //
    // A Square has no sub-structure, so rotation leaves it unchanged.
    //
    // For Quad, the quadrants sit in this layout:
    //   q1 (top-left) | q2 (top-right)
    //   q4 (bot-left) | q3 (bot-right)
    //
    // After a 90° clockwise rotation each quadrant moves to the NEXT clockwise position:
    //   q1 → q2 position (old top-left becomes top-right)
    //   q2 → q3 position (old top-right becomes bot-right)
    //   q3 → q4 position (old bot-right becomes bot-left)
    //   q4 → q1 position (old bot-left becomes top-left)
    //
    // So the new Quad is:  Quad(old_q4, old_q1, old_q2, old_q3)
    // Each sub-quadrant is rotated recursively (hint from the exam).
    //
    // Verify with example:
    //   rotateRight (Quad(Square 0uy, Square 85uy, Square 170uy, Square 255uy))
    //   = Quad(rotateRight(Square 255uy), rotateRight(Square 0uy),
    //          rotateRight(Square 85uy),  rotateRight(Square 170uy))
    //   = Quad(Square 255uy, Square 0uy, Square 85uy, Square 170uy)  ✓
    let rec rotateRight =
        function
        | Square _ as sq             -> sq
        | Quad(q1, q2, q3, q4)       ->
            Quad(rotateRight q4, rotateRight q1, rotateRight q2, rotateRight q3)

(* ── Question 1.3 ────────────────────────────────────────────── *)

    // map — applies a mapper function to every leaf (Square) value in the tree.
    //
    // TYPE: (uint8 -> grayscale) -> grayscale -> grayscale
    //
    // The mapper receives a uint8 value and returns a grayscale — it may return
    // either a Square or a Quad (more general than a simple colour transform).
    //
    // RECURSIVE STRUCTURE mirrors the type:
    //   Square v          → mapper v             (apply mapper to the leaf)
    //   Quad(q1,q2,q3,q4) → rebuild with map applied to each sub-quadrant
    let rec map mapper =
        function
        | Square v                   -> mapper v
        | Quad(q1, q2, q3, q4)       ->
            Quad(map mapper q1, map mapper q2, map mapper q3, map mapper q4)

    // bitmap — converts a grayscale image to strict black-and-white using map.
    //
    // TYPE: grayscale -> grayscale
    //
    // THRESHOLD RULE:
    //   pixel value ≤ 127uy → black (Square 0uy)
    //   pixel value > 127uy → white (Square 255uy)
    //
    // NON-RECURSIVE: all recursion is inside `map`.
    // The lambda converts each uint8 to the appropriate black/white Square.
    //
    // Example: bitmap (Square 120uy) = Square 0uy   (120 ≤ 127 → black)
    //          bitmap (Square 150uy) = Square 255uy  (150 > 127 → white)
    let bitmap img =
        map (fun v -> if v <= 127uy then Square 0uy else Square 255uy) img

(* ── Question 1.4 ────────────────────────────────────────────── *)

    // fold — left fold over every leaf in the grayscale tree, in quadrant order.
    //
    // TYPE: ('a -> uint8 -> 'a) -> 'a -> grayscale -> 'a
    //
    // Mirrors List.fold but for the grayscale tree.
    // `folder` is applied to the accumulator and each leaf value in turn.
    // Quadrants are visited in order q1, q2, q3, q4 (clockwise from top-left).
    //
    // DERIVATION:
    //   fold f acc (Square v)                   = f acc v
    //   fold f acc (Quad(Square v1,…,Square v4)) = f (f (f (f acc v1) v2) v3) v4
    //
    //   For nested quads, each Quad is unfolded recursively before its accumulator
    //   is passed to the next sibling.
    //
    // Verify: fold (fun a x -> a + int x) 0 img
    //   visits 255, 128, 255, 128, 192, 64, 0 → sum = 1022  ✓
    let rec fold folder acc =
        function
        | Square v                   -> folder acc v
        | Quad(q1, q2, q3, q4)       ->
            let acc1 = fold folder acc  q1
            let acc2 = fold folder acc1 q2
            let acc3 = fold folder acc2 q3
            fold folder acc3 q4

    // countWhite2 — same as countWhite, rewritten using fold.
    //
    // TYPE: grayscale -> int
    //
    // NON-RECURSIVE: all recursion is inside fold.
    // The folder adds 1 whenever the current leaf value is 255uy.
    let countWhite2 img =
        fold (fun acc v -> if v = 255uy then acc + 1 else acc) 0 img


(* ═══════════════════════════════════════════════════════════════
   2: Code Comprehension (25%)
   ═══════════════════════════════════════════════════════════════ *)

    let rec foo =
        function
        | 0 -> ""
        | x when x % 2 = 0 -> foo (x / 2) + "0"
        | x when x % 2 = 1 -> foo (x / 2) + "1"

    let rec bar =
        function
        | []      -> []
        | x :: xs -> (foo x) :: (bar xs)

(* ── Question 2.1 ────────────────────────────────────────────── *)

    (*

    Q: What are the types of functions foo and bar?

    A: foo : int -> string
       bar : int list -> string list

       Derivation:
         foo pattern-matches an int.  The base case 0 returns "".
         The recursive cases return `foo (x/2) + "0"` / `+ "1"` — string concatenation.
         Therefore foo : int -> string.

         bar takes a list, recurses on its tail, and builds `(foo x) :: (bar xs)`.
         Since foo x : string, the list elements are strings.
         Therefore bar : int list -> string list.


    Q: What do the functions foo and bar do?
       Focus on what they do rather than how they do it.

    A: foo — converts a NON-NEGATIVE INTEGER to its BINARY STRING REPRESENTATION.
             The recursion emits bits from least-significant to most-significant;
             they are appended to the right as the call stack unwinds, producing
             the standard big-endian binary notation.

             Examples:
               foo 0  = ""       (special: the empty string, not "0")
               foo 1  = "1"
               foo 6  = "110"    (6 = 4+2 = 110₂)
               foo 10 = "1010"

       bar — maps foo over an int list, converting every integer to its binary
             string representation.
             e.g. bar [6; 10] = ["110"; "1010"]


    Q: What would be appropriate names for functions foo and bar?

    A: foo → toBinary / intToBinaryString / decToBin
       bar → mapToBinary / intsToBinaryStrings


    Q: The function foo does not return reasonable results for all possible inputs.
       What requirements must we have on the input to foo in order to get reasonable results?

    A: x must be non-negative: x ≥ 0.

       REASON — negative inputs:
         In F#, the `%` operator for negative numbers returns 0 or a NEGATIVE remainder.
         For example, -3 % 2 = -1 (not 1).
         The guard `x % 2 = 1` never matches when x is negative and odd.
         No case matches, so F# raises MatchFailureException at runtime.
         For negative EVEN numbers (e.g. -6), the recursion f(-6) → f(-3) → MatchFailure,
         so it still crashes.

       NOTE on x = 0:
         foo 0 = "" (the empty string, not "0").  This is technically unreasonable
         if the caller expects "0" for the number zero, but is within the intended
         contract of the function for x ≥ 0.

    *)


(* ── Question 2.2 ────────────────────────────────────────────── *)

    (*
    The function foo compiles with a warning.

    Q: What warning and why?

    A: WARNING: "Incomplete pattern matches on this expression."

       REASON:
       The last two cases of foo are:
         | x when x % 2 = 0 -> ...
         | x when x % 2 = 1 -> ...

       Both use a `when` guard.  Guards can FAIL — even though the pattern `x`
       always matches any int, the guard condition might be false.  The compiler
       cannot prove statically that `x % 2 = 0 OR x % 2 = 1` covers all integers
       (it does not reason about modular arithmetic).  It sees two potentially-failing
       guards and warns that some input might reach a missing case and raise
       MatchFailureException.

       FIX: Remove the guard from the LAST arm, replacing `x when x % 2 = 1`
       with a plain `x` (no guard).  A pattern without a guard is irrefutable —
       it always matches.  The compiler now knows: "if neither of the first two
       arms matched, the third arm is a guaranteed catch-all."

       This is safe: by the time we reach the third arm we know x ≠ 0 (first case
       didn't match) and x % 2 ≠ 0 (second case didn't match), so x % 2 must be 1
       for non-negative inputs.
    *)

    let rec foo2 =
        function
        | 0 -> ""
        | x when x % 2 = 0 -> foo2 (x / 2) + "0"
        | x                 -> foo2 (x / 2) + "1"

(* ── Question 2.3 ────────────────────────────────────────────── *)

    // bar2 — non-recursive version of bar using a higher-order function from List.
    //
    // TYPE: int list -> string list
    //
    // List.map f xs applies f to every element of xs and collects the results.
    // This is exactly what bar does: apply foo to each element.
    // All recursion is hidden inside List.map — bar2 itself contains no `rec`.
    let bar2 lst = List.map foo lst

(* ── Question 2.4 ────────────────────────────────────────────── *)

    (*

    Q: Neither foo nor bar is tail recursive. Pick one and explain why.

    A: We pick FOO.

       DEFINITION: a recursive call is in "tail position" if it is the VERY LAST
       operation — nothing remains to be computed after it returns.
       Pending work after the call means it is NOT a tail call.

       Evaluate foo 6 step by step:

         foo 6
         = foo (6/2) + "0"           (* 6 % 2 = 0 *)
         = foo 3 + "0"
         = (foo (3/2) + "1") + "0"   (* 3 % 2 = 1 *)
         = (foo 1 + "1") + "0"
         = ((foo (1/2) + "1") + "1") + "0"   (* 1 % 2 = 1 *)
         = ((foo 0 + "1") + "1") + "0"
         = (("" + "1") + "1") + "0"          (* base case *)
         = ("1" + "1") + "0"
         = "11" + "0"
         = "110"

       At the step `foo 3 + "0"`:
       The recursive call `foo 3` is the ARGUMENT to `+`, not the final result.
       After foo 3 returns, we still need to concatenate "+ "0"".
       That pending concatenation keeps the current frame on the call stack.
       For a number with n bits, n pending concatenations accumulate — not tail-recursive.


    Q: Even though neither foo nor bar is tail recursive, only one of them risks
       overflowing the stack. Which one, and why does the other one not risk it?

    A: BAR risks stack overflow; FOO does not.

       foo: recursion depth = number of binary digits of x = ⌊log₂(x)⌋ + 1.
            For any int (up to 2^63 on 64-bit), that is at most ~63 frames.
            63 stack frames is far below the overflow threshold (typically millions).
            foo is safe regardless of the input value.

       bar: recursion depth = length of the input list.
            A list with 100 000 elements creates 100 000 stack frames.
            This will cause a StackOverflowException on typical runtimes.
            bar's recursion depth grows without any fixed bound.

    *)

(* ── Question 2.5 ────────────────────────────────────────────── *)

    // fooTail — tail-recursive version of foo using an ACCUMULATOR.
    //
    // THE PROBLEM WITH foo: `foo (x/2) + "0"` leaves a pending string
    // concatenation on the stack after each recursive call.
    //
    // ACCUMULATOR IDEA:
    //   Instead of appending the bit AFTER the recursive call, we PREPEND
    //   the current bit to `acc` BEFORE the call.
    //   Because the recursion visits bits from least-significant to most-significant
    //   (dividing by 2 each time), prepending naturally builds the big-endian string.
    //
    // TRACE: fooTail 6
    //   aux "" 6
    //   = aux ("0" + "") 3          (* 6 % 2 = 0 → prepend "0" → acc="0" *)
    //   = aux ("1" + "0") 1         (* 3 % 2 = 1 → prepend "1" → acc="10" *)
    //   = aux ("1" + "10") 0        (* 1 % 2 = 1 → prepend "1" → acc="110" *)
    //   = "110"                     (* base case: return acc directly *)
    //
    // The call `aux newAcc (n/2)` IS the very last operation — tail position. ✓
    let fooTail x =
        let rec aux acc =
            function
            | 0 -> acc
            | n when n % 2 = 0 -> aux ("0" + acc) (n / 2)
            | n                 -> aux ("1" + acc) (n / 2)
        aux "" x

(* ── Question 2.6 ────────────────────────────────────────────── *)

    // barTail — tail-recursive version of bar using Continuation-Passing Style (CPS).
    //
    // THE PROBLEM WITH bar: `(foo x) :: (bar xs)` leaves a pending cons on the stack.
    // After `bar xs` returns a list, we still need to prepend `foo x` to it.
    //
    // CPS SOLUTION:
    //   Pass a CONTINUATION `cont` — a function representing "what to do with the
    //   eventual result list."  Instead of prepending after the recursive call,
    //   we CAPTURE the prepend in a new closure and pass it as the next continuation.
    //   The closures live on the HEAP, not the stack — so the stack stays flat.
    //
    // BASE CASE  ([] → cont []):
    //   The list is empty, the result is [].  Apply the accumulated continuation to [].
    //
    // RECURSIVE CASE  (x :: xs → aux xs (fun result -> cont (fooTail x :: result))):
    //   Immediately recurse on xs.
    //   The new continuation says: "once you have the result for xs,
    //   prepend fooTail(x) to it, then pass that to the outer cont."
    //   The call `aux xs (...)` is the LAST thing done — tail position. ✓
    //
    // Start with `id` as the initial continuation: "just return the result as-is."
    //
    // TRACE: barTail [6; 1]
    //   aux id [6;1]
    //   = aux (fun r -> id ("110"::r)) [1]          (* fooTail 6 = "110" *)
    //   = aux (fun r -> (fun r -> id ("110"::r)) ("1"::r)) []
    //                                               (* fooTail 1 = "1" *)
    //   = (outermost cont) []
    //   = (fun r -> id ("110"::r)) ("1"::[])
    //   = id ["110"; "1"]
    //   = ["110"; "1"]  ✓
    let barTail lst =
        let rec aux cont =
            function
            | []      -> cont []
            | x :: xs -> aux (fun result -> cont (fooTail x :: result)) xs
        aux id lst


(* ═══════════════════════════════════════════════════════════════
   3: Matrix operations (25%)
   ═══════════════════════════════════════════════════════════════ *)

    type matrix = int[,]

    let init f rows cols = Array2D.init rows cols f

    let numRows (m : matrix) = Array2D.length1 m
    let numCols (m : matrix) = Array2D.length2 m

    let get (m : matrix) row col = m.[row, col]
    let set (m : matrix) row col v = m.[row, col] <- v

    let print (m : matrix) =
        for row in 0..numRows m - 1 do
            for col in 0..numCols m - 1 do
                printf "%d\t" (get m row col)
            printfn ""

(* ── Question 3.1 ────────────────────────────────────────────── *)

    // failDimensions — raises an exception reporting the dimensions of two incompatible matrices.
    //
    // TYPE: matrix -> matrix -> 'a
    //   The polymorphic return type 'a allows failDimensions to be called anywhere,
    //   since `failwith` never actually returns — it always throws.
    //
    // ERROR MESSAGE FORMAT (exact, including the "roms" typo for m2, which the exam requires):
    //   "Invalid matrix dimensions: m1 rows = R1, m1 columns = C1, m2 roms = R2, m2 columns = C2"
    //
    // Example:
    //   failDimensions (init (fun _ _ -> 0) 3 4) (init (fun _ _ -> 1) 8 9)
    //   → System.Exception: Invalid matrix dimensions: m1 rows = 3, m1 columns = 4,
    //                        m2 roms = 8, m2 columns = 9
    let failDimensions (m1 : matrix) (m2 : matrix) : 'a =
        failwith (sprintf "Invalid matrix dimensions: m1 rows = %d, m1 columns = %d, m2 roms = %d, m2 columns = %d"
                          (numRows m1) (numCols m1) (numRows m2) (numCols m2))

(* ── Question 3.2 ────────────────────────────────────────────── *)

    // add — element-wise sum of two matrices with the SAME dimensions.
    //
    // TYPE: matrix -> matrix -> matrix
    //
    // DIMENSION CHECK: if rows or columns differ, call failDimensions to abort.
    //
    // IMPLEMENTATION: use `init` (not `set`) as suggested by the hint.
    //   init f rows cols creates a brand-new matrix where cell (i,j) = f i j.
    //   The combining function adds the corresponding elements from m1 and m2.
    //
    // Example:
    //   add (init (fun x y -> x+y) 2 3) (init (fun x y -> x*y) 2 3)
    //   m1: [[0,1,2],[1,2,3]]   m2: [[0,0,0],[0,1,2]]   result: [[0,1,2],[1,3,5]]
    let add (m1 : matrix) (m2 : matrix) : matrix =
        if numRows m1 <> numRows m2 || numCols m1 <> numCols m2 then
            failDimensions m1 m2
        else
            init (fun i j -> get m1 i j + get m2 i j) (numRows m1) (numCols m1)

(* ── Question 3.3 ────────────────────────────────────────────── *)

    let m1 = (init (fun i j -> i * 3 + j + 1) 2 3)
    let m2 = (init (fun j k -> j * 2 + k + 1) 3 2)

    // dotProduct — dot product of row `row` in m1 with column `col` in m2.
    //
    // TYPE: matrix -> matrix -> int -> int -> int
    //
    // FORMULA: Σ(k=0..b-1) m1[row,k] * m2[k,col]   where b = numCols m1 = numRows m2.
    //
    // We assume dimensions are compatible (exam says to assume this).
    // List.sumBy sums the products over all shared indices k.
    //
    // Example: dotProduct m1 m2 0 1
    //   m1 row 0 = [1,2,3]   m2 col 1 = [2,4,6]
    //   = 1*2 + 2*4 + 3*6 = 2 + 8 + 18 = 28  ✓
    let dotProduct (m1 : matrix) (m2 : matrix) (row : int) (col : int) : int =
        List.sumBy (fun k -> get m1 row k * get m2 k col) [0..numCols m1 - 1]

    // mult — matrix multiplication: m1 (a×b) * m2 (b×c) = result (a×c).
    //
    // TYPE: matrix -> matrix -> matrix
    //
    // DIMENSION CHECK: m1's columns must equal m2's rows — otherwise failDimensions.
    //
    // IMPLEMENTATION: a one-liner using init and dotProduct (as the exam hints).
    //   Each cell (i,j) of the result is the dot product of row i in m1 with column j in m2.
    //
    // Example: mult m1 m2
    //   [[1*1+2*3+3*5, 1*2+2*4+3*6], [4*1+5*3+6*5, 4*2+5*4+6*6]]
    //   = [[22, 28], [49, 64]]  ✓
    let mult (m1 : matrix) (m2 : matrix) : matrix =
        if numCols m1 <> numRows m2 then failDimensions m1 m2
        else init (fun i j -> dotProduct m1 m2 i j) (numRows m1) (numCols m2)

(* ── Question 3.4 ────────────────────────────────────────────── *)

    // parInit — parallel version of init that spawns rows×cols threads.
    //
    // TYPE: (int -> int -> int) -> int -> int -> matrix
    //   Exactly the same type as `init`.
    //
    // STRATEGY (as suggested by the hint):
    //   1. Create an empty matrix of the right size (filled with 0).
    //   2. For every cell (i,j), create an async task that calls `set` to write f(i,j).
    //   3. Run all tasks concurrently with Async.Parallel.
    //   4. Block until all are done with Async.RunSynchronously.
    //   5. Return the now-filled matrix.
    //
    // WHY NO DATA RACES: each task writes to a UNIQUE cell (i,j) — no two tasks
    // share the same memory location, so mutation is safe without locking.
    //
    // The list comprehension [for i ... for j ... yield async {...}] generates
    // rows*cols async workflows, one per cell.
    let parInit (f : int -> int -> int) (rows : int) (cols : int) : matrix =
        let m = init (fun _ _ -> 0) rows cols
        [for i in 0..rows - 1 do
            for j in 0..cols - 1 do
                yield async { set m i j (f i j) }]
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
        m


(* ═══════════════════════════════════════════════════════════════
   4: Stack machines (25%)
   ═══════════════════════════════════════════════════════════════ *)

    type cmd = Push of int | Add | Mult
    type stackProgram = cmd list

(* ── Question 4.1 ────────────────────────────────────────────── *)

    // DESIGN CHOICE: the stack is an immutable int list.
    //   - The HEAD of the list is the TOP of the stack.
    //   - `push x` prepends x (O(1)); `pop` takes the head (O(1)).
    //   - Immutability fits cleanly with the option-state monad in Q4.3.
    type stack = int list

    // emptyStack — returns a fresh, empty stack.
    // Takes unit as argument: idiomatic F# "factory function" pattern.
    let emptyStack () : stack = []

(* ── Question 4.2 ────────────────────────────────────────────── *)

    // runStackProgram — directly interprets a stack program.
    //
    // TYPE: stackProgram -> int option
    //   Returns Some(top) when the program finishes successfully.
    //   Returns None on stack underflow (Add/Mult with fewer than 2 elements).
    //
    // The helper `run s cmds` carries the current stack `s`.
    //   Push x  → prepend x to the stack, continue
    //   Add     → pop two values, push their sum, continue
    //   Mult    → pop two values, push their product, continue
    //   []      → return the top of the stack (or None if empty)
    let runStackProgram (prog : stackProgram) : int option =
        let rec run (s : stack) (cmds : stackProgram) =
            match cmds with
            | [] ->
                match s with
                | top :: _ -> Some top
                | []       -> None
            | Push x :: rest -> run (x :: s) rest
            | Add    :: rest ->
                match s with
                | a :: b :: s' -> run (a + b :: s') rest
                | _            -> None
            | Mult   :: rest ->
                match s with
                | a :: b :: s' -> run (a * b :: s') rest
                | _            -> None
        run [] prog

(* ── Question 4.3 ────────────────────────────────────────────── *)

    // StateMonad<'a> = SM of (stack -> ('a * stack) option)
    //
    //   A recipe: given a current stack, produce (result, newStack) on success,
    //   or None on failure (e.g. stack underflow).
    //
    // ret x       — succeed immediately with value x, stack unchanged
    // fail        — always fail (return None)
    // bind f m    — run m; if it returns Some(x, s'), run (f x) on s'; short-circuit on None
    // >>=         — infix bind
    // >>>=        — sequence two monadic actions, discarding the first result
    // evalSM m    — run the computation on an empty stack
    type StateMonad<'a> = SM of (stack -> ('a * stack) option)

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

    let evalSM (SM f) = f (emptyStack ())

    // push x — monadic: push integer x onto the top of the stack.
    //
    // The new stack is x :: s.  Result value is unit (no meaningful return).
    // push never fails — adding to a stack always succeeds.
    let push (x : int) : StateMonad<unit> =
        SM (fun s -> Some ((), x :: s))

    // pop — monadic: remove and return the top element of the stack.
    //
    // Empty stack  → None (monadic failure = stack underflow)
    // x :: s'      → Some(x, s')  — returns x, leaves s' as the new stack
    let pop : StateMonad<int> =
        SM (fun s ->
            match s with
            | []      -> None
            | x :: s' -> Some (x, s'))

(* ── Question 4.4 ────────────────────────────────────────────── *)

    // StateBuilder — maps F# computation expression (CE) keywords to monadic functions.
    //
    //   let! x = m   → Bind(m, fun x -> <rest>)   run m, bind result to x
    //   do! m        → Bind(m, fun () -> <rest>)   run m for its side-effect (state change)
    //   return x     → Return(x) = ret x            wrap x in the monad
    //   return! m    → ReturnFrom(m) = m             return an already-monadic value
    //
    // NOTE on Bind parameter ORDER:
    //   F# CE passes Bind(computation, continuation), but the template writes Bind(f, x).
    //   Here f IS the computation and x IS the continuation, so the body calls `bind x f`.
    type StateBuilder() =

        member this.Bind(f, x)    = bind x f
        member this.Return(x)     = ret x
        member this.ReturnFrom(x) = x
        member this.Combine(a, b) = a >>= (fun _ -> b)

    let state = new StateBuilder()

    // runStackProg2 — interprets a stack program using the STATE MONAD and CE.
    //
    // TYPE: stackProgram -> int option
    //   Identical behaviour to runStackProgram from Q4.2.
    //
    // Each command is monadic:
    //   Push x  → `do! push x`           (push x, discard unit result)
    //   Add     → pop twice, push sum
    //   Mult    → pop twice, push product
    //   []      → `return! pop`           (final result is the top of the stack)
    //
    // If any pop fails (stack underflow), the bind chain short-circuits to None.
    // evalSM runs the computation on an empty stack; Option.map fst extracts the result.
    let runStackProg2 (prog : stackProgram) : int option =
        let rec runProg cmds =
            state {
                match cmds with
                | [] ->
                    return! pop
                | Push x :: rest ->
                    do! push x
                    return! runProg rest
                | Add :: rest ->
                    let! a = pop
                    let! b = pop
                    do! push (a + b)
                    return! runProg rest
                | Mult :: rest ->
                    let! a = pop
                    let! b = pop
                    do! push (a * b)
                    return! runProg rest
            }
        evalSM (runProg prog) |> Option.map fst

(* ── Question 4.5 ────────────────────────────────────────────── *)

    open JParsec.TextParser

    // parseStackProg — parses a string of stack machine commands into a stackProgram.
    //
    // TYPE: string -> stackProgram
    //
    // GRAMMAR:
    //   program  ::= cmd*              (zero or more commands separated by spaces)
    //   cmd      ::= 'Push' ' ' int    (push an integer)
    //              | 'Add'             (binary add)
    //              | 'Mult'            (binary multiply)
    //
    // COMBINATORS USED:
    //   pstring s       — match the exact string s
    //   pchar c         — match the exact character c
    //   pint32          — match a signed decimal integer
    //   |>> f           — transform parse result with f
    //   <|>             — try left parser; fall back to right if it fails
    //   >>.             — sequence: run both, keep RIGHT result
    //   many (pchar ' ')— skip zero or more spaces (used as separator between commands)
    //   sepBy p sep     — parse p repeatedly with sep between occurrences
    //
    // EXAMPLE: parseStackProg "Push 5 Add Push 3 Mult"
    //   → [Push 5; Add; Push 3; Mult]
    let parseStackProg (s : string) : stackProgram =
        let ws      = many (pchar ' ')
        let parsePush =
            pstring "Push" >>. pchar ' ' >>. pint32 |>> Push
        let parseAdd  = pstring "Add"  |>> fun _ -> Add
        let parseMult = pstring "Mult" |>> fun _ -> Mult
        let parseCmd  = parsePush <|> parseAdd <|> parseMult
        let prog      = ws >>. sepBy parseCmd ws
        run prog s |> getSuccess
