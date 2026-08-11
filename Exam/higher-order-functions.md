# F# Recursion → Higher-Order Function Cheat Sheet

## The method (do this every time)

1. **Read the recursive function like a story.** What happens at the base case (`[]`)? What happens at `x :: xs`?
2. **Describe the recursive case in one sentence:** "combine (something with x) with (result on xs)."
3. **Match the sentence against the shapes below.**
4. **Swap in the matching HOF**, using the base case as the initial/accumulator value.

If you can eyeball "this recurses by combining head with tail-result" and instantly think "fold," you're basically done.

---

## Shape 1: `List.map`

**Recursive pattern:**
```fsharp
let rec aux = function
    | [] -> []
    | x :: xs -> f x :: aux xs
```
Each element is transformed independently; order/structure of the list is preserved (still a list, same length).

**HOF version:**
```fsharp
let aux = List.map f
```

**Recognize by:** `f x :: aux xs` — cons of "transformed head" onto "processed tail."

---

## Shape 2: `List.foldBack` (right fold)

**Recursive pattern:**
```fsharp
let rec aux = function
    | [] -> init
    | x :: xs -> combine x (aux xs)
```
The recursive call `aux xs` appears **on the right** of the combine — i.e., you need the result of the rest of the list *before* you can finish combining.

**HOF version:**
```fsharp
let aux list = List.foldBack combine list init
```

**Recognize by:** `combine x (aux xs)` — recursive call is an *argument*, sitting on the right.

**Example (from baz):**
```fsharp
let rec aux = function
    | [] -> ""
    | c :: cs -> string (foo c) + (aux cs)

// becomes:
let aux str = List.foldBack (fun c acc -> string (foo c) + acc) str ""
```

---

## Shape 3: `List.fold` (left fold, accumulator-style)

**Recursive pattern (accumulator threaded through, tail-recursive style):**
```fsharp
let rec aux acc = function
    | [] -> acc
    | x :: xs -> aux (combine acc x) xs
```
Here the accumulator is updated *first*, then passed forward — recursive call is not "wrapped around" anything.

**HOF version:**
```fsharp
let aux list = List.fold combine init list
```

**Recognize by:** the recursive call is the **entire result** (tail call), with an already-updated accumulator passed in — nothing wraps around `aux xs`.

**map vs foldBack vs fold — quick test:**
| Pattern | HOF |
|---|---|
| `f x :: aux xs` | `List.map f` |
| `combine x (aux xs)` (call wrapped inside) | `List.foldBack` |
| `aux (combine acc x) xs` (call is the whole tail, acc updated before) | `List.fold` |

---

## Shape 4: `List.filter`

**Recursive pattern:**
```fsharp
let rec aux = function
    | [] -> []
    | x :: xs when pred x -> x :: aux xs
    | x :: xs -> aux xs
```
Elements either kept as-is or dropped based on a condition.

**HOF version:**
```fsharp
let aux = List.filter pred
```

---

## Shape 5: map + filter combined (choose)

**Recursive pattern:**
```fsharp
let rec aux = function
    | [] -> []
    | x :: xs when pred x -> f x :: aux xs
    | x :: xs -> aux xs
```
Elements are both filtered AND transformed.

**HOF version:**
```fsharp
let aux = List.choose (fun x -> if pred x then Some (f x) else None)
```

---

## Shape 6: map + concatenate to a string

If you're transforming characters/elements and gluing the results into one string (common in these string-processing exercises):

```fsharp
let aux str =
    str
    |> List.map f
    |> List.map string   // if f returns e.g. char, and you need string
    |> String.concat ""
```
This is often equivalent to (and simpler than) a `foldBack` with `+`.

---

## Shape 7: `List.exists` / `List.forall`

**Recursive pattern (exists):**
```fsharp
let rec aux = function
    | [] -> false
    | x :: xs -> pred x || aux xs
```
**HOF version:** `List.exists pred`

**Recursive pattern (forall):**
```fsharp
let rec aux = function
    | [] -> true
    | x :: xs -> pred x && aux xs
```
**HOF version:** `List.forall pred`

---

## Worked example (baz → baz2)

```fsharp
let foo = function
    | c when Char.IsWhiteSpace c -> c
    | c when c > 'w' -> char (int c - 23)
    | c when c < 'x' -> char (int c + 3)

let bar (str : string) = [for c in str -> c]

let baz str =
    let rec aux = function
        | [] -> ""
        | c :: cs -> string (foo c) + (aux cs)
    aux (bar str)
```

**Reasoning:**
- Base case `[]` → `""`
- Recursive case: `string (foo c) + (aux cs)` → recursive call wrapped on the right → **foldBack shape**

**baz2 (foldBack version):**
```fsharp
let baz2 str =
    List.foldBack (fun c acc -> string (foo c) + acc) (bar str) ""
```

**baz2 (map + concat version — often cleaner, also valid):**
```fsharp
let baz2 str =
    bar str
    |> List.map foo
    |> List.map string
    |> String.concat ""
```

Both are non-recursive and built from higher-order functions — either is a correct answer unless the question specifies which HOF to use.

---

## Checklist for exam

- [ ] Write out the base case and recursive case in plain English first.
- [ ] Is the recursive call **wrapped inside** an operation? → `foldBack`
- [ ] Is the recursive call the **entire result**, with accumulator pre-updated? → `fold`
- [ ] Is each element just transformed 1-to-1, same length list out? → `map`
- [ ] Are elements being dropped based on a condition? → `filter`
- [ ] Both transformed AND dropped? → `choose`
- [ ] Boolean short-circuit over the list? → `exists` / `forall`
- [ ] String-building from list of chars? → `map` + `String.concat ""` (or `foldBack` with `+`)

---

## ⚡ Don't-think-just-do version

If you're panicking and don't have time to reason about it, **almost every one of these questions is a `foldBack` in disguise.** Here's the copy-paste mechanical recipe:

1. Find the recursive function's two cases: `[] -> BASE` and `x :: xs -> STUFF`.
2. Write this template, no thinking required:
   ```fsharp
   let aux2 list = List.foldBack (fun x acc -> STUFF_BUT_REPLACE_(aux xs)_WITH_acc) list BASE
   ```
3. Literally take the `STUFF` line from the original, and everywhere you see `(aux xs)` (or `(aux cs)`, whatever it's called), replace it with `acc`. That's your lambda body. Done.

**Mechanical example:**
Original recursive case:
```fsharp
c :: cs -> string (foo c) + (aux cs)
```
→ replace `(aux cs)` with `acc`, `c` becomes the lambda's first arg:
```fsharp
fun c acc -> string (foo c) + acc
```
→ base case `""` slots in as the last argument:
```fsharp
let baz2 str = List.foldBack (fun c acc -> string (foo c) + acc) (bar str) ""
```

**That's the whole trick.** `foldBack` is *literally* "your recursive function, pre-written for you" — `x` is the head, `acc` stands in for "whatever `aux` would've returned on the rest of the list," and the base case is the last argument. You almost never need to derive this from first principles — just do the find-and-replace.

**If `foldBack` doesn't look right** (e.g. it's clearly just transforming each element with nothing extra, like `f x :: aux xs`) — that's the *only* other common case, and it's just `List.map f`. Everything else in this doc is for edge cases; 90% of exam questions are one of these two.
