using System;
using System.Data;
using System.Data.SqlClient;

namespace ORM.Annotations
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SqlColumnAttribute : Attribute
    {
        private readonly string columnName;
        private readonly SqlDbType columnType;

        public SqlColumnAttribute(string columnName, SqlDbType columnType)
        {
            this.columnName = columnName;
            this.columnType = columnType;
        }

        public string ColumnName { get => columnName; }
        public SqlDbType ColumnType { get => columnType; }
        public bool IsKey { get; set; } = false;
        public SqlParameter Parameter { get => GenerateParameter(); }

        private SqlParameter GenerateParameter()
        {
            return new SqlParameter($"@{ColumnName}", ColumnType);
        }
    }
}
