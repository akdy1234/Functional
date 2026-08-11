# Q1 Notes — Types, Recursion & Tail Recursion

## 1. Discriminated Unions

A type where each value can be one of several "shapes". Each shape can carry different data.

```fsharp
type animal =
    | Dog of string            // carries a name
    | Cat of string * int      // carries a name AND an age
    | Fish                     // carries nothing
```

Creating values:

```fsharp
let a = Dog "Rex"
let b = Cat ("Whiskers", 5)
let c = Fish
```

## 2. Pattern Matching

How you "take apart" a discriminated union and do something with each case.

```fsharp
let describe a =
    match a with
    | Dog name -> "A dog called " + name
    | Cat (name, age) -> "A cat called " + name
    | Fish -> "Just a fish"
```

- The variable names (like `name`, `age`) get **bound** to the data inside each case
- Use `_` when you don't care about a value: `| Dog _ -> "some dog"`

## 3. Recursive Types

A type that **references itself**. Like a linked list — each node points to the rest.

```fsharp
type shapeList =
    | Empty                                  // base case: the end
    | AddShape of float * float * shapeList  // width, height, then THE REST
```

A value looks like a chain:

```fsharp
let shapes = AddShape(2.0, 3.0, AddShape(4.0, 5.0, AddShape(1.0, 1.0, Empty)))
//           ^ first shape       ^ second shape       ^ third shape      ^ end
```

## 4. Basic Recursion (NOT tail-recursive)

When you write a function over a recursive type, the function becomes recursive too.

```fsharp
let rec totalArea sl =
    match sl with
    | Empty -> 0.0                                   // base case: nothing left, return 0
    | AddShape (w, h, rest) -> (w * h) + totalArea rest  // do something + recurse
```

**Why this is NOT tail-recursive:** the `+` happens AFTER `totalArea rest` returns. The call stack has to remember the `(w * h) +` part while waiting.

```
totalArea [shape1, shape2, shape3]
= (2*3) + totalArea [shape2, shape3]           -- waiting...
= (2*3) + ((4*5) + totalArea [shape3])         -- still waiting...
= (2*3) + ((4*5) + ((1*1) + totalArea Empty))  -- still waiting...
= (2*3) + ((4*5) + ((1*1) + 0.0))             -- NOW unwind everything
= (2*3) + ((4*5) + 1.0)
= (2*3) + 21.0
= 27.0
```

## 5. Tail Recursion with Accumulator

Instead of doing work AFTER the recursive call, we carry a running total INTO the call.

```fsharp
let totalAreaAcc sl =
    let rec loop acc remaining =          // acc = running total, remaining = what's left
        match remaining with
        | Empty -> acc                    // done! return whatever we've accumulated
        | AddShape (w, h, rest) -> loop (acc + (w * h)) rest     // update acc, keep going. NOTHING after this call.
    loop 0.0 sl                           // kick it off: acc starts at 0.0, remaining is the full input
```

## 6. Custom Folds

A custom fold is your tail-recursive accumulator function, **generalized** — instead of hardcoding the operation (like `+`), the caller supplies a function `f`.

### The general shape

fsharp

```fsharp
let myFold f acc data =
    let rec aux acc remaining =
        match remaining with
        | <base case>     -> acc
        | <recursive case> -> aux (f acc <current value>) <rest>
    aux acc data
```

Same structure as your tail-recursive accumulator function. The only difference is `acc + x` becomes `f acc x` — the operation is supplied by the caller.

### Example: fold over a list (like List.fold)

fsharp

```fsharp
let myListFold f acc lst =
    let rec aux acc remaining =
        match remaining with
        | [] -> acc
        | x :: xs -> aux (f acc x) xs
    aux acc lst

// Usage:
myListFold (+) 0 [1; 2; 3]            // 6  — sum
myListFold (fun acc _ -> acc + 1) 0 [1; 2; 3]  // 3  — count
```

