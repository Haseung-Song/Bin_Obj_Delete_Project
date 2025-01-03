using Bin_Obj_Delete_Project.Common;
using Bin_Obj_Delete_Project.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bin_Obj_Delete_Project.ViewModels
{
    public class FilterWindowVM : MainVM
    {
        #region 생성자 (Initialize)

        public FilterWindowVM(MainVM mainVM)
        {
            FilterFolderName = mainVM.FilterFolderName;
            FilterExtensions = mainVM.FilterExtensions;
            FilterResetFNCommand = new AsyncRelayCommand(FilterResetFN);
            FilterResetFECommand = new AsyncRelayCommand(FilterResetFE);
        }

        #endregion

        #region [ICommand]

        // 5-1. 검색 필터리셋 (FilterFolderName)
        public ICommand FilterResetFNCommand { get; set; }

        // 5-2. 검색 필터리셋 (FilterExtensions)
        public ICommand FilterResetFECommand { get; set; }

        #endregion

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
                        return _explorerService.EnumerateFolders(cancellationToken, ProgressBar, ProgressBar); // [Filter 01] 초기화
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
                        return _explorerService.EnumerateFolders(cancellationToken, ProgressBar, ProgressBar); // [Filter 02] 초기화
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

    }

}
