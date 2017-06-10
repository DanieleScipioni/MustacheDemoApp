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

using MustacheDemo.App.UserControls;
using MustacheDemo.App.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace MustacheDemo.App.Bridges
{
    internal class DataService
    {
        public async Task<Tuple<string, object>> NewData()
        {
            var editDataUserControlViewModel = new EditDataUserControlViewModel();
            var contentDialog = new ContentDialog
            {
                Content = new EditDataUserControl { DataContext = editDataUserControlViewModel },
                PrimaryButtonText = "OK",
                SecondaryButtonText = "Cancel",
            };
            contentDialog.PrimaryButtonClick += (sender, args) =>
            {
                if (!editDataUserControlViewModel.IsInputValid()) args.Cancel = true;
            };

            ContentDialogResult contentDialogResult = await contentDialog.ShowAsync();
            if (contentDialogResult == ContentDialogResult.Primary)
            {
                return new Tuple<string, object>(
                    editDataUserControlViewModel.Key,
                    editDataUserControlViewModel.Value
                );
            }
            return null;
        }

        public async Task<Tuple<string, object>> EditData(Tuple<string, object> tuple, bool canEditKey)
        {
            var editDataUserControlViewModel = new EditDataUserControlViewModel(tuple.Item1, tuple.Item2, canEditKey);
            var contentDialog = new ContentDialog
            {
                Content = new EditDataUserControl { DataContext = editDataUserControlViewModel },
                PrimaryButtonText = "OK",
                SecondaryButtonText = "Cancel",
            };
            contentDialog.PrimaryButtonClick += (sender, args) =>
            {
                if (!editDataUserControlViewModel.IsInputValid()) args.Cancel = true;
            };

            ContentDialogResult contentDialogResult = await contentDialog.ShowAsync();
            if (contentDialogResult == ContentDialogResult.Primary)
            {
                return new Tuple<string, object>(
                    editDataUserControlViewModel.Key,
                    editDataUserControlViewModel.Value
                );
            }
            return null;
        }
    }
}