using Bin_Obj_Delete_Project.Models;
using Bin_Obj_Delete_Project.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Bin_Obj_Delete_Project.Repository
{
    public sealed class SqlServerAuditRepository : IAuditService
    {
        private readonly string _cs; // 연결 문자열 (App.config의 name="sqlDB")

        private static bool IsFolder(DelMatchingInfo item) => string.Equals(item?.DelMatchingCategory, "파일 폴더", StringComparison.OrdinalIgnoreCase);

        public SqlServerAuditRepository(string connectionString)
        {
            _cs = connectionString ?? throw new ArgumentNullException(nameof(connectionString)); // [QUERY] 구문 에러 시, 기본 예외 메시지 생성
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
        public async Task<bool> LogAsync(string actionType, DelMatchingInfo item, bool ok, string error, CancellationToken ct)
        {
            try
            {
                using (var con = new SqlConnection(_cs))
                {
                    await con.OpenAsync(ct).ConfigureAwait(false);

                    // [DML Query]: Data Manipulation Language, 데이터 조작어)
                    const string SQL = @"INSERT INTO dbo.ACTION_LOG
                                    (ACTION, ITEM, NAME, PATH, SIZE, IS_ERROR, RESULT)
                                 VALUES
                                    (@ACTION, @ITEM, @NAME, @PATH, @SIZE, @IS_ERROR, @RESULT);";

                    using (var cmd = new SqlCommand(SQL, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 30;

                        cmd.Parameters.Add("@ACTION", SqlDbType.VarChar, 10).Value = actionType ?? "기타";
                        cmd.Parameters.Add("@ITEM", SqlDbType.VarChar, 10).Value = IsFolder(item) ? "폴더" : "파일";

                        var name = (item != null && item.DelMatchingName != null) ? (object)item.DelMatchingName : DBNull.Value;
                        cmd.Parameters.Add("@NAME", SqlDbType.NVarChar, 260).Value = name;

                        var path = (item != null && item.DelMatchingPath != null) ? (object)item.DelMatchingPath : DBNull.Value;
                        cmd.Parameters.Add("@PATH", SqlDbType.NVarChar, 1024).Value = path;

                        var size = (item != null) ? item.DelMatchingOfSize : 0L;
                        cmd.Parameters.Add("@SIZE", SqlDbType.BigInt).Value = size;
                        cmd.Parameters.Add("@IS_ERROR", SqlDbType.VarChar, 20).Value = ok ? " No Error" : "   Error";
                        cmd.Parameters.Add("@RESULT", SqlDbType.NVarChar, 10).Value = string.IsNullOrEmpty(error) ? (object)DBNull.Value : error;

                        await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                    _ = MessageBox.Show(mainWindow, $"SQL 오류:\r\n{ex.Message}", "Query 재확인", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                return false;
            }

        }

        /// <summary>
        /// [InsertScanItemsAsync]
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="items"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task InsertScanItemsAsnyc(string sessionId, IEnumerable<DelMatchingInfo> items, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

    }

}
