module Exam2024

(* If you are importing this into F# interactive then comment out
   the line above and remove the comment for the line bellow.

   Do note that the project will not compile if you do this, but
   it does allow you to work in interactive mode and you can just remove the '='
   to make the project compile again.

   You will also need to load JParsec.fs. Do this by typing
   #load "JParsec.fs"
   in the interactive environment. You may need the entire path.

   Do not remove the module declaration (even though that does work) because you may inadvertently
   introduce indentation errors in your code that may be hard to find when switching back to project mode.

   Alternative, keep the module declaration as is, but load ExamInteractive.fsx into the interactive environment
   *)
(*
 module Exam2024 =
 *)

(* 1: Transactions *)

    // The `transactions` type represents a linked-list-style ledger.
    // Each node is either:
    //   Empty              — end of the ledger
    //   Pay(name, amt, t)  — the named person PAID OUT `amt` (money leaves, balance goes down)
    //   Receive(name, amt, t) — the named person RECEIVED `amt` (money arrives, balance goes up)
    // The third field in Pay/Receive is the TAIL — the rest of the transaction list.
    type transactions =
        | Empty
        | Pay     of string * int * transactions
        | Receive of string * int * transactions

    // Q1.1: balance — computes the net balance of all transactions.
    //
    // HOW IT WORKS:
    //   We pattern-match on the transaction list and recurse.
    //   Pay  -> money went OUT  -> subtract amount  -> negative contribution
    //   Receive -> money came IN  -> add amount  -> positive contribution
    //   Empty -> nothing left -> base case, return 0
    //
    // Trace: balance (Pay("Alice", 500, Receive("Bob", 200, Empty)))
    //      = -500 + balance (Receive("Bob", 200, Empty))
    //      = -500 + (200 + balance Empty)
    //      = -500 + (200 + 0)
    //      = -300
    //
    // WHY NOT TAIL-RECURSIVE:
    //   The recursive call `balance rest` is inside an arithmetic expression
    //   (-amount + balance rest). The addition cannot happen until the recursive call
    //   returns, so each call frame must remain on the stack. O(n) stack depth.
    let rec balance =
        function
        | Empty                    -> 0
        | Pay     (_, amount, rest) -> -amount + balance rest
        | Receive (_, amount, rest) ->  amount + balance rest

    // Q1.2: balanceAcc — tail-recursive version of balance using an accumulator.
    //
    // IDEA: carry the running total in `acc` instead of building it up on the stack.
    //   At each step we update acc (no pending arithmetic!) and tail-call aux.
    //   When we hit Empty we return acc directly — the last thing done IS the recursive call,
    //   so the compiler can reuse the current stack frame (tail-call optimisation).
    //
    // Trace: balanceAcc (Pay("Alice", 500, Receive("Bob", 200, Empty)))
    //      = aux 0 (Pay(...))
    //      = aux (0 - 500) (Receive("Bob", 200, Empty))
    //      = aux (-500) (Receive("Bob", 200, Empty))
    //      = aux (-500 + 200) Empty
    //      = aux (-300) Empty
    //      = -300          <- base case, return acc directly
    let balanceAcc t =
        let rec aux acc =
            function
            | Empty                    -> acc
            | Pay     (_, amount, rest) -> aux (acc - amount) rest
            | Receive (_, amount, rest) -> aux (acc + amount) rest
        aux 0 t

    // Q1.3: participants — returns a PAIR of sets: (payerSet, receiverSet).
    //   payerSet   = all names that appear in a Pay node
    //   receiverSet = all names that appear in a Receive node
    //
    // We use Set<string> from F#'s immutable set module.
    // Set.empty is the empty set; Set.add x s inserts x (duplicates are ignored — sets are unique).
    // Pattern: recurse to Empty (both sets empty), then unwind adding names to the correct set.
    //
    // Example:
    //   Pay("Alice", 500, Receive("Bob", 200, Empty))
    //   participants rest = participants (Receive("Bob", 200, Empty))
    //                     = (Set.empty, {"Bob"})
    //   => add "Alice" to payers => ({"Alice"}, {"Bob"})
    let rec participants =
        function
        | Empty                    -> (Set.empty, Set.empty)
        | Pay     (name, _, rest)  ->
            let (payers, receivers) = participants rest
            (Set.add name payers, receivers)
        | Receive (name, _, rest)  ->
            let (payers, receivers) = participants rest
            (payers, Set.add name receivers)

    // Q1.4: balanceFold — a GENERIC FOLD over the transaction list.
    //
    // Like List.foldBack, it processes nodes from the END back to the front.
    // Two folder functions let the caller handle Pay and Receive differently:
    //   fPay    : string -> int -> 'acc -> 'acc
    //   fReceive: string -> int -> 'acc -> 'acc
    // acc0 is the base value returned for Empty.
    //
    // The signature mirrors the shape of the type:
    //   balanceFold fPay fReceive acc0 Empty             = acc0
    //   balanceFold fPay fReceive acc0 (Pay(n,a,rest))   = fPay n a (balanceFold ... rest)
    //   balanceFold fPay fReceive acc0 (Receive(n,a,rest)) = fReceive n a (balanceFold ... rest)
    //
    // Note: the recursive call is the ARGUMENT to fPay/fReceive, not a tail call —
    // same structure as non-TR balance above.
    let rec balanceFold fPay fReceive acc0 =
        function
        | Empty                     -> acc0
        | Pay     (name, amount, rest) ->
            fPay     name amount (balanceFold fPay fReceive acc0 rest)
        | Receive (name, amount, rest) ->
            fReceive name amount (balanceFold fPay fReceive acc0 rest)

    // Q1.5: collect — builds a Map<string, int> mapping each person to their net balance.
    //
    // Uses balanceFold with two lambdas.
    // The accumulator is a Map<string, int>.
    //
    //   Pay(name, amount)    -> person's balance DECREASES by amount
    //   Receive(name, amount) -> person's balance INCREASES by amount
    //
    // `defaultArg (Map.tryFind name acc) 0`:
    //   Map.tryFind returns Some n if name is already in the map, or None if absent.
    //   defaultArg unwraps the option, using 0 as the default (first time we see this person).
    //
    // Map.add name newBalance acc:
    //   Inserts or REPLACES the entry for `name`. If the name was already in the map,
    //   the old balance is replaced with the updated one.
    let collect t =
        balanceFold
            (fun name amount acc ->
                let cur = defaultArg (Map.tryFind name acc) 0
                Map.add name (cur - amount) acc)
            (fun name amount acc ->
                let cur = defaultArg (Map.tryFind name acc) 0
                Map.add name (cur + amount) acc)
            Map.empty
            t


