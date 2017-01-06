using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App4WithDataBind
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BindToFunctionPage : Page
    {
        public BindToFunctionPage()
        {
            this.InitializeComponent();
            this.hostVM = new HostViewModel();
            //this.DataContext = new ChangeStringViewModel();
            this.ChangeStringVM = new ChangeStringViewModel();
        }

        public HostViewModel hostVM { set; get; }

        public ChangeStringViewModel ChangeStringVM { set; get; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            hostVM.NextText = "Another String!";
            
        }

        //ICommand ClickMeCommand { get; set; }



        //string tList = new IList(tObjectStruct.ToList());

        //ObservableCollection tObjectStruct = new ObservableCollection(tList);
    }


}
