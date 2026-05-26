module Exam2024

open System

(* ═══════════════════════════════════════════════════════════════
   1: Shapes (25%)
   ═══════════════════════════════════════════════════════════════ *)

type shape =
    | Rectangle of float * float
    | Circle of float
    | Triangle of float * float

type shapeList =
    | Empty
    | AddShape of shape * shape * shapeList

(* ── Question 1.1 ────────────────────────────────────────────── *)

(* HOW TO READ THE FUNCTION SYNTAX:
   `let area = function | ... -> ...`
   is shorthand for
   `let area s = match s with | ... -> ...`
   The `function` keyword creates an anonymous function that immediately
   pattern-matches its single argument.  Each `| Constructor(args) -> expr`
   is one case.

   area: for each shape we apply the standard geometric formula.

   Rectangle(w, h):
     Area = width × height.  Simple multiplication.

   Circle(r):
     Area = π × r².  Math.PI comes from the System namespace (opened at the top)
     and equals 3.14159265…  We write `r * r` rather than `r ** 2.0` to keep
     it explicit — both work in F#.

   Triangle(b, h):
     The exam says this is a RIGHT triangle (one 90-degree angle).
     Area of any triangle = (base × height) / 2.
     The 2.0 literal is needed because F# does not auto-promote int literals
     to float; writing just `2` here would be a type error. *)
let area =
    function
    | Rectangle(w, h) -> w * h
    | Circle r        -> Math.PI * r * r
    | Triangle(b, h)  -> b * h / 2.0

(* circumference / perimeter of each shape.

   Rectangle(w, h):
     Four sides: two of width w and two of height h → 2w + 2h.

   Circle(r):
     The circumference of a circle is 2πr.

   Triangle(b, h):
     This is a RIGHT triangle, so we know two legs: base b and height h.
     The third side (hypotenuse) has length √(b² + h²) by the Pythagorean theorem.
     Total perimeter = b + h + √(b² + h²).
     Math.Sqrt comes from System.Math (available via `open System`). *)
let circumference =
    function
    | Rectangle(w, h) -> 2.0 * w + 2.0 * h
    | Circle r        -> 2.0 * Math.PI * r
    | Triangle(b, h)  -> b + h + Math.Sqrt(b * b + h * h)


(* ── Question 1.2 ────────────────────────────────────────────── *)

(* UNDERSTANDING THE TYPE:
   `shapeList` is NOT a standard list — it is a CUSTOM recursive type.
   `AddShape of shape * shape * shapeList` holds exactly TWO shapes per
   node and a tail.  This enforces even-length shape lists.
   Compare with a normal list: `Cons of 'a * 'a list` holds ONE element.

   totalArea:
   We recurse over the shapeList and sum the area of every shape.
   Each AddShape node has TWO shapes, so we call `area` on both.

   WHY THIS IS NOT TAIL-RECURSIVE:
   Look at the AddShape case:
       area s1 + area s2 + totalArea rest
   F# evaluates this left-to-right:
     1. Compute area s1          → some float x
     2. Compute area s2          → some float y
     3. Call totalArea rest      → this is the RECURSIVE CALL
     4. Add x + y + (result of step 3)

   Step 4 can only happen AFTER step 3 returns.  That means the runtime
   must REMEMBER steps 1, 2, and the pending addition while it dives into
   the recursion.  That "remember" is exactly what the call stack does —
   each stack frame holds the pending work.  More AddShape nodes = deeper
   stack = not tail-recursive. *)
let rec totalArea = function
    | Empty                  -> 0.0
    | AddShape(s1, s2, rest) -> area s1 + area s2 + totalArea rest


(* ── Question 1.3 ────────────────────────────────────────────── *)

(* totalCircumference — tail-recursive via accumulator.

   THE ACCUMULATOR PATTERN:
   We introduce an inner helper `aux` that takes an extra argument `acc`
   (the running total).  Instead of leaving a pending addition on the stack,
   we UPDATE acc before the recursive call and pass the updated value in.
   After the recursive call there is NOTHING left to do — it is the very
   last expression, so the runtime can reuse the current stack frame
   (this is what "tail call" means in practice).

   Trace for a two-node list:
     totalCircumference (AddShape(s1, s2, AddShape(s3, s4, Empty)))
   = aux 0.0 (AddShape(s1, s2, AddShape(s3, s4, Empty)))
   = aux (0.0 + circ s1 + circ s2) (AddShape(s3, s4, Empty))
   = aux (circ s1 + circ s2 + circ s3 + circ s4) Empty
   = circ s1 + circ s2 + circ s3 + circ s4       ← returns acc directly

   Notice the recursive call is the LAST thing in each step — no pending `+`. *)