(* 2: Code Comprehension *)

    let foo (x : char) = x |> int |> fun y -> y - (int '0')

    let bar (x : string) = [for c in x -> c]

    let rec baz =
        function
        | [] -> 0
        | x :: xs -> x + 10 * baz xs

(* Question 2.1 *)

    (*
    Q: What are the types of functions foo, bar, and baz?

    A: foo : char -> int
       bar : string -> char list
       baz : int list -> int

       Derivation:
         foo takes a char (the explicit annotation confirms this) and pipes it through
         `int` (char -> int, gives the ASCII code) then subtracts `int '0'` (48).
         Result type is int.

         bar takes a string x and builds a list comprehension over it.
         [for c in x -> c] iterates over each char c in the string and yields it.
         Result type is char list.

         baz is a recursive function on lists. The base case [] -> 0 gives int.
         The recursive case x :: xs -> x + 10 * baz xs: `x` must be int (added to int),
         so the list element type is int.


    Q: What do the functions foo, bar, and baz do?
       Focus on what they do rather than how they do it.

    A: foo — converts a decimal DIGIT CHARACTER to its integer value.
             e.g. foo '0' = 0,  foo '7' = 7,  foo '9' = 9
             Mechanism: ASCII code of a digit char minus the ASCII code of '0' (which is 48).

       bar — EXPLODES a string into a list of its characters.
             e.g. bar "hello" = ['h';'e';'l';'l';'o']
             Mechanism: [for c in x -> c] iterates over each char in the string in order.

       baz — interprets an int list as a LITTLE-ENDIAN decimal number.
             The FIRST element is the LEAST significant digit.
             e.g. baz [3;2;1] = 3 + 10*2 + 100*1 = 123
             e.g. baz [1;2;3] = 1 + 10*2 + 100*3 = 321   <- note: NOT 123!
             Mechanism: each recursive call multiplies the "rest" by 10 and adds the current digit.


    Q: What would be appropriate names for foo, bar, and baz?

    A: foo  -> digitCharToInt   (converts a digit char to its numeric value)
       bar  -> stringToCharList / explode   (splits a string into individual characters)
       baz  -> listToIntLittleEndian / fromDigitsLE  (reads a digit list as a little-endian integer)


    Q: foo only behaves reasonably if certain constraint(s) are met on its argument.
       What is/are these constraints?

    A: x must be a decimal digit character: '0' <= x <= '9'.
       If x is outside this range, the subtraction still executes without error but yields
       a meaningless result. For example:
         foo 'a' = int 'a' - int '0' = 97 - 48 = 49  (not a digit 0-9)
         foo ' ' = 32 - 48 = -16                      (negative, not a digit)


    Q: baz only behaves reasonably if certain constraint(s) are met on its argument.
       What is/are these constraints?

    A: Every element of the list must be a valid decimal digit: 0 <= each element <= 9.
       Elements outside this range make the result meaningless as a decimal number
       (e.g. a digit value of 15 would produce an invalid "carry" into the next position).
    *)


