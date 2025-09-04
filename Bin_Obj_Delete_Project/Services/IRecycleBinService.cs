using Bin_Obj_Delete_Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Obj_Delete_Project.Services
{
    public interface IRecycleBinService
    {
        Task<bool> RestoreDelInfoAsync(List<DelMatchingInfo> lstDelInfo, Action<DelMatchingInfo> onItemRestored);
    }

    public class RecycleBinService : IRecycleBinService
    {
        public async Task<bool> RestoreDelInfoAsync(List<DelMatchingInfo> lstDelInfo, Action<DelMatchingInfo> onItemRestored)
        {
            return await Task.Run(() =>
            {
                bool itemRestored = false; // 복원이 실행되었는가의 여부를 확인!

                Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
                dynamic shell = Activator.CreateInstance(shellAppType);

                dynamic recycleBin = shell.NameSpace(10); // 휴지통 ID!
                dynamic items = recycleBin.Items();

                // 기준: [삭제 데이터] 정보 중에서 프로그램 실행 후, [삭제한 폴더 경로]를 중복 없이 .ToList()화 하는 것
                var deletedPathInfo = lstDelInfo.Where(x => !string.IsNullOrEmpty(x.DelMatchingPath))
                .Select(x => x.DelMatchingPath.ToLowerInvariant())
                .Distinct()
                .ToList();
                for (int i = 0; i < items.Count; i++)
                {
                    dynamic item = items.Item(i);

                    string originPath = recycleBin.GetDetailsOf(item, 1)?.Trim();
                    string itemName = item.Name?.Trim();
                    if (string.IsNullOrEmpty(originPath) || string.IsNullOrEmpty(itemName))
                        continue;

                    // 삭제 경로 = [originPath] + [itemName]
                    string fullDeletedPath = Path.Combine(originPath, itemName).ToLowerInvariant();
                    if (!deletedPathInfo.Contains(fullDeletedPath))
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
                                verb.DoIt(); // 복원 실행
                                itemRestored = true; // 복원이 실행되었음.
                                var matchInfo = lstDelInfo.FirstOrDefault(
                                    x => x.DelMatchingPath.Equals(fullDeletedPath, StringComparison.OrdinalIgnoreCase));
                                if (matchInfo != null)
                                {
                                    onItemRestored?.Invoke(matchInfo); // 콜백 호출: [MainVM]에서 UI 실시간 반영
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
