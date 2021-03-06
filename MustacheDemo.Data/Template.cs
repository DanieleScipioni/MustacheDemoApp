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

using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using MustacheDemo.Core.Database;
using System.Threading.Tasks;

namespace MustacheDemo.Data
{
    public class Template
    {
        public string Name;
        public string Text;

        public static async Task<List<Template>> GetTemplates(SqliteConnection connection)
        {
            List<Template> templates;
            using (SqliteDataReader reader = await connection.ExecuteReaderAsync("select * from [templates];"))
            {
                templates = new List<Template>();
                if (!reader.HasRows) return templates;

                int name = reader.GetOrdinal("name");
                int template = reader.GetOrdinal("template");

                while (reader.Read())
                {
                    templates.Add(new Template
                    {
                        Name = reader.GetString(name),
                        Text = reader.GetString(template)
                    });
                }
            }

            return templates;
        }

        public static async Task SaveTemplate(SqliteConnection connection, Template template)
        {
            const string stmt = "UPDATE [templates] SET [template] = @template WHERE [name] = @name";
            await connection.ExecuteNonQueryAsync(stmt, new SqliteParameter("name", template.Name),
                new SqliteParameter("template", template.Text));
        }
    }
}
