using Bin_Obj_Delete_Project.Infrastructure;
using System.Configuration;
using System.Windows;

namespace Bin_Obj_Delete_Project
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var cs = ConfigurationManager.ConnectionStrings["sqlDB"].ConnectionString;
            SchemaBootstrapper.EnsureActionLogTable(cs);

            var main = new MainWindow();
            main.Show();
        }

    }

}
