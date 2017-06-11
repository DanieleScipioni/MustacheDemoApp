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

namespace MustacheDemo.App.ViewModels
{
    internal interface IContextEntryDataService
    {
        void UpdateDataValue(string key, object value);
        void EditContextEntry(ContextEntry contextEntry);
    }

    internal class MainPageViewModel : BindableBase, IContextEntryDataService
    {
        #region Propery backing fields

        private string _template;
        private bool _renderCompleted;

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

        public int SelectedIndex { get; set; }

        public readonly DelegateCommand RenderCommand;

        public readonly DelegateCommand EditTemplateCommand;

        public readonly DelegateCommand AddDataCommand;

        public readonly DelegateCommand EditSelectedDataCommand;

        public readonly DelegateCommand RemoveDataCommand;

        public readonly DelegateCommand UpCommand;

        #endregion

        private Dictionary<string, object> _data;

        private string _templateCache;
        private readonly Stack<object> _dataStack;
        private bool _canRender;
        private readonly DataService _dataService;

        public MainPageViewModel(DataService dataService)
        {
            _dataService = dataService;
            RenderCommand = new DelegateCommand(Render, RenderCanExecute);
            EditTemplateCommand = new DelegateCommand(EditTemplate, EditTemplateCanExecute);
            AddDataCommand = new DelegateCommand(AddData);
            EditSelectedDataCommand = new DelegateCommand(EditSelectedData);
            RemoveDataCommand = new DelegateCommand(RemoveData);
            UpCommand = new DelegateCommand(Up, UpCanExecute);

            _data = new Dictionary<string, object>
            {
                {"Name", "Chris"},
                {"Value", 10000},
                {"TaxedValue", 6000m},
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

            Tuple<string, object> tuple = await _dataService.NewData();
            if (tuple == null) return;

            Data.Add(new ContextEntry(tuple.Item1, tuple.Item2, this));

            dictionary.Add(tuple.Item1, tuple.Item2);
        }

        private void EditSelectedData(object parameter)
        {
            var keyValue = Data[SelectedIndex] as ContextEntry;
            if (keyValue == null) return;

            EditContextEntry(keyValue);
        }

        private void RemoveData(object parameter)
        {
            var keyValue = Data[SelectedIndex] as ContextEntry;
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

        #region IContextEntryDataService

        public void UpdateDataValue(string key, object value)
        {
            var dictionary = _dataStack.Peek() as IDictionary;
            if (dictionary == null) return;

            dictionary[key] = value;
        }

        public async void EditContextEntry(ContextEntry contextEntry)
        {
            var list = contextEntry.Value as IList<object>;
            if (list != null)
            {
                SetContext(list);
                return;
            }

            object currentContext = _dataStack.Peek();
            var dictionary = currentContext as IDictionary<string, object>;
            var listContext = currentContext as IList<object>;

            var tuple = new Tuple<string, object>(contextEntry.Key, contextEntry.Value);
            Tuple<string, object> newTuple = await _dataService.EditData(tuple, listContext != null);
            if (newTuple == null) return;

            if (newTuple.Item2.GetType() == tuple.Item2.GetType())
            {
                contextEntry.Key = newTuple.Item1;
                contextEntry.Value = newTuple.Item2;
            }
            else
            {
                int index = Data.IndexOf(contextEntry);
                Data.RemoveAt(index);
                Data.Insert(index, new ContextEntry(newTuple.Item1, newTuple.Item2, this));
            }


            if (dictionary != null)
            {
                SetDictionaryData(dictionary, tuple, newTuple);
            }

            if (listContext == null) return;

            SetListData(listContext, tuple, newTuple);
        }

        private static void SetDictionaryData(IDictionary<string, object> dictionary,
            Tuple<string, object> tuple, Tuple<string, object> newTuple)
        {
            if (tuple.Item1 != newTuple.Item1) dictionary.Remove(tuple.Item1);
            dictionary[newTuple.Item1] = newTuple.Item2;
        }

        private static void SetListData(IList<object> list,
            Tuple<string, object> tuple, Tuple<string, object> newTuple)
        {
            int index = list.IndexOf(tuple.Item2);
            list[index] = newTuple.Item2;
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
                foreach (ContextEntry keyValue in DataToList(dictionary))
                {
                    Data.Add(keyValue);
                }
            }

            var list = context as List<object>;
            if (list != null)
            {
                List<ContextEntry> keyValues = list.Select((item, index) => new ContextEntry(index.ToString(), item, this)).ToList();
                Data.Clear();
                foreach (ContextEntry keyValue in keyValues)
                {
                    Data.Add(keyValue);
                }
            }
        }

        private IEnumerable<ContextEntry> DataToList(Dictionary<string, object> data)
        {
            return data.Select(keyValuePair => new ContextEntry(keyValuePair.Key, keyValuePair.Value, this));
        }
    }
}
