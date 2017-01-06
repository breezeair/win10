using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace App4WithDataBind
{
    public class HostViewModel : INotifyPropertyChanged
    {
        private string nextText;
        private string lastText;
        private string imagePath= "Assets/22.png";
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public HostViewModel()
        {
            this.NextText = "Next";
            this.LastText = "Last";
        }

        public string NextText
        {
            get
            {
                return this.nextText;
            }
            set
            {
                this.nextText = value;
                this.OnPropertyChanged();
            }
        }

        public string LastText
        {
            get
            {
                return this.lastText;
            }
            set
            {
                this.lastText = value;
                this.OnPropertyChanged();
            }
        }

        public string ImagePath
        {
            get
            {
                return this.imagePath;
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)  //
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
