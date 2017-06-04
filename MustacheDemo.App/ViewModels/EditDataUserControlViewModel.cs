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
using System;
using System.Collections.Generic;
using System.Linq;

namespace MustacheDemo.App.ViewModels
{
    public class EditDataUserControlViewModel : BindableBase
    {
        private static readonly List<Type> ManagedTypes = new List<Type>(4)
        {
            typeof(int),
            typeof(decimal),
            typeof(bool),
            typeof(string)
        };

        private Type _selectedType;

        public object RawValue { get; private set; }

        public string Key { get; set; }

        public List<string> Types { get; }

        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                if (!SetProperty(ref _value, value)) return;
                EvaluateValueAndType();
            }
        }

        private int _selectedTypeIndex;

        public int SelectedTypeIndex
        {
            get { return _selectedTypeIndex; }
            set
            {
                if (!SetProperty(ref _selectedTypeIndex, value)) return;

                _selectedType = ManagedTypes[_selectedTypeIndex];
                EvaluateValueAndType();
            }
        }

        private bool _isValueValid;
        public bool IsValueValid
        {
            get { return _isValueValid; }
            private set { SetProperty(ref _isValueValid, value); }
        }

        public EditDataUserControlViewModel() : this(null, null) {}

        public EditDataUserControlViewModel(string key, object value)
        {
            Types = (from type in ManagedTypes select type.Name).ToList();

            if (value == null)
            {
                _selectedTypeIndex = -1;
            }
            else
            {
                _selectedType = value.GetType();
                _selectedTypeIndex = ManagedTypes.IndexOf(_selectedType);
            }


            Key = key;
            Value = value?.ToString();
        }

        public bool IsInputValid()
        {
            return !string.IsNullOrEmpty(Key) && SelectedTypeIndex != -1 && _isValueValid;
        }

        private void EvaluateValueAndType()
        {
            if (_selectedType == typeof(int))
            {
                int parsed;
                IsValueValid = int.TryParse(_value, out parsed);
                if (_isValueValid)
                {
                    RawValue = parsed;
                }
            }
            else if (_selectedType == typeof(decimal))
            {
                decimal parsed;
                IsValueValid = decimal.TryParse(_value, out parsed);
                if (_isValueValid)
                {
                    RawValue = parsed;
                }
            }
            else if (_selectedType == typeof(bool))
            {
                bool parsed;
                IsValueValid = bool.TryParse(_value, out parsed);
                if (_isValueValid)
                {
                    RawValue = parsed;
                }
            }
            else
            {
                IsValueValid = true;
                RawValue = _value;
            }
        }
    }
}