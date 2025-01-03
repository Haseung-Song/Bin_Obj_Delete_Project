using Bin_Obj_Delete_Project.Common;
using Bin_Obj_Delete_Project.Models;
using Bin_Obj_Delete_Project.Services;
using Bin_Obj_Delete_Project.Views;
using Microsoft.VisualBasic.FileIO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SearchOption = System.IO.SearchOption;

namespace Bin_Obj_Delete_Project.ViewModels
{
    public class MainVM : CommonProperty
    {
        #region [프로퍼티]

        public readonly EnumerateService _explorerService;

        public readonly DeleteService _deleteService;

        /// <summary>
        /// [_deleteFolderPath]
        /// </summary>
        protected string _deleteFolderPath;

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
        /// [ascendingOrDescending]
        /// </summary>
        private bool orderByAscendingOrNot;

        /// <summary>
        /// [mouseHook]
        /// </summary>
        public static GlobalMouseHook mouseHook;

        /// <summary>
        /// [enumerateFldrCache]
        /// </summary>
        private readonly Dictionary<string, IEnumerable<string>> enumerateFldrCache;

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

        #endregion

        #region [Action]

        public Action CloseWindowAction { get; set; }

        #endregion

        #region [OnPropertyChanged]

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

        #endregion

        #region 생성자 (Initialize)

        public MainVM()
        {
            _explorerService = new EnumerateService(this);
            _deleteService = new DeleteService(this);
            enumerateFldrCache = new Dictionary<string, IEnumerable<string>>();
            TheBtnEnabledOrNot = true;
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
            orderByAscendingOrNot = true;
            mouseHook = new GlobalMouseHook();
            DelSelMatchesCommand = new AsyncRelayCommand(DelSelMatches);
            DelAllMatchesCommand = new AsyncRelayCommand(DelAllMatches);
            GoOrderByNameCommand = new RelayCommand(GoOrderByName);
            OrderByCrTimeCommand = new RelayCommand(OrderByCrTime);
            GoOrderByTypeCommand = new RelayCommand(GoOrderByType);
            OrderByMdTimeCommand = new RelayCommand(OrderByMdTime);
            GoOrderBySizeCommand = new RelayCommand(GoOrderBySize);
            GoOrderByPathCommand = new RelayCommand(GoOrderByPath);
            GoToNextPageCommand = new RelayCommand(GoToNextPage);
            GoToPreviousPageCommand = new RelayCommand(GoToPreviousPage);
        }

        public MainVM(EnumerateService explorerService)
        {
            _explorerService = explorerService;
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
            TheBtnEnabledOrNot = false;
            VisibleLoading = true;
            Task enumerateTask = Task.Run(() =>
            {
                return _explorerService.EnumerateFolders(cancellationToken, ProgressBar, ProgressBar); // 비동기 호출 반환
            }, cancellationToken);
            try
            {
                Task cancelingTask = Task.Delay(180000); // [약 180초] 후, 작업 취소!!
                Task completedTask = await Task.WhenAny(enumerateTask, cancelingTask);
                // [약 3분]이 지나도 작업이 끝나지 않을 때, 작업 취소 요청!
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
                    });
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
        /// [하위 디렉토리] 파일의 총량 계산 (기능)
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static long GetDirectorySize(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir); // DirectoryInfo 객체 생성
            long sizeofDir = 0; // [총량] 초기화
            try
            {
                // 병렬 옵션 설정
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

                // 병렬 루프 제공
                _ = Parallel.ForEach(dirInfo.GetFiles("*", SearchOption.AllDirectories), options, (files) =>
                  {
                      try
                      {
                          _ = Interlocked.Add(ref sizeofDir, files.Length); // 각 파일의 [크기 계산 및 누적]
                      }
                      catch
                      {
                          /* 파일 접근 오류 무시 */
                      }

                  });

            }
            catch
            {
                /* 폴더 접근 오류 무시 */
            }
            return sizeofDir; // 누적된 파일 크기 총합(sizeofDir) 반환
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
        /// 3. [선택 삭제하기] 기능 (버튼)
        /// </summary>
        private async Task DelSelMatches()
        {
            bool shouldDelete = false;
            if (!string.IsNullOrEmpty(DeleteFolderPath) && SelectFolderInfo?.Count > 0)
            {
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
                    await _deleteService.DelSelConfirm(ProgressBar);
                    // UI Update (총 항목 개수)
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        TotalNumbersInfo = LstAllData.Count();
                    });

                }
                ResetUIState(); // UI 상태 초기화
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
                entireToDelete = new List<DelMatchingInfo>(LstAllData);
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
                    await _deleteService.DelAllConfirm(ProgressBar);
                    // UI Update (총 항목 개수)
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        TotalNumbersInfo = LstAllData.Count();
                    });

                }
                ResetUIState(); // UI 상태 초기화
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
            LstAllData = orderByAscendingOrNot
                ? LstAllData.OrderBy(item => item.DelMatchingName).ToList()
                : LstAllData.OrderByDescending(item => item.DelMatchingName).ToList();

            // 1) 정렬 후, 페이지 초기화 작업 (갱신)
            LoadPageData();

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
            LstAllData = orderByAscendingOrNot
                ? LstAllData.OrderBy(item => item.DelMatchingCreationTime).ToList()
                : LstAllData.OrderByDescending(item => item.DelMatchingCreationTime).ToList();

            // 1) 정렬 후, 페이지 초기화 작업 (갱신)
            LoadPageData();

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
            LstAllData = orderByAscendingOrNot
                ? LstAllData.OrderBy(item => item.DelMatchingCategory).ToList()
                : LstAllData.OrderByDescending(item => item.DelMatchingCategory).ToList();

            // 1) 정렬 후, 페이지 초기화 작업 (갱신)
            LoadPageData();

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
            LstAllData = orderByAscendingOrNot
                ? LstAllData.OrderBy(item => item.DelMatchingModifiedTime).ToList()
                : LstAllData.OrderByDescending(item => item.DelMatchingModifiedTime).ToList();

            // 1) 정렬 후, 페이지 초기화 작업 (갱신)
            LoadPageData();

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
            LstAllData = orderByAscendingOrNot
                ? LstAllData.OrderBy(item => item.DelMatchingOfSize).ToList()
                : LstAllData.OrderByDescending(item => item.DelMatchingOfSize).ToList();

            // 1) 정렬 후, 페이지 초기화 작업 (갱신)
            LoadPageData();

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
            LstAllData = orderByAscendingOrNot
                ? LstAllData.OrderBy(item => item.DelMatchingPath).ToList()
                : LstAllData.OrderByDescending(item => item.DelMatchingPath).ToList();

            // 1) 정렬 후, 페이지 초기화 작업 (갱신)
            LoadPageData();

            // 2) 플래그(flag) 값, 반전시키기
            orderByAscendingOrNot = !orderByAscendingOrNot;
        }


        /// <summary>
        /// [페이징 처리] 기능
        /// </summary>
        public void LoadPageData()
        {
            if (LstAllData?.Count > 0)
            {
                if (CurrentPage < 1)
                {
                    CurrentPage = 1;
                }
                List<DelMatchingInfo> filteredData = LstAllData.Skip((CurrentPage - 1) * PageRecords).Take(PageRecords).ToList();
                ActiveFolderInfo = new ObservableCollection<DelMatchingInfo>(filteredData); // [현재 페이지] 초기화 및 갱신
            }
            else
            {
                // 데이터가 없을 때 초기화
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
