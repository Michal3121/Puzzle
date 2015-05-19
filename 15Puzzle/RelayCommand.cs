using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace _15Puzzle
{
    class RelayCommand : ICommand
    {
        Action onExecute;
        Func<bool> canExecute;

        public RelayCommand(Action onExecute, Func<bool> canExecute)
        {
            this.onExecute = onExecute;
            this.canExecute = canExecute;
        }

        public RelayCommand(Action onExecute)
            : this(onExecute, () => true)
        {
        }

        public bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            onExecute();
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
