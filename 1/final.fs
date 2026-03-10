module Assignment1

open System

let sqr x = x * x

let pow (x:float) (n:float) = Math.Pow(x, n)

let rec fib x = 
            if x <= 0 
                then x
                else 
            x + fib(x-1)


let rec sum x = 
            if x <= 1
                then x
                else
            sum(x-1) + sum(x-2)



let dup (s:string) = s + s



let dupn (s:string) x = String.replicate x s



let rec bin (k, n) = 
            if k = 0
                then 1
            else if k = n
                then 1
            else 
            bin(k-1, n-1) + bin(k, n-1) 



let readFromConsole () = System.Console.ReadLine().Trim()


let tryParseInt (str : string) = System.Int32.TryParse str


let readInt () = 
        let input = readFromConsole ()
        let result, value = tryParseInt input
        if result
            then printfn "%A" value
        else
            printfn "%A is not an integer" input