module Rational.Rational

type rational = R of int * int
    
let makeRational num den = R (num,den)

let add r1 r2 = 
    match r1, r2 with
    | R(n1, d1), R(n2, d2) -> 
    makeRational(n1*d2+n2*d1) (d1*d2)


let add2 r1 r2 = 
    let (R (n1,d1)) = r1 in
    let (R (n2,d2)) = r2 in
    makeRational(n1*d2+n2*d1) (d1*d2)
