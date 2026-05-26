# FP Exam Quick-Lookup Reference (2019–2025)

Organized by topic pattern. Find a similar problem → check that year's solution.

---

## 2025

**Q1.1** `valid`: predicate on a `rectangle` (width > 0 and height > 0).  
**Q1.2** `coords`: non-tail-recursive function building a `Set<coord>` of all integer grid points inside a rectangle, column by column.  
**Q1.3** `coordsAcc`: tail-recursive accumulator version of `coords`.  
**Q1.4** `merge`: HOF pipeline — filter valid rectangles, map to coord sets, fold with `Set.union`.  
**Q1.5** `foldRect`: generic fold over every coordinate in a rectangle using nested recursion (no library functions); `coords2` reimplemented via `foldRect`.

**Q2.1** `foo`/`bar` code comprehension: `foo` merges two sorted lists; `bar` sorts a list (merge sort). Name them and state constraints on `foo`'s arguments.  
**Q2.2** Match-order swap analysis in `bar`: explain what happens when lines 1/2, 1/3, or 2/3 are swapped (base case reordering → infinite recursion).  
**Q2.3** `bar` performance: `List.length` is O(n) and called at every recursion level; `bar2` fixes it with O(1) pattern-match base cases.  
**Q2.4** `foo` non-tail-recursion proof via evaluation trace (cons cell pending on stack).  
**Q2.5** `footail`: CPS (continuation-passing) version of the sorted-list merge function.

**Q3.1** Dining philosophers — mutable `bool[]` table; `getLeftFork`/`getRightFork`/`putLeftFork`/`putRightFork` with error messages; `canEat` check.  
**Q3.2** `eat`/`think` composite actions; `canEat` predicate on mutable table.  
**Q3.3** `MailboxProcessor` with `Eat(p, rc)` and `Think(p)` messages; pending queue for blocked philosophers; unblock on `Think`.  
**Q3.4** `newTable`/`philEat`/`philThink` wrapping the mailbox; `philEat` blocks until seats available.  
**Q3.5** `philosopher` async loop (eat → sleep → think → sleep, n meals); `diningPhilosophers` with `Async.Parallel`.

**Q4.1** Towers of Hanoi types: `peg = Start|Middle|Goal`, `disc = D of int`, `hanoi = H of Map<peg, disc list>`; `newGame`; `isFinished` check.  
**Q4.2** `take`/`place` returning `Result<_, error>` (Empty / Invalid peg errors).  
**Q4.3** `hanoiMonad<'a>` state monad — `ret`/`fail`/`bind`/`>>=`; `take2`/`place2` lifting Result into monad.  
**Q4.4** `HanoiBuilder` computation expression; `move`/`doMoves`/`solveHanoi` recursive solution list.  
**Q4.5** JParsec parsers: `parsePeg` (`start`/`middle`/`goal`), `parseMove` (`peg->peg;`), `parseMoves` (`many`).

---

## 2025re / 2024re (same exam, Aug 2024)

**Q1.1** `area`: pattern-match on `shape = Rectangle|Circle|Triangle`, compute area with different formulas.  
**Q1.2** `circumference`: same shape DU, different formula per case (includes hypotenuse for triangle).  
**Q1.3** `type shapeList = Empty | AddShape of shape*shape*shapeList` (always pairs of shapes); `totalArea` non-tail-recursive.  
**Q1.4** `totalCircumference`: tail-recursive accumulator over the custom `shapeList` type.  
**Q1.5** `shapeListFold`: generic fold over `shapeList`; `totalArea2`/`totalCircumference2` via fold.

**Q2.1** `foo` code comprehension: Atbash-like char encoder (shift letters, pass whitespace). `bar` converts string to char list. `baz` applies `foo` to each char and concatenates = Atbash encode string.  
**Q2.2** Incomplete pattern warning in `foo`; `foo2` that removes the warning.  
**Q2.3** `baz2`: HOF version using `List.map` + `String.concat`.  
**Q2.4** `baz` non-tail-recursion proof via evaluation trace.  
**Q2.5** `bazTail`: CPS version of `baz`.