let totalCircumference sl =
    let rec aux acc = function
        | Empty                  -> acc
        | AddShape(s1, s2, rest) ->
            aux (acc + circumference s1 + circumference s2) rest
    aux 0.0 sl


(* ── Question 1.4 ────────────────────────────────────────────── *)

(* shapeListFold — a generic fold over shapeList.

   WHAT IS A FOLD?
   A fold is a way to collapse a data structure into a single value by
   repeatedly applying a combining function.  For a normal list:
       List.fold f acc [a; b; c] = f (f (f acc a) b) c
   Our shapeList has TWO elements per node, so we apply f TWICE per step:
       shapeListFold f acc (AddShape(s1, s2, rest))
     = shapeListFold f (f (f acc s1) s2) rest
   The acc "travels through" the list collecting information.

   SIGNATURE:  ('a -> shape -> 'a) -> 'a -> shapeList -> 'a
     - The first argument is the combining function f.
     - The second is the initial accumulator.
     - The third is the shapeList to fold over.
     - The result has the same type as the accumulator ('a).

   WHY TAIL-RECURSIVE:
   The recursive call `shapeListFold f (...) rest` is the last expression
   in the AddShape branch — nothing waits on the stack after it.

   EXAMPLE — containsCircle (provided):
     shapeListFold (fun acc c -> acc || isCircle c) false myList
   Here f = (fun acc c -> acc || isCircle c).  The accumulator starts as
   false and becomes true as soon as a Circle is encountered, staying true
   for all remaining shapes thanks to (||). *)
let rec shapeListFold f acc = function
    | Empty                  -> acc
    | AddShape(s1, s2, rest) -> shapeListFold f (f (f acc s1) s2) rest

let isCircle =
    function
    | Circle _ -> true
    | _        -> false

let containsCircle trs =
    shapeListFold (fun acc c -> acc || isCircle c) false trs


(* ── Question 1.5 ────────────────────────────────────────────── *)

(* Now that shapeListFold exists, totalArea and totalCircumference become
   trivial one-liners.

   totalArea2:
   The combining function is `fun acc s -> acc + area s`.
   - `acc` is the running sum (starts at 0.0).
   - `s` is the current shape.
   - We add area s to the running sum.
   shapeListFold applies this to every shape in the list.

   totalCircumference2: identical pattern, but with `circumference` instead
   of `area`.

   WHY 0.0 AS INITIAL ACCUMULATOR?
   0.0 is the identity element for addition: 0 + x = x.  Starting with 0
   means we correctly get 0.0 for an Empty shapeList, and get the exact
   sum for any non-empty list.

   NOTE: these are NON-recursive at the user level — all recursion is
   hidden inside shapeListFold. *)
let totalArea2 sl =
    shapeListFold (fun acc s -> acc + area s) 0.0 sl

let totalCircumference2 sl =
    shapeListFold (fun acc s -> acc + circumference s) 0.0 sl


(* ═══════════════════════════════════════════════════════════════
   2: Code Comprehension (25%)
   ═══════════════════════════════════════════════════════════════ *)

let foo =
    function
    | c when Char.IsWhiteSpace c -> c
    | c when c > 'w'             -> char (int c - 23)
    | c when c < 'x'             -> char (int c + 3)

let bar (str : string) = [for c in str -> c]

let baz str =
    let rec aux =
        function
        | [] -> ""
        | c :: cs -> string (foo c) + (aux cs)
    aux (bar str)

(* ── Question 2.1 ────────────────────────────────────────────── *)

(*
  ── TYPES ────────────────────────────────────────────────────────

  foo : char -> char
    It takes one character and returns one character.  We see this because
    the body is `function | c -> ...` which matches a single value, and
    every branch returns a `char` (either `c` unchanged, or `char(...)`).

  bar : string -> char list
    The argument `(str : string)` is explicitly annotated as string.
    The body `[for c in str -> c]` is a list comprehension producing a list
    of whatever type c has — since iterating over a string yields chars,
    the result is char list.

  baz : string -> string
    Takes the string `str`, converts it via bar to a char list, then the
    inner `aux` processes that list and returns a string (the "" base case
    and `string(...) + ...` recursive case both return strings).

  ── WHAT THEY DO ─────────────────────────────────────────────────

  HOW TO FIGURE OUT WHAT foo DOES:
  Step 1 — identify the three branches by their guards:
    Branch 1: `Char.IsWhiteSpace c`  → pass the character through unchanged.
    Branch 2: `c > 'w'`              → shift back by 23.
    Branch 3: `c < 'x'`              → shift forward by 3.

  Step 2 — trace specific characters to spot the pattern:
    'a' (int = 97):   not whitespace, 97 < 120('x') → char(97+3)  = char(100) = 'd'
    'b' (int = 98):   not whitespace, 98 < 120       → char(98+3)  = char(101) = 'e'
    'w' (int = 119):  not whitespace, 119 < 120      → char(119+3) = char(122) = 'z'
    'x' (int = 120):  not whitespace, 120 > 119('w') → char(120-23)= char(97)  = 'a'
    'y' (int = 121):  120 > 119                      → char(121-23)= char(98)  = 'b'
    'z' (int = 122):  122 > 119                      → char(122-23)= char(99)  = 'c'

  Step 3 — recognise the pattern:
    a→d, b→e, …, w→z, x→a, y→b, z→c
    Every letter shifts forward by 3 positions, wrapping x/y/z back to
    a/b/c.  This is the ROT-3 (Caesar) cipher with shift 3.

  Why -23 for x/y/z?  The alphabet has 26 letters.  Shifting forward by 3
  and then wrapping is the same as shifting by -(26-3) = -23 for the last
  three letters.  That is: char(int 'x' - 23) = char(120 - 23) = 'a'. ✓

  IMPORTANT — foo does NOT implement the Atbash cipher.
  Atbash maps a↔z, b↔y (reversal of the whole alphabet).
  foo maps a→d, b→e (shift of 3).  These are completely different.
  Question 3 is about the actual Atbash cipher, independently of foo/bar/baz.

  bar:
  `[for c in str -> c]` is an F# list comprehension.
  It means "iterate over every element c in str, and collect c into a list."
  In F#, strings are sequences of chars, so iterating over a string with
  `for c in str` visits each character left to right.
  Result: the same characters as in the string, in order, as a char list.

  baz:
  - bar str          → converts the string to a char list
  - aux (char list)  → recursively encodes the list:
      * Empty list  → return ""   (base case)
      * c :: cs     → encode c with foo, convert to a string with `string`,
                       concatenate with the encoded tail `aux cs`
  Overall: baz encodes a full string using the ROT-3 cipher (foo per char).

  ── APPROPRIATE NAMES ────────────────────────────────────────────

  foo → encodeChar  (or rot3Char, caesarShift)
  bar → toCharList  (or stringToChars, chars)
  baz → encode      (or rot3, encodeString)
*)


(* ── Question 2.2 ────────────────────────────────────────────── *)

(*
  WHY THE WARNING?

  The three patterns in foo are all written as `c when <guard>`.
  In F#, a `when` guard can FAIL — even if the pattern `c` matches,
  the guard might be false.  The compiler sees three potentially-failing
  cases and cannot determine at compile time whether they cover every
  possible input.  It does not reason about the guards' math; it just
  sees three guarded wildcards and warns "some input might not be covered."

  Logically the guards ARE exhaustive: every char is either whitespace,
  has an int value > 119 ('w'), or has an int value ≤ 119 (i.e. < 120 = 'x').
  But the compiler doesn't do that proof — it warns anyway.

  FIX:
  Remove the `when c < 'x'` guard from the last arm, replacing it with a
  plain `c` (no guard).  A pattern without a guard is irrefutable — it
  always succeeds.  The compiler now knows: "if we reach the third arm,
  we match no matter what — no case is missing."

  This is correct because by the time we reach the third arm we KNOW:
    - c is not whitespace (arm 1 didn't match)
    - c is not > 'w' (arm 2 didn't match)
  Therefore c MUST be ≤ 'w', i.e. < 'x'.  We can drop the guard safely. *)
let foo2 =
    function
    | c when Char.IsWhiteSpace c -> c
    | c when c > 'w'             -> char (int c - 23)
    | c                          -> char (int c + 3)


(* ── Question 2.3 ────────────────────────────────────────────── *)

(* baz2: rewrite baz without explicit recursion, using higher-order functions.

   PIPELINE BREAKDOWN  (|> threads the value left-to-right):

   str |> bar
     Converts the string to a char list.
     e.g. "ab" → ['a'; 'b']

   |> List.map (foo >> string)
     `List.map f xs` applies f to every element of xs and collects results.
     `foo >> string` is FUNCTION COMPOSITION:
       - `>>` in F# composes two functions: (f >> g) x = g (f x)
       - `foo >> string` means: first apply foo (char→char), then apply
         string (char→string, converting the char to a 1-char string).
     So each char is encoded by foo and then wrapped in a string.
     e.g. ['a';'b'] → ["d"; "e"]

   |> String.concat ""
     Joins a list of strings with "" as the separator (no separator).
     e.g. ["d"; "e"] → "de"

   The key difference from baz: NO explicit `let rec`.  All recursion is
   inside List.map (which internally iterates over the list). *)
let baz2 str =
    str |> bar |> List.map (foo >> string) |> String.concat ""


(* ── Question 2.4 ────────────────────────────────────────────── *)

(*
  PROVING baz IS NOT TAIL-RECURSIVE.

  Rule: a recursive call is in "tail position" if it is the VERY LAST
  operation performed before the function returns.  If anything still
  needs to happen AFTER the recursive call, it is NOT in tail position.

  We evaluate `baz "ab"` step by step.  We may evaluate foo and bar
  immediately (the exam gives this permission):

    baz "ab"
  = aux (bar "ab")              (* bar "ab" = ['a';'b'] *)
  = aux ['a'; 'b']
  = string (foo 'a') + (aux ['b'])
                                (* foo 'a' = 'd'; string 'd' = "d" *)
  = "d" + (aux ['b'])           (* ← THE PROBLEM IS HERE *)
  = "d" + (string (foo 'b') + aux [])
                                (* foo 'b' = 'e'; string 'e' = "e" *)
  = "d" + ("e" + "")
  = "d" + "e"
  = "de"

  In the step  "d" + (aux ['b']):
  The recursive call `aux ['b']` is INSIDE the `+` operation.
  Before `+` can complete, it needs the left operand ("d") AND the right
  operand (result of `aux ['b']`).  So after `aux ['b']` returns, the
  runtime still has to perform the concatenation "d" + ...
  That pending concatenation is held on the call stack as a stack frame.

  For a string of length n, there are n pending concatenations on the
  stack simultaneously.  This is the definition of non-tail-recursive.
*)


(* ── Question 2.5 ────────────────────────────────────────────── *)

(* bazTail: tail-recursive version using Continuation-Passing Style (CPS).

   WHAT IS A CONTINUATION?
   A continuation is a function that represents "the rest of the computation."
   Instead of returning a value directly, we pass the value to the
   continuation, which decides what to do with it.

   In normal baz, the pending `+` operations stack up on the call stack.
   In CPS, we represent those pending operations as a closure (a function)
   that gets passed deeper into the recursion.  The call stack stays flat.

   PARAMETER `c`:
   `c` is the current continuation — a function of type `string -> string`.
   It means: "once you have computed the encoded string for the remaining
   chars, call me with that result so I can finish the work."
   Initially `c = id` (identity), meaning "just return the result as-is."

   BASE CASE  ([] → c ""):
   The list is empty, so the encoded string is "".
   We call the continuation with "" — `c ""` — letting it do any pending work.

   RECURSIVE CASE  (x :: xs → aux xs (fun result -> c (string (foo x) + result))):
   We need to encode x and prepend it to the encoding of xs.
   Instead of computing the encoding of xs FIRST and then prepending (which
   would leave a pending `+`), we:
     1. Immediately recurse on xs.
     2. Pass a NEW continuation: `fun result -> c (string (foo x) + result)`
        This new continuation says: "when you have the result for xs, prepend
        the encoded x to it, then call the outer continuation c."
   The call `aux xs (...)` is the LAST thing done — tail position. ✓

   TRACE of bazTail "ab":
     aux ['a';'b'] id
   = aux ['b'] (fun r -> id ("d" + r))         (* x='a', foo 'a'='d' *)
   = aux []    (fun r -> (fun r -> id ("d"+r)) ("e" + r))
                                                (* x='b', foo 'b'='e' *)
   = (fun r -> (fun r -> id ("d"+r)) ("e"+r)) ""
   = (fun r -> id ("d"+r)) ("e"+"")
   = (fun r -> id ("d"+r)) "e"
   = id ("d"+"e")
   = "de"  ✓

   All recursive calls to aux are in tail position; the "stack" of
   pending work lives in the closure chain, not in stack frames. *)
let bazTail str =
    let rec aux cs c =
        match cs with
        | []      -> c ""
        | x :: xs -> aux xs (fun result -> c (string (foo x) + result))
    aux (bar str) id


(* ═══════════════════════════════════════════════════════════════
   3: Atbash Cipher (25%)
   ═══════════════════════════════════════════════════════════════ *)

open JParsec.TextParser

(* ── Question 3.1 ────────────────────────────────────────────── *)

(* THE ATBASH CIPHER:
   The cipher reverses the alphabet — every letter maps to its mirror:
     a ↔ z,  b ↔ y,  c ↔ x, …,  m ↔ n
   Whitespace stays the same.

   HOW TO DERIVE THE FORMULA:
   In ASCII:  'a' = 97,  'z' = 122.
   For letter c we want: encoded = 'a' + ('z' - c) = 97 + 122 - int(c) = 219 - int(c)
   Check:  'a'(97) → 219-97 = 122 = 'z' ✓
           'z'(122)→ 219-122 = 97  = 'a' ✓
           'h'(104)→ 219-104 = 115 = 's' ✓  (hello → svool)
           'n'(110)→ 219-110 = 109 = 'm' ✓

   IMPLEMENTATION NOTES:
   - `int c` converts a char to its ASCII integer value.
   - `char n` converts an integer back to the corresponding char.
   - We reuse `bar` from Q2 to convert string → char list, apply
     atbashChar to each element, convert chars back to strings with
     `string`, and join with `String.concat ""`.
   - `atbashChar >> string` is function composition: atbashChar first,
     then string. *)
let encrypt (str : string) : string =
    let atbashChar c =
        if Char.IsWhiteSpace c then c
        else char (219 - int c)
    str |> bar |> List.map (atbashChar >> string) |> String.concat ""


(* ── Question 3.2 ────────────────────────────────────────────── *)

(* WHY decrypt = encrypt:
   Atbash is an INVOLUTION — applying it twice returns the original.
   Proof: apply the formula twice to letter c:
     First:  219 - int(c)           → gives some encoded char e
     Second: 219 - int(e)           = 219 - (219 - int(c)) = int(c) → gives c back ✓

   Concretely:  'h' → 's' → 'h'
                'e' → 'v' → 'e'
   So encrypt (encrypt s) = s for all valid strings s.
   Therefore decrypt text = encrypt text (applying the cipher undoes itself). *)
let decrypt (str : string) : string = encrypt str


(* ── Question 3.3 ────────────────────────────────────────────── *)

(* splitAt: split a string into a list of substrings of at most i characters.

   F# STRING SLICE SYNTAX:
     str.[0..i-1]  — characters at positions 0, 1, …, i-1  (first i chars)
     str.[i..]     — characters from position i to the end
   Indices are 0-based.  For "hello" with i=2: "hello".[0..1]="he", "hello".[2..]="llo".

   LOGIC:
   If the remaining string is ≤ i characters, it IS the last chunk — wrap in a list.
   Otherwise, take the first i characters as one chunk, then recurse on the rest.

   The constraint "last element must not be empty string" is automatically
   satisfied because we only recurse when s.Length > i, meaning s.[i..] has
   at least 1 character (it's non-empty).

   Example: splitAt 2 "hello"
   = "he" :: splitAt 2 "llo"
   = "he" :: "ll" :: splitAt 2 "o"
   = "he" :: "ll" :: ["o"]         (length 1 ≤ 2, stop)
   = ["he"; "ll"; "o"] *)
let splitAt (i : int) (str : string) : string list =
    let rec aux (s : string) =
        if s.Length <= i then [s]
        else s.[0..i-1] :: aux s.[i..]
    aux str


(* ── Question 3.4 ────────────────────────────────────────────── *)

(* parEncrypt: encrypt a string by splitting it into chunks and encrypting
   each chunk in a SEPARATE PARALLEL THREAD.

   ASYNC WORKFLOW PATTERN (the standard F# parallelism recipe):

   Step 1 — splitAt n str
     Produces a list of string chunks, e.g. ["hel"; "lo "; "wor"; "ld"]

   Step 2 — List.map (fun chunk -> async { return encrypt chunk })
     `async { return x }` creates an ASYNC COMPUTATION — a description of
     work to be done, but NOT started yet.  Like wrapping a task in an
     envelope.  Each chunk gets its own envelope.

   Step 3 — Async.Parallel
     Takes a seq/list of async computations and returns ONE async computation
     that, when run, executes ALL of them concurrently and collects results
     in an array.  The order of the output array matches the input order.

   Step 4 — Async.RunSynchronously
     Actually RUNS the parallel computation.  The calling thread waits until
     every async finishes, then returns the results as a string array.

   Step 5 — String.concat ""
     Joins the array of encrypted chunks back into one string (no separator).

   Note: String.concat works on both arrays and lists in F#. *)
let parEncrypt (str : string) (n : int) : string =
    splitAt n str
    |> List.map (fun chunk -> async { return encrypt chunk })
    |> Async.Parallel
    |> Async.RunSynchronously
    |> String.concat ""


(* ── Question 3.5 ────────────────────────────────────────────── *)

(* parseEncrypt: a JParsec parser that reads letters/whitespace and encrypts
   each letter with Atbash.  Upper-case letters are treated as lower-case.

   JPARSEC COMBINATOR GLOSSARY (what each operator does):

   satisfy (pred : char -> bool) : Parser<char>
     The most primitive parser.  Reads ONE character from the input.
     If `pred` returns true for that character, succeeds and returns it.
     If `pred` returns false (or there is no character), fails without
     consuming any input.

   p |>> (f : 'a -> 'b) : Parser<'b>
     "Parser map."  Runs parser p; if it succeeds with value x, transforms
     the result to f(x).  Like List.map but for parsers.

   many (p : Parser<'a>) : Parser<'a list>
     Runs p repeatedly until it fails.  Returns a list of all successful
     results.  Always succeeds (returns [] if p fails on the first try).
     This is what causes the parser to STOP at '!' — satisfy fails on '!',
     many stops, returns what it collected so far.

   (List.map string >> String.concat "")
     Function composition applied to the final char list:
     - List.map string : (char list → string list)  convert each char to "x"
     - String.concat "" : (string list → string)    join with no separator

   UPPERCASE HANDLING:
   `Char.ToLower c` converts 'A'–'Z' to 'a'–'z', leaving other chars alone.
   We call it inside encryptChar before applying the Atbash formula, so 'H'
   is treated as 'h' (both encrypt to 's').  We must NOT lower-case the
   char before the parser sees it — the work must happen inside the parser. *)
let parseEncrypt : Parser<string> =
    let encryptChar c =
        if Char.IsWhiteSpace c then c
        else char (219 - int (Char.ToLower c))
    many (satisfy (fun c -> Char.IsLetter c || Char.IsWhiteSpace c) |>> encryptChar)
    |>> (List.map string >> String.concat "")


(* ═══════════════════════════════════════════════════════════════
   4: Tally Clickers (25%)
   ═══════════════════════════════════════════════════════════════ *)

(* ── Question 4.1 ────────────────────────────────────────────── *)

(* DESIGN DECISIONS:

   We need to store:
     (a) the wheel — the list of characters every wheel shows, in order.
         Since all wheels are identical, we only need ONE copy.
     (b) the position of each wheel — an integer index into the wheel for
         each wheel, since they can all differ as clicking progresses.

   DATA STRUCTURE CHOICES:

   Wheel stored as char[] (array, not list):
     The exam says clicking must run in O(n_wheels) time, NOT O(n_chars).
     To get the character at a position, we need O(1) lookup.
     Arrays give O(1) random access: wheel.[i] is instant regardless of size.
     A list would give O(n_chars) per lookup (traverse from head), which
     would make click O(n_wheels × n_chars) — too slow.

   Positions stored as int list (immutable):
     We only ever visit all n positions once per click (carry propagation).
     int list gives O(n_wheels) per click — acceptable.
     Using an immutable list keeps the overall design functional (no mutation
     needed for Q4.2-4.5).

   newClicker chars n:
   - List.toArray chars  converts char list to char array (O(n_chars), done once).
   - List.replicate n 0  creates [0; 0; …; 0] with n zeros — all wheels
                         start at index 0, which is `chars.[0]` (first symbol). *)
type clicker = Clicker of char[] * int list

let newClicker (chars : char list) (n : int) : clicker =
    Clicker(List.toArray chars, List.replicate n 0)


(* ── Question 4.2 ────────────────────────────────────────────── *)

(* click: advance the clicker by one press.

   CARRY PROPAGATION (think of binary addition but in base `size`):
   - Increment the rightmost wheel's index by 1.
   - If that index hits `size` (overflows), reset it to 0 AND carry into
     the next wheel to the left.
   - Repeat until a wheel does NOT overflow, or all wheels overflow (full wrap).

   Example with wheel = {a,b,c} (size=3), positions = [0; 2] = "ac":
   Reversed: [2; 0]
   addCarry [2; 0]:
     next = (2+1) % 3 = 0  → overflow! → 0 :: addCarry [0]
     addCarry [0]:
       next = (0+1) % 3 = 1  → no overflow → 1 :: []  = [1]
     → 0 :: [1] = [0; 1]
   Reversed back: [1; 0] = "ba" ✓

   WHY REVERSE?
   The carry goes RIGHT-TO-LEFT (rightmost wheel first).  Our positions list
   is stored LEFT-TO-RIGHT.  Reversing lets `addCarry` process from the head,
   which is now the rightmost wheel.  We reverse back at the end to restore
   the original order. *)
let click (Clicker(wheel, positions) : clicker) : clicker =
    let size = wheel.Length
    let rec addCarry = function
        | []      -> []
        | i :: rest ->
            let next = (i + 1) % size
            if next = 0 then next :: addCarry rest
            else next :: rest
    Clicker(wheel, positions |> List.rev |> addCarry |> List.rev)

(* read: convert every wheel's current position to the corresponding char
   and concatenate into one string.
   `wheel.[i]` uses the array index `i` to look up the char — O(1).
   `string wheel.[i]` converts that char to a single-character string.
   `String.concat ""` joins the per-wheel strings with no separator. *)
let read (Clicker(wheel, positions) : clicker) : string =
    positions |> List.map (fun i -> string wheel.[i]) |> String.concat ""


(* ── Question 4.3 ────────────────────────────────────────────── *)

(* THE STATE MONAD:

   type StateMonad<'a> = SM of (clicker -> 'a * clicker)

   A StateMonad<'a> is a VALUE that WRAPS a function from clicker to ('a * clicker).
   Think of it as a "recipe": given an input clicker state, it produces
   a result of type 'a AND a (possibly modified) clicker state.

   Operators provided by the exam:

   ret x : StateMonad<'a>
     A recipe that does NOT change the clicker and returns x.
     SM (fun cl -> (x, cl))

   bind f (SM a) : StateMonad<'b>
     Sequences two recipes:
     1. Run recipe `a` on the clicker → get result x and new clicker cl'.
     2. Apply `f` to x to get a second recipe.
     3. Run that second recipe on cl'.
     In plain English: "run a, feed its result to f, run what f gives back."

   >>= is infix bind:   `m >>= f`  means "run m, pass result to f."
   >>>= is sequence:    `m >>>= n` means "run m (ignore result), run n."

   evalSM cl (SM f) = f cl
     Actually RUNS the recipe on clicker `cl`.  Returns ('a * clicker).
     Usually we use `|> fst` to get just the value, discarding the final state.

   ── WHY click2 AND read2 MUST USE SM DIRECTLY ──────────────────

   The exam says: "You cannot use monadic operators like bind or ret."
   Why?  Because ret and bind only THREAD state through — they don't CHANGE
   it.  To actually call `click` (which updates the clicker) or `read`
   (which reads the clicker), we must reach inside SM and write a function
   that calls them.

   click2 = SM (fun cl -> ((), click cl))
     When run on clicker `cl`:
     - Calls `click cl` to produce a new clicker.
     - Returns `()` as the value (unit — no useful result).
     - Returns the new clicker as the updated state.

   read2 = SM (fun cl -> (read cl, cl))
     When run on clicker `cl`:
     - Calls `read cl` to get the display string.
     - Returns that string as the value.
     - Returns `cl` UNCHANGED (reading doesn't advance the clicker). *)
type StateMonad<'a> = SM of (clicker -> 'a * clicker)

let ret x = SM (fun cl -> (x, cl))

let bind f (SM a) : StateMonad<'b> =
    SM (fun cl ->
           let x, cl'  = a cl
           let (SM g) = f x
           g cl')

let (>>=) x f = bind f x
let (>>>=) x y = x >>= (fun _ -> y)

let evalSM cl (SM f) = f cl

let click2 = SM (fun cl -> ((), click cl))
let read2  = SM (fun cl -> (read cl, cl))


(* ── Question 4.4 ────────────────────────────────────────────── *)

(* multipleClicks x : StateMonad<string list>

   WHAT IT RETURNS:
   A list of x strings.  The first string is the CURRENT state (before any
   clicks); the remaining x-1 strings are the states after each click.
   Total list length = x.

   Example with 2-wheel {a,b,c} clicker starting at "aa" and x=4:
   Expected: ["aa"; "ab"; "ac"; "ba"]
   (Initial "aa", then click→"ab", click→"ac", click→"ba")

   CONSTRAINTS: must use only >>= / ret / >>>=.  Cannot pattern-match
   on StateMonad (no `let (SM f) = ...`).

   DESIGN — recursive helper aux i:
   - i=0: no readings needed → ret [] (empty list, no state changes)
   - i=1: read once, no click needed → read2 >>= fun s -> ret [s]
   - i>1: read current state (s), click once, collect i-1 more readings,
           prepend s to the front of that list.

   HOW THE >>= CHAINING WORKS:
   `read2 >>= fun s ->` runs read2 on the clicker, binds the string result
   to `s`, then continues with the rest of the expression.  `s` is now a
   plain string (not a monad) inside the lambda.

   `click2 >>= fun () ->` runs click2 (updates the clicker), discards the
   unit result with `()`, and continues.

   `aux (i-1) >>= fun rest ->` runs the recursive computation on the new
   clicker state, binds the resulting list to `rest`.

   `ret (s :: rest)` wraps the final list in the monad (no state change). *)
let multipleClicks (x : int) : StateMonad<string list> =
    let rec aux i =
        if i = 0 then ret []
        elif i = 1 then
            read2 >>= fun s -> ret [s]
        else
            read2     >>= fun s    ->
            click2    >>= fun ()   ->
            aux (i-1) >>= fun rest ->
            ret (s :: rest)
    aux x


(* ── Question 4.5 ────────────────────────────────────────────── *)

(* StateBuilder: maps F# computation expression keywords to monadic functions.

   HOW COMPUTATION EXPRESSIONS (CE) WORK:
   A `state { ... }` block is syntax sugar.  The F# compiler rewrites it
   into calls to the StateBuilder methods.  Specifically:

     `let! x = expr`    → this.Bind(expr, fun x -> <rest>)
     `do! expr`         → this.Bind(expr, fun () -> <rest>)
     `return x`         → this.Return(x)
     `return! expr`     → this.ReturnFrom(expr)
     `a; b` (sequencing)→ this.Combine(a, b)

   StateBuilder.Bind(f, x) = bind x f
   NOTE: the CE passes (computation, continuation), but the template writes
   Bind(f, x).  This just means the first argument `f` IS the computation
   and the second `x` IS the continuation function — and the body calls
   `bind continuation computation`.  This is consistent with the monad:
   bind f (SM a) applies f to the result of running a.

   WHY multipleClicks2 IS STRUCTURALLY IDENTICAL TO multipleClicks:
   Every `let! s = read2` desugars to exactly `read2 >>= fun s -> ...`
   Every `do! click2`     desugars to exactly `click2 >>= fun () -> ...`
   The CE is just prettier syntax for the same operations. *)
type StateBuilder() =
    member this.Bind(f, x)    = bind x f
    member this.Return(x)     = ret x
    member this.ReturnFrom(x) = x
    member this.Combine(a, b) = a >>= (fun _ -> b)

let state = StateBuilder()

let multipleClicks2 (x : int) : StateMonad<string list> =
    let rec aux i =
        if i = 0 then state { return [] }
        elif i = 1 then
            state {
                let! s = read2
                return [s]
            }
        else
            state {
                let! s    = read2   (* run read2; bind result to s *)
                do! click2          (* run click2; discard () result *)
                let! rest = aux (i - 1)  (* recurse; bind result list to rest *)
                return s :: rest    (* wrap (s :: rest) in the monad *)
            }
    aux x
