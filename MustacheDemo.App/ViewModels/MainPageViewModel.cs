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
    internal class MainPageViewModel : BindableBase
    {
        private bool _canRender;

        private string _template;
        public string Template
        {
            get { return _template; }
            set {
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

        public readonly DelegateCommand RenderCommand;

        public readonly DelegateCommand EditTemplateCommand;

        private string _templateCache;

        public MainPageViewModel()
        {
            RenderCommand = new DelegateCommand(Render, RenderCanExecute);
            EditTemplateCommand = new DelegateCommand(EditTemplate, EditTemplateCanExecute);
        }

        private void Render(object parameter)
        {
            _templateCache = Template;
            Template = "{{Mustache}}";
            RenderCompleted = true;
            RenderCommand.RaiseCanExecuteChanged();
            EditTemplateCommand.RaiseCanExecuteChanged();
        }

        private bool RenderCanExecute(object parameter)
        {
            return !RenderCompleted;
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
    }
}
