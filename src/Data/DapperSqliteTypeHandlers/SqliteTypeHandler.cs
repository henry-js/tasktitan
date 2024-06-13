using System.Data;

using Dapper;

namespace TaskTitan.Data.DapperSqliteTypeHandlers;

internal abstract class SqliteTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    public abstract override T? Parse(object value);

    public override void SetValue(IDbDataParameter parameter, T? value)
    {
        parameter.Value = value?.ToString();
    }
}
