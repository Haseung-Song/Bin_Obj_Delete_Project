using Bin_Obj_Delete_Project.Infrastructure;
using System;
using System.Configuration;
using System.Diagnostics;
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
            // 시작 시, 로그 테이블 생성
            var cs = ConfigurationManager.ConnectionStrings["sqlDB"].ConnectionString;
            SchemaBootstrapper.EnsureActionLogTable(cs);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["SqlDB"].ConnectionString;
                // 종료 시, 로그 테이블 삭제
                SchemaBootstrapper.ClearActionLogTable(cs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OnExit error: " + ex.Message);
            }

        }

    }

}
