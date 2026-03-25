using Bin_Obj_Delete_Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bin_Obj_Delete_Project.Services
{
    public interface IRecycleBinService
    {
        Task<bool> RestoreDelInfoAsync(List<DelMatchingInfo> lstDelInfo, Action<DelMatchingInfo> onItemRestored);
    }

    public class RecycleBinService : IRecycleBinService
    {
        private readonly IAuditService _auditService;

        public RecycleBinService(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public async Task<bool> RestoreDelInfoAsync(List<DelMatchingInfo> lstDelInfo, Action<DelMatchingInfo> onItemRestored)
        {
            return await Task.Run(() =>
            {
                bool itemRestored = false; // 복원이 실행되었는가의 여부를 확인!

                Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
                dynamic shell = Activator.CreateInstance(shellAppType);

                dynamic recycleBin = shell.NameSpace(10); // 휴지통 ID!
                dynamic items = recycleBin.Items();

                for (int i = 0; i < items.Count; i++)
                {
                    dynamic item = items.Item(i);

                    // 휴지통 안의 해당 파일의 삭제 전 저장된 폴더 경로를 가져옴.
                    string originPath = recycleBin.GetDetailsOf(item, 1)?.Trim();

                    // 휴지통 안에 있는 해당 [파일 이름]
                    string itemName = item.Name?.Trim();

                    if (string.IsNullOrEmpty(originPath) || string.IsNullOrEmpty(itemName))
                        continue;

                    // lstDelInfo에서 직접 폴더/파일 구분 후, 매칭
                    var matchInfo = lstDelInfo.FirstOrDefault(x =>
                    {
                        if (string.IsNullOrEmpty(x.DelMatchingPath))
                            return false;

                        // [폴더] (DelMatchingCategory가 "파일 폴더"인 경우)
                        if (string.Equals(x.DelMatchingCategory, "파일 폴더", StringComparison.OrdinalIgnoreCase))
                        {
                            string folderFullPath = Path.Combine(originPath, itemName);
                            return x.DelMatchingPath.Equals(folderFullPath, StringComparison.OrdinalIgnoreCase);
                        }

                        // [파일] (DelMatchingCategory가 "파일 폴더"가 아닌 경우)
                        // [파일] 매칭 로직
                        // 휴지통 항목과 저장된 삭제 이력(lstDelInfo)의 파일 정보를 비교 후
                        // 동일한 파일인지 판단하는 구간

                        // ex) C:\A\test.txt -> C:\A => [원래 폴더 경로] 추출
                        string savedDir = Path.GetDirectoryName(x.DelMatchingPath)?.Trim();

                        // ex) C:\A\test.txt -> test.txt => [파일명 + 확장자] 추출
                        string savedFileName = Path.GetFileName(x.DelMatchingPath)?.Trim();

                        // ex) C:\A\test.txt -> test => [파일명(확장자 제외)] 추출
                        string savedFileNameWithoutExt = Path.GetFileNameWithoutExtension(x.DelMatchingPath)?.Trim();

                        // ※ item.Name은 환경에 따라 확장자 포함/제외 여부가 달라짐.
                        //    둘 다 대응하기 위해 별도로 [확장자 제외 버전]도 생성함.
                        // ※ 휴지통에서 가져온 파일명(itemName)에서 [파일명(확장자 제외)] 추출
                        string itemNameWithoutExt = Path.GetFileNameWithoutExtension(itemName)?.Trim();

                        // ※ 최종 비교 조건
                        // 1. 원래 폴더 경로(savedDir)가 휴지통 안의 파일의 삭제 전 저장된 폴더 경로(originPath)와 동일하고,
                        // 2. [삭제 전 & 삭제 후] 파일명 - 아래 3가지 케이스 중, 하나라도 일치하면 동일한 파일로 판단
                        // 1) 확장자를 두 쪽다 포함한 형태
                        // 2) 확장자를 한 쪽만 포함한 형태 (저장 경로)
                        // 3) 확장자가 두 쪽다 제거된 형태
                        return string.Equals(savedDir, originPath, StringComparison.OrdinalIgnoreCase) &&
                               (
                                   string.Equals(savedFileName, itemName, StringComparison.OrdinalIgnoreCase) ||
                                   string.Equals(savedFileNameWithoutExt, itemName, StringComparison.OrdinalIgnoreCase) ||
                                   string.Equals(savedFileNameWithoutExt, itemNameWithoutExt, StringComparison.OrdinalIgnoreCase)
                               );
                    });

                    //Console.WriteLine($"[휴지통] originPath = [{originPath}]");
                    //Console.WriteLine($"[휴지통] itemName   = [{itemName}]");

                    //foreach (var x in lstDelInfo)
                    //{
                    //    Console.WriteLine($"[저장] Category = [{x.DelMatchingCategory}]");
                    //    Console.WriteLine($"[저장] Path     = [{x.DelMatchingPath}]");
                    //    Console.WriteLine($"[저장] Dir      = [{Path.GetDirectoryName(x.DelMatchingPath)}]");
                    //    Console.WriteLine($"[저장] Name     = [{Path.GetFileName(x.DelMatchingPath)}]");
                    //}

                    if (matchInfo == null)
                        continue;

                    var verbs = item.Verbs();

                    for (int j = 0; j < verbs.Count; j++)
                    {
                        dynamic verb = verbs.Item(j);

                        // 해당 휴지통 항목의 우클릭 메뉴(명령어 목록)
                        if (verb.Name is string name)
                        {
                            if (name.Contains("복원"))
                            {
                                bool ok = false; // 복원 [성공/실패] 여부!

                                try
                                {
                                    verb.DoIt(); // 복원 실행
                                    ok = true;
                                    itemRestored = true; // 복원이 실행되었음.
                                }
                                catch
                                {
                                    ok = false;
                                }

                                // 복원 성공 시, 성공 로그, 실패 시, 실패 로그를 띄움.
                                _auditService.LogAsync("  복원", matchInfo, ok, ok ? "  성공" : "  실패", CancellationToken.None);

                                // 1. 부모 본인 복원 콜백
                                onItemRestored?.Invoke(matchInfo); // 콜백 호출: [MainVM]에서 UI 실시간 반영

                                // 2. 폴더면, 하위 항목들도 같이 UI 복원 콜백
                                if (string.Equals(matchInfo.DelMatchingCategory, "파일 폴더", StringComparison.OrdinalIgnoreCase))
                                {
                                    string parentPath = matchInfo.DelMatchingPath;

                                    var childItems = lstDelInfo
                                    .Where(x =>
                                    !string.IsNullOrEmpty(x.DelMatchingPath) &&
                                    !x.DelMatchingPath.Equals(parentPath, StringComparison.OrdinalIgnoreCase) &&
                                    x.DelMatchingPath.StartsWith(parentPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                                    .OrderBy(x => x.DelMatchingPath.Length) // (부모 -> 자식 -> 손자 -> 파일) 순으로 처리함.
                                    .ToList();

                                    foreach (var child in childItems)
                                    {
                                        onItemRestored?.Invoke(child);
                                    }

                                }
                                break;
                            }

                        }

                    }

                }
                return itemRestored; // 값 반환
            });

        }

    }

}
