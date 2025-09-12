using Bin_Obj_Delete_Project.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Bin_Obj_Delete_Project.Services
{
    /// <summary>
    /// 앱에서 발생한 '사실'을 DB에 기록하는 서비스(Write-only)
    /// [LogAsync]: 삭제(DELETE)/복원(RESTORE) 같은 '행위' 1건 기록
    /// 왜 인터페이스를 쓰나???
    /// ViewModel은 "DB가 [MSSQL]인지/[어떤 방식]으로 쓰는지" 알 필요 없음.
    /// 따라서, 결합도↓, 나중에 교체 쉬움.
    /// </summary>
    public interface IAuditService
    {
        Task<bool> LogAsync(string actionType, DelMatchingInfo item, bool ok, string error, CancellationToken ct);
    }

}
