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
        /// [_IsDelBtnEnabledOrNot]
        /// </summary>
        private bool _IsDelBtnEnabledOrNot;

        /// <summary>
        /// [_deleteFolderPath]
        /// </summary>
        protected string _deleteFolderPath;

        /// <summary>
        /// [_folderNameFiltered]
        /// </summary>
        private string _folderNameFiltered;

        /// <summary>
        /// [_extensionsFiltered]
        /// </summary>
        private string _extensionsFiltered;

        /// <summary>
        /// [matchingFileInfoOrNot]
        /// </summary>
        private bool matchingFileInfoOrNot;

        /// <summary>
        /// [matchingFldrName]
        /// </summary>
        private string matchingFldrName;

        /// <summary>
        /// [matchingFileName]
        /// </summary>
        private string matchingFileName;

        /// <summary>
        /// [matchingFldrCreationTime]
        /// </summary>
        private string matchingFldrCreationTime;

        /// <summary>
        /// [matchingFileCreationTime]
        /// </summary>
        private string matchingFileCreationTime;

        /// <summary>
        /// [matchingFldrCategory]
        /// </summary>
        private string matchingFldrCategory;

        /// <summary>
        /// [matchingFileCategory]
        /// </summary>
        private string matchingFileCategory;

        /// <summary>
        /// [matchingFldrModifiedTime]
        /// </summary>
        private string matchingFldrModifiedTime;

        /// <summary>
        /// [matchingFileModifiedTime]
        /// </summary>
        private string matchingFileModifiedTime;

        /// <summary>
        /// [matchingFldrSize]
        /// </summary>
        private string matchingFldrSize;

        /// <summary>
        /// [matchingFileSize]
        /// </summary>
        private string matchingFileSize;

        /// <summary>
        /// [matchingFldrPath]
        /// </summary>
        private string matchingFldrPath;

        /// <summary>
        /// [matchingFilePath]
        /// </summary>
        private string matchingFilePath;

        /// <summary>
        /// [AbsolutePath]
        /// </summary>
        public static string AbsolutePath { get; set; }

        /// <summary>
        /// [uniqueFilePathSet]
        /// </summary>
        private readonly HashSet<string> uniqueFilePathSet;

        /// <summary>
        /// [_selectedCrFolder]
        /// </summary>
        private DelMatchingInfo _selectedCrFolder;

        /// <summary>
        /// [_selectFolderInfo]
        /// </summary>
        private ObservableCollection<DelMatchingInfo> _selectFolderInfo;

        /// <summary>
        /// [_deleteFolderInfo]
        /// </summary>
        private ObservableCollection<DelMatchingInfo> _deleteFolderInfo;

        /// <summary>
        /// [_activeFolderInfo]
        /// </summary>
        private ObservableCollection<DelMatchingInfo> _activeFolderInfo;

        #endregion

        #region [OnPropertyChanged]

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// [IsDelBtnEnabledOrNot]
        /// </summary>
        public bool IsDelBtnEnabledOrNot
        {
            get => _IsDelBtnEnabledOrNot;
            private set
            {
                if (_IsDelBtnEnabledOrNot != value)
                {
                    _IsDelBtnEnabledOrNot = value;
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
                    // static 필드에 절대 경로 설정.
                    AbsolutePath = DeleteFolderPath;
                }

            }

        }

        /// <summary>
        /// [FilterFolderName]
        /// </summary>
        public string FilterFolderName
        {
            get => _folderNameFiltered;
            set
            {
                if (_folderNameFiltered != value)
                {
                    _folderNameFiltered = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [FilterExtensions]
        /// </summary>
        public string FilterExtensions
        {
            get => _extensionsFiltered;
            set
            {
                if (_extensionsFiltered != value)
                {
                    _extensionsFiltered = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [DeleteFolderInfo]
        /// </summary>
        public ObservableCollection<DelMatchingInfo> DeleteFolderInfo
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
        public DelMatchingInfo SelectedCrFolder
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
        public ObservableCollection<DelMatchingInfo> SelectFolderInfo
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
        /// [선택 삭제하기] 시 => [ActiveFolderInfo = SelectFolderInfo]
        /// [일괄 삭제하기] 시 => [ActiveFolderInfo = DeleteFolderInfo]
        /// </summary>
        public ObservableCollection<DelMatchingInfo> ActiveFolderInfo
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

        // 3. 선택 삭제하기
        public ICommand DelSelMatchesCommand { get; set; }

        // 4. 일괄 삭제하기
        public ICommand DelAllMatchesCommand { get; set; }

        // 5. 검색 필터리셋 (FilterFolderName)
        public ICommand FilterResetFNCommand { get; set; }

        // 6. 검색 필터리셋 (FilterExtensions)
        public ICommand FilterResetFECommand { get; set; }

        #endregion

        #region 생성자 (Initialize)

        public MainVM()
        {
            LoadingFolderCommand = new RelayCommand(LoadingFolder);
            EnterLoadPathCommand = new RelayCommand(EnterLoadPath);
            _selectedCrFolder = new DelMatchingInfo();
            _selectFolderInfo = new ObservableCollection<DelMatchingInfo>();
            _deleteFolderInfo = new ObservableCollection<DelMatchingInfo>();
            _activeFolderInfo = new ObservableCollection<DelMatchingInfo>();
            uniqueFilePathSet = new HashSet<string>();
            matchingFldrName = string.Empty;
            matchingFileName = string.Empty;
            matchingFldrCreationTime = string.Empty;
            matchingFileCreationTime = string.Empty;
            matchingFldrCategory = string.Empty;
            matchingFileCategory = string.Empty;
            matchingFldrModifiedTime = string.Empty;
            matchingFileModifiedTime = string.Empty;
            matchingFldrSize = string.Empty;
            matchingFileSize = string.Empty;
            matchingFldrPath = string.Empty;
            matchingFilePath = string.Empty;
            DelSelMatchesCommand = new RelayCommand(DelSelMatches);
            DelAllMatchesCommand = new RelayCommand(DelAllMatches);
            FilterResetFNCommand = new RelayCommand(FilterResetFN);
            FilterResetFECommand = new RelayCommand(FilterResetFE);
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
            DeleteFolderPath = Path.GetFullPath(AbsolutePath);
            EnumerateFolders();
        }

        /// <summary>
        /// 선택적 [하위 디렉토리] 검색 함수
        /// </summary>
        protected void EnumerateFolders()
        {
            IsDelBtnEnabledOrNot = true; // [폴더 선택삭제], [폴더 일괄삭제] 버튼 활성화
            if (!string.IsNullOrEmpty(DeleteFolderPath))
            {
                // 모든 하위 디렉토리를 검색하되, 접근이 거부된 디렉토리는 제외함!
                IEnumerable<string> directories = Directory.EnumerateDirectories(DeleteFolderPath, "*", SearchOption.AllDirectories);
                //.Where(dir => dir.EndsWith("bin") || dir.EndsWith("obj")); // 경로의 마지막 글자가 "bin"이거나 "obj"인 파일만 찾음!
                DeleteFolderInfo?.Clear(); // (전체) 컬렉션 초기화
                uniqueFilePathSet.Clear(); // (중복) 해쉬셋 초기화

                // 파일 경로가 존재하면,
                if (directories != null)
                {
                    try
                    {
                        foreach (string dir in directories)
                        {
                            DirectoryInfo dirInfo = new DirectoryInfo(dir);
                            matchingFldrName = dirInfo.Name;
                            matchingFldrCreationTime = dirInfo.CreationTime.ToString();
                            matchingFldrCategory = "파일 폴더";
                            matchingFldrModifiedTime = dirInfo.LastWriteTime.ToString();
                            matchingFldrSize = dir.Length == 1 ? dir.Length.ToString() + " Byte" : dir.Length.ToString() + " Bytes";
                            matchingFldrPath = dir;

                            // 1. 필터 키워드를 콤마(',')로 구분 후, 배열로 생성 (FilterFolderName)
                            string[] filterComma1 = string.IsNullOrEmpty(FilterFolderName) ? Array.Empty<string>() : FilterFolderName.Split(',');

                            // Filter 01: 폴더 이름으로 검색(대소문자 구분(X))
                            // 1) [FilterFolderName]이 null이거나 string.Empty 문자열인 경우
                            // 2) [FilterFolderName]이 디렉토리 또는 하위 디렉토리 폴더의 이름과 일치하는 경우
                            //bool folderMatches1 = string.IsNullOrEmpty(FilterFolderName) || dirInfo.Name.Equals(FilterFolderName, StringComparison.OrdinalIgnoreCase);

                            //if (!folderMatches1)
                            //{
                            //    continue;
                            //}

                            // Filter 01: 폴더 이름으로 검색(대소문자 구분(X))
                            // 지정한 배열에 정의된 조건과 일치하는지 확인 함!
                            // 1) [FilterFolderName]이 null이거나 string.Empty 문자열인 경우
                            // 2) 콤마(',')로 구분된 [FilterFolderName]이 디렉토리 또는 하위 디렉토리 폴더의 이름과 일치하는 경우
                            bool folderMatches2 = string.IsNullOrEmpty(FilterFolderName) ||
                                Array.Exists(filterComma1, comma1 => dirInfo.Name.Equals(comma1.Trim(), StringComparison.OrdinalIgnoreCase));

                            // 1), 2)가 아닐 때,
                            if (!folderMatches2)
                            {
                                continue;
                            }

                            // 2. 필터 키워드를 콤마(',')로 구분 후, 배열로 생성 (FilterExtensions)
                            string[] filterComma2 = string.IsNullOrEmpty(FilterExtensions) ? Array.Empty<string>() : FilterExtensions.Split(',');

                            // Filter 02: 파일 확장자로 검색(대소문자 구분(X))
                            // 1) [FilterExtensions]이 null이거나 string.Empty 문자열이 아닌 경우
                            if (!string.IsNullOrEmpty(FilterExtensions))
                            {
                                matchingFileInfoOrNot = false;
                                FileInfo[] fileInfo = dirInfo.GetFiles("*", SearchOption.AllDirectories);
                                foreach (FileInfo item in fileInfo)
                                {
                                    // 이미 처리가 된 파일 경로는 무시 (중복 제거)
                                    if (uniqueFilePathSet.Contains(item.FullName))
                                    {
                                        continue;
                                    }

                                    // 지정한 배열에 정의된 조건과 일치하는지 확인 함!
                                    // 2) 콤마(',')로 구분된 [FilterExtensions]이 파일의 확장명 부분의 문자열과 일치하는 경우 (확장자 비교)
                                    if (Array.Exists(filterComma2, comma2 => item.Extension.Equals(comma2.Trim(), StringComparison.OrdinalIgnoreCase)))
                                    {
                                        matchingFileInfoOrNot = true;
                                        matchingFileName = item.Name;
                                        matchingFileCreationTime = item.CreationTime.ToString();
                                        Dictionary<string, string> extensionCategoryMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                        {
                                            { ".pdb", "Program Debug Database" },
                                            { ".sln", "Visual Studio Solution" },
                                            { ".cs", "C# Source File" },
                                            { ".csproj", "C# Project File" },
                                            { ".user", "Per-User Project Options File" },
                                            { ".config", "VisualStudio.config.11.0" },
                                            { ".dll", "응용 프로그램 확장" },
                                            { ".cache", "CACHE 파일" },
                                            { ".resources", "RESOURCES 파일" },
                                            { ".baml", "BAML 파일" },
                                            { ".resx", "Microsoft .NET Managed Resource File" },
                                            { ".settings", "Settings-Designer File" },
                                            { ".xaml", "Windows 태그 파일" },
                                            { ".xml", "xmlfile" },
                                            { ".nupkg", "NUPKG 파일" },
                                            { ".gitattributes", "txtfile" },
                                            { ".gitignore", "txtfile" },
                                            { ".md", "MD 파일" },
                                            { ".p7s", "PKCS #7 서명" },
                                            { ".txt", "텍스트 문서" },
                                            { ".exe", "응용 프로그램" },
                                            { ".suo", "Visual Studio Solution User Options" }
                                        };
                                        matchingFileCategory = extensionCategoryMap.TryGetValue(item.Extension, out string category) ? category : "기타 파일";
                                        matchingFileModifiedTime = item.LastWriteTime.ToString();
                                        matchingFileSize = (item.Length / 1024.0).ToString("F1") + " KB";
                                        matchingFilePath = item.FullName;
                                        _ = uniqueFilePathSet.Add(matchingFilePath); // 중복 제거
                                        break;
                                    }

                                }

                            }
                            // 1) [FilterFolderName] => 해당 폴더 및 정보를 리스트의 형태로 전시
                            // 2) [FilterExtensions] => 해당 파일 및 정보를 리스트의 형태로 전시
                            // 즉, 필터링 (X) => 필터링 없이 전시, 필터링 (O) => 필터링해서 전시
                            if (string.IsNullOrEmpty(FilterExtensions) || matchingFileInfoOrNot)
                            {
                                DeleteFolderInfo.Add(new DelMatchingInfo
                                {
                                    DelMatchingName = matchingFileInfoOrNot ? matchingFileName : matchingFldrName,
                                    DelMatchingCreationTime = matchingFileInfoOrNot ? matchingFileCreationTime : matchingFldrCreationTime,
                                    DelMatchingCategory = matchingFileInfoOrNot ? matchingFileCategory : matchingFldrCategory,
                                    DelMatchingModifiedTime = matchingFileInfoOrNot ? matchingFileModifiedTime : matchingFldrModifiedTime,
                                    DelMatchingSize = matchingFileInfoOrNot ? matchingFileSize : matchingFldrSize,
                                    DelMatchingPath = matchingFileInfoOrNot ? matchingFilePath : matchingFldrPath,
                                });

                            }

                        }

                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine($"Access denied to directory: {directories}. Exception: {ex.Message}"); // 접근이 거부된 폴더는 무시
                    }

                }
                else
                {
                    _ = MessageBox.Show("불러올 폴더 경로가 없습니다.", "경로 재확인", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            // DelMatchingInfo 정보 확인: 디버깅으로 확인 가능
            foreach (DelMatchingInfo item in DeleteFolderInfo)
            {
                Console.WriteLine(item.DelMatchingName);
                Console.WriteLine(item.DelMatchingCreationTime);
                Console.WriteLine(item.DelMatchingCategory);
                Console.WriteLine(item.DelMatchingModifiedTime);
                Console.WriteLine(item.DelMatchingSize);
                Console.WriteLine(item.DelMatchingPath);
            }
            ActiveFolderInfo = DeleteFolderInfo; // [ActiveFolderInfo] 컬렉션에 [DeleteFolderInfo] 컬렉션을 할당
        }

        /// <summary>
        /// 3. [선택 삭제하기] 기능 (버튼)
        /// </summary>
        private void DelSelMatches()
        {
            IsDelBtnEnabledOrNot = false; // [선택 삭제하기] 버튼 비활성화
            if (!string.IsNullOrEmpty(DeleteFolderPath))
            {
                if (SelectFolderInfo?.Count > 0)
                {
                    // [SelectFolderInfo] 컬렉션 항목들 기반 새로운 List<DeleteFolderInfo> 객체를 생성!
                    List<DelMatchingInfo> selectToDelete = new List<DelMatchingInfo>(SelectFolderInfo);

                    foreach (DelMatchingInfo match in selectToDelete)
                    {
                        string dir = match.DelMatchingPath;

                        // [try ~ catch]문 활용, 예외 처리!
                        try
                        {
                            IsDelBtnEnabledOrNot = true; // [선택 삭제하기] 버튼 활성화

                            // 해당 폴더 경로 존재 시,
                            if (Directory.Exists(dir))
                            {
                                Directory.Delete(dir, true); // 지정한 디렉터리 및 해당 디렉터리의 하위 디렉터리 및 폴더 삭제
                            }
                            // 해당 파일 경로 존재 시,
                            else if (File.Exists(dir))
                            {
                                File.Delete(dir); // 지정한 파일 삭제
                            }
                            else
                            {
                                _ = MessageBox.Show("경로가 존재하지 않습니다.", "경로 미존재", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error Deleting Folder... FolderPath: {dir} : {ex.Message}");
                        }
                        finally
                        {
                            _ = ActiveFolderInfo.Remove(match); // [ActiveFolderInfo] 컬렉션 => selectToDelete 항목 제외
                        }

                    }

                }

            }

        }

        /// <summary>
        /// 4. [일괄 삭제하기] 기능 (버튼)
        /// </summary>
        private void DelAllMatches()
        {
            IsDelBtnEnabledOrNot = false; // [일괄 삭제하기] 버튼 비활성화
            if (!string.IsNullOrEmpty(DeleteFolderPath))
            {
                if (DeleteFolderInfo?.Count > 0)
                {
                    foreach (DelMatchingInfo match in DeleteFolderInfo)
                    {
                        string dir = match.DelMatchingPath;

                        // [try ~ catch]문 활용, 예외 처리!
                        try
                        {
                            IsDelBtnEnabledOrNot = true; // [일괄 삭제하기] 버튼 활성화

                            // 해당 폴더 경로 존재 시,
                            if (Directory.Exists(dir))
                            {
                                Directory.Delete(dir, true); // 지정한 디렉터리 및 해당 디렉터리의 하위 디렉터리 및 폴더 삭제
                            }
                            // 해당 파일 경로 존재 시,
                            else if (File.Exists(dir))
                            {
                                File.Delete(dir); // 지정한 파일 삭제
                            }
                            else
                            {
                                _ = MessageBox.Show("경로가 존재하지 않습니다.", "경로 미존재", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error Deleting Folder... FolderPath: {dir} : {ex.Message}");
                        }
                        finally
                        {
                            DeleteFolderInfo?.Clear(); // (전체) 컬렉션 초기화
                        }

                    }

                }

            }

        }

        /// <summary>
        /// 5. [검색 필터리셋] 기능 (FilterFolderName)
        /// </summary>
        private void FilterResetFN()
        {
            if (FilterFolderName?.Length > 0)
            {
                FilterFolderName = string.Empty;
            }
            EnumerateFolders(); // 필터링 초기화
        }

        /// <summary>
        /// 6. [검색 필터리셋] 기능 (FilterExtensions)
        /// </summary>
        private void FilterResetFE()
        {
            if (FilterExtensions?.Length > 0)
            {
                FilterExtensions = string.Empty;
            }
            EnumerateFolders(); // 필터링 초기화
        }

    }
    #endregion
}