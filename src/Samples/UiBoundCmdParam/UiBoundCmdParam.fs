module Elmish.Uno.Samples.UiBoundCmdParam.Program

open System
open Elmish
open Elmish.Uno

type Model =
    { Numbers: int list
      EnabledMaxLimit: int }

let init () =
    { Numbers = [0 .. 10]
      EnabledMaxLimit = 5 }

type Msg =
| SetLimit of int
| Command

let update msg m =
    match msg with
    | SetLimit x -> { m with EnabledMaxLimit = x }
    | Command -> m

let bindings model dispatch = [
    "Numbers" |> Binding.oneWay (fun m -> m.Numbers)
    "Limit" |> Binding.twoWay
        (fun m -> float m.EnabledMaxLimit)
        (fun v m -> int v |> SetLimit)
    "Command" |> Binding.paramCmdIf
        (fun p m -> Command)
        (fun p m -> not (isNull p) && p :?> int <= m.EnabledMaxLimit)
        true
]


[<CompiledName("Program")>]
let program =
    Program.mkSimple init update bindings
    |> Program.withConsoleTrace

[<CompiledName("Config")>]
let config = { ElmConfig.Default with LogConsole = true }
