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

using MustacheDemo.App.Bridges;
using MustacheDemo.App.UserControls.Splash;
using MustacheDemo.App.ViewModels;
using MustacheDemo.Core.Database.Schema;
using MustacheDemo.Data;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MustacheDemo.App
{
    public class ViewModelLocator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            object viewModel = null;

            if (value is MainPage) viewModel = new MainPageViewModel(new DataService());

            if (value is Control control)
            if (viewModel is IDisposable disposable)
            {
                SetupViewModelDispose(control, disposable);
            }

            return viewModel;
        }

        private static void SetupViewModelDispose(Control control, IDisposable disposable)
        {
            void UnloadedHandler (object sender, RoutedEventArgs args)
            {
                var senderControl = (Control) sender;
                senderControl.Unloaded -= UnloadedHandler;
                disposable.Dispose();
            }

            control.Unloaded += UnloadedHandler;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public async void AppOnLauched(LaunchActivatedEventArgs e)
        {
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

                Task Provider(CancellationToken s, IProgress<Tuple<long, long>> progress)
                {
                    return Task.Run(() => schemaManager.Upgrade(progress));
                }

                IAsyncActionWithProgress<Tuple<long, long>> asyncActionWithProgress =
                    AsyncInfo.Run((Func<CancellationToken, IProgress<Tuple<long, long>>, Task>) Provider);

                splashControlViewModel.Text = "Database upgrade";
                asyncActionWithProgress.Progress += (info, progressInfo) =>
                {
                    splashControlViewModel.Indeterminate = false;
                    splashControlViewModel.Total = progressInfo.Item1;
                    splashControlViewModel.Partial = progressInfo.Item2;
                };

                if (e.PrelaunchActivated == false) window.Activate();
                await asyncActionWithProgress;
                window.Content = new MainPage();
            }
            else
            {
                window.Content = new MainPage();
                if (e.PrelaunchActivated == false) window.Activate();
            }

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }
        }

        public void AppOnSuspending(SuspendingEventArgs suspendingEventArgs)
        {
        }
    }
}
