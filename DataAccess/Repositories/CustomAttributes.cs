using DataAccess.SqlModels;
using Npgsql;
using System.Data;

namespace DataAccess.Repositories
{
    public class CustomAttributes
    {
        public static List<CustomAttribute> GetAllAttributes()
        {
            DataTable dt = new();
            dt.Fill("SELECT * FROM custom_attributes");
            return dt.To<CustomAttribute>();
        }

        public static void CreateNewAttributes(List<string> newAttributes)
        {
            var attCsv = string.Join(",", newAttributes);
            AddAttributes(attCsv);
        }

        public static void CreateNewAttributes(string newAttributesCsv)
        {
            AddAttributes(newAttributesCsv);
        }

        private static void AddAttributes(string newAttributeCsv)
        {
            using var connection = new NpgsqlConnection(Configuration.ConnectionString);
            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "iu_sp_import_custom_attributes(@atts)";
            command.Parameters.Add(new NpgsqlParameter("atts", newAttributeCsv));
            command.ExecuteNonQuery();
        }
    }
}
