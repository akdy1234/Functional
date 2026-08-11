# F# Cheat Sheet — Operators, Syntax & Standard Library Lookup

Ctrl+F for the exact symbol/name (e.g. search `>>=` or `List.fold`). Each entry: what it does, in one line, plus a tiny example.

---

## Arithmetic, comparison & boolean operators

### `+` `-` `*` `/`
Standard add/subtract/multiply/divide. **Integer `/` truncates** (rounds toward zero) rather than giving a fractional result — a very common exam gotcha.
```fsharp
7 / 2     // 3   (int division, truncates!)
7.0 / 2.0 // 3.5 (float division)
```

### `%` (modulo / remainder)
Remainder after integer division. Used constantly for even/odd checks, cycling, divisibility.
```fsharp
7 % 2    // 1
8 % 2    // 0
x % 2 = 0   // "is x even"
```

### `**` (power, floats only)
Exponentiation — **only works on `float`**, not `int`. For integer powers, use `pown` instead.
```fsharp
2.0 ** 3.0   // 8.0
```

### `=` (equality) / `<>` (inequality)
F# uses a single `=` for comparison (not `==`), and `<>` for "not equal" (not `!=`).
```fsharp
5 = 5    // true
5 <> 4   // true
```

### `<` `>` `<=` `>=`
Standard ordering comparisons — work on numbers, strings, chars, and anything comparable (tuples, lists of comparables, etc.).
```fsharp
3 < 5      // true
"abc" < "abd"   // true (lexicographic)
```

