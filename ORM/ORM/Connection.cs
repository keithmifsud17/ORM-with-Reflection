using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ORM
{
    public enum TransactionStatus { Commit, Rollback }

    public class Connection
    {
        public TransactionStatus TransactionStatus { get; set; }

        private SqlTransaction _Transaction;
        private SqlConnection _Connection;

        private SqlConnection GetSQLConnection
        {
            get
            {
                if (_Connection == null)
                {
                    _Connection = new SqlConnection(ConnectionSettings.Instance.GetConnectionString());
                    TransactionStatus = TransactionStatus.Commit;
                }
                return _Connection;
            }
        }

        private bool SaveTransaction()
        {
            try
            {
                if (_Connection != null)
                {
                    if (_Connection.State == System.Data.ConnectionState.Open)
                    {
                        if (_Transaction != null)
                        {
                            if (TransactionStatus == TransactionStatus.Commit)
                                _Transaction.Commit();
                            else
                                _Transaction.Rollback();

                            _Transaction.Dispose();
                            _Transaction = null;
                        }
                        _Connection.Close();
                    }
                    _Connection.Dispose();
                    _Connection = null;
                }
                return true;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<SqlCommand> CreateCommandAsync()
        {
            if (_Transaction == null)
            {
                var SQL = new SqlCommand()
                {
                    Connection = GetSQLConnection
                };

                SQL.Disposed += SQL_Disposed;
                try
                {
                    await SQL.Connection.OpenAsync();

                    _Transaction = SQL.Connection.BeginTransaction();
                    SQL.Transaction = _Transaction;

                    return SQL;
                }
                catch
                {
                    _Connection.Dispose();
                    _Connection = null;
                    throw;
                }
            }
            else
            {
                var SQL = new SqlCommand()
                {
                    Connection = _Transaction.Connection,
                    Transaction = _Transaction
                };
                return SQL;
            }
        }

        public SqlCommand CreateCommand()
        {
            if (_Transaction == null)
            {
                var SQL = new SqlCommand()
                {
                    Connection = GetSQLConnection
                };

                SQL.Disposed += SQL_Disposed;
                try
                {
                    SQL.Connection.Open();

                    _Transaction = SQL.Connection.BeginTransaction();
                    SQL.Transaction = _Transaction;

                    return SQL;
                }
                catch
                {
                    _Connection.Dispose();
                    _Connection = null;
                    throw;
                }
            }
            else
            {
                var SQL = new SqlCommand()
                {
                    Connection = _Transaction.Connection,
                    Transaction = _Transaction
                };
                return SQL;
            }
        }

        private void SQL_Disposed(object sender, EventArgs e)
        {
            SaveTransaction();
        }
    }
}
