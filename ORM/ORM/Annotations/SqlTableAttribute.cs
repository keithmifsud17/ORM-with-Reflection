using System;

namespace ORM.Annotations
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SqlTableAttribute : Attribute
    {
        readonly string tableName;

        public SqlTableAttribute(string tableName)
        {
            this.tableName = tableName;
        }

        public string TableName { get => tableName; }
    }
}
