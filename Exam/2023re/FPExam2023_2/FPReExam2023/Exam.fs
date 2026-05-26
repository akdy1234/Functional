module ReExam2023

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
 module ReExam2023 =
 *)

(* ═══════════════════════════════════════════════════════════════
   1: Arithmetic (25%)
   ═══════════════════════════════════════════════════════════════ *)

    type arith =
    | Num of int
    | Add of arith * arith

    let p1 = Num 42
    let p2 = Add(Num 5, Num 3)
    let p3 = Add(Add(Num 5, Num 3), Add(Num 7, Num (-9)))

(* ── Question 1.1 ────────────────────────────────────────────── *)

    (* eval : arith -> int
       Evaluates an arithmetic expression to an integer.
       Rules (where [[a]] means eval a):
         [[⌈x⌉]]     = x
         [[a ⊕ b]]   = [[a]] + [[b]]

       WHY NOT TAIL-RECURSIVE?
       The exam hints "Do not try tail recursion. It's significantly more
       complicated than using plain recursion."
       In `eval p + eval q`, after `eval p` returns we still need `eval q`
       and the addition — not a tail call. Simple structural recursion suffices. *)
    let rec eval =
        function
        | Num x     -> x
        | Add(a, b) -> eval a + eval b


(* ── Question 1.2 ────────────────────────────────────────────── *)

    (* negate : arith -> arith
       Negates an arithmetic expression WITHOUT evaluating it.
       Rules:
         ⊖⌈x⌉     = ⌈-x⌉
         ⊖(a ⊕ b) = (⊖a) ⊕ (⊖b)

       This pushes negation through the tree, flipping every Num.
       We do NOT use eval — the exam says "Without using the eval function."

       Check: negate p2 = negate(Add(Num 5, Num 3))
            = Add(negate(Num 5), negate(Num 3))
            = Add(Num(-5), Num(-3)) ✓ (eval gives -8 = -(5+3)) *)
    let rec negate =
        function
        | Num x     -> Num (-x)
        | Add(a, b) -> Add(negate a, negate b)

    (* subtract : arith -> arith -> arith
       Subtracts b from a: a ⊖ b = a ⊕ (⊖b).
       We encode a - b as Add(a, negate b).

       Check: subtract p2 (Num 1) = Add(Add(Num 5, Num 3), Num(-1))
       eval: 5 + 3 + (-1) = 7 ✓ *)
    let subtract a b = Add(a, negate b)


(* ── Question 1.3 ────────────────────────────────────────────── *)

    (* multiply : arith -> arith -> arith
       Multiplies two arithmetic expressions WITHOUT using eval.
       Rules:
         ⌈x⌉ ⊗ ⌈y⌉   = ⌈x * y⌉           (literal × literal)
         ⌈x⌉ ⊗ (a ⊕ b) = (⌈x⌉ ⊗ a) ⊕ (⌈x⌉ ⊗ b)  (left-distributivity)
         (a ⊕ b) ⊗ c  = (a ⊗ c) ⊕ (b ⊗ c)        (right-distributivity)

       These rules expand the multiplication into a sum of products of literals.
       The result is an arith expression, NOT an integer.

       Example: multiply (Num 2) (Num 3) = Num 6
       Example: multiply (Num 2) (Add(Num 3, Num 4))
              = Add(multiply(Num 2)(Num 3), multiply(Num 2)(Num 4))
              = Add(Num 6, Num 8)  → eval = 14 = 2*(3+4) ✓ *)
    let rec multiply a b =
        match a, b with
        | Num x,     Num y     -> Num (x * y)
        | Num x,     Add(c, d) -> Add(multiply (Num x) c, multiply (Num x) d)
        | Add(c, d), _         -> Add(multiply c b, multiply d b)


