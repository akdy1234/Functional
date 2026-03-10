module Assignment1

open System






let add5 x = x + 5
let mul3 x = x * 3

let add5mul3 x = 
    x 
    |> add5
    |> mul3

let add5mul3_2 = 
    add5 >> mul3


let add5_2 f x = 
    x 
    |> f
    |> add5

let mul3_2 f x = 
    x 
    |> f
    |> mul3

let rec downto4 f n e = 
    if n <= 0 
        then e
    else 
        downto4 f (n-1) (f n e)


let fac x = 
    downto4 (fun n k -> n * k) x 1 


let rec fac2 x = 
    if x <= 0 
        then 1
    else x * fac(x-1)  


let range g n = 
    downto4 (fun n lst -> g n :: lst) n [] 


let rec double lst = 
    match lst with
    |[] -> []
    |x :: rest -> x * 2 :: double rest



let rec map f = 
    function
    |[] -> []
    |x :: rest -> f x :: (map f rest)

let db x = x * 2
let double_2 lst = map db lst


let rec stringLength lst = 
    match lst with
    |[] -> []
    |str :: rest -> String.length str :: stringLength rest

let length x = String.length x
let stringLength_2 lst = map length lst



let rec keepEven lst = 
    match lst with
    |[] -> []
    |x :: rest -> 
        if x % 2 = 0 then
            x :: keepEven rest
        else 
            keepEven rest


let rec filter f = 
    function
    |[] -> []
    |x :: rest -> 
        if f x then 
            x :: filter f rest
        else 
            filter f rest



let ke x = x % 2 = 0

let keepEven_2 lst = filter ke lst
    



let rec keepLengthGT5 lst = 
    match lst with
    |[] -> []
    |x :: rest -> 
        if (String.length x > 5) then
            x :: keepLengthGT5 rest
        else 
            keepLengthGT5 rest
        

let gt5 x = String.length x > 5
let keepLengthGT5_2 lst = filter gt5 lst



let rec sumPositive lst = 
    match lst with
    |[] -> 0
    |x :: rest -> 
        if (x > 0) then
            x + sumPositive rest
        else 
            sumPositive rest
        


let sumPositive_2 lst =
    let rec loop accumulator =
        function
        |[] -> accumulator
        |el :: rest -> 
            if (el > 0) then
                loop (accumulator + el) rest
            else 
                loop (accumulator) rest
    loop 0 lst
        


let rn x = x > 0


let rec sumPositive_3 lst = 
    lst
    |> filter rn
    |> List.fold (fun acc x -> acc + x) 0









[<EntryPoint>]
let main argv =
    
    printfn "%A" (add5 5)
    
    printfn "%A" (add5mul3 10)

    printfn "%A" (add5mul3_2 10)

    printfn "%A" (add5_2 mul3 7)

    printfn "%A" (mul3_2 String.length "Hello World!!!")

    printfn "%A" (downto4 (fun i x -> i + x) 50 10)

    printfn "%A" (fac 5)

    printfn "%A" (range fac 10)

    printfn "%A" (double [5; 8; 11; 14])

    printfn "%A" (double_2 [1; 2; 3; 4])

    printfn "%A" (stringLength ["hej"; "test"; "a"; "les gooooo"])

    printfn "%A" (stringLength_2 ["hejsa"; "tester"; "abcdef"; "les gooooo!!!!!!"])

    printfn "%A" (keepEven [1; 2; 3; 4])

    printfn "%A" (keepEven_2 [1; 2; 3; 4])

    printfn "%A" (keepLengthGT5 ["hejsa"; "test"; "abcdef"; "les gooooo!!!!!!"])

    printfn "%A" (keepLengthGT5_2 ["hejsa"; "test"; "abcdef"; "les gooooo!!!!!!"])

    printfn "%A" (sumPositive [-5; 2; -6; 0; 4; 4;])

    printfn "%A" (sumPositive_2 [-5; 2; -6; 0; 4; 4;])



    0





