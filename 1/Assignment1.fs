module Assignment1

    open System
    
    let sqr x = x * x

    let pow (x:float) (n:float) = Math.Pow(x, n)

    let rec sum x = 
            if x <= 0 
                then x
                else 
            x + sum(x-1)


    let rec fib x = 
            if x <= 1
                then x
                else
            fib(x-1) + fib(x-2)



    let dup (s:string) = s + s



    let dupn (s:string) x = String.replicate x s



    let rec bin (n, k) = 
            if k = 0 then 1
            else if k = n then 1
            else bin (n-1, k-1) + bin (n-1  , k) 



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

    let minutes _ = failwith "not implemented"

    let curry _ = failwith "not implemented"
    let uncurry _ = failwith "not implemented"