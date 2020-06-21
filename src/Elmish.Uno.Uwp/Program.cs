using Elmish.Uno.Internal;
using ViewModel = Elmish.Uno.Utilities.ViewModel;

using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Elmish.Uno
{
    //public class App : Application
    //{
    //    // Fields
    //    internal Func<FrameworkElement> windowBuilder;
    //    internal Action<FrameworkElement> initProgram;

    //    // Methods
    //    public App(Func<FrameworkElement> windowBuilder, Action<FrameworkElement> initProgram)
    //    {
    //        this.windowBuilder = windowBuilder;
    //        this.initProgram = initProgram;
    //    }

    //    protected override void OnLaunched(LaunchActivatedEventArgs args)
    //    {
    //        //Windows.UI.Xaml.GenericStyles.Initialize();
    //        FrameworkElement root = this.windowBuilder();
    //        this.initProgram(root);
    //        Window.Current.Content = root;
    //        if (args.PrelaunchActivated == false)
    //            Window.Current.Activate();
    //    }
    //}

    //[RequireQualifiedAccess, CompilationMapping(SourceConstructFlags.Module)]
    //public static class Program
    //{
    //    public static Program<Unit, TModel, TMsg, FSharpList<BindingSpec<TModel, TMsg>>> MakeSimple<TModel, TMsg>(
    //Func<TModel> init, Func<TMsg, TModel, TModel> update, Func<TModel, FSharpFunc<TMsg, Unit>, FSharpList<BindingSpec<TModel, TMsg>>> bindings)
    //    {
    //        var initFunc = FuncConvert.FromFunc(init);
    //        var updateFunc = FuncConvert.FromFunc(update);
    //        var bindingsFunc = FuncConvert.FromFunc(bindings);
    //        return ProgramModule.mkSimple(initFunc, updateFunc, bindingsFunc);
    //    }

    //    public static Program<TArg, TModel, TMsg, FSharpList<BindingSpec<TModel, TMsg>>> MakeSimple<TArg, TModel, TMsg>(
    //  Func<TArg, TModel> init, Func<TMsg, TModel, TModel> update, Func<TModel, FSharpFunc<TMsg, Unit>, FSharpList<BindingSpec<TModel, TMsg>>> bindings)
    //    {
    //        var initFunc = FuncConvert.FromFunc(init);
    //        var updateFunc = FuncConvert.FromFunc(update);
    //        var bindingsFunc = FuncConvert.FromFunc(bindings);
    //        return ProgramModule.mkSimple(initFunc, updateFunc, bindingsFunc);
    //    }

    //    // Methods
    //    [CompilationArgumentCounts(new int[] { 1, 1, 1 })]
    //    public static App CreateApp<TModel, TDispatch>(ElmConfig config, Func<FrameworkElement> windowBuilder, Program<Unit, TModel, TDispatch, FSharpList<BindingSpec<TModel, TDispatch>>> program) =>
    //        new App(windowBuilder, CreateInit(config, program));

    //    public static Action<FrameworkElement> CreateInit<TModel, TDispatch>(ElmConfig config, Program<Unit, TModel, TDispatch, FSharpList<BindingSpec<TModel, TDispatch>>> program)
    //    {
    //        void ProgramRun(Program<Unit, TModel, TDispatch, FSharpList<BindingSpec<TModel, TDispatch>>> p)
    //          => ProgramModule.run(p);

    //        void StartLoop(FrameworkElement root) => ViewModel.StartLoop(config, root, ProgramRun, program);

    //        return StartLoop;
    //    }
    //}
}
