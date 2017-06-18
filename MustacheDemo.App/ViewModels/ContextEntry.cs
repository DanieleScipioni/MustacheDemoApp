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

using MustacheDemo.Core;
using MustacheDemo.Core.Data;
using System;
using Windows.UI.Xaml.Media;
using MustacheDemo.App.Converters;

namespace MustacheDemo.App.ViewModels
{
    internal interface IContextEntryDataService
    {
        void UpdateDataValue(string key, object value);
        void EditContextEntry(ContextEntry contextEntry);
    }

    internal class ContextEntry : BindableBase
    {
        private readonly IContextEntryDataService _contextEntryDataService;

        #region backing fields

        private string _key;
        private object _value;

        #endregion

        #region bindable properties

        public string Key
        {
            get => _key;
            set => SetProperty(ref _key, value);
        }

        public object Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, value))
                {
                    _contextEntryDataService.UpdateDataValue(_key, _value);
                }
            }
        }

        public DelegateCommand EditCommand { get; }

        public string IconText { get; } 

        public FontFamily FontFamily { get; }

        #endregion

        public ContextEntry(string key, object value, IContextEntryDataService contextEntryDataService)
        {
            _key = key;
            _value = value;
            _contextEntryDataService = contextEntryDataService;
            EditCommand = new DelegateCommand(EditCommandImpl);
            Type type = _value.GetType();
            IconText = TypeToSymbolConverter.TypeToSymbolString(type);
            FontFamily = TypeToSymbolConverter.TypeToFontFamily(type);
        }

        private void EditCommandImpl(object parameter)
        {
            _contextEntryDataService.EditContextEntry(this);
        }
    }
}