(* Question 2.2 *)

    // stringToInt — converts a decimal string like "321" to the integer 321.
    //
    // PIPELINE:
    //   "321"
    //   |> bar          => ['3';'2';'1']     string -> char list, left to right
    //   |> List.map foo => [3;2;1]           each char -> its digit int
    //   |> List.rev     => [1;2;3]           CRITICAL STEP — explained below
    //   |> baz          => 1 + 10*2 + 100*3 = 321
    //
    // WHY List.rev?
    //   baz reads the FIRST element as the LEAST significant digit (little-endian).
    //   But in the string "321", '3' is the MOST significant digit (big-endian, like normal writing).
    //   After bar+map we have [3;2;1] where 3 is at position 0 (most significant).
    //   baz [3;2;1] would give 3 + 20 + 100 = 123 — WRONG!
    //   After List.rev we have [1;2;3] where 1 is at position 0 (least significant).
    //   baz [1;2;3] = 1 + 20 + 300 = 321 — CORRECT.
    let stringToInt (s : string) =
        s |> bar |> List.map foo |> List.rev |> baz


(* Question 2.3 *)

    // baz2 — re-implements baz using a higher-order function instead of explicit recursion.
    //
    // INSIGHT: baz processes from the RIGHT (the last element contributes the highest power of 10).
    // This is exactly what List.foldBack does: it applies the combining function starting from
    // the RIGHTMOST element and working left.
    //
    // List.foldBack f [x1;x2;x3] z
    //   = f x1 (f x2 (f x3 z))
    //   = f x1 (f x2 (x3 + 10*0))
    //   = f x1 (x2 + 10*x3)
    //   = x1 + 10*(x2 + 10*x3)
    //
    // Compare: baz [x1;x2;x3] = x1 + 10*(x2 + 10*(x3 + 10*0))   <- identical structure!
    //
    // The combining function is (fun x acc -> x + 10 * acc), zero value is 0.
    let baz2 lst = List.foldBack (fun x acc -> x + 10 * acc) lst 0


