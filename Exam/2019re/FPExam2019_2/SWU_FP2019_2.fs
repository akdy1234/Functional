(* Remove this namespace if submitting as an .fsx-file *)
namespace SWU_FP2019

module Sum =

    // Sum<'A,'B> is a discriminated union that holds EITHER an 'A (Left) or a 'B (Right)
    // Equivalent to Either in Haskell — a type-safe union of two disjoint types
    type Sum<'A, 'B> =
    | Left  of 'A
    | Right of 'B

    // ─── Question 1.1 ────────────────────────────────────────────────────────────

    // sum1 and sum2: concrete examples of Sum values
    // Type annotation makes the polymorphic Sum monomorphic so F# doesn't generalise weirdly
    let sum1 : Sum<int, string> = Left 1          // holds an int on the Left side
    let sum2 : Sum<int, string> = Right "hello"   // holds a string on the Right side

    // sumMap f g: apply f to a Left payload, g to a Right payload
    // The wrapper constructor is preserved — Left stays Left, Right stays Right
    // This is "bimap" / "mapBoth" in Haskell terminology
    //
    // sumMap (fun n -> n*2) String.length (Left 5)      = Left 10
    // sumMap (fun n -> n*2) String.length (Right "hi")  = Right 2
    let sumMap f g =
        function
        | Left  a -> Left  (f a)   // left side: transform payload with f
        | Right b -> Right (g b)   // right side: transform payload with g

    // ─── Question 1.2 ────────────────────────────────────────────────────────────

    // SumColl: a heterogeneous singly-linked list where each node is either
    // a CLeft node (holding an 'A) or a CRight node (holding a 'B), plus its tail
    // Nil terminates the list — like [] for ordinary lists
    type SumColl<'A, 'B> =
    | Nil
    | CLeft  of 'A * SumColl<'A, 'B>
    | CRight of 'B * SumColl<'A, 'B>

    // sumColl: example value mixing ints and strings
    let sumColl : SumColl<int, string> =
        CLeft(1, CRight("hello", CLeft(2, CRight("world", Nil))))

    // ofList: convert a Sum<'A,'B> list into a SumColl
    // Structural recursion: the head determines whether to build a CLeft or CRight node;
    // the tail is processed recursively to form the rest of the collection
    let rec ofList : Sum<'A,'B> list -> SumColl<'A,'B> =
        function
        | []              -> Nil
        | Left  a :: rest -> CLeft  (a, ofList rest)   // Left  element → CLeft  node
        | Right b :: rest -> CRight (b, ofList rest)   // Right element → CRight node

    // ─── Question 1.3 ────────────────────────────────────────────────────────────

    // reverse: reverse a SumColl using the standard accumulator pattern
    // Each element is prepended to the accumulator (building the reversed list).
    // The recursive call is always in tail position — no pending work after it returns.
    //
    // Trace on CLeft(1, CRight("a", Nil)):
    //   aux Nil           (CLeft(1, CRight("a", Nil)))
    // → aux CLeft(1,Nil)  (CRight("a", Nil))
    // → aux CRight("a", CLeft(1,Nil))  Nil
    // → CRight("a", CLeft(1, Nil))      ← reversed
    let reverse (coll : SumColl<'A,'B>) : SumColl<'A,'B> =
        let rec aux acc =
            function
            | Nil              -> acc
            | CLeft  (a, rest) -> aux (CLeft  (a, acc)) rest   // prepend to acc, tail-recurse
            | CRight (b, rest) -> aux (CRight (b, acc)) rest
        aux Nil coll

    // ─── Question 1.4 ────────────────────────────────────────────────────────────

    // ofList2: same as ofList but implemented via List.foldBack
    // List.foldBack f [x1;x2;x3] init = f x1 (f x2 (f x3 init))
    // Each element wraps the result of processing its tail → same forward order as ofList
    // Difference from List.fold: foldBack starts from the RIGHT, so the structure
    // is built from the end and the head ends up outermost — matching ofList's behaviour
    let ofList2 (lst : Sum<'A,'B> list) : SumColl<'A,'B> =
        List.foldBack
            (fun x acc ->
                match x with
                | Left  a -> CLeft  (a, acc)    // wrap current element, acc is the tail
                | Right b -> CRight (b, acc))
            lst Nil                             // Nil is the seed for the empty tail

    // ─── Question 1.5 ────────────────────────────────────────────────────────────

    // foldBackSumColl: structural right-fold over a SumColl
    //   fLeft  : 'A → 'acc → 'acc   how to combine a CLeft  element with the folded tail
    //   fRight : 'B → 'acc → 'acc   how to combine a CRight element with the folded tail
    //   init   : 'acc               seed value used when Nil is reached
    //
    // For CLeft(a, CRight(b, Nil)):
    //   fLeft a (fRight b init)
    //
    // NOT tail-recursive: fLeft/fRight are applied AFTER the recursive call returns
    let rec foldBackSumColl fLeft fRight (init : 'acc) (coll : SumColl<'A,'B>) : 'acc =
        match coll with
        | Nil              -> init
        | CLeft  (a, rest) -> fLeft  a (foldBackSumColl fLeft fRight init rest)
        | CRight (b, rest) -> fRight b (foldBackSumColl fLeft fRight init rest)


module CodeComprehension =

    // f: convert string s to a char list, preserving order
    // Uses an inner recursive function aux that indexes into s from 0 to length-1
    let f s =
        let l = String.length s
        let rec aux =
            function
            | i when i = l -> []
            | i -> s.[i] :: aux (i + 1)   // prepend s[i], then recurse on rest

        aux 0

    // g: palindrome check — ignores non-letter chars and is case-insensitive
    // Pipeline: string → char list → keep letters → lowercase → check list = reverse
    let g s =
        s |> f |>
        List.filter System.Char.IsLetter |>
        List.map System.Char.ToLower |>
        fun lst -> lst = List.rev lst

    // ─── Question 2.1 ────────────────────────────────────────────────────────────

    (*
    Analysis of f:

      f s converts a string to a char list by recursive index traversal.

      Mechanics:
        l = String.length s                   — total length, computed once
        aux i: if i = l → []                  — past the end: empty list
               else s.[i] :: aux (i+1)        — prepend current char, advance
      Returns [s.[0]; s.[1]; ...; s.[l-1]] — same order as the string.

      Time complexity: O(n) calls, each O(1) → O(n) overall.
      Space: O(n) stack depth + O(n) result list.

    Analysis of g:

      g builds on f with a five-step pipeline:
        1. f s            → char list of length n           (O(n))
        2. List.filter    → keep only System.Char.IsLetter  (O(n))
        3. List.map       → apply System.Char.ToLower       (O(n))
        4. List.rev       → reverse the filtered list       (O(n))
        5. (=) comparison → structural equality             (O(n))

      g returns true iff, after stripping non-letters and lowercasing,
      the string reads the same forwards and backwards (palindrome).

      Examples:
        g "racecar"                          → true
        g "A man a plan a canal Panama"      → true   ("amanaplanacanalpanama")
        g "hello"                            → false
        g "Was it a car or a cat I saw?"     → true
    *)

    // ─── Question 2.2 ────────────────────────────────────────────────────────────

    // f2: idiomatic reimplementation of f using List.init
    // List.init n fn = [fn 0; fn 1; ...; fn (n-1)]
    // → each element is the char at the corresponding index
    // Equivalent output to f, without manual recursion
    let f2 s = List.init (String.length s) (fun i -> s.[i])

    // ─── Question 2.3 ────────────────────────────────────────────────────────────

    // g2: same as g but uses f2 to convert string → char list
    // Behaviour is identical to g; only the conversion function changes
    let g2 s =
        s |> f2 |>
        List.filter System.Char.IsLetter |>
        List.map System.Char.ToLower |>
        fun lst -> lst = List.rev lst

    // ─── Question 2.4 ────────────────────────────────────────────────────────────

    (*
    Tail-recursion analysis of f:

      f is NOT tail-recursive.

      In the recursive branch:
          | i -> s.[i] :: aux (i + 1)
      The call aux (i+1) is NOT in tail position.
      After aux returns a list, we must still cons s.[i] onto its front (::).
      That pending cons operation means the current stack frame cannot be
      discarded before the recursive call — the call stack grows to depth n.
      On very long strings this causes a StackOverflowException.

    Tail-recursion analysis of g:

      g itself is not recursive at all — it pipelines library functions.
      The only recursion happens inside f, which is not tail-recursive as above.
      The library functions (filter, map, rev, =) are called sequentially and
      each returns before the next begins, so g itself never grows a stack.
      However, f's O(n) stack is still an issue for very long strings.
    *)

    // fTail: tail-recursive version of f using an accumulator
    // Collects chars in reverse order (acc = [s[i-1]; ...; s[0]]) during descent,
    // then reverses once at the end to restore natural order
    // The recursive call aux (i+1) (s.[i] :: acc) is in tail position — no pending work
    //
    // Trace on "hi":
    //   aux 0 []         → aux 1 ['h']        → aux 2 ['i';'h']
    //   i=2=l            → List.rev ['i';'h'] = ['h';'i']   ✓
    let fTail s =
        let l = String.length s
        let rec aux i acc =
            if i = l then List.rev acc              // done — restore forward order
            else aux (i + 1) (s.[i] :: acc)         // tail call: accumulate in reverse
        aux 0 []

    // ─── Question 2.5 ────────────────────────────────────────────────────────────

    // gOpt: palindrome check returning bool option
    // Returns None      if the string contains no alphabetic characters
    //                   (no meaningful palindrome to speak of)
    // Returns Some true  if the filtered+lowercased chars form a palindrome
    // Returns Some false otherwise
    let gOpt s =
        let filtered =
            s |> f |>
            List.filter System.Char.IsLetter |>
            List.map System.Char.ToLower
        match filtered with
        | []  -> None                           // no letters → undefined
        | lst -> Some (lst = List.rev lst)      // wrap result in Some


module GoldenRatio =

    // ─── Question 3.1 ────────────────────────────────────────────────────────────

    // The golden ratio φ ≈ 1.6180339887... satisfies φ² = φ + 1, equivalently φ = 1 + 1/φ.
    // Iterative convergence via continued-fraction approximation:
    //   start x₀ = 1.0, then xₙ₊₁ = 1 + 1/xₙ
    //
    // Convergence:
    //   n=0 : 1.0
    //   n=1 : 1 + 1/1   = 2.0
    //   n=2 : 1 + 1/2   = 1.5
    //   n=3 : 1 + 1/1.5 = 1.666...
    //   n=4 : 1 + 1/1.6̄ = 1.6
    //   n=5 : 1 + 1/1.6 = 1.625        →  converges to φ
    //
    // Implementation: tail-recursive aux counts down from n to 0
    let calculateGoldenRatio n =
        let rec aux i x =
            if i = 0 then x                        // 0 iterations left → current approx
            else aux (i - 1) (1.0 + 1.0 / x)      // apply one iteration; tail call
        aux n 1.0

    // ─── Question 3.2 ────────────────────────────────────────────────────────────

    // grSeq: infinite sequence of golden ratio approximations using Seq.unfold
    // State: current approximation x
    // Each step yields x (current approx) and advances to 1 + 1/x
    // The argument is the starting value x₀ (use 1.0 for the standard sequence)
    //
    // grSeq 1.0 = seq [1.0; 2.0; 1.5; 1.6̄; 1.6; 1.625; ...]
    let grSeq startVal =
        Seq.unfold (fun x -> Some (x, 1.0 + 1.0 / x)) startVal

    // ─── Question 3.3 ────────────────────────────────────────────────────────────

    // goldenRectangleSeq: infinite sequence of (width, height) pairs
    // A golden rectangle has width:height = φ.
    // Subdivision rule: given rectangle (a, b) with a ≥ b, removing the b×b square
    // leaves (b, a-b) — a smaller rectangle whose ratio also converges to φ.
    // (This is the geometric origin of the Fibonacci spiral.)
    //
    // State: (a, b) current rectangle, always normalised so a ≥ b
    // goldenRectangleSeq 2.0 = seq [(2.0,1.0); (1.0,1.0); (1.0,0.0); ...]
    let goldenRectangleSeq r =
        Seq.unfold
            (fun (a, b) ->
                let (w, h) = if a >= b then (a, b) else (b, a)  // normalise: wider first
                Some ((w, h), (h, w - h)))                       // yield (w,h); next = (h, w-h)
            (r, 1.0)

    // goldenTriangleSeq: infinite sequence of (legs, base) pairs for golden triangles
    // A golden triangle is a 36°-72°-72° isosceles triangle with legs:base = φ.
    // Subdivision: bisect from a base angle — the smaller similar triangle has
    //   legs = old base, base = old legs - old base
    // The ratio legs/base converges to φ through successive subdivision.
    //
    // State: (legs, base)
    let goldenTriangleSeq r =
        Seq.unfold
            (fun (legs, bse) -> Some ((legs, bse), (bse, legs - bse)))
            (r, 1.0)

    // ─── Question 3.4 ────────────────────────────────────────────────────────────

    // goldenRectangleTriangle: zip the golden rectangle and triangle sequences
    // Each element is a pair ((rect_width, rect_height), (tri_legs, tri_base))
    // representing one level of subdivision of each shape simultaneously.
    // Both sequences start from the same initial ratio r.
    let goldenRectangleTriangle r =
        Seq.zip (goldenRectangleSeq r) (goldenTriangleSeq r)


module Qwirkle =

    // ─── Question 4.1 ────────────────────────────────────────────────────────────

    // Qwirkle tiles: every tile has exactly one color and one shape.
    // There are 6 colors × 6 shapes = 36 distinct tiles in the full game set.
    type color = Red | Orange | Yellow | Green | Blue | Purple
    type shape = Circle | Square | Diamond | Clover | Star | Cross

    // Tile of color * shape — a pair packed in a single-case DU for type safety
    type tile = Tile of color * shape

    // ─── Question 4.2 ────────────────────────────────────────────────────────────

    // validTiles: generate all 36 valid tiles via a nested list comprehension
    // Every (color, shape) combination appears exactly once — no repeats
    let validTiles () =
        [ for c in [Red; Orange; Yellow; Green; Blue; Purple] do
          for s in [Circle; Square; Diamond; Clover; Star; Cross] do
            yield Tile(c, s) ]

    // ─── Question 4.3 optional ───────────────────────────────────────────────────

    type coord     = Coord of int * int
    type board     = Board of Map<coord, tile>
    type direction = Left | Right | Up | Down

    // moveCoord: shift a coordinate one step in the given direction
    // Left/Right → x axis; Up/Down → y axis
    let moveCoord dir (Coord(x, y)) =
        match dir with
        | Left  -> Coord(x - 1, y)
        | Right -> Coord(x + 1, y)
        | Up    -> Coord(x,     y + 1)
        | Down  -> Coord(x,     y - 1)

    // collectTiles: collect all tiles on the board starting from the NEIGHBOUR of coord
    // in the given direction, continuing until an empty cell is found.
    // Returns tiles in order nearest-to-farthest from coord.
    //
    // We start from the neighbour (not coord itself) because coord is the cell
    // we are about to place a tile in — it is empty and is handled by the caller.
    let collectTiles (Board board) dir coord =
        let rec aux pos acc =
            match Map.tryFind pos board with
            | None   -> List.rev acc                         // empty cell → done; restore order
            | Some t -> aux (moveCoord dir pos) (t :: acc)  // found tile → accumulate; continue
        aux (moveCoord dir coord) []

    // ─── Question 4.4 ────────────────────────────────────────────────────────────

    // validLine: check whether a contiguous line (row or column) of tiles is valid
    // Qwirkle rules:
    //   1. All tiles in a line must share exactly ONE attribute:
    //      either all the same color (different shapes) or all the same shape (different colors)
    //   2. No two identical tiles in the same line
    let private validLine (tiles : tile list) =
        match tiles with
        | [] | [_] -> true   // 0 or 1 tile: trivially valid
        | _ ->
            let colors    = tiles |> List.map (fun (Tile(c, _)) -> c) |> List.distinct
            let shapes    = tiles |> List.map (fun (Tile(_, s)) -> s) |> List.distinct
            let n         = List.length tiles
            let noDups    = List.length (List.distinct tiles) = n   // no duplicate tile objects
            let sharedAttr = List.length colors = 1 || List.length shapes = 1
            noDups && sharedAttr

    // placeTile: attempt to place tile t at coord on the board
    // 1. Fail immediately if coord is already occupied
    // 2. Collect the tiles that would form the resulting row and column
    // 3. Validate both lines; return Some updatedBoard on success, None on failure
    //
    // collectTiles returns nearest-to-farthest, so we reverse the left/up halves
    // to assemble them in left-to-right / bottom-to-top order
    let placeTile (brd : board) coord (t : tile) : board option =
        let (Board map) = brd
        if Map.containsKey coord map then
            None   // cell already occupied
        else
            let leftTiles  = collectTiles brd Left  coord
            let rightTiles = collectTiles brd Right coord
            let upTiles    = collectTiles brd Up    coord
            let downTiles  = collectTiles brd Down  coord
            // full row: tiles left of coord (reversed to get L→R order) + new tile + tiles right
            let rowTiles = List.rev leftTiles @ [t] @ rightTiles
            // full column: tiles above coord (reversed to get bottom→top) + new tile + tiles below
            let colTiles = List.rev upTiles   @ [t] @ downTiles
            if validLine rowTiles && validLine colTiles then
                Some (Board (Map.add coord t map))
            else
                None

    // ─── Question 4.5 ────────────────────────────────────────────────────────────

    // Railroad-oriented programming (option monad)
    // ret wraps a value in Some; bind threads a Some value through a function
    // None short-circuits the chain — any failed step aborts the whole sequence
    let ret = Some
    let bind f =
        function
        | None   -> None
        | Some x -> f x
    let (>>=) x f = bind f x

    // Computation expression builder for option monad
    type opt<'a> = 'a option
    type OptBuilderClass() =
        member t.Return     (x : 'a)                          : opt<'a> = ret x
        member t.ReturnFrom (x : opt<'a>)                               = x
        member t.Bind       (o : opt<'a>, f : 'a -> opt<'b>) : opt<'b> = bind f o
    let opt = new OptBuilderClass()

    // placeTiles: attempt to place a sequence of (coord, tile) pairs on the board
    // Uses the option monad via >>= to chain placements.
    // If any single placement fails, the whole operation yields None —
    // no partial updates are applied (fail-fast behaviour).
    //
    // List.fold carries Some board as the running state:
    //   accOpt >>= (fun b -> placeTile b coord tile)
    // If accOpt = None, >>= immediately returns None (short-circuit).
    // If accOpt = Some b, we attempt the next placement and thread the result.
    let placeTiles (board : board) (tiles : (coord * tile) list) : board option =
        List.fold
            (fun accOpt (coord, tile) ->
                accOpt >>= (fun b -> placeTile b coord tile))
            (Some board)
            tiles
