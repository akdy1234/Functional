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

**Q1.1** `type transactions = Empty | Pay of string*int*transactions | Receive of string*int*transactions`; `balance` non-tail-recursive (Pay subtracts, Receive adds).  
**Q1.2** `balanceAcc`: tail-recursive accumulator version of `balance`.  
**Q1.3** `participants`: returns `(Set<string>, Set<string>)` — (payers, receivers) — built by recursion.  
**Q1.4** `balanceFold fPay fReceive acc0`: generic fold over transactions (same shape as the type).  
**Q1.5** `collect`: builds `Map<string, int>` of net balance per person using `balanceFold`; uses `defaultArg (Map.tryFind name acc) 0` to handle first-time names.

**Q2.1** `foo`/`bar`/`baz` code comprehension: `foo` converts a digit char to int (`int c - int '0'`); `bar` converts string to char list (`[for c in str -> c]`); `baz` interprets an int list as a **little-endian** decimal number (`x + 10 * baz xs` — first element is LEAST significant).  
**Q2.2** `stringToInt`: pipeline `s |> bar |> List.map foo |> List.rev |> baz` — `List.rev` is critical because baz is little-endian but strings are big-endian.  
**Q2.3** `baz2`: rewrite baz using `List.foldBack (fun x acc -> x + 10 * acc) lst 0` (processes right-to-left, same structure as baz).  
**Q2.4** `baz` non-tail-recursion proof via evaluation trace (pending `x + 10 * (...)` on stack).  
**Q2.5** `bazTail`: CPS version of baz.

**Q3.1** `encrypt (s : string) (offset : int)`: Caesar cipher — `char (int 'a' + (int c - int 'a' + offset) % 26)` for lowercase letters; pass others unchanged. Uses `String.map`.  
**Q3.2** `decrypt`: reverse Caesar by encrypting with complement: `encrypt s (26 - offset % 26)`.  
**Q3.3** `decode plainText encryptedText`: try all 26 offsets with `List.tryFind`; returns `int option`.  
**Q3.4** `parEncrypt`: split string by spaces, encrypt each word in parallel with `Async.Parallel`, rejoin with `String.concat " "`.  
**Q3.5** `parseEncrypt (offset : int)`: JParsec — `many (satisfy (fun c -> c >= 'a' && c <= 'z')) |>> (fun chars -> encrypt (System.String(Array.ofList chars)) offset)`.

**Q4.1** `type letterbox = Map<string, string list>` — sender→message-queue map; `empty ()` = `Map.empty`; `post sender message lb` appends to queue with `msgs @ [message]`; `read sender lb` removes head with match on `Some (msg :: rest)`.  
**Q4.2–4.3** `StateMonad<'a> = SM of (letterbox -> ('a * letterbox) option)`; `ret`/`fail`/`bind`/`>>=`/`>>>=`; `evalSM (SM f) = f (empty ())`; `post2`/`read2` (read2 returns None if no messages).  
**Q4.4** `StateBuilder` + `trace`: processes a `log = MType list` (MType = Post|Read); returns `string list option` of all Read results.

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
**Q3.2** `balanced2`: generalized with `Map<char,char>` for arbitrary delimiter pairs; backtracking with `(||)` for chars that can be both opener and closer.  
**Q3.3** `balanced3`: uses `balanced2` with standard bracket map.  
**Q3.4** `symmetric`: even-length palindrome check ignoring non-letters (filter letters, lowercase, check `letters = List.rev letters`).  
**Q3.5 (parser)** `createParserForwardedToRef<unit>()` for recursive bracket grammar; `parseBalancedAux = many (pchar '(' >>. ParseBalanced .>> pchar ')')` etc.; `do bref := parseBalancedAux`.  
**Q3.6** `countBalanced lst x`: count balanced strings in parallel across x threads; ceiling chunk size = `(n + x - 1) / x`.

**Q4.1** `type basicProgram = Map<uint32, stmnt>`; `mkBasicProgram` = `Map.ofList`; `getStmnt l p` = `Map.find l p`; `nextLine l p` = `Map.filter (fun k _ -> k > l) p |> Map.minKeyValue |> fst`; `firstLine p` = `Map.minKeyValue p |> fst`.  
**Q4.2** `type state = { lineNumber: uint32; variables: Map<string,int> }`; `emptyState`; `goto l st` = `{ st with lineNumber = l }`; `update v a st` / `lookup v st`.  
**Q4.3** `evalExpr`/`step`/`evalProg`: plain recursive BASIC interpreter; `If(e,l)` = goto l if e≠0 else step; `Let(v,e)` = update then step; `Goto l` = goto l; `End` = return state.  
**Q4.4** `StateMonad<'a> = SM of (basicProgram -> state -> 'a * state)` (extra read-only program param, no option); `goto2`/`getCurrentStmnt2`/`lookup2`/`update2`/`step2`.  
**Q4.5** `StateBuilder` + `evalExpr2`/`evalProg2` via computation expression.

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

**Q4.1** `type expr = Num of int | Lookup of var | Plus of expr*expr | Minus of expr*expr`; `type stmnt = Assign of var*expr | While of expr*prog`; `type prog = stmnt list`; `type mem = int array`.  
**Q4.2** `evalExpr`/`evalStmnt`/`evalProg` — mutually recursive via `and` (stmnt calls evalProg for While body; evalProg calls evalStmnt); `evalExpr` reads from `mem` array.  
**Q4.3** `StateMonad<'a> = SM of (mem -> ('a * mem) option)` (can fail — e.g. variable not found); `lookup2`/`assign2`; `evalExpr2`/`evalStmnt2`/`evalProg2` via StateBuilder.  
**Q4.4** JParsec with `createParserForwardedToRef` for the recursive grammar (stmnt can contain prog which contains stmnt); `pWhile = pstring "while" >>. parseExpr .>>. parseProg`.

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

**Q4.1** `type register = R1|R2|R3`; `type assembly = MOVI of register*int | MULT of register*register | SUB of register*register*register | JGTZ of register*uint`; `assemblyToProgram : assembly list -> Map<uint, assembly>`.  
**Q4.2** `type state = { pc: uint; r1: int; r2: int; r3: int }`; `emptyState`; `setRegister`/`getRegister`/`setProgramCounter`/`incPC`/`lookupCmd`.  
**Q4.3** `StateMonad<'a> = SM of (state -> 'a * state)` (NO option — always succeeds); `StateBuilder`; `runProgram` (loop: execute current instruction, check pc in bounds to terminate).

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

**Q4.1** `type cmd = Push of int | Add | Mult`; `type stack = int list`; `runStackProgram`: pattern-match each cmd, push/pop from stack.  
**Q4.2–4.3** `StateMonad<'a> = SM of (stack -> ('a * stack) option)`; `push x`/`pop`; `StateBuilder`; `runStackProg2` via CE.  
**Q4.4** JParsec: `pstring "Push" >>. pchar ' ' >>. pint32 |>> Push`; `sepBy parseCmd ws` for full program.

---

## 2021re (Aug 2021)

**Q1.1** `type binList<'a,'b> = Nil | Cons1 of 'a*binList<'a,'b> | Cons2 of 'b*binList<'a,'b>`; `length` non-tail-recursive.  
**Q1.2** `split`: separate into two plain lists (one per constructor type); `length2`: tail-recursive accumulator.  
**Q1.3** `map f g`: map different functions over Cons1/'a elements vs Cons2/'b elements.  
**Q1.4** `filter p1 p2`: two predicates, one per constructor type.  
**Q1.5** `fold f g acc`: two combining functions, one per constructor.

**Q2.1** `foo`/`bar` code comprehension with `and` keyword: `foo` = merge two sorted lists (pick smaller head); `bar` = mergeSort (uses `List.splitAt (List.length a / 2)`). Constraint: both foo arguments must already be sorted.  
**Q2.2** `foo2`: same merge behavior using `List.unfold`.  
**Q2.3** Performance: `List.length` is O(n) and called at every recursion level; `bar2` replaces with O(1) structural match on [] and [_].  
**Q2.4** `fooTail`: tail-recursive accumulator version of merge (reverse at end).  
**Q2.5** `barTail`: CPS version of mergeSort.

**Q3.1** `approxSquare (n : float) (g : float) (eps : float)`: Newton iteration `(g + n/g)/2` until `|g*g - n| < eps`.  
**Q3.2** `quadratic a b c`: returns `(float * float) option` — both roots via discriminant `b²-4ac`; `None` if discriminant < 0.  
**Q3.3** `parQuadratic`: solve list of equations in parallel with `Async.Parallel`.  
**Q3.4** `solveQuadratic`: JParsec parser for `ax^2+bx+c=0` format; returns roots.

**Q4.1** `type rat = Rat of int * int`; `gcd` (Euclidean); `mkRat (n,d)` normalises: positive denominator, divide both by gcd; `ratToString`.  
**Q4.2** `plus`/`minus`/`mult`/`div` — `div` returns `option` (None if divisor is 0).  
**Q4.3** `StateMonad<'a> = SM of (rat list -> ('a * rat list) option)`; `ret`/`fail`/`bind`; `smPlus`/`smMinus`/`smMult`/`smDiv` pop two values, combine, push result.  
**Q4.4** `ringStep`: check first two CCW elements of a ring, remove pair if sum is even, else move CCW.  
**Q4.5** `iterRemoveSumEven`: run `ringStep` n times via state monad.

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