(* Question 2.4 *)

    (*
    Q: The function baz from Question 2.1 is not tail recursive. Demonstrate why.
       (Note: the template says "bar" but bar is a non-recursive list comprehension;
        the question is clearly about baz, which has explicit recursion and pending arithmetic.)

    A: Evaluate baz [1;2;3] step by step:

       baz [1;2;3]
       = 1 + 10 * baz [2;3]
            ^^^^^^^^^^^^^^^^^
            The recursive call baz [2;3] is NESTED INSIDE the expression (1 + 10 * ...).
            We cannot discard the current stack frame because we still need to
            multiply the result by 10 and add 1 once the call returns.

       = 1 + 10 * (2 + 10 * baz [3])
            The frame for baz [2;3] is also stuck — needs to multiply baz [3] by 10 and add 2.

       = 1 + 10 * (2 + 10 * (3 + 10 * baz []))
            Third recursive level — same situation.

       = 1 + 10 * (2 + 10 * (3 + 10 * 0))    <- baz [] = 0, base case reached
       = 1 + 10 * (2 + 10 * 3)
       = 1 + 10 * 32
       = 321

    Why NOT tail recursive:
    At each step the recursive call `baz xs` is the ARGUMENT to multiplication, not the
    final result. The calling frame must survive the recursive call so it can compute
    `x + 10 * (result_of_recursive_call)`.
    A tail call would be one where the return value of the recursive call IS directly
    returned — no further computation after it. Here there is always pending arithmetic,
    so 3 call frames are alive simultaneously for a 3-element list. Stack depth = list length.
    *)


(* Question 2.5 *)

    // bazTail — tail-recursive baz using Continuation-Passing Style (CPS).
    //
    // THE PROBLEM WITH baz: the pending `x + 10 * (...)` arithmetic keeps stack frames alive.
    //
    // CPS SOLUTION: instead of returning a value up the call chain, we pass a CONTINUATION
    // function `cont` that represents "what to do with the final answer".
    // The pending arithmetic (x + 10 * acc) is folded INTO the continuation closure,
    // which lives on the HEAP rather than the stack.
    //
    // TRACE: bazTail [1;2;3]
    //   = aux id [1;2;3]
    //   = aux (fun acc -> id (1 + 10 * acc)) [2;3]            -- cont grows
    //   = aux (fun acc -> (fun acc -> id (1 + 10*acc)) (2 + 10*acc)) [3]
    //   = aux (fun acc -> (fun acc -> (fun acc -> id (1+10*acc)) (2+10*acc)) (3+10*acc)) []
    //   = (outermost cont) 0                   -- base case: apply cont to 0
    //   = ... evaluates inward ... = 321
    //
    // The recursive call `aux newCont xs` IS the last thing done — tail position. ✓
    //
    // Start with `id` (identity function): the initial "thing to do with the result" is
    // just return it as-is.
    let bazTail lst =
        let rec aux cont =
            function
            | []      -> cont 0
            | x :: xs -> aux (fun acc -> cont (x + 10 * acc)) xs
        aux id lst


(* 3: Caesar Ciphers *)

(* Question 3.1 *)

    // encrypt — applies a Caesar cipher shift of `offset` positions to a string.
    //
    // Only LOWERCASE letters ('a'-'z') are shifted; all other characters
    // (spaces, punctuation, uppercase) pass through unchanged.
    //
    // SHIFT FORMULA (for a lowercase letter c):
    //   new_char = char ( int 'a'  +  (int c - int 'a' + offset) % 26 )
    //
    //   int c - int 'a'        => 0-based letter index: 'a'->0, 'b'->1, ..., 'z'->25
    //   + offset               => apply the shift
    //   % 26                   => wrap around: stays within 0-25
    //                             also handles offset > 25 (e.g. offset=27 same as offset=1)
    //   int 'a' + (...)        => shift back to ASCII range
    //   char (...)             => convert the int back to a char
    //
    // Example: encrypt "hello world" 3
    //   'h'(7) -> (7+3)%26=10 -> 'k'
    //   'e'(4) -> (4+3)%26=7  -> 'h'
    //   'l'(11)-> (11+3)%26=14-> 'o'   (twice)
    //   'o'(14)-> (14+3)%26=17-> 'r'
    //   ' '   -> unchanged
    //   'w'(22)-> (22+3)%26=25-> 'z'
    //   'o','r','l','d' -> 'r','u','o','g'
    //   Result: "khoor zruog"
    //
    // String.map applies the char->char function to every character in the string.
    let encrypt (s : string) (offset : int) : string =
        s |> String.map (fun c ->
            if c >= 'a' && c <= 'z' then
                char (int 'a' + (int c - int 'a' + offset) % 26)
            else c)