### Example: fold over a custom recursive type (route)

fsharp

```fsharp
type route =
    | Done
    | Walk of int * route
    | Bus of int * route

let routeFold f acc r =
    let rec aux acc remaining =
        match remaining with
        | Done -> acc
        | Walk (d, rest) -> aux (f acc d) rest
        | Bus (d, rest) -> aux (f acc d) rest
    aux acc r

// Usage:
routeFold (+) 0 myTrip                            // total distance
routeFold (fun acc _ -> acc + 1) 0 myTrip         // number of steps
```

### Example: linked list & multiple f calls per step

```fsharp
let shapeListFold f acc sl =
    let rec aux acc remaining =
        match remaining with
        | Empty -> acc
        | AddShape (s1, s2, rest) -> aux (f (f acc s1) s2) rest
    aux acc sl
```

The key bit is `f (f acc s1) s2`:

- Inner: `f acc s1` produces a new accumulator
- Outer: `f (that result) s2` produces an even newer accumulator
- Then pass that into the recursive call

It's the same template as before — base case returns acc, recursive case updates acc and recurses. The only difference is you update acc twice (once per shape) in each step.

### Example: fold over a 2D structure (rectangle)

For Q1.5 on the 2025 exam — fold over every coordinate in a rectangle, row by row:

fsharp

```fsharp
let rectFold f acc (R(C(cx, cy), w, h)) =
    let rec aux acc x y =
        if y >= h then acc
        elif x >= w then aux acc 0 (y + 1)
        else aux (f acc (C(cx + x, cy + y))) (x + 1) y
    aux acc 0 0
```

Same 2D walking pattern as `coords`, but instead of building a Set, you apply `f` to every coordinate.

### Re-implementing earlier functions with the fold

The exam usually then asks you to rewrite earlier functions using your fold. The trick is to figure out: what function `f` and what initial `acc` produce the right behavior?

fsharp

```fsharp
// totalDistance with routeFold
let totalDistance r = routeFold (fun acc d -> acc + d) 0 r

// coords2 with rectFold (from 2025 exam Q1.5)
let coords2 r = rectFold (fun acc c -> Set.add c acc) Set.empty r
```

The pattern: figure out what one "step" looks like and pass it as `f`. Initial `acc` is whatever an empty input would return (0, [], Set.empty, etc.)

---
### The three parts to remember:

1. **Inner helper function** `loop` with an `acc` parameter and the remaining data
2. **Base case** returns `acc` (not a hardcoded value like 0!)
3. **Recursive case** updates `acc` and passes the rest — the recursive call is the LAST thing

### Why this IS tail-recursive:

```
loop 0.0 [shape1, shape2, shape3]
= loop 6.0 [shape2, shape3]        -- no waiting, acc is updated
= loop 26.0 [shape3]               -- no waiting
= loop 27.0 Empty                  -- no waiting
= 27.0                             -- done in one step
```

No build-up of pending operations. Each call is self-contained.

## What makes a function tail recursive?
Definition (Tail-Recursive Form): a function is in tail-recursive form if, **for every recursive function call that it makes, that no additional work is performed after that call**. In other words, a tail recursive function immediately returns the result of any recursive call that it makes.

## The Pattern

Every Q1 on the exam follows this exact escalation:

```
1. They give you a TYPE             (discriminated union, often recursive)
2. Write a RECURSIVE function       (let rec, pattern match, NOT tail-recursive)
3. Write a TAIL-RECURSIVE version   (add acc parameter, inner helper, loop 0 input)
4. Write a NON-RECURSIVE version    (List.fold, List.map, comprehensions)
5. Write a CUSTOM FOLD              (your own fold over the custom type)
```

---

# Q2 Notes — Code Comprehension

## 1. Reading Code / Explaining What It Does

- Trace through with a small input
- Describe the **high-level purpose**, not the mechanics
- Bad answer: "it pattern matches and recurses on the tail"
- Good answer: "it performs one pass of bubble sort, moving the largest element to the end"
- Give a good name (e.g. `bubblePass`, `mergeSort`, `encrypt`)