**Q4.1** `'a ring = 'a list * 'a list` (two-list zipper); `length`/`ringFromList`/`ringToList`/`empty`.  
**Q4.2** `push x r` prepends x CCW; `peek r` = head of CCW list as option; `pop r` = head + remaining ring.  
**Q4.3** `moveClockwise`/`moveCounterClockwise`: move current position around the ring by shifting between the two lists.

---

## 2020

**Q1.1** `insert`: insert element into sorted list, non-tail-recursive.  
**Q1.2** `insertionSort`: sort using `insert`; `insertTail` accumulator version; `insertionSortTail`.  
**Q1.3** `insertionSort2` HOF; `insertBy` with comparison function; `insertionSortBy`.

**Q2.1** `foo`/`bar`/`baz` code comprehension: `foo x lst` = removeFirst (remove first x from list, returns `Some rest` or `None`); `bar xs ys` = prependAll (prepend xs head to every list in ys); `baz lst` = permutations (all permutations of lst using foo+bar).  
**Q2.2** Incomplete pattern warning in foo (missing `[]` case); `foo2` fixed version; `bar2` HOF using `List.map`.  
**Q2.3** `fooTail`: accumulator version of removeFirst; non-tail-recursion proof for baz.

**Q3.1** `type shape = Rock|Paper|Scissors`; `type result = Win|Lose|Draw`; `rps : shape -> shape -> result`; `type strategy = (shape*shape) list -> shape`.  
**Q3.2** `parrot`: repeat last move; `beatingStrat`: always play winning counter; `roundRobin`: cycle through shapes.  
**Q3.3** `bestOutOf`: run strategy tournament via `Seq.unfold` (keeps score history, stops when one wins); `playTournament`.

**Q4.1** `type stack = int list`; `SM<'a> = S of (stack -> ('a * stack) option)`; `ret`/`fail`/`bind`/`push`/`pop`.  
**Q4.2** `StateBuilder`; `calculateRPN` via CE — processes RPN expression token by token; push numbers, pop two and push result for operators.

---

## 2020re (Aug 2020)

**Q1.1** `type 'a bintree = Leaf | Node of 'a bintree * 'a * 'a bintree`; `insert` (maintains BST invariant: smaller → left, larger → right); `fromList = List.fold (fun t x -> insert x t) Leaf`.  
**Q1.2** `fold f acc` (in-order left fold); `foldBack f acc` (in-order right fold); `inOrder t = fold (fun acc x -> acc @ [x]) [] t`.  
**Q1.3** `badMap` (shape-preserving: breaks BST property if f changes ordering); `map f t = fold (fun t' x -> insert (f x) t') Leaf t` (correct: re-inserts via fold to maintain BST).

**Q2.1** `foo`/`bar`/`baz` code comprehension: `foo` = bubbleStep (one left-to-right pass, swaps adjacent out-of-order elements); `bar` = isSorted predicate; `baz` = bubbleSort (repeats foo until bar returns true).  
**Q2.2** `bar3`: `List.pairwise lst |> List.forall (fun (a,b) -> a <= b)`.  
**Q2.3** `fooTail`: accumulator version of bubbleStep.

**Q3.1** `type bigInt = int list` (digits, MSD first); `fromString`/`toString`; `add` (carry propagation using reversed lists).  
**Q3.2** `multSingle`: multiply bigInt by single digit with carry; `mult`: long multiplication — for each digit of second number, multSingle + shift.  
**Q3.3** `fact n`: parallel factorial — chunk `[1..n]`, multiply each chunk's big integers, then multiply results; uses `Async.Parallel`.

**Q4.1** `type 'a llist = Cons of (unit -> 'a * 'a llist)`; `step (Cons f) = f ()`; `cons hd tl = Cons (fun () -> hd, tl)`; `init`/`llmap`/`filter`/`takeFirst`.  
**Q4.2** `unfold : ('b -> 'a * 'b) -> 'b -> 'a llist`; `fibll1` (pair-based, O(1) per step) vs `fibll2` (calls itself twice, exponential — same issue as naive Fibonacci).

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

Quick lookup table — find a topic, jump to that year's solution file.

| Pattern | Where to look |
|---|---|
| Non-tail-recursive → tail-recursive accumulator | Every exam Q1 |
| CPS (continuation-passing style) | Every exam Q2.4–2.5 |
| Code comprehension + name functions | Every exam Q2.1 |
| Incomplete pattern match warning + fix | 2022 Q2, 2022re Q2, 2025re Q2.2, 2024re Q2.2 |
| `and` keyword (mutual recursion) | 2023re Q2 (isEven/isOdd), 2021re Q2 (merge/sort), 2021 Q2 (memo/fib) |
| `Seq.unfold` / infinite sequence | 2021 Q2.4, 2019 Q3, 2019re Q3 |
| Lazy list `Cons of unit -> ...` | 2020re Q4 |
| `Async.Parallel` parallel computation | 2025 Q3.5, 2025re Q3.4, 2024 Q3.4, 2023 Q3.5, 2023re Q3.6, 2022 Q3.4, 2020re Q3.3 |
| `MailboxProcessor` / message passing | 2025 Q3.3–3.4 |
| State monad with option (`ret`/`fail`/`bind`) | 2024 Q4, 2022 Q4, 2021re Q4, 2020 Q4 |
| State monad without option (always succeeds) | 2024re/2025re Q4, 2022re Q4 |
| State monad with Result + typed error | 2025 Q4 |
| State monad with extra read-only param | 2023re Q4 (BASIC) |
| Computation expression (`Builder`) | 2025 Q4.4, 2024 Q4.4, 2024re/2025re Q4.5, 2023re Q4.5 |
| JParsec parsers | 2025 Q4.5, 2025re/2024re Q3.5, 2024 Q3.5, 2023re Q3.5, 2023 Q4.4, 2021 Q3.4, 2022 Q4 |
| JParsec with `createParserForwardedToRef` | 2023re Q3.5, 2023 Q4.4 (recursive grammar) |
| Recursive algebraic type + `eval` | 2023re Q1 (arith), 2023 Q1 (prop) |
| Quadtree / grayscale image | 2022 Q1, 2022re Q1 |
| Ring data structure | 2021 Q4, 2021re Q4.4 |
| Assembly language interpreter | 2022re Q4 |
| BASIC language interpreter | 2023re Q4 |
| Memory machine (expr/stmnt) | 2023 Q4 |
| Peano numbers | 2019 Q1 |
| Custom linked list (non-standard) | 2021re Q1 (binList), 2019re Q1 (SumColl), 2024 Q1 (transactions) |
| Big integers (digit lists) | 2020re Q3 |
| Rock-Paper-Scissors strategies + `Seq.unfold` | 2020 Q3 |
| RPN calculator + state monad | 2020 Q4 |
| Binary search tree | 2020re Q1 |
| Matrix operations | 2022 Q3 |
| Stack machine | 2022 Q4 |
| Grid/coordinate movement | 2021 Q1, 2025 Q1 |
| Balanced brackets + JParsec recursive grammar | 2023re Q3 |
| Memoization + mutable Map | 2021 Q2 |
| Caesar / ROT-3 cipher | 2024 Q3, 2024re/2025re Q2 |
| Atbash cipher (self-inverse) | 2024re/2025re Q3 |
| Letterbox / message queue | 2024 Q4 |
| Tally clicker (carry propagation) | 2024re/2025re Q4 |
| Dining philosophers (MailboxProcessor) | 2025 Q3 |
| Towers of Hanoi (Result monad) | 2025 Q4 |

---

### What each pattern means and how to do it

---

#### Non-tail-recursive → tail-recursive accumulator

**What it is:** A function is *tail-recursive* if the very last thing it does is call itself — nothing is waiting for the result afterward. If, instead, the recursive call is nested inside an expression like `x + foo xs` or `x :: foo xs`, then work is left pending on the call stack while the recursion dives deeper. For a list of 10,000 elements this means 10,000 stack frames, which can crash the program.

**The fix:** Add an inner function `aux` with an extra `acc` (accumulator) argument that carries the result so far. Update `acc` *before* the recursive call so there is nothing left to do afterward.

```fsharp
// Non-tail-recursive — pending "+" on stack after each call:
let rec sum lst =
    match lst with
    | []      -> 0
    | x :: xs -> x + sum xs   // "x +" must wait for sum xs to return

// Tail-recursive — acc absorbs the work, nothing pending:
let sum lst =
    let rec aux acc lst =
        match lst with
        | []      -> acc          // done, return accumulated total
        | x :: xs -> aux (acc + x) xs   // update FIRST, then recurse
    aux 0 lst
```

When building a list instead of a number, prepend to `acc` and reverse at the end:
```fsharp
let rec filterTail pred lst =
    let rec aux acc lst =
        match lst with
        | []      -> List.rev acc
        | x :: xs -> if pred x then aux (x :: acc) xs else aux acc xs
    aux [] lst
```

---

#### CPS (continuation-passing style)

