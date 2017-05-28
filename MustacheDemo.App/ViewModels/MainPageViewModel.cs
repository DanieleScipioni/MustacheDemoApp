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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MustacheDemo.App.ViewModels
{
    internal class KeyValue : BindableBase
    {
        private readonly KeyValuePair<string, object> _keyValuePair;
        public string Key { get; }
        public object Value { get; }

        public KeyValue(KeyValuePair<string, object> keyValuePair)
        {
            _keyValuePair = keyValuePair;
            Key = keyValuePair.Key;
            object value = keyValuePair.Value;
            var enumerable = value as IList;
            Value = enumerable == null ? value.ToString() : $"[{enumerable.Count}]";
        }
    }

    internal class MainPageViewModel : BindableBase
    {
        #region Binding properties

        private string _template;
        public string Template
        {
            get { return _template; }
            set
            {
                if (SetProperty(ref _template, value))
                {
                    bool templateEmpty = string.IsNullOrEmpty(_template);
                    if (_canRender && templateEmpty || !_canRender && !templateEmpty)
                    {
                        _canRender = !templateEmpty;
                        RenderCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        private bool _renderCompleted;
        public bool RenderCompleted
        {
            get { return _renderCompleted; }
            private set { SetProperty(ref _renderCompleted, value); }
        }

        public ObservableCollection<object> Data { get; } = new ObservableCollection<object>();

        public readonly DelegateCommand RenderCommand;

        public readonly DelegateCommand EditTemplateCommand;

        public readonly DelegateCommand AddDataCommand;

        public readonly DelegateCommand EditDataCommand;

        public readonly DelegateCommand RemoveDataCommand;

        #endregion

        private string _templateCache;
        private Dictionary<string, object> _data;
        private bool _canRender;

        public MainPageViewModel()
        {
            RenderCommand = new DelegateCommand(Render, RenderCanExecute);
            EditTemplateCommand = new DelegateCommand(EditTemplate, EditTemplateCanExecute);
            AddDataCommand = new DelegateCommand(AddData);
            EditDataCommand = new DelegateCommand(EditData);
            RemoveDataCommand = new DelegateCommand(RemoveData);

            _data = new Dictionary<string, object>
            {
                {"Name", "Chris"},
                {"Value", 10000},
                {"TaxedValue", 6000},
                {"Currency", "dollars"},
                {"InCa", true},
                {"List", new List<object> {
                    "a", "b", 1
                }}
            };

            foreach (KeyValue item in DataToList(_data))
            {
                Data.Add(item);
            }

            _canRender = true;
            _template = @"Hello {{Name}}
You have just won {{Value}} {{Currency}}!
{{#InCa}}
Well, {{TaxedValue}} {{Currency}}, after taxes.
{{/InCa}}";
        }

        private static IEnumerable<KeyValue> DataToList(Dictionary<string, object> data)
        {
            foreach (KeyValuePair<string, object> keyValuePair in data)
            {
                yield return new KeyValue(keyValuePair);
            }
        }

        #region Command implementations

        private void Render(object parameter)
        {
            if (Data == null) return;
            
            _templateCache = Template;
            Template = Mustache.Template.Compile(_templateCache.Replace("\r", Environment.NewLine)).Render(_data);
            RenderCompleted = true;
            RenderCommand.RaiseCanExecuteChanged();
            EditTemplateCommand.RaiseCanExecuteChanged();
        }

        private bool RenderCanExecute(object parameter)
        {
            return _canRender && !RenderCompleted;
        }

        private void EditTemplate(object parameter)
        {
            Template = _templateCache;
            _templateCache = null;
            RenderCompleted = false;
            RenderCommand.RaiseCanExecuteChanged();
            EditTemplateCommand.RaiseCanExecuteChanged();
        }

        private bool EditTemplateCanExecute(object parameter)
        {
            return RenderCompleted;
        }

        private void AddData(object parameter)
        {

        }

        private void RemoveData(object obj)
        {
            throw new NotImplementedException();
        }

        private void EditData(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
