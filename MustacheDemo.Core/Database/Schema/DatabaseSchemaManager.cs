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
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace MustacheDemo.Core.Database.Schema
{
    public class DatabaseSchemaManager
    {
        private readonly string _connectionString;
        private readonly long _expectedSchemaVersion;
        private long _currentSchemaVersion;
        private readonly Dictionary<long, UpgradeStep> _stepsByStartVersion;

        public DatabaseSchemaManager(string connectionStringString, long expectedSchemaVersion)
        {
            _connectionString = connectionStringString;
            _expectedSchemaVersion = expectedSchemaVersion;
            _currentSchemaVersion = -1;
            _stepsByStartVersion = new Dictionary<long, UpgradeStep>();
        }

        public long SchemaVersion
        {
            get
            {
                if (_currentSchemaVersion != -1) return _currentSchemaVersion;

                using (SqliteConnection connection = DatabaseConnectionManager.GetMustacheDemoConnection())
                {
                    connection.Open();
                    return _currentSchemaVersion = (long)connection.ExecuteScalar("PRAGMA user_version;");
                }
            }
        }

        public bool NeedsUpgrade => SchemaVersion < _expectedSchemaVersion;

        public void Add(UpgradeStep upgradeStep)
        {
            _stepsByStartVersion[upgradeStep.StartVersion] = upgradeStep;
        }

        public async Task Upgrade(IProgress<Tuple<long, long>> progress = null)
        {
            long currentVersion = SchemaVersion;

            long count = _stepsByStartVersion.Count(kv => kv.Key >= currentVersion);
            long partial = 0;
            progress?.Report(new Tuple<long, long>(count, partial));

            using (SqliteConnection connection = DatabaseConnectionManager.GetConnection(_connectionString))
            {
                connection.Open();
                do
                {
                    if (!_stepsByStartVersion.ContainsKey(currentVersion)) break;

                    UpgradeStep upgradeStep = _stepsByStartVersion[currentVersion];
                    await PerformUpgrade(connection, upgradeStep);
                    currentVersion = upgradeStep.TargetVersion;
                    progress?.Report(new Tuple<long, long>(count, partial++));
                } while (currentVersion < _expectedSchemaVersion);
            }
        }

        private static async Task PerformUpgrade(SqliteConnection connection, UpgradeStep upgradeStep)
        {
            string sqlStmt = null;
            if (upgradeStep is SqlStmtStringUpgradeStep sqlStmtStringUpgradeStep)
            {
                sqlStmt = sqlStmtStringUpgradeStep.GetSqlStmt();
            }
            else if (upgradeStep is ResourceSqlFileUpgradeStep resourceSqlFileUpgradeStep)
            {
                sqlStmt = await resourceSqlFileUpgradeStep.GetSqlStmt();
            }

            string userVersionStmt = $"PRAGMA user_version = {upgradeStep.TargetVersion};";

            if (sqlStmt != null)
            {
                SqliteTransaction transaction = connection.BeginTransaction();
                try
                {
                    transaction.ExecuteNonQuery(sqlStmt);
                    int pragmaResult = transaction.ExecuteNonQuery(userVersionStmt);
                    if (pragmaResult == -1) throw new Exception($"Failed to execute {userVersionStmt}");
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            else if (upgradeStep is DelayUpgradeStep delayUpgradeStep)
            {
                ExecuteDelayUpgradeStep(delayUpgradeStep);
                int pragmaResult = connection.ExecuteNonQuery(userVersionStmt);
                if (pragmaResult == -1) throw new Exception($"Failed to execute {userVersionStmt}");
            }
        }

        private static void ExecuteDelayUpgradeStep(DelayUpgradeStep delayUpgradeStep)
        {
            delayUpgradeStep.ExecuteDelay();
        }
    }
}
