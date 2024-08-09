using Bin_Obj_Delete_Project.Common;
using Bin_Obj_Delete_Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Bin_Obj_Delete_Project.ViewModels
{
    public class FilterWindowVM : MainVM
    {
        #region [프로퍼티]

        #endregion

        #region [OnPropertyChanged]


        #endregion

        #region [ICommand]

        // 1. 검색 필터 적용
        public ICommand SearchPathCommand { get; set; }

        #endregion

        #region 생성자 (Initialize)

        public FilterWindowVM()
        {
            SearchPathCommand = new RelayCommand(SearchConfirm);
        }

        #endregion

        #region [버튼기능]

        private void SearchConfirm()
        {
            EnumerateFolders();
        }

        /// <summary>
        /// 선택적 [하위 디렉토리] 검색 함수
        /// </summary>
        private void EnumerateFolders()
        {
            //IsDelBtnEnabledOrNot = true; // [폴더 선택삭제], [폴더 일괄삭제] 버튼 활성화
            // [dirInfo] = 상위 폴더 정보
            // [dirSubInfo] = 하위 폴더 리스트 정보
            if (!string.IsNullOrEmpty(DeleteFolderPath))
            {
                // [directoryInfo] 관련 참고해야 할 코드 (By. 박상훈 선임)
                //var directoryInfo = new DirectoryInfo(DeleteFolderPath);
                //var directoryInfos = directoryInfo.GetFileSystemInfos("*", SearchOption.AllDirectories);
                IEnumerable<string> directories = Directory.EnumerateDirectories(DeleteFolderPath, "*", SearchOption.AllDirectories);
                //.Where(dir => dir.EndsWith("bin") || dir.EndsWith("obj")); // 경로의 마지막 글자가 "bin"이거나 "obj"인 파일만 찾음!
                DeleteFolderInfo?.Clear(); // (전체) 컬렉션 초기화
                // 파일 경로가 존재할 때,
                if (directories != null)
                {
                    foreach (string dir in directories)
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(dir);
                        // 해당 경로의 [폴더 이름]이 "bin" 또는 "obj"인 경우에만 포함
                        if (dirInfo.Name.Equals("bin") || dirInfo.Name.Equals("obj"))
                        {
                            DeleteFolderInfo.Add(new DeleteFolderInfo
                            {
                                DelFolderPath = dir,
                                DelFolderName = dirInfo.Name,
                                DelFolderCreationTime = dirInfo.CreationTime.ToString(),
                                DelFolderModifiedTime = dirInfo.LastWriteTime.ToString(),
                                DelFolderCategory = "파일 폴더",
                                DelFolderSize = dir.Length.ToString() + " Byte"
                            });

                        }

                    }

                }
                else
                {
                    _ = MessageBox.Show("불러올 폴더 경로가 없습니다.", "경로 미존재", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            // DeleteFolderInfo 정보 확인: 디버깅으로 확인 가능
            foreach (DeleteFolderInfo item in DeleteFolderInfo)
            {
                Console.WriteLine(item.DelFolderPath);
                Console.WriteLine(item.DelFolderName);
                Console.WriteLine(item.DelFolderCreationTime);
                Console.WriteLine(item.DelFolderModifiedTime);
                Console.WriteLine(item.DelFolderCategory);
                Console.WriteLine(item.DelFolderSize);
            }
            ActiveFolderInfo = DeleteFolderInfo; // [ActiveFolderInfo] 컬렉션에 [DeleteFolderInfo] 컬렉션을 할당
        }


        #endregion
    }

}
