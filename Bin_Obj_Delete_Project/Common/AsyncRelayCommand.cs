using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bin_Obj_Delete_Project.Common
{
    public class AsyncRelayCommand : ICommand
    {
        // [비동기 실행] 메서드
        private readonly Func<Task> executeAsync;

        private readonly Func<bool> canExecute;

        /**
         * @brief AsyncRelayCommand 생성자 함수.
         * @param Func<Task> executeAsync : 실행 이벤트가 들어왔을 때 동작하기 위한 비동기 메서드 등록
         * @return
         * @exception
         */
        public AsyncRelayCommand(Func<Task> executeAsync)
            : this(executeAsync, null)
        {
        }

        /**
         * @brief AsyncRelayCommand 생성자 함수.
         * @param Func<Task> executeAsync : 실행 이벤트가 들어왔을 때 동작하기 위한 비동기 메서드 등록,
         * Func<bool> canExecute : Control이 실행 가능 여부를 판단하는 함수 등록
         * @return
         * @exception
         */
        public AsyncRelayCommand(Func<Task> executeAsync, Func<bool> canExecute)
        {
            this.executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /**
         * @brief 현재 컨트롤이 실행 가능한지 불가능한지 확인하는 함수
         * @param object parameter
         * @return true: 실행 가능, false: 실행 불가능
         */
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }

        /**
         * @brief [비동기 메서드] 실행
         * @param object parameter
         * @return
         */
        public async void Execute(object parameter)
        {
            await ExecuteAsync();
        }

        /**
         * @brief 비동기 메서드를 직접 호출
         * @return Task
         */
        public async Task ExecuteAsync()
        {
            await executeAsync();
        }

        /**
         * @brief CanExecuteChanged 이벤트를 발생시킵니다
         * @param
         * @return
         * @exception
         */
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        /**
         * @brief CanExecuteChanged 이벤트를 발생시킵니다
         * @param
         * @return
         * @exception
         */
        protected virtual void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

    }

}
