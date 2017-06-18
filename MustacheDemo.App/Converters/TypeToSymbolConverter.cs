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

using MustacheDemo.Data;
using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MustacheDemo.App.Converters
{
    class TypeToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Type type = value as Type ?? value.GetType();
            if (targetType == typeof(string)) return TypeToSymbolString(type);
            if (targetType == typeof(FontFamily)) return TypeToFontFamily(type);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public static string TypeToSymbolString(Type type)
        {
            if (type == DataTypes.StringType) return "Abc";
            if (type == DataTypes.IntType) return "###";
            if (type == DataTypes.DecimalType) return "#.##";
            if (type == DataTypes.ListType) return "";
            if (type == DataTypes.DictionaryType) return "";
            return "?";
        }

        public static string StringTypeToSymbol(string type)
        {
            if (type == DataTypes.StringTypeName) return "Abc";
            if (type == DataTypes.IntTypeName) return "###";
            if (type == DataTypes.DecimalTypeName) return "#.##";
            if (type == DataTypes.ListTypeName) return "";
            if (type == DataTypes.DictionaryTypeName) return "";
            return "?";
        }

        public static string StringTypeToText(string type)
        {
            if (type == DataTypes.StringTypeName) return "String";
            if (type == DataTypes.IntTypeName) return "Integer";
            if (type == DataTypes.DecimalTypeName) return "Decimal";
            if (type == DataTypes.ListTypeName) return "List";
            if (type == DataTypes.DictionaryTypeName) return "Dictionary";
            return "Boolean";
        }

        public static FontFamily StringTypeToFontFamily(string type)
        {
            return type == DataTypes.ListTypeName || type == DataTypes.DictionaryTypeName
                ? new FontFamily("Segoe MDL2 Assets")
                : FontFamily.XamlAutoFontFamily;
        }

        public static FontFamily TypeToFontFamily(Type type)
        {
            FontFamily typeToFontFamily = type == DataTypes.ListType || type == DataTypes.DictionaryType
                ? new FontFamily("Segoe MDL2 Assets")
                : FontFamily.XamlAutoFontFamily;
            return typeToFontFamily;
        }
    }
}
