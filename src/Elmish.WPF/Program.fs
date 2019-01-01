[<RequireQualifiedAccess>]
module Elmish.Uno.Program

open Elmish.Uno.Utilities
open Windows.UI.Xaml

type App(windowBuilder: unit -> FrameworkElement, initProgram: FrameworkElement -> unit) =
  inherit Application()

  override u.OnLaunched activatedArgs =
    Windows.UI.Xaml.GenericStyles.Initialize()
    let root = windowBuilder()
    initProgram(root)
    Windows.UI.Xaml.Window.Current.Content <- root


let startApp config windowBuilder program =
  Application.Start(fun _ ->
    let init = fun root -> ViewModel.startLoop config root Elmish.Program.run program
    let app = new App(windowBuilder, init)
    ()
  )
  0
