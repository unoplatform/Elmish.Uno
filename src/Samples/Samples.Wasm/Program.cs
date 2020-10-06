using System;

using Windows.UI.Xaml;

namespace Samples.Wasm
{
    public class Program
    {
        private static App app;

        public static int Main(string[] args)
        {
            Windows.UI.Xaml.Application.Start(_ => app = new App());

            return 0;
        }
    }
}
