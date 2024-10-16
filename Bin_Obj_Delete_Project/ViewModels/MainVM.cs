﻿using Bin_Obj_Delete_Project.Common;
using Bin_Obj_Delete_Project.Models;
using Bin_Obj_Delete_Project.Views;
using Microsoft.VisualBasic.FileIO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SearchOption = System.IO.SearchOption;

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
        /// [_aVisibleLoadingOrNot]
        /// </summary>
        private bool _aVisibleLoadingOrNot;

        /// <summary>
        /// [loadingControl]
        /// </summary>
        private UserControl loadingControl;

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
        /// [ascendingOrDescending]
        /// </summary>
        private bool orderByAscendingOrNot;

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
        private long matchingFldrSize;

        /// <summary>
        /// [matchingFileSize]
        /// </summary>
        private long matchingFileSize;

        /// <summary>
        /// [matchingFldrPath]
        /// </summary>
        private string matchingFldrPath;

        /// <summary>
        /// [matchingFilePath]
        /// </summary>
        private string matchingFilePath;

        /// <summary>
        /// [mouseHook]
        /// </summary>
        public static GlobalMouseHook mouseHook;

        /// <summary>
        /// [AbsolutePath]
        /// </summary>
        public static string AbsolutePath { get; set; }

        /// <summary>
        /// [uniqueFilePathSet]
        /// </summary>
        private readonly HashSet<string> uniqueFilePathSet;

        /// <summary>
        /// [enumerateFldrCache]
        /// </summary>
        private readonly Dictionary<string, IEnumerable<string>> enumerateFldrCache;

        /// <summary>
        /// [selectToDelete]
        /// </summary>
        private List<DelMatchingInfo> selectToDelete;

        /// <summary>
        /// [entireToDelete]
        /// </summary>
        private List<DelMatchingInfo> entireToDelete;

        /// <summary>
        /// [lstOrderByName]
        /// </summary>
        private List<DelMatchingInfo> lstOrderByName;

        /// <summary>
        /// [lstOrderByCrTime]
        /// </summary>
        private List<DelMatchingInfo> lstOrderByCrTime;

        /// <summary>
        /// [lstOrderByType]
        /// </summary>
        private List<DelMatchingInfo> lstOrderByType;

        /// <summary>
        /// [lstOrderByMdTime]
        /// </summary>
        private List<DelMatchingInfo> lstOrderByMdTime;

        /// <summary>
        /// [lstOrderBySize]
        /// </summary>
        private List<DelMatchingInfo> lstOrderBySize;

        /// <summary>
        /// [lstOrderByPath]
        /// </summary>
        private List<DelMatchingInfo> lstOrderByPath;

        /// <summary>
        /// [totalNumbersInfo]
        /// </summary>
        private static int totalNumbersInfo;

        /// <summary>
        /// [selectedCntsInfo]
        /// </summary>
        private static int selectedCntsInfo;

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

        #region [Action]

        public Action CloseWindowAction { get; set; }

        #endregion

        #region [OnPropertyChanged]

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// [IsDelBtnEnabledOrNot]
        /// </summary>
        public bool DelBtnEnabledOrNot
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
        /// [VisibleLoading]
        /// </summary>
        public bool VisibleLoading
        {
            get => _aVisibleLoadingOrNot;
            set
            {
                if (_aVisibleLoadingOrNot != value)
                {
                    _aVisibleLoadingOrNot = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [LoadingControl]
        /// </summary>
        public UserControl LoadingControl
        {
            get => loadingControl;
            set
            {
                if (loadingControl != value)
                {
                    loadingControl = value;
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
        /// [TotalNumberInfo]
        /// </summary>
        public int TotalNumbersInfo
        {
            get => totalNumbersInfo;
            set
            {
                if (totalNumbersInfo != value)
                {
                    totalNumbersInfo = value;
                    OnPropertyChanged();
                }

            }
        }

        /// <summary>
        /// [TotalSelectInfo]
        /// </summary>
        public int SelectedCntsInfo
        {
            get => selectedCntsInfo;
            set
            {
                if (selectedCntsInfo != value)
                {
                    selectedCntsInfo = value;
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

        // 5-1. 검색 필터리셋 (FilterFolderName)
        public ICommand FilterResetFNCommand { get; set; }

        // 5-2. 검색 필터리셋 (FilterExtensions)
        public ICommand FilterResetFECommand { get; set; }

        // 6-1. 정렬 (이름 순)
        public ICommand GoOrderByNameCommand { get; set; }

        // 6-2. 정렬 (생성한 날짜 순)
        public ICommand OrderByCrTimeCommand { get; set; }

        // 6-3. 정렬 (유형 순)
        public ICommand GoOrderByTypeCommand { get; set; }

        // 6-4. 정렬 (수정한 날짜 순)
        public ICommand OrderByMdTimeCommand { get; set; }

        // 6-5. 정렬 (크기 순)
        public ICommand GoOrderBySizeCommand { get; set; }

        // 6-6. 정렬 (경로 순)
        public ICommand GoOrderByPathCommand { get; set; }

        #endregion

        #region 생성자 (Initialize)

        public MainVM()
        {
            DelBtnEnabledOrNot = true;
            VisibleLoading = false;
            LoadingControl = new LoadingView();
            SelectedCntsInfo = 0;
            LoadingFolderCommand = new RelayCommand(LoadingFolder);
            EnterLoadPathCommand = new RelayCommand(EnterLoadPath);
            _selectedCrFolder = new DelMatchingInfo();
            _selectFolderInfo = new ObservableCollection<DelMatchingInfo>();
            _deleteFolderInfo = new ObservableCollection<DelMatchingInfo>();
            _activeFolderInfo = new ObservableCollection<DelMatchingInfo>();
            uniqueFilePathSet = new HashSet<string>();
            enumerateFldrCache = new Dictionary<string, IEnumerable<string>>();
            lstOrderByName = new List<DelMatchingInfo>();
            lstOrderByCrTime = new List<DelMatchingInfo>();
            lstOrderByType = new List<DelMatchingInfo>();
            lstOrderByMdTime = new List<DelMatchingInfo>();
            lstOrderBySize = new List<DelMatchingInfo>();
            lstOrderByPath = new List<DelMatchingInfo>();
            orderByAscendingOrNot = true;
            matchingFldrName = string.Empty;
            matchingFileName = string.Empty;
            matchingFldrCreationTime = string.Empty;
            matchingFileCreationTime = string.Empty;
            matchingFldrCategory = string.Empty;
            matchingFileCategory = string.Empty;
            matchingFldrModifiedTime = string.Empty;
            matchingFileModifiedTime = string.Empty;
            matchingFldrSize = 0;
            matchingFileSize = 0;
            matchingFldrPath = string.Empty;
            matchingFilePath = string.Empty;
            mouseHook = new GlobalMouseHook();
            DelSelMatchesCommand = new RelayCommand(DelSelMatches);
            DelAllMatchesCommand = new RelayCommand(DelAllMatches);
            FilterResetFNCommand = new RelayCommand(FilterResetFN);
            FilterResetFECommand = new RelayCommand(FilterResetFE);
            GoOrderByNameCommand = new RelayCommand(GoOrderByName);
            OrderByCrTimeCommand = new RelayCommand(OrderByCrTime);
            GoOrderByTypeCommand = new RelayCommand(GoOrderByType);
            OrderByMdTimeCommand = new RelayCommand(OrderByMdTime);
            GoOrderBySizeCommand = new RelayCommand(GoOrderBySize);
            GoOrderByPathCommand = new RelayCommand(GoOrderByPath);
        }

        #endregion

        #region [버튼 및 기능]

        /// <summary>
        /// [폴더 및 파일 열기] (기능)
        /// </summary>
        public void StartContents()
        {
            string path = SelectedCrFolder.DelMatchingPath;
            try
            {
                // 선택된 아이템이 "파일 폴더"(= 폴더)이거나, "응용 프로그램"(= 파일[.exe])일 경우, 파일 탐색기를 통해 열 수 있음.
                _ = SelectedCrFolder.DelMatchingCategory == "파일 폴더" || SelectedCrFolder.DelMatchingCategory == "응용 프로그램"
                    ? Process.Start(new ProcessStartInfo()
                    {
                        FileName = path, // 선택된 폴더의 경로
                        UseShellExecute = true // 운영체제 셸 사용 여부: Yes
                    })
                    // 선택된 아이템이 (그 밖의 파일)에 해당하는 경우, [앱 선택] 창을 통해 열도록 함.
                    : Process.Start(new ProcessStartInfo()
                    {
                        FileName = "rundll32.exe", // "rundll32.exe" 사용 DLL 호출
                        Arguments = $"shell32.dll,OpenAs_RunDLL {path}", // [OpenWith] 창 호출 명령어
                        UseShellExecute = false, // 운영체제 셸 사용 여부: No
                        RedirectStandardError = true, // 표준 오류 리다이렉션 여부: Yes
                        CreateNoWindow = true // 창 없이 실행
                    });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening contents: {ex.Message}");
            }

        }

        /// <summary>
        /// [작업 수행 및 취소] (기능)
        /// </summary>
        private async void OperatingTask()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            TotalNumbersInfo = 0; // 총 항목 개수 초기화
            //mouseHook.HookMouse();
            DelBtnEnabledOrNot = false;
            VisibleLoading = true;
            await Task.Delay(1000);
            Task enumerateTask = Task.Run(() =>
            {
                EnumerateFolders(cancellationToken);
            }, cancellationToken);
            try
            {
                Task cancelingTask = Task.Delay(60000); // 60초 후 작업 취소!
                Task completedTask = await Task.WhenAny(enumerateTask, cancelingTask);
                // 60초가 지나도 작업이 끝나지 않을 때, 작업 취소 요청!
                if (completedTask == cancelingTask)
                {
                    cancellationTokenSource.Cancel();
                    Console.WriteLine("Task has been canceled. Please perform another task.");
                    //mouseHook.UnhookMouse();
                    VisibleLoading = false;
                    DelBtnEnabledOrNot = true;
                    _ = MessageBox.Show("로딩 시간이 초과되었습니다. 다른 작업을 수행하세요.", "작업 취소", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                await enumerateTask; // 해당 작업 수행!
                //mouseHook.UnhookMouse();
                VisibleLoading = false;
                DelBtnEnabledOrNot = true;
                TotalNumbersInfo = ActiveFolderInfo.Count(); // 총 항목 개수
                enumerateTask.Dispose();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("The task has been canceled.");
            }

        }

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

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DeleteFolderPath = folderDialog.FileName;
                DeleteFolderInfo = new ObservableCollection<DelMatchingInfo>(); // [ObservableCollection] 초기화
                DeleteFolderInfo?.Clear(); // (전체) 컬렉션 초기화
                uniqueFilePathSet.Clear(); // (중복) 해시셋 초기화
                ActiveFolderInfo?.Clear(); // (화면) 초기화
                OperatingTask(); // 작업 수행 및 취소
            }

        }

        /// <summary>
        /// 2. [경로 불러오기] 기능 (Enter 키)
        /// </summary>
        public void EnterLoadPath()
        {
            // [DeleteFolderPath] 경로 null 처리 조건문 추가!
            if (!string.IsNullOrWhiteSpace(DeleteFolderPath))
            {
                DeleteFolderPath = Path.GetFullPath(AbsolutePath);
                DeleteFolderInfo = new ObservableCollection<DelMatchingInfo>(); // [ObservableCollection] 초기화
                DeleteFolderInfo?.Clear(); // (전체) 컬렉션 초기화
                uniqueFilePathSet.Clear(); // (중복) 해시셋 초기화
                ActiveFolderInfo?.Clear(); // (화면) 초기화
                OperatingTask(); // 작업 수행 및 취소
                CloseWindowAction?.Invoke(); // 창 닫기 동작 호출!
            }
            else
            {
                FilterFolderName = string.Empty;
                FilterExtensions = string.Empty;
                _ = MessageBox.Show("불러올 폴더 경로가 없습니다.", "경로 미입력", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// [하위 디렉토리] 파일의 총량 계산 (기능)
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private static long GetDirectorySize(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir); // DirectoryInfo 객체 생성

            long sizeofDir = 0; // [총량] 초기화

            // [현재 디렉토리] 및 [모든 하위 디렉토리]를 포함한 파일 목록 배열을 반환!
            FileInfo[] arrayInfo = dirInfo.GetFiles("*", SearchOption.AllDirectories);

            // 파일 목록을 돌며 파일의 총량 계산!
            foreach (FileInfo files in arrayInfo)
            {
                sizeofDir += files.Length;
            };
            return sizeofDir;
        }

        protected void EnumerateFolders(CancellationToken cancellationToken)
        {
            try
            {
                foreach (string dir in GetEneumerateFldrList())
                {
                    // 작업 취소 요청 (약 60초) 후, 작업 취소 수행
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    matchingFldrName = dirInfo.Name;
                    matchingFldrCreationTime = dirInfo.CreationTime.ToString();
                    matchingFldrCategory = "파일 폴더";
                    matchingFldrModifiedTime = dirInfo.LastWriteTime.ToString();
                    matchingFldrSize = GetDirectorySize(dir);
                    matchingFldrPath = dir;
                    matchingFileInfoOrNot = false; // [폴더]로 구분
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

                    // [Filter 01]: 폴더 이름으로 검색(대소문자 구분(X))
                    // 지정한 배열에 정의된 조건과 일치하는지 여부 확인!
                    // 1) [FilterFolderName]이 null이거나 string.Empty 문자열인 경우
                    // 2) 콤마(',')로 구분된 [FilterFolderName]이 디렉토리 또는 하위 디렉토리 폴더의 이름과 일치하는 경우
                    bool folderMatches2 = string.IsNullOrEmpty(FilterFolderName) ||
                         Array.Exists(filterComma1, comma1 => dirInfo.Name.Equals(comma1.Trim(), StringComparison.OrdinalIgnoreCase));

                    // 1), 2)가 아닐 때,
                    if (!folderMatches2)
                    {
                        continue;
                    }

                    // 1. [FilterFolderName] => 해당 폴더 및 정보를 리스트의 형태로 전시!
                    // 즉, 필터링 (X) => 필터링 없이 전시, 필터링 (O) => 필터링해서 전시!
                    if (string.IsNullOrEmpty(FilterExtensions) && !matchingFileInfoOrNot)
                    {
                        DeleteFolderInfo.Add(new DelMatchingInfo
                        {
                            DelMatchingName = matchingFldrName,
                            DelMatchingCreationTime = matchingFldrCreationTime,
                            DelMatchingCategory = matchingFldrCategory,
                            DelMatchingModifiedTime = matchingFldrModifiedTime,
                            DelMatchingOfSize = matchingFldrSize,
                            DelMatchingPath = matchingFldrPath
                        });

                    }

                    // 2. 필터 키워드를 콤마(',')로 구분 후, 배열로 생성 (FilterExtensions)
                    string[] filterComma2 = string.IsNullOrEmpty(FilterExtensions) ? Array.Empty<string>() : FilterExtensions.Split(',');

                    // [Filter 02]: 파일 확장자로 검색(대소문자 구분(X))
                    // 지정한 배열에 정의된 조건과 일치하는지 여부 확인!
                    // 1) [FilterExtensions]가 null이거나 string.Empty 문자열이 아닌 경우: 빈 배열(null)을 반환
                    // 2) [FilterExtensions]가 null이거나 string.Empty 문자열인 경우: 파일 확장자를 콤마(',')로 구분 반환
                    if (!string.IsNullOrEmpty(FilterExtensions))
                    {
                        IEnumerable<FileInfo> filesInfo = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories);
                        matchingFileInfoOrNot = true; // [파일]로 구분
                        foreach (FileInfo files in filesInfo)
                        {
                            // 이미 처리가 된 파일 경로는 무시! (중복 제거)
                            if (uniqueFilePathSet.Contains(files.FullName))
                            {
                                continue;
                            }
                            // 2) 콤마(',')로 구분된 [FilterExtensions]이 파일의 확장명 부분의 문자열과 일치하는 경우 (확장자 비교)
                            if (Array.Exists(filterComma2, comma2 => files.Extension.Equals(comma2.Trim(), StringComparison.OrdinalIgnoreCase)))
                            {
                                matchingFileName = files.Name;
                                matchingFileCreationTime = files.CreationTime.ToString();
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
                                matchingFileCategory = extensionCategoryMap.TryGetValue(files.Extension, out string category) ? category : "기타 파일";
                                matchingFileModifiedTime = files.LastWriteTime.ToString();
                                matchingFileSize = files.Length;
                                matchingFilePath = files.FullName;
                                _ = uniqueFilePathSet.Add(matchingFilePath); // 중복 제거!

                                // 2. [FilterExtensions] => 해당 파일 및 정보를 리스트의 형태로 전시
                                // 즉, 필터링 (X) => 필터링 없이 전시, 필터링 (O) => 필터링해서 전시
                                if (string.IsNullOrEmpty(FilterFolderName) && matchingFileInfoOrNot)
                                {
                                    DeleteFolderInfo.Add(new DelMatchingInfo
                                    {
                                        DelMatchingName = matchingFileName,
                                        DelMatchingCreationTime = matchingFileCreationTime,
                                        DelMatchingCategory = matchingFileCategory,
                                        DelMatchingModifiedTime = matchingFileModifiedTime,
                                        DelMatchingOfSize = matchingFileSize,
                                        DelMatchingPath = matchingFilePath
                                    });

                                }

                            }

                        }

                    }

                }

            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access Denied To Directories: Exception: {ex.Message}"); // 경로에 대한 엑세스거부 오류.
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Directories Not Found: {GetEneumerateFldrList()}. Exception: {ex.Message}"); // 경로를 찾을 수 없음.
            }
            catch (PathTooLongException ex)
            {
                Console.WriteLine($"Path Is Too Long: {DeleteFolderPath}. Exception: {ex.Message}"); // 경로가 너무 긴 경우.
            }

            // DelMatchingInfo 정보 확인: 디버깅으로 확인 가능!!
            //foreach (DelMatchingInfo item in DeleteFolderInfo)
            //{
            //    Console.WriteLine(item.DelMatchingName);
            //    Console.WriteLine(item.DelMatchingCreationTime);
            //    Console.WriteLine(item.DelMatchingCategory);
            //    Console.WriteLine(item.DelMatchingModifiedTime);
            //    Console.WriteLine(item.DelMatchingOfSize);
            //    Console.WriteLine(item.DelMatchingPath);
            //}
            ActiveFolderInfo = DeleteFolderInfo; // [ActiveFolderInfo] 컬렉션에 [DeleteFolderInfo] 컬렉션을 할당
            TotalNumbersInfo = ActiveFolderInfo.Count(); // 총 항목 개수 표시
        }

        /// <summary>
        /// [폴더, 파일] 선택 삭제하기 (기능)
        /// 1) 휴지통에서 삭제
        /// 2) 영구적으로 삭제
        /// </summary>
        private void DelSelConfirm()
        {
            foreach (DelMatchingInfo match in selectToDelete)
            {
                string dir = match.DelMatchingPath;
                try
                {
                    // 해당 디렉토리의 경로가 존재할 때,
                    if (FileSystem.DirectoryExists(dir))
                    {
                        // 1) 지정한 디렉토리 및 해당 디렉토리의 하위 디렉토리 및 폴더 휴지통에서 삭제
                        FileSystem.DeleteDirectory(dir, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                        // 2) 지정한 디렉토리 및 해당 디렉토리의 하위 디렉토리 및 폴더 영구적으로 삭제
                        //FileSystem.DeleteDirectory(dir, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);

                        _ = ActiveFolderInfo.Remove(match); // UI 클리어!
                    }
                    // 해당 파일 경로 존재 시,
                    else if (FileSystem.FileExists(dir))
                    {
                        // 1) 지정한 파일 휴지통에서 삭제
                        FileSystem.DeleteFile(dir, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                        // 2) 지정한 파일 영구적으로 삭제
                        //FileSystem.DeleteFile(dir, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);

                        _ = ActiveFolderInfo.Remove(match); // UI 클리어!
                    }
                    else
                    {
                        return;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Deleting Folder... FolderPath: {dir} : {ex.Message}");
                }

            }

        }

        /// <summary>
        /// 3. [선택 삭제하기] 기능 (버튼)
        /// </summary>
        private void DelSelMatches()
        {
            if (!string.IsNullOrEmpty(DeleteFolderPath))
            {
                if (SelectFolderInfo?.Count > 0)
                {
                    selectToDelete = new List<DelMatchingInfo>(SelectFolderInfo);
                    if (!selectToDelete.Any(v => v.DelMatchingName.Equals("bin") || v.DelMatchingName.Equals("obj")))
                    {
                        // 선택된 [삭제할 폴더 유형]이 모두 "파일 폴더"인 경우에 해당 사항
                        if (selectToDelete.All(v => v.DelMatchingCategory == "파일 폴더"))
                        {
                            MessageBoxResult messageBox = MessageBox.Show("선택한 폴더를 정말 삭제하시겠습니까?", "폴더 삭제", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (messageBox == MessageBoxResult.OK)
                            {
                                DelSelConfirm();
                            }
                            else
                            {
                                return;
                            }

                        }
                        // 그 외의 경우("파일")에 해당 사항!
                        else
                        {
                            MessageBoxResult messageBox = MessageBox.Show("선택한 파일을 정말 삭제하시겠습니까?", "파일 삭제", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (messageBox == MessageBoxResult.OK)
                            {
                                DelSelConfirm();
                            }
                            else
                            {
                                return;
                            }

                        }

                    }
                    else
                    {
                        DelSelConfirm();
                    }
                    TotalNumbersInfo = ActiveFolderInfo.Count(); // UI Update (총 항목 개수)
                }

            }

        }

        /// <summary>
        /// [폴더, 파일] 일괄 삭제하기 (기능)
        /// 1) 휴지통에서 삭제
        /// 2) 영구적으로 삭제
        /// </summary>
        private void DelAllConfirm()
        {
            foreach (DelMatchingInfo match in DeleteFolderInfo)
            {
                string dir = match.DelMatchingPath;
                try
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

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Deleting Folder... FolderPath: {dir} : {ex.Message}");
                }

            }
            DeleteFolderInfo?.Clear(); // UI 클리어!
        }

        /// <summary>
        /// 4. [일괄 삭제하기] 기능 (버튼)
        /// </summary>
        private void DelAllMatches()
        {
            if (!string.IsNullOrEmpty(DeleteFolderPath))
            {
                if (DeleteFolderInfo?.Count > 0)
                {
                    entireToDelete = new List<DelMatchingInfo>(DeleteFolderInfo);
                    if (!entireToDelete.Any(v => v.DelMatchingName.Equals("bin") || v.DelMatchingName.Equals("obj")))
                    {
                        MessageBoxResult messageBox = MessageBox.Show("전체 삭제하시겠습니까?", "일괄 삭제", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        if (messageBox == MessageBoxResult.OK)
                        {
                            DelAllConfirm();
                        }
                        else
                        {
                            return;
                        }

                    }
                    else
                    {
                        DelAllConfirm();
                    }
                    TotalNumbersInfo = ActiveFolderInfo.Count(); // UI Update (총 항목 개수)
                }

            }

        }

        /// <summary>
        /// 5-1. [검색 필터리셋] 기능 (FilterFolderName)
        /// </summary>
        public async void FilterResetFN()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            if (!string.IsNullOrWhiteSpace(DeleteFolderPath))
            {
                if (!string.IsNullOrWhiteSpace(FilterFolderName))
                {
                    DeleteFolderInfo = new ObservableCollection<DelMatchingInfo>(); // [ObservableCollection] 초기화
                    DeleteFolderInfo?.Clear(); // (전체) 컬렉션 초기화
                    uniqueFilePathSet.Clear(); // (중복) 해시셋 초기화
                    ActiveFolderInfo?.Clear(); // (화면) 초기화
                    FilterFolderName = string.Empty; // TextBox 초기화
                    CloseWindowAction?.Invoke(); // 창 닫기 동작 호출!
                    TotalNumbersInfo = 0; // 총 항목 개수 초기화
                    DelBtnEnabledOrNot = false;
                    VisibleLoading = true;
                    await Task.Delay(1000);
                    await Task.Run(() =>
                    {
                        EnumerateFolders(cancellationToken); // [Filter 01] 초기화
                    });
                    DelBtnEnabledOrNot = true;
                    VisibleLoading = false;
                }
                else
                {
                    _ = MessageBox.Show("초기화 할 내용이 없습니다.", "재입력 필요", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            else
            {
                FilterFolderName = string.Empty;
                _ = MessageBox.Show("초기화 할 경로가 없습니다.", "경로 미입력", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// 5-2. [검색 필터리셋] 기능 (FilterExtensions)
        /// </summary>
        public async void FilterResetFE()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            if (!string.IsNullOrWhiteSpace(DeleteFolderPath))
            {
                if (!string.IsNullOrWhiteSpace(FilterExtensions))
                {
                    DeleteFolderInfo = new ObservableCollection<DelMatchingInfo>(); // [ObservableCollection] 초기화
                    DeleteFolderInfo?.Clear(); // (전체) 컬렉션 초기화
                    uniqueFilePathSet.Clear(); // (중복) 해시셋 초기화
                    ActiveFolderInfo?.Clear(); // (화면) 초기화
                    FilterExtensions = string.Empty; // TextBox 초기화
                    CloseWindowAction?.Invoke(); // 창 닫기 동작 호출!
                    TotalNumbersInfo = 0; // 총 항목 개수 초기화
                    DelBtnEnabledOrNot = false;
                    VisibleLoading = true;
                    await Task.Delay(1000);
                    await Task.Run(() =>
                    {
                        EnumerateFolders(cancellationToken); // [Filter 02] 초기화
                    });
                    DelBtnEnabledOrNot = true;
                    VisibleLoading = false;
                }
                else
                {
                    _ = MessageBox.Show("초기화 할 내용이 없습니다.", "재입력 필요", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            else
            {
                FilterExtensions = string.Empty;
                _ = MessageBox.Show("초기화 할 경로가 없습니다.", "경로 미입력", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// 6-1. 정렬 (이름 순)
        /// </summary>
        private void GoOrderByName()
        {
            // (n = 1, 2, 3...)
            // (2n - 1)번 클릭 후: [오름차순] 정렬!
            // (2n)번 클릭 후: [내림차순]으로 정렬!
            lstOrderByName = orderByAscendingOrNot
                ? ActiveFolderInfo.OrderBy(item => item.DelMatchingName).ToList()
                : ActiveFolderInfo.OrderByDescending(item => item.DelMatchingName).ToList();

            // 1) 정렬 후, 컬렉션 초기화 작업
            ActiveFolderInfo = new ObservableCollection<DelMatchingInfo>(lstOrderByName);

            // 2) 플래그(flag) 값, 반전시키기
            orderByAscendingOrNot = !orderByAscendingOrNot;
        }

        /// <summary>
        /// 6-2. 정렬 (생성한 날짜 순)
        /// </summary>
        private void OrderByCrTime()
        {
            // (n = 1, 2, 3...)
            // (2n - 1)번 클릭 후: [오름차순] 정렬!
            // (2n)번 클릭 후: [내림차순]으로 정렬!
            lstOrderByCrTime = orderByAscendingOrNot
                ? ActiveFolderInfo.OrderBy(item => item.DelMatchingCreationTime).ToList()
                : ActiveFolderInfo.OrderByDescending(item => item.DelMatchingCreationTime).ToList();

            // 1) 정렬 후, 컬렉션 초기화 작업
            ActiveFolderInfo = new ObservableCollection<DelMatchingInfo>(lstOrderByCrTime);

            // 2) 플래그(flag) 값, 반전시키기
            orderByAscendingOrNot = !orderByAscendingOrNot;
        }

        /// <summary>
        /// 6-3. 정렬 (유형 순)
        /// </summary>
        private void GoOrderByType()
        {
            // (n = 1, 2, 3...)
            // (2n - 1)번 클릭 후: [오름차순] 정렬!
            // (2n)번 클릭 후: [내림차순]으로 정렬!
            lstOrderByType = orderByAscendingOrNot
                ? ActiveFolderInfo.OrderBy(item => item.DelMatchingCategory).ToList()
                : ActiveFolderInfo.OrderByDescending(item => item.DelMatchingCategory).ToList();

            // 1) 정렬 후, 컬렉션 초기화 작업
            ActiveFolderInfo = new ObservableCollection<DelMatchingInfo>(lstOrderByType);

            // 2) 플래그(flag) 값, 반전시키기
            orderByAscendingOrNot = !orderByAscendingOrNot;
        }

        /// <summary>
        /// 6-4. 정렬 (수정한 날짜 순)
        /// </summary>
        private void OrderByMdTime()
        {
            // (n = 1, 2, 3...)
            // (2n - 1)번 클릭 후: [오름차순] 정렬!
            // (2n)번 클릭 후: [내림차순]으로 정렬!
            lstOrderByMdTime = orderByAscendingOrNot
                ? ActiveFolderInfo.OrderBy(item => item.DelMatchingModifiedTime).ToList()
                : ActiveFolderInfo.OrderByDescending(item => item.DelMatchingModifiedTime).ToList();

            // 1) 정렬 후, 컬렉션 초기화 작업
            ActiveFolderInfo = new ObservableCollection<DelMatchingInfo>(lstOrderByMdTime);

            // 2) 플래그(flag) 값, 반전시키기
            orderByAscendingOrNot = !orderByAscendingOrNot;
        }

        /// <summary>
        /// 6-5. 정렬 (크기 순)
        /// </summary>
        private void GoOrderBySize()
        {
            // (n = 1, 2, 3...)
            // (2n - 1)번 클릭 후: [오름차순] 정렬!
            // (2n)번 클릭 후: [내림차순]으로 정렬!
            lstOrderBySize = orderByAscendingOrNot
                ? ActiveFolderInfo.OrderBy(item => item.DelMatchingOfSize).ToList()
                : ActiveFolderInfo.OrderByDescending(item => item.DelMatchingOfSize).ToList();

            // 1) 정렬 후, 컬렉션 초기화 작업
            ActiveFolderInfo = new ObservableCollection<DelMatchingInfo>(lstOrderBySize);

            // 2) 플래그(flag) 값, 반전시키기
            orderByAscendingOrNot = !orderByAscendingOrNot;
        }

        /// <summary>
        /// 6-6. 정렬 (경로 순)
        /// </summary>
        private void GoOrderByPath()
        {
            // (n = 1, 2, 3...)
            // (2n - 1)번 클릭 후: [오름차순] 정렬!
            // (2n)번 클릭 후: [내림차순]으로 정렬!
            lstOrderByPath = orderByAscendingOrNot
                ? ActiveFolderInfo.OrderBy(item => item.DelMatchingPath).ToList()
                : ActiveFolderInfo.OrderByDescending(item => item.DelMatchingPath).ToList();

            // 1) 정렬 후, 컬렉션 초기화 작업
            ActiveFolderInfo = new ObservableCollection<DelMatchingInfo>(lstOrderByPath);

            // 2) 플래그(flag) 값, 반전시키기
            orderByAscendingOrNot = !orderByAscendingOrNot;
        }
        #endregion

        #region [캐싱 처리]

        /// <summary>
        /// [폴더 캐싱]
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetEneumerateFldrList()
        {
            // 모든 하위 디렉토리를 검색하되, 접근이 거부된 디렉토리는 제외!
            IEnumerable<string> directories = Directory.EnumerateDirectories(DeleteFolderPath, "*", SearchOption.AllDirectories);
            //.Where(dir => dir.EndsWith("bin") || dir.EndsWith("obj")); // 경로의 마지막 글자가 "bin"이거나 "obj"인 파일만 찾음!
            // 하위 디렉토리 중 요소가 하나라도 존재하면,
            if (directories != null && directories.Any())
            {
                // [Cache]에 데이터 존재 여부 확인!
                if (enumerateFldrCache.ContainsKey(DeleteFolderPath))
                {
                    return enumerateFldrCache[DeleteFolderPath];
                }
                // [Cache]에 데이터가 없으면, 데이터를 새로 저장함!
                enumerateFldrCache[DeleteFolderPath] = directories;
            }
            return directories;
        }
        #endregion
    }

}