**Q3.1** `encrypt`/`decrypt` strings using Atbash (self-inverse cipher).  
**Q3.2** `splitAt`: split a string into chunks of a given size.  
**Q3.3** `parEncrypt`: parallel encryption of string chunks with `Async.Parallel`.  
**Q3.4** `parseEncrypt`: JParsec parser matching letters/spaces and encrypting them on the fly.

**Q4.1** `type clicker` — tally clicker with multiple wheels, each over a list of chars; `newClicker` creates n identical wheels.  
**Q4.2–4.5** Press, display, and advance functions on the clicker DU.

---

## 2024

**Q1.1** `type binList = Cons1 of int*binList | Cons2 of int*int*binList | Nil`; `length` non-tail-recursive.  
**Q1.2** `length2`: tail-recursive accumulator; `split` helper.  
**Q1.3** `map`: two separate mapping functions applied to `Cons1` / `Cons2` elements respectively.  
**Q1.4** `filter`: two predicates, one per constructor type.  
**Q1.5** `fold`: two accumulating functions, one per constructor.

**Q2.1** `foo`/`bar` code comprehension using `and` keyword (mutually defined merge + merge sort with `List.splitAt`).  
**Q2.2** `foo2`: same behavior using `List.unfold`.  
**Q2.3** Efficiency: why computing length repeatedly is O(n) per level; `bar2` with `List.splitAt`.  
**Q2.4** `fooTail`: tail-recursive accumulator version of merge.  
**Q2.5** `barTail`: CPS version of merge sort.

**Q3.1** `approxSquare`: Newton-Raphson square-root iteration until convergence.  
**Q3.2** `quadratic`: returns both roots of ax²+bx+c=0 as a pair.  
**Q3.3** `parQuadratic`: solve many equations in parallel with `Async.Parallel`.  
**Q3.4** `solveQuadratic`: JParsec parser for `ax^2+bx+c=0` format, returning roots.

**Q4.1** `type rat = int*int`; `mkRat` with GCD normalization; `ratToString`.  
**Q4.2** `plus`/`minus`/`mult`/`div` returning `option` (fail on div-by-zero).  
**Q4.3** State monad `SM<'a>` — `ret`/`fail`/`bind`; `smPlus`/`smMinus`/`smMult`/`smDiv`.  
**Q4.4** `calculate`: fold list of `(rat, op)` pairs through the state monad.

---

## 2023re (Aug 2023)

**Q1.1** `type arith = Num of int | Add of arith*arith`; `eval : arith -> int`.  
**Q1.2** `negate`: structural negation (no eval) distributing over Add nodes.  
**Q1.3** `subtract a b = Add(a, negate b)`.  
**Q1.4** `multiply`: structural distribution of multiplication over Add (no eval).  
**Q1.5** `pow`: tail-recursive accumulator (exponentiation by repeated multiply, uses `eval` only to check exponent = 1).  
**Q1.6** `iterate f acc e`: applies f to acc exactly `eval e` times; `pow2` via `iterate`.

**Q2.1** `foo`/`bar` mutually recursive with `and`: `foo` = isEven, `bar` = isOdd (both defined by decrement recursion). `baz` partitions list into (evens, odds).  
**Q2.2** Why `and` is needed; what breaks if replaced by `let rec`.  
**Q2.3** `foo2`/`bar2`: non-recursive using `%`.  
**Q2.4** `baz` tail-recursion proof.  
**Q2.5** `bazTail`: CPS version of `baz`.

**Q3.1** `balanced`: check balanced brackets `{([` vs `})]` using a stack.  
**Q3.2** `balanced2`: generalized with `Map<char,char>` for arbitrary delimiter pairs; backtracking.  
**Q3.3** `balanced3`: uses `balanced2`.  
**Q3.4** `symmetric`: even-length palindrome check ignoring non-letters.

