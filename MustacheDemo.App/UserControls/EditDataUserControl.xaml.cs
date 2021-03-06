﻿// ******************************************************************************
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

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MustacheDemo.App.ViewModels;

namespace MustacheDemo.App.UserControls
{
    public sealed partial class EditDataUserControl
    {
        private EditDataUserControlViewModel _viewModel;

        public EditDataUserControl()
        {
            InitializeComponent();
            _viewModel = DataContext as EditDataUserControlViewModel;
            DataContextChanged += (sender, args) =>
            {
                if (_viewModel == args.NewValue) return;
                
                _viewModel = args.NewValue as EditDataUserControlViewModel;
                Bindings.Update();
                args.Handled = true;
            };
        }

        private void TextBoxKey_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.Key = ((TextBox) sender).Text;
        }

        private void TextBoxValue_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.StringValue = ((TextBox)sender).Text;
        }
    }
}
