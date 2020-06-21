using Elmish.Uno.Internal;

using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Elmish.Uno.Utilities
{
    public class DynamicCustomProperty<TValue> : ICustomProperty
    {
        public Func<TValue> Getter { get; }
        public Action<TValue> Setter { get; }
        public Func<object, TValue> IndexGetter { get; }
        public Action<object, TValue> IndexSetter { get; }

        public object GetValue(object target) => Getter.Invoke();
        public void SetValue(object target, object value) => Setter.Invoke((TValue)value);
        public object GetIndexedValue(object target, object index) => IndexGetter.Invoke(index);
        public void SetIndexedValue(object target, object value, object index) => IndexSetter.Invoke(index, (TValue)value);

        public bool CanRead => Getter != null || IndexGetter != null;
        public bool CanWrite => Setter != null || IndexSetter != null;
        public string Name { get; }
        public Type Type => typeof(TValue);

        public DynamicCustomProperty(string name, Func<TValue> getter, Action<TValue> setter = null, Func<object, TValue> indexGetter = null, Action<object, TValue> indexSetter = null)
        {
            Name = name;
            Getter = getter;
            Setter = setter;
            IndexGetter = indexGetter;
            IndexSetter = indexSetter;
        }
    }

    public class ViewModel<TModel, TMsg> : Elmish.Uno.Internal.ViewModel<TModel, TMsg>, ICustomPropertyProvider
    {
        public ViewModel(TModel initialModel, FSharpFunc<TMsg, Unit> dispatch, FSharpList<BindingSpec<TModel, TMsg>> bindingSpecs, ElmConfig config) : base(initialModel, dispatch, bindingSpecs, config) { }

        public override Internal.ViewModel<object, object> Create(object initialModel, FSharpFunc<object, Unit> dispatch, FSharpList<BindingSpec<object, object>> bindingSpecs, ElmConfig config)
        {
            return new ViewModel<object, object>(initialModel, dispatch, bindingSpecs, config);
        }

        private ICustomProperty GetProperty(string name)
        {
            var binding = this.Bindings[name];
            switch (binding)
            {
                case Binding<TModel, TMsg>.OneWay oneWay:
                    return new DynamicCustomProperty<object>(name, () => GetMember(oneWay));
                case Binding<TModel, TMsg>.OneWayLazy oneWayLazy:
                    return new DynamicCustomProperty<object>(name, () => GetMember(oneWayLazy));
                case Binding<TModel, TMsg>.OneWaySeq oneWaySeq:
                    return new DynamicCustomProperty<ObservableCollection<object>>(name, () => GetMember(oneWaySeq) as ObservableCollection<object>);
                case Binding<TModel, TMsg>.TwoWay twoWay:
                    return new DynamicCustomProperty<object>(name, () => GetMember(twoWay), value => SetMember(name, twoWay, value));
                case Binding<TModel, TMsg>.TwoWayValidate twoWayValidate:
                    return new DynamicCustomProperty<object>(name, () => GetMember(twoWayValidate), value => SetMember(name, twoWayValidate, value));
                case Binding<TModel, TMsg>.TwoWayIfValid twoWayIsValid:
                    return new DynamicCustomProperty<object>(name, () => GetMember(twoWayIsValid), value => SetMember(name, twoWayIsValid, value));
                case Binding<TModel, TMsg>.Cmd cmd:
                    return new DynamicCustomProperty<ICommand>(name, () => GetMember(cmd) as ICommand);
                case Binding<TModel, TMsg>.CmdIfValid cmdIfValid:
                    return new DynamicCustomProperty<ICommand>(name, () => GetMember(cmdIfValid) as ICommand);
                case Binding<TModel, TMsg>.ParamCmd paramCmd:
                    return new DynamicCustomProperty<object>(name, () => GetMember(paramCmd));
                case Binding<TModel, TMsg>.SubModel subModel:
                    return new DynamicCustomProperty<ViewModel<object, object>>(name, () => GetMember(subModel) as ViewModel<object, object>);
                case Binding<TModel, TMsg>.SubModelSeq subModelSeq:
                    return new DynamicCustomProperty<ObservableCollection<ViewModel<object, object>>>(name, () => GetMember(subModelSeq) as ObservableCollection<ViewModel<object, object>>);
                default:
                    throw new NotSupportedException();
            }
        }

        public ICustomProperty GetCustomProperty(string name) => GetProperty(name);

        public ICustomProperty GetIndexedProperty(string name, Type type)
        {
            return GetProperty(name);
        }

        public string GetStringRepresentation() => typeof(TModel).ToString();

        public Type Type => typeof(ViewModel<TModel, TMsg>);
    }

    [RequireQualifiedAccess, CompilationMapping(SourceConstructFlags.Module)]
    public static class ViewModel
    {
        public static ViewModel<TModel, TMsg> DesignInstance<T, TModel, TMsg>(TModel model, Program<T, TModel, TMsg, FSharpList<BindingSpec<TModel, TMsg>>> program)
        {
            var emptyDispatch = FuncConvert.FromAction((TMsg msg) => { });
            var mapping = FSharpFunc<TModel, FSharpFunc<TMsg, Unit>>.InvokeFast(ProgramModule.view(program), model, emptyDispatch);
            return new ViewModel<TModel, TMsg>(model, emptyDispatch, mapping, ElmConfig.Default);
        }

        public static void StartLoop<T, TModel, TMsg>(ElmConfig config, FrameworkElement element, Action<Program<T, TModel, TMsg, FSharpList<BindingSpec<TModel, TMsg>>>> programRun, Program<T, TModel, TMsg, FSharpList<BindingSpec<TModel, TMsg>>> program)
        {
            FSharpRef<FSharpOption<ViewModel<TModel, TMsg>>> lastModel = new FSharpRef<FSharpOption<ViewModel<TModel, TMsg>>>(null);
            FSharpFunc<FSharpFunc<TMsg, Unit>, FSharpFunc<TMsg, Unit>> syncDispatch =
              FuncConvert.FromAction(MakeSyncDispatch<TMsg>(element));
            var setSate = FuncConvert.FromAction(MakeSetState(config, element, program, lastModel));

            programRun.Invoke(
                ProgramModule.withSyncDispatch(syncDispatch,
                  ProgramModule.withSetState(setSate, program)));
        }

        private static Action<TModel, FSharpFunc<TMsg, Unit>> MakeSetState<TArg, TModel, TMsg>(ElmConfig config, FrameworkElement element, Program<TArg, TModel, TMsg, FSharpList<BindingSpec<TModel, TMsg>>> program, FSharpRef<FSharpOption<ViewModel<TModel, TMsg>>> lastModel)
        {
            void SetState(TModel model, FSharpFunc<TMsg, Unit> dispatch)
            {
                FSharpOption<ViewModel<TModel, TMsg>> contents = lastModel.contents;
                if (contents != null)
                {
                    contents.Value.UpdateModel(model);
                    return;
                }
                var bindedModel = ProgramModule.view(program).Invoke(model);
                var bindingSpecs = bindedModel.Invoke(dispatch);
                var viewModel = new ViewModel<TModel, TMsg>(model, dispatch, bindingSpecs, config);
                element.DataContext = viewModel;
                lastModel.contents = FSharpOption<ViewModel<TModel, TMsg>>.Some(viewModel);
            }
            return SetState;
        }

        private static Action<FSharpFunc<TMsg, Unit>, TMsg> MakeSyncDispatch<TMsg>(FrameworkElement element)
        {
            void UiDispatch(FSharpFunc<TMsg, Unit> innerDispatch, TMsg msg)
            {
                void DoDispatch(TMsg m)
                {
                    Console.WriteLine("Dispatch");
                    innerDispatch.Invoke(m);
                }

                element.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => DoDispatch(msg));
            }

            return UiDispatch;
        }
    }
}
