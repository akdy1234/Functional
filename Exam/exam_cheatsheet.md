# FP Exam Cheatsheet — Condensed Reference

---

## FIRST: which question is it?

| Q | What it almost always is |
|---|---|
| **Q1** | A custom type is defined. You write 4–6 functions for it, going from simple → tail-recursive → higher-order → fold. |
| **Q2** | Three mystery functions `foo`/`bar`/`baz` are given. You name them, fix a warning, prove one is not tail-recursive, then rewrite it. |
| **Q3** | A concrete algorithm (cipher, sorting, etc.) → then a parallel version → then a parser. |
| **Q4** | A bigger abstraction: state monad, mailbox agent, lazy lists, or rings. |

---

## Q2 — Recognising what foo/bar/baz does

> **What is Q2?** Every exam gives you three pre-written functions with meaningless names. You need to figure out what they compute (by tracing examples by hand), give them real names, and then rewrite them in different styles. The table below lets you recognise them quickly.

| Year | foo | bar | baz |
|---|---|---|---|
| **2025** | merge two sorted lists | mergeSort | — |
| **2025re/2024re** | ROT-3 char shift (a→d, x→a) — **warning: all arms have `when` guard** | string → char list | encode full string with ROT-3 |
| **2024** | digit char → int (`int c - int '0'`) | string → char list | little-endian digit list → int |
| **2023re** | isEven (decrement recursion, needs `and`) | isOdd | partition list → (evens, odds) |
| **2022re** | filter list by predicate | apply multiple filters | — |
| **2022** | int → binary string (**warning: last arm has `when` guard**) | map foo over list | — |
| **2021** | memoize a function (using mutable Map cache) | raw Fibonacci | memoized Fibonacci (`baz = foo bar`) |
| **2020re** | bubbleStep (one pass, swaps adjacent out-of-order) | isSorted | bubbleSort |
| **2020** | removeFirst (find+remove first occurrence) | prependAll | permutations |

**Quick recognition shortcuts:**
- Last arm has `when c > 'w' → char(int c - 23)` → ROT-3 cipher, has incomplete-pattern warning
- Calls `x - 1` and bounces between two functions → isEven/isOdd, needs `and`
- Picks smaller of two list heads → merge; bar = mergeSort
- `int c - int '0'` → digit decoder; baz does `x + 10 * baz xs` (little-endian)
- `mutable Map` cache → memoize; bar = Fibonacci; baz = memoized Fibonacci

---

## Q2 Sub-question Script (same every year)

| Sub-question asks | What to write |
|---|---|
| "Name the functions, state constraints" | Trace 2–3 examples by hand. Name = what it computes. Constraint = what must be true about input (e.g. "both lists must already be sorted"). |
| "Incomplete pattern warning — fix it" | Remove the `when` guard from the LAST arm. Replace `\| c when ... ->` with plain `\| c ->` |
| "Prove not tail-recursive" | Write an evaluation trace (see template below). Show the pending operation on the stack. |
| "Tail-recursive accumulator version" | Add inner function `aux` with extra `acc` parameter. Update acc first, then tail-call. |
| "CPS version" | Add `c` continuation parameter. See CPS section below. |
| "What if you swap match arms 1 and 2?" | The arm that moves up now matches first — if it's a wildcard it swallows everything below it → wrong result or infinite loop. |
| "Why is `List.length` slow here?" | It's O(n) and called at every recursion level = O(n²) total. Fix: use structural match on `[]` / `[_]` instead. |
| "Can `and` be replaced by `let rec`?" | **No.** See `and` section below. |

---

## Tail-recursive accumulator

> **What is this?** In F#, every function call uses a little piece of memory called a "stack frame". If a function calls itself 10,000 times before returning, you get 10,000 frames stacked up — and the program crashes. A function is *tail-recursive* if the very last thing it does is call itself, with nothing waiting after. If there IS something waiting (like `x +` in `x + foo xs`), it's NOT tail-recursive.
>
> **The fix:** Add an extra argument `acc` (short for "accumulator") that carries the work-in-progress result. Do all the work BEFORE the recursive call, so there is nothing left pending.
>
> **Simple example:** Summing a list.
> - Normal: `sum [1;2;3]` = `1 + sum [2;3]` = `1 + (2 + sum [3])` = ... The `+` operations pile up waiting.
> - Tail-recursive: `aux 0 [1;2;3]` → `aux 1 [2;3]` → `aux 3 [3]` → `aux 6 []` → `6`. No pending work — each call immediately does the work and moves on.

