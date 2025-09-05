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
            [Id]          INT IDENTITY(1,1) CONSTRAINT [PK_ACTION_LOG] PRIMARY KEY,
            [ACTION_TYPE] NVARCHAR(20)   NOT NULL,
            [ITEM_TYPE]   NVARCHAR(10)   NOT NULL,
            [NAME]        NVARCHAR(260)  NULL,
            [FULL_PATH]   NVARCHAR(1024) NULL,
            [SIZE_BYTES]  BIGINT         NULL,
            [RESULT]      NVARCHAR(10)   NOT NULL,
            [ERROR_MSG]   NVARCHAR(MAX)  NULL,
            [CreatedAt]   DATETIME2(0)   NOT NULL CONSTRAINT [DF_ACTION_LOG_CreatedAt] DEFAULT SYSUTCDATETIME()

            ALTER TABLE [dbo].[ACTION_LOG]
              ADD CONSTRAINT [CK_ACTION_LOG_RESULT] CHECK (RESULT] IN (N'Success', N'Failure'));
            CREATE INDEX [IX_ACTION_LOG_CreatedAt] ON [dbo].[ACTION_LOG]([CreatedAt]);
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

    }

}