---

## 2023

**Q1.1** `type prop = TT|FF|And of prop*prop|Or of prop*prop`; `eval : prop -> bool`.  
**Q1.2** `negate`: De Morgan structural negation.  
**Q1.3** `implies p q = Or(negate p, q)`.  
**Q1.4** `forall f xs`: tail-recursive accumulator building And-conjunction over a list.  
**Q1.5** `exists f xs`: builds Or-disjunction.  
**Q1.6** `existsOne f xs`: exactly one element satisfies predicate.

**Q2.1** `foo`/`bar` code comprehension: `foo` finds first occurrence of one list as prefix in another; `bar` applies `foo` repeatedly to find all occurrences; `baz` checks common subsequence of two strings.  
**Q2.2** `foo2`: non-recursive using `List.splitAt`.  
**Q2.3** `bar` non-tail-recursion proof.  
**Q2.4** `barTail`: CPS version.

**Q3.1** `collatz`: returns full Collatz sequence from n to 1 (fail on negative; linear time with accumulator).  
**Q3.2** `evenOddCollatz`: count even and odd numbers in the Collatz sequence.  
**Q3.3** `maxCollatz`: find number in range with longest Collatz sequence.  
**Q3.4** `collect`: group numbers by Collatz sequence length into `Map<int, Set<int>>`.  
**Q3.5** `parallelMaxCollatz`: parallel version splitting range into n chunks.

**Q4.1** `balanced`/`balanced2`/`balanced3`/`symmetric` (see 2023re Q3 above — identical).  
**Q4.2** `type register = R1|R2|R3`; `type assembly = MOVI|MULT|SUB|JGTZ`; `assemblyToProgram`.  
**Q4.3** `type state`; `emptyState`; assembly interpreter `step`/`run`.

---

## 2022re (Aug 2022)

**Q1.1** `maxDepth : grayscale -> int` — max nesting depth of quadtree (`Square` = 0).  
**Q1.2** `mirror : grayscale -> grayscale` — mirror quadtree around Y-axis (swap q1↔q2 and q3↔q4 recursively).  
**Q1.3** `operate : (g->g->g->g->g) -> grayscale -> grayscale` — generic structural combinator over a quadtree.  
**Q1.4** `mirror2` via `operate`.  
**Q1.5** `compress`: collapse `Quad(a,b,c,d)` to `Square` if all four children are equal (bottom-up).

**Q2.1** `foo`/`bar` code comprehension: `foo f xs` filters list keeping elements where `f` holds; `bar fs xs` applies each function in `fs` sequentially to `xs`.  
**Q2.2** Underscore usage in patterns; `bar2` HOF; `baz` such that `bar fs xs = foo (baz fs) xs`.  
**Q2.3** Which of `foo`/`bar` is not tail-recursive and why.  
**Q2.4** `fooTail` or `barTail`: CPS version.

**Q3.1** `type oracle = {max:int; f:int->guessResult}`; `validOracle` correctness check.  
**Q3.2** `randomOracle`: picks random target; `findNumber` binary search returning guess history.  
**Q3.3** `evilOracle`: adversarial oracle that delays commitment to maximize search (mutable range tracking).  
**Q3.4** `parFindNumbers`: parallel binary search on a list of oracles.

**Q4.1** `'a ring = 'a list * 'a list`; `length`; `ringFromList`; `ringToList`; `empty`.  
**Q4.2** `push` (prepend CCW); `peek` (CCW head option); `pop` (CCW head + remaining ring).  
**Q4.3** `moveClockwise`/`moveCounterClockwise` — shift current position around the ring.

---

## 2022

