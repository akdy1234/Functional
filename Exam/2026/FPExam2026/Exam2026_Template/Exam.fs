module Exam2026_Template.Exam

    open JParsec.TextParser

    (* Question 1: Parametric Lucas numbers (25%) *)

    (* Question 1.1
       Direct translation of the recurrence in the problem text:
         L(0) = 0
         L(1) = 1
         L(n) = k * L(n-1) + L(n-2)
       This is recursive but NOT tail-recursive: the result of the recursive
       calls is used in an addition/multiplication AFTER the calls return,
       so F# cannot turn this into a loop. That's fine here, the question
       explicitly does not ask for performance. *)
    let rec lucas_number (k : int) (n : int) : int =
        if n = 0 then 0
        elif n = 1 then 1
        else k * lucas_number k (n - 1) + lucas_number k (n - 2)

    (* Question 1.2
       Tail-recursive, linear-time version using an accumulator pair (a, b)
       that always holds two consecutive Lucas numbers (a = L(n-i), b = L(n-i+1)).
       Each step slides the window forward by one and decreases the counter,
       so the call to 'aux' is the very last thing that happens - that's what
       makes it tail-recursive (the compiler can reuse the stack frame). *)
    let lucas_number_acc (k : int) (n : int) : int =
        let rec aux n a b =
            if n = 0 then a
            else aux (n - 1) b (k * b + a)
        aux n 0 1

    (* Question 1.3
       sqrt(k^2 + 4) ~= 2 * (L(n) / L(n-1)) - k
       Straight translation of the formula given in the problem text. *)
    let sqrt_approx (k : int) (n : int) : float =
        let numerator = float (lucas_number_acc k n)
        let denominator = float (lucas_number_acc k (n - 1))
        2.0 * (numerator / denominator) - float k

    (* Question 1.4
       Smallest n > 1 such that the approximation is within epsilon of the
       real square root. We compute the real value once, then walk n upward
       (starting at 2, since the question demands n > 1) until we're close
       enough. *)
    let approx_steps_needed (k : int) (epsilon : float) : int =
        let actual = sqrt (float (k * k + 4))
        let rec aux n =
            if abs (actual - sqrt_approx k n) < epsilon then n
            else aux (n + 1)
        aux 2

    (* Question 1.5
       Seq.unfold generates a lazy sequence from a "state" and a function that,
       given the state, returns (next element, next state) or None to stop.
       Our state is the pair of the last two Lucas numbers, so every new
       element is produced in O(1) from the state - nothing is recomputed,
       and because seq is lazy nothing is generated until it's asked for
       (e.g. by Seq.take). *)
    let lucas_seq (k : int) : seq<int> =
        Seq.unfold (fun (a, b) -> Some (a, (b, k * b + a))) (0, 1)

    (* Question 2: Code comprehension (25%) *)


    (* Question 2.1

     Q: What do the functions `foo`, `bar`, and `baz` do? Focus on what they do rather than how they do it.
     A: `foo x a` searches upward from `a` for the smallest number that divides `x` evenly (it stops either
        because it reached `x` itself, which always divides `x`, or because it found a proper divisor first).
        `bar x` uses `foo x 2` to find the smallest divisor of `x` that is >= 2. Because the smallest divisor
        (>1) of any integer is always prime, `bar x` actually computes the smallest PRIME factor of `x`
        (with 0 and 1 handled as special base cases that just return themselves).
        `baz x` repeatedly pulls out the smallest prime factor of `x` (via `bar`) and divides it out, collecting
        the factors into a list until nothing is left to factor. So `baz x` computes the full prime
        factorization of `x`, in ascending order.

     Q: What would be appropriate names for functions `foo`, `bar`, and `baz`.
     A: foo  -> smallestDivisorFrom   (or: findDivisorFrom)
        bar  -> smallestPrimeFactor
        baz  -> primeFactors          (or: factorize)

     Q: For these functions to behave meaningfully, we must place a restriction on the input values. What restriction?
     A: x must be a positive integer, i.e. x >= 1. If x <= 0, "smallest divisor" doesn't make sense (x % a is
        either always non-zero, causing foo to loop forever hunting for a divisor that is never reached, or the
        modulo behaves in a way that is not meaningful for factorization), and prime factorization itself is only
        defined for positive integers. (0 and 1 are handled explicitly by bar as edge cases, but foo itself
        implicitly assumes it is called with an x >= 2 and a starting point a <= x that will eventually be
        reached, i.e. x >= 2.)
    *)


    






    (* Question 2.2
       foo's first branch ("x = a -> a") is redundant: whenever a has climbed all the way up to x, x % a is
       x % x, which is always 0, so the second branch ("x % a = 0 -> a") already catches that case too.
       We can therefore drop the first guard entirely and get a simpler function with identical behaviour
       on every input. *)
    let rec foo2 (x : int) (a : int) : int =
        if x % a = 0 then a
        else foo2 x (a + 1)

    (* Question 2.3
       baz x factors x into a list of primes whose product is x (that's what factorization means), so
       to invert it we simply multiply the factors back together. This also happens to handle the edge
       cases baz produces for x = 0 and x = 1 correctly: product of [0] is 0, product of [1] is 1. *)
    let baz_inverse (factors : int list) : int =
        List.fold (*) 1 factors

    (* Question 2.4

      Q: One of the functions from Question 2.1 is not tail recursive.
      Explain which one and why. To make a compelling argument you must evaluate
      a function call of the function, similarly to what is done in
      Chapter 1.4 of HR, and reason about that evaluation. You need to make clear
      what aspects of the evaluation tell you that the function is not tail recursive.
      Keep in mind that all steps in an evaluation chain must evaluate to the
      same value (```(5 + 4) * 3 --> 9 * 3 --> 27```, for instance).

      A: `baz` is the function that is not tail recursive (`foo` is: its recursive call `foo x (a + 1)` is the
      entire result of that branch, nothing is done to it afterwards, so it compiles to a loop).

      Consider evaluating `baz 12`:

        baz 12
        --> (bar 12 = 2, and 12 <> 2, so we take the second branch)
            2 :: (baz (12 / 2))
        --> 2 :: (baz 6)
        --> (bar 6 = 2, and 6 <> 2)
            2 :: (2 :: (baz (6 / 2)))
        --> 2 :: (2 :: (baz 3))
        --> (bar 3 = 3, and 3 = 3, base case)
            2 :: (2 :: [3])
        --> 2 :: [2; 3]
        --> [2; 2; 3]

      The key thing to notice is the shape of the intermediate expressions: at every step, the pending
      operation is "y :: (baz ...)" - the cons `::` is NOT part of the recursive call, it is an operation that
      wraps AROUND the recursive call and can only be performed once that call has returned a value. This means
      the evaluator must remember ("hold open") every one of those `::` operations while it dives into the next
      recursive call, e.g. it cannot forget that it still owes a "2 ::" and another "2 ::" until it finally hits
      the base case `[3]` and can start applying them from the inside out. That growing chain of pending
      operations is exactly what a call stack has to keep track of, and it is precisely the property that a
      tail call does NOT have (a tail call's result IS the function's result, with nothing left to do
      afterwards). Because `baz`'s recursive call is always used as an argument to `::` rather than being
      returned directly, `baz` is not tail recursive.
    *)

    (* Question 2.5
       We turn baz into a tail-recursive function using continuation-passing style: instead of doing "y :: (rest)"
       AFTER the recursive call returns, we pass a continuation function 'c' that describes "what still needs to
       happen to the eventual answer", and each recursive call becomes the very last thing we do (a true tail
       call), just with a bigger continuation each time. *)
    let cont (x : int) : int list =
        let rec bazC x c =
            match bar x with
            | y when x = y -> c [ y ]
            | y -> bazC (x / y) (fun acc -> c (y :: acc))
        bazC x id

    (* Question 3: The robbers language (25%) *)

    let explode (str : string) = [for c in str -> c]

    let implode (cs : char list) = cs |> Array.ofList |> System.String

    let isConsonant (c : char) = "bcdfghjklmnpqrstvwxz".IndexOf(System.Char.ToLower c) >= 0

    (* Question 3.1
       Not higher-order: a plain recursive function over the character list. For each character: if it's a
       consonant, output "c" + "o" + "c"; otherwise output just the character. We work on the char list (rather
       than re-imploding a substring on every recursive call) purely so the code reads simply and directly
       mirrors the character-by-character description in the problem text. *)
    let encode (str : string) : string =
        let rec aux (chars : char list) : char list =
            match chars with
            | [] -> []
            | c :: rest ->
                if isConsonant c then
                    c :: 'o' :: c :: aux rest
                else
                    c :: aux rest
        str |> explode |> aux |> implode

    (* Question 3.2
       Also not higher-order. We walk the character list looking three at a time: if we see the pattern
       "consonant, 'o', same consonant again" that is exactly an encoded consonant, so we output the consonant
       once and skip past all three characters. Otherwise we just output the current character unchanged and
       move on by one. *)
    let decode (str : string) : string =
        let rec aux (chars : char list) : char list =
            match chars with
            | c :: 'o' :: c2 :: rest when isConsonant c && c = c2 ->
                c :: aux rest
            | c :: rest ->
                c :: aux rest
            | [] -> []
        str |> explode |> aux |> implode

    (* Question 3.3
       encode_fun is higher-order: it takes a per-character encoding function f and applies it to every
       character, concatenating the results. *)
    let encode_fun (f : char -> string) (str : string) : string =
        str |> explode |> List.map f |> String.concat ""

    (* encode2 reproduces encode's behaviour by handing encode_fun the "encode one character" rule as a
       lambda - it does no recursion of its own. *)
    let encode2 (str : string) : string =
        let encodeChar (c : char) : string =
            if isConsonant c then
                let cs = string c
                cs + "o" + cs
            else
                string c
        encode_fun encodeChar str

    (* Question 3.4
       A parser that reads characters one at a time (like `encode`, but expressed with parser combinators
       instead of explicit recursion) and produces the encoded string. We map each consumed character to
       its (possibly doubled) encoding and glue the pieces together with `many` + string concatenation.
       NOTE: the exact combinator names (anyChar / many / |>>) depend on your course's JParsec library -
       check your project's Parser module for the precise names/signatures before the real exam and adjust
       if needed; the approach below is the standard "map each char, then repeat, then join" shape used for
       this kind of character-level parser. *)
    let parser_robbers_language : Parser<string> =
        let encodeOneChar =
            anyChar
            |>> fun c ->
                    if isConsonant c then string c + "o" + string c
                    else string c
        many encodeOneChar |>> String.concat ""

    (* Question 3.5
       Split into words, hand out (roughly) evenly sized chunks of words to `num` parallel tasks, let each
       task encode its own words with the ordinary sequential `encode` from Q3.1, then glue the per-task
       results back together with spaces. We are not allowed to mutate anything - we only ever read from the
       word array via .Split, everything else is plain immutable lists. *)
    let compose_words (words : string list) : string =
        String.concat " " words

    let encode_par (str : string) (num : int) : string =
        let words = str.Split(' ') |> List.ofArray
        let numWords = List.length words
        // Ceiling division so that `num` chunks (tasks) are enough to cover all words.
        let chunkSize = max 1 ((numWords + num - 1) / num)
        let chunks = words |> List.chunkBySize chunkSize

        chunks
        |> List.map (fun chunk ->
            System.Threading.Tasks.Task.Run(fun () ->
                chunk |> List.map encode |> compose_words))
        |> List.map (fun (t : System.Threading.Tasks.Task<string>) -> t.Result)
        |> compose_words

    (* Question 4: The N-Queens problem (25%) *)



    




    type board2 = {width: int; queen: (int*int) list}

    let empty2 (N: int) : board2 = {width = N; queen = []}

    let get_dimension2 (b: board2) = b.width

    let has_queen (r: int) (c: int) (b: board2) : bool = 
        if c < b.width && r < b.width then List.contains (r, c) b.queen else false











































    (* Question 4.1
       We represent a board simply as its size plus the list of (row, column) squares that currently hold a
       queen. This is deliberately the simplest possible immutable representation: no arrays, no mutation,
       and every operation on it (has_queen, place_queen, valid_solution) is a one-line list operation. *)
    type board = { size : int; queens : (int * int) list }

    let empty (n : int) : board = { size = n; queens = [] }

    let get_dimension (b : board) : int = b.size

    let has_queen (r : int) (c : int) (b : board) : bool =
        r >= 0 && r < b.size && c >= 0 && c < b.size
        && List.exists (fun (qr, qc) -> qr = r && qc = c) b.queens

    (* Question 4.2
       place_queen only succeeds if the target square is on the board, currently empty, and not attacked by
       (nor attacking) any existing queen. Two queens threaten each other if they share a row, share a column,
       or lie on the same diagonal (the row-difference equals the column-difference in absolute value). *)
    let place_queen (r : int) (c : int) (b : board) : board option =
        if r < 0 || r >= b.size || c < 0 || c >= b.size then
            None
        elif has_queen r c b then
            None
        else
            let threatens (qr, qc) =
                qr = r || qc = c || abs (qr - r) = abs (qc - c)
            if List.exists threatens b.queens then
                None
            else
                Some { b with queens = (r, c) :: b.queens }

    (* Since (per the question's assumption) a board can only have been built up via `empty` and
       `place_queen`, every queen already placed is guaranteed to be conflict-free. So a solution is valid
       exactly when there is one queen per row, i.e. exactly N queens on an N*N board. *)
    let valid_solution (b : board) : bool =
        List.length b.queens = b.size

    (* Question 4.3
       chessMonad wraps a function board -> ('a * board) option, i.e. "run me on a board and you either get a
       result plus a (possibly updated) board, or nothing if something failed". This is exactly what
       place_queen and valid_solution already return (a board option / a bool), so place_queen2 / valid_solution2
       just need to lift those into the monad's shape. The question explicitly says we must break the
       abstraction (pattern-match on CM) here - that is unavoidable since these two functions are the ones
       that actually connect the "pure board" world of Q4.2 to the monad world. *)
    type chessMonad<'a> = CM of (board -> ('a * board) option)

    let ret x = CM (fun h -> (Some (x, h)))
    let fail  = CM (fun _ -> None)
    let bind f (CM a)  =
        CM (fun b ->
        match a b with
        | Some (x, b') ->
            let (CM g) = f x
            g b'
        | None -> None)

    let (>>=) a f = bind f a
    let (>>>=) a b = a >>= (fun _ -> b)

    let evalCM (CM f) N = f (empty N)

    let place_queen2 (r : int) (c : int) : chessMonad<unit> =
        CM (fun b ->
                match place_queen r c b with
                | Some b' -> Some ((), b')
                | None -> None)

    let valid_solution2 : chessMonad<bool> =
        CM (fun b -> Some (valid_solution b, b))

    (* Question 4.4
       create_solution just threads place_queen2 calls together with >>>= (i.e. "do this, ignore its unit
       result, then do the next thing"), and finishes with valid_solution2. Crucially this code never pattern
       matches on CM itself - it only uses the monadic operators - so it respects the abstraction boundary,
       as required. If any placement along the way fails, `bind` (hidden inside >>>=) automatically short-
       circuits the rest of the chain to None, which is exactly the "monadic failure" the question asks for. *)
    let create_solution (squares : (int * int) list) : chessMonad<bool> =
        let rec aux squares =
            match squares with
            | [] -> valid_solution2
            | (r, c) :: rest -> place_queen2 r c >>>= aux rest
        aux squares

    (* Question 4.5
       Same logic as create_solution, but written with the `chess` computation expression instead of the raw
       operators: `do!` plays the role of >>>= (run this step, discard its result, continue), and `return!`
       hands back an already-existing chessMonad value unchanged (via ReturnFrom). No CM pattern matching, and
       no explicit bind/>>=/>>>=/ret/fail in our own code - the computation expression handles all of that for
       us via the builder's members. *)
    type ChessBuilder() =
        member this.Bind(f, x)    = bind x f
        member this.Return(x)     = ret x
        member this.ReturnFrom(x) = x
        member this.Combine(a, b) = a >>= (fun _ -> b)

    let chess = new ChessBuilder()

    let create_solution2 (squares : (int * int) list) : chessMonad<bool> =
        let rec aux squares =
            chess {
                match squares with
                | [] -> return! valid_solution2
                | (r, c) :: rest ->
                    do! place_queen2 r c
                    return! aux rest
            }
        aux squares