(* ── Question 1.4 ────────────────────────────────────────────── *)

    (* pow : arith -> arith -> arith
       TAIL-RECURSIVE with an accumulator.
       Raises a to the power b (where b evaluates to a positive integer).

       Rules:
         a^b = a           if [[b]] = 1
         a^b = a ⊗ a^(b-1) otherwise

       We may only use eval to CHECK whether b evaluates to 1.
       We may ASSUME [[b]] ≥ 1.

       ACCUMULATOR PATTERN:
       Instead of computing a ⊗ (a ⊗ (… ⊗ a)) left-to-right (which would
       build up pending multiplications), we maintain an accumulator `acc`
       that starts at `a` and multiplies by `a` on each step.

       Trace: pow (Num 2) (Num 3)
         aux (Num 2) (Num 3) (Num 2)
       = (* eval(Num 3) ≠ 1 *)
         aux (Num 2) (subtract(Num 3)(Num 1)) (multiply (Num 2) (Num 2))
       = aux (Num 2) (Add(Num 3, Num(-1))) (Num 4)
         (* eval(Add(Num 3, Num(-1))) = 2 ≠ 1 *)
       = aux (Num 2) (Add(Add(Num 3,Num(-1)),Num(-1))) (multiply (Num 2)(Num 4))
       = aux (Num 2) (...) (Num 8)
         (* eval = 1 → return Num 8 ✓ *)

       WHY TAIL-RECURSIVE:
       The recursive call `aux a (subtract b (Num 1)) (multiply a acc)` is the
       LAST thing computed — no pending multiplication after it. *)
    let pow a b =
        let rec aux a b acc =
            if eval b = 1 then acc
            else aux a (subtract b (Num 1)) (multiply a acc)
        aux a b a


