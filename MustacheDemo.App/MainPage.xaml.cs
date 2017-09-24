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

using MustacheDemo.App.ViewModels;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MustacheDemo.App
{
    public sealed partial class MainPage
    {
        private readonly MainPageViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = DataContext as MainPageViewModel;
            HiddenElementContainer.Children.Remove(TemplateCommandBar);
            HiddenElementContainer.Children.Remove(DataCommandBar);
        }

        private void TemplateTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.TemplateText = ((TextBox) sender).Text;
        }

        private void Pivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = (Pivot)sender;
            switch (pivot.SelectedIndex)
            {
                case 0:
                    BottomAppBar = TemplateCommandBar;
                    break;
                case 1:
                    BottomAppBar = DataCommandBar;
                    break;
            }
        }

        private void DataEntry_OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var frameworkElement = e.OriginalSource as FrameworkElement;
            if (!(frameworkElement?.DataContext is ContextEntry keyValue)) return;

            e.Handled = true;

            _viewModel.EditContextEntry(keyValue);
        }

        #region Mouse Back Button

        private bool _mouseBackButtonPressed;

        private void UIElement_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!(sender is UIElement uiElement)) return;

            PointerPoint currentPoint = e.GetCurrentPoint(uiElement);
            _mouseBackButtonPressed = currentPoint.Properties.IsXButton1Pressed;
        }

        private void UIElement_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!_mouseBackButtonPressed) return;
            _mouseBackButtonPressed = false;
            if (!_viewModel.UpCommand.CanExecute(null)) return;

            _viewModel.UpCommand.Execute(null);
            e.Handled = true;
        }

        private void UIElement_OnPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            _mouseBackButtonPressed = false;
        }

        private void UIElement_OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            _mouseBackButtonPressed = false;
        }

        private void UIElement_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _mouseBackButtonPressed = false;
        }

        #endregion

        private void ListViewBase_OnDragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            _viewModel.Reordering = true;
        }

        private void ListViewBase_OnDragItemsCompleted(ListViewBase listView, DragItemsCompletedEventArgs args)
        {
            _viewModel.Reordering = false;
        }
    }
}
