using Bin_Obj_Delete_Project.Models;
using Bin_Obj_Delete_Project.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Bin_Obj_Delete_Project.Repository
{
    public sealed class SqlServerAuditRepository : IAuditService
    {
        // 연결 문자열 (App.config의 name="SqlServerDb")
        private readonly string _cs;

        private static bool IsFolder(DelMatchingInfo item) => string.Equals(item.DelMatchingCategory, "파일 폴더", StringComparison.OrdinalIgnoreCase);

        public SqlServerAuditRepository()
        {
            var cs = ConfigurationManager.ConnectionStrings["SqlServerDb"].ConnectionString;
            if (cs != null && !string.IsNullOrEmpty(cs))
            {
                _cs = cs;
            }
            else
            {
                throw new InvalidOperationException("connectionStrings[\"SqlServerDb\"]가 설정되지 않았습니다.");
            }

        }

        /// <summary>
        /// 1건의 '행위' 기록 (삭제/복구/스캔 등)
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="item"></param>
        /// <param name="ok"></param>
        /// <param name="error"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task LogAsync(string actionType, DelMatchingInfo item, bool ok, string error, CancellationToken ct)
        {
            const string SQL = @"INSERT INTO dbo.ACTION_LOG
                                    (ACTION_TYPE, ITEM_TYPE, NAME, FULL_PATH, SIZE_BYTES, RESULT, ERROR_MSG)
                                 VALUES
                                    (@ACTION_TYPE, @ITEM_TYPE, @NAME, @FULL_PATH, @SIZE_BYTES, @RESULT, @ERROR_MSG);";

            using (var con = new SqlConnection(_cs))
            {
                await con.OpenAsync(ct).ConfigureAwait(false);
                using (var cmd = new SqlCommand(SQL, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 30;

                    cmd.Parameters.Add("@ACTION_TYPE", SqlDbType.VarChar, 10).Value = actionType ?? "UNKNOWN";
                    cmd.Parameters.Add("@ITEM_TYPE", SqlDbType.VarChar, 10).Value = IsFolder(item) ? "Folder" : "File";
                    var name = (item != null && item.DelMatchingName != null)
                        ? (object)item.DelMatchingName
                        : DBNull.Value;
                    cmd.Parameters.Add("@NAME", SqlDbType.NVarChar, 260).Value = name;
                    var path = (item != null && item.DelMatchingPath != null)
                        ? (object)item.DelMatchingPath
                        : DBNull.Value;
                    cmd.Parameters.Add("@FULL_PATH", SqlDbType.NVarChar, 2000).Value = path;
                    var size = item != null ? item.DelMatchingOfSize : 0L;
                    cmd.Parameters.Add("@SIZE_BYTES", SqlDbType.BigInt).Value = size;
                    cmd.Parameters.Add("@RESULT", SqlDbType.VarChar, 10).Value = ok ? "OK" : "FAIL";
                    cmd.Parameters.Add("@ERROR_MSG", SqlDbType.NVarChar, 1000).Value = string.IsNullOrEmpty(error) ? (object)DBNull.Value : error;
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
