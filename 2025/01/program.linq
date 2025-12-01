<Query Kind="FSharpProgram" />

let (</>) a b = Path.Combine(a, b)
let input = Path.GetDirectoryName(Util.CurrentQueryPath) </> "input.txt"

let (|Prefix|_|) (p:char) (s:string) =
    if (s.[0] = p) then Some s.[1..] else None

let divMod n d =
    let q = int (floor (float n / float d))
    let r = n - q * d
    (q |> abs, r)

let groupAdd max (value, clicks) y =
    let q, r = divMod (value + y) max
    let adjust = if r = 0 && y < 0 then 1 else 0 // if we stopped at 0 via <- rotation, it clicks
    let adjust2 = if value = 0 && y < 0 then -1 else 0 // if we started at 0 and went left, it does not click
    (r, clicks + q + adjust + adjust2)

let group100add = groupAdd 100

File.ReadLines(input) |> 
    Seq.map (
        function
            | Prefix 'R' n -> +int n
            | Prefix 'L' n -> -int n
            | _ -> failwith "wrong input"
    ) |>
    Seq.scan (group100add) (50, 0) |> 
    Seq.last |>
    Dump |> ignore