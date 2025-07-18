﻿using Bin_Obj_Delete_Project.Common;
using Bin_Obj_Delete_Project.Models;
using Bin_Obj_Delete_Project.Services;
using Bin_Obj_Delete_Project.Views;
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

namespace Bin_Obj_Delete_Project.ViewModels
{
    public class MainVM : INotifyPropertyChanged
    {
        #region [프로퍼티]

        /// <summary>
        /// [_enumerateService]
        /// </summary>
        private readonly IEnumerateService _enumerateService;

        /// <summary>
        /// [_deleteService]
        /// </summary>
        private readonly IDeleteService _deleteService;

        /// <summary>
        /// [_recyclebinService]
        /// </summary>
        private readonly IRecycleBinService _recycleBinService;

        /// <summary>
        /// [_IssTheBtnEnabledOrNot]
        /// </summary>
        private bool _IsTheBtnEnabledOrNot;

        /// <summary>
        /// [_aVisibleLoadingOrNot]
        /// </summary>
        private bool _aVisibleLoadingOrNot;

        /// <summary>
        /// [_aVisibleDestroyOrNot]
        /// </summary>
        private bool _aVisibleDestroyOrNot;

        /// <summary>
        /// [_progressValue]
        /// </summary>
        private double _progressValue;

        /// <summary>
        /// [_progressStyle]
        /// </summary>
        private string _progressStyle;

        /// <summary>
        /// [_progressBar]
        /// </summary>
        private Progress<double> _progressBar;

        /// <summary>
        /// [loadingControl]
        /// </summary>
        private UserControl loadingControl;

        /// <summary>
        /// [destroyControl]
        /// </summary>
        private UserControl destroyControl;

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
        /// [lastSortAscending]
        /// </summary>
        private bool lastSortAscending;

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
        /// [_lstAllData]
        /// </summary>
        private List<DelMatchingInfo> _lstAllData;

        /// <summary>
        /// [_lstDelInfo]
        /// </summary>
        private List<DelMatchingInfo> _lstDelInfo;

        /// <summary>
        /// [lastSortKey]
        /// </summary>
        private Func<DelMatchingInfo, object> lastSortKey;

        /// <summary>
        /// [deletedSuccessfully]
        /// </summary>
        private readonly List<DelMatchingInfo> deletedSuccessfully;

        /// <summary>
        /// [_currentPage]
        /// </summary>
        private int _currentPage;

