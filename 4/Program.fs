module Assignment1

open System


type error =
    | DivisionByZero

type aexpr =
    | Num of int
    | Add of aexpr * aexpr
    | Mul of aexpr * aexpr
    | Div of aexpr * aexpr
    
let (.+.) a b = Add (a, b)
let (.*.) a b = Mul (a, b)
let (.-.) a b = a .+. (b .*. Num -1)    (* Minus defined as adding a negative number *)
let (./.) a b = Div (a, b)
let (.%.) a b = a .-. ((a ./. b) .*. b)   (* Modulo a%b = a - (a / b) * b *)
    


    
type bexpr =
    | TT
    | Eq of aexpr * aexpr
    | Lt of aexpr * aexpr
    | Conj of bexpr * bexpr
    | Not of bexpr
    
let FF = Not TT
let (~~) b = Not b
let (.&&.) b1 b2 = Conj (b1, b2)
let (.||.) b1 b2 = ~~(~~b1 .&&. ~~b2)       (* boolean disjunction *)

let (.=.)  a b = Eq (a, b)   
let (.<.)  a b = Lt (a, b)   
let (.<>.) a b = ~~(a .=. b)                (* numeric inequality *)
let (.<=.) a b = (a .<. b) .||. (a .=. b)   (* numeric smaller than or equal to *)
let (.>=.) a b = ~~(a .<. b)                (* numeric greater than or equal to *)
let (.>.)  a b = (a .<>. b) .&&. (a .>=. b) (* numeric greater than *)



let add5 x = x + 5
let mul3 x = x * 3


let rec aexprToString : aexpr -> string = 
    function
    | Num f -> string f
    | Add (t1, t2) -> 
        "(" + aexprToString t1 + " + " + aexprToString t2 + ")"
    | Mul (t1, t2) ->
        "(" + aexprToString t1 + " * " + aexprToString t2 + ")"
    | Div (t1, t2) ->
        "(" + aexprToString t1 + " / " + aexprToString t2 + ")"
    

let rec bexprToString : bexpr -> string =
    function
    | TT -> string "true"
    | Eq (t1, t2) ->
        "(" + aexprToString t1 + " = " + aexprToString t2 + ")"
    | Lt (t1, t2) -> 
        "(" + aexprToString t1 + " < " + aexprToString t2 + ")"
    | Conj (t1, t2) -> 
        "(" + bexprToString t1 + " /\ " + bexprToString t2 + ")"
    | Not (t1) ->
        "(" + "not " + bexprToString t1 + ")"







let rec aexprEval =
    function
    | Num f -> Some f
    | Add (b,c) ->
        match (aexprEval b, aexprEval c) with
        |Some x, Some y -> Some (x+y)
        |_ -> None
    | Mul(b,c) -> 
        match (aexprEval b, aexprEval c) with
        |Some x, Some y -> Some (x*y)
        |_ -> None
    | Div (b, c) -> 
        match (aexprEval b, aexprEval c) with
        |Some x, Some y -> if y = 0 then None else Some (x/y)
        |_ -> None


let bind binder =
    function
    | None -> None
    | Some x -> binder x


let rec aexprEval2 =
    function
    | Num f -> Some f
    | Add (b, c) ->
        aexprEval2 b |> Option.bind (fun x ->
        aexprEval2 c |> Option.map  (fun y -> x + y))
    | Mul (b, c) ->
        aexprEval2 b |> Option.bind (fun x ->
        aexprEval2 c |> Option.map  (fun y -> x * y))
    | Div (b, c) ->
        aexprEval2 b |> Option.bind (fun x ->
        aexprEval2 c |> Option.bind (fun y ->
            if y = 0 then None else Some (x / y)))
    


let rec bexprEval =
    function
    | TT -> Some true
    | Eq (a, c) ->
        match (aexprEval a, aexprEval  c) with
        |Some x, Some y -> Some (x = y)
        |_ -> None
    | Lt (a, c) -> 
        match (aexprEval a, aexprEval c) with
        |Some x, Some y -> Some (x < y)
        |_ -> None

    | Conj (a, c) -> 
        match (bexprEval a, bexprEval c) with
        |Some x, Some y -> Some (x && y)
        |_ -> None
    | Not (a) -> 
        match (bexprEval a) with
        |Some x -> Some (not x)
        |_ -> None

    


[<EntryPoint>]
let main argv =
    
    printfn "%A" (add5 5)

    printfn "%A" (aexprToString (Num 4 .+. Num 2 .*. Num 3))

    printfn "%A" (aexprToString (Num 42 .*. (Num 13 .%. Num 3)))

    printfn "%A" (bexprToString TT)

    printfn "%A" (bexprToString FF)

    printfn "%A" (bexprToString(Num 42 .=. Num 32))

    printfn "%A" (bexprToString(Num 42 .<. Num 32 .+. Num 10))

    printfn "%A" (bexprToString ((Num 42 .<. Num 32 .+. Num 10) .||. (Num 32 .>=. Num 27 .%. Num 25)))

    printfn "%A" (aexprEval (Num 4))

    printfn "%A" (aexprEval (Num 4 .+. Num 2))

    printfn "%A" (aexprEval (Num 42 .*. (Num 13 .%. Num 0)))

    printfn "%A" (aexprEval (Num 42 .*. (Num 13 .%. Num 3)))

    printfn "%A" (bexprEval TT)

    printfn "%A" (bexprEval FF)

    printfn "%A" (bexprEval (Num 42 .=. Num 32))

    printfn "%A" (bexprEval (Num 42 .<. Num 32 .+. Num 10))

    printfn "%A" (bexprEval  ((Num 42 .<. Num 32 .+. Num 10) .||. (Num 32 .>=. Num 27 .%. Num 25)))

    printfn "%A" ()




    0





