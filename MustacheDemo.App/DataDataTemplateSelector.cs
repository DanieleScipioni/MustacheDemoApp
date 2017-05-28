using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MustacheDemo.App.ViewModels;

namespace MustacheDemo.App
{
    public class DataDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate KeyValyeDataTemplate { get; set; }

        public DataTemplate ListDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var keyValue = item as KeyValue;
            if (keyValue == null) return base.SelectTemplateCore(item, container);


            return KeyValyeDataTemplate;
        }
    }
}