### `&&` (and) / `||` (or) / `not`
Boolean operators. `&&` and `||` short-circuit (the right side isn't evaluated if the left side already decides the answer).
```fsharp
(x > 0) && (x < 10)   // true only if both hold
(x = 0) || (y = 0)    // true if either holds
not true               // false
```

### `abs`
Absolute value.
```fsharp
abs (-5)    // 5
abs 5       // 5
```

### `sqrt`
Square root (works on `float`; give it a `float` input, e.g. via `float n`).
```fsharp
sqrt 9.0        // 3.0
sqrt (float 9)  // 3.0
```

### `pown` (integer power)
Raises a number to an **integer** power — use this instead of `**` when you want to stay in `int` (or raise a float to an int power) without converting to float.
```fsharp
pown 2 10     // 1024   (int base, int exponent)
pown 2.0 3    // 8.0    (float base, int exponent)
```

### `min` / `max`
Smaller/larger of two values.
```fsharp
min 3 7   // 3
max 3 7   // 7
```

### `sign`
Returns `-1`, `0`, or `1` depending on whether the number is negative, zero, or positive.
```fsharp
sign (-5)   // -1
sign 0      // 0
sign 5      // 1
```

### `ceil` / `floor` / `round`
Round a float up / down / to nearest (all still return a `float` — wrap in `int` to convert). `round` uses "round half to even" (banker's rounding), which can surprise you on `.5` values.
```fsharp
ceil 2.1    // 3.0
floor 2.9   // 2.0
round 2.5   // 2.0  (rounds to even, not always "up")
```

### `int` / `float` (type conversion)
Convert between numeric types. `int` on a float **truncates** toward zero (does not round).
```fsharp
int 3.9     // 3   (truncates, doesn't round!)
float 3     // 3.0
```

### `compare`
Compares two values and returns a negative int, `0`, or a positive int — the same three-way result `List.sortBy`/`Map`/etc. rely on internally. Rarely called directly, but useful to know it exists.
```fsharp
compare 3 5   // negative
compare 5 5   // 0
compare 5 3   // positive
```

### `System.Math.PI`
The constant π, if you need it (e.g. area/circumference formulas). No `sin`/`cos`/etc. needed for these exams typically, but they exist the same way: `sin x`, `cos x`, etc. (radians, `float` in/out).
```fsharp
System.Math.PI   // 3.14159265358979...
let circleArea r = System.Math.PI * r * r
```

---

## Pipes & function composition

### `|>` (pipe forward)
Passes the value on the left as the **last** argument to the function on the right. Used constantly to read code top-to-bottom instead of inside-out.
```fsharp
[1;2;3] |> List.map (fun x -> x * 2)   // [2;4;6]
5 |> (+) 1                              // 6
```

### `<|` (pipe backward)
Same as `|>` but reversed — passes the right-hand value into the left-hand function. Mostly used to avoid parentheses.
```fsharp
printfn "%d" <| 1 + 2   // same as printfn "%d" (1 + 2)
```

### `>>` (compose forward)
Glues two functions into one: `(f >> g) x` means `g (f x)` — do `f` first, then `g`.
```fsharp
let addThenDouble = (+) 1 >> (*) 2
addThenDouble 3   // (3+1)*2 = 8
```

### `<<` (compose backward)
Like `>>` but reversed: `(f << g) x` means `f (g x)` — do `g` first, then `f`.
```fsharp
let doubleThenAdd = (+) 1 << (*) 2
doubleThenAdd 3   // (3*2)+1 = 7
```

### `ignore`
Throws away a value and returns `unit`. Used when you must call something for its side effect but don't care about its result (F# warns if you silently drop a non-unit value).
```fsharp
List.iter (fun x -> printfn "%d" x) [1;2;3] |> ignore
```

---

## Lists — literals & core operators

### `[]`
The empty list.
```fsharp
let xs : int list = []
```

### `[a; b; c]`
A list literal — elements separated by `;` (not commas!).
```fsharp
let xs = [1; 2; 3]
```

### `::` (cons)
Prepends a single element to the front of a list. The most common way to build/deconstruct lists recursively.
```fsharp
1 :: [2; 3]              // [1; 2; 3]
match xs with
| x :: rest -> ...        // x = head, rest = tail
| [] -> ...
```

### `@` (append)
Concatenates two lists. **O(n)** in the length of the left list — avoid using it in a loop (that's how you accidentally write an O(n²) function).
```fsharp
[1;2] @ [3;4]   // [1;2;3;4]
```

### `[a .. b]` (range)
A list of consecutive integers from `a` to `b` inclusive.
```fsharp
[1 .. 5]   // [1;2;3;4;5]
```

### `[for x in xs -> expr]` (list comprehension)
Builds a new list by transforming every element of an existing sequence/range. Can be nested for quadratic-style problems.
```fsharp
[for x in 1 .. 5 -> x * x]                       // [1;4;9;16;25]
[for i in 1..3 -> [for j in 1..i -> j]]           // nested version
```

---

## Pattern matching & function definitions

### `match ... with`
The core control-flow construct: compares a value against a series of patterns, top to bottom, and runs the first one that matches.
```fsharp
match x with
| 0 -> "zero"
| n when n > 0 -> "positive"
| _ -> "negative"
```

### `function`
Shorthand for `fun x -> match x with ...` — saves you naming the argument when you're immediately pattern-matching on it.
```fsharp
let rec length = function
    | [] -> 0
    | _ :: rest -> 1 + length rest
```

### `_` (wildcard)
Matches anything without binding it to a name. Use it when you don't need the value (and to avoid "unused variable" warnings).
```fsharp
match xs with
| [_] -> "exactly one element, don't care what"
| _ -> "something else"
```

### `when` (guard)
Adds an extra boolean condition to a match case — the case only fires if the pattern matches **and** the guard is true.
```fsharp
match n with
| x when x % 2 = 0 -> "even"
| _ -> "odd"
```

### `as`
Binds a name to an entire pattern while still letting you take it apart.
```fsharp
match xs with
| (first :: _) as whole -> printfn "%d is first of %A" first whole
```

### `let rec`
Defines a function that can call itself. Plain `let` cannot refer to itself.
```fsharp
let rec factorial n = if n <= 1 then 1 else n * factorial (n - 1)
```

### `fun` (lambda)
An anonymous, inline function.
```fsharp
List.map (fun x -> x + 1) [1;2;3]   // [2;3;4]
```

### `'a`, `'b` (generic type parameters)
A lowercase name starting with `'` means "any type" — the function works for every type, not just one.
```fsharp
let identity (x : 'a) : 'a = x
```

---

## Custom / course-specific operators (monad plumbing)

### `>>=` (bind, custom-defined)
Not built into F# — courses define this themselves as an alias for a monad's `bind`: "run this monadic computation, take its result, and feed it into the next step." Recognize it by the accompanying `let (>>=) a f = bind f a` definition nearby.
```fsharp
let (>>=) a f = bind f a
place_queen2 1 0 >>= (fun () -> valid_solution2)
```

### `>>>=` (sequence, discard result)
Also custom-defined, usually as `let (>>>=) a b = a >>= (fun _ -> b)` — "run this step, throw away its result, then run the next step." Used to chain a series of actions where you only care about the last one's value.
```fsharp
place_queen2 1 0 >>>= place_queen2 3 1 >>>= valid_solution2
```

### Defining your own operator
Any symbol sequence in parentheses becomes an infix operator when you `let`-bind it.
```fsharp
let (+++) a b = a + b + 1
2 +++ 3   // 6
```

---

## Option — "maybe there's a value"

### `Some` / `None`
`Some x` wraps a present value; `None` represents absence. The type is `'a option`.
```fsharp
let safeDivide a b = if b = 0 then None else Some (a / b)
```

### `Option.map`
Applies a function to the value **only if** it's `Some`; leaves `None` alone.
```fsharp
Option.map ((+) 1) (Some 5)   // Some 6
Option.map ((+) 1) None       // None
```

### `Option.bind`
Like `Option.map`, but the function itself returns an option (so results don't get doubly-wrapped as `Some (Some x)`). Used for chaining operations that can each fail.
```fsharp
Some 4 |> Option.bind (fun x -> if x > 0 then Some (sqrt (float x)) else None)
```

### `Option.isSome` / `Option.isNone`
Check which case you have, as a bool.
```fsharp
Option.isSome (Some 3)   // true
```

### `Option.get`
Unwraps `Some x` to `x`. **Crashes** if given `None` — only use when you're certain it's `Some`.
```fsharp
Option.get (Some 3)   // 3
```

### `Option.defaultValue`
Unwraps an option, substituting a fallback if it's `None`.
```fsharp
Option.defaultValue 0 None       // 0
Option.defaultValue 0 (Some 7)   // 7
```

---

## Result — "success value, or an error"

### `Ok` / `Error`
`Ok x` represents success with value `x`; `Error e` represents failure with reason `e`. The type is `Result<'a, 'error>`.
```fsharp
let parse s = if s = "" then Error "empty" else Ok (int s)
```

### `Result.map`
Applies a function to the value only if it's `Ok`; passes `Error` straight through.
```fsharp
Result.map ((+) 1) (Ok 5)        // Ok 6
Result.map ((+) 1) (Error "no")  // Error "no"
```

### `Result.bind`
Like `Result.map`, but for chaining functions that themselves return a `Result` — this is how you sequence a series of steps that can each fail, short-circuiting at the first `Error`.
```fsharp
Ok 4 |> Result.bind (fun x -> if x > 0 then Ok (x * 2) else Error "not positive")
```

### `Result.toOption`
Converts `Ok x` to `Some x` and `Error _` to `None` (throwing away the error detail).
```fsharp
Result.toOption (Ok 5)      // Some 5
Result.toOption (Error "x") // None
```

---

## List module — the most-used functions

### `List.map`
Applies a function to every element, returning a new list of the same length.
```fsharp
List.map (fun x -> x * 2) [1;2;3]   // [2;4;6]
```

### `List.filter`
Keeps only the elements for which a predicate returns true.
```fsharp
List.filter (fun x -> x % 2 = 0) [1;2;3;4]   // [2;4]
```

### `List.fold`
Walks the list left-to-right, combining elements into a single accumulated result using a function you supply. General-purpose "reduce to one value."
```fsharp
List.fold (+) 0 [1;2;3;4]        // 10
List.fold (fun acc x -> x :: acc) [] [1;2;3]   // [3;2;1] (reverses!)
```

### `List.foldBack`
Like `List.fold`, but walks right-to-left (starts from the last element). Signature has the list before the seed.
```fsharp
List.foldBack (fun x acc -> x :: acc) [1;2;3] []   // [1;2;3] (rebuilds, doesn't reverse)
```

### `List.collect`
Like `List.map`, but each element is turned into a **list**, and all the resulting lists get flattened together (`map` + `concat` in one step).
```fsharp
List.collect (fun x -> [x; x]) [1;2;3]   // [1;1;2;2;3;3]
```

### `List.iter`
Runs a function on every element purely for its **side effect** (like printing) — returns `unit`, doesn't build a new list.
```fsharp
List.iter (fun x -> printfn "%d" x) [1;2;3]
```

### `List.exists`
True if **any** element satisfies the predicate.
```fsharp
List.exists (fun x -> x > 2) [1;2;3]   // true
```

### `List.forall`
True if **every** element satisfies the predicate (vacuously true for `[]`).
```fsharp
List.forall (fun x -> x > 0) [1;2;3]   // true
```

### `List.head` / `List.tail`
First element / everything except the first element. Both **crash** on `[]`.
```fsharp
List.head [1;2;3]   // 1
List.tail [1;2;3]   // [2;3]
```

### `List.rev`
Reverses a list.
```fsharp
List.rev [1;2;3]   // [3;2;1]
```

### `List.length`
Number of elements. **O(n)** — don't call it repeatedly in a loop if you can track a count instead.
```fsharp
List.length [1;2;3]   // 3
```

### `List.sum`
Adds up all elements (they must be a numeric type).
```fsharp
List.sum [1;2;3;4]   // 10
```

### `List.concat`
Flattens a list of lists into one list.
```fsharp
List.concat [[1;2]; [3]; [4;5]]   // [1;2;3;4;5]
```

### `List.choose`
Like `List.map` with a function returning `'a option`, but keeps only the `Some` results, unwrapped (`map` + `filter` + unwrap in one step).
```fsharp
List.choose (fun x -> if x > 2 then Some (x*10) else None) [1;2;3;4]   // [30;40]
```

### `List.partition`
Splits a list into two lists — a tuple of (elements satisfying the predicate, elements that don't).
```fsharp
List.partition (fun x -> x % 2 = 0) [1;2;3;4]   // ([2;4], [1;3])
```

### `List.sortBy` / `List.sort`
`sortBy f` sorts using `f element` as the comparison key; plain `sort` sorts elements directly (must be comparable).
```fsharp
List.sortBy (fun (_, age) -> age) [("Bob",30); ("Amy",20)]   // [("Amy",20); ("Bob",30)]
List.sort [3;1;2]   // [1;2;3]
```

### `List.pairwise`
Turns a list into the list of all overlapping consecutive pairs — this is literally what "foo" was hand-rolling in both exams' Q2!
```fsharp
List.pairwise [1;2;3;4]   // [(1,2); (2,3); (3,4)]
```

### `List.chunkBySize`
Splits a list into consecutive sub-lists ("chunks") of (at most) the given size.
```fsharp
List.chunkBySize 2 [1;2;3;4;5]   // [[1;2]; [3;4]; [5]]
```

### `List.zip`
Combines two equal-length lists element-by-element into a list of tuples.
```fsharp
List.zip [1;2;3] ["a";"b";"c"]   // [(1,"a"); (2,"b"); (3,"c")]
```

### `List.take` / `List.skip`
`take n` keeps the first `n` elements; `skip n` drops the first `n` elements. Both crash if `n` exceeds the list length.
```fsharp
List.take 2 [1;2;3;4]   // [1;2]
List.skip 2 [1;2;3;4]   // [3;4]
```

### `List.tryFind` / `List.find`
Returns the first element satisfying a predicate. `tryFind` gives `'a option` (safe); `find` gives `'a` directly and **crashes** if nothing matches.
```fsharp
List.tryFind (fun x -> x > 2) [1;2;3]   // Some 3
List.find (fun x -> x > 2) [1;2;3]      // 3
```

### `List.init`
Builds a list of a given length by calling a function on each index `0 .. n-1`.
```fsharp
List.init 5 (fun i -> i * i)   // [0;1;4;9;16]
```

### `List.last`
The final element. **O(n)** and crashes on `[]` — repeatedly calling this in a loop is a common accidental-quadratic bug.
```fsharp
List.last [1;2;3]   // 3
```

### `List.distinct`
Removes duplicate elements, keeping first occurrence order.
```fsharp
List.distinct [1;2;2;3;1]   // [1;2;3]
```

### `List.reduce`
Like `List.fold`, but uses the **first element as the seed** instead of a separate initial value — no need to supply a starting accumulator. **Crashes on `[]`** (unlike `fold`, which handles the empty list fine since it has an explicit seed).
```fsharp
List.reduce (+) [1;2;3;4]   // 10
List.reduce (fun a b -> if a > b then a else b) [3;7;2]   // 7
```

### `List.mapi` / `List.iteri`
Like `List.map`/`List.iter`, but the function also receives the element's **index** as the first argument.
```fsharp
List.mapi (fun i x -> (i, x)) ["a";"b";"c"]   // [(0,"a"); (1,"b"); (2,"c")]
List.iteri (fun i x -> printfn "%d: %s" i x) ["a";"b"]
```

### `List.groupBy`
Groups elements into `(key, elements)` pairs, based on a key function — all elements sharing a key end up together in one list.
```fsharp
List.groupBy (fun x -> x % 2) [1;2;3;4;5]
// [(1, [1;3;5]); (0, [2;4])]
```

### `List.append`
Function form of `@` — concatenates two lists. Same O(n)-in-left-list-length cost as `@`.
```fsharp
List.append [1;2] [3;4]   // [1;2;3;4]
```

### `List.indexed`
Pairs every element with its index, as a list of tuples — an alternative to `mapi` when you just want the pairs, not a transformation.
```fsharp
List.indexed ["a";"b";"c"]   // [(0,"a"); (1,"b"); (2,"c")]
```

### `List.contains`
True if the given value appears anywhere in the list (equality check element by element).
```fsharp
List.contains 3 [1;2;3]   // true
List.contains 9 [1;2;3]   // false
```

### `List.max` / `List.min` / `List.maxBy` / `List.minBy`
`max`/`min` return the largest/smallest element directly (must be comparable); `maxBy`/`minBy` pick the element for which a **key function** is largest/smallest. All **crash on `[]`**.
```fsharp
List.max [3;1;4;1;5]   // 5
List.min [3;1;4;1;5]   // 1
List.maxBy (fun (_, age) -> age) [("Bob",30); ("Amy",20)]   // ("Bob",30)
List.minBy String.length ["ccc";"a";"bb"]   // "a"
```

### `List.replicate`
Builds a list containing the same value repeated `n` times.
```fsharp
List.replicate 3 "x"   // ["x";"x";"x"]
```

### `List.splitAt`
Splits a list into two lists at a given index — returns a tuple `(before, from-index-onward)`. Different from `take`/`skip`, which each give you only one side.
```fsharp
List.splitAt 2 [1;2;3;4]   // ([1;2], [3;4])
```

### `List.unzip`
The inverse of `List.zip` — splits a list of tuples into a tuple of two lists.
```fsharp
List.unzip [(1,"a"); (2,"b"); (3,"c")]   // ([1;2;3], ["a";"b";"c"])
```

### `List.isEmpty`
True if the list has no elements — a readable alternative to `xs = []`.
```fsharp
List.isEmpty []        // true
List.isEmpty [1;2;3]   // false
```

---

## Seq module — lazy sequences (for infinite generation)

### `seq { }`
Builds a lazy sequence using imperative-looking `yield`/loops — elements are computed on demand, not up front.
```fsharp
seq { for i in 1 .. 5 -> i * i }
```

### `Seq.unfold`
Builds a lazy, potentially **infinite** sequence from a seed and a step function `state -> (element, nextState) option`; returning `None` stops it. This is the standard tool for "generate an infinite sequence without recomputing."
```fsharp
Seq.unfold (fun n -> Some (n, n + 1)) 0   // 0, 1, 2, 3, ... forever
```

### `Seq.take`
Pulls the first `n` elements out of a (possibly infinite) sequence, forcing evaluation of just those.
```fsharp
Seq.unfold (fun n -> Some (n, n+1)) 0 |> Seq.take 5 |> Seq.toList   // [0;1;2;3;4]
```

### `Seq.initInfinite`
Builds an infinite sequence by calling a function on every index `0, 1, 2, ...`. Simpler than `unfold` but **recomputes from scratch** each time rather than carrying state forward — use `unfold` instead when a question explicitly asks you to avoid recomputation.
```fsharp
Seq.initInfinite (fun i -> i * i) |> Seq.take 3 |> Seq.toList   // [0;1;4]
```

---

## Records, discriminated unions & Map

### Record type — `{ field : type; ... }`
A named bundle of fields, each with a type. Construct with `{ field = value; ... }`.
```fsharp
type person = { name : string; age : int }
let bob = { name = "Bob"; age = 30 }
bob.age   // 30
```

### `{ x with field = value }` (record update)
Creates a **new** record copying every field from `x` except the ones you override — the original is untouched (records are immutable).
```fsharp
let olderBob = { bob with age = 31 }
```

### Discriminated union — `type X = A | B of int`
A type that is exactly one of several named "cases," some of which can carry data. Pattern-match to tell them apart.
```fsharp
type shape = Circle of float | Square of float | Point
let area = function
    | Circle r -> 3.14159 * r * r
    | Square s -> s * s
    | Point -> 0.0
```

### `Map.empty`
An empty immutable dictionary/lookup table.
```fsharp
let m : Map<string, int> = Map.empty
```

### `Map.add`
Returns a **new** map with a key added or overwritten (original map unchanged).
```fsharp
Map.add "x" 1 Map.empty   // map [("x", 1)]
```

### `Map.tryFind`
Looks up a key, safely: `Some value` if present, `None` if not.
```fsharp
Map.tryFind "x" (Map.add "x" 1 Map.empty)   // Some 1
```

### `Map.containsKey`
True/false, whether a key is present.
```fsharp
Map.containsKey "x" Map.empty   // false
```

### `Map.count`
Number of key/value pairs in the map.
```fsharp
Map.count (Map.add "x" 1 Map.empty)   // 1
```

---

## Printing / formatting

### `printfn "%d" x`
Prints a formatted string followed by a newline. Common format specifiers: `%d` int, `%s` string, `%f` float, `%b` bool, `%A` "print anything" (records, DUs, lists — great default when unsure), `%c` char.
```fsharp
printfn "Hello %s, you are %d years old" "Bob" 30
```

### `sprintf "%d" x`
Same formatting as `printfn`, but **returns a string** instead of printing it.
```fsharp
let msg = sprintf "Result: %d" 42
```

### `string x`
Converts (almost) any value to its default string representation. Simpler than `sprintf "%A"` for single values.
```fsharp
string 42   // "42"
```

---

## Async & MailboxProcessor (concurrency)

### `async { }`
Defines an asynchronous computation block — code inside can use `let!`/`do!`/`return!` to work with other async values without blocking a thread while waiting.
```fsharp
let work = async { printfn "hi"; return 42 }
```

### `let!`
Inside an `async`/computation-expression block: runs an async computation and binds its **result** to a name once it completes (the "await" of F#).
```fsharp
async {
    let! x = someAsyncThing
    return x + 1
}
```

### `do!`
Like `let!` but for computations whose result you don't care about (typically `unit`) — just "do this step, then continue."
```fsharp
async {
    do! Async.Sleep 1000
    printfn "done waiting"
}
```

### `return` / `return!`
`return x` wraps a plain value as the block's result. `return! m` returns an **already-wrapped** computation directly (no double-wrapping) — used when the last thing you do is itself another async/monadic call.
```fsharp
async { return 5 }              // wraps 5
async { return! someAsyncThing } // hands back someAsyncThing's result directly
```

### `Async.RunSynchronously`
Blocks the current thread until an async computation finishes, and returns its result. Typically the outermost call that kicks everything off.
```fsharp
Async.RunSynchronously (async { return 42 })   // 42
```

### `Async.Parallel`
Takes a sequence of async computations and runs them **concurrently**, giving back one async that completes (with an array of all results) once every one of them is done.
```fsharp
[1;2;3]
|> List.map (fun x -> async { return x * 2 })
|> Async.Parallel
|> Async.RunSynchronously   // [|2;4;6|]
```

### `MailboxProcessor.Start`
Spawns a new mailbox (actor) running the given `mbox -> Async<unit>` loop function, and returns a handle you can `.Post`/`.PostAndReply` messages to.
```fsharp
let agent = MailboxProcessor.Start(fun mbox -> async {
    let! msg = mbox.Receive()
    printfn "got %A" msg
})
```

### `mbox.Receive()`
Inside a mailbox loop: asynchronously waits for and returns the next queued message.
```fsharp
let! msg = mbox.Receive()
```

### `mbox.Post`
Fire-and-forget: sends a message to the mailbox and returns immediately, without waiting for any reply.
```fsharp
agent.Post(SomeMessage)
```

### `mbox.PostAndReply`
Sends a message (built with a reply-channel function) and **blocks** until the mailbox replies — used when the caller needs the mailbox's answer before continuing.
```fsharp
agent.PostAndReply(fun replyChannel -> Read replyChannel)
```

---

## Parser combinators (course-specific "JParsec"-style library)

These names vary by course/library — confirm the exact names in your own project, but the concepts and shapes below are standard.

### `|>>` (map a parser's result)
Runs a parser, then applies an ordinary function to whatever it successfully parsed.
```fsharp
pstring "X" |>> (fun _ -> SomeValue)
```

### `.>>.` (sequence, keep both results)
Runs two parsers one after another, succeeding only if both do, and returns **both** results as a tuple.
```fsharp
pstring "row" .>>. pint   // parses "row42" -> ("row", 42)
```

### `.>>` (sequence, keep left result)
Runs two parsers in sequence but only keeps the **first** one's result (useful for consuming-but-discarding a separator/keyword).
```fsharp
pint .>> pstring ";"   // parses "5;" -> 5
```

### `>>.` (sequence, keep right result)
Same as above but keeps the **second** result instead.
```fsharp
pstring "Player " >>. pint   // parses "Player 5" -> 5
```

### `<|>` (alternative / "try this, or that")
Tries the left parser; if it fails, tries the right one instead.
```fsharp
pstring "X" <|> pstring "O"
```

### `many`
Applies a parser repeatedly (zero or more times) and collects all the results into a list; stops (without failing) as soon as the parser stops matching.
```fsharp
many (pstring "a")   // parses "aaab" -> ["a";"a";"a"], leaving "b" unconsumed
```

### `run`
Actually executes a parser on an input string and gives back a `Result<'a, string>` — the entry point you call to test/use a parser.
```fsharp
run parseMove "Player X places a tile on row midRow and column midCol"
```
