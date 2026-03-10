module Assignment1

open System

let sqr x = x * x
sqr(5)

let pow (x:float) (n:float) = Math.Pow(x, n)
pow(5)

let rec fib x = 
            if x <= 0 
                then x
                else 
            x + fib(x-1)
fib(5)


let rec sum x = 
            if x <= 1
                then x
                else
            sum(x-1) + sum(x-2)
sum(5)


let dup (s:string) = s + s
dup("hej! ")



let dupn (s:string) x = String.replicate x s
dupn "hej! " 3


let rec bin (k, n) = 
            if k = 0
                then 1
            else if k = n
                then 1
            else 
            bin(k-1, n-1) + bin(k, n-1) 
bin(2,4)


let readFromConsole () = System.Console.ReadLine().Trim()
let tryParseInt (str : string) = System.Int32.TryParse str


let readInt() = failwith "not implemented"
let timediff _ = failwith "not implemented"
let minutes _ = failwith "not implemented"
let curry _ = failwith "not implemented"
let uncurry _ = failwith "not implemented"do