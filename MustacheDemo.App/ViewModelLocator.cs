using MustacheDemo.App.ViewModels;
using System;
using Windows.UI.Xaml.Data;

namespace MustacheDemo.App
{
    public class ViewModelLocator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var mainPage = value as MainPage;
            if (mainPage != null) return new MainPageViewModel();

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
