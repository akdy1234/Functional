module Interpreter.State

type state

val mkState : int -> int option -> state
val declare : string -> state -> state option
val getVar : string -> state -> int option
val setVar : string -> int -> state -> state option
val free : int -> int -> state -> state option
val getMem : int -> state -> int option
val setMem : int -> int -> state -> state option
val random : state -> int
val alloc : string -> int -> state -> state option