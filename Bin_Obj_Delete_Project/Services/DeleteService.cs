using Bin_Obj_Delete_Project.Common;
using Bin_Obj_Delete_Project.Models;
using Bin_Obj_Delete_Project.ViewModels;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Bin_Obj_Delete_Project.Services
{
    public class DeleteService : CommonProperty
    {
        private readonly MainVM _mainVM;

        public DeleteService()
        {

        }

        public DeleteService(MainVM mainVM)
        {
            _mainVM = mainVM;
        }

        /// <summary>
        /// [폴더, 파일] 선택 삭제하기 (기능)
        /// 1) 휴지통에서 삭제
        /// 2) 영구적으로 삭제
        /// </summary>
        public async Task DelSelConfirm(IProgress<double> progress)
        {
            if (ActiveFolderInfo?.Count == 0)
            {
                VisibleDestroy = false;
                return;
            }
            progress?.Report(0);
            try
            {
                TheBtnEnabledOrNot = false;
                VisibleDestroy = true;
                int totalSelMatch = selectToDelete.Count();
                int processedSelMatch = 0;
                foreach (DelMatchingInfo match in selectToDelete)
                {
                    string dir = match.DelMatchingPath;
                    await Task.Run(async () =>
                    {
                        // 해당 디렉토리의 경로가 존재할 때,
                        if (FileSystem.DirectoryExists(dir))
                        {
                            // 1) 지정한 디렉토리 및 해당 디렉토리의 하위 디렉토리 및 폴더 휴지통에서 삭제
                            FileSystem.DeleteDirectory(dir, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                            // 2) 지정한 디렉토리 및 해당 디렉토리의 하위 디렉토리 및 폴더 영구적으로 삭제
                            //FileSystem.DeleteDirectory(dir, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
                        }
                        // 해당 파일 경로 존재 시,
                        else if (FileSystem.FileExists(dir))
                        {
                            // 1) 지정한 파일 휴지통에서 삭제
                            FileSystem.DeleteFile(dir, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                            // 2) 지정한 파일 영구적으로 삭제
                            //FileSystem.DeleteFile(dir, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
                        }
                        else
                        {
                            return;
                        }
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            _ = ActiveFolderInfo.Remove(match); // [UI 초기화]
                        });

                    });
                    // [폴더, 파일] 선택 삭제하기 후, [진행률 업데이트] 작업!
                    processedSelMatch++;
                    progress?.Report((double)processedSelMatch / totalSelMatch * 100);
                    await Task.Delay(10);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: Error Deleting Folder... FolderPath: {ex.Message}");
            }
            finally
            {
                if (ProgressValue < 100)
                {
                    progress?.Report(100); // [진행률: 100]: 작업 완료
                }
                TheBtnEnabledOrNot = true;
                VisibleDestroy = false;
                // 삭제 후 데이터 업데이트
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // 삭제된 항목 제거
                    if (selectToDelete?.Count > 0)
                    {
                        _mainVM.LstAllData = _mainVM.LstAllData.Where(item => !selectToDelete.Any(deleted => deleted.DelMatchingPath == item.DelMatchingPath)).ToList();
                        DeleteFolderInfo = new ObservableCollection<DelMatchingInfo>(_mainVM.LstAllData);
                        selectToDelete.Clear();
                    }
                    else
                    {
                        ActiveFolderInfo?.Clear();
                    }
                    _mainVM.LoadPageData();
                });

            }

        }

        /// <summary>
        /// [폴더, 파일] 일괄 삭제하기 (기능)
        /// 1) 휴지통에서 삭제
        /// 2) 영구적으로 삭제
        /// </summary>
        public async Task DelAllConfirm(IProgress<double> progress)
        {
            if (ActiveFolderInfo?.Count == 0)
            {
                VisibleDestroy = false;
                return;
            }
            progress?.Report(0);
            try
            {
                TheBtnEnabledOrNot = false;
                VisibleDestroy = true;
                int totalAllMatch = DeleteFolderInfo.Count();
                int processedAllMatch = 0;
                foreach (DelMatchingInfo match in DeleteFolderInfo)
                {
                    string dir = match.DelMatchingPath;
                    await Task.Run(() =>
                    {
                        // 해당 디렉토리의 경로가 존재할 때,
                        if (FileSystem.DirectoryExists(dir))
                        {
                            // 1) 지정한 디렉토리 및 해당 디렉토리의 하위 디렉토리 및 폴더 휴지통에서 삭제
                            FileSystem.DeleteDirectory(dir, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                            // 2) 지정한 디렉토리 및 해당 디렉토리의 하위 디렉토리 및 폴더 영구적으로 삭제
                            //FileSystem.DeleteDirectory(dir, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
                        }
                        // 해당 파일 경로 존재 시,
                        else if (FileSystem.FileExists(dir))
                        {
                            // 1) 지정한 파일 휴지통에서 삭제
                            FileSystem.DeleteFile(dir, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                            // 2) 지정한 파일 영구적으로 삭제
                            //FileSystem.DeleteFile(dir, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
                        }
                        else
                        {
                            return;
                        }

                    });
                    // [폴더, 파일] 일괄 삭제하기 후, [진행률 업데이트] 작업!
                    processedAllMatch++;
                    progress?.Report((double)processedAllMatch / totalAllMatch * 100);
                    await Task.Delay(10);
                }
                ActiveFolderInfo?.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: Error Deleting Folder... FolderPath: {ex.Message}");
            }
            finally
            {
                if (ProgressValue < 100)
                {
                    progress?.Report(100); // [진행률: 100]: 작업 완료
                }
                TheBtnEnabledOrNot = true;
                VisibleDestroy = false;
                // 삭제 후 데이터 업데이트
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // 삭제된 항목 제거
                    if (entireToDelete?.Count > 0)
                    {
                        _mainVM.LstAllData = _mainVM.LstAllData.Where(item => !entireToDelete.Any(deleted => deleted.DelMatchingPath == item.DelMatchingPath)).ToList();
                        DeleteFolderInfo = new ObservableCollection<DelMatchingInfo>(_mainVM.LstAllData);
                        entireToDelete.Clear();
                    }
                    _mainVM.LoadPageData();
                });

            }

        }

    }

}
