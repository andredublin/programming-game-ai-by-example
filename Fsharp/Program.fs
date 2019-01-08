// Learn more about F# at http://fsharp.org

open System
open WestWorld1.Entities

[<EntryPoint>]
let main argv =
    let miner = Miner(EntityNames.Miner(1))

    for i in 1..20 do
        miner.Update()

    Console.ReadKey() |> ignore
    0 // return an integer exit code