(* Question 3.2 *)

    // decrypt — reverses the Caesar cipher. Given an encrypted string and the original
    // offset, returns the plaintext.
    //
    // REASONING: encryption shifts each letter forward by `offset` (mod 26).
    // To undo that shift, we shift BACKWARD by `offset`, which is the same as
    // shifting FORWARD by (26 - offset % 26), since going forward 26 - k positions
    // is the same as going backward k positions in a 26-letter alphabet.
    //
    //   offset % 26 normalises the offset first (e.g. offset=26 -> complement=26-0=26, and
    //   26 mod 26 = 0, meaning no shift — correct).
    //   offset=3  -> complement = 23: 'k'->'h', 'h'->'e', 'o'->'l', 'r'->'o' ✓
    //
    // decrypt is therefore just encrypt with the complementary offset.
    let decrypt (s : string) (offset : int) : string =
        encrypt s (26 - offset % 26)

(* Question 3.3 *)

    // decode — finds the encryption offset given a known plaintext / ciphertext pair.
    //
    // APPROACH: a Caesar cipher has only 26 possible offsets (0 through 25).
    // We try every one and return the first offset for which
    // `encrypt plainText offset = encryptedText`.
    //
    // List.tryFind f [0..25]:
    //   Tries each element of [0;1;2;...;25] in order, applying predicate f.
    //   Returns Some(offset) for the FIRST match, or None if none found.
    //
    // For a valid Caesar cipher pair, exactly one offset in 0..25 will match.
    // (Offset 0 means "no shift" — both texts are identical.)
    let decode (plainText : string) (encryptedText : string) : int option =
        List.tryFind (fun offset -> encrypt plainText offset = encryptedText) [0..25]

(* Question 3.4 *)

    // parEncrypt — encrypts a string IN PARALLEL, one word at a time.
    //
    // STEPS:
    //   1. s.Split([|' '|])
    //      Splits the string at every space into an array of word strings.
    //      e.g. "hello world" -> [|"hello"; "world"|]
    //
    //   2. Array.map (fun word -> async { return encrypt word offset })
    //      For each word, create an ASYNC COMPUTATION that encrypts it.
    //      `async { return x }` is an F# async workflow — it represents a
    //      lazy computation that can be scheduled to run concurrently.
    //      This step creates the array of workflows WITHOUT executing them yet.
    //
    //   3. Async.Parallel
    //      Bundles the array of async workflows into a SINGLE async workflow
    //      that runs ALL of them concurrently and collects their results into an array.
    //
    //   4. Async.RunSynchronously
    //      EXECUTES the combined async workflow, blocking the current thread until
    //      all parallel encryptions are done. Returns the array of encrypted words.
    //
    //   5. String.concat " "
    //      Joins the encrypted words back together with spaces between them.
    let parEncrypt (s : string) (offset : int) : string =
        s.Split([|' '|])
        |> Array.map (fun word -> async { return encrypt word offset })
        |> Async.Parallel
        |> Async.RunSynchronously
        |> String.concat " "

(* Question 3.5 *)

    open JParsec.TextParser

    // parseEncrypt — a JParsec parser that reads a run of LOWERCASE letters and
    // returns the encrypted result as a string.
    //
    // PARSER COMBINATORS USED:
    //
    //   satisfy (fun c -> c >= 'a' && c <= 'z')
    //     A PRIMITIVE PARSER that consumes exactly ONE character from the input IF
    //     the predicate holds. Fails (without consuming input) otherwise.
    //     This parser succeeds on 'a'-'z' only — uppercase letters, spaces, digits, etc. cause it to fail.
    //
    //   many p
    //     Runs parser p ZERO OR MORE TIMES, collecting each success into a char list.
    //     Stops the moment p fails (e.g. the next character is a space or uppercase).
    //     `many` never fails — it returns [] if p fails immediately.
    //
    //   |>> f   (pipe-into / fmap)
    //     Transforms the successful parse result using function f.
    //     Here: takes the char list produced by `many` and converts it:
    //       Array.ofList chars  => char list -> char array (needed by System.String constructor)
    //       System.String(...)  => char array -> string
    //       encrypt s offset    => encrypt the parsed string with the given offset
    //
    // NOTE: uppercase letters STOP the parser (they don't match 'a'-'z').
    // This differs from 2025re which explicitly lowercased with Char.ToLower.
    let parseEncrypt (offset : int) : Parser<string> =
        many (satisfy (fun c -> c >= 'a' && c <= 'z')) |>>
        (fun chars ->
            let s = System.String(Array.ofList chars)
            encrypt s offset)


