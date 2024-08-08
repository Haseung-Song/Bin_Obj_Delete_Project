using Bin_Obj_Delete_Project.Models;
using Bin_Obj_Delete_Project.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Bin_Obj_Delete_Project
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainVM();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainVM vm = DataContext as MainVM;
            // [SelectionChanged] 이벤트 발생!
            // [SelectFolderInfo] + 선택된 항목의 목록 포함
            foreach (DeleteFolderInfo item in e.AddedItems)
            {
                vm.SelectFolderInfo?.Add(item);
            }

            // [SelectionChanged] 이벤트 발생!
            // [SelectFolderInfo] + 선택된 항목의 목록을 제거
            foreach (DeleteFolderInfo item in e.RemovedItems)
            {
                if (vm.SelectFolderInfo?.Count > 0)
                {
                    _ = vm.SelectFolderInfo.Remove(item);
                }

            }

        }

    }

}