(* ── Question 1.5 ────────────────────────────────────────────── *)

    (* iterate : ('a -> 'a) -> 'a -> arith -> 'a
       Applies f to acc a total of [[a]] times.
       i.e. iterate f acc a = f (f (... (f acc) ...))  [[a]] times.

       Base: if [[a]] = 0, return acc unchanged.
       Rec:  apply f once and decrement a by 1.

       The exam says this is NOT a continuation (it's an accumulator).
       We may only use eval to check a = 0.
       We may assume [[a]] ≥ 0.

       This is TAIL-RECURSIVE: `aux (f acc) (subtract a (Num 1))` is the last
       thing done — the `f acc` is computed before the recursive call.

       pow2: implements pow using iterate.
       a^b = multiply applied to a, starting from a, iterated [[b]]-1 times.
       (We start with a and multiply by a repeatedly; after [[b]]-1 multiplications
       we have a * a * ... * a = a^[[b]].) *)
    let iterate f acc a =
        let rec aux acc a =
            if eval a = 0 then acc
            else aux (f acc) (subtract a (Num 1))
        aux acc a

    (* pow2: non-recursive implementation using iterate.
       a^b means: start from a, apply (multiply a) exactly [[b]]-1 more times.
       iterate (multiply a) a (subtract b (Num 1))
       For b=1: iterate ... a (Num 0) = a ✓
       For b=3: multiply a (multiply a a) = a^3 ✓ *)
    let pow2 a b = iterate (multiply a) a (subtract b (Num 1))


(* ═══════════════════════════════════════════════════════════════
   2: Code Comprehension (25%)
   ═══════════════════════════════════════════════════════════════ *)

    let rec foo =
        function
        | 0            -> true
        | x when x > 0 -> bar (x - 1)
        | x            -> bar (x + 1)

    and bar =
        function
        | 0            -> false
        | x when x > 0 -> foo (x - 1)
        | x            -> foo (x + 1)

    let rec baz =
        function
        | []                 -> [], []
        | x :: xs when foo x ->
            let ys, zs = baz xs
            (x::ys, zs)
        | x :: xs ->
            let ys, zs = baz xs
            (ys, x::zs)

(* ── Question 2.1 ────────────────────────────────────────────── *)

    (*

    Q: What are the types of functions foo, bar, and baz?

    A:
      foo : int -> bool
        Takes an int, pattern-matches it, and returns bool.
        (The `and bar` uses foo, both are mutually recursive.)

      bar : int -> bool
        Same type as foo: int -> bool.

      baz : 'a list -> 'a list * 'a list
        Takes a list of any type (that can be tested by foo, so 'a = int
        in practice), returns a PAIR of lists.
        The full type as used: int list -> int list * int list.

    Q: What do the functions foo, bar, and baz do.
       Focus on what they do rather than how they do it.

    A:
      HOW TO FIGURE OUT WHAT foo AND bar DO:
      Step 1 — trace some values:
        foo 0 = true
        foo 1 = bar 0 = false
        foo 2 = bar 1 = foo 0 = true
        foo 3 = bar 2 = foo 1 = bar 0 = false
        foo 4 = bar 3 = foo 2 = bar 1 = foo 0 = true
        foo(-1) = bar(-1+1) = bar 0 = false
        foo(-2) = bar(-1) = foo(-1+1) = foo 0 = true

      Step 2 — recognise the pattern:
        foo returns true for even integers and false for odd integers.
        bar returns false for even integers and true for odd integers.
        In other words: foo = isEven, bar = isOdd.

      WHY DOES THIS WORK?
      Both foo and bar handle negative numbers by moving toward 0 (+1 each step).
      They handle positives by stepping down (-1 each step).
      At 0: foo returns true (0 is even), bar returns false (0 is not odd).
      Each step swaps even/odd because adding or subtracting 1 flips parity.

      baz:
      Partitions a list of integers into two lists:
        - First list: all EVEN elements (those for which foo returns true),
                      in their original order.
        - Second list: all ODD elements (those for which foo returns false),
                       in their original order.
      baz is exactly List.partition foo for integer lists.

    Q: What would be appropriate names for functions
       foo, bar, and baz?

    A:
      foo → isEven
      bar → isOdd
      baz → partitionEvenOdd  (or partitionParity)

    *)


(* ── Question 2.2 ────────────────────────────────────────────── *)

    (*

    The function baz contains the following three code snippets.

    * A: `baz xs`
    * B: `bar x`  (appears as `foo x` in the guard, but `bar x` appears implicitly)
    * C: `(ys, x::zs)`

    Q: In the context of baz, what are the types of snippets A, B, and C,
       and what are they?

    A:
      A: baz xs  —  int list * int list
         A recursive call to baz on the tail xs.
         Returns a pair (ys, zs) where ys=evens from xs, zs=odds from xs.
         This is NOT the final result — we extend it by adding x.

      B: bar x  — bool  (though the guard uses `foo x`, same type as `bar x`)
         `bar x : bool` — returns true if x is odd.
         (In the code, the guard is `when foo x` which tests if x is even.
          If we looked at the second arm's implicit `not (foo x)`, that tests
          if x is odd, which is equivalent to `bar x`.)
         More precisely for the question: B is `bar x : bool` — checks if x is odd.

      C: (ys, x::zs)  —  int list * int list
         A tuple where x has been prepended to the odds list zs.
         This is the result for the case where x is ODD (foo x is false).
         ys stays unchanged; x is cons-ed onto zs.

    Q: Explain the `and`-operator connecting foo and bar.
       Argue if replacing `and` with `let rec` would work.

    A:
      The `and`-operator declares MUTUALLY RECURSIVE functions.
      foo calls bar, and bar calls foo — neither can be defined completely
      without the other.  If we wrote:

        let rec foo = ...   (* calls bar — but bar doesn't exist yet *)
        let rec bar = ...   (* calls foo — this one works, foo is already defined *)

      The first `let rec foo` would FAIL to compile because `bar` is referenced
      inside foo's body but hasn't been defined yet.  F# only allows a function
      to call itself with `let rec`, not another function defined later.

      With `and`:
        let rec foo = ...
        and bar = ...
      Both definitions are in scope for each other simultaneously.

      Replacing `and` with `let rec` would NOT work for the same reason:
      the second `let rec bar` definition would come after `foo`, so foo's
      reference to bar would be unresolved at the time foo is defined.

    *)


(* ── Question 2.3 ────────────────────────────────────────────── *)

    (* foo2, bar2: non-recursive versions.

       foo checks if an integer is even; the simplest non-recursive way:
         x % 2 = 0   (the remainder when dividing by 2)

       For negative numbers:
         In F#, -1 % 2 = -1 (result has the sign of the dividend).
         So `-1 % 2 = -1 ≠ 0` → isOdd ✓  (`foo (-1) = false` as traced above)
         And `-2 % 2 = 0` → isEven ✓  (`foo (-2) = true`)
         But to be safe and match the recursive definition exactly, we compare
         `abs x % 2 = 0`, which works for all integers.

       bar2 x = NOT (foo2 x) — odd means not even. *)
    let foo2 x = abs x % 2 = 0

    let bar2 x = not (foo2 x)


(* ── Question 2.4 ────────────────────────────────────────────── *)

    (*

    Q: The function `baz` is not tail recursive. Demonstrate why.

    A:
      Rule: a recursive call is in "tail position" only if it is the LAST
      operation performed — nothing waits on it to complete.

      Look at the first x :: xs case in baz (when foo x is true):
          let ys, zs = baz xs
          (x::ys, zs)

      After `baz xs` returns with (ys, zs), the computation is NOT done:
      we still need to prepend x to ys, forming the final (x::ys, zs).
      That pending cons (`x :: ys`) is held on the call stack as a stack frame.

      Be careful with the `let`-operator.  When we write:
          let ys, zs = baz xs
          (x::ys, zs)
      this is equivalent to:
          (fun (ys, zs) -> (x::ys, zs)) (baz xs)
      The function body `(fun (ys, zs) -> ...)` is still pending after
      `baz xs` returns.  The `let` does NOT mean "evaluate baz xs now and
      store it" — it means "bind the result to ys, zs for use in the body."
      The body is the pending work.

      We evaluate `baz [1; 2]` step by step
      (evaluating foo immediately as permitted):

        baz [1; 2]
      = (* foo 1 = false, so second arm *)
        let ys, zs = baz [2]          ← RECURSIVE CALL
        (ys, 1::zs)                   ← PENDING: cons 1 onto zs
      = let ys, zs = (let ys', zs' = baz []
                       (2::ys', zs'))  ← another pending cons
        (ys, 1::zs)
      = let ys, zs = (let ys', zs' = ([], [])
                       (2::ys', zs'))
        (ys, 1::zs)
      = let ys, zs = ([2], [])
        (ys, 1::zs)
      = ([2], [1])

      The step that shows non-tail-recursion: after `baz [2]` returns,
      we still need `(ys, 1::zs)`.  Two cons operations are pending on the
      stack for a list of length 2.

    *)


(* ── Question 2.5 ────────────────────────────────────────────── *)

    (* bazTail: tail-recursive version of baz using Continuation-Passing Style.

       baz returns a PAIR of lists (evens, odds).  The non-tail-recursive
       version builds each list with pending cons operations on the stack.
       In CPS, we pass a continuation `c : int list * int list -> 'r` that
       represents "what to do with (evens, odds) when we have them."

       BASE CASE ([]):
       The result is ([], []).  Call the continuation: `c ([], [])`.

       RECURSIVE CASE (x :: xs, foo x = true — x is even):
       We recurse on xs with a new continuation:
         "when you have the (ys, zs) for xs, prepend x to ys and call c."
       → aux xs (fun (ys, zs) -> c (x::ys, zs))

       RECURSIVE CASE (x :: xs, foo x = false — x is odd):
       Similar: "when you have (ys, zs) for xs, prepend x to zs and call c."
       → aux xs (fun (ys, zs) -> c (ys, x::zs))

       In both cases, `aux xs (...)` is the LAST thing done — tail position. ✓

       TRACE of bazTail [1; 2]:
         aux [1; 2] id
       = (* foo 1 = false, 1 is odd *)
         aux [2] (fun (ys,zs) -> id (ys, 1::zs))
       = (* foo 2 = true, 2 is even *)
         aux [] (fun (ys,zs) -> (fun (ys',zs') -> id (ys', 1::zs')) (2::ys, zs))
       = (fun (ys,zs) -> (fun (ys',zs') -> id (ys', 1::zs')) (2::ys, zs)) ([], [])
       = (fun (ys',zs') -> id (ys', 1::zs')) (2::[], [])
       = (fun (ys',zs') -> id (ys', 1::zs')) ([2], [])
       = id ([2], 1::[])
       = ([2], [1]) ✓ *)
    let bazTail lst =
        let rec aux lst c =
            match lst with
            | [] -> c ([], [])
            | x :: xs when foo x ->
                aux xs (fun (ys, zs) -> c (x::ys, zs))
            | x :: xs ->
                aux xs (fun (ys, zs) -> c (ys, x::zs))
        aux lst id


(* ═══════════════════════════════════════════════════════════════
   3: Balanced Brackets (25%)
   ═══════════════════════════════════════════════════════════════ *)

    let explode (str : string) = [for c in str -> c]
    let implode (lst : char list) = lst |> List.toArray |> System.String

(* ── Question 3.1 ────────────────────────────────────────────── *)

    (* balanced : string -> bool
       Returns true if str contains only balanced bracket characters {,},(,),[,].

       ALGORITHM (stack-based, as described in the exam):
       Maintain a STACK (as a list) of expected closing characters.
       Process each character:
       - '(' → push ')' onto the stack and recurse
       - '{' → push '}' onto the stack and recurse
       - '[' → push ']' onto the stack and recurse
       - closing bracket c → pop the top of the stack:
           If stack is empty: no matching open → false
           If top ≠ c: wrong pair → false
           If top = c: match! → recurse with rest of string and rest of stack
       - End of string: balanced iff stack is empty (all opens were closed)

       Example: "(){[]}":
       Process '(' → stack=[')']
       Process ')' → matches ')' → stack=[]
       Process '{' → stack=['}']
       Process '[' → stack=[']','}']
       Process ']' → matches ']' → stack=['}']
       Process '}' → matches '}' → stack=[]
       End: stack empty → true ✓ *)
    let balanced (str : string) : bool =
        let rec aux chars stack =
            match chars, stack with
            | [], []     -> true
            | [], _      -> false         // unclosed brackets remain
            | c :: cs, _ when c = '(' || c = '{' || c = '[' ->
                let closing =
                    match c with
                    | '(' -> ')' | '{' -> '}' | _ -> ']'
                aux cs (closing :: stack)
            | c :: cs, top :: rest when c = top -> aux cs rest   // matched
            | _                                 -> false          // mismatch or empty stack
        aux (explode str) []


(* ── Question 3.2 ────────────────────────────────────────────── *)

    (* balanced2 : Map<char, char> -> string -> bool
       Generalises balanced to arbitrary open→close character mappings.
       The map m specifies which characters are "openers" and what they close to.

       ALGORITHM (same stack logic, but using the map):
       - If the current character c is a KEY in m: it's an opener.
         Push m.[c] (the corresponding closer) onto the stack and recurse.
       - Otherwise, c might be a closer: pop the stack.
         If stack is empty OR top ≠ c: failure.
         If top = c: match, recurse.
       - BUT: a character can be BOTH an opener and a closer (e.g. mapping b→c
         means when we see b, we might choose to open or to close).
         The exam says we must TRY BOTH and return true if either succeeds.
         → We use (||) to attempt both paths.

       `Map.containsKey c m` checks if c is an opener.
       `Map.find c m` returns the corresponding closer.

       For the standard bracket pairs, we'd pass:
         Map.ofList [('(', ')'); ('{', '}'); ('[', ']')] *)
    let balanced2 (m : Map<char, char>) (str : string) : bool =
        let rec aux chars stack =
            match chars, stack with
            | [], []  -> true
            | [], _   -> false
            | c :: cs, _ ->
                let tryOpen =
                    if Map.containsKey c m then
                        aux cs (Map.find c m :: stack)
                    else false
                let tryClose =
                    match stack with
                    | top :: rest when top = c -> aux cs rest
                    | _                        -> false
                tryOpen || tryClose
        aux (explode str) []


(* ── Question 3.3 ────────────────────────────────────────────── *)

    (* balanced3 : string -> bool
       Non-recursive version of balanced using balanced2.
       We pass the standard bracket map and let balanced2 handle everything.

       This works because balanced is exactly balanced2 for the standard pairs. *)
    let balanced3 (str : string) : bool =
        balanced2 (Map.ofList [('(', ')'); ('{', '}'); ('[', ']')]) str

    (* symmetric : string -> bool
       A phrase is symmetric if:
         - it is empty, OR
         - csc where s is symmetric and c is any a-z letter, OR
         - s1s2 where s1 and s2 are both symmetric.

       Equivalently, the even-length string of letters (ignoring non-letters and
       case) is a palindrome.  An odd-length string can never be symmetric.

       STRATEGY:
       1. Keep only letter characters (filter out spaces, punctuation, etc.).
       2. Convert all to lower case (upper/lower are equal).
       3. Check if the resulting list is a palindrome: equal to its reverse.

       We use balanced2 with a mapping built from the letters: this approach
       is NOT what makes most sense here — simpler to check palindrome directly.

       PALINDROME CHECK: a char list is a palindrome iff it equals its reverse.
       For symmetric phrases, the string is symmetric iff the letters (lower case)
       form an even-length palindrome.

       Example: "aabbaa" → letters = ['a';'a';'b';'b';'a';'a']
                           reversed = ['a';'a';'b';'b';'a';'a'] → equal → true ✓
       Example: "Dromedaren Alpotto planerade mord!!!"
                → letters (lower) = ['d';'r';'o';'m';'e';'d';'a';'r';'e';'n';
                                      'a';'l';'p';'o';'t';'t';'o';'p';'l';'a';
                                      'n';'e';'r';'a';'d';'e';'m';'o';'r';'d']
                → reversed = same → true ✓  (it's a palindrome) *)
    let symmetric (s : string) : bool =
        let letters =
            s
            |> explode
            |> List.filter System.Char.IsLetter
            |> List.map System.Char.ToLower
        letters = List.rev letters


(* ── Question 3.4 ────────────────────────────────────────────── *)

    open JParsec.TextParser

    let ParseBalanced, bref = createParserForwardedToRef<unit>()

    (* parseBalancedAux: a JParsec parser for balanced bracket strings.

       GRAMMAR:
       A balanced string is a sequence (possibly empty) of balanced "chunks",
       where each chunk is one of:
         ()   →  '(' then balanced then ')'
         {}   →  '{' then balanced then '}'
         []   →  '[' then balanced then ']'

       COMBINATORS USED:
       `pchar c` — parses exactly the character c.
       `p1 >>. p2` — runs p1 then p2, returns p2's result.
       `p1 .>> p2` — runs p1 then p2, returns p1's result.
       `p1 >>= fun _ -> p2` — sequences p1 and p2, ignoring p1's result.
       `many p` — runs p zero or more times, returning a list of results.
       `p |>> f` — maps f over p's result.

       A single bracket pair: e.g. '(' then ParseBalanced then ')'
       parseBalancedAux = many (one bracket pair) |>> ignore

       We use ParseBalanced (the forwarded ref) inside each pair, allowing
       nesting. After defining parseBalancedAux, we set bref := parseBalancedAux
       to "close the loop."

       parseBalanced = parseBalancedAux .>> pstring "**END**"
       The "**END**" sentinel is NOT part of the balanced string — it's just
       the termination marker for the parser. *)
    let parseBalancedAux =
        many (
            (pchar '(' >>. ParseBalanced .>> pchar ')')
            <|> (pchar '{' >>. ParseBalanced .>> pchar '}')
            <|> (pchar '[' >>. ParseBalanced .>> pchar ']')
        ) |>> ignore

    let parseBalanced = parseBalancedAux .>> pstring "**END**"

    do bref := parseBalancedAux


(* ── Question 3.5 ────────────────────────────────────────────── *)

    (* countBalanced : string list -> int -> int
       Counts how many strings in lst are balanced (brackets only), using x
       parallel threads where lst is split into x roughly-equal chunks.

       List.byChunkSize k lst splits lst into chunks of size AT MOST k.
       Note: the argument is the CHUNK SIZE, not the NUMBER of chunks.
       To get x chunks from a list of length n: chunk size = n / x (ceiling).

       PARALLEL PATTERN:
       1. Split lst into chunks of size (List.length lst) / x.
       2. For each chunk, create an async that counts balanced strings in it.
       3. Run all asyncs in parallel.
       4. Sum the counts.

       `balanced` only accepts the six bracket characters; the exam says we
       only consider strings "containing only the characters {,},(,),[,]".
       We use our `balanced` function directly. *)
    let countBalanced (lst : string list) (x : int) : int =
        let n = List.length lst
        let chunkSize = max 1 ((n + x - 1) / x)   // ceiling division
        lst
        |> List.chunkBySize chunkSize
        |> List.map (fun chunk ->
            async { return List.sumBy (fun s -> if balanced s then 1 else 0) chunk }
        )
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Array.sum


(* ═══════════════════════════════════════════════════════════════
   4: BASIC (25%)
   ═══════════════════════════════════════════════════════════════ *)

    type var = string

    type expr =
    | Num    of int              // Integer literal
    | Lookup of var              // Variable lookup
    | Plus   of expr * expr      // Addition
    | Minus  of expr * expr      // Subtraction

    type stmnt =
    | If of expr * uint32       // Conditional statement (if-then, no else)
    | Let of var * expr         // Variable update/declaration
    | Goto of uint32            // Goto
    | End                       // Terminate program

    type prog = (uint32 * stmnt) list

    let (.+.) e1 e2 = Plus(e1, e2)
    let (.-.) e1 e2 = Minus(e1, e2)

    let fibProg xarg =
        [(10u, Let("x",    Num xarg))
         (20u, Let("acc1", Num 1))
         (30u, Let("acc2", Num 0))
         (40u, If(Lookup "x", 60u))
         (50u, Goto 110u)
         (60u, Let ("x", Lookup "x" .-. Num 1))
         (70u, Let ("result", Lookup "acc1"))
         (80u, Let ("acc1", Lookup "acc1" .+. Lookup "acc2"))
         (90u, Let ("acc2", Lookup "result"))
         (100u, Goto 40u)
         (110u, End)
         ]

(* ── Question 4.1 ────────────────────────────────────────────── *)

    (* DESIGN:
       type basicProgram = Map<uint32, stmnt>
       Already defined above (before Q4.1).

       A Map from line numbers (uint32) to statements.
       Map gives O(log n) lookup for getStmnt and nextLine.
       The exam says "all one-liners if you use the correct functions."

       mkBasicProgram: converts a prog (list of (lineNum, stmnt) pairs) to a Map.
         Map.ofList directly constructs a map from a list of (key, value) pairs.

       getStmnt l p: returns the statement at line l.
         Map.find l p — standard map lookup (raises exception if not found, but
         the exam says we don't need to handle missing lines).

       nextLine l p: returns the smallest line number l' > l in p.
         We want the minimum key greater than l.
         Map.filter (fun k _ -> k > l) p gives a map of all lines after l.
         Map.minKeyValue gives the smallest key-value pair.

       firstLine p: the smallest line number in p.
         Map.minKeyValue p |> fst — the minimum key. *)
    type basicProgram = Map<uint32, stmnt>

    let mkBasicProgram (p : prog) : basicProgram =
        Map.ofList p

    let getStmnt (l : uint32) (p : basicProgram) : stmnt =
        Map.find l p

    let nextLine (l : uint32) (p : basicProgram) : uint32 =
        Map.filter (fun k _ -> k > l) p |> Map.minKeyValue |> fst

    let firstLine (p : basicProgram) : uint32 =
        Map.minKeyValue p |> fst


(* ── Question 4.2 ────────────────────────────────────────────── *)

    (* state TYPE DESIGN:
       We need to store:
         (a) currentLine: the current line number (uint32)
         (b) variables: a Map<string, int> for the variable environment

       We use an F# record for clarity.

       emptyState p: start at the first line, empty variable environment.
       goto l st: return a new state with lineNumber updated to l.
       getCurrentStmnt p st: look up the statement at st.lineNumber.
       update v a st: add/overwrite variable v → a in the environment.
       lookup v st: retrieve variable v from the environment. *)
    type state =
        { lineNumber : uint32
          variables  : Map<string, int> }

    let emptyState (p : basicProgram) : state =
        { lineNumber = firstLine p; variables = Map.empty }

    let goto (l : uint32) (st : state) : state =
        { st with lineNumber = l }

    let getCurrentStmnt (p : basicProgram) (st : state) : stmnt =
        getStmnt st.lineNumber p

    let update (v : var) (a : int) (st : state) : state =
        { st with variables = Map.add v a st.variables }

    let lookup (v : var) (st : state) : int =
        Map.find v st.variables


(* ── Question 4.3 ────────────────────────────────────────────── *)

    (* evalExpr : expr -> state -> int
       Evaluates an expression in the current state.
       - Num x      → x
       - Lookup v   → look up variable v in the state's variable environment
       - Plus(e1,e2) → eval e1 + eval e2
       - Minus(e1,e2)→ eval e1 - eval e2

       step : basicProgram -> state -> state
       Advances the current line to the NEXT line in the program.
       Does NOT consider whether the current statement is a Goto.
       One-liner: `{ st with lineNumber = nextLine st.lineNumber p }`

       evalProg : basicProgram -> state -> state
       Runs the BASIC program starting from the current state.
       Looks up the current statement and dispatches:
       - If(e, l): evaluate e; if nonzero, goto l; else step to next line
       - Let(v, e): evaluate e, update v, step to next line
       - Goto l:   jump to l (no step — goto targets the line directly)
       - End:      terminate, return current state *)
    let rec evalExpr (e : expr) (st : state) : int =
        match e with
        | Num x        -> x
        | Lookup v     -> lookup v st
        | Plus(e1, e2) -> evalExpr e1 st + evalExpr e2 st
        | Minus(e1,e2) -> evalExpr e1 st - evalExpr e2 st

    let step (p : basicProgram) (st : state) : state =
        { st with lineNumber = nextLine st.lineNumber p }

    let rec evalProg (p : basicProgram) (st : state) : state =
        match getCurrentStmnt p st with
        | If(e, l)  ->
            if evalExpr e st <> 0 then evalProg p (goto l st)
            else evalProg p (step p st)
        | Let(v, e) ->
            evalProg p (step p (update v (evalExpr e st) st))
        | Goto l    -> evalProg p (goto l st)
        | End       -> st


(* ── Question 4.4 ────────────────────────────────────────────── *)

    (* THE STATE MONAD FOR BASIC:
       type StateMonad<'a> = SM of (basicProgram -> state -> 'a * state)

       KEY DIFFERENCES from the Q4.3 (2023) monad:
       1. No Option wrapper — there is no error handling.
       2. The function takes BOTH basicProgram AND state.
          The program p never changes during execution (it's read-only), but
          bind must propagate it through.
       3. evalSM p (SM f) = f p (emptyState p) — initialises the state.

       This models a computation that:
         - can read the current program (p is always available)
         - can read/update the current state
         - always succeeds (no failure)

       All one-liners using Q4.1 and Q4.2 functions:

       goto2 l: update the state's lineNumber to l.
         SM (fun _ s -> ((), goto l s))

       getCurrentStmnt2: get the current statement from the program.
         SM (fun p s -> (getCurrentStmnt p s, s))

       update2 v a: update variable v to a in the state.
         SM (fun _ s -> ((), update v a s))

       lookup2 v: look up variable v in the state.
         SM (fun _ s -> (lookup v s, s))

       step2: advance to next line.
         SM (fun p s -> ((), step p s)) *)
    type StateMonad<'a> = SM of (basicProgram -> state -> 'a * state)

    let ret x = SM (fun _ s -> (x, s))

    let bind f (SM a) : StateMonad<'b> =
        SM (fun p s ->
            let x, s' = a p s
            let (SM g) = f x
            g p s')

    let (>>=) x f = bind f x
    let (>>>=) x y = x >>= (fun _ -> y)

    let evalSM p (SM f) = f p (emptyState p)

    let goto2 (l : uint32) : StateMonad<unit> =
        SM (fun _ s -> ((), goto l s))

    let getCurrentStmnt2 : StateMonad<stmnt> =
        SM (fun p s -> (getCurrentStmnt p s, s))

    let lookup2 (v : var) : StateMonad<int> =
        SM (fun _ s -> (lookup v s, s))

    let update2 (v : var) (a : int) : StateMonad<unit> =
        SM (fun _ s -> ((), update v a s))

    let step2 : StateMonad<unit> =
        SM (fun p s -> ((), step p s))


(* ── Question 4.5 ────────────────────────────────────────────── *)

    (* StateBuilder: maps computation expression syntax to monadic operations.

       evalExpr2 : expr -> SM<int>
       Works like evalExpr but the state (and program) is abstracted in the monad.
       Lookup uses lookup2 to read variables monadically.

       evalProg2 : SM<unit>
       Works like evalProg but threaded through the state monad.
       getCurrentStmnt2 reads the current statement without exposing the state.
       goto2, step2, update2 modify state monadically. *)

    type StateBuilder() =
        member this.Bind(f, x)    = bind x f
        member this.Return(x)     = ret x
        member this.ReturnFrom(x) = x
        member this.Combine(a, b) = a >>= (fun _ -> b)

    let state = StateBuilder()

    let rec evalExpr2 (e : expr) : StateMonad<int> =
        match e with
        | Num x        -> ret x
        | Lookup v     -> lookup2 v
        | Plus(e1, e2) ->
            evalExpr2 e1 >>= fun v1 ->
            evalExpr2 e2 >>= fun v2 ->
            ret (v1 + v2)
        | Minus(e1, e2) ->
            evalExpr2 e1 >>= fun v1 ->
            evalExpr2 e2 >>= fun v2 ->
            ret (v1 - v2)

    let rec evalProg2 : StateMonad<unit> =
        getCurrentStmnt2 >>= fun stmnt ->
        match stmnt with
        | If(e, l) ->
            evalExpr2 e >>= fun v ->
            if v <> 0 then goto2 l >>>= evalProg2
            else step2 >>>= evalProg2
        | Let(v, e) ->
            evalExpr2 e >>= fun a ->
            update2 v a >>>= step2 >>>= evalProg2
        | Goto l ->
            goto2 l >>>= evalProg2
        | End ->
            ret ()
