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
        public FilterWindow()
        {
            InitializeComponent();
            Loaded += FilterWindow_Closed;
        }

        /// <summary>
        /// [검색 필터] 창 Close() 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterWindow_Closed(object sender, EventArgs e)
        {
            MainVM vm = DataContext as MainVM;
            vm.CloseWindowAction = new Action(() => Close());
        }

        /// <summary>
        /// (Filter 01), (Filter 02) [적용] 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            MainVM vm = DataContext as MainVM;
            vm?.EnterLoadPath();
        }

        /// <summary>
        /// (Filter 01) [초기화] 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void F01ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MainVM vm = DataContext as MainVM;
            // (Filter 01) Reset
            vm?.FilterResetFN();
        }

        /// <summary>
        /// (Filter 02) [초기화] 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void F02ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MainVM vm = DataContext as MainVM;
            // (Filter 02) Reset
            vm?.FilterResetFE();
        }

    }

}
