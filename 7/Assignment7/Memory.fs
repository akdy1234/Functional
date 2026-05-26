module Interpreter.Memory
    
    open Result
    open Language

    type memory = { mem: Map<int, int>; next: int }
    let empty (memSize: int) : memory =
        { mem = Map.empty; next = 0 }

    let alloc (size: int) (mem: memory) : (memory * int) option =
        if size <= 0 then None
        else
            let addresses = [mem.next .. mem.next + size - 1]
            let newMem = List.fold (fun m addr -> Map.add addr 0 m) mem.mem addresses
            Some ({ mem = newMem; next = mem.next + size }, mem.next)
    
    let free (ptr: int) (size: int) (mem: memory) : memory option =
        match List.forall (fun addr -> Map.containsKey addr mem.mem) [ptr .. ptr + size - 1] with
        | true -> Some { mem = List.fold (fun m addr -> Map.remove addr m) mem.mem [ptr .. ptr + size - 1]; next = mem.next }
        | false -> None
        
    let setMem (ptr: int) (v: int) (mem: memory) : memory option =
        match Map.containsKey ptr mem.mem with
        | true -> Some { mem = Map.add ptr v mem.mem; next = mem.next }
        | false -> None
        
    let getMem (ptr: int) (mem: memory) : int option =
        Map.tryFind ptr mem.mem