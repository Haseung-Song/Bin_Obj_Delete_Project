using Bin_Obj_Delete_Project.ViewModels;
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
            DataContext = new FilterWindowVM();
        }

    }

}
