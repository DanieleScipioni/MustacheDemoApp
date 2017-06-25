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
using System.Linq;
using Microsoft.Data.Sqlite;

namespace MustacheDemo.Core.Database.Schema
{
    public class DatabaseSchemaManager : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly long _expectedSchemaVersion;
        private long _currentSchemaVersion;
        private readonly Dictionary<long, UpgradeStep> _stepsByStartVersion;

        public DatabaseSchemaManager(SqliteConnection connection, long expectedSchemaVersion)
        {
            _connection = connection;
            _connection.Open();
            _expectedSchemaVersion = expectedSchemaVersion;
            _currentSchemaVersion = -1;
            _stepsByStartVersion = new Dictionary<long, UpgradeStep>();
        }

        public long SchemaVersion => _currentSchemaVersion == -1
            ? _currentSchemaVersion = (long) _connection.ExecuteScalar("PRAGMA user_version;")
            : _currentSchemaVersion;

        public bool NeedsUpgrade => SchemaVersion < _expectedSchemaVersion;


        public void Add(UpgradeStep upgradeStep)
        {
            _stepsByStartVersion[upgradeStep.StartVersion] = upgradeStep;
        }

        public void Upgrade(IProgress<Tuple<long, long>> progress = null)
        {
            long currentVersion = SchemaVersion;

            long count = _stepsByStartVersion.Count(kv => kv.Key >= currentVersion);
            long partial = 0;
            progress?.Report(new Tuple<long, long>(count, partial));

            do
            {
                if (!_stepsByStartVersion.ContainsKey(currentVersion)) break;

                UpgradeStep upgradeStep = _stepsByStartVersion[currentVersion];
                PerformUpgrade(_connection, upgradeStep);
                currentVersion = upgradeStep.TargetVersion;
                progress?.Report(new Tuple<long, long>(count, partial++));
            } while (currentVersion < _expectedSchemaVersion);
        }

        private static void PerformUpgrade(SqliteConnection connection, UpgradeStep upgradeStep)
        {
            using (SqliteTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    upgradeStep.ExecuteUpgrade(connection);
                    connection.ExecuteNonQuery($"PRAGMA user_version = {upgradeStep.TargetVersion};", transaction);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
