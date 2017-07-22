// ******************************************************************************
// MIT License
// 
// Copyright (c) 2017 Daniele Scipioni
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ******************************************************************************

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using MustacheDemo.App.UserControls.Splash;
using MustacheDemo.App.ViewModels;
using MustacheDemo.Core.Database.Schema;
using MustacheDemo.Data;

namespace MustacheDemo.App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            //// Do not repeat app initialization when the Window already has content,
            //// just ensure that the window is active
            Window window = Window.Current;
            if (window.Content != null) return;

            DatabaseSchemaManager schemaManager = Schema.GetMustacheDemoDatabaseSchemaManager();
            if (schemaManager.NeedsUpgrade)
            {
                var splashControlViewModel = new SplashControlViewModel();
                window.Content = new SplashControl
                {
                    DataContext = splashControlViewModel
                };

#pragma warning disable CS4014
                Window.Current.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        StartSchemaUpgrade(schemaManager, splashControlViewModel);
                    }
                );
#pragma warning restore CS4014
            }
            else
            {
                window.Content = new MainPage();
            }

            if (e.PrelaunchActivated == false) window.Activate();

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private static void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        private static async void StartSchemaUpgrade(DatabaseSchemaManager schemaManager, SplashControlViewModel splashControlViewModel)
        {
            Task Provider(CancellationToken s, IProgress<Tuple<long, long>> progress)
            {
                return Task.Run(() => schemaManager.Upgrade(progress));
            }

            IAsyncActionWithProgress<Tuple<long, long>> asyncActionWithProgress =
                AsyncInfo.Run((Func<CancellationToken, IProgress<Tuple<long, long>>, Task>)Provider);

            splashControlViewModel.Text = "Database upgrade";
            asyncActionWithProgress.Progress += (info, progressInfo) =>
            {
                splashControlViewModel.Indeterminate = false;
                splashControlViewModel.Total = progressInfo.Item1;
                splashControlViewModel.Partial = progressInfo.Item2;
            };
            await asyncActionWithProgress;
            Window.Current.Content = new MainPage();
        }

    }
}