**Q1.1** `type grayscale = Square of uint8 | Quad of grayscale*grayscale*grayscale*grayscale`; `countWhite` non-tail-recursive.  
**Q1.2** `rotateRight`: 90° clockwise rotation of quadtree (q1→q2→q3→q4→q1).  
**Q1.3** `map : (uint8->grayscale) -> grayscale -> grayscale`; `bitmap` via map (threshold at 127).  
**Q1.4** `fold : ('a->uint8->'a) -> 'a -> grayscale -> 'a`; `countWhite2` via fold.

**Q2.1** `foo`/`bar` code comprehension: `foo` converts int to binary string; `bar` maps `foo` over a list. Incomplete pattern warning in `foo` and why.  
**Q2.2** `foo2`: fixed version without warning; `bar2` HOF.  
**Q2.3** Neither `foo` nor `bar` is tail-recursive; why `bar` risks stack overflow more.  
**Q2.4** `fooTail`: tail-recursive accumulator; `barTail`: CPS.

**Q3.1** `type matrix = int[,]`; `init`/`numRows`/`numCols`/`get`/`set`/`print`; `failDimensions`.  
**Q3.2** `add`: element-wise matrix addition with dimension check.  
**Q3.3** `dotProduct`: dot product of row i from m1 with column k from m2.  
**Q3.4** `mult`: matrix multiplication using `dotProduct`; `parInit`: parallel matrix init with `Async.Parallel`.

**Q4.1** `type cmd = Push of int | Add | Mult`; `type stackProgram = cmd list`; stack machine executor.

---

## 2021re (Aug 2021)

Same as **2024 Q1–Q3** (binary lists, merge sort comprehension, sqrt approximation).

**Q4.1** `type rat`; `mkRat`/`ratToString`/`plus`/`minus`/`mult`/`div` (same as 2024 Q4).  
**Q4.2** State monad operations on rat; `smLength`/`smPush`/`smPop`/`smCW`/`smCCW` ring operations.  
**Q4.3** `ringStep`: check first two CCW elements, remove pair if sum is even else move CCW.  
**Q4.4** `iterRemoveSumEven`: run `ringStep` n times.

---

## 2021

**Q1.1** `type direction = North|East|South|West`; `type coord = C of int*int`; `move : int -> direction -> coord -> coord`.  
**Q1.2** `turnRight`/`turnLeft`; `type position = P of coord*direction`; `type move = TurnLeft|TurnRight|Forward of int`; `step`.  
**Q1.3** `walk`: recursive walk through move list; `walk2` HOF using `List.fold`.  
**Q1.4** `path`: collect visited coords on Forward moves only (linear complexity).  
**Q1.5** `path2`: tail-recursive accumulator version of `path`.  
**Q1.6** `path3`: CPS version of `path`.

**Q2.1** `foo`/`bar`/`baz` code comprehension: `foo f` = memoized version of `f` (mutable Map cache); `bar` = Fibonacci; `baz = foo bar` = memoized Fibonacci. Role of `mutable` and `and` keywords.  
**Q2.2** Incomplete pattern in `foo`; `foo2` without redundancy.  
**Q2.3** `barbaz`: combined single function (slower — loses memo sharing between recursive calls).  
**Q2.4** `bazSeq : int seq` — infinite Fibonacci sequence via `Seq.unfold`.

**Q3.1** Look-and-say: `type element` = run-length encoded digit groups; `elToString`/`elFromString`.  
**Q3.2** `nextElement`: produce next look-and-say element from current one.  
**Q3.3** `elSeq`/`elSeq2`: infinite look-and-say sequence via `Seq.unfold` and seq comprehension.  
**Q3.4** `elParse`: JParsec parser for a well-formed element string; `elFromString2` via parser.

**Q4.1** `'a ring = 'a list * 'a list`; `length`/`ringFromList`/`ringToList`/`empty` (see 2022re Q4).  
**Q4.2** `push`/`peek`/`pop`; `moveClockwise`/`moveCounterClockwise`.

---

## 2020

