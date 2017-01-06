using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace App4WithDataBind
{
    class CommandHandler2 : ICommand
    {
        private Action _action;
        private bool _canExcute;

        public CommandHandler2(Action action, bool canExcute)
        {
            _action = action;
            _canExcute = canExcute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            //throw new NotImplementedException();
            return _canExcute;
        }

        public void Execute(object parameter)
        {
            //throw new NotImplementedException();
            _action();
        }
    }
}
