using Npgsql;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace DataAccess
{
    public static class Extensions
    {
        private static string ConnectionString => Configuration.ConnectionString;

        internal static DataSet Fill(this DataSet ds, string selectCommandText, params NpgsqlParameter[] sqlParameters)
        {
            ds ??= new DataSet();
            using var connection = new NpgsqlConnection(ConnectionString);
            var command = new NpgsqlCommand(selectCommandText, connection);
            command.Parameters.AddRange(sqlParameters);
            var adapter = new NpgsqlDataAdapter(command);
            adapter.Fill(ds);
            return ds;
        }

        internal static DataTable Fill(this DataTable dt, string selectCommandText, params NpgsqlParameter[] sqlParameters)
        {
            dt ??= new DataTable();
            using var connection = new NpgsqlConnection(ConnectionString);
            var command = new NpgsqlCommand(selectCommandText, connection);
            command.Parameters.AddRange(sqlParameters);
            var adapter = new NpgsqlDataAdapter(command);
            adapter.Fill(dt);
            return dt;
        }

        internal static DataTable Fill<T>(this DataTable dt, List<T> objList)
        {
            var columnProps = GetColumnProps<T>();
            dt.Reset();
            foreach (var columnProp in columnProps)
            {
                dt.Columns.Add(columnProp.ColumnName, Nullable.GetUnderlyingType(columnProp.Property.PropertyType) ?? columnProp.Property.PropertyType);
            }
            foreach (var obj in objList)
            {
                var row = dt.NewRow();
                foreach (var columnProp in columnProps)
                {
                    row[columnProp.ColumnName] = columnProp.Property.GetValue(obj);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        internal static void CopyToSqlTable(this DataTable dt, string tableName, string postLoadSproc)
        {
            if (!tableName.EndsWith("staging"))
                throw new ArgumentException("Copying to non-staging tables is not allowed!!");

            var columns = dt.Columns.Cast<DataColumn>();
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            using var copier = connection.BeginBinaryImport($"COPY {tableName} ({string.Join(",", columns.Select(col => col.ColumnName))}) FROM STDIN (FORMAT BINARY)");

            foreach (var row in dt.AsEnumerable())
            {
                copier.StartRow();
                foreach (var col in columns)
                {
                    copier.WriteDynamic(row[col]);
                }
            }
            copier.Complete();
            copier.Close();

            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = postLoadSproc;
            command.ExecuteNonQuery();
        }

        internal static void WriteDynamic(this NpgsqlBinaryImporter copier, object val)
        {
            switch (val)
            {
                case int i:
                    copier.Write(i, NpgsqlDbType.Integer);
                    break;
                case string str:
                    copier.Write(str, NpgsqlDbType.Text);
                    break;
                case float f:
                    copier.Write(f, NpgsqlDbType.Real);
                    break;
                case DateTime dtm:
                    copier.Write(dtm, NpgsqlDbType.Timestamp);
                    break;
                case short s:
                    copier.Write(s, NpgsqlDbType.Smallint);
                    break;
                case bool b:
                    copier.Write(b, NpgsqlDbType.Boolean);
                    break;
                case null:
                case DBNull:
                    copier.WriteNull();
                    break;
                default:
                    throw new NotImplementedException($"Type conversion for {val.GetType()} has not been implemented.");
            }
        }

        internal static List<T> To<T>(this DataTable dt) where T : new()
        {
            var columnProps = GetColumnProps<T>();
            CheckColumns<T>(dt, columnProps);
            return dt.AsEnumerable().Select(row => row.To<T>(columnProps)).ToList();
        }

        public static T To<T>(this DataRow dataRow) where T : new()
        {
            var columnProps = GetColumnProps<T>();
            CheckColumns<T>(dataRow.Table, columnProps);
            return dataRow.To<T>(columnProps);
        }

        private static T To<T>(this DataRow dataRow, IEnumerable<ColumnProperty> columnProps) where T : new()
        {
            var t = new T();
            foreach (var columnProp in columnProps)
            {
                columnProp.Property.SetValue(t, dataRow[columnProp.ColumnName]);
            }
            return t;
        }

        private static IEnumerable<ColumnProperty> GetColumnProps<T>()
        {
            return typeof(T).GetProperties().Select(prop => new ColumnProperty()
            {
                Property = prop,
                ColumnName = prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? ""
            }).Where(columnProp => columnProp.ColumnName != "");
        }

        private static void CheckColumns<T>(DataTable dt, IEnumerable<ColumnProperty> columnProps)
        {
            var missingColumns = columnProps.Where(columnProp => !dt.Columns.Contains(columnProp.ColumnName)).ToList();

            if (missingColumns.Any())
                throw new ArgumentException($"Data could not be converted to type: {typeof(T)}. Missing columns: {string.Join(", ", missingColumns)}");
        }

        public static T1 AddMatchingPropertyValues<T1, T2>(this T1 obj1, T2 obj2)
        {
            var matchingProps = typeof(T1).GetProperties().Join(typeof(T2).GetProperties(), prop1 => prop1.Name, prop2 => prop2.Name, (prop1, prop2) => (prop1, prop2));
            foreach (var (prop1, prop2) in matchingProps)
            {
                var val = prop2.GetValue(obj2);
                if (prop1.PropertyType != prop2.PropertyType)
                {
                    prop1.SetValue(obj1, Converters.Get(prop1.PropertyType)(val));
                }
                else
                {
                    prop1.SetValue(obj1, val);
                }
            }
            return obj1;
        }

        public static string AsString(this object? obj)
        {
            return obj switch
            {
                IEnumerable<object?> enumerable => string.Join(",", enumerable.Select(o => o.AsString())),
                _ => obj?.ToString() ?? string.Empty,
            };
        }

        public static int AsInt(this object? obj)
        {
            if (obj is int intObj)
                return intObj;
            else if (int.TryParse(obj.AsString(), out var val))
                return val;
            else
                return 0;
        }

        public static DateTime AsDate(this object? obj)
        {
            if (obj is DateTime dtObj)
                return dtObj;
            else if (DateTime.TryParse(obj.AsString(), out var dt))
                return dt;
            else
                return DateTime.MinValue;
        }

        public static bool AsBool(this object? obj)
        {
            if (obj is bool boolObj)
                return boolObj;
            else if (obj is int intObj)
                return !(intObj == 0);
            else if (bool.TryParse(obj.AsString(), out var b))
                return b;
            else
                return false;
        }

        public static short AsShort(this object? obj)
        {
            if (obj is short intObj)
                return intObj;
            else if (short.TryParse(obj.AsString(), out var val))
                return val;
            else
                return 0;
        }

        public static float AsFloat(this object? obj)
        {
            if (obj is float intObj)
                return intObj;
            else if (float.TryParse(obj.AsString(), out var val))
                return val;
            else
                return 0f;
        }

        private static class Converters
        {
            private static readonly Dictionary<Type, Func<object?, object>> _converters = new()
            {
                { typeof(string), obj => obj.AsString() },
                { typeof(int), obj => obj.AsInt() },
                { typeof(DateTime), obj => obj.AsDate() },
                { typeof(bool), obj => obj.AsBool() },
                { typeof(short), obj => obj.AsShort() },
                { typeof(float), obj => obj.AsFloat() },
                { typeof(int?), obj => obj.AsInt() },
                { typeof(DateTime?), obj => obj.AsDate() },
                { typeof(bool?), obj => obj.AsBool() },
                { typeof(short?), obj => obj.AsShort() },
                { typeof(float?), obj => obj.AsFloat() }
            };

            public static T1 Convert<T1>(object? obj2)
            {
                return (T1)_converters[typeof(T1)](obj2);
            }

            public static Func<object?, object> Get(Type t)
            {
                return _converters[t];
            }
        }

        private class ColumnProperty
        {
            public PropertyInfo Property { get; set; }
            public string ColumnName { get; set; }
        }
    }
}