## 2. Constraints on Inputs

Ask: what inputs would break this function?

- "both lists must be sorted"
- "input must be digit characters"
- "list must be non-empty"

## 3. Swapping Match Cases

F# matches **top to bottom**, first match wins.

Key rule: **variable patterns match everything**. Specific patterns (`0`, `1`, `[x]`) only match one thing.

```fsharp
| 0 -> []       // matches only 0
| 1 -> a        // matches only 1
| l -> ...      // matches EVERYTHING — including 0 and 1
```

If you move `| l` above `| 1`, it steals the match. `| 1` becomes unreachable.

Structurally disjoint patterns (like `[]`, `[x]`, `x::y::rest`) **cannot** steal from each other — swapping them changes nothing.

## 4. Proving NOT Tail-Recursive (step-by-step eval)

Pick a small input. Show each step. Point out the pending work.

```fsharp
let rec sum lst =
    match lst with
    | [] -> 0
    | x :: xs -> x + sum xs
```

```
sum [2; 5; 1]
--> 2 + sum [5; 1]
--> 2 + (5 + sum [1])
--> 2 + (5 + (1 + sum []))
--> 2 + (5 + (1 + 0))
--> 2 + (5 + 1)
--> 2 + 6
--> 8
```

Explanation: "The `+` is applied after each recursive call returns. The recursive call is not the last operation — there is pending work on the call stack. Therefore `sum` is not tail-recursive."

## 5. Rewriting with Continuations

The pattern — move the pending work into a function you pass forward:

```fsharp
// Original:
let rec sum lst =
    match lst with
    | [] -> 0
    | x :: xs -> x + sum xs

// With continuations:
let sumTail lst =
    let rec aux lst cont =
        match lst with
        | [] -> cont 0                                      // pass base value to continuation
        | x :: xs -> aux xs (fun result -> cont (x + result))  // move the + inside continuation
    aux lst id                                               // start with id (do nothing)
```


```fsharp
let rec foo lst =
    match lst with
    | [] -> SOMETHING_SIMPLE
    | x :: xs -> SOMETHING_WITH_X (foo xs)
```

The recipe — fill in these blanks mechanically:

```fsharp
let fooCont lst =
    let rec aux lst cont =
        match lst with
        | [] -> cont SOMETHING_SIMPLE
        | x :: xs -> aux xs (fun result -> cont (SOMETHING_WITH_X result))
    aux lst id
```

That's it. Three mechanical changes:

1. Add `cont` parameter to the helper
2. Base case becomes `cont SOMETHING_SIMPLE`
3. Recursive case: take whatever was wrapping the recursive call (the `SOMETHING_WITH_X` part), and replace `foo xs` with `result` inside a lambda

Example mapping for `sum`:

- `SOMETHING_SIMPLE` = `0`
- `SOMETHING_WITH_X (foo xs)` = `x + foo xs` → so `SOMETHING_WITH_X result` = `x + result`

For `doubleAll`:

- `SOMETHING_SIMPLE` = `[]`
- `SOMETHING_WITH_X (foo xs)` = `(x * 2) :: doubleAll xs` → so `SOMETHING_WITH_X result` = `(x * 2) :: result`

Don't worry about _why_ `result` works. Just apply the recipe. The understanding will come with practice.
### The mechanical conversion:

```
Original:      | x :: xs -> SOMETHING (recurse xs)
Continuation:  | x :: xs -> recurse xs (fun result -> cont (SOMETHING result))
```

1. The pending work (`SOMETHING`) moves inside `(fun result -> ...)`
2. `cont` wraps the whole thing — it's the chain of all previous pending work
3. Start with `id` because at the beginning there's no pending work
4. Base case calls `cont` with the base value

---

# Q3 Notes — Async, Mailbox & Parallelism