(* 4: Letterboxes *)

(* Question 4.1 *)

    // letterbox — persistent map from SENDER NAMES to their MESSAGE QUEUES.
    //
    // DESIGN CHOICE: Map<string, string list>
    //   key   = sender name (string)
    //   value = list of messages from that sender, stored OLDEST FIRST
    //           (head of list = next message to be read)
    //
    // RATIONALE:
    //   Map<string, ...> gives O(log n) lookup and update by sender name.
    //   A simple list is the easiest queue: reading takes the head (O(1)),
    //   posting appends to the tail using @ (O(n_messages_per_sender)).
    //   Oldest-first ordering means the reader always gets messages in chronological order.
    type letterbox = Map<string, string list>

    // empty — returns a fresh, empty letterbox.
    // Takes unit () as an argument rather than being a value, which is idiomatic for
    // "factory functions" in F# — especially useful if the type were mutable.
    let empty () : letterbox = Map.empty


(* Question 4.2 *)

    // post sender message lb — returns a new letterbox with `message` added to sender's queue.
    //
    // STEPS:
    //   Map.tryFind sender lb  => Some(existingMsgs) if sender already has a queue, else None
    //   defaultArg ... []      => unwrap the option, using [] as default for first message
    //   msgs @ [message]       => APPEND the new message to the END (oldest-first: new messages go last)
    //   Map.add sender ... lb  => insert/replace the updated list in the map
    //
    // NOTE: Map is IMMUTABLE. Map.add returns a NEW map with the entry added/updated.
    // The original `lb` is unmodified.
    let post (sender : string) (message : string) (lb : letterbox) : letterbox =
        let msgs = defaultArg (Map.tryFind sender lb) []
        Map.add sender (msgs @ [message]) lb

    // read sender lb — returns the OLDEST message from sender and the updated letterbox.
    //
    // Pattern matching:
    //   None      => sender never posted anything (key absent from map)
    //   Some []   => sender's queue is empty (all messages already read)
    //   Some (msg :: rest) => take msg (the oldest, i.e. head), keep rest in the map
    //
    // Raises a runtime exception if no messages exist.
    // (The monadic read2 below handles this gracefully by returning None instead.)
    let read (sender : string) (lb : letterbox) : string * letterbox =
        match Map.tryFind sender lb with
        | None | Some []           -> failwith (sprintf "No messages from %s" sender)
        | Some (msg :: rest)       -> (msg, Map.add sender rest lb)


