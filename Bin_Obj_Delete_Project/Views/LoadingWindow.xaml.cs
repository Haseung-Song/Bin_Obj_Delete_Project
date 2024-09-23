using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace Bin_Obj_Delete_Project.Views
{
    /// <summary>
    /// LoadingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// [로딩 창 열기 (Fade_In)]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Loading_Fade_In(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fade_In = new DoubleAnimation
            {
                // [시작 값] 투명도 (완전 투명)
                From = 0,
                // [끝 값] 투명도 (완전 불투명)
                To = 1,
                // [(fade_In) 지속 시간: 1초]
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };
            // [fade_In] 애니메이션 시작
            BeginAnimation(OpacityProperty, fade_In);
        }

        /// <summary>
        /// [로딩 창 닫기 (Fade_Out)]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Fade_Out(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            DoubleAnimation fade_Out = new DoubleAnimation
            {
                // [시작 값] 투명도 (완전 불투명)
                From = 1,
                // [끝 값] 투명도 (완전 투명)
                To = 0,
                // [(fade_Out) 지속 시간: 1초]
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };
            // [fade_Out] 애니메이션 시작
            BeginAnimation(OpacityProperty, fade_Out);

            // 애니메이션 완료, 창 닫기 이벤트 실행!
            fade_Out.Completed += (s, _) => Close();
        }

    }

}
