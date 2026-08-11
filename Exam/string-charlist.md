# F# String ↔ Char List Conversions

## Part 1: `string` → `char list`

### 1. Normal / idiomatic way — `List.ofSeq`
```fsharp
let chars = "hello" |> List.ofSeq
// ['h'; 'e'; 'l'; 'l'; 'o']
```
Strings implement `IEnumerable<char>`, so this works directly.

### 2. Piped `Seq.toList`
```fsharp
let chars = "hello" |> Seq.toList
// ['h'; 'e'; 'l'; 'l'; 'o']
```
Functionally identical to `List.ofSeq`, just from the `Seq` module. This is the most common idiom you'll see.

### 3. Via `ToCharArray` + `Array.toList`
```fsharp
let chars = "hello".ToCharArray() |> Array.toList
// ['h'; 'e'; 'l'; 'l'; 'o']
```
Uses the .NET method first, then converts the array to a list.

### 4. Using `List.ofArray` (explicit, no pipe)
```fsharp
let chars = List.ofArray ("hello".ToCharArray())
```
Same idea as #3, written without the pipe operator.

### 5. Building it yourself with `fold`
```fsharp
let chars =
    "hello"
    |> Seq.fold (fun acc c -> c :: acc) []
    |> List.rev
// ['h'; 'e'; 'l'; 'l'; 'o']
```
You fold over the string prepending each char (cheap, O(1) cons), then reverse at the end since folding left-to-right naturally builds the list backwards.

### 6. Recursive version (manual, for learning/interview purposes)
```fsharp
let rec toCharList (s: string) =
    if s.Length = 0 then []
    else s.[0] :: toCharList (s.Substring(1))
```
Not idiomatic (allocates lots of substrings) but shows the underlying recursive structure explicitly.

**Which to actually use:** `Seq.toList` or `List.ofSeq` — they're equivalent, O(n), and the standard idiom. Use `fold` if you want to demonstrate/practice fold mechanics rather than for production code.

---

## Part 2: `char list` → `string`

### 1. Normal / idiomatic way — `System.String(Array)`
```fsharp
let s = System.String(chars |> Array.ofList)
// "hello"
```
The `String` constructor takes a `char[]`, so you convert the list to an array first.

### 2. Piped `List.toArray` + `String`
```fsharp
let s = chars |> List.toArray |> System.String
// "hello"
```
Same idea as #1, but written pipe-style with `List.toArray`.

### 3. Using `String.Concat`
```fsharp
let s = System.String.Concat(chars)
// "hello"
```
`String.Concat` has an overload that accepts a sequence of chars directly, so no explicit array conversion is needed.

### 4. Using `List.map string` + `String.concat`
```fsharp
let s = chars |> List.map string |> String.concat ""
// "hello"
```
Converts each char to a one-character string, then joins them all with an empty separator. Less efficient, but a common functional-style pattern.

### 5. Building it yourself with `fold`
```fsharp
let s =
    chars
    |> List.fold (fun acc c -> acc + string c) ""
// "hello"
```
Accumulates a string one char at a time. Simple to read, but O(n²) due to repeated string concatenation — fine for small lists, not great for large ones.

### 6. Recursive version (manual, for learning/interview purposes)
```fsharp
let rec toString (cs: char list) =
    match cs with
    | [] -> ""
    | c :: rest -> string c + toString rest
```
Shows the underlying recursion explicitly; same O(n²) concatenation caveat as #5.

**Which to actually use:** `System.String(List.toArray chars)` or `String.Concat(chars)` — both are O(n) and idiomatic. Avoid the `fold`/recursive `+`-concatenation versions for anything performance-sensitive.
