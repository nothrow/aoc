<Query Kind="FSharpProgram" />

let (</>) a b = Path.Combine(a, b)
let input = Path.GetDirectoryName(Util.CurrentQueryPath) </> "input.txt"

let isDivisor n x = n % x = 0

let validPatternLengths x =
    { 1 .. x / 2 } |> Seq.filter (isDivisor x)

let ranges strlen =
    validPatternLengths strlen |>
    Seq.map (fun pl -> [| 0 .. pl .. (strlen - pl) |] |> Seq.map (fun start -> (start, pl)))

let splitWithRanges (x : ReadOnlyMemory<char>) range =
    range |> Seq.map(fun (start, len) -> x.Slice(start, len))
    
let mequal (a : ReadOnlyMemory<char>, b : ReadOnlyMemory<char>) =
    a.Span.SequenceEqual(b.Span)
    
let validateRange (x : ReadOnlyMemory<char>) range =
    range |> 
    splitWithRanges (x : ReadOnlyMemory<char>) |>
    Seq.pairwise |>
    Seq.forall (mequal)
    
let isInvalidIdStr2 (x : ReadOnlyMemory<char>) = 
    ranges (x.Length) |>
    Seq.map (validateRange x) |> 
    Seq.contains true
    
let isValidIdStr1 (x: string) = 
    x.Length % 2 = 1 ||
        x[0..x.Length / 2 - 1] <> x[x.Length / 2..]
    

let isValidId x = isValidIdStr1 (string x)
let isInvalidId x = not (isValidId x)

let isInvalidId2 x = isInvalidIdStr2 ((string x).AsMemory())
    
File.ReadAllText(input).Split(',') |>
    Seq.map (fun x -> x.Split('-')) |>
    Seq.map (fun x -> {| rstart = int64 x.[0]; rend = int64 x.[1] |}) |>
    Seq.collect (fun x -> { x.rstart .. x.rend } ) |>
    Seq.filter (isInvalidId2) |>
    Seq.sum |>
    Dump
