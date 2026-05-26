module Exam2020
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
(* module Exam2020 = *)

(* 1: Insertion sort *)

(* Question 1.1 *)

    // insert x lst — insert x into a sorted list in the correct position.
    //
    // PATTERN:
    //   []            → [x]          base case: x goes at the end
    //   y :: rest when x <= y → x :: y :: rest   found the position, insert here
    //   y :: rest     → y :: insert x rest       y is smaller, recurse on tail
    //
    // WHY NOT TAIL-RECURSIVE: the `y :: insert x rest` case leaves a pending
    // `::` on the stack after each recursive call.
    let rec insert (x : 'a) (lst : 'a list) : 'a list =
        match lst with
        | []                   -> [x]
        | y :: rest when x <= y -> x :: y :: rest
        | y :: rest             -> y :: insert x rest

    // insertionSort — sort a list by folding insert over each element.
    //
    // We start with an empty sorted list and insert each element.
    // List.fold processes the input left-to-right; each element is inserted
    // into the current sorted accumulator.
    let insertionSort (lst : 'a list) : 'a list =
        List.fold (fun sorted x -> insert x sorted) [] lst

(* Question 1.2 *)

    // insertTail — tail-recursive version of insert using an accumulator.
    //
    // We collect elements that belong before x in `acc` (in reverse order),
    // then once we find the insertion point we combine:
    //   List.rev acc @ (x :: y :: rest)
    //
    // WHY IT WORKS:
    //   When we find a y >= x, the reversed acc is the correct prefix.
    //   At the empty-list base case, x goes at the very end.
    let insertTail (x : 'a) (lst : 'a list) : 'a list =
        let rec aux acc = function
            | []                    -> List.rev (x :: acc)
            | y :: rest when x <= y -> List.rev acc @ (x :: y :: rest)
            | y :: rest             -> aux (y :: acc) rest
        aux [] lst

    let insertionSortTail (lst : 'a list) : 'a list =
        List.fold (fun sorted x -> insertTail x sorted) [] lst

(* Question 1.3 *)

    (*
    Q: Why are the higher-order functions from the List library
    not a good fit to implement insert?

    A: The insert function must find the FIRST position where x is ≤ the element,
       insert x there, and STOP — leaving the rest of the list unchanged.

       Higher-order functions like List.fold and List.map always process the
       ENTIRE list; they cannot short-circuit once the insertion point is found.
       Using List.fold to simulate insert would still traverse all elements
       after the insertion point unnecessarily — O(n) comparisons even when
       the insertion point is near the front.

       List.span (or List.takeWhile + List.skipWhile) can split the list at the
       correct position in one pass, and the implementation below uses that:
    *)

    // insertionSort2 — uses List.span to find the split point for each insert.
    //
    // List.span p lst = (List.takeWhile p lst, List.skipWhile p lst)
    // before = elements strictly less than x (go before x in sorted order)
    // after  = elements >= x (go after x)
    // We sandwich x between them.
    let insertionSort2 (lst : 'a list) : 'a list =
        let insert2 x sorted =
            let (before, after) = List.span (fun y -> y < x) sorted
            before @ [x] @ after
        List.fold (fun sorted x -> insert2 x sorted) [] lst

(* Question 1.4 *)

    // insertBy cmp x lst — generalised insert using a comparison function.
    //
    // cmp a b returns:
    //   < 0 if a < b
    //     0 if a = b
    //   > 0 if a > b
    //
    // We insert x before the first element y where cmp x y <= 0.
    // The accumulator pattern from insertTail is reused here for tail recursion.
    let insertBy (cmp : 'a -> 'a -> int) (x : 'a) (lst : 'a list) : 'a list =
        let rec aux acc = function
            | []                          -> List.rev (x :: acc)
            | y :: rest when cmp x y <= 0 -> List.rev acc @ (x :: y :: rest)
            | y :: rest                   -> aux (y :: acc) rest
        aux [] lst

    // insertionSortBy — general insertion sort parameterised on a comparison.
    let insertionSortBy (cmp : 'a -> 'a -> int) (lst : 'a list) : 'a list =
        List.fold (fun sorted x -> insertBy cmp x sorted) [] lst


(* 2: Code Comprehension *)

    let rec foo x =
        function
        | y :: ys when x = y -> ys
        | y :: ys            -> y :: (foo x ys)

    let rec bar x =
        function
        | []        -> []
        | xs :: xss -> (x :: xs) :: bar x xss

    let rec baz =
        function
        | [] -> []
        | [x] -> [[x]]
        | xs  ->
            let rec aux =
                function
                | []      -> []
                | y :: ys -> ((foo y >> baz >> bar y) xs) @ (aux ys)
            aux xs

(* Question 2.1 *)

    (*

    Q: What are the types of functions foo, bar, and baz?

    A: foo : 'a -> 'a list -> 'a list    (requires 'a : equality)
         Takes a value x and a list; removes the FIRST occurrence of x.
         The `when x = y` guard requires equality comparison.

       bar : 'a -> 'a list list -> 'a list list
         Takes a value x and a list of lists; prepends x to every sublist.

       baz : 'a list -> 'a list list     (requires 'a : equality)
         Takes a list; returns all permutations of that list.


    Q: What do functions foo, bar, and baz do?
       Focus on what they do rather than how they do it.

    A: foo x lst — removes the first occurrence of x from lst.
         Traverses until it finds x, skips it, returns the rest unchanged.
         If x is not in lst, raises MatchFailureException (see Q2.2).

       bar x xss — prepends x to every list in xss.
         Maps (x ::) over the outer list.

       baz xs — returns ALL permutations of xs.
         For each element y in xs:
           1. Remove y from xs with foo.
           2. Generate all permutations of the remainder with baz.
           3. Prepend y to each permutation with bar.
         Concatenating over all choices of y gives every permutation.


    Q: What would be appropriate names for functions foo, bar, and baz?

    A: foo → removeFirst     (removes first occurrence of an element)
       bar → prependAll      (prepends a value to every sublist)
       baz → permutations    (generates all permutations of a list)

    *)

(* Question 2.2 *)

    (*
    The function foo generates a warning during compilation:
    Warning: Incomplete pattern matches on this expression.

    Q: Why does this happen, and where?

    A: The function foo is defined as:
         | y :: ys when x = y -> ys
         | y :: ys            -> y :: (foo x ys)
       The empty list `[]` is NOT handled.  If x is not found and the
       recursion reaches [], F# would throw MatchFailureException.
       The compiler warns at the `function` expression inside foo.


    Q: For these particular three functions will this incomplete
       pattern match ever cause problems for any possible execution of baz?
       If yes, why; if no, why not.

    A: NO, it will never cause problems when called from baz.

       In baz, `foo y xs` is called where y is an element taken from xs
       via pattern matching.  Since y ∈ xs, foo will always find y before
       reaching the empty list.  The [] case is unreachable from baz's
       call sites.

    *)

    // foo2 — fixed version that handles the empty list case gracefully.
    //
    // When x is not found ([] reached), we return [] rather than crashing.
    // This is a safe behaviour change for the "not found" case.
    let rec foo2 (x : 'a) : 'a list -> 'a list = function
        | []                 -> []
        | y :: ys when x = y -> ys
        | y :: ys            -> y :: (foo2 x ys)

(* Question 2.3 *)

    (*
    In the function baz there is a sub expression foo y >> baz >> bar y

    Q: What is the type of this expression

    A: foo y   : 'a list -> 'a list
       baz     : 'a list -> 'a list list
       bar y   : 'a list list -> 'a list list

       Composing with `>>` (left-to-right function composition):
       (foo y >> baz)        : 'a list -> 'a list list
       (foo y >> baz >> bar y): 'a list -> 'a list list

       So the overall type is: 'a list -> 'a list list


    Q: What does it do? Focus on what it does rather than how it does it.

    A: Given a list xs, the composed function:
       1. Removes the first occurrence of y from xs (foo y xs).
       2. Generates all permutations of the shortened list (baz).
       3. Prepends y to every permutation (bar y).

       The result is all permutations of xs that START WITH y.

    *)

(* Question 2.4 *)

    // bar2 — non-recursive version of bar using List.map.
    //
    // bar x xss = List.map (fun xs -> x :: xs) xss
    // Both prepend x to every sublist; List.map replaces explicit recursion.
    let bar2 (x : 'a) (xss : 'a list list) : 'a list list =
        List.map (fun xs -> x :: xs) xss

(* Question 2.5 *)

    // baz2 — version of baz using bar2 instead of bar.
    //
    // Structurally identical to baz; the only change is replacing `bar y` with
    // `bar2 y` in the pipeline.  bar2 uses List.map internally rather than
    // explicit recursion.
    let rec baz2 : 'a list -> 'a list list = function
        | []  -> []
        | [x] -> [[x]]
        | xs  ->
            let rec aux = function
                | []      -> []
                | y :: ys -> ((foo y >> baz2 >> bar2 y) xs) @ (aux ys)
            aux xs

(* Question 2.6 *)

    (*

    Q: The function foo is not tail recursive. Why?

    A: Evaluate foo 1 [2;1;3]:

         foo 1 [2;1;3]
       = 2 :: (foo 1 [1;3])
               ^^^^^^^^^^^
               recursive call is the ARGUMENT to `::`.
               After `foo 1 [1;3]` returns [3], we still need to prepend 2.
               The current frame must stay alive to perform that prepend.

       = 2 :: ([3])     (* x=1 found at head of [1;3] → return [3] *)
       = [2;3]

       The `::` operator has PENDING work after the recursive call, which
       means each call frame stays on the stack.  A list of length n can
       cause up to n stack frames — not tail-recursive.

    *)

    // fooTail — tail-recursive version of foo using an accumulator.
    //
    // Elements before x are collected in `acc` (in reverse order).
    // When x is found, we combine:  List.rev acc @ ys  (elements after x).
    // When the list is empty (x not found), we return List.rev acc (original list minus x... which means x was absent; we return unchanged equivalent).
    //
    // The call `aux (y :: acc) ys` is the last thing done — tail position. ✓
    let fooTail (x : 'a) (lst : 'a list) : 'a list =
        let rec aux acc = function
            | []                 -> List.rev acc
            | y :: ys when x = y -> List.rev acc @ ys
            | y :: ys            -> aux (y :: acc) ys
        aux [] lst


(* 3: Rock Paper Scissors *)

(* Question 3.1 *)

    type shape  = Rock | Paper | Scissors
    type result = Win | Lose | Draw

    // rps s1 s2 — determine the result for player 1 (s1) playing against player 2 (s2).
    //
    // RULES:
    //   Rock beats Scissors, Paper beats Rock, Scissors beats Paper.
    //   Same shape → Draw.
    //
    // We list all winning combinations for s1 explicitly;
    // equal shapes give Draw; everything else is Lose.
    let rps (s1 : shape) (s2 : shape) : result =
        match s1, s2 with
        | Rock, Rock | Paper, Paper | Scissors, Scissors -> Draw
        | Rock, Scissors | Paper, Rock | Scissors, Paper -> Win
        | _                                              -> Lose

(* Question 3.2 *)

    // strategy — given the history of (own_move, opponent_move) pairs,
    // return the next move.  History is most-recent first.
    type strategy = (shape * shape) list -> shape

    // parrot — copy the opponent's last move.
    // On the first move (empty history), default to Rock.
    let parrot : strategy = function
        | []           -> Rock
        | (_, opp) :: _ -> opp

    // beatingStrat — play the shape that BEATS the opponent's last move.
    // Counters Rock with Paper, Paper with Scissors, Scissors with Rock.
    let beatingStrat : strategy = function
        | []           -> Rock
        | (_, opp) :: _ ->
            match opp with
            | Rock     -> Paper
            | Paper    -> Scissors
            | Scissors -> Rock

    // roundRobin — cycle through Rock → Paper → Scissors → Rock → ...
    // Uses the number of rounds played so far (mod 3) as the index.
    let roundRobin : strategy = fun history ->
        match List.length history % 3 with
        | 0 -> Rock
        | 1 -> Paper
        | _ -> Scissors

(* Question 3.3 *)

    (*

    Q: It may be tempting to generate a function that calculates your
       point tuple after n rounds and then use Seq.initInfinite to
       generate the sequence. This is not a good solution. Why?

    A: Seq.initInfinite f creates the sequence [f 0; f 1; f 2; ...] where
       f takes the index and returns the element.

       For a game score after n rounds, to compute f n you need to:
         - replay all n rounds from the start to accumulate the score.
       This means computing f n requires O(n) work from scratch.

       Generating the first N elements would cost 1 + 2 + ... + N = O(N²) total.

       Seq.unfold is better: it carries the CURRENT STATE (history, scores)
       forward from one round to the next.  Each step is O(1) (just play one
       move and update scores), so generating N elements costs O(N).

    *)

    // bestOutOf strat1 strat2 — infinite sequence of running scores (p1, p2).
    //
    // State: (history, p1_score, p2_score)
    // Each step: play one round, update history and scores, emit (p1, p2).
    //
    // Seq.unfold carries state forward — O(1) per element.
    let bestOutOf (strat1 : strategy) (strat2 : strategy) : (int * int) seq =
        Seq.unfold (fun (history, p1, p2) ->
            let move1 = strat1 history
            let move2 = strat2 history
            let newHistory = (move1, move2) :: history
            let (p1', p2') =
                match rps move1 move2 with
                | Win  -> (p1 + 1, p2)
                | Lose -> (p1, p2 + 1)
                | Draw -> (p1, p2)
            Some ((p1', p2'), (newHistory, p1', p2'))
        ) ([], 0, 0)

(* Question 3.4 *)

    // playTournament strats n — run a round-robin tournament between all strategies.
    //
    // Each strategy plays n rounds against every other strategy.
    // Returns the strategies sorted by total wins (most wins first).
    //
    // For each pair (s1, s2), we get n rounds from bestOutOf and take the score
    // at round n-1 (0-indexed).  We add p1's wins from that game to s1's total.
    let playTournament (strats : strategy list) (n : int) : (int * strategy) list =
        strats
        |> List.map (fun s1 ->
            let wins =
                strats
                |> List.filter (fun s2 -> s2 <> s1)
                |> List.sumBy (fun s2 ->
                    let results = bestOutOf s1 s2
                    fst (Seq.item (n - 1) results))
            (wins, s1))
        |> List.sortByDescending fst


(* 4: Reverse Polish Notation *)

(* Question 4.1 *)

    // DESIGN CHOICE: the stack is an immutable int list.
    //   - Head of the list = top of the stack.
    //   - push/pop are O(1) list cons/head operations.
    //   - Immutability fits cleanly with the option-state monad below.
    type stack = int list

    let emptyStack : stack = []

(* Question 4.2 *)

    type SM<'a> = S of (stack -> ('a * stack) option)

    let ret x = S (fun s -> Some (x, s))
    let fail  = S (fun _ -> None)
    let bind f (S a) : SM<'b> =
        S (fun s ->
            match a s with
            | Some (x, s') ->
                let (S g) = f x
                g s'
            | None -> None)

    let (>>=) x f = bind f x
    let (>>>=) x y = x >>= (fun _ -> y)

    let evalSM (S f) = f emptyStack

    // push x — monadic: push integer x onto the stack.
    // The new stack is x :: s.  Always succeeds (returns unit).
    let push (x : int) : SM<unit> =
        S (fun s -> Some ((), x :: s))

    // pop — monadic: remove and return the top element of the stack.
    // Empty stack → None (monadic failure = stack underflow).
    let pop : SM<int> =
        S (fun s ->
            match s with
            | []      -> None
            | x :: s' -> Some (x, s'))

(* Question 4.3 *)

    let write str : SM<unit> = S (fun s -> printf "%s" str; Some ((), s))

    let read =
        let rec aux acc =
            match System.Console.Read() |> char with
            | '\n' when acc = [] -> None
            | c    when System.Char.IsWhiteSpace c ->
                acc |> List.fold (fun strAcc ch -> (string ch) + strAcc) "" |> Some
            | c -> aux (c :: acc)

        S (fun s -> Some (aux [], s))

    (*

    Q: Consider the definition of write. There is a reason that the definition
       is S (fun s -> printf "%s" str; Some ((), s)) and not just
       ret (printf "%s" str). For a similar reason, in read, we write
       S (fun s -> Some (aux [], s)) and not ret (aux []).
       What is the problem with using ret in both of these cases?

    A: `ret x` = `S (fun s -> Some (x, s))`.
       If we wrote `ret (printf "%s" str)`, the printf executes IMMEDIATELY
       when `write str` is called — at the moment the monadic value is CONSTRUCTED,
       not when it is run.  This breaks the monadic model: side effects should
       only happen when the state machine actually executes the action.

       Correct: `S (fun s -> printf "%s" str; Some((), s))` wraps the printf
       inside the function, so it only runs when the SM is executed.

       Similarly for read: `ret (aux [])` would immediately block on stdin
       during construction of the monadic value.  Wrapping in
       `S (fun s -> Some (aux [], s))` defers the read to execution time.

    *)

(* Question 4.4 *)

    (* You may solve this exercise either using monadic operators or
        using computational expressions. *)

    type StateBuilder() =
        member this.Bind(f, x)    = bind x f
        member this.Return(x)     = ret x
        member this.ReturnFrom(x) = x
        member this.Combine(a, b) = a >>= (fun _ -> b)

    let state = new StateBuilder()

    // calculateRPN s — evaluate a Reverse Polish Notation expression string.
    //
    // TOKENS: numbers (push) and operators +, -, * (pop two, push result).
    // The final value is the top of the stack after all tokens are processed.
    //
    // The state monad threads the stack; pop failures return None.
    //
    // Example: calculateRPN "3 4 + 2 *"
    //   push 3 → [3]
    //   push 4 → [4;3]
    //   +      → pop 4, pop 3, push 7 → [7]
    //   push 2 → [2;7]
    //   *      → pop 2, pop 7, push 14 → [14]
    //   result → Some 14
    let calculateRPN (s : string) : int option =
        let tokens =
            s.Split([|' '|], System.StringSplitOptions.RemoveEmptyEntries)
            |> Array.toList
        let rec run toks =
            state {
                match toks with
                | [] -> return! pop
                | tok :: rest ->
                    match tok with
                    | "+" ->
                        let! a = pop
                        let! b = pop
                        do! push (a + b)
                        return! run rest
                    | "-" ->
                        let! a = pop
                        let! b = pop
                        do! push (b - a)
                        return! run rest
                    | "*" ->
                        let! a = pop
                        let! b = pop
                        do! push (a * b)
                        return! run rest
                    | n ->
                        do! push (int n)
                        return! run rest
            }
        evalSM (run tokens) |> Option.map fst
