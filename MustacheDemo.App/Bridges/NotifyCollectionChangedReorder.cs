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

using System;
using System.Collections.Specialized;

namespace MustacheDemo.App.Bridges
{
    class ListViewReorderArgs : EventArgs
    {
        public int OldIndex;
        public int NewIndex;
    }

    delegate void ReorderCallback(object sender, ListViewReorderArgs listViewReorderArgs);

    class NotifyCollectionChangedReorder
    {
        private int _oldIndex, _newIndex;
        private readonly INotifyCollectionChanged _sender;
        private readonly ReorderCallback _reorderCallback;

        public NotifyCollectionChangedReorder(INotifyCollectionChanged sender, ReorderCallback reorderCallback)
        {
            _sender = sender;
            _reorderCallback = reorderCallback;
        }

        public void StartTracking() => _sender.CollectionChanged += CollectionChangedOnCollectionChanged;

        public void StopTracking() => _sender.CollectionChanged -= CollectionChangedOnCollectionChanged;

        private void CollectionChangedOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    _oldIndex = e.OldStartingIndex;
                    break;
                case NotifyCollectionChangedAction.Add:
                    _newIndex = e.NewStartingIndex;
                    _reorderCallback.Invoke(_sender, new ListViewReorderArgs {NewIndex = _newIndex, OldIndex = _oldIndex});
                    break;
                default:
                    _sender.CollectionChanged -= CollectionChangedOnCollectionChanged;
                    break;
            }
        }


    }
}
