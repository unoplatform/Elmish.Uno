[<RequireQualifiedAccess>]
module Elmish.WPF.Program

open Elmish.WPF.Utilities
open Windows.UI.Xaml

/// Start WPF dispatch loop. Blocking function.
let private startApp window =
  true

/// Starts both Elmish and WPF dispatch loops. Blocking function.
let runWindow window program =
  ViewModel.startLoop ElmConfig.Default window Elmish.Program.run program
  startApp window

/// Starts both Elmish and WPF dispatch loops with the specified configuration.
/// Blocking function.
let runWindowWithConfig config window program =
  ViewModel.startLoop config window Elmish.Program.run program
  startApp window