```fsharp
// BEFORE — "x +" is pending after the recursive call:
let rec sum lst =
    match lst with
    | []      -> 0
    | x :: xs -> x + sum xs   // bad: x + (...) waits on stack

// AFTER — acc absorbs all the work, nothing pending:
let sum lst =
    let rec aux acc lst =
        match lst with
        | []      -> acc               // done, return total
        | x :: xs -> aux (acc + x) xs // update FIRST, then recurse
    aux 0 lst
```

When building a **list** (not a number), prepend to acc and reverse at the end:
```fsharp
let fooAcc lst =
    let rec aux acc lst =
        match lst with
        | []      -> List.rev acc          // reverse because we prepended backwards
        | x :: xs -> aux (x :: acc) xs
    aux [] lst
```

### Evaluation trace template (proving NOT tail-recursive)

```
foo [1;2;3]
= 1 :: foo [2;3]         ← PENDING: "(1 ::)" left waiting on stack
= 1 :: (2 :: foo [3])    ← ANOTHER frame: "(2 ::)" also waiting
= 1 :: (2 :: (3 :: foo []))
= 1 :: (2 :: (3 :: []))  ← base case; now unwind all the waiting frames
= [1;2;3]
```

Write this in the exam and then say: *"The call `foo [2;3]` is not in tail position because `1 ::` must still be applied to its result. For a list of length n, n stack frames accumulate simultaneously."*

---

## CPS (Continuation-Passing Style)

> **What is this?** CPS is a different way to make a function tail-recursive when the accumulator trick isn't enough — for example when the function returns a pair, or recurses in two directions.
>
> The idea: instead of *returning* a value, you pass a function `c` (called a "continuation" — think of it as "what to do next"). The recursive call passes a new continuation that includes the pending work, so nothing is left on the stack.
>
> **Simple example:** Copying a list. Normally: `x :: copyList xs` — the `x ::` is pending. In CPS: you give `copyList xs` a function that says "when you're done, prepend x and give it to my parent". That function lives in memory (heap), not on the call stack, so there's no stack overflow.
>
> You always start the outer call with `id` (the identity function — "just return whatever you get").

```fsharp
// General CPS template:
let fooTail lst =
    let rec aux lst c =
        match lst with
        | []      -> c baseValue                               // give base value to continuation
        | x :: xs -> aux xs (fun result -> c (combine x result))
        //           ↑ recurse   ↑ new continuation wraps the pending work
    aux lst id   // start: "just return whatever comes out"
```

**Concrete: CPS merge (2025 Q2)**
```fsharp
let footail a b =
    let rec aux a b c =
        match a, b with
        | x::ra, y::rb when x < y -> aux ra (y::rb) (fun r -> c (x :: r))
        | x::ra, y::rb            -> aux (x::ra) rb  (fun r -> c (y :: r))
        | [], _ -> c b
        | _, [] -> c a
    aux a b id
```

**How to explain in writing:** *"The pending `x :: (recursive call)` would leave a stack frame. In CPS we wrap it: instead of waiting for the result, we pass `fun result -> c (x :: result)` as the next continuation. The call `aux xs newCont` is then the last thing done — nothing waits. Stack frames are replaced by closures on the heap."*

---

## Incomplete pattern match warning

