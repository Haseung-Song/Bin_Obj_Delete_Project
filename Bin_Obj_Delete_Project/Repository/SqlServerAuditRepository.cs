using Bin_Obj_Delete_Project.Models;
using Bin_Obj_Delete_Project.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Bin_Obj_Delete_Project.Repository
{
    public sealed class SqlServerAuditRepository : IAuditService
    {
        private readonly string _cs; // 연결 문자열 (App.config의 name="sqlDB")

        private static bool IsFolder(DelMatchingInfo item) => string.Equals(item.DelMatchingCategory, "파일 폴더", StringComparison.OrdinalIgnoreCase);

        public SqlServerAuditRepository(string connectionString)
        {
            _cs = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// [삭제, 복원] "ACTION" 기록
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="item"></param>
        /// <param name="ok"></param>
        /// <param name="error"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task LogAsync(string actionType, DelMatchingInfo item, bool ok, string error, CancellationToken ct)
        {
            if (item == null || String.IsNullOrEmpty(error)) return;
            const string SQL = @"INSERT INTO dbo.ACTION_LOG
                                    (ACTION, ITEM, NAME, PATH, SIZE, ERROR_MSG, RESULT_MSG)
                                 VALUES
                                    (@ACTION, @ITEM, @NAME, @PATH, @SIZE, @ERROR_MSG, @RESULT_MSG);";

            using (var con = new SqlConnection(_cs))
            {
                await con.OpenAsync(ct).ConfigureAwait(false);
                using (var cmd = new SqlCommand(SQL, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.Add("@ACTION", SqlDbType.VarChar, 10).Value = actionType ?? "기타";
                    cmd.Parameters.Add("@ITEM", SqlDbType.VarChar, 10).Value = IsFolder(item) ? "폴더" : "파일";
                    var name = (item != null && item.DelMatchingName != null) ? (object)item.DelMatchingName : DBNull.Value;
                    cmd.Parameters.Add("@NAME", SqlDbType.NVarChar, 260).Value = name;
                    string path = item.DelMatchingPath;
                    cmd.Parameters.Add("@PATH", SqlDbType.NVarChar, 260).Value = path;
                    var size = item != null ? item.DelMatchingOfSize : 0L;
                    cmd.Parameters.Add("@SIZE", SqlDbType.BigInt).Value = size;
                    cmd.Parameters.Add("@ERROR_MSG", SqlDbType.VarChar, 10).Value = ok ? "OK" : "Not OK";
                    cmd.Parameters.Add("@RESULT_MSG", SqlDbType.NVarChar, 1000).Value = string.IsNullOrEmpty(error) ? (object)DBNull.Value : error;

                    await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
                }

            }

        }

        public Task InsertScanItemsAsnyc(string sessionId, IEnumerable<DelMatchingInfo> items, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

    }

}
