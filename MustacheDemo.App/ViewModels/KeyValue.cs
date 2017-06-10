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

namespace MustacheDemo.App.ViewModels
{
    internal class KeyValue : BindableBase
    {
        private string _key;
        private object _value;
        private readonly IKeyValueDataService _keyValueDataService;

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                if (SetProperty(ref _value, value))
                {
                    _keyValueDataService.UpdateDataValue(_key, _value);
                }
            }
        }

        public readonly DelegateCommand EditCommand;

        public KeyValue(string key, object value, IKeyValueDataService keyValueDataService)
        {
            _key = key;
            _value = value;
            _keyValueDataService = keyValueDataService;
            EditCommand = new DelegateCommand(EditCommandImpl);
        }

        private void EditCommandImpl(object parameter)
        {
            _keyValueDataService.EditKeyValue(this);
        }
    }
}