**What it is:** Sometimes the accumulator trick isn't enough — for example when a function returns a *pair* of lists or recurses in two directions. CPS is the general solution: instead of returning a value, you pass a function `c` (the "continuation") that says "here's what to do with the result once you have it." The pending work gets baked into that function rather than sitting on the stack.

**The fix:** Add a parameter `c` of type `'result -> 'result`. The initial call passes `id` (the identity function — "just return whatever you get"). At each recursive step, instead of doing work *after* the recursive call, wrap that work in a new continuation and pass it deeper.

```fsharp
// Non-tail-recursive — cons waits on stack:
let rec copyList lst =
    match lst with
    | []      -> []
    | x :: xs -> x :: copyList xs   // "x ::" waits

// CPS version — nothing waits, all pending work lives in the closure:
let copyListCPS lst =
    let rec aux lst c =
        match lst with
        | []      -> c []                                // base: give empty list to continuation
        | x :: xs -> aux xs (fun result -> c (x :: result))  // wrap pending cons in new cont
    aux lst id
```

**How to explain it in writing:** "The call `aux xs (...)` is the last thing done — nothing waits on it. The pending `x ::` operation is captured in the closure `fun result -> c (x :: result)`, which lives on the heap rather than the call stack."

---

#### Code comprehension + name functions

**What it is:** Q2 always gives you three mystery functions named `foo`, `bar`, `baz`. You must figure out what they compute and give them sensible names. You are not expected to prove anything — just trace a few examples by hand and recognize the pattern.

**How to do it:**
1. Look at the *base case* — what does the function return for `[]` or `0`? That tells you the "identity" value.
2. Trace 2–3 small inputs manually (e.g. `foo [1;2;3]` or `foo 3`).
3. Describe what the output is in plain English ("removes the first occurrence of x", "checks if the list is sorted").
4. Name it from that description: `removeFirst`, `isSorted`, `merge`, etc.

The exam also asks about *constraints*: what must be true about the input for the function to behave sensibly? (e.g. "both lists must already be sorted", "each element must be a digit 0–9").

---

#### Incomplete pattern match warning + fix

**What it is:** In F#, `when` guards on `match` arms can fail even if the pattern looks like it matches everything. The compiler warns "this pattern is incomplete" because it can't prove mathematically that your guards cover every case.

```fsharp
// WARNING — all three arms have guards, compiler can't prove coverage:
let foo = function
    | c when Char.IsWhiteSpace c -> c
    | c when c > 'w'             -> char (int c - 23)
    | c when c < 'x'             -> char (int c + 3)   // ← warning here
```

**The fix:** Remove the `when` guard from the *last* arm and replace it with a plain wildcard. A pattern without a guard always matches, so the compiler knows every input is handled. This is safe because if execution reaches the last arm, all previous guards have already failed — so the last guard would be true anyway.

```fsharp
// FIXED — last arm has no guard:
let foo2 = function
    | c when Char.IsWhiteSpace c -> c
    | c when c > 'w'             -> char (int c - 23)
    | c                          -> char (int c + 3)   // ← no guard, always matches
```

---

#### `and` keyword (mutual recursion)

**What it is:** Normally in F# a function can only call functions that were defined *before* it. If `foo` calls `bar` and `bar` also calls `foo`, you have a chicken-and-egg problem — neither can be defined first. The `and` keyword solves this by letting two (or more) definitions be in scope for each other simultaneously.

```fsharp
let rec foo x =
    if x = 0 then true else bar (x - 1)   // foo calls bar
and     bar x =
    if x = 0 then false else foo (x - 1)  // bar calls foo — OK because of `and`
```

Without `and`, the first `let rec foo` would fail to compile because `bar` doesn't exist yet. With `and`, both are defined together.

**When the exam asks:** "Can `and` be replaced by `let rec`?" — Answer: **No.** If you write two separate `let rec` bindings, the first one references a function that hasn't been defined yet, which is a compile error.

---

#### `Seq.unfold` / infinite sequence

**What it is:** `Seq.unfold` generates an infinite (or finite) sequence from a *seed* value. You provide a function that takes the current seed and returns `Some(nextValue, newSeed)` to keep going, or `None` to stop. F# sequences are *lazy* — they only compute the next element when you ask for it, so an infinite sequence doesn't run forever immediately.

```fsharp
// Generate: 0, 1, 1, 2, 3, 5, 8, ... (Fibonacci)
let fibs = Seq.unfold (fun (a, b) -> Some(a, (b, a + b))) (0, 1)

// Take the first 10:
Seq.take 10 fibs |> Seq.toList   // [0;1;1;2;3;5;8;13;21;34]
```

The seed `(0, 1)` is the starting pair. Each step returns the first number `a` as the next sequence element, and `(b, a+b)` as the new seed.

---

#### Lazy list `Cons of unit -> ...`

**What it is:** An alternative to `Seq` for truly lazy (on-demand) lists. The type is:
```fsharp
type 'a llist = Cons of (unit -> 'a * 'a llist)
```
Each node is a *thunk* — a zero-argument function `unit -> ...` that, when called, produces the head value and the next node. Nothing is computed until you call the thunk.

```fsharp
let step (Cons f) = f ()            // un-thunk: get (head, tail)
let cons hd tl = Cons (fun () -> hd, tl)   // build a node

// Infinite list of 1s:
let rec ones = Cons (fun () -> 1, ones)

// Take first n elements:
let rec takeFirst n (Cons f) =
    if n = 0 then []
    else let (x, rest) = f () in x :: takeFirst (n-1) rest
```

The key difference from `Seq.unfold`: the laziness is explicit in the type — you can see the `unit ->` wrapping. `Seq` hides this behind an interface.

---

#### `Async.Parallel` parallel computation

**What it is:** F# lets you run independent computations at the same time using `async { ... }` blocks and `Async.Parallel`. The pattern is always the same 4-step pipeline: wrap each piece of work in `async { return ... }`, combine with `Async.Parallel`, execute with `Async.RunSynchronously`, then combine the results.

```fsharp
// Encrypt each word in a string in parallel:
let parEncrypt (s : string) (offset : int) =
    s.Split([|' '|])
    |> Array.map (fun word -> async { return encrypt word offset })  // step 1: wrap
    |> Async.Parallel          // step 2: combine into one async
    |> Async.RunSynchronously  // step 3: run and wait for all
    |> String.concat " "       // step 4: combine results
```

For a numeric range, split into chunks first:
```fsharp
let chunkSize = max 1 ((total + numThreads - 1) / numThreads)  // ceiling division
[lo..hi]
|> List.chunkBySize chunkSize
|> List.map (fun chunk -> async { return List.sumBy process chunk })
|> Async.Parallel
|> Async.RunSynchronously
|> Array.sum
```

Each `async { return x }` is just a description of work — it doesn't start until `Async.RunSynchronously`. `Async.Parallel` runs them all at the same time, then waits for all to finish.

---

#### `MailboxProcessor` / message passing

**What it is:** A `MailboxProcessor` is a background agent that owns some state and processes messages from a queue *one at a time*. Because messages are processed serially, multiple threads can send messages concurrently without race conditions — the agent serialises access automatically.

You define a message type, write a loop that receives and handles messages, and expose send functions.

```fsharp
type message =
    | DoSomething of int * AsyncReplyChannel<unit>  // sends reply when done
    | Notify of int                                  // fire and forget

let agent = MailboxProcessor.Start(fun mbox ->
    let rec loop state =
        async {
            let! msg = mbox.Receive()   // await next message
            match msg with
            | DoSomething(x, rc) ->
                // ... do work with x, update state ...
                rc.Reply(())            // unblock the caller
                return! loop newState
            | Notify x ->
                return! loop (update state x)
        }
    loop initialState)

// Caller blocks until agent replies:
agent.PostAndReply(fun rc -> DoSomething(42, rc))

// Caller sends and moves on immediately:
agent.Post(Notify 7)
```

`PostAndReply` sends a message and *blocks the calling thread* until `rc.Reply()` is called. `Post` is fire-and-forget.

---

#### State monad with option (`ret`/`fail`/`bind`)

**What it is:** A state monad threads a piece of state (a stack, a map, a clicker) through a sequence of operations without you having to pass it explicitly everywhere. The `option` wrapper means any step can fail — and if it does, all remaining steps are skipped automatically ("short-circuit").

Think of it as a chain of operations where each step:
1. Receives the current state
2. Either succeeds (returning a value + new state) or fails (returning `None`)
3. Passes its result to the next step

```fsharp
type StateMonad<'a> = SM of (state -> ('a * state) option)

let ret x   = SM (fun s -> Some (x, s))       // succeed with x, don't change state
let fail    = SM (fun _ -> None)               // always fail
let bind f (SM a) =
    SM (fun s ->
        match a s with
        | None         -> None                 // already failed: propagate
        | Some (x, s') ->
            let (SM g) = f x
            g s')                              // succeeded: pass x and new state to next step

let (>>=) m f = bind f m                      // m >>= f: run m, feed result to f
let (>>>=) m n = m >>= (fun _ -> n)           // m >>>= n: run m, ignore result, run n
```

The magic is in `bind`: if any step returns `None`, the whole chain returns `None` without running the rest.

