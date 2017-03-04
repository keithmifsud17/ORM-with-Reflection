using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ORM
{
    public static class ConnectionExtensions
    {
        public static async Task<IDataReader> ReadSQLAsync(this Connection connection, string MySQL, params SqlParameter[] parameters)
        {
            using (SqlCommand SQL = await connection.CreateCommandAsync())
            {
                try
                {
                    SQL.CommandText = MySQL;
                    SQL.CommandType = CommandType.Text;
                    SQL.Parameters.AddRange(parameters);
                    return await SQL.ExecuteReaderAsync();
                }
                catch
                {
                    connection.TransactionStatus = TransactionStatus.Rollback;
                    throw;
                }
            }
        }

        public static IDataReader ReadSQL(this Connection connection, string MySQL, params SqlParameter[] parameters)
        {
            using (SqlCommand SQL = connection.CreateCommand())
            {
                try
                {
                    SQL.CommandText = MySQL;
                    SQL.CommandType = CommandType.Text;
                    SQL.Parameters.AddRange(parameters);
                    return SQL.ExecuteReader();
                }
                catch
                {
                    connection.TransactionStatus = TransactionStatus.Rollback;
                    throw;
                }
            }
        }
    }
}