        /// <summary>
        /// [_pageRecords]
        /// </summary>
        private int _pageRecords;

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
        /// [TheBtnEnabledOrNot]
        /// </summary>
        public bool TheBtnEnabledOrNot
        {
            get => _IsTheBtnEnabledOrNot;
            private set
            {
                if (_IsTheBtnEnabledOrNot != value)
                {
                    _IsTheBtnEnabledOrNot = value;
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
        /// [VisibleDestroy]
        /// </summary>
        public bool VisibleDestroy
        {
            get => _aVisibleDestroyOrNot;
            set
            {
                if (_aVisibleDestroyOrNot != value)
                {
                    _aVisibleDestroyOrNot = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ProgressValue]
        /// </summary>
        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ProgressStyle]
        /// </summary>
        public string ProgressStyle
        {
            get => _progressStyle;
            set
            {
                if (_progressStyle != value)
                {
                    _progressStyle = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ProgressBar]
        /// </summary>
        public Progress<double> ProgressBar
        {
            get => _progressBar;
            private set
            {
                if (_progressBar != value)
                {
                    _progressBar = value;
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
        /// [DestroyControl]
        /// </summary>
        public UserControl DestroyControl
        {
            get => destroyControl;
            set
            {
                if (destroyControl != value)
                {
                    destroyControl = value;
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
        /// [SelectedCntsInfo]
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
        /// [LstAllData]
        /// [전체 데이터 개수] (List 형태)
        /// </summary>
        public List<DelMatchingInfo> LstAllData
        {
            get => _lstAllData;
            set
            {
                if (_lstAllData != value)
                {
                    _lstAllData = value;
                    OnPropertyChanged();
                    LoadPageData();
                }

            }

        }

        /// <summary>
        /// [LstDelInfo]
        /// [삭제(복원) 데이터 정보] (List 형태)
        /// </summary>
        public List<DelMatchingInfo> LstDelInfo
        {
            get => _lstDelInfo;
            set
            {
                if (_lstDelInfo != value)
                {
                    _lstDelInfo = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [CurrentPage]
        /// [현재 페이지]
        /// </summary>
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [PageRecords]
        /// [페이지 당 데이터 개수]
        /// </summary>
        public int PageRecords
        {
            get => _pageRecords = 100;
            set
            {
                if (_pageRecords != value)
                {
                    _pageRecords = value;
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

        // 7-1. 다음 페이지로 이동
        public ICommand GoToNextPageCommand { get; set; }

        // 7-2. 이전 페이지로 이동
        public ICommand GoToPreviousPageCommand { get; set; }

        // 8. 휴지통 복원하기
        public ICommand RestoreFromRecycleBinCommand { get; set; }

        #endregion

        #region 생성자 (Initialize)

        public MainVM() : this(new EnumerateService(), new DeleteService(), new RecycleBinService())
        {
            TheBtnEnabledOrNot = true;
            VisibleLoading = false;
            VisibleDestroy = false;
            ProgressBar = new Progress<double>(value =>
            {
                ProgressValue = value;
                ProgressStyle = $"{value:F0}%"; // [정수 포맷] 형태
            });
            LoadingControl = new LoadingView();
            DestroyControl = new DestroyView();
            SelectedCntsInfo = 0;
            LoadingFolderCommand = new RelayCommand(LoadingFolder);
            EnterLoadPathCommand = new RelayCommand(EnterLoadPath);
            SelectedCrFolder = new DelMatchingInfo();
            LstAllData = new List<DelMatchingInfo>();
            LstDelInfo = new List<DelMatchingInfo>();
            lastSortKey = new Func<DelMatchingInfo, object>(x => x.DelMatchingOfSize); // 기본 정렬
            deletedSuccessfully = new List<DelMatchingInfo>();
            SelectFolderInfo = new ObservableCollection<DelMatchingInfo>();
            DeleteFolderInfo = new ObservableCollection<DelMatchingInfo>();
            ActiveFolderInfo = new ObservableCollection<DelMatchingInfo>();
            uniqueFilePathSet = new HashSet<string>();
            enumerateFldrCache = new Dictionary<string, IEnumerable<string>>();
            orderByAscendingOrNot = true;
            lastSortAscending = true;
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
            DelSelMatchesCommand = new AsyncRelayCommand(DelSelMatches);
            DelAllMatchesCommand = new AsyncRelayCommand(DelAllMatches);
            FilterResetFNCommand = new AsyncRelayCommand(FilterResetFN);
            FilterResetFECommand = new AsyncRelayCommand(FilterResetFE);
            GoOrderByNameCommand = new RelayCommand(GoOrderByName);
            OrderByCrTimeCommand = new RelayCommand(OrderByCrTime);
            GoOrderByTypeCommand = new RelayCommand(GoOrderByType);
            OrderByMdTimeCommand = new RelayCommand(OrderByMdTime);
            GoOrderBySizeCommand = new RelayCommand(GoOrderBySize);
            GoOrderByPathCommand = new RelayCommand(GoOrderByPath);
            GoToNextPageCommand = new RelayCommand(GoToNextPage);
            GoToPreviousPageCommand = new RelayCommand(GoToPreviousPage);
            RestoreFromRecycleBinCommand = new AsyncRelayCommand(RestoreFromRecycleBin);
        }


        /// <summary>
        /// 의존성 주입 생성자 (Service 인터페이스)
        /// </summary>
        /// <param name="enumerateService"></param>
        /// <param name="deleteService"></param>
        /// <param name="recycleBinService"></param>
        public MainVM(EnumerateService enumerateService, IDeleteService deleteService, IRecycleBinService recycleBinService)
        {
            _enumerateService = enumerateService;
            _deleteService = deleteService;
            _recycleBinService = recycleBinService;
        }

        #endregion

        #region [버튼 및 기능]

        /// <summary>
        /// [폴더 및 파일 열기] (기능)
        /// </summary>
        public void StartContents()
        {
            string path = SelectedCrFolder.DelMatchingPath;
            if (TheBtnEnabledOrNot)
            {
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
            return;
        }

        /// <summary>
        /// [작업 수행 및 취소] (기능)
        /// </summary>
        private async void OperatingTask()
        {
            //mouseHook.HookMouse();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            TotalNumbersInfo = 0; // 총 항목 개수 초기화
            TheBtnEnabledOrNot = false;
            VisibleLoading = true;
            Task enumerateTask = Task.Run(() =>
            {
                return EnumerateFolders(cancellationToken, ProgressBar, ProgressBar); // 비동기 호출 반환
            }, cancellationToken);
            try
            {
                Task cancelingTask = Task.Delay(300000); // [5분] 이후, 작업 취소!!!!!
                Task completedTask = await Task.WhenAny(enumerateTask, cancelingTask);
                // [5분] 이후가 지나도 작업이 끝나지 않으면, 작업 취소 요청!
                if (completedTask == cancelingTask)
                {
                    cancellationTokenSource.Cancel();
                    Console.WriteLine("Task has been canceled. Please perform another task.");
                    //mouseHook.UnhookMouse();
                    VisibleLoading = false;
                    TheBtnEnabledOrNot = true;
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        DeleteFolderInfo?.Clear();
                        LstAllData?.Clear();
                        Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                        _ = MessageBox.Show(mainWindow, "로딩 시간이 초과되었습니다. 다른 작업을 수행하세요.", "작업 취소", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }).Task;
                    return;
                }
                await enumerateTask; // 해당 작업 수행!
                //mouseHook.UnhookMouse();
                VisibleLoading = false;
                TheBtnEnabledOrNot = true;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("The task has been canceled.");
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    enumerateTask.Dispose();
                }
                TotalNumbersInfo = LstAllData.Count(); // 전체 데이터 개수!
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
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                    _ = MessageBox.Show(mainWindow, "불러올 폴더 경로가 없습니다.", "경로 미입력", MessageBoxButton.OK, MessageBoxImage.Error);
                });

            }

        }

        /// <summary>
        /// [비동기적] 폴더 열거(탐색) 및 작업의 진행률 업데이트 및 UI 도시 (기능)
        /// </summary>
        /// <param name="cancellationToken">작업 취소</param>
        /// <param name="progress">작업 진행률</param>
        /// <returns>작업 완료 후, Task 반환</returns>
        protected async Task EnumerateFolders(CancellationToken cancellationToken, IProgress<double> fldrProgress, IProgress<double> fileProgress)
        {
            fldrProgress?.Report(0); // [폴더 진행률: 0]으로 초기화
            fileProgress?.Report(0); // [파일 진행률: 0]으로 초기화
            try
            {
                IEnumerable<string> lstEneumerateFldr = await Task.Run(() => GetEneumerateFldrList());
                int totalFldrs = lstEneumerateFldr.Count();
                int processedFldrs = 0;
                foreach (string dir in lstEneumerateFldr)
                {
                    // 작업 취소 요청 (5분) 이후 작업 취소 수행!!!
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    matchingFldrName = dirInfo.Name;
                    matchingFldrCreationTime = dirInfo.CreationTime.ToString("yyyy-MM-dd tt HH:mm:ss");
                    matchingFldrCategory = "파일 폴더";
                    matchingFldrModifiedTime = dirInfo.LastWriteTime.ToString("yyyy-MM-dd tt HH:mm:ss");
                    matchingFldrSize = await Task.Run(() => _enumerateService.GetDirectorySize(dir));
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
                        processedFldrs++;
                        fldrProgress.Report((double)processedFldrs / totalFldrs * 100);
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
                        IEnumerable<FileInfo> lstEnumerateFilesInfo = await Task.Run(() => _enumerateService.GetFiles(dirInfo));
                        IEnumerable<FileInfo> lst = lstEnumerateFilesInfo;
                        int totalFiles = lstEnumerateFilesInfo.Count();
                        int processedFiles = 0;
                        matchingFileInfoOrNot = true; // [파일]로 구분됨!
                        foreach (FileInfo files in lstEnumerateFilesInfo)
                        {
                            // 작업 취소 요청 (5분) 이후 작업 취소 수행!!!
                            if (cancellationToken.IsCancellationRequested)
                            {
                                return;
                            }

                            // 이미 처리가 된 파일 경로는 무시! (중복 제거)
                            if (uniqueFilePathSet.Contains(files.FullName))
                            {
                                continue;
                            }

                            // 2) 콤마(',')로 구분된 [FilterExtensions]이 파일의 확장명 부분의 문자열과 일치하는 경우 (확장자 비교)
                            if (Array.Exists(filterComma2, comma2 => files.Extension.Equals(comma2.Trim(), StringComparison.OrdinalIgnoreCase)))
                            {
                                matchingFileName = files.Name;
                                matchingFileCreationTime = files.CreationTime.ToString("yyyy-MM-dd tt HH:mm:ss");
                                Dictionary<string, string> extensionCategoryMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                {
                                    { ".7z", "압축(7Z) 파일" },
                                    { ".aac", "오디오 압축 파일" },
                                    { ".avi", "AVI 비디오 파일" },
                                    { ".baml", "BAML 파일" },
                                    { ".bmp", "비압축 비트맵 이미지 파일" },
                                    { ".cache", "CACHE 파일" },
                                    { ".cfg", "CFG 파일" },
                                    { ".cs", "C# Source File" },
                                    { ".csproj", "C# Project File" },
                                    { ".csv", "Microsoft Excel 쉼표로 구분된 값 파일" },
                                    { ".config", "VisualStudio.config" },
                                    { ".dll", "응용 프로그램 확장" },
                                    { ".doc", "Word 97–2003 문서" },
                                    { ".docx", "Word 문서" },
                                    { ".exe", "응용 프로그램" },
                                    { ".flac", "무손실 오디오 파일" },
                                    { ".gitattributes", "txtfile" },
                                    { ".gitignore", "txtfile" },
                                    { ".gif", "GIF 이미지 파일" },
                                    { ".hwp", "한컴오피스 한글 문서" },
                                    { ".jpeg", "JPEG 이미지 파일" },
                                    { ".jpg", "JPEG 이미지 파일" },
                                    { ".md", "MD 파일" },
                                    { ".mp3", "MP3 오디오 파일" },
                                    { ".mp4", "MP4 비디오 파일" },
                                    { ".nupkg", "NUPKG 파일" },
                                    { ".p7s", "PKCS #7 서명" },
                                    { ".pdb", "Program Debug Database" },
                                    { ".pdf", "Microsoft Edge PDF Document" },
                                    { ".png", "PNG 이미지 파일" },
                                    { ".resx", "Microsoft .NET Managed Resource File" },
                                    { ".resources", "RESOURCES 파일" },
                                    { ".sln", "Visual Studio Solution" },
                                    { ".settings", "Settings-Designer File" },
                                    { ".suo", "Visual Studio Solution User Options" },
                                    { ".svg", "벡터 이미지 파일" },
                                    { ".txt", "텍스트 문서" },
                                    { ".wav", "WAV 오디오 파일" },
                                    { ".xaml", "Windows 태그 파일" },
                                    { ".xml", "xmlfile" },
                                    { ".xlsx", "Excel 통합 문서" },
                                    { ".zip", "압축(ZIP) 파일" }
                                };
                                matchingFileCategory = extensionCategoryMap.TryGetValue(files.Extension, out string category) ? category : "기타 파일";
                                matchingFileModifiedTime = files.LastWriteTime.ToString("yyyy-MM-dd tt HH:mm:ss");
                                matchingFileSize = files.Length;
                                matchingFilePath = files.FullName;
                                _ = uniqueFilePathSet.Add(matchingFilePath); // [중복] 제거

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
                        processedFiles++;
                        fileProgress?.Report((double)processedFiles / totalFiles * 100);
                    }
                    processedFldrs++;
                    fldrProgress?.Report((double)processedFldrs / totalFldrs * 100);
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                    _ = MessageBox.Show(mainWindow, $"{ex.Message}", "액세스 거부", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                // 경로에 대한 엑세스 거부 오류.
                Console.WriteLine($"Exception: Access Denied to Directories: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                    _ = MessageBox.Show(mainWindow, $"{ex.Message}", "경로 미존재", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                // 경로를 찾을 수 없음.
                Console.WriteLine($"Exception: Directories Not Found: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                    _ = MessageBox.Show(mainWindow, $"{ex.Message}", "액세스 거부", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                // 특정 객체에 대한 엑세스 거부 오류.
                Console.WriteLine($"Exception: Access Denied to the Object: {ex.Message}");
            }
            catch (PathTooLongException ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                    _ = MessageBox.Show(mainWindow, $"{ex.Message}", "경로 재설정", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                // 경로가 너무 긴 경우.
                Console.WriteLine($"Exception: Path is Too Long: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: : {ex.Message}");
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
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
                    if (ProgressValue < 100)
                    {
                        fldrProgress?.Report(100); // [진행률: 100] 작업 완료
                        fileProgress?.Report(100); // [진행률: 100] 작업 완료
                    }
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        LstAllData = DeleteFolderInfo.ToList(); // [DeleteFolderInfo] 컬렉션 데이터 리스트화 (완료)
                        ActiveFolderInfo = new ObservableCollection<DelMatchingInfo>(LstAllData.Take(PageRecords)); // 페이지 데이터 로드
                        CurrentPage = 1;
                    });

                }

            }

        }

        /// <summary>
        /// SyncActiveFolderInfo()
        /// [ActiveFolderInfo = DeleteFolderInfo]
        /// </summary>
        private void SyncActiveFolderInfo()
        {
            foreach (DelMatchingInfo item in DeleteFolderInfo)
            {
                // [정렬 또는 상태] 유지 (중복 방지!)
                if (!ActiveFolderInfo.Contains(item))
                {
                    ActiveFolderInfo.Add(item);
                }

            }

        }

        /// <summary>
        /// ResetUIState()
        /// 취소 후 UI 상태를 초기화
        /// </summary>
        private void ResetUIState()
        {
            TheBtnEnabledOrNot = true;
            VisibleLoading = false;
            selectToDelete?.Clear();
            entireToDelete?.Clear();
            LoadPageData();
        }

        /// <summary>
        /// [폴더, 파일] 선택 삭제하기 (기능)
        /// 1) 휴지통에서 삭제
        /// 2) 영구적으로 삭제
        /// </summary>
        private async Task DelSelConfirm(IProgress<double> progress)
        {
            bool isDeletedSel = false;
            if (selectToDelete?.Count == 0)
            {
                VisibleDestroy = false;
                return;
            }
            deletedSuccessfully?.Clear();
            progress?.Report(0);
            try
            {
                /* [중복 경로 방지용 필터]
                 * [selectToDelete] 안에 있는 항목들 중에서 이미 LstDelInfo에 같은 DelMatchingPath를 가진 항목이 없을 경우에만 누적 추가 */
                LstDelInfo?.AddRange(selectToDelete?.Where(sel => !LstDelInfo.Any(saved => saved.DelMatchingPath == sel.DelMatchingPath)));
                TheBtnEnabledOrNot = false;
                VisibleDestroy = true;
                int totalSelMatch = selectToDelete.Count();
                int processedSelMatch = 0;
                /*
                * [SelectFolderInfo]로 반복문을 돌리면 .Remove(match)을 수행 시, 
                * [ActiveFolderInfo]와 동일 참조이기 때문에, 컬렉션이 변경되므로
                * foreach문(반복문)이 중단되는 문제가 발생함. (반드시 참고)
                * 이에, 컬렉션을 .ToList()화하여 복사 후 순회하는 것이 중요!
                */
                foreach (DelMatchingInfo match in SelectFolderInfo.ToList())
                {
                    string dir = match.DelMatchingPath;
                    isDeletedSel = await _deleteService.DeleteAsync(dir, true); // _deleteService 사용
                    if (isDeletedSel)
                    {
                        deletedSuccessfully?.Add(match); // 삭제 성공 항목만 저장!
                        // [폴더, 파일] 선택 삭제하기 후, [진행률 업데이트] 작업!!
                        processedSelMatch++;
                        progress?.Report((double)processedSelMatch / totalSelMatch * 100);
                        await Task.Delay(5);
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: Error Deleting Folder... FolderPath: {ex.Message}");
            }
            finally
            {
                await Task.Delay(5);
                if (ProgressValue < 100)
                {
                    progress?.Report(100); // [진행률: 100]: 작업 완료
                }
                TheBtnEnabledOrNot = true;
                VisibleDestroy = false;
                if (isDeletedSel)
                {
                    // 삭제 후 데이터 업데이트
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (deletedSuccessfully?.Count > 0)
                        {
                            LstAllData = LstAllData.Where(item => !deletedSuccessfully.Any(deleted => deleted.DelMatchingPath == item.DelMatchingPath)).ToList();
                            DeleteFolderInfo = new ObservableCollection<DelMatchingInfo>(LstAllData);
                            selectToDelete.Clear();
                        }
                        ActiveFolderInfo = DeleteFolderInfo;
                        TotalNumbersInfo = ActiveFolderInfo.Count; // UI Update! (총 항목 개수)
                        CommonSortedFunc(); // 삭제 후, 정렬 재적용
                    });

                }

            }

        }

        /// <summary>
        /// 3. [선택 삭제하기] 기능 (버튼)
        /// </summary>
        private async Task DelSelMatches()
        {
            bool shouldDelete = false;
            if (!string.IsNullOrEmpty(DeleteFolderPath) && SelectFolderInfo?.Count > 0)
            {
                SyncActiveFolderInfo();
                selectToDelete = new List<DelMatchingInfo>(SelectFolderInfo);
                if (selectToDelete?.Count > 0 &&
                    !selectToDelete.Any(v => v.DelMatchingName.Equals("bin", StringComparison.OrdinalIgnoreCase) ||
                    v.DelMatchingName.Equals("obj", StringComparison.OrdinalIgnoreCase)))
                {
                    // 선택된 [삭제할 폴더 유형]이 모두 "파일 폴더"인 경우에 해당 사항
                    if (selectToDelete.All(v => v.DelMatchingCategory == "파일 폴더"))
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                            MessageBoxResult messageBox = MessageBox.Show(mainWindow, "선택한 폴더를 정말 삭제하시겠습니까?", "폴더 삭제", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (messageBox == MessageBoxResult.OK)
                            {
                                shouldDelete = true;
                            }

                        });

                    }
                    // 그 외의 경우("파일")에 해당 사항
                    else
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                            MessageBoxResult messageBox = MessageBox.Show(mainWindow, "선택한 파일을 정말 삭제하시겠습니까?", "파일 삭제", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (messageBox == MessageBoxResult.OK)
                            {
                                shouldDelete = true;
                            }

                        });

                    }

                }
                else
                {
                    shouldDelete = true;
                }
                if (shouldDelete)
                {
                    await DelSelConfirm(ProgressBar);
                }
                ResetUIState(); // UI 상태 초기화
            }

        }

        /// <summary>
        /// [폴더, 파일] 일괄 삭제하기 (기능)
        /// 1) 휴지통에서 삭제
        /// 2) 영구적으로 삭제
        /// </summary>
        private async Task DelAllConfirm(IProgress<double> progress)
        {
            bool isDeletedAll = false;
            if (entireToDelete?.Count == 0)
            {
                VisibleDestroy = false;
                return;
            }
            deletedSuccessfully?.Clear();
            progress?.Report(0);
            try
            {
                /* [중복 경로 방지용 필터]
                 * [entireToDelete] 안에 있는 항목들 중에서 이미 LstDelInfo에 같은 DelMatchingPath를 가진 항목이 없을 경우에만 누적 추가 */
                LstDelInfo?.AddRange(entireToDelete?.Where(ent => !LstDelInfo.Any(saved => saved.DelMatchingPath == ent.DelMatchingPath)));
                TheBtnEnabledOrNot = false;
                VisibleDestroy = true;
                int totalAllMatch = entireToDelete.Count();
                int processedAllMatch = 0;
                /*
                * [DeleteFolderInfo]로 반복문을 돌리면 .Remove(match)을 수행 시, 
                * [ActiveFolderInfo]와 동일 참조이기 때문에, 컬렉션이 변경되므로
                * foreach문(반복문)이 중단되는 문제가 발생함. (반드시 참고)
                * 이에, 컬렉션을 .ToList()화하여 복사 후 순회하는 것이 중요!
                */
                foreach (DelMatchingInfo match in DeleteFolderInfo.ToList())
                {
                    string dir = match.DelMatchingPath;
                    isDeletedAll = await _deleteService.DeleteAsync(dir, true); // _deleteService 사용
                    if (isDeletedAll)
                    {
                        deletedSuccessfully?.Add(match); // 삭제 성공 항목만 저장!
                        // [폴더, 파일] 일괄 삭제하기 후, [진행률 업데이트] 작업!!
                        processedAllMatch++;
                        progress?.Report((double)processedAllMatch / totalAllMatch * 100);
                        await Task.Delay(5);
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            ActiveFolderInfo.Remove(match); // [UI 초기화]
                            TotalNumbersInfo = ActiveFolderInfo.Count; // UI Update! (총 항목 개수)
                        });

                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: Error Deleting Folder... FolderPath: {ex.Message}");
            }
            finally
            {
                await Task.Delay(5);
                if (ProgressValue < 100)
                {
                    progress?.Report(100); // [진행률: 100]: 작업 완료
                }
                TheBtnEnabledOrNot = true;
                VisibleDestroy = false;
                if (isDeletedAll)
                {
                    // 삭제 후 데이터 업데이트
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (deletedSuccessfully?.Count > 0)
                        {
                            LstAllData = LstAllData.Where(item => !deletedSuccessfully.Any(deleted => deleted.DelMatchingPath == item.DelMatchingPath)).ToList();
                            DeleteFolderInfo = new ObservableCollection<DelMatchingInfo>(LstAllData);
                            entireToDelete.Clear();
                        }
                        CommonSortedFunc(); // 삭제 후, 정렬 재적용
                    });

                }

            }

        }

        /// <summary>
        /// 4. [일괄 삭제하기] 기능 (버튼)
        /// </summary>
        private async Task DelAllMatches()
        {
            bool shouldDelete = false;
            if (!string.IsNullOrEmpty(DeleteFolderPath) && DeleteFolderInfo?.Count > 0)
            {
                SyncActiveFolderInfo();
                entireToDelete = new List<DelMatchingInfo>(DeleteFolderInfo);
                if (entireToDelete?.Count > 0 &&
                    !entireToDelete.All(v => v.DelMatchingName.Equals("bin", StringComparison.OrdinalIgnoreCase) ||
                    v.DelMatchingName.Equals("obj", StringComparison.OrdinalIgnoreCase)))
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                        MessageBoxResult messageBox = MessageBox.Show(mainWindow, "전체 삭제하시겠습니까?", "일괄 삭제", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        if (messageBox == MessageBoxResult.OK)
                        {
                            shouldDelete = true;
                        }

                    });

                }
                else
                {
                    shouldDelete = true;
                }
                if (shouldDelete)
                {
                    await DelAllConfirm(ProgressBar);
                }
                ResetUIState(); // UI 상태 초기화
            }

        }

        /// <summary>
        /// 5-1. [검색 필터리셋] 기능 (FilterFolderName)
        /// </summary>
        public async Task FilterResetFN()
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
                    TheBtnEnabledOrNot = false;
                    VisibleLoading = true;
                    await Task.Run(() =>
                    {
                        return EnumerateFolders(cancellationToken, ProgressBar, ProgressBar); // [Filter 01] 초기화
                    });
                    TheBtnEnabledOrNot = true;
                    VisibleLoading = false;
                    // UI Update (총 항목 개수)
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        TotalNumbersInfo = LstAllData.Count();
                    });

                }
                else
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                        _ = MessageBox.Show(mainWindow, "초기화 할 내용이 없습니다.", "재입력 필요", MessageBoxButton.OK, MessageBoxImage.Information);
                    });

                }

            }
            else
            {
                FilterFolderName = string.Empty;
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                    _ = MessageBox.Show(mainWindow, "초기화 할 경로가 없습니다.", "경로 미입력", MessageBoxButton.OK, MessageBoxImage.Error);
                });

            }

        }

