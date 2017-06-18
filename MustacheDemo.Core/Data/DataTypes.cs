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
using System.Collections.Generic;
using Windows.UI.Xaml.Media;

namespace MustacheDemo.Core.Data
{
    public static class DataTypes
    {
        private static readonly Type StringType = typeof(string);
        private static readonly Type IntType = typeof(int);
        private static readonly Type DecimalType = typeof(decimal);
        private static readonly Type ListType = typeof(List<object>);
        private static readonly Type DictionaryType = typeof(Dictionary<string, object>);

        private static readonly string StringTypeName = StringType.FullName;
        private static readonly string IntTypeName = IntType.FullName;
        private static readonly string DecimalTypeName = DecimalType.FullName;
        private static readonly string ListTypeName = ListType.FullName;
        private static readonly string DictionaryTypeName = DictionaryType.FullName;

        public static string TypeToSymbolString(Type type)
        {
            if (type == StringType) return "Abc";
            if (type == IntType) return "###";
            if (type == DecimalType) return "#.##";
            if (type == ListType) return "";
            if (type == DictionaryType) return "";
            return "?";
        }

        public static string StringTypeToSymbol(string type)
        {
            if (type == StringTypeName) return "Abc";
            if (type == IntTypeName) return "###";
            if (type == DecimalTypeName) return "#.##";
            if (type == ListTypeName) return "";
            if (type == DictionaryTypeName) return "";
            return "?";
        }

        public static string StringTypeToText(string type)
        {
            if (type == StringTypeName) return "String";
            if (type == IntTypeName) return "Integer";
            if (type == DecimalTypeName) return "Decimal";
            if (type == ListTypeName) return "List";
            if (type == DictionaryTypeName) return "Dictionary";
            return "Boolean";
        }

        public static FontFamily StringTypeToFontFamily(string type)
        {
            return type == ListTypeName || type == DictionaryTypeName
                ? new FontFamily("Segoe MDL2 Assets")
                : FontFamily.XamlAutoFontFamily;
        }

        public static FontFamily TypeToFontFamily(Type type)
        {
            FontFamily typeToFontFamily = type == ListType || type == DictionaryType
                ? new FontFamily("Segoe MDL2 Assets")
                : FontFamily.XamlAutoFontFamily;
            return typeToFontFamily;
        }
    }
}
