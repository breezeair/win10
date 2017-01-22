using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App9Networking.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page1 : Page
    {
        //set up in configuration cs.
        public const string FEATURE_NAME = "RolesList";
        public static Page1 Current;

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Server Listener", ClassType=typeof(ScenarioServer)},
            new Scenario() { Title="Client", ClassType=typeof(ScenarioClient)},
            new Scenario() { Title="HttpClientPage", ClassType=typeof(HttpClientPage)},
            new Scenario() { Title="BackgroundTransfer", ClassType=typeof(ScenarioBackgroundTransfer)},
            new Scenario() { Title="SocketActivityTrigger", ClassType=typeof(SocketActivityTriggerPage)},
            new Scenario() { Title="WebSockets", ClassType=typeof(WebSocketsPage)},
            new Scenario() { Title="FilePage", ClassType=typeof(FilePage)},

        };
        //set up in configuration cs.

        public Page1()
        {

            this.InitializeComponent();
            Current = this;
            SampleTitle.Text = FEATURE_NAME;
            this.NavigationCacheMode = NavigationCacheMode.Required;

            
        }

        /*private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //Create a new view
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(View.Page2), null);
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);

        }
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ContentDialog noWifiDialog = new ContentDialog()
            {
                Title = "No wifi connection",
                Content = "Check connection and try again",
                PrimaryButtonText = "Ok"
            };

            ContentDialogResult result = await noWifiDialog.ShowAsync();
        }*/
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Populate the scenario list from the SampleConfiguration.cs file
            ScenarioControl.ItemsSource = scenarios;
            if (Window.Current.Bounds.Width < 640)
            {
                ScenarioControl.SelectedIndex = -1;
            }
            else
            {
                ScenarioControl.SelectedIndex = 0;
            }

        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
        }

        async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }

        private void ScenarioControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear the status block when navigating scenarios.

            ListBox scenarioListBox = sender as ListBox;
            Scenario s = scenarioListBox.SelectedItem as Scenario;
            if(scenarioListBox.SelectedIndex == 0)
            {
                StatusMessage("Server", Notification.NoneMessage);
            }
            else
            {
                StatusMessage("Client", Notification.NoneMessage);
            }
            if (s != null)
            {
                ScenarioFrame.Navigate(s.ClassType);
                if (Window.Current.Bounds.Width < 640)
                {
                    Splitter.IsPaneOpen = false;
                }
            }
        }

        public void StatusMessage(string strMessage, Notification type)
        {
            switch (type)
            {
                case Notification.StatusMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.LawnGreen);
                    break;
                case Notification.ReadMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.OrangeRed);
                    break;
                case Notification.NoneMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
                    break;
            }
            StatusBlock.Text = strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            StatusBorder.Visibility = (StatusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
            //if (StatusBlock.Text != String.Empty)
            //{
                StatusBorder.Visibility = Visibility.Visible;
                StatusPanel.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    StatusBorder.Visibility = Visibility.Collapsed;
            //    StatusPanel.Visibility = Visibility.Collapsed;
            //}
        }

        public List<Scenario> Scenarios
        {
            get { return this.scenarios; }
        }
    }

    public enum Notification
    {
        StatusMessage,
        ReadMessage,
        NoneMessage
    };

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }

    public class ScenarioBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Scenario s = value as Scenario;
            return (Page1.Current.Scenarios.IndexOf(s) + 1) + ") " + s.Title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }
}
