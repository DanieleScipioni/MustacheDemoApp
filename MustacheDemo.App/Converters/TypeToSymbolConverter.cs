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

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MustacheDemo.App.Converters
{
    class TypeToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var type = value as Type;
            if (targetType == typeof(string))
            {
                return type != null ? TypeToSymbolString(type) : TypeToSymbolString(value);
            }
            if (targetType == typeof(FontFamily))
            {
                return type != null ? TypeToFontFamily(type) : TypeToFontFamily(value);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public static string TypeToSymbolString(object value)
        {
            return TypeToSymbolString(value.GetType());
        }

        public static string TypeToSymbolString(Type type)
        {
            if (type == typeof(string)) return "Abc";
            if (type == typeof(int)) return "###";
            if (type == typeof(decimal)) return "#.##";
            if (type == typeof(List<object>)) return "";
            return "?";
        }

        public static FontFamily TypeToFontFamily(object value)
        {
            return TypeToFontFamily(value.GetType());
        }

        public static FontFamily TypeToFontFamily(Type type)
        {
            return type == typeof(List<object>) ? new FontFamily("Segoe MDL2 Assets") : FontFamily.XamlAutoFontFamily;
        }
    }
}
