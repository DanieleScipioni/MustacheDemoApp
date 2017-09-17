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

using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace MustacheDemo.Core.Database
{
    public static class SqliteConnectionExtension
    {
        public static object ExecuteScalar(this SqliteConnection connection, string stmt, params SqliteParameter[] parameters)
        {
            using (var command = new SqliteCommand(stmt, connection))
            {
                command.Parameters.AddRange(parameters);
                return command.ExecuteScalar();
            }
        }

        public static int ExecuteNonQuery(this SqliteConnection connection, string stmt,
            params SqliteParameter[] parameters)
        {
            using (var command = new SqliteCommand(stmt, connection))
            {
                command.Parameters.AddRange(parameters);
                return command.ExecuteNonQuery();
            }
        }

        public static async Task<int> ExecuteNonQueryAsync(this SqliteConnection connection, string stmt,
            params SqliteParameter[] parameters)
        {
            using (var command = new SqliteCommand(stmt, connection))
            {
                command.Parameters.AddRange(parameters);
                return await command.ExecuteNonQueryAsync();
            }
        }

        public static async Task<SqliteDataReader> ExecuteReaderAsync(this SqliteConnection connection, string commandText)
        {
            using (var command = new SqliteCommand(commandText, connection))
            {
                return await command.ExecuteReaderAsync();
            }
        }
    }
}