---

#### State monad without option (always succeeds)

**What it is:** Same idea as above, but without the `option` — every operation is guaranteed to succeed. The `bind` is simpler because there's no failure case to handle.

```fsharp
type StateMonad<'a> = SM of (state -> 'a * state)

let ret x = SM (fun s -> (x, s))
let bind f (SM a) =
    SM (fun s ->
        let x, s' = a s        // always succeeds, no match needed
        let (SM g) = f x
        g s')
```

Used when operations can't fail (e.g. reading/writing a clicker's wheels, executing assembly instructions that always do *something*).

---

#### State monad with Result + typed error

**What it is:** Like the option monad, but failures carry a specific reason. Instead of `None` you get `Error e` where `e` is a value describing what went wrong. Used in 2025 (Towers of Hanoi) where you want to know *why* a move failed — was the peg empty, or was the disc too large?

```fsharp
type MyMonad<'a> = HM of (state -> Result<'a * state, error>)

let ret x       = HM (fun h -> Ok (x, h))
let fail err    = HM (fun _ -> Error err)
let bind f (HM a) =
    HM (fun h ->
        match a h with
        | Ok (x, h') ->
            let (HM g) = f x
            g h'
        | Error err -> Error err)   // propagate the typed error
```

---

#### State monad with extra read-only parameter

