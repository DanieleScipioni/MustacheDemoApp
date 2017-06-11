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
        private static readonly List<Type> InternalTypes = new List<Type>(5)
        {
            typeof(int),
            typeof(decimal),
            typeof(bool),
            typeof(string),
            typeof(List<object>)
        };

        private static readonly List<string> ManagedTypes = (from t in InternalTypes
                                                             select t.FullName).ToList();

        #region Backing fields

        private int _selectedTypeIndex;
        private string _stringValue;
        private bool _boolValue;
        private bool _isValueValid;
        private bool _toggleSwitchVisible;
        private bool _textBoxVisible;

        #endregion

        #region Binding properties

        public object Value { get; private set; }

        public string Key { get; set; }

        public bool KeyReadOnly { get; }

        public List<string> Types => ManagedTypes;

        public int SelectedTypeIndex
        {
            get { return _selectedTypeIndex; }
            set
            {
                if (!SetProperty(ref _selectedTypeIndex, value)) return;

                Type selectedType = InternalTypes[_selectedTypeIndex];

                ToggleSwitchVisible = selectedType == typeof(bool);
                TextBoxVisible = selectedType != typeof(bool) && selectedType != typeof(List<object>);

                EvaluateValueAndType();
            }
        }

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

        public bool IsValueValid
        {
            get { return _isValueValid; }
            private set { SetProperty(ref _isValueValid, value); }
        }

        public bool ToggleSwitchVisible
        {
            get { return _toggleSwitchVisible; }
            private set { SetProperty(ref _toggleSwitchVisible, value); }
        }

        public bool TextBoxVisible
        {
            get { return _textBoxVisible; }
            set { SetProperty(ref _textBoxVisible, value); }
        }

        #endregion

        public EditDataUserControlViewModel() : this(null, null, true) {}

        public EditDataUserControlViewModel(string key, object value, bool canEditKey)
        {
            KeyReadOnly = !canEditKey;
            if (value == null)
            {
                _selectedTypeIndex = -1;
            }
            else
            {
                Type selectedType = value.GetType();
                _selectedTypeIndex = InternalTypes.IndexOf(selectedType);

                ToggleSwitchVisible = selectedType == typeof(bool);
                TextBoxVisible = selectedType != typeof(bool) && selectedType != typeof(List<object>);

                if (ToggleSwitchVisible)
                {
                    _boolValue = (bool) value;
                }

                Value = value;
                _stringValue = value.ToString();
                EvaluateValueAndType();
            }
            Key = key;
        }

        public bool IsInputValid()
        {
            return !string.IsNullOrEmpty(Key) && SelectedTypeIndex != -1 && _isValueValid;
        }

        private void EvaluateValueAndType()
        {
            Type selectedType = InternalTypes[_selectedTypeIndex];
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
            else if (selectedType == typeof(List<object>))
            {
                IsValueValid = true;
                Value = new List<object>();
            }
            else
            {
                IsValueValid = true;
                Value = _stringValue;
            }
        }
    }
}