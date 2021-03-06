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

using Mustache;
using MustacheDemo.App.Bridges;
using MustacheDemo.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Template = MustacheDemo.Data.Template;

namespace MustacheDemo.App.ViewModels
{
    internal class MainPageViewModel : BindableBase, IContextEntryDataService
    {
        #region Properties backing fields

        private string _templateText;
        private bool _renderCompleted;
        private string _contextKey;
        private bool _canReorderItems;

        #endregion

        #region Binding properties

        public string TemplateText
        {
            get => _templateText;
            set
            {
                if (SetProperty(ref _templateText, value))
                {
                    _canSave = !_renderCompleted;
                    SaveTemplateCommand.RaiseCanExecuteChanged();
                    bool templateEmpty = string.IsNullOrEmpty(_templateText);
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
            get => _renderCompleted;
            private set => SetProperty(ref _renderCompleted, value);
        }

        public ObservableCollection<ContextEntry> Data { get; } = new ObservableCollection<ContextEntry>();

        public int SelectedIndex { get; set; }

        public string ContextKey
        {
            get => _contextKey;
            private set
            {
                if (SetProperty(ref _contextKey, value))
                {
                    OnPropertyChangedByName(nameof(Context));
                }
            }
        }

        public bool CanReorderItems
        {
            get => _canReorderItems;
            private set => SetProperty(ref _canReorderItems, value);
        }

        public bool Reordering
        {
            set
            {
                if (value) _notifyCollectionChangedReorder.StartTracking();
                else _notifyCollectionChangedReorder.StopTracking();
            }
        }

        public object Context => _dataStack.Peek().Value;

        public readonly DelegateCommand RenderCommand;

        public readonly DelegateCommand EditTemplateCommand;

        public readonly DelegateCommand SaveTemplateCommand;

        public readonly DelegateCommand SaveDataCommand;

        public readonly DelegateCommand AddDataCommand;

        public readonly DelegateCommand EditSelectedDataCommand;

        public readonly DelegateCommand RemoveDataCommand;

        public readonly DelegateCommand UpCommand;

        #endregion

        private Dictionary<string, object> _data;

        private readonly DataService _dataService;
        private readonly Stack<KeyValuePair<string, object>> _dataStack;
        private Template _template;
        private bool _canRender;
        private bool _canSave;
        private readonly NotifyCollectionChangedReorder _notifyCollectionChangedReorder;

        public MainPageViewModel(DataService dataService)
        {
            _dataService = dataService;
            RenderCommand = new DelegateCommand(Render, RenderCanExecute);
            EditTemplateCommand = new DelegateCommand(EditTemplate, EditTemplateCanExecute);
            SaveTemplateCommand = new DelegateCommand(SaveTemplateAction, SaveTemplateCanExecute);
            SaveDataCommand = new DelegateCommand(SaveDataAction);

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
                {
                    "List", new List<object>
                    {
                        "a",
                        "b",
                        1
                    }
                },
                {
                    "d", new Dictionary<string, object>
                    {
                        {"e", "funge"},
                        {"tt", true}
                    }
                }
            };

            _dataStack = new Stack<KeyValuePair<string, object>>();

            _canRender = true;

            SelectedIndex = -1;

            SetContext("root", _data);

            _notifyCollectionChangedReorder = new NotifyCollectionChangedReorder(Data, (sender, args) =>
            {
                if (!(_dataStack.Peek().Value is List<object> list)) return;
                object o = list[args.OldIndex];
                list.RemoveAt(args.OldIndex);
                list.Insert(args.NewIndex, o);

                int minIndexToRecalculate = Math.Min(args.OldIndex, args.NewIndex);
                int maxIndexToRecalculate = Math.Max(args.OldIndex, args.NewIndex);
                for (int index = minIndexToRecalculate; index <= maxIndexToRecalculate; index++)
                {
                    Data[index].Key = index.ToString();
                }
            });

            InitFieldsAsync();
        }

        private async void InitFieldsAsync()
        {
            List<Template> templates = await _dataService.GetTemplates();
            if (templates.Count > 0)
            {
                _template = templates[0];
                _templateText = _template.Text;
                OnPropertyChangedByName(nameof(TemplateText));
                SaveTemplateCommand.RaiseCanExecuteChanged();
                RenderCommand.RaiseCanExecuteChanged();
            }
        }


        #region Command implementations

        private void Render(object parameter)
        {
            if (Data == null) return;
            
            RenderCompleted = true;
            try
            {
                TemplateText = Mustache.Template.Compile(_templateText.Replace('\r', '\n')).Render(_data);
            }
            catch (MustacheException e)
            {
                TemplateText = e.Message;
            }
            RenderCommand.RaiseCanExecuteChanged();
            EditTemplateCommand.RaiseCanExecuteChanged();
        }

        private bool RenderCanExecute(object parameter)
        {
            return _canRender && !RenderCompleted;
        }

        private void EditTemplate(object parameter)
        {
            TemplateText = _template.Text;
            RenderCompleted = false;
            RenderCommand.RaiseCanExecuteChanged();
            EditTemplateCommand.RaiseCanExecuteChanged();
        }

        private bool EditTemplateCanExecute(object parameter)
        {
            return RenderCompleted;
        }

        private async void SaveTemplateAction(object parameter)
        {
            _template.Text = _templateText;
            _canSave = false;
            SaveTemplateCommand.RaiseCanExecuteChanged();
            await _dataService.SaveTemplate(_template);
        }

        private bool SaveTemplateCanExecute(object parameter)
        {
            return _canSave;
        }

        private void SaveDataAction(object obj)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(Dictionary<string, object>));
            var stringBuilder = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder))
            {
                dataContractSerializer.WriteObject(xmlWriter, _data);
            }
            string s = stringBuilder.ToString();
        }

        private async void AddData(object parameter)
        {
            KeyValuePair<string, object> currentContext = _dataStack.Peek();
            var dictionary = currentContext.Value as IDictionary;
            var list = currentContext.Value as IList;

            Tuple<string, object> tuple = await _dataService.NewData(list == null);
            if (tuple == null) return;

            if (list != null)
            {
                Data.Add(new ContextEntry(list.Count.ToString(), tuple.Item2, this));
                list.Add(tuple.Item2);
            }
            else if (dictionary != null)
            {
                Data.Add(new ContextEntry(tuple.Item1, tuple.Item2, this));
                dictionary.Add(tuple.Item1, tuple.Item2);
            }

        }

        private void EditSelectedData(object parameter)
        {
            if (SelectedIndex == -1) return;

            ContextEntry keyValue = Data[SelectedIndex];
            if (keyValue == null) return;

            EditContextEntry(keyValue);
        }

        private void RemoveData(object parameter)
        {
            if (SelectedIndex == -1) return;

            var keyValue = Data[SelectedIndex];
            if (keyValue == null) return;

            // store selectedIndex in a local variable because
            // removing from Data cause SelectedIndex to be set to -1
            int selectedIndex = SelectedIndex;
            Data.RemoveAt(selectedIndex);

            if (_dataStack.Peek().Value is IDictionary dictionary) dictionary.Remove(keyValue.Key);
            if (_dataStack.Peek().Value is List<object> list) list.RemoveAt(selectedIndex);
        }

        private bool UpCanExecute(object parameter)
        {
            return _dataStack.Count > 1;
        }

        private void Up(object parameter)
        {
            ToParetContext();
        }

        #endregion

        #region IContextEntryDataService

        public void UpdateDataValue(string key, object value)
        {
            if (!(_dataStack.Peek().Value is IDictionary dictionary)) return;
            dictionary[key] = value;
        }

        public async void EditContextEntry(ContextEntry contextEntry)
        {
            if (contextEntry.Value is IList<object> list)
            {
                SetContext(contextEntry.Key, list);
                return;
            }
            else if (contextEntry.Value is Dictionary<string, object> dictionary)
            {
                SetContext(contextEntry.Key, dictionary);
                return;
            }

            object currentContext = _dataStack.Peek().Value;
            var dictionaryContext = currentContext as IDictionary<string, object>;
            var listContext = currentContext as IList<object>;

            var tuple = new Tuple<string, object>(contextEntry.Key, contextEntry.Value);
            Tuple<string, object> newTuple = await _dataService.EditData(tuple, /*canEditKey*/listContext == null);
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


            if (dictionaryContext != null)
            {
                SetDictionaryData(dictionaryContext, tuple, newTuple);
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
            KeyValuePair<string, object> keyValuePair = _dataStack.Peek();
            ContextKey = keyValuePair.Key;
            UpdateContext(keyValuePair.Value);
            UpCommand.RaiseCanExecuteChanged();
        }

        private void SetContext(string contextKey, object context)
        {
            _dataStack.Push(new KeyValuePair<string, object>(contextKey, context));
            ContextKey = contextKey;
            UpdateContext(context);
            UpCommand.RaiseCanExecuteChanged();
        }

        private void UpdateContext(object context)
        {
            if (context is Dictionary<string, object> dictionary)
            {
                CanReorderItems = false;
                Data.Clear();
                foreach (ContextEntry keyValue in DataToList(dictionary))
                {
                    Data.Add(keyValue);
                }
            }
            else if (context is List<object> list)
            {
                CanReorderItems = true;
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