> **What is this?** In F#, a `when` guard is an extra condition on a match arm (like `| c when c > 'w' ->`). If ALL arms have `when` guards, the compiler doesn't know for certain that one of them will always match — so it warns you the pattern is "incomplete".
>
> **The fix:** Remove the `when` guard from the last arm. A plain `| c ->` always matches (it's a catch-all), so the compiler is happy. This is safe because: if we reach the last arm, all previous guards have already failed — so the last guard would have been true anyway.

```fsharp
// WARNING — all three arms have guards:
let foo = function
    | c when Char.IsWhiteSpace c -> c
    | c when c > 'w'             -> char (int c - 23)
    | c when c < 'x'             -> char (int c + 3)   // ← WARNING here

// FIXED — remove guard from last arm:
let foo2 = function
    | c when Char.IsWhiteSpace c -> c
    | c when c > 'w'             -> char (int c - 23)
    | c                          -> char (int c + 3)   // ← no guard, always matches
```

Seen in: 2025re Q2, 2024re Q2, 2022 Q2, 2023re Q2.

---

## `and` keyword — mutual recursion

> **What is this?** Normally in F#, a function can only call functions defined *before* it. If `foo` calls `bar` and `bar` also calls `foo`, you have a chicken-and-egg problem — you can't define either one first.
>
> The `and` keyword solves this: it lets you define two functions at the same time so each can see the other.
>
> **Simple example:** `isEven` and `isOdd` defined by counting down. `isEven 4` → `isOdd 3` → `isEven 2` → `isOdd 1` → `isEven 0` → `true`. They call each other back and forth, so neither can be defined without the other.

```fsharp
let rec isEven x = if x = 0 then true  else isOdd  (x - 1)
and     isOdd  x = if x = 0 then false else isEven (x - 1)
// "and" makes both definitions exist at the same time
```

**Exam question:** *"Can `and` be replaced by `let rec`?"*
**Answer:** No. If you write two separate `let rec` blocks, the first one (`isEven`) references `isOdd` which doesn't exist yet — compile error.

Seen in: 2023re Q2 (isEven/isOdd), 2025 Q2 (merge/mergeSort), 2021 Q2 (memo/fib).

---

## State Monad

> **What is this?** A state monad is a pattern for threading some changing state (a stack, a map, a clicker) through a sequence of operations — without having to pass it manually to every function.
>
> Think of it like a conveyor belt: you put operations on the belt, and the state is automatically carried from one to the next. If one operation fails, the whole belt stops (for the `option` variant).
>
> **Simple example:** Imagine a stack of numbers. You want to: push 3, push 4, pop two numbers, push their sum. Without a state monad you'd pass the stack to every function and thread the result through. With the state monad, you just write `push 3`, `push 4`, `pop`, `pop`, `push result` in sequence and the state flows automatically.
>
> There are 4 variants depending on whether operations can fail and whether there's extra config:

### Variant 1: with `option` — operations can fail (2024, 2022, 2020)
```fsharp
type SM<'a> = SM of (state -> ('a * state) option)
//                   ↑ takes state    ↑ returns new value + new state, or None if failed

let ret x = SM (fun s -> Some (x, s))   // succeed with value x, state unchanged
let fail  = SM (fun _ -> None)           // always fail

let bind f (SM a) =
    SM (fun s ->
        match a s with
        | None         -> None           // already failed — stop everything
        | Some (x, s') ->
            let (SM g) = f x
            g s')                        // pass result and new state to next step

let (>>=)  m f = bind f m               // m >>= f : run m, feed result to f
let (>>>=) m n = m >>= fun _ -> n       // m >>>= n: run m, ignore result, run n
let evalSM initState (SM f) = f initState
```

### Variant 2: no option — operations always succeed (2024re/2025re, 2022re)
```fsharp
type SM<'a> = SM of (state -> 'a * state)    // no option — always returns a result

let ret x = SM (fun s -> (x, s))
let bind f (SM a) =
    SM (fun s ->
        let x, s' = a s                  // no match needed — can't fail
        let (SM g) = f x
        g s')
```

### Variant 3: with `Result` — fails with a specific error message (2025 Hanoi)
```fsharp
// Instead of None, you get Error "reason why it failed"
type HM<'a> = HM of (hanoi -> Result<'a * hanoi, error>)

let ret x    = HM (fun h -> Ok (x, h))
let fail err = HM (fun _ -> Error err)   // fail with a specific error value
let bind f (HM a) =
    HM (fun h ->
        match a h with
        | Ok (x, h')  -> let (HM g) = f x in g h'
        | Error e     -> Error e)         // propagate the error
```

### Variant 4: with extra read-only param — fixed config + mutable state (2023re BASIC)
```fsharp
// The program never changes; only the state (current line, variables) changes
type SM<'a> = SM of (program -> state -> 'a * state)
//                   ↑ fixed, read-only   ↑ mutable

let ret x = SM (fun _ s -> (x, s))
let bind f (SM a) =
    SM (fun p s ->
        let x, s' = a p s           // pass BOTH program and state
        let (SM g) = f x
        g p s')                     // pass same program to next step (unchanged)
let evalSM p (SM f) = f p (emptyState p)
```

### StateBuilder — pretty syntax for the monad (same every time)
> **What is this?** The `StateBuilder` lets you write monadic code using `let!` and `return` instead of chains of `>>=`. The compiler translates the nice syntax into monad operations for you.

```fsharp
type StateBuilder() =
    member _.Bind(f, x)    = bind x f   // NOTE: args are SWAPPED — f=monad, x=continuation
    member _.Return(x)     = ret x
    member _.ReturnFrom(x) = x
    member _.Combine(a, b) = a >>= fun _ -> b

let state = StateBuilder()

// With StateBuilder (readable):         Same as without (ugly):
state {                              //  op1 >>= fun x ->
    let! x = op1                    //  op2 x >>>= 
    do! op2 x                       //  ret (x + 1)
    return x + 1
}
```

### Which variant to use?

| Year | Type | Fails? |
|---|---|---|
| 2025 Hanoi | `HM of (hanoi -> Result<'a*hanoi, error>)` | Typed error |
| 2024 Letterbox | `SM of (letterbox -> ('a*letterbox) option)` | Yes → None |
| 2024re/2025re Clicker | `SM of (clicker -> 'a * clicker)` | Never |
| 2023re BASIC | `SM of (program -> state -> 'a * state)` | Never + read-only param |
| 2022re Assembly | `SM of (state -> 'a * state)` | Never |
| 2022 Stack Machine | `SM of (stack -> ('a*stack) option)` | Yes → None |
| 2020 RPN Calculator | `SM of (stack -> ('a*stack) option)` | Yes → None |

---

## Async.Parallel — running things at the same time

> **What is this?** Normally F# runs one thing at a time. `Async.Parallel` lets you run several computations simultaneously (on separate CPU threads) and wait for all of them to finish.
>
> **When the exam uses it:** Q3 usually ends with "now do it in parallel" — you split the input into pieces, process each piece independently at the same time, then combine the results.
>
> **The pattern is always the same 4 steps:**
> 1. Wrap each piece of work in `async { return ... }`
> 2. Combine with `Async.Parallel`
> 3. Run with `Async.RunSynchronously` (this blocks until ALL pieces are done)
> 4. Combine the results (sum, join with space, etc.)

```fsharp
// Over a list of items:
items
|> List.map (fun x -> async { return f x })   // step 1: wrap each item
|> Async.Parallel                              // step 2: combine
|> Async.RunSynchronously                      // step 3: run all, wait
|> Array.sum                                   // step 4: combine results

// Over a range [lo..hi] split into chunks:
let chunkSize = max 1 ((total + nThreads - 1) / nThreads)   // ceiling division
[lo..hi]
|> List.chunkBySize chunkSize
|> List.map (fun chunk -> async { return List.sumBy f chunk })
|> Async.Parallel |> Async.RunSynchronously |> Array.sum

// Split a string by spaces, process each word:
s.Split([|' '|])
|> Array.map (fun word -> async { return processWord word })
|> Async.Parallel |> Async.RunSynchronously
|> String.concat " "
```

---

## JParsec — building parsers from small pieces

> **What is this?** JParsec is a library for reading structured text. Instead of writing a parser by hand (tracking which character you're on, handling errors etc.), you build small parsers for tiny pieces and combine them.
>
> A parser of type `Parser<'a>` reads some characters from the input and either:
> - **Succeeds**: consumes some characters and returns a value of type `'a`
> - **Fails**: consumed nothing, so another parser can try
>
> **Simple example:** To parse `"Push 42"` into a `Push 42` value:
> - `pstring "Push"` reads the word "Push"
> - `pchar ' '` reads a space
> - `pint32` reads the number 42
> - Chain them: `pstring "Push" >>. pchar ' ' >>. pint32 |>> Push`
>
> The `.>>.` / `.>>` / `>>.` operators chain parsers. The **dot shows which side to keep**: `.>>` keeps left, `>>.` keeps right, `.>>.` keeps both.

```fsharp
// Primitives:
pchar 'a'          // matches exactly the character 'a'
pstring "hello"    // matches exactly the string "hello"
pint32             // matches an integer like 42
satisfy (fun c -> c >= 'a' && c <= 'z')   // matches one char where condition is true

// Chaining (dot = which side to KEEP):
p1 >>. p2   // run p1 then p2, KEEP p2's result  (dot on RIGHT → keep right)
p1 .>> p2   // run p1 then p2, KEEP p1's result  (dot on LEFT  → keep left)
p1 .>>. p2  // run p1 then p2, KEEP BOTH as a pair

// Transform:
p |>> f     // run p, then apply function f to the result

// Choose:
p1 <|> p2   // try p1; if it fails, try p2

// Repeat:
many p       // run p 0 or more times, returns a list
sepBy p sep  // p separated by sep, e.g. sepBy pint32 (pchar ',') parses "1,2,3"
```

**Common pattern — keyword → discriminated union case:**
```fsharp
pstring "start"  |>> fun _ -> Start    // read "start", return Start
pstring "middle" |>> fun _ -> Middle
let parsePeg = pstart <|> pmiddle <|> pgoal   // try each option in order
```

**Recursive grammars** (when a parser needs to reference itself):
```fsharp
// Use createParserForwardedToRef when grammar is recursive
// e.g. a balanced bracket string can CONTAIN another balanced bracket string
let myParser, myRef = createParserForwardedToRef<unit>()   // step 1: placeholder
let inner = pchar '(' >>. myParser .>> pchar ')'           // step 2: use placeholder
let full  = many inner |>> ignore
do myRef := full   // step 3: close the loop — MUST come after all definitions
```

---

## MailboxProcessor — message-passing agent

> **What is this?** A `MailboxProcessor` is a background worker that has its own state and processes messages one at a time from a queue. Because messages are handled one-by-one, multiple threads can send at the same time without race conditions — the agent acts as a "traffic controller".
>
> **When the exam uses it (2025 Q3 — dining philosophers):** N philosophers share forks. A `MailboxProcessor` manages the table. Philosophers send `Eat` or `Think` messages. The agent decides who can eat (both their forks are free) and puts others in a waiting queue.
>
> **Two ways to send a message:**
> - `PostAndReply` — send a message and **wait/block** until the agent replies (used for `Eat`, because the philosopher can't continue until they have forks)
> - `Post` — send and immediately move on, don't wait (used for `Think`, because you don't need a reply)

```fsharp
type msg =
    | Eat   of int * AsyncReplyChannel<unit>  // philosopher p wants to eat; reply when forks given
    | Think of int                             // philosopher p is done eating

let agent = MailboxProcessor.Start(fun mbox ->
    let rec loop state =
        async {
            let! m = mbox.Receive()    // wait for next message
            match m with
            | Eat(p, rc) ->
                // check if philosopher p can eat (both forks free)
                // if yes: give forks, rc.Reply(()) to unblock them
                // if no:  add (p, rc) to pending queue
                return! loop newState
            | Think(p) ->
                // put down forks for p
                // check pending queue: can anyone eat now? if yes, rc.Reply(()) them
                return! loop newState
        }
    loop initialState)

// How philosophers interact with the agent:
agent.PostAndReply(fun rc -> Eat(p, rc))   // BLOCKS until rc.Reply() called
agent.Post(Think p)                         // fire and forget
```

---

## Common Fixes / Gotchas

### `defaultArg` with Map lookup
```fsharp
// Instead of: match Map.tryFind k m with Some v -> v | None -> 0
defaultArg (Map.tryFind key m) 0   // returns 0 if key not in map
```

### Caesar cipher formulas
```fsharp
// Shift lowercase letter c by n positions (wrapping: z+1 = a):
char (int 'a' + (int c - int 'a' + n) % 26)
// Decrypt: encrypt with the complement offset
let decrypt s offset = encrypt s (26 - offset % 26)

// Atbash (a↔z, b↔y — each letter maps to its mirror):
char (219 - int c)   // 219 = int 'a' + int 'z' = 97 + 122
// Self-inverse: applying atbash twice returns original — so decrypt = encrypt
```

### String ↔ char list
```fsharp
[for c in str -> c]                           // string → char list
System.String(Array.ofList chars)             // char list → string
chars |> List.map string |> String.concat ""  // also works
```

### `List.fold` vs `List.foldBack`
> `fold` goes left-to-right and IS tail-recursive. `foldBack` goes right-to-left and is NOT. Use `foldBack` only when the last element must be processed first (e.g. the little-endian `baz` in 2024 Q2).

```fsharp
List.fold     f acc [x1;x2;x3]  // = f (f (f acc x1) x2) x3   — left to right, tail-recursive
List.foldBack f [x1;x2;x3] z    // = f x1 (f x2 (f x3 z))     — right to left, NOT tail-recursive
```

### Map cheat sheet
```fsharp
Map.empty                              // empty map
Map.ofList [(k1,v1); (k2,v2)]         // build from list of pairs
Map.find k m                           // get value — throws exception if missing
Map.tryFind k m                        // get value as option — None if missing
Map.add k v m                          // insert/replace (returns NEW map, original unchanged)
Map.remove k m                         // remove key (returns new map)
Map.containsKey k m                    // true if key exists
Map.filter (fun k v -> ...) m          // keep only entries where condition is true
Map.minKeyValue m                      // (smallest key, its value)
Map.maxKeyValue m                      // (largest key, its value)
defaultArg (Map.tryFind k m) default   // tryFind with fallback default value
```

### Set cheat sheet
```fsharp
Set.empty                  // empty set
Set.ofList [x; y; z]       // build from list (duplicates removed automatically)
Set.add x s                // add element
Set.contains x s           // true if x is in the set
Set.union s1 s2            // all elements from both sets
Set.intersect s1 s2        // only elements in BOTH sets
Set.difference s1 s2       // elements in s1 but NOT in s2
Set.fold f acc s           // fold over elements in sorted order
Set.isEmpty s              // true if empty
Set.count s                // number of elements
```

---

## Keyword → Year Lookup

| Keyword in problem | Go to |
|---|---|
| `Async.Parallel`, "parallel", "threads" | Q3 of most years |
| `pstring`, `pchar`, `many`, `sepBy`, "parser" | JParsec — Q3 last sub-q or Q4 last sub-q |
| `createParserForwardedToRef` | 2023re Q3, 2023 Q4 |
| `ret`/`bind`/`fail`/`>>=`, "state monad" | Q4 — pick variant from table above |
| `MailboxProcessor`, `PostAndReply`, `Receive` | 2025 Q3 |
| `and` keyword, "mutual recursion" | 2023re Q2, 2025 Q2, 2021 Q2 |
| incomplete pattern, `when` guard warning | 2025re Q2, 2022 Q2 |
| `Seq.unfold`, "infinite sequence" | 2021 Q2, 2019 Q3 |
| lazy list, `Cons of unit ->` | 2020re Q4 |
| Caesar cipher, ROT | 2024 Q3 |
| Atbash cipher | 2025re/2024re Q3 |
| little-endian digit list | 2024 Q2 |
| balanced brackets | 2023re Q3 |
| assembly / MOVI / JGTZ | 2022re Q4 |
| BASIC / Goto / Let | 2023re Q4 |
| memory machine / While / Assign | 2023 Q4 |
| Peano / `O` / `S of Peano` | 2019 Q1 |
| memoize / mutable Map cache | 2021 Q2 |
| big integer / digit list / carry | 2020re Q3 |
| RPN / push / pop / evaluate expression | 2020 Q4 |
| ring / clockwise / counterclockwise | 2021 Q4 |
| dining philosophers / forks | 2025 Q3 |
| Towers of Hanoi / peg / disc | 2025 Q4 |
| transactions / Pay / Receive | 2024 Q1 |
| quadtree / grayscale / Quad | 2022 Q1, 2022re Q1 |
| binary search tree / bintree | 2020re Q1 |
| matrix / dot product / `int[,]` | 2022 Q3 |
