using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Elmish.Uno.Utilities;

using Microsoft.FSharp.Core;

using Samples.NewWindow;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using ElmishProgram = Elmish.Uno.Samples.NewWindow.Program;

namespace Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewWindowPage : Page
    {
        public NewWindowPage()
        {
            this.InitializeComponent();
            var program = ElmishProgram.CreateProgram<Window1Page, Window2Page>(FuncConvert.FromFunc(() => this.DataContext));
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, program);
        }
    }
}