**Q1.1** `insert`: insert element into sorted list, non-tail-recursive.  
**Q1.2** `insertionSort`: sort using `insert`; `insertTail` accumulator version; `insertionSortTail`.  
**Q1.3** `insertionSort2` HOF; `insertBy` with comparison function; `insertionSortBy`.

**Q2.1** `foo`/`bar`/`baz` code comprehension: `foo` = remove adjacent duplicates from sorted list; `bar` = isSorted predicate; `baz` = partition list into maximal sorted runs.  
**Q2.2** Incomplete pattern warning; `foo2`/`bar2` fixed versions.  
**Q2.3** `baz2` HOF; non-tail-recursion proof; `fooTail`/`bazTail` CPS.

**Q3.1** `type bigInt = int list` (digits); `fromString`/`toString`; `add` long addition with carry.  
**Q3.2** `multSingle`: multiply bigInt by single digit; `mult`: full big-integer multiplication.  
**Q3.3** `fact`: parallel factorial with `Async.Parallel`.

**Q4.1** `type llist = Cons of unit -> 'a * 'a llist`; `step`/`cons`/`init`/`map`/`filter`/`takeFirst`.  
**Q4.2** `unfold : ('b -> ('a * 'b) option) -> 'b -> 'a llist`; Fibonacci via unfold.

---

## 2020re (Aug 2020)

**Q1.1** `type bintree`; `insert`: maintain BST invariant; `fromList` tail-recursive.  
**Q1.2** `fold` inorder + `foldBack`; `inOrder : bintree -> 'a list` via fold.  
**Q1.3** `map`: map over BST via fold.

**Q2–Q4** Same themes as 2020 (code comprehension, big integers, lazy lists).

---

## 2019

**Q1.1** `type Peano = O | S of Peano`; `toInt`/`fromInt`; `add`/`mult`/`pow` via recursion.  
**Q1.2** `tailAdd`/`tailMult`/`tailPow`: tail-recursive accumulator versions.  
**Q1.3** `loop f p x`: apply `f` exactly `toInt p` times; `loopAdd`/`loopMult`/`loopPow` non-recursive using `loop`.

**Q2.1** `f`/`g` code comprehension: `f` finds first occurrence of one list as prefix and returns remainder as option; `g` checks if one list is a subsequence of another.  
**Q2.2** `fOpt`/`gOpt` safe versions; tail-recursive proof; CPS.

**Q3.1** `calculatePi`: Nilakantha series approximation; `piSeq`: infinite pi sequence via `Seq.unfold`.  
**Q3.2** `circleArea`/`sphereVolume`; `circleSphere`: infinite seq of (area, volume) pairs.  
**Q3.3** `parallelPi`: parallel approximation splitting iterations.

**Q4.1** `type Tape`; `tapeFromList`/`tapeToList`; `moveHead` left/right; `readTape`/`writeTape`.  
**Q4.2** `evalAction`; `evalProgram`: bidirectional tape machine interpreter.

---

## 2019re (Aug 2019)

**Q1.1** `type Sum<'a,'b> = Left of 'a | Right of 'b`; `sumMap`.  
**Q1.2** `type SumColl<'a,'b> = Nil | CLeft of 'a*SumColl | CRight of 'b*SumColl`; `ofList`.  
**Q1.3** `reverse`: tail-recursive accumulator over `SumColl`.  
**Q1.4** `ofList2` HOF; `foldBackSumColl`.

**Q2.1** `f`/`g` code comprehension: `f` converts string to char list (by index); `g` palindrome check.  
**Q2.2** `f2` using list comprehension; `g2` using `>>` composition; `fTail` CPS; `gOpt` single-pass.

**Q3.1** `calculateGoldenRatio`: iterative approximation; `grSeq` via `Seq.unfold`.  
**Q3.2** `goldenRectangleSeq`/`triangleSeq`; pairs seq comprehension.

**Q4.1** Qwirkle: `type tile` with shape + colour enums; `validTiles` predicate list.  
**Q4.2** `moveCoord`/`collectTiles`; `placeTile` single tile; `placeTiles` via option monad.

