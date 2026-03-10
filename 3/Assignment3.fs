module Assignment3


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





  
let add5mul3_3 _ = failwith "not implemented"
    
 
let rec mergeFuns _ = failwith "not implemented"
        
let rec facFuns _ = failwith "not implemented"
        
let fac_2 _ = failwith "not implemented"

let removeOddIdx _= failwith "not implemented"
        
    
let weird _ = failwith "not implemented"
    
   
let insert _= failwith "not implemented"
                
let rec permutations _ = failwith "not implemented"