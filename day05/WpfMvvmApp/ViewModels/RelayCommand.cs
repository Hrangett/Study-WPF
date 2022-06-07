using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfMvvmApp.ViewModels
{
    /// <summary>
    /// ViewMode과 View를 Glue하기위한 클래스
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class RelayCommand<T> : ICommand    
    {
        private readonly Action<T> execute; //실행처리를 위한 Generic
        private readonly Predicate<T> canExecute;   //실행여부를 판단하기위한 Generic


        /// <summary>
        /// execute, canExecute를 파라미터로 받는 생성자
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentException(nameof(execute));
            this.canExecute = canExecute;
        }

        /// <summary>
        /// execute만 파라미터로 받는 생성자
        /// </summary>
        /// <param name="execute"></param>
        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        //실행여부에 따라서 이벤트를 추가해주거나 삭제하는 이벤트 핸들러
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;

            }

        }

        public bool CanExecute(object parameter)
        {
            //특정조건 충족 시 버튼 활성화 하도록 도와주는 메소드
            return canExecute?.Invoke((T)parameter) ?? true;

        }

        public void Execute(object parameter)
        {
            execute((T)parameter);
        }
    }
}