---

## Pattern Index

| Pattern | Where to look |
|---|---|
| Non-tail-recursive → tail-recursive accumulator | Every exam Q1 |
| CPS (continuation-passing style) | Every exam Q2.4–2.5 |
| Code comprehension + name functions | Every exam Q2.1 |
| Incomplete pattern match warning + fix | 2022 Q2, 2022re Q2, 2025re Q2, 2023re Q2 |
| `and` keyword (mutual recursion) | 2024 Q2, 2023re Q2, 2021 Q2 |
| `Seq.unfold` / infinite sequence | 2021 Q2.4, 2019 Q3, 2020re Q4 |
| Lazy list `Cons of unit -> ...` | 2020 Q4, 2020re Q4 |
| `Async.Parallel` parallel computation | 2025 Q3.5, 2025re Q3.3, 2024 Q3.3, 2023 Q3.5, 2022 Q3.4, 2020 Q3.3 |
| `MailboxProcessor` / message passing | 2025 Q3.3–3.4 |
| State monad (`ret`/`fail`/`bind`/`>>=`) | 2025 Q4, 2024 Q4, 2021re Q4 |
| Computation expression (`Builder`) | 2025 Q4.4 |
| JParsec parsers | 2025 Q4.5, 2025re Q3.4, 2024 Q3.4, 2021 Q3.4 |
| Recursive algebraic type + `eval` | 2023re Q1 (arith), 2023 Q1 (prop) |
| Quadtree / grayscale image | 2022 Q1, 2022re Q1 |
| Ring data structure | 2022re Q4, 2021 Q4, 2021re Q4 |
| Binary search tree | 2020re Q1 |
| Peano numbers | 2019 Q1 |
| Custom linked list (non-standard) | 2024 Q1 (binList), 2019re Q1 (SumColl) |
| Big integers (digit lists) | 2020 Q3, 2020re Q3 |
| Matrix operations | 2022 Q3 |
| Stack machine | 2022 Q4, 2023 Q4 |
| Grid/coordinate movement | 2021 Q1, 2025 Q1 |
| Balanced brackets | 2023 Q4, 2023re Q3 |
| Memoization + mutable Map | 2021 Q2 |

---

## How to Find the Right Previous Solution

Use this as a step-by-step triage guide. Work through the steps in order until you land on a section.

---

### Step 1 — Which question number is it?

The exam always has 4 questions. Their roles are almost fixed across all years:

| Question | What it almost always is |
|---|---|
| **Q1** | A custom F# type is defined. You write 4–6 functions that work with it, progressing from simple to complex. |
| **Q2** | Mystery functions `foo`, `bar`, `baz` are given. You explain what they do, fix warnings, rewrite tail-recursively or in CPS. |
| **Q3** | A concrete algorithm (sorting, parsing, parallel computation). Often ends with `Async.Parallel` or a JParsec parser. |
| **Q4** | A more advanced abstraction: state monad, MailboxProcessor, lazy list, rings, or a game/simulation. |

If you know which question number it is, you can **immediately skip to that question's section** in the year entries above.

---

### Step 2 — What does the progression of sub-questions look like?

Exams follow very predictable progressions within each question. Match the sub-question you are stuck on:

**Stuck on the first 1–2 sub-questions of Q1?**  
→ Simple recursive function over a custom type. Look for the same type shape (tree, list, pair, grid) in the Q1 entries. The first sub-question is almost always a predicate or a `count`/`eval` function.

**Stuck on a sub-question that says "tail-recursive" or "accumulator"?**  
→ It is always asking you to rewrite a just-written non-tail-recursive function using an `acc` parameter. Look at the previous sub-question's solution in the same year and add `aux` + accumulator. See every Q1.2–1.3 across all years.

