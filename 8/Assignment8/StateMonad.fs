module Interpreter.StateMonad

    // Use one of these state monads depending on which combination of green, yellow, and red exercises you are doing.
    // Feel free to just delete the rest to unclutter the code.
    
    open State
    open Language
    
    // Green exercises
    
    type 'a stateMonad = SM of (state -> ('a * state) option)
        
    let ret x= SM (fun st -> Some(x, st))
    let fail    = SM (fun _ -> None)
    
    let bind (SM f) g =
        SM (fun st ->
            match f st with
            | Some (x, st') -> let (SM h) = g x in h st'
            | None -> None)
        
    
    
    
    
    let (>>=) a f = bind a f
    let (>>>=) a b = a >>= (fun _ -> b)

    let random = SM (fun st -> Some(State.random st, st))   

    let declare str = SM (fun st ->
        match State.declare str st with
        | Some st' -> Some((), st')
        | None -> None)
    
    let setVar str v = SM (fun st ->
        match State.setVar str v st with
        | Some st' -> Some((), st')
        | None -> None)

    let getVar x = SM (fun st ->
        match State.getVar x st with
        | Some v -> Some(v, st)
        | None -> None)
        
    
    let alloc str size = SM (fun st ->
        match State.alloc str size st with
        | Some st' -> Some((), st')
        | None -> None)
        
    let free ptr size = SM (fun st ->
        match State.free ptr size st with
        | Some st' -> Some((), st')
        | None -> None)
        
    let setMem ptr v = SM (fun st ->
        match State.setMem ptr v st with
        | Some st' -> Some((), st')
        | None -> None) 
        
    let getMem ptr = SM (fun st ->
        match State.getMem ptr st with
        | Some v -> Some(v, st)
        | None -> None)
    
    let push _ = failwith "not implemented"
    let pop _ = failwith "not implemented"
    
    let evalState st (SM f) =
        match f st with
        | Some (v, _) -> Some v
        | None -> None