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

                for (int i = 0; i < items.Count; i++)
                {
                    dynamic item = items.Item(i);

                    string originPath = recycleBin.GetDetailsOf(item, 1)?.Trim();
                    string itemName = item.Name?.Trim(); // [파일] 기준으로, 확장자 제외됨!
                    if (string.IsNullOrEmpty(originPath) || string.IsNullOrEmpty(itemName))
                        continue;

                    // lstDelInfo에서 직접 [폴더/파일] 구분 후, 매칭
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

                        // [파일] (DelMatchingCategory가 "파일 폴더"가 아닌 경우만)
                        string savedDir = Path.GetDirectoryName(x.DelMatchingPath); // [파일]의 폴더 경로 +
                        string savedFileNameWithoutExt = Path.GetFileNameWithoutExtension(x.DelMatchingPath); // 확장자 제외

                        // 즉, [파일]의 폴더 경로도 일치하면서, itemName(확장자 제외)도 동시에 일치해야 return 가능!
                        return string.Equals(savedDir, originPath, StringComparison.OrdinalIgnoreCase) &&
                               string.Equals(savedFileNameWithoutExt, itemName, StringComparison.OrdinalIgnoreCase);
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
                                verb.DoIt(); // 복원 실행
                                itemRestored = true; // 복원이 실행되었음.

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