        /// <summary>
        /// 5-2. [검색 필터리셋] 기능 (FilterExtensions)
        /// </summary>
        public async Task FilterResetFE()
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
                    TheBtnEnabledOrNot = false;
                    VisibleLoading = true;
                    await Task.Run(() =>
                    {
                        return EnumerateFolders(cancellationToken, ProgressBar, ProgressBar); // [Filter 02] 초기화
                    });
                    TheBtnEnabledOrNot = true;
                    VisibleLoading = false;
                    // UI Update (총 항목 개수)
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        TotalNumbersInfo = LstAllData.Count();
                    });

                }
                else
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                        _ = MessageBox.Show(mainWindow, "초기화 할 내용이 없습니다.", "재입력 필요", MessageBoxButton.OK, MessageBoxImage.Information);
                    });

                }

            }
            else
            {
                FilterExtensions = string.Empty;
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                    _ = MessageBox.Show(mainWindow, "초기화 할 경로가 없습니다.", "경로 미입력", MessageBoxButton.OK, MessageBoxImage.Error);
                });

            }

        }

        /// <summary>
        /// 6. [CommonSortFunc(): 공통 정렬 함수]
        /// </summary>
        private void CommonSortedFunc()
        {
            // (n = 1, 2, 3...)
            // (2n - 1)번 클릭 후: [오름차순] 정렬!
            // (2n)번 클릭 후: [내림차순]으로 정렬!
            LstAllData = lastSortAscending
                ? LstAllData.OrderBy(lastSortKey).ToList()
                : LstAllData.OrderByDescending(lastSortKey).ToList();

            LoadPageData();
        }

        /// <summary>
        /// 6-1. 정렬 (이름 순)
        /// </summary>
        private void GoOrderByName()
        {
            lastSortKey = x => x.DelMatchingName;
            lastSortAscending = orderByAscendingOrNot; // lastSortAscending = 이전 정렬 기준
            CommonSortedFunc();
            orderByAscendingOrNot = !orderByAscendingOrNot;
        }

        /// <summary>
        /// 6-2. 정렬 (생성한 날짜 순)
        /// </summary>
        private void OrderByCrTime()
        {
            lastSortKey = x => x.DelMatchingCreationTime;
            lastSortAscending = orderByAscendingOrNot; // lastSortAscending = 이전 정렬 기준
            CommonSortedFunc();
            orderByAscendingOrNot = !orderByAscendingOrNot;
        }

        /// <summary>
        /// 6-3. 정렬 (유형 순)
        /// </summary>
        private void GoOrderByType()
        {
            lastSortKey = x => x.DelMatchingCategory;
            lastSortAscending = orderByAscendingOrNot; // lastSortAscending = 이전 정렬 기준
            CommonSortedFunc();
            orderByAscendingOrNot = !orderByAscendingOrNot;
        }

        /// <summary>
        /// 6-4. 정렬 (수정한 날짜 순)
        /// </summary>
        private void OrderByMdTime()
        {
            lastSortKey = x => x.DelMatchingModifiedTime;
            lastSortAscending = orderByAscendingOrNot; // lastSortAscending = 이전 정렬 기준
            CommonSortedFunc();
            orderByAscendingOrNot = !orderByAscendingOrNot;
        }

        /// <summary>
        /// 6-5. 정렬 (크기 순)
        /// </summary>
        private void GoOrderBySize()
        {
            lastSortKey = x => x.DelMatchingOfSize;
            lastSortAscending = orderByAscendingOrNot; // lastSortAscending = 이전 정렬 기준
            CommonSortedFunc();
            orderByAscendingOrNot = !orderByAscendingOrNot;
        }

        /// <summary>
        /// 6-6. 정렬 (경로 순)
        /// </summary>
        private void GoOrderByPath()
        {
            lastSortKey = x => x.DelMatchingPath;
            lastSortAscending = orderByAscendingOrNot; // lastSortAscending = 이전 정렬 기준
            CommonSortedFunc();
            orderByAscendingOrNot = !orderByAscendingOrNot;
        }

        /// <summary>
        /// [페이징 처리] 기능
        /// </summary>
        private void LoadPageData()
        {
            if (LstAllData?.Count > 0)
            {
                List<DelMatchingInfo> filteredData = LstAllData.Skip((CurrentPage - 1) * PageRecords).Take(PageRecords).ToList();
                // 현재 페이지 표시 항목 (X) => 이전 페이지 이동!
                if (filteredData.Count() == 0 && CurrentPage > 1)
                {
                    CurrentPage--; // 해당 페이지 자동 삭제 후, 전 페이지로 이동시킴.
                }
                ActiveFolderInfo = new ObservableCollection<DelMatchingInfo>(filteredData); // [현재 페이지] 초기화 및 갱신
            }
            else
            {
                ActiveFolderInfo?.Clear();
                CurrentPage = 1;
            }

        }

        /// <summary>
        /// 7-1. 다음 페이지로 이동 (버튼)
        /// </summary>
        private void GoToNextPage()
        {
            if (LstAllData?.Count > 0)
            {
                // 방법 2: [ChatGPT]방식
                // [삭제된 항목 수] 계산
                int deleteCount = (selectToDelete?.Count ?? 0) + (entireToDelete?.Count ?? 0);

                // [총 페이지 수] 계산
                int totalPages = (int)Math.Ceiling((double)(LstAllData.Count - deleteCount) / PageRecords);
                if (CurrentPage < totalPages)
                {
                    CurrentPage++;
                    LoadPageData();
                }

                // 방법 1: [MyOwner]방식
                // [삭제된 항목 수] 계산
                // [총 페이지 수] 계산
                //if (selectToDelete?.Count > 0)
                //{
                //    int totalPages = (int)Math.Ceiling((double)(LstAllData.Count - selectToDelete.Count) / PageRecords);
                //    if (CurrentPage < totalPages)
                //    {
                //        CurrentPage++;
                //        LoadPageData();
                //    }

                //}
                //else if (entireToDelete?.Count > 0)
                //{
                //    int totalPages = (int)Math.Ceiling((double)(LstAllData.Count - entireToDelete.Count) / PageRecords);
                //    if (CurrentPage < totalPages)
                //    {
                //        CurrentPage++;
                //        LoadPageData();
                //    }

                //}
                //else
                //{
                //    int totalPages = (int)Math.Ceiling((double)LstAllData.Count / PageRecords);
                //    if (CurrentPage < totalPages)
                //    {
                //        CurrentPage++;
                //        LoadPageData();
                //    }

                //}

            }

        }

        /// <summary>
        /// 7-2. 이전 페이지로 이동 (버튼)
        /// </summary>
        private void GoToPreviousPage()
        {
            if (LstAllData?.Count > 0 && CurrentPage > 1)
            {
                CurrentPage--;
                LoadPageData();
            }

        }

        /// <summary>
        /// 8. 휴지통 복원하기 기능 (버튼)
        /// </summary>
        private async Task RestoreFromRecycleBin()
        {
            try
            {
                TheBtnEnabledOrNot = false;
                // _recycleBinService 사용!
                await _recycleBinService.RestoreDelInfoAsync(LstDelInfo, restoredItem =>
                {
                    // 복원 항목마다 UI 실시간 업데이트 반영
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LstAllData.Add(restoredItem);
                        DeleteFolderInfo.Add(restoredItem);
                        ActiveFolderInfo.Add(restoredItem);
                        // [UI Update] (총 항목 개수)
                        TotalNumbersInfo = LstAllData.Count();
                    });

                });
                CommonSortedFunc(); // 복원 후, 정렬 재적용
            }
            catch (Exception ex)
            {
                Console.WriteLine($"복원 중 오류 발생: {ex.Message}");
            }
            finally
            {
                if (LstDelInfo.Count > 0)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                        _ = MessageBox.Show(mainWindow, "복원이 완료되었습니다.", "복원 완료", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                    LstDelInfo.Clear(); // 삭제(복원) 데이터 정보 초기화
                }
                else
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Window mainWindow = Application.Current.MainWindow; // [MainWindow] 가져오기 (Owner 설정용)
                        _ = MessageBox.Show(mainWindow, "복원할 항목이 존재하지 않습니다.", "복원 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                    });

                }
                TheBtnEnabledOrNot = true;
            }

        }

        #endregion

        #region [캐싱 처리]

        /// <summary>
        /// [폴더 캐싱]
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetEneumerateFldrList()
        {
            // [Cache]에 데이터 존재 여부 확인!
            if (enumerateFldrCache.ContainsKey(DeleteFolderPath))
            {
                return enumerateFldrCache[DeleteFolderPath];
            }

            IEnumerable<string> directories = _enumerateService.GetDirectories(DeleteFolderPath); // _enumerateService 사용

            // 하위 디렉토리 중 요소가 하나라도 존재하면,
            if (directories != null && directories.Any())
            {
                enumerateFldrCache[DeleteFolderPath] = directories; // [Cache]에 데이터가 없으면, 데이터를 새로 저장함!
            }
            return directories ?? Enumerable.Empty<string>();
        }
        #endregion
    }

}
