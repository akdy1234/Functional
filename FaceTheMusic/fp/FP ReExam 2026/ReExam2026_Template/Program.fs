open Exam2026_Template.Exam


let testQ1 () =
    (* Testsfor Q1.1 *)
    printfn "Testing Question 1"
    
    printfn "%A" (tribonacci 5)
    printfn "%A" (tribonacci 9)


    printfn "acc0: %A" (tribonacci_acc 0)
    printfn "acc1: %A" (tribonacci_acc 1)
    printfn "acc2: %A" (tribonacci_acc 2)
    printfn "acc3: %A" (tribonacci_acc 3)
    printfn "acc4: %A" (tribonacci_acc 4)
    printfn "acc5: %A" (tribonacci_acc 5)
    printfn "acc6: %A" (tribonacci_acc 6)
    printfn "acc7: %A" (tribonacci_acc 7)
    printfn "acc8: %A" (tribonacci_acc 8)
    printfn "acc9: %A" (tribonacci_acc 9)

    printfn "approx: %A" (approx 5)
    printfn "approx: %A" (approx 6)
    printfn "approx: %A" (approx 7)
    printfn "approx: %A" (approx 8)
    printfn "approx: %A" (approx 9)
    printfn "approx: %A" (approx 10)

    printfn "steps: %A" (approx_steps_needed 0.1)
    printfn "steps: %A" (approx_steps_needed 0.001)
    printfn "steps: %A" (approx_steps_needed 0.0000001)

    printfn "seq: %A" (tribonacci_seq |> Seq.take 10 |> Seq.toList)
    
    ()



let testQ2 () =
    (* Testsfor Q1.1 *)
    printfn "Testing Question 2"
    
    printfn "%A" (foo ['a'; 'b'; ' '; 'c'; ' '; 'd'])
    printfn "%A" (foo ['c'; ' '; 'd'])
    printfn "%A" (foo ['d'])


    printfn "bar: %A" (bar ['a'; 'b'; ' '; 'c'; ' '; 'd'])
    printfn "old baz: %A" (baz "ab c")
    printfn "new baz: %A" (baz "ab c")

    //printfn "%A" (baz "Functional Programming 2026")


    printfn "cont: %A" (cont ['a'; 'b'; ' '; 'c'; ' '; 'd'])

    
    ()

let testQ3 () =
    (* Testsfor Q1.1 *)
    printfn "Testing Question 2"
    
    printfn "3.1: %A" (next 1 'c')
    printfn "3.1: %A" (next 2 'c')
    printfn "3.1: %A" (next 4 'y')
    printfn "3.1h: %A" (next 8 'h')
    printfn "3.1: %A" (next 5 'e')
    printfn "3.1: %A" (next 12 'l')
    printfn "3.1: %A" (next 12 'l')
    printfn "3.1: %A" (next 4 'o')
    printfn "3.1: %A" (next 4 'w')
    printfn "3.1: %A" (next 4 'o')
    printfn "3.1: %A" (next 4 'r')
    printfn "3.1: %A" (next 4 'l')
    printfn "3.1: %A" (next 4 'd')




    printfn "3.1d: %A" (next -4 'c')
    printfn "3.1: %A" (next 4 'ø')

    printfn "3.2: %A" (prev 5 'a')
    printfn "3.2: %A" (prev 5 'z')
    printfn "3.2: %A" (prev 2 'd')

    printfn "3.3: %A" (encode "hello world")
    printfn "3.3: %A" (decode "hfnos cvzun")

    
    printfn "3.5 %A" (encode_par "hello world" 2)



    ()

[<EntryPoint>]
let main argv =
    //testQ1 ()
    testQ2 ()
    testQ3 ()

    0 // return an integer exit code
