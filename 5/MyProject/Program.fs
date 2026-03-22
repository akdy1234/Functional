module Assignment1

open System
open Option

let reservedVariableName (str: string) : bool =
    let reservedWords = ["if"; "then"; "else"; "while"; "declare"; "print"; "random"; "fork"; "__result__"]
    let result = List.exists (fun word -> word = str) reservedWords
    result
    
    
    
    
let validVariableName (str: string) : bool =
    let first = str.[0]
    let start = System.Char.IsAsciiLetter(first) || first = '_'
    let rest = String.forall (fun c -> System.Char.IsAsciiLetterOrDigit(c) || c = '_') str
    start && rest
    
type state = { variables: Map<string, int> }
    
let mkState () : state =
    { variables = Map.empty }
    
let declare (str: string) (st: state) : state option =
    match Map.containsKey str st.variables, validVariableName str, reservedVariableName str with
    | false, true, false -> Some { variables = Map.add str 0 st.variables }
    | _ -> None
    
let getVar (x: string) (st: state) : int option =
    match Map.tryFind x st.variables with
    | Some v -> Some v
    | None -> None

let setVar (x: string) (v: int) (st: state) : state option =
    match Map.containsKey x st.variables with
    | true -> Some { variables = Map.add x v st.variables }
    | false -> None
    
let push _ = failwith "not implemented"
let pop _ = failwith "not implemented"     





[<EntryPoint>]
let main argv =
    
    printfn "%A" (reservedVariableName "if")

    printfn "%A" (reservedVariableName "hej")

    printfn "%A" (validVariableName  "_hello_1")

    printfn "%A" (validVariableName  "1_hello")

    printfn "%A" (() |> mkState |> getVar "x")
    printfn "%A" (() |> mkState |> declare "x" |> bind (getVar "x"))
    printfn "%A" (() |> mkState |> declare "x" |> bind (setVar "x" 42) |> bind (getVar "x"))

    0





