using Microsoft.Data.Sqlite;

namespace MustacheDemo.Core.Database
{
    public static class SqliteTransactionExtension
    {
        public static int ExecuteNonQuery(this SqliteTransaction transaction, string stmt,
            params SqliteParameter[] parameters)
        {
            using (var command = new SqliteCommand(stmt, transaction.Connection, transaction))
            {
                command.Parameters.AddRange(parameters);
                return command.ExecuteNonQuery();
            }
        }
    }
}
