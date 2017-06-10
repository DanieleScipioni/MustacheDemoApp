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

using MustacheDemo.App.Bridges;
using MustacheDemo.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Media.Animation;

namespace MustacheDemo.App.ViewModels
{
    internal interface IKeyValueDataService
    {
        void UpdateDataValue(string key, object value);
        void EditValue(string key);
    }

    internal class MainPageViewModel : BindableBase, IKeyValueDataService
    {
        #region Propery backing fields

        private string _template;
        private bool _renderCompleted;
        private object _selectedData;

        #endregion

        #region Binding properties

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

        public bool RenderCompleted
        {
            get { return _renderCompleted; }
            private set { SetProperty(ref _renderCompleted, value); }
        }

        public ObservableCollection<object> Data { get; } = new ObservableCollection<object>();

        public object SelectedData
        {
            get { return _selectedData; }
            set
            {
                if (SetProperty(ref _selectedData, value))
                {
                    EditDataCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public int SelectedIndex { get; set; }

        public readonly DelegateCommand RenderCommand;

        public readonly DelegateCommand EditTemplateCommand;

        public readonly DelegateCommand AddDataCommand;

        public readonly DelegateCommand EditDataCommand;

        public readonly DelegateCommand RemoveDataCommand;

        public readonly DelegateCommand UpCommand;

        #endregion

        private Dictionary<string, object> _data;

        private string _templateCache;
        private readonly Stack<object> _dataStack;
        private bool _canRender;
        private readonly MainPageViewModelService _mainPageViewModelService;

        public MainPageViewModel(MainPageViewModelService mainPageViewModelService)
        {
            _mainPageViewModelService = mainPageViewModelService;
            RenderCommand = new DelegateCommand(Render, RenderCanExecute);
            EditTemplateCommand = new DelegateCommand(EditTemplate, EditTemplateCanExecute);
            AddDataCommand = new DelegateCommand(AddData);
            EditDataCommand = new DelegateCommand(EditData);
            RemoveDataCommand = new DelegateCommand(RemoveData);
            UpCommand = new DelegateCommand(Up, UpCanExecute);

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

            _dataStack = new Stack<object>();

            _canRender = true;
            _template = @"Hello {{Name}}
You have just won {{Value}} {{Currency}}!
{{#InCa}}
Well, {{TaxedValue}} {{Currency}}, after taxes.
{{/InCa}}
{{#List}}{{.}}{{/List}}";

            SelectedIndex = -1;

            SetContext(_data);
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

        private async void AddData(object parameter)
        {
            var dictionary = _dataStack.Peek() as IDictionary;
            if (dictionary == null) return;

            Tuple<string, object> tuple = await _mainPageViewModelService.NewData();
            if (tuple == null) return;

            Data.Add(new KeyValue(tuple.Item1, tuple.Item2, this));

            dictionary.Add(tuple.Item1, tuple.Item2);
        }

        private async void EditData(object parameter)
        {
            var keyValue = SelectedData as KeyValue;
            if (keyValue == null) return;

            if (keyValue.Value is List<object>)
            {
                SetContext(keyValue.Value);
                return;
            }

            var tuple = new Tuple<string, object>(keyValue.Key, keyValue.Value);
            Tuple<string, object> newTuple = await _mainPageViewModelService.EditData(tuple);
            if (newTuple == null) return;

            var dictionary = _dataStack.Peek() as IDictionary;
            if (dictionary == null) return;

            if (tuple.Item1 != newTuple.Item1) dictionary.Remove(tuple.Item1);
            
            dictionary[newTuple.Item1] = newTuple.Item2;

            if (newTuple.Item2.GetType() == tuple.Item2.GetType())
            {
                keyValue.Key = newTuple.Item1;
                keyValue.Value = newTuple.Item2;
            }
            else
            {
                int selectedIndex = SelectedIndex;
                Data.RemoveAt(selectedIndex);
                Data.Insert(selectedIndex, new KeyValue(newTuple.Item1, newTuple.Item2, this));
            }
        }

        private void RemoveData(object parameter)
        {
            var keyValue = SelectedData as KeyValue;
            if (keyValue == null) return;

            var dictionary = _dataStack.Peek() as IDictionary;
            if (dictionary == null) return;

            Data.RemoveAt(SelectedIndex);

            dictionary.Remove(keyValue.Key);
        }

        private bool UpCanExecute(object arg)
        {
            return _dataStack.Count > 1;
        }

        private void Up(object obj)
        {
            ToParetContext();
        }

        #endregion

        #region IKeyValueDataService

        public void UpdateDataValue(string key, object value)
        {
            var dictionary = _dataStack.Peek() as IDictionary;
            if (dictionary == null) return;

            dictionary[key] = value;
        }

        public void EditValue(string key)
        {
            var dictionary = _dataStack.Peek() as IDictionary;
            if (dictionary == null) return;

            object value = dictionary[key];
            if (value is List<object>)
            {
                SetContext(value);
                return;
            }
            if (value is IDictionary)
            {
                return;
            }
        }

        #endregion

        private void ToParetContext()
        {
            _dataStack.Pop();
            UpdateContext(_dataStack.Peek());
            UpCommand.RaiseCanExecuteChanged();
        }

        private void SetContext(object obj)
        {
            _dataStack.Push(obj);
            UpdateContext(obj);
            UpCommand.RaiseCanExecuteChanged();
        }

        private void UpdateContext(object context)
        {
            var dictionary = context as Dictionary<string, object>;
            if (dictionary != null)
            {
                Data.Clear();
                foreach (KeyValue keyValue in DataToList(dictionary))
                {
                    Data.Add(keyValue);
                }
            }

            var list = context as List<object>;
            if (list != null)
            {
                List<KeyValue> keyValues = list.Select((item, index) => new KeyValue(index.ToString(), item, this)).ToList();
                Data.Clear();
                foreach (KeyValue keyValue in keyValues)
                {
                    Data.Add(keyValue);
                }
            }
        }

        private IEnumerable<KeyValue> DataToList(Dictionary<string, object> data)
        {
            foreach (KeyValuePair<string, object> keyValuePair in data)
            {
                if (keyValuePair.Value is IList)
                {
                    yield return new KeyList(keyValuePair.Key, keyValuePair.Value, this);
                }
                else
                {
                    yield return new KeyValue(keyValuePair.Key, keyValuePair.Value, this);
                }
            }
        }
    }
}