**What it is:** Sometimes a computation needs to read from a fixed configuration (e.g. a program's instruction list) while also maintaining mutable state (e.g. the current line number and variables). The fix: make the monad's function take *two* arguments — the fixed read-only thing and the mutable state.

```fsharp
type StateMonad<'a> = SM of (program -> state -> 'a * state)
//                                ↑ fixed, never changes   ↑ mutable

let ret x = SM (fun _ s -> (x, s))

let bind f (SM a) =
    SM (fun p s ->
        let x, s' = a p s          // run: pass program AND state
        let (SM g) = f x
        g p s')                    // pass same program to next step

let evalSM prog (SM f) = f prog (emptyState prog)
```

Used in 2023re (BASIC interpreter): the program `Map<uint32, stmnt>` never changes while the `state` (current line, variable values) changes with every instruction.

---

#### Computation expression (`Builder`)

**What it is:** F# computation expressions let you write monadic code using `let!`, `do!`, `return` instead of chains of `>>=`. The compiler desugars the nice syntax into monad operations. You define a `Builder` class with `Bind`, `Return`, etc.

```fsharp
type StateBuilder() =
    member this.Bind(f, x)    = bind x f    // let! x = m  →  bind continuation m
    member this.Return(x)     = ret x        // return x    →  ret x
    member this.ReturnFrom(x) = x            // return! m   →  m (already in monad)
    member this.Combine(a, b) = a >>= (fun _ -> b)  // a; b → sequence

let state = StateBuilder()

// Pretty syntax (left) is exactly equivalent to chained >>= (right):
state {
    let! x = readSomething     // readSomething >>= fun x ->
    do! writeSomething x       // writeSomething x >>>= 
    return x + 1               // ret (x + 1)
}
```

**Key gotcha:** `this.Bind(f, x)` — the arguments are *swapped* compared to what you'd expect. `f` is the monad (the left side of `let!`) and `x` is the continuation. The body calls `bind x f` which means `bind continuation monad`.

---

#### JParsec parsers

**What it is:** JParsec is a library for building *parser combinators* — small parsers that you combine into bigger ones. A parser of type `Parser<'a>` reads characters from the input and either succeeds (returning a value of type `'a` and consuming some input) or fails.

**The essential combinators:**
```fsharp
// Primitives — match one thing:
pchar 'a'             // matches the character 'a'
pstring "hello"       // matches the exact string "hello"
pint32                // matches an integer
digit                 // matches one digit character '0'–'9'
satisfy (fun c -> c >= 'a' && c <= 'z')  // matches one char where predicate is true

// Sequencing — run two parsers in order:
p1 >>. p2    // run p1 then p2, keep p2's result  (mnemonic: dot on right = keep right)
p1 .>> p2    // run p1 then p2, keep p1's result  (mnemonic: dot on left = keep left)
p1 .>>. p2   // run both, return pair of results

// Transformation:
p |>> f      // run p, transform the result with function f

// Alternation:
p1 <|> p2    // try p1; if it fails, try p2

// Repetition:
many p       // run p zero or more times, returns a list
sepBy p sep  // p separated by sep, returns a list
```

**Example — parse "Push 42":**
```fsharp
pstring "Push" >>. pchar ' ' >>. pint32 |>> Push
// reads "Push", discards it, reads " ", discards it, reads 42, wraps in Push constructor
```

---

#### JParsec with `createParserForwardedToRef`

**What it is:** When a grammar is *recursive* (e.g. a balanced bracket string can contain another balanced bracket string, or a `while` loop can contain more statements), you need to reference the parser before you define it. `createParserForwardedToRef` creates a placeholder parser and a mutable reference. You build all your sub-parsers using the placeholder, then at the end assign the real parser to the reference.

```fsharp
// Step 1: create placeholder
let myParser, myRef = createParserForwardedToRef<unit>()

// Step 2: build sub-parsers that use the placeholder
let parseChunk =
    (pchar '(' >>. myParser .>> pchar ')')   // uses myParser before it's defined
    <|> (pchar '[' >>. myParser .>> pchar ']')

let fullParser = many parseChunk |>> ignore

// Step 3: close the loop — assign the real parser to the ref
do myRef := fullParser    // MUST come after fullParser is defined
```

Without this, you'd get a "value used before it's defined" error.

---

#### Recursive algebraic type + `eval`

**What it is:** Some exams define a type representing an expression tree — for example `type arith = Num of int | Add of arith * arith` or `type prop = TT | FF | And of prop * prop | Or of prop * prop`. Functions over these types are always recursive and mirror the structure of the type: one match arm per constructor.

```fsharp
type arith = Num of int | Add of arith * arith

// eval mirrors the type exactly:
let rec eval = function
    | Num x      -> x              // base case: just return the number
    | Add(a, b)  -> eval a + eval b  // recursive: eval both children, combine

// negate (push negation through without evaluating):
let rec negate = function
    | Num x      -> Num (-x)
    | Add(a, b)  -> Add(negate a, negate b)
```

The key insight: **every function on a recursive type is itself recursive**, with one case per constructor. The base constructors (`Num`, `TT`, `FF`) don't recurse; the compound constructors (`Add`, `And`, `Or`) recurse on their children.

---

#### Quadtree / grayscale image

**What it is:** A quadtree represents an image by recursively dividing it into four quadrants. A `Square` is a single uniform pixel (grey level 0–255); a `Quad` has four child quadtrees (top-left, top-right, bottom-left, bottom-right).

```fsharp
type grayscale = Square of uint8 | Quad of grayscale * grayscale * grayscale * grayscale
//                                         q1=TL        q2=TR        q3=BL        q4=BR
```

Functions follow the same pattern as any recursive type:
```fsharp
let rec countWhite = function
    | Square px               -> if px = 255uy then 1 else 0
    | Quad(q1, q2, q3, q4)   -> countWhite q1 + countWhite q2 + countWhite q3 + countWhite q4

// Rotate 90° clockwise: q4→q1, q1→q2, q2→q3, q3→q4
let rec rotateRight = function
    | Square _ as s           -> s
    | Quad(q1, q2, q3, q4)   -> Quad(rotateRight q4, rotateRight q1, rotateRight q3, rotateRight q2)
```

---

#### Ring data structure

**What it is:** A ring (circular buffer) is represented as *two lists*: elements to the left of the current position and elements to the right. The current element is the head of the right list. Moving clockwise shifts one element from right to left; moving counterclockwise shifts one element from left to right.

```fsharp
type 'a ring = Ring of 'a list * 'a list
//                    left (reversed)   right (current is head)

let peek (Ring(_, r)) = List.tryGetHead r   // current element

let cw (Ring(l, r)) =        // move clockwise: take from right, put on left
    match r with
    | []      -> Ring([], List.rev l)   // wrap around
    | [x]     -> Ring([], x :: List.rev l)
    | x :: xs -> Ring(x :: l, xs)

let ccw (Ring(l, r)) =       // move counterclockwise: take from left, put on right
    match l with
    | []      -> Ring(List.rev r, [])
    | x :: xs -> Ring(xs, x :: r)
```

---

#### Assembly language interpreter

**What it is (2022re Q4):** A tiny CPU simulator. Instructions like `MOVI R1 5` (load constant into register), `MULT R1 R2` (multiply registers), `SUB R1 R2 R3` (subtract), `JGTZ R1 10` (jump if register > 0). State is the program counter plus register values. The state monad (no option — can't fail) threads the state through execution.

The program is stored as `Map<uint, assembly>` — address to instruction. The main loop: read current instruction, execute it (update registers / jump), repeat until PC falls off the end.

---

#### BASIC language interpreter (2023re Q4)

**What it is:** A small interpreter for a BASIC-like language with statements `Let(var, expr)`, `If(expr, lineNum)`, `Goto(lineNum)`, `End`. The program is `Map<uint32, stmnt>` (line number → statement). State is the current line number plus a `Map<string, int>` of variable values.

The state monad here has an *extra read-only parameter* for the program (which never changes during execution). Execution follows: look up current statement, execute it (update variables or jump), repeat until `End`.

---

#### Memory machine (2023 Q4)

**What it is:** A mini programming language with expressions (`Num`, `Lookup`, `Plus`, `Minus`) and statements (`Assign`, `While`). The evaluator functions are *mutually recursive* using `and` because `evalStmnt` calls `evalProg` for the body of a While loop, and `evalProg` calls `evalStmnt` for each statement.

```fsharp
let rec evalExpr e mem = ...
and     evalStmnt s mem = ...   // calls evalProg for While body
and     evalProg  p mem = ...   // calls evalStmnt for each statement
```

The state monad version wraps the `int array` memory, using option to handle lookup failures.

---

#### Peano numbers (2019 Q1)

**What it is:** A way of representing natural numbers purely with a recursive type — no built-in integers used for the value itself.

```fsharp
type Peano = O | S of Peano   // O = zero, S x = x + 1
// Examples: 0 = O, 1 = S O, 2 = S(S O), 3 = S(S(S O))
```

Operations like `add`, `mult`, `pow` recurse on the Peano structure:
```fsharp
let rec add m = function
    | O    -> m           // m + 0 = m
    | S n  -> S (add m n) // m + (n+1) = (m+n) + 1

// Tail-recursive version:
let tailAdd m n =
    let rec aux acc = function
        | O    -> acc
        | S n  -> aux (S acc) n
    aux m n
```

`loop f p x` applies `f` to `x` exactly `toInt p` times — useful for writing multiplication and exponentiation without explicit recursion on the Peano type.

---

#### Custom linked list (non-standard)

**What it is:** The exam sometimes defines its own list type instead of using F#'s built-in `'a list`. The type usually has a `Nil`/`Empty` base and one or more `Cons`-like constructors. Functions over it follow the same pattern as any recursive type.

Examples seen:
- `binList<'a,'b> = Nil | Cons1 of 'a*binList | Cons2 of 'b*binList` — alternates between two element types
- `SumColl = Nil | CLeft of 'a*SumColl | CRight of 'b*SumColl` — elements tagged Left or Right
- `transactions = Empty | Pay of string*int*transactions | Receive of string*int*transactions`

For any of these, `fold` is always the same shape:
```fsharp
// Two-constructor list:
let rec fold f1 f2 acc = function
    | Nil             -> acc
    | Cons1(x, rest)  -> fold f1 f2 (f1 acc x) rest
    | Cons2(x, rest)  -> fold f1 f2 (f2 acc x) rest
```

---

#### Big integers (digit lists)

**What it is (2020re Q3):** Represent arbitrarily large integers as lists of digits (one `int` per digit, 0–9). Addition works just like long addition by hand: add digit by digit from right to left, carry the overflow.

```fsharp
type bigInt = int list   // MSD (most significant digit) first, e.g. 123 = [1;2;3]

// Addition: reverse both, add with carry, reverse result:
let add a b =
    let rec aux carry a b =
        match a, b with
        | [], []         -> if carry = 0 then [] else [carry]
        | x :: xs, []   -> let s = x + carry in (s % 10) :: aux (s / 10) xs []
        | [], y :: ys   -> let s = y + carry in (s % 10) :: aux (s / 10) [] ys
        | x::xs, y::ys  -> let s = x + y + carry in (s % 10) :: aux (s / 10) xs ys
    List.rev (aux 0 (List.rev a) (List.rev b))
```

---

#### Rock-Paper-Scissors strategies (2020 Q3)

**What it is:** Model the game as a `type shape = Rock | Paper | Scissors` and `type result = Win | Lose | Draw`. A `strategy` is a function `(shape * shape) list -> shape` — it looks at the history of (your move, opponent's move) pairs and picks the next move. `bestOutOf` uses `Seq.unfold` to generate a sequence of rounds until one player wins enough.

---

#### RPN calculator + state monad (2020 Q4)

**What it is:** RPN (Reverse Polish Notation) expressions are evaluated using a stack. `3 4 + 2 *` means: push 3, push 4, pop two and push 3+4=7, push 2, pop two and push 7×2=14. The state monad threads the stack through operations; `push` and `pop` are primitive monadic operations; `calculateRPN` chains them in a computation expression.

---

#### Binary search tree (2020re Q1)

**What it is:** A binary search tree stores values such that everything in the left subtree is smaller than the root and everything in the right subtree is larger. `insert` must maintain this property.

```fsharp
type 'a bintree = Leaf | Node of 'a bintree * 'a * 'a bintree

let rec insert x = function
    | Leaf                     -> Node(Leaf, x, Leaf)
    | Node(l, v, r) when x < v -> Node(insert x l, v, r)
    | Node(l, v, r) when x > v -> Node(l, v, insert x r)
    | t                        -> t   // already in tree, do nothing

// WRONG: badMap preserves tree shape but breaks BST property if f changes ordering
// CORRECT: map must re-insert each value so the BST property is maintained
let map f t = fold (fun acc x -> insert (f x) acc) Leaf t
```

---

#### Matrix operations (2022 Q3)

**What it is:** Matrices are `int[,]` (2D arrays in F#). Access with `m.[i,j]`. Matrix multiplication: `result.[i,k] = sum over j of (m1.[i,j] * m2.[j,k])`. This is called the dot product of row i from m1 with column k from m2. The parallel version runs each cell's dot product in a separate async.

---

#### Stack machine (2022 Q4)

**What it is:** A small virtual machine that evaluates expressions stored as a list of commands: `Push n` (push integer n), `Add` (pop two, push their sum), `Mult` (pop two, push their product). The state monad threads the stack (an `int list`) through the commands. JParsec parses text like `"Push 3 Push 4 Add"` into a list of `cmd` values.

---

#### Grid/coordinate movement (2021 Q1, 2025 Q1)

**What it is:** A coordinate system where you track `(x, y)` position and sometimes a facing direction (North/East/South/West). Functions like `move`, `turnLeft`, `turnRight` update the position or direction. A sequence of moves is processed by `List.fold` or tail recursion; visited coordinates are collected into a set.

```fsharp
type coord = C of int * int
type direction = North | East | South | West

let move dist dir (C(x,y)) =
    match dir with
    | North -> C(x, y + dist)
    | South -> C(x, y - dist)
    | East  -> C(x + dist, y)
    | West  -> C(x - dist, y)

let turnRight = function North -> East | East -> South | South -> West | West -> North
```

---

#### Balanced brackets + JParsec recursive grammar (2023re Q3)

**What it is:** Check if a string's brackets `(){}[]` are properly nested and matched. The stack-based approach: push expected closing bracket when you see an opener; when you see a closer, check it matches the top of the stack.

The JParsec parser uses `createParserForwardedToRef` because balanced bracket strings can nest arbitrarily:
```fsharp
let ParseBalanced, bref = createParserForwardedToRef<unit>()
let parseChunk =
    (pchar '(' >>. ParseBalanced .>> pchar ')')
    <|> (pchar '{' >>. ParseBalanced .>> pchar '}')
    <|> (pchar '[' >>. ParseBalanced .>> pchar ']')
let fullParser = many parseChunk |>> ignore
do bref := fullParser   // close the recursive loop
```

---

#### Memoization + mutable Map (2021 Q2)

**What it is:** Memoization caches the result of a function call so you don't recompute it. In F# this is done with a mutable `Map` (or `Dictionary`) stored in a closure. The key insight: `foo f` takes a function `f` and returns a *memoized version of f* — a new function that checks the cache before calling the original.

```fsharp
let foo f =
    let mutable cache = Map.empty
    fun x ->
        match Map.tryFind x cache with
        | Some v -> v          // cache hit: return stored result
        | None   ->
            let v = f x        // cache miss: actually call f
            cache <- Map.add x v cache
            v

// bar = raw (non-memoized) Fibonacci
let rec bar n = if n <= 1 then n else bar (n-1) + bar (n-2)  // exponential

// baz = memoized Fibonacci — much faster
let baz = foo bar
```

**Gotcha:** `barbaz = foo (fun n -> if n <= 1 then n else barbaz(n-1) + barbaz(n-2))` is *slower* than `baz`, because `barbaz` creates a new cache every time it's called (the cache isn't shared across the recursive calls the way `baz`'s is).

---

#### Caesar / ROT-3 cipher (2024 Q3, 2024re Q2)

**What it is:** A Caesar cipher shifts each letter by a fixed number of positions in the alphabet, wrapping around. The formula for encrypting lowercase letter `c` with offset `n`:
```fsharp
char (int 'a' + (int c - int 'a' + n) % 26)
// int c - int 'a'  = 0-based position (a=0, b=1, ..., z=25)
// + n              = shift
// % 26             = wrap around
// + int 'a'        = back to ASCII range
```
Decryption uses the complementary offset: `encrypt s (26 - offset % 26)`.

---

#### Atbash cipher (self-inverse) (2024re/2025re Q3)

**What it is:** Atbash maps each letter to its mirror: a↔z, b↔y, c↔x, etc. The formula is simply `219 - int c` (where 219 = int 'a' + int 'z' = 97 + 122). It's self-inverse: applying it twice returns the original letter, so `decrypt = encrypt`.

```fsharp
let atbashChar c = char (219 - int c)   // 'a' → 'z', 'z' → 'a'
// Proof: 219 - (219 - int c) = int c  ✓
```

---

#### Letterbox / message queue (2024 Q4)

**What it is:** A `letterbox` is a `Map<string, string list>` — each sender name maps to their queue of unread messages (oldest first, so the head is the next to be read). `post` appends to the end; `read` removes the head. The state monad (with option) threads the map through operations; `read` fails with `None` if there are no messages.

---

#### Tally clicker (carry propagation) (2024re/2025re Q4)

**What it is:** A clicker has `n` wheels, each cycling through a list of characters. Clicking increments the *rightmost* wheel; if it overflows back to position 0, carry to the next wheel (like an odometer). The clicker stores the wheel as a `char[]` (array for O(1) lookup) and positions as an `int list`.

The carry propagation reverses the list, walks from head (now the rightmost wheel), and reverses again:
```fsharp
let click (Clicker(wheel, positions)) =
    let size = wheel.Length
    let rec addCarry = function
        | [] -> []
        | i :: rest ->
            let next = (i + 1) % size
            if next = 0 then next :: addCarry rest   // overflow: carry
            else next :: rest                         // no overflow: stop
    Clicker(wheel, positions |> List.rev |> addCarry |> List.rev)
```

---

#### Dining philosophers (MailboxProcessor) (2025 Q3)

**What it is:** N philosophers sit at a table. Each needs *two* forks (left and right) to eat, but each fork is shared between adjacent philosophers. A `MailboxProcessor` acts as the table manager: it processes `Eat(p, rc)` and `Think(p)` messages. If philosopher `p` can't eat (a fork is taken), the request goes into a pending queue and is retried when someone calls `Think`.

```fsharp
// When philosopher calls Think(p):
// 1. Put down both forks
// 2. Scan pending queue — anyone who can now eat? Give them forks and unblock them.
```

---

#### Towers of Hanoi (Result monad) (2025 Q4)

**What it is:** Move a stack of discs from one peg to another, smallest-on-top. The recursive solution: move the top n-1 discs out of the way, move the largest disc, move the n-1 discs onto the destination. The state monad uses `Result` (not option) so failures carry a typed reason (`Empty of peg` or `Invalid of peg * disc * disc`).

```fsharp
// Recursive move list:
let rec solveHanoi size from via dest =
    match size with
    | 0 -> []
    | n -> solveHanoi (n-1) from dest via   // move n-1 to middle (via dest)
           @ [(from, dest)]                  // move largest
           @ solveHanoi (n-1) via from dest  // move n-1 from middle to dest
```

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
| "Atbash", "encode", "shift characters" | 2025re/2024re Q2/Q3 |
| "Collatz" | 2023 Q3 |
| "Newton", "square root", "quadratic" | 2021re Q3 |
| "big integer", "digit list", "carry" | 2020re Q3 |
| "RPN", "stack", "push", "pop", "evaluate expression" | 2020 Q4 |
| "rock-paper-scissors", "strategy", "tournament" | 2020 Q3 |
| "little-endian", "digit list to int" | 2024 Q2 (`baz`) |
| "Caesar cipher", "ROT" | 2024 Q3 |
| "Peano", `O`, `S of Peano` | 2019 Q1 |
| "memoize", "mutable Map", "cache" | 2021 Q2 |
| "look-and-say", "run-length" | 2021 Q3 |
| "balanced brackets", "matching delimiters" | 2023re Q3 |
| "assembly", "register", "MOVI", "JGTZ" | 2022re Q4 |
| "BASIC", "Goto", "Let", "If" | 2023re Q4 |
| "memory machine", "While", "Assign", "Lookup" | 2023 Q4 |
| "matrix", "dot product", `int[,]` | 2022 Q3 |
| "stack machine", `Push`, `Add`, `Mult` | 2022 Q4 |
| "direction", "North/South/East/West", "walk", "path" | 2021 Q1 |
| "golden ratio", "pi", "Nilakantha" | 2019 Q3 |
| "palindrome", "symmetric" | 2019re Q2, 2023re Q3.4 |
| "ring", "clockwise", "counterclockwise" | 2021 Q4, 2021re Q4 |
| "clicker", "tally", "wheel" | 2025re/2024re Q4 |
| "philosopher", "fork", "dining" | 2025 Q3 |
| "Hanoi", "peg", "disc", "move disc" | 2025 Q4 |
| "letterbox", "post", "read", "sender" | 2024 Q4 |
| "transactions", "Pay", "Receive", "balance" | 2024 Q1 |
| "shape", "area", "circumference", "Rectangle", "Circle" | 2025re/2024re Q1 |
| "binary search", "oracle", "guessResult" | 2022re Q3 |

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

---

## Q2 Code Comprehension History

Every exam gives three mystery functions. This table shows what each one was, so you can pattern-match quickly.

| Year | foo | bar | baz |
|---|---|---|---|
| **2025** | merge two sorted lists (pick smaller head) | mergeSort (recursive, splits on `List.length`) | — |
| **2025re/2024re** | ROT-3 char cipher (a→d, x→a via +3/-23) | string → char list (`[for c in str -> c]`) | encode full string with ROT-3 |
| **2024** | digit char → int (`int c - int '0'`) | string → char list | little-endian digit list → int (`x + 10 * baz xs`) |
| **2023re** | isEven (`0 → true`, decrements to 0 alternating) | isOdd (mutual recursion with `and`) | partition list → (evens, odds) |
| **2023** | stripPrefix (does list A start list B?) | removeAll (strips A from everywhere in B) | remove sub-pattern from string |
| **2022re** | filter list by predicate (= `List.filter`) | apply multiple filters sequentially | combine all predicates via `List.forall` |
| **2022** | int → binary string (last branch has `when` guard = warning) | map foo over int list | — |
| **2021** | memoize function using mutable `Map` cache | raw Fibonacci (calls itself twice, exponential) | memoized Fibonacci (`baz = foo bar`) |
| **2020re** | bubbleStep: one pass, swaps adjacent out-of-order | isSorted: check all pairs with `List.pairwise` | bubbleSort: repeat bubbleStep until sorted |
| **2020** | removeFirst: find+remove first occurrence of x | prependAll: add element to each sublist | permutations (uses foo to remove, bar to extend) |
| **2019re** | string → char list by index iteration | palindrome check (list = List.rev list) | — |
| **2019** | removeFirst returning `Some rest` or `None` | isPermutation (check if foo works for all elements) | — |

**Key recognition tricks:**
- If foo has `when x > 'w' → char (int x - 23)` and `when x < 'x' → char (int x + 3)`: it's ROT-3, warning on last guard
- If foo decrement-recurses (`x-1` then bounces between two functions): it's isEven/isOdd, needs `and` keyword
- If foo picks the smaller of two heads: it's merge, bar is mergeSort
- If foo has `int c - int '0'`: digit char to int, baz does little-endian decode
- If baz = `fun x -> foo (bar x)` pattern: baz is memoized version of bar

---

## Code Template Library

### 1. Accumulator (tail-recursive rewrite)

Add inner `aux` with `acc`. The pattern:

```fsharp
// BEFORE (non-tail-recursive — pending operation after recursive call):
let rec foo lst =
    match lst with
    | [] -> 0
    | x :: xs -> x + foo xs   // "+ foo xs" pending on stack

// AFTER (tail-recursive — acc absorbs all pending work):
let foo lst =
    let rec aux acc lst =
        match lst with
        | [] -> acc            // base case: return accumulated result
        | x :: xs -> aux (acc + x) xs   // update acc FIRST, then tail call
    aux 0 lst                  // start with identity value for the operation
```

For list-building (not sum), reverse at the end:
```fsharp
let fooAcc lst =
    let rec aux acc lst =
        match lst with
        | [] -> List.rev acc   // reverse because we prepended (cheapest)
        | x :: xs -> aux (processedX :: acc) xs
    aux [] lst
```

### 2. CPS (continuation-passing style)

The key: wrap the pending operation into the continuation `c`, recurse with `aux xs newCont`. Base case calls `c` with the base value.

```fsharp
// General CPS template for list processing:
let fooTail lst =
    let rec aux lst c =
        match lst with
        | [] -> c baseValue           // apply continuation to base value
        | x :: xs ->
            aux xs (fun result -> c (combine x result))
            // instead of: combine x (aux xs)
            // we:  aux xs, then continuation does the combining
    aux lst id                        // id = "just return whatever you get"

// Concrete example — CPS merge (foo from 2025):
let footail a b =
    let rec aux a b c =
        match a, b with
        | x :: ra, y :: rb when x < y -> aux ra (y :: rb) (fun res -> c (x :: res))
        | x :: ra, y :: rb            -> aux (x :: ra) rb  (fun res -> c (y :: res))
        | [], _  -> c b
        | _,  [] -> c a
    aux a b id
```

**How to explain CPS in writing:** "The pending `x :: (recursive call)` would leave a stack frame. In CPS, instead of waiting for the recursive call, we pass a continuation `fun result -> c (x :: result)` that captures the pending cons. The recursive call `aux xs newCont` is then the last thing done, so no stack frame is needed."

### 3. State Monad — with `option` (can fail)

Used when operations can fail (empty queue, division by zero, empty stack).

```fsharp
type StateMonad<'a> = SM of (state -> ('a * state) option)

let ret x = SM (fun s -> Some (x, s))
let fail  = SM (fun _ -> None)

let bind f (SM a) : StateMonad<'b> =
    SM (fun s ->
        match a s with
        | Some (x, s') ->
            let (SM g) = f x
            g s'
        | None -> None)

let (>>=)  x f = bind f x
let (>>>=) x y = x >>= (fun _ -> y)

let evalSM initState (SM f) = f initState
```

Used in: 2024 (letterbox), 2022 (stack machine), 2021re (rational), 2020 (RPN calculator).

### 4. State Monad — without `option` (always succeeds)

Used when all operations are guaranteed to succeed.

```fsharp
type StateMonad<'a> = SM of (state -> 'a * state)

let ret x = SM (fun s -> (x, s))

let bind f (SM a) : StateMonad<'b> =
    SM (fun s ->
        let x, s' = a s
        let (SM g) = f x
        g s')

let (>>=)  x f = bind f x
let (>>>=) x y = x >>= (fun _ -> y)

let evalSM initState (SM f) = f initState
```

Used in: 2024re/2025re (clicker), 2022re (assembly).

### 5. State Monad — with `Result` (typed error)

Used when operations fail with a specific error type (not just `None`).

```fsharp
type MyMonad<'a> = HM of (state -> Result<'a * state, error>)

let ret x = HM (fun h -> Ok (x, h))
let fail err = HM (fun _ -> Error err)

let bind f (HM a) =
    HM (fun h ->
        match a h with
        | Ok (x, h') ->
            let (HM g) = f x
            g h'
        | Error err -> Error err)

let (>>=)  a f = bind f a
let (>>>=) a b = a >>= (fun _ -> b)
```

Used in: 2025 (Towers of Hanoi with `error = Empty of peg | Invalid of ...`).

### 6. State Monad — with extra read-only parameter

When the computation reads from a fixed program/config that never changes.

```fsharp
type StateMonad<'a> = SM of (program -> state -> 'a * state)

let ret x = SM (fun _ s -> (x, s))

let bind f (SM a) : StateMonad<'b> =
    SM (fun p s ->
        let x, s' = a p s
        let (SM g) = f x
        g p s')   // pass p through unchanged

let evalSM p (SM f) = f p (emptyState p)

// Primitive operations:
let readProgram = SM (fun p s -> (p, s))   // read the fixed program
let getState    = SM (fun _ s -> (s, s))   // read current state
```

Used in: 2023re (BASIC interpreter — program is read-only, state mutates).

### 7. StateBuilder (identical in every exam)

```fsharp
type StateBuilder() =
    member this.Bind(f, x)    = bind x f    // NOTE: f=monad, x=continuation (SWAPPED from what you'd expect)
    member this.Return(x)     = ret x
    member this.ReturnFrom(x) = x
    member this.Combine(a, b) = a >>= (fun _ -> b)

let state = StateBuilder()

// Usage:
let myComputation =
    state {
        let! x = someMonadicOp     // runs op, binds result to x
        do! anotherMonadicOp       // runs op, discards () result
        return x + 1               // wraps result in monad
    }
```

**Why `Bind(f, x) = bind x f` instead of `bind f x`?**
F# calls `this.Bind(computation, continuation)` — the CE puts the monad first and continuation second. Our `bind` expects `bind continuation monad`, so we swap.

### 8. Async.Parallel Recipe

```fsharp
// Pattern 1 — map over a list:
let result =
    items
    |> List.map (fun item -> async { return processItem item })
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Array.sum   // or Array.toList, String.concat " ", etc.

// Pattern 2 — chunked range (for parallel computation over [lo..hi]):
let parallelProcess lo hi numThreads =
    let total = hi - lo + 1
    let chunkSize = max 1 ((total + numThreads - 1) / numThreads)  // ceiling division
    [lo..hi]
    |> List.chunkBySize chunkSize
    |> List.map (fun chunk ->
        async { return chunk |> List.sumBy processItem })
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Array.sum

// Pattern 3 — word-level parallelism (split string by spaces):
let parProcess (s : string) =
    s.Split([|' '|])
    |> Array.map (fun word -> async { return processWord word })
    |> Async.Parallel
    |> Async.RunSynchronously
    |> String.concat " "
```

### 9. MailboxProcessor (Actor Model)

```fsharp
// Step 1: define message types
type message =
    | DoWork of int * AsyncReplyChannel<unit>  // async reply (caller blocks)
    | Notify of int                             // fire-and-forget (caller doesn't block)

// Step 2: define agent wrapper type
type myAgent = Agent of MailboxProcessor<message>

// Step 3: write the message loop
let inbox (mbox : MailboxProcessor<message>) =
    let rec loop state =
        async {
            let! msg = mbox.Receive()              // await next message
            match msg with
            | DoWork(x, rc) ->
                let newState = update state x
                rc.Reply(())                        // unblock the caller
                return! loop newState               // tail-call to keep looping
            | Notify x ->
                return! loop (update state x)
        }
    loop initialState

// Step 4: wrap in constructor/accessor functions
let newAgent ()            = Agent (MailboxProcessor.Start inbox)
let callAgent (Agent mb) x = mb.PostAndReply(fun rc -> DoWork(x, rc))  // blocks
let notifyAgent (Agent mb) x = mb.Post(Notify x)                        // non-blocking
```

**Key rule:** `PostAndReply` blocks the calling thread until `rc.Reply()` is called. `Post` fires and forgets. A pending queue (list of `(id, rc)` pairs) handles "wants to act but can't yet" scenarios.

### 10. JParsec Quick Reference

```fsharp
open JParsec.TextParser

// ── Primitives ────────────────────────────────────────────────────
pchar 'a'                     // matches exactly char 'a', returns char
pstring "hello"               // matches exactly string "hello", returns string
pint32                        // matches sequence of digits, returns int
digit                         // matches one '0'-'9' char, returns char
satisfy (fun c -> c = 'a')   // matches one char where predicate is true

// ── Sequencing ───────────────────────────────────────────────────
p1 >>. p2        // run p1 then p2, return p2's result  (discard LEFT)
p1 .>> p2        // run p1 then p2, return p1's result  (discard RIGHT)
p1 .>>. p2       // run p1 then p2, return PAIR (result1, result2)

// Memory aid: dots show which side to KEEP:
//   .>> = keep left   (dot is on the LEFT of >>)
//   >>. = keep right  (dot is on the RIGHT of >>)
//   .>>. = keep both

// ── Transformation ───────────────────────────────────────────────
p |>> f          // run p, if succeeds apply f to result  (like List.map for parsers)
p |>> fun _ -> x // run p, ignore its result, return x instead

// ── Alternation ──────────────────────────────────────────────────
p1 <|> p2        // try p1; if it fails WITHOUT consuming input, try p2

// ── Repetition ───────────────────────────────────────────────────
many p           // run p 0 or more times, return list  (never fails)
many1 p          // run p 1 or more times, return list  (fails if 0 matches)
sepBy p sep      // p separated by sep, return list  (0 or more)
sepBy1 p sep     // p separated by sep, return list  (1 or more)

// ── Recursive grammars ───────────────────────────────────────────
let myParser, myRef = createParserForwardedToRef<resultType>()
// ... define other parsers that use myParser ...
do myRef := actualParserDefinition  // close the loop (MUST come after all definitions)

// ── Common patterns ──────────────────────────────────────────────
// Parse keyword and return DU case:
pstring "start" |>> fun _ -> Start

// Parse bracketed expression (recursive):
pchar '(' >>. ParseInner .>> pchar ')'

// Parse comma-separated list of ints:
sepBy pint32 (pchar ',')

// Parse and transform char list to string:
many (satisfy (fun c -> Char.IsLetter c)) |>> (fun cs -> System.String(Array.ofList cs))

// Parse number prefixed by label:
pstring "Push" >>. pchar ' ' >>. pint32 |>> Push

// Example: parsePeg from 2025
let pstart  = pstring "start"  |>> fun _ -> Start
let pmiddle = pstring "middle" |>> fun _ -> Middle
let pgoal   = pstring "goal"   |>> fun _ -> Goal
let parsePeg = pstart <|> pmiddle <|> pgoal
let parseMove = parsePeg .>>. (pstring "->" >>. parsePeg) .>> pchar ';'
let parseMoves = many parseMove
```

### 11. Custom Type Fold Template

```fsharp
// For custom linked list:  type myList = Nil | Node of 'a * myList
let rec myFold (f : 'acc -> 'a -> 'acc) (acc : 'acc) = function
    | Nil          -> acc
    | Node(x, rest) -> myFold f (f acc x) rest   // tail-recursive

// Then everything else uses myFold:
let myLength lst = myFold (fun acc _ -> acc + 1) 0 lst
let myToList lst = myFold (fun acc x -> acc @ [x]) [] lst

// For binary tree:  type 'a bintree = Leaf | Node of 'a bintree * 'a * 'a bintree
let rec treeFold (f : 'acc -> 'a -> 'acc) (acc : 'acc) = function
    | Leaf              -> acc
    | Node(left, x, right) ->
        let accLeft = treeFold f acc left
        let accMid  = f accLeft x
        treeFold f accMid right   // in-order traversal
```

### 12. Infinite Sequence Patterns

```fsharp
// Seq.unfold: generate infinite sequence
// unfold (state -> (value * newState) option) initialState
let fibs = Seq.unfold (fun (a, b) -> Some(a, (b, a + b))) (0, 1)
// Fibonaccis: 0, 1, 1, 2, 3, 5, 8, ...

// Seq.initInfinite: index-based
let naturals = Seq.initInfinite id       // 0, 1, 2, 3, ...
let evens    = Seq.initInfinite ((*) 2)  // 0, 2, 4, 6, ...

// Take first n elements:
Seq.take 10 fibs |> Seq.toList

// Zip two sequences:
Seq.zip seq1 seq2   // stops at the shorter one (use with infinite)
```

### 13. Lazy List Pattern

```fsharp
// Type definition (used in 2020):
type 'a llist = Cons of (unit -> 'a * 'a llist)

// Step function (un-thunk to get head + tail):
let step (Cons f) = f ()

// Construct:
let cons hd tl = Cons (fun () -> hd, tl)

// Build with unfold:
let unfold f b =
    let rec aux b = Cons (fun () ->
        let (a, b') = f b
        a, aux b')
    aux b

// Fibonacci lazy list:
let fibll = unfold (fun (a, b) -> a, (b, a + b)) (0, 1)

// Take first n:
let takeFirst n ll =
    let rec aux n (Cons f) =
        if n = 0 then []
        else let (x, rest) = f () in x :: aux (n-1) rest
    aux n ll
```

---

## State Monad Variants by Year

| Year | Monad type signature | Fails? | Notes |
|---|---|---|---|
| **2025** Hanoi | `HM of (hanoi -> Result<'a*hanoi, error>)` | Typed error | `fail err` not `fail`; `Ok`/`Error` pattern |
| **2024** Letterbox | `SM of (letterbox -> ('a*letterbox) option)` | Yes → `None` | `fail = SM(fun _ -> None)` |
| **2024re/2025re** Clicker | `SM of (clicker -> 'a * clicker)` | Never | No option; bind uses `let x, s' = a s` |
| **2023re** BASIC | `SM of (basicProgram -> state -> 'a * state)` | Never | Extra read-only param; `evalSM p (SM f) = f p (emptyState p)` |
| **2022re** Assembly | `SM of (state -> 'a * state)` | Never | No option |
| **2022** Stack Machine | `SM of (stack -> ('a*stack) option)` | Yes → `None` | Standard option monad |
| **2021re** Rational | `SM of (stack -> ('a*stack) option)` | Yes → `None` | Standard option monad |
| **2020** RPN | `SM of (stack -> ('a*stack) option)` | Yes → `None` | Standard option monad |

**When to use which variant:**
- Operations that **always succeed** (read/write clicker wheels, run assembly): no option
- Operations that **might fail with generic None** (empty stack, empty queue): option monad
- Operations that **fail with a specific reason** (wrong move in Hanoi): Result monad
- Program has a **fixed configuration** alongside mutable state: extra read-only parameter

---

## Common Exam Tricks and Gotchas

### Incomplete pattern match warning — always the same fix
The warning appears when ALL branches have `when` guards. The compiler cannot prove coverage.
```fsharp
// BROKEN — warning because all three branches have guards:
let foo = function
    | c when Char.IsWhiteSpace c -> c
    | c when c > 'w'             -> char (int c - 23)
    | c when c < 'x'             -> char (int c + 3)   // ← WARNING here

// FIXED — remove guard from last branch (it's logically true anyway):
let foo2 = function
    | c when Char.IsWhiteSpace c -> c
    | c when c > 'w'             -> char (int c - 23)
    | c                          -> char (int c + 3)   // ← no guard, always matches
```

Seen in: 2025re Q2.2, 2024re Q2.2, 2022 Q2, 2023re Q2.

### `and` keyword — mutual recursion
```fsharp
let rec foo x = ... bar (x-1) ...   // foo calls bar
and     bar x = ... foo (x-1) ...   // bar calls foo

// Without `and`: the second `let rec bar` would work (bar can see foo),
// but `let rec foo` would fail because bar doesn't exist yet.
// `and` makes BOTH definitions in scope for each other simultaneously.
```

Seen in: 2023re Q2 (isEven/isOdd), 2025 Q2 (merge/mergeSort), 2021 Q2 (memo/fib).

**Exam question format:** "Can `and` be replaced by `let rec`?" Answer: No, because `foo` references `bar` which isn't defined yet at that point.

### Proving a function is NOT tail-recursive — evaluation trace template

Write 3–4 evaluation steps and show the **pending operation** left on the stack:

```
fooTR [1;2;3]
= 1 :: foo [2;3]          ← PENDING: (1 ::) waits on stack
= 1 :: (2 :: foo [3])     ← ANOTHER FRAME: (2 ::) also on stack
= 1 :: (2 :: (3 :: foo []))
= 1 :: (2 :: (3 :: 0))    ← base case reached, now unwind
= 1 :: (2 :: 3)
= 1 :: [2;3]
= [1;2;3]
```

The recursive call `foo [2;3]` is NOT in tail position because `1 ::` is still pending after it. For a list of length n, n stack frames accumulate simultaneously.

### `List.fold` vs `List.foldBack`
```fsharp
// List.fold   f acc [x1;x2;x3] = f (f (f acc x1) x2) x3  ← left to right, TAIL-RECURSIVE
// List.foldBack f [x1;x2;x3] z = f x1 (f x2 (f x3 z))   ← right to left, NOT tail-recursive
```

Use `List.foldBack` when the LAST element needs to be processed first (e.g., `baz` little-endian decoding). Use `List.fold` when building an accumulator left-to-right.

### `Map` operations cheat sheet
```fsharp
Map.empty                       // empty map
Map.ofList [(k1,v1); (k2,v2)]  // build from list
Map.find k m                    // get value (exception if missing)
Map.tryFind k m                 // get value as option (None if missing)
Map.add k v m                   // insert/replace (returns new map, original unchanged)
Map.remove k m                  // remove key (returns new map)
Map.containsKey k m             // true if key exists
Map.filter pred m               // keep entries where pred k v = true
Map.minKeyValue m               // (smallest key, its value)
Map.maxKeyValue m               // (largest key, its value)
defaultArg (Map.tryFind k m) defaultVal  // tryFind with a default
```

### `Set` operations cheat sheet
```fsharp
Set.empty                       // empty set
Set.ofList [x; y; z]           // build from list
Set.add x s                     // add element (no duplicates)
Set.contains x s                // true if x is in s
Set.union s1 s2                 // all elements from both
Set.intersect s1 s2             // elements in both
Set.difference s1 s2            // elements in s1 but not s2
Set.fold f acc s                // fold over elements (sorted order)
Set.isEmpty s                   // true if s = {}
Set.count s                     // number of elements
```

### String / char operations
```fsharp
// String → char list:
[for c in str -> c]             // list comprehension (bar in many exams)
str |> Seq.toList               // also works

// Char list → string:
System.String(Array.ofList chars)    // via char array
chars |> List.map string |> String.concat ""  // via string list

// Char ↔ int:
int 'a'                // 97 (ASCII value)
char 97                // 'a'
int 'A'                // 65
Char.IsLetter c        // true for a-z, A-Z
Char.IsWhiteSpace c    // true for space, tab, newline
Char.ToLower c         // 'A' → 'a', others unchanged

// String slicing:
str.[0..i-1]           // first i chars (0-indexed, inclusive)
str.[i..]              // from index i to end
str.Length             // length of string

// Caesar cipher formula (for lowercase letters):
// encrypt: char(int 'a' + (int c - int 'a' + offset) % 26)
// Atbash:  char(219 - int c)   [219 = int 'a' + int 'z' = 97 + 122]
// Atbash is self-inverse: applying twice returns original
```

### Recursion over custom types — the fold pattern

When the exam defines a custom type and asks you to write `fold` for it, the shape mirrors the type:

```fsharp
// If the type has constructors:  Nil | Cons of 'a * myList
// Then fold has the same shape: fold fNil fCons acc
let rec myFold fNil fCons acc = function
    | Nil           -> fNil acc
    | Cons(x, rest) -> myFold fNil fCons (fCons acc x) rest
```

Or more commonly, for list-like types with a single base case:
```fsharp
let rec fold f acc = function
    | Nil           -> acc                     // base value returned directly
    | Cons(x, rest) -> fold f (f acc x) rest  // apply f then recurse
```

Once fold exists, everything else is one-liner:
```fsharp
let length lst  = fold (fun acc _ -> acc + 1) 0 lst
let sum lst     = fold (+) 0 lst
let toList lst  = fold (fun acc x -> acc @ [x]) [] lst
let filter p lst = fold (fun acc x -> if p x then acc @ [x] else acc) [] lst
```
