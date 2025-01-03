using Bin_Obj_Delete_Project.ViewModels;
using System;
using System.Windows;

namespace Bin_Obj_Delete_Project.Views
{
    /// <summary>
    /// FilterWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FilterWindow : Window
    {
        public FilterWindow(MainVM mainVM)
        {
            InitializeComponent();
            DataContext = new FilterWindowVM(mainVM);
            Loaded += FilterWindow_Closed;
        }

        /// <summary>
        /// [검색 필터] 창 Close() 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterWindow_Closed(object sender, EventArgs e)
        {
            if (DataContext is FilterWindowVM vm)
            {
                vm.CloseWindowAction = new Action(() => Close());
            }
        }

        /// <summary>
        /// (Filter 01), (Filter 02) [적용] 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FilterWindowVM vm)
            {
                vm.EnterLoadPath(); // FilterWindowVM의 로직 호출
            }

        }

    }

}
