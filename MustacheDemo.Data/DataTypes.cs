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

namespace MustacheDemo.Data
{
    public static class DataTypes
    {
        public static readonly Type StringType = typeof(string);
        public static readonly Type IntType = typeof(int);
        public static readonly Type DecimalType = typeof(decimal);
        public static readonly Type ListType = typeof(List<object>);
        public static readonly Type DictionaryType = typeof(Dictionary<string, object>);
        
        public static readonly string StringTypeName = StringType.FullName;
        public static readonly string IntTypeName = IntType.FullName;
        public static readonly string DecimalTypeName = DecimalType.FullName;
        public static readonly string ListTypeName = ListType.FullName;
        public static readonly string DictionaryTypeName = DictionaryType.FullName;
    }
}