**Stuck on a sub-question that says "higher-order" or "using `List.fold`/`List.map`"?**  
→ You are supposed to collapse a recursive function into a one-liner using library HOFs. The answer is almost always `List.fold`, `List.map`, `List.filter`, or `Set.fold`. Look at Q1.4–1.5 in any year.

**Stuck on a sub-question that says "using fold" where `fold` was defined earlier in the same question?**  
→ Use the `fold` you just wrote (not a library fold). Pass it the right accumulating function. See 2025 Q1.5, 2022 Q1.4, 2025re Q1.5.

---

### Step 3 — Is it a Q2 "code comprehension" question?

All Q2s follow the same 5-part script. Find which part you are on:

| Sub-question asks… | What it wants | Where to look |
|---|---|---|
| "What does `foo` do? Give it a better name." | Read it carefully. Trace 2–3 examples by hand. Name = what the function computes. | Any Q2.1 entry |
| "What constraint must hold on the input?" | Think about what the function silently assumes (e.g. input must be sorted). | 2025 Q2.1, 2024 Q2.1 |
| "What happens if you swap these two match lines?" | Work out which case now matches first. If a wildcard moves up, it swallows everything below it → infinite recursion or wrong result. | 2025 Q2.2 |
| "Why is this not as performant?" | Look for a list traversal (e.g. `List.length`) called inside a recursive function. O(n) per recursion level = hidden quadratic cost. | 2025 Q2.3, 2024 Q2.3 |
| "There is an incomplete pattern warning. Fix it." | Add the missing case, or restructure so the compiler can see it is covered. | 2022 Q2, 2025re Q2, 2023re Q2 |
| "Prove `foo` is not tail-recursive." | Write out 2–4 evaluation steps by hand. Show a pending operation (`::`  cons, `+`, etc.) waiting on the stack after the recursive call. | 2025 Q2.4, any Q2.4 |
| "Write a tail-recursive / accumulator version." | Add an `aux` inner function with an extra `acc` parameter. Base case returns `acc`. | Every Q2, 2nd-to-last sub-question |
| "Write a CPS version." | Add a continuation `c` parameter. Instead of `x :: recursiveCall`, do `recursiveCall (fun result -> c (x :: result))`. Base case calls `c` directly. Start outer call with `aux a b id`. | Every Q2, last sub-question |

---

### Step 4 — Is it a Q3 algorithm/parallel/parser question?

Q3 always builds up in 3–4 sub-questions:

1. **Pure algorithm** (no concurrency, no parsing) — write the core function.  
2. **Variant or extension** of that algorithm.  
3. **Parallel version** with `Async.Parallel` — split input into chunks, run each chunk as `async { return f chunk }`, collect with `Async.Parallel |> Async.RunSynchronously`.  
4. **JParsec parser** for input of that type — use `pchar`, `pstring`, `pint32`, `.>>.`, `.>>`, `>>.`, `|>>`, `<|>`, `many`.

If you are stuck on the parallel sub-question, check: 2025 Q3.5, 2025re Q3.3, 2024 Q3.3, 2023 Q3.5, 2022 Q3.4, 2020 Q3.3.  
If you are stuck on the parser sub-question, check: 2025 Q4.5, 2025re Q3.4, 2024 Q3.4, 2021 Q3.4.

---

### Step 5 — Is it a Q4 abstraction question?

Identify the abstraction by these keywords:

