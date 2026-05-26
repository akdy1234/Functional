module Exam2020_2
(* If you are importing this into F# interactive then comment out
   the line above and remove the comment for the line bellow.

   Do note that the project will not compile if you do this, but
   it does allow you to work in interactive mode and you can just remove the '='
   to make the project compile work again.

   Do not remove the line (even though that does work) because you may inadvertantly
   introduce indentation errors in your code that may be hard to find if you want
   to switch back to project mode.

   Alternative, keep the line as is, but load ExamInteractive.fsx into the interactive environment
   *)
(* module Exam2020_2 = *)

(* 1: Binary search trees *)

    // A BST where every node's value is greater than all values in its left subtree
    // and less than all values in its right subtree.
    type 'a bintree =
    | Leaf
    | Node of 'a bintree * 'a * 'a bintree

(* Question 1.1 *)

    // insert x t — insert x into BST t, maintaining the BST property.
    //
    // CASES:
    //   Leaf          → create a new leaf node with x
    //   Node(l,y,r) where x < y → insert into left subtree
    //   Node(l,y,r) where x > y → insert into right subtree
    //   x = y         → x already present; BSTs store no duplicates, return unchanged
    //
    // Returns a new tree (F# values are immutable).
    let rec insert (x : 'a) : 'a bintree -> 'a bintree = function
        | Leaf                     -> Node(Leaf, x, Leaf)
        | Node(l, y, r) when x < y -> Node(insert x l, y, r)
        | Node(l, y, r) when x > y -> Node(l, y, insert x r)
        | t                        -> t  // x = y, no duplicate

(* Question 1.2 *)

    // fromList lst — build a BST from a list by inserting elements one by one.
    //
    // List.fold accumulates into a tree; each element is inserted in sequence.
    // The resulting BST shape depends on the insertion order.
    let fromList (lst : 'a list) : 'a bintree =
        List.fold (fun t x -> insert x t) Leaf lst

(* Question 1.3 *)

    // fold f acc t — in-order left fold over the BST.
    //
    // IN-ORDER: visits left subtree, then current node, then right subtree.
    // This visits elements in ascending sorted order (BST property).
    //
    //   fold f acc Leaf        = acc
    //   fold f acc (Node l x r) = fold f (f (fold f acc l) x) r
    //
    // The accumulator flows left → node → right.
    let rec fold (f : 'acc -> 'a -> 'acc) (acc : 'acc) : 'a bintree -> 'acc = function
        | Leaf          -> acc
        | Node(l, x, r) ->
            let acc'  = fold f acc l
            let acc'' = f acc' x
            fold f acc'' r

    // foldBack f acc t — in-order RIGHT fold over the BST.
    //
    // Visits right subtree first, then current node, then left subtree.
    // Produces elements in DESCENDING order.
    let rec foldBack (f : 'acc -> 'a -> 'acc) (acc : 'acc) : 'a bintree -> 'acc = function
        | Leaf          -> acc
        | Node(l, x, r) ->
            let acc'  = foldBack f acc r
            let acc'' = f acc' x
            foldBack f acc'' l

    // inOrder — extract all elements in sorted order as a list.
    //
    // Uses fold with (fun acc x -> acc @ [x]) — each element appended in order.
    // Note: @ is O(n) per call; for large trees a more efficient version would
    // use an accumulator and List.rev.
    let inOrder (t : 'a bintree) : 'a list =
        fold (fun acc x -> acc @ [x]) [] t

(* Question 1.4 *)

    (*

    Q: Consider the following map function

    *)

    let rec badMap f =
      function
      | Leaf -> Leaf
      | Node (l, y, r) -> Node (badMap f l, f y, badMap f r)

    (*
    Even though the type of this function is `('a -> 'b) -> 'a bintree -> 'b bintree`
    as we would expect from a map function, this function does not do what
    we want it to do. What is the problem? Provide an example to demonstrate the problem.

    A: badMap preserves the SHAPE of the tree but does NOT maintain the BST property
       after mapping.

       PROBLEM: if f changes the relative ordering of values (e.g. negation), the
       resulting tree may have nodes in the wrong positions — left subtrees may
       contain values larger than the root, violating the BST invariant.

       EXAMPLE:
       Let t = fromList [2; 1; 3]
       = Node(Node(Leaf,1,Leaf), 2, Node(Leaf,3,Leaf))
         (BST: 1 < 2 < 3)

       badMap (fun x -> -x) t
       = Node(Node(Leaf,-1,Leaf), -2, Node(Leaf,-3,Leaf))

       But -1 > -2, so -1 should be in the RIGHT subtree of -2, not the left.
       The tree structure is wrong: the BST invariant is violated.
       If you then tried to search this tree for -1, you would look in the left
       subtree (since -1 < -2? No, -1 > -2) and fail to find it.

       CORRECT map must RE-INSERT each mapped value into a new BST so that
       the BST property is maintained after the transformation:
    *)

    // map f t — structure-preserving map that maintains BST property.
    //
    // We traverse all elements in-order (using fold) and INSERT each mapped
    // value into a fresh BST.  This ensures the result is a valid BST.
    //
    // NOTE: the shape of the result may differ from the input (re-inserting
    // mapped values in sorted order may rebalance the tree).
    let map (f : 'a -> 'b) (t : 'a bintree) : 'b bintree =
        fold (fun acc x -> insert (f x) acc) Leaf t


(* 2: Code Comprehension *)

    let rec foo =
        function
        | [x]                 -> [x]
        | x::y::xs when x > y -> y :: (foo (x::xs))
        | x::xs               -> x :: foo xs

    let rec bar =
        function
        | [x]          -> true
        | x :: y :: xs -> x < y && bar (y :: xs)

    let rec baz =
        function
        | []               -> []
        | lst when bar lst -> lst
        | lst              -> baz (foo lst)

(* Question 2.1 *)

    (*

    Q: What are the types of functions foo, bar, and baz?

    A: foo : 'a list -> 'a list    (requires 'a : comparison)
         Takes a non-empty list, returns a list of the same type.
         The `when x > y` guard requires comparison.

       bar : 'a list -> bool       (requires 'a : comparison)
         Takes a non-empty list, returns a bool.

       baz : 'a list -> 'a list    (requires 'a : comparison)
         Takes a list, returns a sorted list.


    Q: What do functions bar and baz do
       (not foo, we admit that it is a bit contrived)?
       Focus on what they do rather than how they do it.

    A: bar lst — checks whether lst is sorted in STRICTLY ASCENDING order.
         Returns true if every element is strictly less than the next.
         e.g. bar [1;2;3] = true,  bar [1;3;2] = false

       baz lst — SORTS a list using a bubble-sort-like algorithm.
         Repeatedly applies foo (one sweep that moves elements toward sorted order)
         until bar confirms the list is sorted.
         e.g. baz [3;1;2] = [1;2;3]


    Q: What would be appropriate names for functions foo, bar, and baz?

    A: foo → bubbleStep   (one pass of bubble sort, swapping adjacent out-of-order pairs)
       bar → isSorted     (checks strict ascending order)
       baz → bubbleSort   (repeated sweeps until sorted)

    *)

(* Question 2.2 *)

    (*
    The functions foo and bar generate a warning during compilation:
    'Warning: Incomplete pattern matches on this expression.'

    Q: Why does this happen, and where?

    A: foo is missing the empty list `[]` case.
       bar is missing the empty list `[]` case.

       The first pattern of foo is `[x]` (a single-element list), not `_` or `[]`.
       Similarly bar starts with `[x]`.
       The F# compiler warns because [] is not covered — matching [] would raise
       MatchFailureException at runtime.


    Q: For these particular three functions will this incomplete
       pattern match ever cause problems for any possible execution of baz?
       If yes, why; if no, why not.

    A: NO, it will never cause problems.

       baz calls foo and bar only with non-empty lists:
         - `baz []` immediately returns [] without calling foo or bar.
         - `baz lst` for non-empty lst: bar lst handles [x] and x::y::xs (all
           non-empty cases), and foo lst is only called when bar lst = false,
           which requires lst to have at least two elements.

       Therefore the [] case of foo and bar is never reached from baz.

    *)

    // foo2 / bar2 — safe versions that handle the empty list.
    let rec foo2 : 'a list -> 'a list = function
        | []                  -> []
        | [x]                 -> [x]
        | x::y::xs when x > y -> y :: (foo2 (x::xs))
        | x::xs               -> x :: foo2 xs

    let rec bar2 : 'a list -> bool = function
        | []           -> true
        | [_]          -> true
        | x :: y :: xs -> x < y && bar2 (y :: xs)

    (* Uncomment code to run after you have written foo2 and bar2 *)
    let rec baz2 : 'a list -> 'a list = function
        | lst when bar2 lst -> lst
        | lst               -> baz2 (foo2 lst)

(* Question 2.3 *)

    (* Consider this alternative definition of *)

    let rec foo3 =
      function
      | [x]                 -> [x]
      | x::xs               -> x :: foo3 xs
      | x::y::xs when x > y -> y :: (foo3 (x::xs))

    (*

    Q: Do the functions `foo` and `foo3` produce the same output for all possible inputs?
       If yes, why; if no why not and provide a counter example.

    A: NO, they do NOT produce the same output.

       In foo3, the pattern `x::xs` appears BEFORE `x::y::xs when x > y`.
       Since `x::y::xs` is a special case of `x::xs` (xs = y::rest), the more
       general pattern `x::xs` ALWAYS matches first — the third pattern is
       DEAD CODE and is never reached.

       foo3 therefore just returns `x :: foo3 xs` for any list with 2+ elements,
       meaning it never performs any swap — it returns the input unchanged
       (structurally it's the identity function on non-singleton lists).

       Counter-example:
         foo  [3;1;2] = [1;2;3]   (3 > 1, swap: 1 :: foo [3;2]; then 3 > 2, swap: 2 :: foo [3] = [1;2;3])

       Wait let me retrace foo [3;1;2]:
         x=3, y=1, xs=[2], 3>1: 1 :: (foo [3;2])
           foo [3;2]: x=3, y=2, xs=[], 3>2: 2 :: (foo [3]) = 2 :: [3] = [2;3]
         = 1 :: [2;3] = [1;2;3]

       foo3 [3;1;2]:
         x=3, xs=[1;2]: 3 :: foo3 [1;2]
           foo3 [1;2]: x=1, xs=[2]: 1 :: foo3 [2] = 1 :: [2] = [1;2]
         = 3 :: [1;2] = [3;1;2]   (unchanged!)

       So foo [3;1;2] = [1;2;3] but foo3 [3;1;2] = [3;1;2]. They differ. ✓

    *)

(* Question 2.4 *)

    // bar3 — non-recursive version of bar using List.pairwise.
    //
    // List.pairwise [a;b;c;d] = [(a,b);(b,c);(c,d)]
    // We check that every adjacent pair (x,y) satisfies x < y.
    // List.forall short-circuits on the first false.
    let bar3 (lst : 'a list) : bool =
        match lst with
        | [] -> true
        | _  -> lst |> List.pairwise |> List.forall (fun (x, y) -> x < y)

(* Question 2.5 *)

    (*

    Q: The function foo or baz is not tail recursive. Which one and why?

    A: FOO is not tail recursive.

       Evaluate foo [3;1]:
         foo [3;1]
         = 1 :: (foo [3])          (* 3 > 1, swap *)
                  ^^^^^^^^
                  recursive call inside `::` — the `::` is pending.
         = 1 :: ([3])              (* base case *)
         = [1;3]

       The recursive call `foo [3]` is the ARGUMENT to `::`.
       After it returns, the runtime still needs to prepend 1.
       That pending operation keeps the current frame on the stack.
       For a list of length n, up to n frames accumulate — not tail-recursive.

       baz IS tail-recursive: `baz (foo lst)` is the last operation in the
       non-sorted branch.  After `baz (foo lst)` returns, there is nothing
       else to do — the result is returned directly.  This is tail position.

    *)

    (* ONLY implement the one that is NOT already tail recursive *)

    // fooTail — tail-recursive version of foo using an accumulator.
    //
    // We collect elements that have already passed (in `acc`, reversed),
    // then combine with the rest at the end.
    //
    // KEY INSIGHT for the swap case:
    //   Original foo: x > y → y :: foo (x::xs)
    //   In fooTail: push y into acc (y goes before x in the output),
    //   then continue with (x::xs).  The `::` happens to acc before the
    //   recursive call — no pending `::` after the call.
    let fooTail (lst : 'a list) : 'a list =
        let rec aux acc = function
            | []                  -> List.rev acc
            | [x]                 -> List.rev (x :: acc)
            | x::y::xs when x > y -> aux (y :: acc) (x :: xs)
            | x::xs               -> aux (x :: acc) xs
        aux [] lst

    // bazTail is NOT needed — baz is already tail recursive (see analysis above).
    // Provided as a no-op for completeness.
    let bazTail (lst : 'a list) : 'a list =
        let rec aux = function
            | []               -> []
            | lst when bar2 lst -> lst
            | lst               -> aux (fooTail lst)
        aux lst


(* 3: Big Integers *)

(* Question 3.1 *)

    // A big integer is stored as a list of single decimal digits 0–9,
    // most significant digit FIRST (big-endian).
    // e.g., the integer 1234 is represented as [1;2;3;4].
    //
    // This matches the human-readable notation and makes toString trivial.
    type bigInt = int list

    // fromString — convert a decimal digit string to bigInt.
    // "1234" → [1;2;3;4]
    // Each character is converted via `int c - int '0'` (ASCII arithmetic).
    let fromString (s : string) : bigInt =
        [ for c in s -> int c - int '0' ]

    // toString — convert bigInt back to its decimal string.
    // [1;2;3;4] → "1234"
    let toString (n : bigInt) : string =
        n |> List.map string |> String.concat ""

(* Question 3.2 *)

    // add n1 n2 — add two big integers.
    //
    // STRATEGY: reverse both (to get LSB first), add digit-by-digit with carry,
    // then reverse the result.
    //
    // addRev processes LSB-first lists with a carry:
    //   When one list is exhausted, continue adding the carry to the other.
    //   When both are exhausted, if carry > 0 prepend it.
    let add (n1 : bigInt) (n2 : bigInt) : bigInt =
        let rec addRev xs ys carry =
            match xs, ys with
            | [], []           ->
                if carry > 0 then [carry] else []
            | [], y :: ys' ->
                let s = y + carry in (s % 10) :: addRev [] ys' (s / 10)
            | x :: xs', [] ->
                let s = x + carry in (s % 10) :: addRev xs' [] (s / 10)
            | x :: xs', y :: ys' ->
                let s = x + y + carry in (s % 10) :: addRev xs' ys' (s / 10)
        List.rev (addRev (List.rev n1) (List.rev n2) 0)

(* Question 3.3 *)

    // multSingle n d — multiply big integer n by a single digit d (0–9).
    //
    // STRATEGY: reverse n (LSB first), multiply each digit by d with carry,
    // then reverse result and strip leading zeros.
    //
    // Each position: s = digit * d + carry; output digit = s % 10; new carry = s / 10.
    let multSingle (n : bigInt) (d : int) : bigInt =
        let rec aux carry = function
            | [] ->
                if carry > 0 then [carry] else []
            | x :: rest ->
                let s = x * d + carry
                (s % 10) :: aux (s / 10) rest
        let raw = List.rev (aux 0 (List.rev n))
        // Remove leading zeros (but always keep at least one digit)
        match raw |> List.skipWhile (fun x -> x = 0) with
        | [] -> [0]
        | xs -> xs

(* Question 3.4 *)

    // mult n1 n2 — multiply two big integers.
    //
    // LONG MULTIPLICATION:
    //   Decompose n2 digit by digit (LSB first).
    //   For the i-th digit d of n2 (from the right), compute n1 * d
    //   and shift left by i positions (multiply by 10^i = append i zeros at end).
    //   Sum all partial products.
    let mult (n1 : bigInt) (n2 : bigInt) : bigInt =
        let n2Rev = List.rev n2
        let partials =
            n2Rev |> List.mapi (fun i d ->
                let product = multSingle n1 d
                product @ List.replicate i 0)  // shift: append i zeros at LSB end
        List.fold add [0] partials

(* Question 3.5 *)

    // fact n — compute n! as a bigInt.
    //
    // Multiplies [1;2;...;n] together using the bigInt mult function.
    // For n=0, returns [1] (0! = 1).
    //
    // fromString converts each int to bigInt; List.fold chains the multiplications.
    let fact (n : int) : bigInt =
        if n <= 0 then [1]
        else
            [1..n]
            |> List.map (fun i -> fromString (string i))
            |> List.fold mult [1]


(* 4: Lazy lists *)

    // 'a llist — a lazy (infinite) list.
    // Each Cons holds a THUNK (unit -> 'a * 'a llist) that produces
    // the head value AND the rest of the list ON DEMAND.
    // Nothing is computed until `step` forces the thunk.
    type 'a llist =
    | Cons of (unit -> ('a * 'a llist))

    let rec llzero = Cons (fun () -> (0, llzero))

(* Question 4.1 *)

    // step ll — force one step: evaluate the thunk to get (head, tail).
    let step (Cons f : 'a llist) : 'a * 'a llist = f ()

    // cons x ll — prepend x to a lazy list without forcing ll.
    // The thunk simply returns (x, ll) when forced.
    let cons (x : 'a) (ll : 'a llist) : 'a llist =
        Cons (fun () -> (x, ll))

(* Question 4.2 *)

    // init f — create the infinite lazy list [f 0; f 1; f 2; ...].
    //
    // aux n creates the n-th element lazily: when forced, it returns (f n, aux (n+1)).
    // No element is computed until stepped.
    let init (f : int -> 'a) : 'a llist =
        let rec aux n = Cons (fun () -> (f n, aux (n + 1)))
        aux 0

(* Question 4.3 *)

    // llmap f ll — lazily apply f to every element of ll.
    //
    // Each step: force ll to get (x, rest), return (f x, llmap f rest).
    // The result is a new lazy list where every element is transformed by f.
    let rec llmap (f : 'a -> 'b) (ll : 'a llist) : 'b llist =
        Cons (fun () ->
            let (x, rest) = step ll
            (f x, llmap f rest))

(* Question 4.4 *)

    // filter pred ll — lazily filter elements of ll by predicate pred.
    //
    // Scans forward until an element satisfying pred is found;
    // that element becomes the next head. The tail recursively filters the remainder.
    //
    // NOTE: if pred is never true, this diverges (infinite scan with no yield).
    let rec filter (pred : 'a -> bool) (ll : 'a llist) : 'a llist =
        Cons (fun () ->
            let rec findNext ll =
                let (x, rest) = step ll
                if pred x then (x, filter pred rest)
                else findNext rest
            findNext ll)

(* Question 4.5 *)

    // takeFirst n ll — extract the first n elements as an ordinary list.
    //
    // Steps through the lazy list n times, collecting heads.
    // n = 0 → [] (base case, stop stepping).
    let rec takeFirst (n : int) (ll : 'a llist) : 'a list =
        if n = 0 then []
        else
            let (x, rest) = step ll
            x :: takeFirst (n - 1) rest

(* Question 4.6 *)

    // unfold f init — create an infinite lazy list from a seed and a generator.
    //
    // f takes the current state and returns (element, nextState).
    // This is the lazy analogue of List.unfold.
    //
    // Example: unfold (fun n -> (n, n+1)) 0 = [0;1;2;3;...]
    let unfold (f : 'state -> 'a * 'state) (init : 'state) : 'a llist =
        let rec aux s = Cons (fun () ->
            let (x, s') = f s
            (x, aux s'))
        aux init

    let fib x =
        let rec aux acc1 acc2 =
            function
            | 0 -> acc1
            | x -> aux acc2 (acc1 + acc2) (x - 1)
        aux 0 1 x

    (* Uncomment after you have implemented init and unfold *)

    let fibll1 = init fib
    let fibll2 = unfold (fun (acc1, acc2) -> (acc1, (acc2, acc1 + acc2))) (0, 1)

    (*

    Q: Both fibll1 and fibll2 correctly calculate a lazy list of Fibonacci numbers.
       Which of these two lazy lists is the most efficient implementation and why?

    A: fibll2 is MORE EFFICIENT.

       fibll1 = init fib:
         `init fib` creates [fib 0; fib 1; fib 2; ...].
         To produce element n, it calls `fib n`, which traverses from 0 up to n
         using the auxiliary accumulator loop — O(n) work per element.
         Getting the first N elements costs 1 + 2 + ... + N = O(N²) total.

       fibll2 = unfold ... (0, 1):
         Each step takes the current pair (acc1, acc2), emits acc1, and produces
         the next pair (acc2, acc1+acc2) — exactly one addition and one tuple
         construction, O(1) per element.
         Getting the first N elements costs O(N) total.

       fibll2 avoids recomputing earlier Fibonacci numbers; it computes each
       number ONCE by passing the previous two values forward as state.

    *)
