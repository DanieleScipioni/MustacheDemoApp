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

using Microsoft.Data.Sqlite;
using System.Threading.Tasks;

namespace MustacheDemo.Core.Database.Schema
{
    public abstract class UpgradeStep
    {
        public readonly long StartVersion;
        public readonly long TargetVersion;

        protected UpgradeStep(long startVersion, long targetVersion)
        {
            StartVersion = startVersion;
            TargetVersion = targetVersion;
        }

        public abstract void ExecuteUpgrade(SqliteConnection connection);
    }

    public class SqlStmtStringUpgradeStep : UpgradeStep
    {
        private readonly string _stmt;

        public SqlStmtStringUpgradeStep(long startVersion, long targetVersion, string stmt) : base(startVersion, targetVersion)
        {
            _stmt = stmt;
        }

        public override void ExecuteUpgrade(SqliteConnection connection)
        {
            connection.ExecuteNonQuery(_stmt);
        }
    }

    public class DelayUpgradeStep : UpgradeStep
    {
        private readonly int _delay;
        public DelayUpgradeStep(long startVersion, long targetVersion, int delay) : base(startVersion, targetVersion)
        {
            _delay = delay;
        }

        public override void ExecuteUpgrade(SqliteConnection connection)
        {
            Task.Delay(_delay).GetAwaiter().GetResult();
        }
    }
}
