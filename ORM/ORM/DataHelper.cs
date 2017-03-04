using ORM.Annotations;
using ORM.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ORM
{
    public class DataHelper<T> : ICrudHelper<T> where T : IBaseModel
    {
        public Connection Connection { get; }
        private string GetAllStatement => GenerateGetAllStatement();
        private string InsertStatement => GenerateInsertStatement();
        private string UpdateStatement => GenerateUpdateStatement();
        private string DeleteStatement => GenerateDeleteStatement();
        private SqlTableAttribute TableAttribute => (SqlTableAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(SqlTableAttribute));

        public DataHelper()
        {
            Connection = new Connection();
        }

        public DataHelper(Connection connection)
        {
            Connection = connection;
        }

        public IEnumerable<T> Get<TResult>(TResult Keys)
        {
            using (var SQL = Connection.CreateCommand())
            {
                using (var reader = Connection.ReadSQL(GenerateGetStatement(Keys), GenerateColumnParameters(Keys)))
                {
                    while (reader.Read())
                    {
                        yield return Map(reader);
                    }
                }
            }
        }

        public IEnumerable<T> GetAll()
        {
            using (var SQL = Connection.CreateCommand())
            {
                using (var reader = Connection.ReadSQL(GetAllStatement))
                {
                    while (reader.Read())
                    {
                        yield return Map(reader);
                    }
                }
            }
        }

        public async Task<IEnumerable<T>> GetAsync<TResult>(TResult Keys)
        {
            using (var SQL = await Connection.CreateCommandAsync())
            {
                var list = new List<T>();
                using (var reader = await Connection.ReadSQLAsync(GenerateGetStatement(Keys), GenerateColumnParameters(Keys)))
                {
                    while (reader.Read())
                    {
                        list.Add(Map(reader));
                    }
                }
                return list.AsEnumerable();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using (var SQL = await Connection.CreateCommandAsync())
            {
                var list = new List<T>();
                using (var reader = await Connection.ReadSQLAsync(GetAllStatement))
                {
                    while (reader.Read())
                    {
                        list.Add(Map(reader));
                    }
                }
                return list.AsEnumerable();
            }
        }

        public bool Insert(T item)
        {
            return CrudOperation(item, InsertStatement);
        }

        public async Task<bool> InsertAsync(T item)
        {
            return await CrudOperationAsync(item, InsertStatement);
        }

        public bool Update(T item)
        {
            return CrudOperation(item, UpdateStatement);
        }

        public async Task<bool> UpdateAsync(T item)
        {
            return await CrudOperationAsync(item, UpdateStatement);
        }

        public bool Delete(T item)
        {
            return CrudOperation(item, DeleteStatement, true);
        }

        public async Task<bool> DeleteAsync(T item)
        {
            return await CrudOperationAsync(item, DeleteStatement, true);
        }

        private async Task<bool> CrudOperationAsync(T item, string Statement, bool ParametersIdOnly = false)
        {
            using (SqlCommand SQL = await Connection.CreateCommandAsync())
            {
                try
                {
                    SQL.CommandType = CommandType.Text;
                    SQL.CommandText = Statement;
                    SQL.Parameters.AddRange(GenerateColumnParameters(item, ParametersIdOnly));

                    return await SQL.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception)
                {
                    Connection.TransactionStatus = TransactionStatus.Rollback;
                    throw;
                }
            }
        }

        private bool CrudOperation(T item, string Statement, bool ParametersIdOnly = false)
        {
            using (SqlCommand SQL = Connection.CreateCommand())
            {
                try
                {
                    SQL.CommandType = CommandType.Text;
                    SQL.CommandText = Statement;
                    SQL.Parameters.AddRange(GenerateColumnParameters(item, ParametersIdOnly));

                    return SQL.ExecuteNonQuery() > 0;
                }
                catch (Exception)
                {
                    Connection.TransactionStatus = TransactionStatus.Rollback;
                    throw;
                }
            }
        }

        #region SQL Statements Generation
        private string GenerateGetAllStatement()
        {
            var table = TableAttribute;
            if (table != null)
            {
                string MySQL = "SELECT {0} FROM {1};";
                return string.Format(MySQL, string.Join(", ", GenerateColumnNames()), table.TableName);
            }
            return string.Empty;
        }

        private string GenerateGetStatement(object keys)
        {
            var table = TableAttribute;
            if (table != null)
            {
                string MySQL = "SELECT {0} FROM {1} WHERE {2};";
                return string.Format(MySQL,
                    string.Join(", ", GenerateColumnNames()),
                    table.TableName,
                    string.Join(" AND ", GenerateColumnNames(keys)));
            }
            return string.Empty;
        }

        private string GenerateGetStatement(string WhereClause)
        {
            var table = TableAttribute;
            if (table != null)
            {
                string MySQL = "SELECT {0} FROM {1} WHERE {2};";
                return string.Format(MySQL,
                    string.Join(", ", GenerateColumnNames()),
                    table.TableName,
                    WhereClause);
            }
            return string.Empty;
        }

        private string GenerateInsertStatement()
        {
            var table = TableAttribute;
            if (table != null)
            {
                string MySQL = "INSERT INTO {0} ({1}) VALUES ({2});";
                return string.Format(MySQL,
                    table.TableName,
                    string.Join(", ", GenerateColumnNames()),
                    string.Join(", ", GenerateColumnNames(columnType: SqlColumnStatementType.ParameterOnly)));
            }
            return string.Empty;
        }

        private string GenerateUpdateStatement()
        {
            var table = TableAttribute;
            if (table != null)
            {
                string MySQL = "UPDATE {0} SET {1} WHERE {2};";
                return string.Format(MySQL,
                    table.TableName,
                    string.Join(", ", GenerateColumnNames(columnType: SqlColumnStatementType.ColumnParameter)),
                    string.Join(" AND ", GenerateColumnNames(true, SqlColumnStatementType.ColumnParameter)));
            }
            return string.Empty;
        }

        private string GenerateDeleteStatement()
        {
            var table = TableAttribute;
            if (table != null)
            {
                string MySQL = "DELETE FROM {0} WHERE {1};";
                return string.Format(MySQL,
                    table.TableName,
                    string.Join(" AND ", GenerateColumnNames(true, SqlColumnStatementType.ColumnParameter)));
            }
            return string.Empty;
        }

        private enum SqlColumnStatementType { ColumnOnly, ColumnParameter, ParameterOnly }

        private string[] GenerateColumnNames(bool IdsOnly = false, SqlColumnStatementType columnType = SqlColumnStatementType.ColumnOnly)
        {
            List<string> columns = new List<string>();
            foreach (var p in typeof(T).GetProperties())
            {
                var column = (SqlColumnAttribute)p.GetCustomAttributes(typeof(SqlColumnAttribute), false).FirstOrDefault();
                if (column != null && ((IdsOnly && column.IsKey) || !IdsOnly))
                {
                    switch (columnType)
                    {
                        case SqlColumnStatementType.ColumnOnly:
                            columns.Add(column.ColumnName);
                            break;
                        case SqlColumnStatementType.ColumnParameter:
                            columns.Add(string.Format("{0} = @{0}", column.ColumnName));
                            break;
                        case SqlColumnStatementType.ParameterOnly:
                            columns.Add(string.Format("@{0}", column.ColumnName));
                            break;
                        default:
                            break;
                    }
                }
            }
            return columns.ToArray();
        }

        private string[] GenerateColumnNames(object Keys)
        {
            List<string> columns = new List<string>();
            foreach (var p in Keys.GetType().GetProperties())
            {
                var t = typeof(T).GetProperty(p.Name);
                if (t != null)
                {
                    var column = (SqlColumnAttribute)t.GetCustomAttributes(typeof(SqlColumnAttribute), false).FirstOrDefault();
                    if (column != null)
                    {
                        columns.Add(string.Format("{0} = @{0}", column.ColumnName));
                    }
                }

            }
            return columns.ToArray();
        }

        private SqlParameter[] GenerateColumnParameters(T item, bool IdsOnly = false)
        {
            List<SqlParameter> columns = new List<SqlParameter>();
            foreach (var p in typeof(T).GetProperties())
            {
                var column = (SqlColumnAttribute)p.GetCustomAttributes(typeof(SqlColumnAttribute), false).FirstOrDefault();
                if (column != null && ((IdsOnly && column.IsKey) || !IdsOnly))
                {
                    var parameter = column.Parameter;
                    parameter.Value = p.GetValue(item);
                    columns.Add(parameter);
                }
            }
            return columns.ToArray();
        }

        private SqlParameter[] GenerateColumnParameters(object keys)
        {
            List<SqlParameter> columns = new List<SqlParameter>();
            foreach (var p in keys.GetType().GetProperties())
            {
                var t = typeof(T).GetProperty(p.Name);
                if (t != null)
                {
                    var column = (SqlColumnAttribute)t.GetCustomAttributes(typeof(SqlColumnAttribute), false).FirstOrDefault();
                    if (column != null)
                    {
                        var parameter = column.Parameter;
                        parameter.Value = p.GetValue(keys);
                        columns.Add(parameter);
                    }
                }
            }

            return columns.ToArray();
        }

        private T Map(IDataReader reader)
        {
            var item = Activator.CreateInstance<T>();

            foreach (var p in typeof(T).GetProperties())
            {
                var column = (SqlColumnAttribute)p.GetCustomAttributes(typeof(SqlColumnAttribute), false).FirstOrDefault();
                if (column != null)
                {
                    try
                    {
                        var value = reader[column.ColumnName];
                        if (value == DBNull.Value)
                        {
                            value = GetDefaultValue(p.PropertyType);
                        }

                        p.SetValue(item, Convert.ChangeType(value, p.PropertyType));
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return item;
        }

        private object GetDefaultValue(Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
            {
                return Activator.CreateInstance(t);
            }
            return null;
        }
        #endregion
    }
}
