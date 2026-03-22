module Interpreter.State

open Result
open Language
    
let reservedVariableName (str: string) : bool =
    let reservedWords = ["if"; "then"; "else"; "while"; "declare"; "print"; "random"; "fork"; "__result__"]
    let result = List.exists (fun word -> word = str) reservedWords
    result
    
    
    
    
let validVariableName (str: string) : bool =
    let first = str.[0]
    let start = System.Char.IsAsciiLetter(first) || first = '_'
    let rest = String.forall (fun c -> System.Char.IsAsciiLetterOrDigit(c) || c = '_') str
    start && rest
    
type state = { variables: Map<string, int>; memory: Memory.memory }
    
let mkState (memSize: int) : state =
    { variables = Map.empty; memory = Memory.empty memSize }
    
let declare (str: string) (st: state) : state option =
    match Map.containsKey str st.variables, validVariableName str, reservedVariableName str with
    | false, true, false -> Some { st with variables = Map.add str 0 st.variables }
    | _ -> None
    
let getVar (x: string) (st: state) : int option =
    match Map.tryFind x st.variables with
    | Some v -> Some v
    | None -> None

let setVar (x: string) (v: int) (st: state) : state option =
    match Map.containsKey x st.variables with
    | true -> Some { st with variables = Map.add x v st.variables }
    | false -> None

let alloc (x: string) (size: int) (st: state) : state option =
    match Memory.alloc size st.memory with
    | Some (newMem, ptr) ->
        match setVar x ptr { st with memory = newMem } with
        | Some st' -> Some st'
        | None -> None
    | None -> None
    

let free (ptr: int) (size: int) (st: state) : state option =
    match Memory.free ptr size st.memory with
    | Some newMem -> Some { st with memory = newMem }
    | None -> None

let getMem (ptr: int) (st: state) : int option =
    Memory.getMem ptr st.memory

let setMem (ptr: int) (v: int) (st: state) : state option =
    match Memory.setMem ptr v st.memory with
    | Some newMem -> Some { st with memory = newMem }
    | None -> None


    
let push _ = failwith "not implemented"
let pop _ = failwith "not implemented"     


