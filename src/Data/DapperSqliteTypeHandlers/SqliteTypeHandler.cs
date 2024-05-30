using System.Data;

using Dapper;

namespace TaskTitan.Data.DapperSqliteTypeHandlers;

internal abstract class SqliteTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    public override T? Parse(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(IDbDataParameter parameter, T? value)
    {
        parameter.Value = value?.ToString();
    }
}
