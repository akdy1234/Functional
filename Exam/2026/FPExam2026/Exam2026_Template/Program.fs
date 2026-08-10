open Exam2026_Template.Exam


let testQ11_block () =
    [0..9] |> List.map (lucas_number 1)

() |> testQ11_block |> printfn "%A"