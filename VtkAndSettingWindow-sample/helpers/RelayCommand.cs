using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VtkAndSettingWindow_sample.helpers
{
    public class RelayCommand : ICommand
    {
        Action<object> _execteMethod;
        Func<object, bool> _canexecuteMethod;
        private Action doDrrCalculate;

        public RelayCommand(Action doDrrCalculate)
        {
            this.doDrrCalculate = doDrrCalculate;
        }

        public RelayCommand(Action<object> execteMethod, Func<object, bool> canexecuteMethod = null)
        {
            _execteMethod = execteMethod ?? throw new ArgumentNullException("execute");
            _canexecuteMethod = canexecuteMethod;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public bool CanExecute(object? parameter)
        {
            if (_canexecuteMethod != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Execute(object? parameter)
        {
            _execteMethod(parameter);
        }
    }
}
