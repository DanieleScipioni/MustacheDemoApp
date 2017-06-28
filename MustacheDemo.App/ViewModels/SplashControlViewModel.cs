using MustacheDemo.App.UserControls.Splash;
using MustacheDemo.Core;
using System;

namespace MustacheDemo.App.ViewModels
{
    internal class SplashControlViewModel : BindableBase, ISplashControlViewModel, IDisposable
    {
        private string _text;
        private double _total;
        private double _partial;
        private bool _indeterminate;

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public double Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

        public double Partial
        {
            get => _partial;
            set => SetProperty(ref _partial, value);
        }

        public bool Indeterminate
        {
            get => _indeterminate;
            set => SetProperty(ref _indeterminate, value);
        }

        public void Dispose()
        {
        }
    }
}