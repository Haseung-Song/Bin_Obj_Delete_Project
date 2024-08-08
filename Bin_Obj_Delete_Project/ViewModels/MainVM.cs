using Bin_Obj_Delete_Project.Common;
using Bin_Obj_Delete_Project.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Bin_Obj_Delete_Project.ViewModels
{
    public class MainVM : INotifyPropertyChanged
    {
        #region [프로퍼티]

        /// <summary>
        /// [_buttonEnalbedOrNot]
        /// </summary>
        private bool _delBtnEnabledOrNot;

        /// <summary>
        /// [_deleteFolderPath]
        /// </summary>
        private string _deleteFolderPath;

        /// <summary>
        /// [_binObjToDelete]
        /// </summary>
        private ObservableCollection<DeleteFolderInfo> _deleteFolderInfo;

        #endregion

        #region [OnPropertyChanged]

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// [ButtonEnabledOrNot]
        /// </summary>
        public bool IsDelBtnEnabledOrNot
        {
            get => _delBtnEnabledOrNot;
            private set
            {
                if (_delBtnEnabledOrNot != value)
                {
                    _delBtnEnabledOrNot = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [DeleteFolderPath]
        /// </summary>
        public string DeleteFolderPath
        {
            get => _deleteFolderPath;
            private set
            {
                if (_deleteFolderPath != value)
                {
                    _deleteFolderPath = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [DelefeFolderInfo]
        /// </summary>
        public ObservableCollection<DeleteFolderInfo> DelefeFolderInfo
        {
            get => _deleteFolderInfo;
            private set
            {
                if (_deleteFolderInfo != value)
                {
                    _deleteFolderInfo = value;
                    OnPropertyChanged();
                }

            }

        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region [ICommand]

        // 1. 폴더 불러오기
        public ICommand LoadFolderCommand { get; set; }

        // 2. bin, obj 폴더 삭제
        public ICommand DelFoldersCommand { get; set; }

        #endregion


        #region 생성자 (Initialize)

        public MainVM()
        {
            LoadFolderCommand = new RelayCommand(LoadFolder);
            _deleteFolderInfo = new ObservableCollection<DeleteFolderInfo>();
            DelFoldersCommand = new RelayCommand(DelFolders);
        }

        #endregion

        /// <summary>
        /// 1. [폴더 불러오기] 기능
        /// </summary>
        private void LoadFolder()
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog
            {
                DefaultDirectory = @"C:\", // 기본 경로
                InitialDirectory = @"O:\", // 초기 경로
                IsFolderPicker = true
            };

            // [폴더 불러오기] 버튼 클릭 후,
            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                IsDelBtnEnabledOrNot = true; // [bin, obj 폴더 삭제] 버튼 활성화
                DeleteFolderPath = folderDialog.FileName; // [폴더 경로]
                // [dirInfo] = 상위 폴더 정보
                // [dirSubInfo] = 하위 폴더 리스트 정보
                if (!string.IsNullOrEmpty(DeleteFolderPath))
                {
                    //var directoryInfo = new DirectoryInfo(DeleteFolderPath);
                    //var directoryInfos = directoryInfo.GetFileSystemInfos("*", SearchOption.AllDirectories); // 내일 참고해서 코드 짜기
                    IEnumerable<string> directories = Directory.EnumerateDirectories(DeleteFolderPath, "*", SearchOption.AllDirectories);
                    //.Where(dir => dir.EndsWith("bin") || dir.EndsWith("obj")); // 경로의 마지막 글자가 "bin"이거나 "obj"인 파일만 찾음.
                    DelefeFolderInfo?.Clear(); // 컬렉션 초기화
                    // 파일 경로가 존재할 때,
                    if (directories != null)
                    {
                        foreach (string dir in directories)
                        {
                            DirectoryInfo dirInfo = new DirectoryInfo(dir);
                            // 해당 경로의 [폴더 이름]이 "bin" 또는 "obj"인 경우에만 포함
                            if (dirInfo.Name.Equals("bin") || dirInfo.Name.Equals("obj"))
                            {
                                DelefeFolderInfo.Add(new DeleteFolderInfo
                                {
                                    DelFolderPath = dir,
                                    DelFolderName = dirInfo.Name,
                                    DelFolderCreationTime = dirInfo.CreationTime.ToString(),
                                    DelFolderAccessedTime = dirInfo.LastAccessTime.ToString(),
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
                // DeleteFolderInfo 정보 확인: 디버깅!
                foreach (DeleteFolderInfo item in DelefeFolderInfo)
                {
                    Console.WriteLine(item.DelFolderPath);
                    Console.WriteLine(item.DelFolderName);
                    Console.WriteLine(item.DelFolderCreationTime);
                    Console.WriteLine(item.DelFolderAccessedTime);
                    Console.WriteLine(item.DelFolderModifiedTime);
                    Console.WriteLine(item.DelFolderCategory);
                    Console.WriteLine(item.DelFolderSize);
                }

            }

        }

        /// <summary>
        /// 2. [bin, obj 폴더 삭제] 기능
        /// </summary>
        private void DelFolders()
        {
            IsDelBtnEnabledOrNot = false; // [bin, obj 폴더 삭제] 버튼 비활성화
            if (!string.IsNullOrEmpty(DeleteFolderPath))
            {
                foreach (DeleteFolderInfo folder in DelefeFolderInfo)
                {
                    string dir = folder.DelFolderPath;
                    // [try ~ catch]문 활용 예외 처리!
                    try
                    {
                        if (Directory.Exists(dir))
                        {
                            Directory.Delete(dir, true); // 지정된 디렉터리 및 해당 디렉터리의 하위 디렉터리 및 파일 삭제
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error Deleting Folder... FolderPath: {dir} : {ex.Message}");
                    }

                }

            }
            DelefeFolderInfo?.Clear(); // 컬렉션 초기화
        }

    }

}
