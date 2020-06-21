namespace Elmish.Uno

open System
open Elmish.Uno.Utilities
open Windows.UI.Xaml

//type App(windowBuilder: unit -> FrameworkElement, initProgram: FrameworkElement -> unit) =
//    inherit Application()

//    override u.OnLaunched activatedArgs =
//        Windows.UI.Xaml.GenericStyles.Initialize()
//        let root = windowBuilder()
//        initProgram(root)
//        Windows.UI.Xaml.Window.Current.Content <- root

//[<RequireQualifiedAccess>]
//module Program =

//    let makeInit config program root =
//      ViewModel.startLoop config root Elmish.Program.run program

    //let createApp config (windowBuilder: unit -> FrameworkElement) program =
    //  let init: FrameworkElement -> unit = fun root -> ViewModel.startLoop config root Elmish.Program.run program
    //  new App(windowBuilder, init)
