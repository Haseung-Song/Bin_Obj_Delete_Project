using System.Data.SqlClient;

namespace Bin_Obj_Delete_Project.Infrastructure
{
    public static class SchemaBootstrapper
    {
        // [DDL 생성]
        private const string EnsureTableSql = @"
IF OBJECT_ID(N'dbo.ACTION_LOG', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ACTION_LOG]
    (
        [ID]          INT IDENTITY(1,1) PRIMARY KEY,
        [ACTION]      NVARCHAR(10)   NOT NULL,
        [ITEM]        NVARCHAR(10)   NOT NULL,
        [NAME]        NVARCHAR(260)  NULL,
        [PATH]        NVARCHAR(1024) NULL,
        [SIZE]        BIGINT         NULL,
        [ERROR_MSG]   NVARCHAR(10)   NULL,
        [RESULT_MSG]  NVARCHAR(10)   NULL
    );
END";

        public static void EnsureActionLogTable(string connectionString)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                con = new SqlConnection(connectionString);
                con.Open();
                cmd = new SqlCommand(EnsureTableSql, con);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd?.Dispose();
                con?.Dispose();
            }

        }

        public static void ClearActionLogTable(string connectionString)
        {
            const string sql = "DROP TABLE IF EXISTS dbo.ACTION_LOG;";

            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                con = new SqlConnection(connectionString);
                con.Open();

                cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd?.Dispose();
                con?.Dispose();
            }

        }

    }

}