## 1. Records (Q3.1-3.2 — free points)

Records are types with named fields. Immutable by default.

```fsharp
type store = {
    data : int
    owner : int option     // None = no owner, Some pid = owned by pid
}
```

Creating:

```fsharp
let s = { data = 0; owner = None }
```

Updating (copy and update — original is unchanged):

```fsharp
let s2 = { s with data = 42 }
// s2 = { data = 42; owner = None }
```

Reading fields:

```fsharp
s.data       // 0
s.owner      // None
```

Pattern matching on option fields:

```fsharp
match s.owner with
| None -> "no owner"
| Some pid -> sprintf "owned by %d" pid
```

Printing AND returning a value in the same branch:

```fsharp
| Some pid2 ->
    printfn "already locked by %d" pid2     // side effect runs first
    st                                       // this is the return value
```

## 2. MailboxProcessor (Q3.3 — hardest, OK to skip)

A mailbox sits in a loop, receives messages one at a time, updates state. Like a goroutine with a channel in Go.

```fsharp
type message =
    | Add of int                        // fire and forget
    | Get of AsyncReplyChannel<int>     // request and wait for reply

let inbox (mbox : MailboxProcessor<message>) =
    let rec loop state = async {
        let! msg = mbox.Receive()       // wait for next message
        match msg with
        | Add x ->
            return! loop (state + x)    // update state, keep looping
        | Get rc ->
            rc.Reply(state)             // send state back to caller
            return! loop state          // keep looping unchanged
    }
    loop 0                              // start with initial state
```

Key syntax inside `async { }`:

- `let! msg = mbox.Receive()` — wait for a message (the `!` means "async wait")
- `return! loop newState` — tail-recursive call inside async
- `rc.Reply(value)` — send a value back on the return channel

## 3. Wrapper Functions (Q3.4 — formulaic)

The exam always asks for thin functions that hide the mailbox:

```fsharp
// Create the mailbox
let start () = MailboxProcessor.Start(inbox)

// Fire and forget — caller doesn't wait
let add x (mb : MailboxProcessor<message>) =
    mb.Post(Add x)

// Request and wait — caller blocks until reply
let get (mb : MailboxProcessor<message>) =
    mb.PostAndReply(fun rc -> Get rc)
```

The rule:

- `mb.Post(...)` → fire and forget, returns unit immediately
- `mb.PostAndReply(fun rc -> Message(args, rc))` → blocks until `rc.Reply` is called

## 4. Async.Parallel (Q3.5)

Run multiple async tasks at the same time. Always the same shape:

```fsharp
let mb = start ()

let tasks =
    [| for i in 1..5 ->
         async {
             add i mb
         } |]

tasks
|> Async.Parallel
|> Async.RunSynchronously
|> ignore

printfn "total: %d" (get mb)   // 15
```

Pattern:

1. Create an array of `async { }` blocks
2. Pipe through `Async.Parallel` (runs them all at once)
3. Pipe through `Async.RunSynchronously` (wait for all to finish)
4. `|> ignore` if you don't need the return values

## Q3 Exam Strategy

- Q3.1-3.2: Simple record/array functions. **Do these first. Free points.**
- Q3.3: Complex mailbox with pending queue. **Skip if short on time.**
- Q3.4: Wrapper functions (Post / PostAndReply). **Formulaic, learn the pattern.**
- Q3.5: Async.Parallel. **Learn the shape, it's the same every year.**

---

# 

## 1. Designing Types (Q4.1 — read all of Q4 first!)

Use discriminated unions for "one of these things" and records for "a collection of fields":



```fsharp
type peg = Start | Middle | Goal

type disc = D of int

type hanoi = {
    start : disc list
    middle : disc list
    goal : disc list
}
```

**IMPORTANT:** Read all of Q4 before committing to your types. Your choices affect how easy Q4.2-4.4 are.

## 2. Result Type (Q4.2)

