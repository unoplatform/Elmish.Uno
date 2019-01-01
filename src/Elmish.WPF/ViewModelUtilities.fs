[<RequireQualifiedAccess>]
module Elmish.Uno.Utilities.ViewModel

open System.Windows
open Elmish
open Elmish.Uno
open Elmish.Uno.Internal
open Windows.UI.Xaml
open Windows.UI.Core
open System

/// Start Elmish dispatch loop
let internal startLoop
    (config: ElmConfig)
    (element: FrameworkElement)
    (programRun: Program<'t, 'model, 'msg, BindingSpec<'model, 'msg> list> -> unit)
    (program: Program<'t, 'model, 'msg, BindingSpec<'model, 'msg> list>) =
  let mutable lastModel = None

  let setState model dispatch =
    match lastModel with
    | None ->
        let mapping = program.view model dispatch
        let vm = ViewModel<'model,'msg>(model, dispatch, mapping, config)
        element.DataContext <- box vm
        lastModel <- Some vm
    | Some vm ->
        vm.UpdateModel model

  let uiDispatch (innerDispatch: Dispatch<'msg>) : Dispatch<'msg> =
    fun msg ->
      let doDispatch = fun () ->
        Console.WriteLine "Dispatch"
        innerDispatch msg
      element.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, fun () -> doDispatch()) |> ignore

  programRun { program with setState = setState; syncDispatch = uiDispatch }


/// Creates a design-time view model using the given model and bindings.
let designInstance (model: 'model) (bindings: BindingSpec<'model, 'msg> list) =
  ViewModel(model, ignore, bindings, ElmConfig.Default)
