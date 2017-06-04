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

        public object Value { get; private set; }

        public string Key { get; set; }

        public List<string> Types { get; }

        private int _selectedTypeIndex;
        public int SelectedTypeIndex
        {
            get { return _selectedTypeIndex; }
            set
            {
                if (!SetProperty(ref _selectedTypeIndex, value)) return;

                ToggleSwitchVisible = ManagedTypes[_selectedTypeIndex] == typeof(bool);
                EvaluateValueAndType();
            }
        }

        private string _stringValue;
        public string StringValue
        {
            get { return _stringValue; }
            set
            {
                if (SetProperty(ref _stringValue, value))
                {
                    EvaluateValueAndType();
                }
            }
        }

        private bool _boolValue;
        public bool BoolValue
        {
            get { return _boolValue; }
            set
            {
                if (SetProperty(ref _boolValue, value))
                {
                    EvaluateValueAndType();
                }
            }
        }

        private bool _isValueValid;
        public bool IsValueValid
        {
            get { return _isValueValid; }
            private set { SetProperty(ref _isValueValid, value); }
        }

        private bool _toggleSwitchVisible;

        public bool ToggleSwitchVisible
        {
            get { return _toggleSwitchVisible; }
            private set
            {
                if (SetProperty(ref _toggleSwitchVisible, value))
                {
                    OnPropertyChangedByName(nameof(TextBoxVisible));
                }
            }
        }

        public bool TextBoxVisible => !_toggleSwitchVisible;

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
                Type selectedType = value.GetType();
                _selectedTypeIndex = ManagedTypes.IndexOf(selectedType);
                bool isBoolValue = selectedType == typeof(bool);
                ToggleSwitchVisible = isBoolValue;
                if (isBoolValue)
                {
                    BoolValue = (bool) value;
                }
                Value = value;
                StringValue = value.ToString();
                EvaluateValueAndType();
            }
        }

        public bool IsInputValid()
        {
            return !string.IsNullOrEmpty(Key) && SelectedTypeIndex != -1 && _isValueValid;
        }

        private void EvaluateValueAndType()
        {
            Type selectedType = ManagedTypes[_selectedTypeIndex];
            if (selectedType == typeof(int))
            {
                int parsed;
                IsValueValid = int.TryParse(_stringValue, out parsed);
                if (_isValueValid)
                {
                    Value = parsed;
                }
            }
            else if (selectedType == typeof(decimal))
            {
                decimal parsed;
                IsValueValid = decimal.TryParse(_stringValue, out parsed);
                if (_isValueValid)
                {
                    Value = parsed;
                }
            }
            else if (selectedType == typeof(bool))
            {
                IsValueValid = true;
                Value = _boolValue;
            }
            else
            {
                IsValueValid = true;
                Value = _stringValue;
            }
        }
    }
}