`Result` is a built-in discriminated union:

```fsharp
type Result<'a, 'b> =
    | Ok of 'a       // success
    | Error of 'b    // failure
```

The exam defines custom error types:

```fsharp
type error =
    | Empty of peg
    | Invalid of peg * disc * disc
```

Pattern for functions that can fail:

```fsharp
let take (p : peg) (h : hanoi) =
    match p with
    | Start ->
        match h.start with
        | [] -> Error (Empty p)                       // fail case
        | x :: xs -> Ok (x, { h with start = xs })   // success case
    | Middle ->
        match h.middle with
        | [] -> Error (Empty p)
        | x :: xs -> Ok (x, { h with middle = xs })
    | Goal ->
        match h.goal with
        | [] -> Error (Empty p)
        | x :: xs -> Ok (x, { h with goal = xs })
```

Chaining Results with `Result.bind`:


```fsharp
Ok startValue
|> Result.bind doFirstThing
|> Result.bind doSecondThing
// stops at first Error, passes Ok values forward
```

## 3. State Monad (Q4.3 — TODO: revisit)

The exam gives you all the monad code. Key idea: `HM` wraps a function that takes game state and returns Result.


```fsharp
type HM<'a> = HM of (hanoi -> Result<'a * hanoi, error>)
```

To "break the abstraction" and wrap an existing function:


```fsharp
let take2 p =
    HM (fun h -> take p h)
```

**Come back to this topic for deeper practice.**

## 4. Computation Expressions (Q4.4 — TODO)

Sugar syntax for monadic code. `let!` = bind, `do!` = bind ignore, `return` = ret.

## 5. Parser Combinators (Q4.5)

A parser is a function that tries to read a string and produce a value. Either succeeds (consuming characters and giving a value) or fails.
### Key combinators

- **`pstring "abc"`** — matches the literal string "abc"
- **`<|>`** — alternative. Try left, if fails try right.
- **`|>>`** — transform parser output. Like `List.map` for parsers.
    - `parser |>> (fun result -> newValue)`
- **`a .>> b`** — sequence both, keep only **a**'s result
- **`a >>. b`** — sequence both, keep only **b**'s result
- **`a .>>. b`** — sequence both, keep both as tuple `(a, b)`
- **`many parser`** — run parser zero or more times, return list
- **`spaces`** / **`whitespace`** / **`pwhitespace`** — match whitespace (check exam template)

Mnemonic: "the dot points to what you keep."

### Pattern: parse a literal string into a value


```fsharp
let parseX = pstring "X" |>> (fun _ -> X)
let parseO = pstring "O" |>> (fun _ -> O)
let parsePlayer = parseX <|> parseO
```

### Pattern: combining multiple parsers with literal text in between

For input like `"Player X places a tile on row midRow and column midCol"`:


```fsharp
let parseMove =
    (pstring "Player " >>. parsePlayer) .>>.
    (pstring " places a tile on row " >>. parseRow) .>>.
    (pstring " and column " >>. parseCol)
    |>> (fun ((p, r), c) -> (p, r, c))     // flatten ((a,b),c) into (a,b,c)
```

Each line: skip literal text with `>>.`, keep the value parser. Combine consecutive captures with `.>>.`. At the end, flatten the nested tuples with `|>>`.

### Pattern: sequence of items separated by whitespace


```fsharp
let parseMoves = many (parseMove .>> spaces)
```

Match a move, skip trailing whitespace, repeat zero or more times. Returns a list of moves.

## Q4 Exam Strategy

- Q4.1: Design types. **Easy if you read ahead.**
- Q4.2: Result type functions. **Pattern match on Ok/Error.** Partial credit possible if you handle the easy cases.
- Q4.3: State monad. **Mechanical wrapping pattern.** Can skip.
- Q4.4: Computation expressions. **Syntax sugar for monads.** Can skip.
- Q4.5: Parser combinators. **Formulaic, learn the operators. Good ROI.**