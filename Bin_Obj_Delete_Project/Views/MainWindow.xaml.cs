using Bin_Obj_Delete_Project.Models;
using Bin_Obj_Delete_Project.ViewModels;
using Bin_Obj_Delete_Project.Views;
using System.Windows;
using System.Windows.Controls;

namespace Bin_Obj_Delete_Project
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainVM vm = new MainVM(); // MainVM() 객체 vm 생성

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // [SelectionChanged] 이벤트 발생!
            // [SelectFolderInfo] + 선택된 항목의 목록 포함
            foreach (DelMatchingInfo item in e.AddedItems)
            {
                vm.SelectFolderInfo?.Add(item);
            }

            // [SelectionChanged] 이벤트 발생!
            // [SelectFolderInfo] + 선택된 항목의 목록을 제거
            foreach (DelMatchingInfo item in e.RemovedItems)
            {
                if (vm.SelectFolderInfo?.Count > 0)
                {
                    _ = vm.SelectFolderInfo.Remove(item);
                }

            }

        }

        private void OpenFilterWindow(object sender, RoutedEventArgs e)
        {
            FilterWindow filterWindow = new FilterWindow
            {
                DataContext = vm
            };
            // [filterWindow] => MainWindow 동기화 (완료)
            filterWindow.Owner = this;

            // [filterWindow] => MainWindow 창(접근 불가)
            _ = filterWindow.ShowDialog();
        }

    }

}
