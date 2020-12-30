Uno done the Elmish Way
=======================

[![NuGet version](https://img.shields.io/nuget/v/Elmish.Uno.svg)](https://www.nuget.org/packages/Elmish.Uno) [![NuGet downloads](https://img.shields.io/nuget/dt/Elmish.Uno.svg)](https://www.nuget.org/packages/Elmish.Uno) [![Build status](https://img.shields.io/appveyor/ci/cmeeren/elmish-Uno/master.svg?label=master)](https://ci.appveyor.com/project/cmeeren/elmish-Uno/branch/master)

Never write a ViewModel class again!

This library uses [Elmish](https://elmish.github.io/elmish), an Elm architecture implemented in F#, to build Uno applications. Elmish was originally written for [Fable](http://fable.io) applications, however it was trimmed and packaged for .NET as well.

Recommended resources
---------------------

* The [Elmish docs site](https://elmish.github.io/elmish) explains the general Elm architecture and principles.
* The [Elmish.Uno samples](https://github.com/XperiAndri/Elmish.Uno/tree/master/src/Samples) provide many concrete usage examples.
* The [official Elm guide](https://guide.elm-lang.org) may also provide some guidance, but note that not everything is relevant. A significant difference between “normal” Elm architecture and Elmish.Uno is that in Elmish.Uno, the views are statically defined using XAML, and the “view” function does not render views, but set up bindings.

Getting started with Elmish.Uno
-------------------------------

See the [SingleCounter](https://github.com/elmish/Elmish.Uno/tree/master/src/Samples) sample for a very simple app. The central points are:

1. Create an Uno Application (note, that the core Elmish logs are currently only written to the console).

2. Create F# .NET Standard class library and add reference to it from Uno projects (UWP, Android, iOS, macOS, WASM).

3. Add NuGet reference to package `Elmish.Uno`.

4. Define the model that describes your app’s state and a function that initializes it:

   ```F#
   type Model =
     { Count: int
       StepSize: int }
   
   let init () =
     { Count = 0
       StepSize = 1 }
   ```

5. Define the various messages that can change your model:

   ```F#
   type Msg =
     | Increment
     | Decrement
     | SetStepSize of int
   ```

6. Define an `update` function that takes a message and a model and returns an updated model:

   ```F#
   let update msg m =
     match msg with
     | Increment -> { m with Count = m.Count + m.StepSize }
     | Decrement -> { m with Count = m.Count - m.StepSize }
     | SetStepSize x -> { m with StepSize = x }
   ```

7. Define the “view” function using the `Bindings` module. This is the central public API of Elmish.Uno. Normally this function is called `view` and would take a model and a dispatch function (to dispatch new messages to the update loop) and return the UI (e.g. a HTML DOM to be rendered), but in Elmish.Uno this function simply sets up bindings that XAML-defined views can use. Therefore, let’s call it `bindings` instead of `view`. In order to be compatible with Elmish it needs to have the same signature, but in many (most?) cases the `model` and `dispatch ` parameters will be unused:

   ```F#
   open Elmish.Uno
   
   let bindings model dispatch =
     [
       "CounterValue" |> Binding.oneWay (fun m -> m.Count)
       "Increment" |> Binding.cmd (fun m -> Increment)
       "Decrement" |> Binding.cmd (fun m -> Decrement)
       "StepSize" |> Binding.twoWay
         (fun m -> float m.StepSize)
         (fun newVal m -> int newVal |> SetStepSize)
     ]
   ```

   The strings identify the binding names to be used in the XAML views. The [Binding module](https://github.com/elmish/Elmish.Uno/blob/master/src/Elmish.Uno/Binding.fs) has many functions to create various types of bindings.

8. Define your views and bindings in XAML:

   ```xaml
   <Page
        x:Class="MyNamespace.MainPage"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:local="using:Samples"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        Background="{ThemeResource ApplicationPageBackgroundThemeBrush}"
        mc:Ignorable="d">

        <StackPanel>
            <StackPanel
                Margin="0,25,0,0"
                HorizontalAlignment="Center"
                VerticalAlignment="Top"
                Orientation="Horizontal">
                <TextBlock Width="130" Margin="0,5,10,5">
                    <Run Text="Counter value: " />
                    <Run Text="{Binding CounterValue}" />
                </TextBlock>
                <Button
                    Width="30"
                    Margin="0,5,10,5"
                    Command="{Binding Decrement}"
                    Content="-" />
                <Button
                    Width="30"
                    Margin="0,5,10,5"
                    Command="{Binding Increment}"
                    Content="+" />
                <TextBlock Width="100" Margin="0,5,10,5">
                    <Run Text="Step size: " />
                    <Run Text="{Binding StepSize}" />
                </TextBlock>
                <Slider
                    Width="100"
                    Margin="0,5,10,5"
                    Maximum="10"
                    Minimum="1"
                    SnapsTo="Ticks"
                    TickFrequency="1"
                    Value="{Binding StepSize, Mode=TwoWay}" />
                <Button
                    Margin="0,5,10,5"
                    Command="{Binding Reset}"
                    Content="Reset" />
            </StackPanel>
        </StackPanel>

    </Page>
   ```

9. Instantiate Elmish view model and set as `DataContext` of the page in its constructor:

   ```F#
   using Elmish.Uno.Utilities;
   
   ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, ElmishProgram.Program);
   ```

   `ViewModel.StartLoop` will instantiate an Elmish `ViewModel` and set the `FrameworkElement`’s `DataContext` to the bindings you defined.

10. Profit! :)

For more complicated examples and other `Binding` functions, see the [samples](https://github.com/elmish/Elmish.Uno/tree/master/src/Samples).

FAQ
---

#### Do I have to use the project structure outlined above?

Yes. As long as UWP does not support F# you must have F# class library and C# UWP app. For more complex apps, you might want to consider a more clear separation of UI and core logic. An example would be the following structure:

* A core library containing the model definitions and `update` functions. This library can include a reference to Elmish (e.g. for the `Cmd` module helpers), but not to Elmish.Uno, which depends on certain Uno UI assemblies and has a UI-centred API (the `Binding` module). This will ensure your core logic (such as the `update` function) is free from any UI concerns, and allow you to re-use the core library should you want to port your app to another Elmish-based solution (e.g. using Fable).
* Uno specific project that contains the `bindings` (or `view`) function. This project would reference the core library and `Elmish.Uno`.
* Uno app projects of shared project containing the XAML-related stuff (windows, user controls, behaviors, etc.) and platform specific projects. Using Xamarin F# projects are not recommended as Uno generates C# code from XAML and you will need intermediary project to generate views.

#### Can I instantiate `Application` myself?

Yes, just do it before calling `Program.runWindow` and it will automatically be used. You might need this if you have application-wide resources in a `ResourceDictionary`, which might require you to instantiate the application before instantiating the main window you pass to `Program.runWindow`.

#### Can I use design-time view models?

Yes. You need to structure your code so you have a place, e.g. a file, that satisfies the following requirements:

* Must be able to instantiate a model and the associated bindings
* Must be reachable by the XAML views

There, open `Elmish.Uno.Utilities` and use `ViewModel.designInstance` to create a view model instance that your XAML can use at design-time:

```F#
module Foo.DesignViewModels
open Elmish.Uno.Utilities
let myVm = ViewModel.designInstance myModel myBindings
```

Then use the following attributes wherever you need a design VM:

```XAML
<Window
    ...
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:vm="clr-namespace:Foo;assembly=Foo"
    mc:Ignorable="d"
    d:DataContext="{x:Static vm:DesignViewModels.myVm}">
```

Project code must of course be enabled in the XAML designer for this to work.

#### Can I open new windows/dialogs?

The short version: Yes, but depending on the use-case, this may not play well with the Elmish architecture, and it is likely conceptually and architecturally clearer to stick with some kind of dialog in the main window, using bindings to control its visibility.

The long version:

You can easily open modeless windows (using `window.Show()`) in  command and set the binding context of the new window to the binding context of the main window. The [NewWindow sample](https://github.com/elmish/Elmish.Uno/tree/master/src/Samples) demonstrates this. It is then, from Elmish’s point of view, absolutely no difference between the windows; the bindings and message dispatches work exactly the same as if you had used multiple user controls in a single window, and you may close the new window without Elmish being affected by it.

Note that the NewWindow sample (like the other samples) keep a very simple project structure where the views are directly accessible in the core logic, which allows for direct instantiation of new windows in the `update` function (or the commands it returns). If you want a clearer separation between UI and core logic as previously described, you would need to write some kind of navigation service abstraction and use inversion of control (such as dependency injection) to allow the core project to instantiate the new window indirectly using the navigation service without needing to reference the UI layer directly. Such architectural patterns of course go very much against the grain of Elmish and functional architecture in general.

While modeless windows are possible, if not necessarily pleasant or idiomatic, you can not use the same method to open modal windows (using `window.ShowDialog()`). This will block the Elmish update loop, and all messages will be queued and only processed when the modal window is closed.

Windows that semantically produce a result, even if you implement them as modeless, can be more difficult. An general example might be a window containing a data entry form used to create a business entity. In these cases, a “Submit” button may need to both dispatch a message containing the window’s result (done via `Binding.cmd` or similar), as well as close the window. This can be problematic, or at least cumbersome, when there is logic determining what actually happens when the “Submit” button is clicked (send the result, display validation errors, etc.). For more on this, see the discussion in [#24](https://github.com/elmish/Elmish.Uno/issues/24).

The recommended approach is to stick to what is available via bindings in a single window. In the case of new windows, this means instead using in-window dialogs, similar to how most SPAs (single-page applications) created with Elm or Elmish would behave. This allows the UI to be a simple function of your model, which is a central point of the Elm architecture (whereas opening and closing windows are events that do not easily derive from any model state). The [SubModelOpt sample](https://github.com/elmish/Elmish.Uno/tree/master/src/Samples) provides a very simple example of custom dialogs, and this method also works great with libraries with ready-made MVVM-friendly dialogs, e.g. those in [Material Design In XAML Toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit).
