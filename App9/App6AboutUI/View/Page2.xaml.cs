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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App6AboutUI.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page2 : Page
    {

        public Page2()
        {
            this.InitializeComponent();
        }
        private void Rectangle_Tapped(object sender, PointerRoutedEventArgs e)
        {
            myStoryboard.Begin();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            //if (rectangleItems.Items.Count > 0)
            //{
            //    rectangleItems.Items.RemoveAt(0);
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(View.FilePage), true);
        }
    }
}
