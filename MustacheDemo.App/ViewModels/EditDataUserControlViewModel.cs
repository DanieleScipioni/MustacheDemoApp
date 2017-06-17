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

        public bool NeedsKey { get; }

        public List<string> Types { get; private set; }

        public int SelectedTypeIndex
        {
            get => _selectedTypeIndex;
            set
            {
                if (!SetProperty(ref _selectedTypeIndex, value)) return;

                Type selectedType = _internalTypes[_selectedTypeIndex];
                SetupValueFieldsVisibility(selectedType);

                EvaluateValueAndType();
            }
        }

        public string StringValue
        {
            get => _stringValue;
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
            get => _boolValue;
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
            get => _isValueValid;
            private set => SetProperty(ref _isValueValid, value);
        }

        public bool ToggleSwitchVisible
        {
            get => _toggleSwitchVisible;
            private set => SetProperty(ref _toggleSwitchVisible, value);
        }

        public bool TextBoxVisible
        {
            get => _textBoxVisible;
            private set => SetProperty(ref _textBoxVisible, value);
        }

        #endregion

        private List<Type> _internalTypes;

        public EditDataUserControlViewModel(bool canEditKey) : this(null, null, canEditKey) {}

        public EditDataUserControlViewModel(string key, object value, bool withKey)
        {
            NeedsKey = withKey;
            Type selectedType;
            if (value == null)
            {
                selectedType = null;
                _selectedTypeIndex = -1;
            }
            else
            {
                selectedType = value.GetType();
            }
            SetupTypes(selectedType);
            _selectedTypeIndex = _internalTypes.IndexOf(selectedType);

            SetupValueFieldsVisibility(selectedType);

            if (value is bool) _boolValue = (bool) value;
            Value = value;
            _stringValue = value?.ToString();

            EvaluateValueAndType();
            Key = key;
        }

        private void SetupTypes(Type selectedType)
        {
            if (selectedType == null)
            {
                _internalTypes = new List<Type>(5)
                {
                    typeof(int),
                    typeof(decimal),
                    typeof(bool),
                    typeof(string),
                    typeof(List<object>),
                    typeof(Dictionary<string, object>)
                };
            }
            else if (selectedType == typeof(List<object>) || selectedType == typeof(Dictionary<string, object>))
            {
                _internalTypes = new List<Type>(1) {selectedType};
            }
            else
            {
                _internalTypes = new List<Type>(5)
                {
                    typeof(int),
                    typeof(decimal),
                    typeof(bool),
                    typeof(string),
                };
            }
            Types = (from t in _internalTypes
                     select t.FullName).ToList();
        }

        private void SetupValueFieldsVisibility(Type selectedType)
        {
            ToggleSwitchVisible = selectedType == typeof(bool);
            TextBoxVisible = selectedType != typeof(bool) && selectedType != typeof(List<object>) && selectedType != typeof(Dictionary<string, object>);
        }

        public bool IsInputValid()
        {
            return (!NeedsKey || !string.IsNullOrEmpty(Key)) && SelectedTypeIndex != -1 && _isValueValid;
        }

        private void EvaluateValueAndType()
        {
            if (_selectedTypeIndex == -1) return;

            Type selectedType = _internalTypes[_selectedTypeIndex];
            if (selectedType == typeof(int))
            {
                IsValueValid = int.TryParse(_stringValue, out int parsed);
                if (_isValueValid)
                {
                    Value = parsed;
                }
            }
            else if (selectedType == typeof(decimal))
            {
                IsValueValid = decimal.TryParse(_stringValue, out decimal parsed);
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
            else if (selectedType == typeof(Dictionary<string, object>))
            {
                IsValueValid = true;
                Value = new Dictionary<string, object>();
            }
            else
            {
                IsValueValid = true;
                Value = _stringValue;
            }
        }
    }
}