(* Question 4.3 *)

    // The state monad wraps an OPTION — computations can FAIL with None.
    // SM of (letterbox -> ('a * letterbox) option)
    //   - Takes the current letterbox as input (the "state")
    //   - Returns Some(value, newLetterbox) on success
    //   - Returns None on failure (e.g. reading from an empty queue)
    type StateMonad<'a> = SM of (letterbox -> ('a * letterbox) option)

    let ret x = SM (fun s -> Some (x, s))
    let fail  = SM (fun _ -> None)

    // bind — sequences two state computations.
    // Run (SM a) on the current state s.
    // If it returns Some(x, s'), pass x to f and run the result on s'.
    // If it returns None (failure), short-circuit and return None.
    // This makes the monad "fail fast": once any step fails, the whole chain fails.
    let bind f (SM a) : StateMonad<'b> =
        SM (fun s ->
            match a s with
            | Some (x, s') -> let (SM g) = f x
                              g s'
            | None -> None)

    let (>>=)  x f = bind f x
    let (>>>=) x y = x >>= (fun _ -> y)

    // evalSM — runs a monadic computation starting from an EMPTY letterbox.
    // Returns (value, finalLetterbox) option.
    let evalSM (SM f) = f (empty ())

    // post2 sender message — monadic version of post.
    //
    // Wraps the pure `post` function in the state monad:
    //   - `fun lb` captures the current letterbox state
    //   - `post sender message lb` computes the new letterbox (pure, no option needed here)
    //   - Some ((), newLb) wraps as success; the value is unit (posting returns nothing)
    //
    // post2 never fails — posting always succeeds.
    let post2 (sender : string) (message : string) : StateMonad<unit> =
        SM (fun lb -> Some ((), post sender message lb))

    // read2 sender — monadic version of read.
    //
    // Unlike the direct `read` which raises an exception, read2 uses MONADIC FAILURE (None)
    // when no messages are available. This lets the bind chain short-circuit cleanly.
    //
    //   None or Some []    => return None  (no messages -> monadic fail)
    //   Some (msg :: rest) => return Some (msg, updatedLetterbox)  (success)
    //
    // The updated letterbox has the message removed from sender's queue.
    let read2 (sender : string) : StateMonad<string> =
        SM (fun lb ->
            match Map.tryFind sender lb with
            | None | Some []     -> None
            | Some (msg :: rest) -> Some (msg, Map.add sender rest lb))


(* Question 4.4 *)

    // StateBuilder — F# computation expression (CE) builder for the state monad.
    //
    // Enables writing monadic code using let!, do!, return, return! syntax.
    //
    // IMPORTANT: Bind parameters are SWAPPED compared to the standard convention.
    //   F# calls: this.Bind(monad, continuation)
    //   But the signature is: Bind(f, x) = bind x f
    //   So: f = monad, x = continuation
    //   Result: bind continuation monad  — which is correct semantics.
    //
    // Return(x)     = ret x              wraps a plain value in the monad
    // ReturnFrom(m) = m                  re-returns a value already in the monad (return!)
    // Combine(a, b) = a >>= (fun _ -> b) sequences two monadic computations, discarding first result
    type StateBuilder() =
        member this.Bind(f, x)    = bind x f
        member this.Return(x)     = ret x
        member this.ReturnFrom(x) = x
        member this.Combine(a, b) = a >>= (fun _ -> b)

    let state = StateBuilder()

    type MType =
        | Post of string * string   // Post(senderName, messageContent)
        | Read of string            // Read(senderName) — read next message from this sender
    type log = MType list

    // trace — processes a log of Post/Read entries using the state monad,
    // returning Some(readResults) if all operations succeed, or None if any Read fails.
    //
    // THE COMPUTATION:
    //   We define a recursive helper `aux` that builds a StateMonad<string list>.
    //   For each entry in the log:
    //
    //   [] (empty log):
    //     return []  (no reads, empty result list)
    //
    //   Post(sender, msg) :: rest:
    //     do! post2 sender msg    => post the message (state updated, unit result discarded)
    //     return! aux rest        => continue processing the rest of the log
    //
    //   Read(sender) :: rest:
    //     let! msg  = read2 sender   => read next message; if None, ENTIRE trace returns None
    //     let! msgs = aux rest       => process remaining log entries
    //     return msg :: msgs         => prepend this message to the collected results
    //
    // evalSM (aux l):
    //   Runs the computation on an empty letterbox.
    //   Result is (string list * letterbox) option.
    //   Option.map fst extracts just the string list (discards the final letterbox state).
    let trace (l : log) : string list option =
        let rec aux entries =
            match entries with
            | [] -> ret []
            | Post (sender, msg) :: rest ->
                state {
                    do! post2 sender msg
                    return! aux rest
                }
            | Read sender :: rest ->
                state {
                    let! msg  = read2 sender
                    let! msgs = aux rest
                    return msg :: msgs
                }
        evalSM (aux l) |> Option.map fst
