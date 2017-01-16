using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App9Networking.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += HubPage_Loaded;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //((Frame)Window.Current.Content).Navigate(typeof(View.Page1));
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(View.Page1), true);
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            //((Frame)Window.Current.Content).Navigate(typeof(View.Page1));
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(View.Page2), true);
        }

        void HubPage_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> sections = new List<string>();

            foreach (HubSection section in NewsHub.Sections)
            {
                if (section.Header != null)
                {
                    sections.Add(section.Header.ToString());
                }
            }

            ZoomedOutList.ItemsSource = sections;
        }
    }
}
