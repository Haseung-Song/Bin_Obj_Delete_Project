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
        /// [_delBtnEnabledOrNot]
        /// </summary>
        private bool _delBtnEnabledOrNot;

        /// <summary>
        /// [_deleteFolderPath]
        /// </summary>
        private string _deleteFolderPath;

        /// <summary>
        /// [_selectedCrFolder]
        /// </summary>
        private DeleteFolderInfo _selectedCrFolder;

        /// <summary>
        /// [_selectFolderInfo]
        /// </summary>
        private ObservableCollection<DeleteFolderInfo> _selectFolderInfo;

        /// <summary>
        /// [_deleteFolderInfo]
        /// </summary>
        private ObservableCollection<DeleteFolderInfo> _deleteFolderInfo;

        /// <summary>
        /// [_activeFolderInfo]
        /// </summary>
        private ObservableCollection<DeleteFolderInfo> _activeFolderInfo;

        #endregion

        #region [OnPropertyChanged]

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// [IsDelBtnEnabledOrNot]
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
            set
            {
                if (_deleteFolderPath != value)
                {
                    _deleteFolderPath = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [DeleteFolderInfo]
        /// </summary>
        public ObservableCollection<DeleteFolderInfo> DeleteFolderInfo
        {
            get => _deleteFolderInfo;
            set
            {
                if (_deleteFolderInfo != value)
                {
                    _deleteFolderInfo = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [SelectedCrFolder]
        /// </summary>
        public DeleteFolderInfo SelectedCrFolder
        {
            get => _selectedCrFolder;
            set
            {
                if (_selectedCrFolder != value)
                {
                    _selectedCrFolder = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [SelectFolderInfo]
        /// </summary>
        public ObservableCollection<DeleteFolderInfo> SelectFolderInfo
        {
            get => _selectFolderInfo;
            set
            {
                if (_selectFolderInfo != value)
                {
                    _selectFolderInfo = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ActiveFolderInfo]
        /// [폴더 선택삭제] 시 => [ActiveFolderInfo = SelectFolderInfo]
        /// [폴더 일괄삭제] 시 => [ActiveFolderInfo = DeleteFolderInfo]
        /// </summary>
        public ObservableCollection<DeleteFolderInfo> ActiveFolderInfo
        {
            get => _activeFolderInfo;
            set
            {
                if (_activeFolderInfo != value)
                {
                    _activeFolderInfo = value;
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
        public ICommand LoadingFolderCommand { get; set; }

        // 2. 경로 불러오기
        public ICommand EnterLoadPathCommand { get; set; }

        // 3. 폴더 선택삭제
        public ICommand DelSelFoldersCommand { get; set; }

        // 4. 폴더 일괄삭제
        public ICommand DelAllFoldersCommand { get; set; }

        #endregion

        #region 생성자 (Initialize)

        public MainVM()
        {
            LoadingFolderCommand = new RelayCommand(LoadingFolder);
            EnterLoadPathCommand = new RelayCommand(EnterLoadPath);
            _selectedCrFolder = new DeleteFolderInfo();
            _selectFolderInfo = new ObservableCollection<DeleteFolderInfo>();
            _deleteFolderInfo = new ObservableCollection<DeleteFolderInfo>();
            _activeFolderInfo = new ObservableCollection<DeleteFolderInfo>();
            DelSelFoldersCommand = new RelayCommand(DelSelFolders);
            DelAllFoldersCommand = new RelayCommand(DelAllFolders);
        }

        #endregion

        #region [버튼기능]

        /// <summary>
        /// 1. [폴더 불러오기] 기능 (버튼)
        /// </summary>
        private void LoadingFolder()
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
                DeleteFolderPath = folderDialog.FileName;
                EnumerateFolders();
            }

        }

        /// <summary>
        /// 2. [경로 불러오기] 기능 (Enter 키)
        /// </summary>
        private void EnterLoadPath()
        {
            DeleteFolderPath = Path.GetFullPath(DeleteFolderPath);
            EnumerateFolders();
        }

        /// <summary>
        /// 선택적 [하위 디렉토리] 검색 함수
        /// </summary>
        private void EnumerateFolders()
        {
            IsDelBtnEnabledOrNot = true; // [폴더 선택삭제], [폴더 일괄삭제] 버튼 활성화
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

        /// <summary>
        /// 3. [폴더 선택삭제] 기능 (버튼)
        /// </summary>
        private void DelSelFolders()
        {
            IsDelBtnEnabledOrNot = false; // [폴더 선택삭제] 버튼 비활성화
            if (!string.IsNullOrEmpty(DeleteFolderPath))
            {
                if (SelectFolderInfo?.Count > 0)
                {
                    // [SelectFolderInfo] 컬렉션 항목들 기반 새로운 List<DeleteFolderInfo> 객체를 생성함.
                    List<DeleteFolderInfo> selectToDelete = new List<DeleteFolderInfo>(SelectFolderInfo);
                    foreach (DeleteFolderInfo folder in selectToDelete)
                    {
                        string dir = folder.DelFolderPath;
                        // [try ~ catch]문 활용 예외 처리!
                        try
                        {
                            if (Directory.Exists(dir))
                            {
                                Directory.Delete(dir, true); // 지정된 디렉터리 및 해당 디렉터리의 하위 디렉터리 및 파일 삭제
                                _ = ActiveFolderInfo.Remove(folder); // [ActiveFolderInfo] 컬렉션 => selectToDelete 항목 제외
                            }
                            IsDelBtnEnabledOrNot = true; // [폴더 선택삭제] 버튼 활성화
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error Deleting Folder... FolderPath: {dir} : {ex.Message}");
                        }

                    }

                }

            }

        }

        /// <summary>
        /// 4. [폴더 일괄삭제] 기능 (버튼)
        /// </summary>
        private void DelAllFolders()
        {
            IsDelBtnEnabledOrNot = false; // [폴더 일괄삭제] 버튼 비활성화
            if (!string.IsNullOrEmpty(DeleteFolderPath))
            {
                if (DeleteFolderInfo?.Count > 0)
                {
                    foreach (DeleteFolderInfo folder in DeleteFolderInfo)
                    {
                        string dir = folder.DelFolderPath;
                        // [try ~ catch]문 활용 예외 처리!
                        try
                        {
                            if (Directory.Exists(dir))
                            {
                                Directory.Delete(dir, true); // 지정된 디렉터리 및 해당 디렉터리의 하위 디렉터리 및 파일 삭제
                            }
                            IsDelBtnEnabledOrNot = true; // [폴더 일괄삭제] 버튼 활성화
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error Deleting Folder... FolderPath: {dir} : {ex.Message}");
                        }

                    }
                    DeleteFolderInfo?.Clear(); // (전체) 컬렉션 초기화
                }

            }

        }

    }
    #endregion
}