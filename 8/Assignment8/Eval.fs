module Interpreter.Eval

    open System
    open Result
    open Language
    open State
    open StateMonad
    let readFromConsole () = System.Console.ReadLine().Trim()


    let tryParseInt (str : string) = System.Int32.TryParse str    
    let rec readInt () = 
        let input = readFromConsole ()
        let result, value = tryParseInt input
        if result then
            printfn "%A" value
            value
        else
            printfn "%A is not an integer" input 
            readInt()

    

    let rec aexprEval (a: aexpr) : int stateMonad =
        match a with
        | Num n -> ret n
        | Var v -> getVar v
        | Add (a1, a2) ->
            aexprEval a1 >>= (fun x ->
            aexprEval a2 >>= (fun y ->
            ret (x + y)))
        | Mul (a1, a2) ->
            aexprEval a1 >>= (fun x ->
            aexprEval a2 >>= (fun y ->
            ret (x * y)))
        | Div (a1, a2) ->
            aexprEval a1 >>= (fun x ->
            aexprEval a2 >>= (fun y ->
            if y = 0 then fail else ret (x / y)))
        | MemRead a1 ->
            aexprEval a1 >>= (fun ptr ->
            getMem ptr)
        | Random -> random
        | Read -> ret (readInt ())
        | Cond (b, a1, a2) ->
            bexprEval b >>= (fun x ->
            if x then aexprEval a1
                else aexprEval a2)
            
    and bexprEval (b: bexpr) : bool stateMonad =
        match b with
        | TT -> ret true
        | Eq (a1, a2) ->
            aexprEval a1 >>= (fun x ->
            aexprEval a2 >>= (fun y ->
            ret (x = y)))
        | Lt (a1, a2) ->
            aexprEval a1 >>= (fun x ->
            aexprEval a2 >>= (fun y ->
            ret (x < y)))
        | Not b ->
            bexprEval b >>= (fun x ->
            ret (not x))
        | Conj (b1, b2) ->
            bexprEval b1 >>= (fun x ->
            bexprEval b2 >>= (fun y ->
            ret (x && y)))

        



    type StateBuilder() =  
        member this.Bind(f, x) = (>>=) f x  
        member this.Return(x) = ret x  
        member this.ReturnFrom(x) = x  
        member this.Combine(a, b) = a >>= (fun _ -> b) 
      
    let eval = StateBuilder()

    let rec aexprEval2 (a: aexpr) : int stateMonad =
        eval {
            match a with
            | Num n -> return n
            | Var v -> return! getVar v
            | Add (a1, a2) ->
                let! x = aexprEval2 a1
                let! y = aexprEval2 a2
                return x + y
            | Mul (a1, a2) ->
                let! x = aexprEval2 a1
                let! y = aexprEval2 a2
                return x * y
            | Div (a1, a2) ->
                let! x = aexprEval2 a1
                let! y = aexprEval2 a2
                if y = 0 then return! fail else return x / y
            | MemRead a1 ->
                let! ptr = aexprEval2 a1
                return! getMem ptr
            | Random -> return! random
            | Read -> return (readInt ())
            | Cond (b, a1, a2) ->
                let! x = bexprEval2 b
                if x then return! aexprEval2 a1
                    else return! aexprEval2 a2
        }

    and bexprEval2 (b: bexpr) : bool stateMonad =
        eval {
            match b with
            | TT -> return true
            | Eq (a1, a2) ->
                let! x = aexprEval2 a1
                let! y = aexprEval2 a2
                return x = y
            | Lt (a1, a2) ->
                let! x = aexprEval2 a1
                let! y = aexprEval2 a2
                return x < y
            | Not b ->
                let! x = bexprEval2 b
                return (not x)
            | Conj (b1, b2) ->
                let! x = bexprEval2 b1
                let! y = bexprEval2 b2
                return (x && y)
        }

    let split (s1 : string) (s2 : string) = s2 |> s1.Split |> Array.toList


    let mergeStrings (es: aexpr list) (s: string) : string stateMonad =
        let parts = split s "%"
        let rec loop (es: aexpr list) (parts: string list) (acc: string) : string stateMonad =
            match es, parts with
            | [], [last] -> ret (acc + last)
            | [], _ -> fail
            | _, [] -> fail
            | e :: restEs, part :: restParts ->
                aexprEval e >>= (fun x ->
                loop restEs restParts (acc + part + string x))
        loop es parts ""

    


    let rec stmntEval (s: stmnt) : unit stateMonad =
        match s with
        | Skip -> ret ()
        | Declare v -> declare v
        | Assign (v, a) ->
            aexprEval a >>= (fun x ->
            setVar v x)
        | Seq (s1, s2) ->
            stmntEval s1 >>>= stmntEval s2
        | If (guard, s1, s2) ->
            bexprEval guard >>= (fun x ->
            if x then stmntEval s1
                else stmntEval s2)
        | While (guard, s') ->
            bexprEval guard >>= (fun x ->
            if x then
                stmntEval s' >>>= stmntEval (While (guard, s'))
            else
                ret ())
        | Alloc (x, e) ->
            aexprEval e >>= (fun size ->
            alloc x size)
        | Free (e1, e2) ->
            aexprEval e1 >>= (fun ptr ->
            aexprEval e2 >>= (fun size ->
            free ptr size))
        | MemWrite (e1, e2) ->
            aexprEval e1 >>= (fun ptr ->
            aexprEval e2 >>= (fun v ->
            setMem ptr v))
        | Print (es, s) ->
            mergeStrings es s >>= (fun str ->
            printfn "%s" str
            ret ())
    