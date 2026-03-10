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


let rec readInt () = 
        let input = readFromConsole ()
        let result, value = tryParseInt input
        if result then
            printfn "%A" value
            value   
        else
            printfn "%A is not an integer" input 
            readInt()

    



let timediff (x, y) (k, n) = 
    let t1 = (x * 60) + y
    let t2 = (k * 60) + n
    t2 - t1
    
    

let minutes (x,y) = 
 (x*60) + y
    



let curry _ = failwith "not implemented"
let uncurry _ = failwith "not implemented"



[<EntryPoint>]
let main argv =
    printfn "25: %d" (sqr 5)

    printfn "32: %f" (pow 2.0 5.0)

    printfn "15: %d" (fib 5)

    printfn "55: %d" (sum 10)

    printfn "hej! hej! : %s" (dup "hej! ")

    printfn "hi! hi! hi!: %s" (dupn "hi! " 3)

    printfn "6: %d" (bin (2,4))

    printfn "%d" (timediff(12,34) (11,35))

    printfn "%d" (minutes (14, 24))
    
    readInt()

    0

    







