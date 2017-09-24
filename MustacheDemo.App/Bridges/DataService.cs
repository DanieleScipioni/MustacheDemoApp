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

using Microsoft.Data.Sqlite;
using MustacheDemo.App.UserControls;
using MustacheDemo.App.ViewModels;
using MustacheDemo.Core.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using MustacheDemo.Data;

namespace MustacheDemo.App.Bridges
{
    internal class DataService
    {
        public async Task<List<Template>> GetTemplates()
        {
            using (SqliteConnection connection = DatabaseConnectionManager.GetMustacheDemoConnection())
            {
                connection.Open();
                return await Template.GetTemplates(connection);
            }
        }

        public async Task<Tuple<string, object>> NewData(bool canEditKey)
        {
            var editDataUserControlViewModel = new EditDataUserControlViewModel(canEditKey);
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

        public async Task SaveTemplate(Template template)
        {
            using (SqliteConnection connection = DatabaseConnectionManager.GetMustacheDemoConnection())
            {
                connection.Open();
                await Template.SaveTemplate(connection, template);
            }
        }
    }
}