| Keyword in problem | Abstraction | Where to look |
|---|---|---|
| `ret`, `bind`, `>>=`, `fail`, "state monad", "HM" | State monad + computation expression | 2025 Q4, 2024 Q4, 2021re Q4 |
| `MailboxProcessor`, `PostAndReply`, `Receive`, "mailbox" | Actor/message-passing concurrency | 2025 Q3.3–3.4 |
| `llist`, `Cons of unit ->`, "lazy list", "infinite list" | Lazy/infinite list | 2020 Q4, 2020re Q4 |
| `Seq.unfold`, `seq<'a>`, "infinite sequence" | Infinite sequence | 2021 Q2.4, 2019 Q3, 2021 Q3 |
| `'a ring`, two-list representation, "clockwise", "counterclockwise" | Ring / circular buffer | 2022re Q4, 2021 Q4, 2021re Q4 |
| "binary search tree", `bintree`, `insert` maintain order | BST | 2020re Q1 |
| `Quad of g*g*g*g`, "quadtree", "grayscale" | Recursive image quadtree | 2022 Q1, 2022re Q1 |
| `type arith`, `type prop`, `eval`, "logical formula" | Recursive expression/formula AST | 2023re Q1, 2023 Q1 |
| "dining philosophers", "forks", philosopher eats/thinks | Concurrency + shared state | 2025 Q3 |
| "Towers of Hanoi", pegs, discs | State monad + recursive solution | 2025 Q4 |

---

### Step 6 — You still can't place it. Use these keyword triggers.

Scan the problem text for these words and jump straight to the indicated section:

| Words in the problem | Go to |
|---|---|
| "tail-recursive", "accumulator", "stack overflow" | Every Q1/Q2, 2nd-to-last sub-question of that year |
| "continuation", "CPS", "continuation-passing" | Every Q2/Q1 last sub-question |
| "higher-order", "HOF", `List.map`, `List.fold` | Q1 or Q2, late sub-questions |
| "not tail-recursive — explain why" / "prove" | Write an evaluation trace. See 2025 Q2.4 for the template. |
| "what would happen if you swap" | Match-order analysis. See 2025 Q2.2. |
| "incomplete pattern" or "warning" | 2022 Q2, 2025re Q2.2, 2023re Q2.2 |
| "`and` keyword" | 2024 Q2, 2023re Q2.2, 2021 Q2 |
| "mutual recursion" | 2023re Q2 (isEven/isOdd), 2023 Q2, 2021 Q2 |
| `Async.Parallel`, "parallel", "threads" | Q3 of most years — see Step 4 |
| `pstring`, `pchar`, `pint32`, `many`, `satisfy` | JParsec — see Step 4 parser section |
| `Set`, "coordinates", "grid", "valid" | 2025 Q1 |
| "sort", "sorted", "merge" | 2025 Q2 / 2024 Q2 (merge sort comprehension) |
| "binary", "convert to binary string" | 2022 Q2 |
| "Atbash", "encode", "shift characters" | 2025re Q2/Q3 |
| "Collatz" | 2023 Q3 |
| "Newton", "square root", "quadratic" | 2024 Q3, 2021re Q3 |
| "big integer", "digit list" | 2020 Q3 |
| "Peano", `O`, `S of Peano` | 2019 Q1 |
| "memoize", "mutable Map", "cache" | 2021 Q2 |
| "look-and-say", "run-length" | 2021 Q3 |
| "balanced brackets", "matching delimiters" | 2023 Q4, 2023re Q3 |
| "assembly", "register", "MOVI", "JGTZ" | 2023 Q4 |
| "matrix", "dot product", `int[,]` | 2022 Q3 |
| "stack machine", `Push`, `Add`, `Mult` | 2022 Q4 |
| "direction", "North/South/East/West", "walk", "path" | 2021 Q1 |
| "golden ratio", "pi", "Nilakantha" | 2019 Q3 |
| "palindrome", "symmetric" | 2019re Q2, 2023re Q3 |
| "ring", "clicker", "tally" | 2025re Q4 |
| "philosopher", "fork" | 2025 Q3 |

---

### Quick decision summary

```
See a problem →
  Q2 (foo/bar/baz given)?  →  Step 3 table
  Q3 (algorithm/parallel)? →  Step 4
  Q4 (fancy abstraction)?  →  Step 5
  Q1 (custom type + functions)? →  Step 2 progression, then keyword in Step 6
  Still lost? →  Ctrl+F a keyword from Step 6 in this document
```
