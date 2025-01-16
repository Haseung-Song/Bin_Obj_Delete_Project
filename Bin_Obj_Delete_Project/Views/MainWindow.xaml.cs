using Bin_Obj_Delete_Project.Models;
using Bin_Obj_Delete_Project.ViewModels;
using Bin_Obj_Delete_Project.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bin_Obj_Delete_Project
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainVM vm = new MainVM();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }

        /// <summary>
        /// [Window_Mouse_Down] 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }

        }

        /// <summary>
        /// [BtnMinimize_Click] 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// [BtnMaximize_Click] 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        /// <summary>
        /// [BtnClose_Click] 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// [ListView_MouseDoubleClick] 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            vm.StartContents();
        }

        /// <summary>
        /// [ListView_SelectionChanged]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // [SelectionChanged] 이벤트 발생!
            // [SelectFolderInfo] + [선택 항목] 목록 포함
            foreach (DelMatchingInfo item in e.AddedItems)
            {
                vm.SelectFolderInfo?.Add(item);
                vm.SelectedCntsInfo = vm.SelectFolderInfo.Count;
            }

            // [SelectionChanged] 이벤트 발생!
            // [SelectFolderInfo] + [선택 항목] 목록 제거
            foreach (DelMatchingInfo item in e.RemovedItems)
            {
                if (vm.SelectFolderInfo?.Count > 0)
                {
                    _ = vm.SelectFolderInfo.Remove(item);
                    vm.SelectedCntsInfo = vm.SelectFolderInfo.Count;
                }

            }

        }

        private void OpenFilterWindow(object sender, RoutedEventArgs e)
        {
            // FilterWindowVM 생성 및 초기화
            FilterWindow filterWindow = new FilterWindow(vm)
            {
                Owner = this, // MainWindow 동기화 (완료)
            };

            // [filterWindow] => MainWindow 창(접근 불가)
            if (filterWindow.ShowDialog() == true)
            {
                // 필터 적용 후 MainVM 데이터 갱신
                vm.DeleteFolderPath = ((FilterWindowVM)filterWindow.DataContext).DeleteFolderPath;
            }

        }

    }

}
