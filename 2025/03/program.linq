<Query Kind="FSharpProgram" />

let (</>) a b = Path.Combine(a, b)
let input = Path.GetDirectoryName(Util.CurrentQueryPath) </> "input.txt"

let largestWithIndex s =
    s |> 
    Seq.mapi (fun i c -> i, c) |> 
    Seq.maxBy snd

let slm max =
    Seq.truncate max

let agg first (idx, c) = (c, first + idx + 1)

let joltage batteries (bank : string)  =
    [ 0 .. batteries - 1 ] |>
        List.mapFold (fun first b ->
            bank.[first .. bank.Length - batteries + b] |> largestWithIndex |> agg first
        ) 0 |>
        fst |>
        List.toArray |>
        String |>
        int64
    
input |>
    File.ReadLines |>
    Seq.map (joltage 2) |>
    Seq.sum |>
    Dump

input |>
    File.ReadLines |>
    Seq.map (joltage 12) |>
    Seq.sum |>
    Dump
    
    