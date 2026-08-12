module Exam2026_Template.Exam

    open JParsec.TextParser
    
    (* Question 1: Tribonacci numbers (25%) *)
    (* Question 1.1 *)
    let rec tribonacci (n: int) = 
        match n with
        | 0 -> 0
        | 1 -> 0
        | 2 -> 1
        | n -> tribonacci (n-1) + tribonacci (n-2) + tribonacci (n-3)


        
    (* Question 1.2 *)
    let tribonacci_acc (n: int) = 
            let rec aux acc0 acc1 acc2 n =
                if n = 0 then acc0 
                elif n = 1 then acc1
                elif n = 2 then acc2
                else aux (acc1) (acc2) (acc2 + acc1 + acc0) (n-1)
            aux 0 0 1 n

    (* Question 1.3 *)
    let approx (n: int) : float = 
        let a = (tribonacci_acc (n))
        let b = (tribonacci_acc (n-1))
        float(a)/float(b) 

               
    (* Question 1.4 *)
    let tau = 1.839286755

    let approx_steps_needed (epsilon: float) : int = 
        let rec aux n =
            if abs (tau - approx n) < epsilon then n 
            else aux (n+1)
        aux 2
        
    (* Question 1.5 *)
    let tribonacci_seq : seq<int> = 
        Seq.unfold (fun (a, b, c) -> Some (a, (b, c, (a+b+c)))) (0, 0, 1)



    (* Question 2: Code comprehension (25%) *)        
    let foo =
        let rec aux n =
            function
            | []        -> (n, [])
            | ' ' :: xs -> (n, xs)
            | _   :: xs -> aux (n + 1) xs
        aux 0
          
    let rec bar a =
        match foo a with
        | x, [] -> [x]
        | x, xs -> x :: bar xs
        
    let baz (str : string) = [for c: char in str -> c] |> bar
    
    (* Question 2.1 *)
    (*
     
     Q: What do the functions `foo`, `bar`, and `baz` do? Focus on what they do rather than how they do it.
     A: 
        foo is a function that takes a char list and returns a new list with the first n chars removed (all chars until and including the first whitespace ' '), as well as n, in the format (n, newlist)
        foo can be wiewed as a function that removes the first 'word' (n chars until whitespace) from a list, as well as provides n, the ammount of non-whitespace chars removed.

        bar is a functin that uses foo to take a char list and returns a new list where every entry is the ammount of chars before each whitespace ' ', as well as the ammount of non-whitespace chars the list ends with. 
        bar can be viewed as a function that counts the length of each whitespace-seperated combination of chars in a list of chars.

        baz is a function that takes a string, transforms it to a char list, and feeds it to bar. 
        The total result of this is a return int list of the ammount of chars before each white-space, AKA a list with the length of each word in the string. 

     Q: What would be appropriate names for functions `foo`, `bar`, and `baz`.
     A:
        foo: remove_word_give_count
        bar: count_chars_before_whitespaces (OR charlist_to_word_length_count)
        baz: word_length_count
     
     Q: The `bar` function matches on `x, []` and `x, xs`. What is the type of the thing that we are matching on?
     A: bar matches on a touple int * list<char> 
    *)
        
    (* Question 2.2 *)



    (*
    Q: The function `foo` is a function and as such takes an argument. However, the declaration of `foo` does not make
       this argument explicit (it says `let foo = ...` rather than `let foo a = ...`). Why does this still work?
       Where in the declaration of `foo` can we see that it takes an argument even though one is not explicitly
       declared?
       
    A: 
        foo takes an argument list<char> as a result of having an internal function which pattern-matches with the function keyword - this is where we can see that it takes the argument.
        This function keyword is the practical equivelant of writing "fun lst -> match lst with ..." or in this case: 
        "let rec aux lst n = 
            match lst with
            | [] -> ..."
        As a result of the pattern-matching, the f# compiler can infer that the function takes a char list. 

    *)

    (* Question 2.3 *)

    let baz2 (str : string) = str.Split(' ') |> List.ofArray |> List.map (fun str -> str.Length)

    (* Question 2.4 *)
    
    (*
      Q: One of the functions from Q2.1 is not tail recursive.
      Explain which one and why. To make a compelling argument you must evaluate
      a function call of the function, similarly to what is done in
      Chapter 1.4 of HR, and reason about that evaluation. You need to make clear
      what aspects of the evaluation tell you that the function is not tail recursive.
      Keep in mind that all steps in an evaluation chain must evaluate to the
      same value (```(5 + 4) * 3 --> 9 * 3 --> 27```, for instance).
      
      A: bar is not tail recursive. 
      let us evaluate bar ['a'; 'b'; ' '; 'c'; ' '; 'd']

        bar ['a'; 'b'; ' '; 'c'; ' '; 'd']
        --> foo ['a'; 'b'; ' '; 'c'] returns (2, ['c'; ' '; 'd']), so we match on second branch
            2 :: (bar ['c'; ' '; 'd'])
        --> foo ['c'; ' '; 'd'] returns (1, ['d']) so we match on the second branch
            2 :: (1 :: (bar ['d']))
        --> foo ['d'] returns (1, []) so we match on the first branch
            2 :: (1 :: [1])
        --> 2 :: ([1; 1])
        --> [2; 1; 1]

        
        As we can see, the stack gets build up as the bar calls itself, first with '2 ::' and then with '1 ::' 
        These parts still need to be resolved, but they have to wait on the stack for the base case to hit, so that the resolution can begin (they have a list to append to). 

    *)
    
    (* Question 2.5 *)
    
    let cont (a: char list) = 
        let rec barC a c =
            match foo a with 
            | x, [] -> c [x]
            | x, xs -> barC xs (fun acc -> c (x :: acc))
        barC a id



    (* Question 3: A rotating cipher (25%) *)
    
    let explode (str : string) = [for c in str -> c]  

    let implode (cs : char list) = cs |> Array.ofList |> System.String  

    let isLetter (c : char) = "abcdefghijklmnopqrstuvwxyz".Contains c
        
    (* Question 3.1 *)

    let next (n: int) (c: char)  = 
        let rec aux (c: char) (n: int) =
            match c, n with 
            | _, 0 -> c
            | c, n when c <> 'z' -> aux (char (int c + 1)) (n-1)
            | c, n when c = 'z' -> aux 'a' (n-1)
            | c, n -> aux c (n-1)
        if isLetter c && (n >= 0) then aux c n else c
    


    (* Question 3.2 *)
    
    let prev (n: int) (c: char) = 
        let rec aux (c: char) (n: int) =
            match c, n with 
            | _, 0 -> c
            | c, n when c <> 'a' -> aux (char (int c - 1)) (n-1)
            | c, n when c = 'a' -> aux 'z' (n-1)
            | c, n -> aux c (n-1)
        if isLetter c && (n >= 0) then aux c n else c
    

    (* Question 3.3 *)

    let encode (str: string) : string = 
        let rec aux (chars: char list) (n: int) : char list =
            match chars with 
            | [] -> []
            | c :: cs -> next n c :: aux cs (n+1)
        implode (aux (explode str) 0)
        
    let decode (str: string) : string = 
        let rec aux (chars: char list) (n: int) : char list =
            match chars with 
            | [] -> []
            | c :: cs -> prev n c :: aux cs (n+1)
        implode (aux (explode str) 0)
        
    (* Question 3.4 *)    
    
    let pencode = pstring "not implemented"
    
    (* Question 3.5 *)
        
    let compose_words (words : string list) : string =
        String.concat " " words



    //You recommend that i do 'str.Split(' ') and the update on wiseflow says "and where each task has its own counter that starts from 0."
    //But you also say that the output should match 3.3, where multiple words of the same string do not all count from 0, e.g. "hello world" = "hfnos cvzun"
    //Since i do not fully understand the instructions I will do it with split, but also add as a note how i would have otherwise done it. 
    let encode_par (str : string) (num : int) : string =
        let words = str.Split(' ') |> List.ofArray //<--- If i had not done split, it would be here: [str] instead of: str.Split(' ') |> List.ofArray
        let numWords = List.length words 
        let chunkSize = max 1 ((numWords + num - 1) / num)
        let chunks = words |> List.chunkBySize chunkSize
        
        chunks
        |> List.map (fun chunk ->
            System.Threading.Tasks.Task.Run(fun () ->
                chunk |> List.map encode |> compose_words))
        |> List.map (fun (t : System.Threading.Tasks.Task<string>) -> t.Result)
        |> compose_words        


    (* Question 4: The N-knights problem (25%) *)

    (* Question 4.1 *)
    
    type board = { size : int; knights : (int * int) list}

    let empty (n : int) : board = { size = n; knights = [] }
    
    let get_dimension (b : board) : int = b.size
    
    let has_knight (r: int) (c: int) (b: board) = 
        r >= 0 && r < b.size && c >= 0 && c < b.size
        && List.exists (fun (qr, qc) -> qr = r && qc = c) b.knights


    
    (* Question 4.2 *)
    
    let place_knight (r : int) (c : int) (b : board) : board option =
        if r < 0 || r >= b.size || c < 0 || c >= b.size then
            None
        elif has_knight r c b then
            None
        else
            let threatens (qr, qc) =
                qr = r || qc = c || abs (qr - r) = abs (qc - c)
            if List.exists threatens b.knights then
                None
            else
                Some { b with knights = (r, c) :: b.knights }
    
    let valid_solution (b : board) : bool =
        List.length b.knights = b.size  

    
    (* Question 4.3 *)
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
            
    let place_knight2 (r : int) (c : int) : chessMonad<unit> =
        CM (fun b ->
                match place_knight r c b with
                | Some b' -> Some ((), b')
                | None -> None)    
    let valid_solution2 : chessMonad<bool> =
        CM (fun b -> Some (valid_solution b, b))

    (* Question 4.4 *)
        
    let create_solution (squares : (int * int) list) : chessMonad<bool> =
        let rec aux squares =
            match squares with
            | [] -> valid_solution2
            | (r, c) :: rest -> place_knight2 r c >>>= aux rest
        aux squares    

    (* Question 4.5 *)
    
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
                    do! place_knight2 r c
                    return! aux rest
            }
        aux squares


    
