using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace App4WithDataBind
{
    public class ChangeStringViewModel
    {
        private ICommand changeStringCommand;
        public ICommand ChangeStringCommand
        {
            get
            {
                return changeStringCommand ?? (changeStringCommand = new CommandHandler(() => ChangeString(),true));
            }
        }

        public void ChangeString()
        {
            //((Frame)Window.Current.Content).Navigate(typeof(MainPage), null);
            ((Frame)Window.Current.Content).Navigate(typeof(MainPage), null);
            
        }
    }
}
