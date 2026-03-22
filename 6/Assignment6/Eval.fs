module Interpreter.Eval

    open Result
    open Language
    open State
    
    let rec aexprEval (a: aexpr) (st: state) : int option =
        match a with
        | Num n -> Some n
        | Var v -> getVar v st
        | Add (a1, a2) ->
            aexprEval a1 st |> Option.bind (fun x ->
            aexprEval a2 st |> Option.bind (fun y ->
            Some (x + y)))
        | Mul (a1, a2) ->
            aexprEval a1 st |> Option.bind (fun x ->
            aexprEval a2 st |> Option.bind (fun y ->
            Some (x * y)))
        | Div (a1, a2) ->
            aexprEval a1 st |> Option.bind (fun x ->
            aexprEval a2 st |> Option.bind (fun y ->
            if y = 0 then None else Some (x / y)))
        | MemRead (a1) -> 
            aexprEval a1 st |> Option.bind (fun ptr ->
            getMem ptr st)


            
    let rec bexprEval (b: bexpr) (st: state) : bool option =
        match b with
        | TT -> Some true
        | Eq (a1, a2) ->
            aexprEval a1 st |> Option.bind (fun x ->
            aexprEval a2 st |> Option.bind (fun y ->
            Some (x = y)))
        | Lt (a1, a2) ->
            aexprEval a1 st |> Option.bind (fun x ->
            aexprEval a2 st |> Option.bind (fun y ->
            Some (x < y)))
        | Not b ->
            bexprEval b st |> Option.bind (fun x ->
            Some (not x))
        | Conj (b1, b2) ->
            bexprEval b1 st |> Option.bind (fun x ->
            bexprEval b2 st |> Option.bind (fun y ->
            Some (x && y)))

        
    let rec stmntEval (s: stmnt) (st: state) : state option =
        match s with
        | Skip -> Some st
        | Declare v -> declare v st
        | Assign (v, a) ->
            aexprEval a st |> Option.bind (fun x ->
            setVar v x st)
        | Seq (s1, s2) ->
            stmntEval s1 st |> Option.bind (fun st' ->
            stmntEval s2 st')
        | If (guard, s1, s2) ->
            bexprEval guard st |> Option.bind (fun x ->
            if x then stmntEval s1 st
                else stmntEval s2 st)
        | While (guard, s') ->
            bexprEval guard st |> Option.bind (fun x ->
            if x then
                stmntEval s' st |> Option.bind (fun st' ->
                stmntEval (While (guard, s')) st')
            else
                Some st)
        | Alloc (x, e) ->
            aexprEval e st |> Option.bind (fun size ->
            alloc x size st)
        | Free (e1, e2) ->
            aexprEval e1 st |> Option.bind (fun ptr ->
            aexprEval e2 st |> Option.bind (fun size ->
            free ptr size st))
        | MemWrite (e1, e2) ->
            aexprEval e1 st |> Option.bind (fun ptr ->
            aexprEval e2 st |> Option.bind (fun v ->
            setMem ptr v st))