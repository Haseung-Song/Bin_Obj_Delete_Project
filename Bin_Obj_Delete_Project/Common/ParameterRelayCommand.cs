using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Bin_Obj_Delete_Project.Common
{
    /**
    *  @class  ParameterRelayCommand
    *  @brief  ICommand를 상속 받아  UI에서 이벤트 받아 처리하는 함수 구현
    *  @warning 
    */
    public class ParameterRelayCommand : ICommand
    {
        private readonly Action<object> execute;

        private readonly Predicate<object> canExecute;

        /**
       * @brief  ParameterRelayCommand 생성자 함수.
       * @param Action<object> execute : 실행 이벤트 가 들어왔을때 동작하기 위한 Action함수를 등록
       * @return
       * @exception
       */
        public ParameterRelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /**
         * @brief ParameterRelayCommand 생성자 함수.
         * @param   Action<object> execute : 실행 이벤트 가 들어왔을때 동작하기 위한 Action함수를 등록, Predicate<object> canExecute : Control이 실행 가능 한지 불가능한지 판단하는 함수 등록
         * @return
         * @exception
         */
        public ParameterRelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException("execute");
            this.canExecute = canExecute;
        }

        /**
       * @brief 현재 컨트롤이 실행 가능한지 불가능한지 확인 하는 함수
       * @param   object parameter
       * @return  true : 실행가능 , false : 실행 불가능
       * @exception
       */
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /**
         * @brief 현재 컨트롤이 실행 가능한지 불가능한지 확인 하는 함수
         * @param   object parameter
         * @return  true : 실행가능 , false : 실행 불가능
         * @exception
         */
        public void Execute(object parameter)
        {
            execute(parameter);
        }

        /**
         * @brief CanExecuteChaged 이벤트를 발생시킵니다
         * @param   
         * @return  
         * @exception
         */
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        /**
        * @brief CanExecuteChaged 이벤트를 발생시킵니다
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
