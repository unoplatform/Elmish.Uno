module Elmish.Uno.Samples.OneWaySeq.Program

open System
open Elmish
open Elmish.Uno


type Model =
    { OneWaySeqNumbers: int list
      OneWayNumbers: int list }

let init () =
    { OneWaySeqNumbers = [ 1000..-1..1 ]
      OneWayNumbers = [ 1000..-1..1 ] }

type Msg =
| AddOneWaySeqNumber
| AddOneWayNumber

let update msg m =
    match msg with
    | AddOneWaySeqNumber -> { m with OneWaySeqNumbers = m.OneWaySeqNumbers.Head + 1 :: m.OneWaySeqNumbers }
    | AddOneWayNumber -> { m with OneWayNumbers = m.OneWayNumbers.Head + 1 :: m.OneWayNumbers }

let bindings model dispatch = [
    "OneWaySeqNumbers" |> Binding.oneWaySeq (fun m -> m.OneWaySeqNumbers) id (=)
    "OneWayNumbers" |> Binding.oneWay (fun m -> m.OneWayNumbers)
    "AddOneWaySeqNumber" |> Binding.cmd (fun m -> AddOneWaySeqNumber)
    "AddOneWayNumber" |> Binding.cmd (fun m -> AddOneWayNumber)
]


[<CompiledName("Program")>]
let program =
    Program.mkSimple init update bindings
    |> Program.withConsoleTrace

[<CompiledName("Config")>]
let config = { ElmConfig.Default with LogConsole = true }
