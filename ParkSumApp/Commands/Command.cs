using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ParkSumApp.Commands
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Func<Task> _asyncAction;
        private bool _isCanExecute;
        public Command(Func<Task> asyncAction)
        {
            _isCanExecute = true;
            _asyncAction = asyncAction;
        }
        public bool CanExecute(object parameter) => _isCanExecute;
        public async void Execute(object parameter)
        {
            if (_asyncAction != null)
            {
                _isCanExecute = false;
                CanExecuteChanged?.Invoke(this, null);
                await _asyncAction.Invoke();
                _isCanExecute = true;
                CanExecuteChanged?.Invoke(this, null);
            }
        }
    }
}
