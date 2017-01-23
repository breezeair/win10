using System;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace App9Networking.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScenarioClient : Page
    {
        Page1 rootPage = Page1.Current;
        public const string logoSecondaryTileId = "SecondaryTile.Logo";

        private const int bytesPerRow = 16;
        private const int bytesPerSegment = 2;
        private const uint chunkSize = 4096;

        // Limit traffic to the same adapter that the listener is using to demonstrate client adapter-binding.
        NetworkAdapter adapter = null;
        public ScenarioClient()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HostNameForConnect.Text = "10.168.172.148";
            //HostNameForConnect.Text = "10.168.196.35";

            bool optedIn = false;
            if (e.Parameter != null)
            {
                if ((bool)e.Parameter)
                {
                    optedIn = true;

                }
            }

            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack && optedIn)
            {
                // If we have pages in our in-app backstack and have opted in to showing back, do so
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if there are no pages in our in-app back stack
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        private async void ConnectSocket_Click(object sender, RoutedEventArgs e)
        {
            if (CoreApplication.Properties.ContainsKey("clientSocket"))
            {
                rootPage.StatusMessage(
                    "This step has already been executed. Please move to the next one.",
                    Notification.ReadMessage);
                return;
            }

            if (String.IsNullOrEmpty(ServiceNameForConnect.Text))
            {
                rootPage.StatusMessage("Please provide a service name.", Notification.ReadMessage);
                return;
            }

            HostName hostName;
            try
            {
                hostName = new HostName(HostNameForConnect.Text);
            }
            catch (ArgumentException)
            {
                rootPage.StatusMessage("Error: Invalid host name.", Notification.ReadMessage);
                return;
            }

            StreamSocket socket = new StreamSocket();

            socket.Control.KeepAlive = false;

            // Save the socket, so subsequent steps can use it.
            CoreApplication.Properties.Add("clientSocket", socket);
            try
            {
                    rootPage.StatusMessage("Connecting to: " + HostNameForConnect.Text, Notification.StatusMessage);
                    // Connect to the server (by default, the listener we created in the previous step).
                    await socket.ConnectAsync(hostName, ServiceNameForConnect.Text);
                    rootPage.StatusMessage("Connected", Notification.StatusMessage);
                // Mark the socket as connected. Set the value to null, as we care only about the fact that the 
                // property is set.
                CoreApplication.Properties.Add("connected", null);
                SendText.IsEnabled = true;
                DisConnectSocket.IsEnabled = true;
                ConnectSocket.IsEnabled = false;
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    //throw;
                }

                rootPage.StatusMessage("Connect failed with error: " + exception.Message, Notification.ReadMessage);
            }
        }

        private async void SendText_Click(object sender, RoutedEventArgs e)
        {
            if (!CoreApplication.Properties.ContainsKey("connected"))
            {
                rootPage.StatusMessage("Please run previous steps before doing this one.", Notification.ReadMessage);
                return;
            }

            object outValue;
            StreamSocket socket;
            if (!CoreApplication.Properties.TryGetValue("clientSocket", out outValue))
            {
                rootPage.StatusMessage("Please run previous steps before doing this one.", Notification.ReadMessage);
                return;
            }

            socket = (StreamSocket)outValue;

            // Create a DataWriter if we did not create one yet. Otherwise use one that is already cached.
            DataWriter writer;
            if (!CoreApplication.Properties.TryGetValue("clientDataWriter", out outValue))
            {
                writer = new DataWriter(socket.OutputStream);
                CoreApplication.Properties.Add("clientDataWriter", writer);
            }
            else
            {
                writer = (DataWriter)outValue;
            }

            // Write first the length of the string as UINT32 value followed up by the string. 
            // Writing data to the writer will just store data in memory.
            string stringToSend = "Hello";
            if(tbForSending.Text != null)
            {
                stringToSend = tbForSending.Text;
            }
            writer.WriteUInt32(writer.MeasureString(stringToSend));
            writer.WriteString(stringToSend);

            // Write the locally buffered data to the network.
            try
            {
                await writer.StoreAsync();
                SendOutput.Text = "\"" + stringToSend + "\" sent successfully.";
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error if fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                rootPage.StatusMessage("Send failed with error: " + exception.Message, Notification.ReadMessage);
            }
        }

        private async void HexDump(object sender, RoutedEventArgs e)
        {
            try
            {
                // Retrieve the uri of the image and use that to load the file.
                Uri uri = new Uri("ms-appx:///assets/22.png");
                var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
                //DataReader reader = new DataReader(args.Socket.InputStream);
                using (var inputStream = await file.OpenSequentialReadAsync())
                {
                    // Pass the input stream to the DataReader.
                    var dataReader = new Windows.Storage.Streams.DataReader(inputStream) ;                 
                        uint numBytes;
                        //var bytes = new byte[chunkSize];
                        numBytes = await dataReader.LoadAsync(chunkSize);
                    //numBytes = await dataReader.LoadAsync(sizeof(byte));
                    //dataReader.ReadBytes(bytes);
                    //var buffer = dataReader.ReadBytes(chunkSize);
                    //dataReader.ReadBuffer(inputStream);




                    
                }
            
            object outValue;
            StreamSocket connectedSocket;
            if (!CoreApplication.Properties.TryGetValue("clientSocket", out outValue))
            {
                rootPage.StatusMessage("Please run previous steps before doing this one.", Notification.ReadMessage);
                return;
            }
            connectedSocket = (StreamSocket)outValue;

            DataWriter writer = new DataWriter(connectedSocket.OutputStream);
            writer.WriteBytes(System.Text.Encoding.UTF8.GetBytes("SSSSSS"));
            await writer.StoreAsync();
            writer.DetachStream();
            writer.Dispose();
            }
            catch (Exception ex)
            {
                //ReadBytesOutput.Text = ex.Message;
            }
        }


        public static Rect GetElementRect(FrameworkElement element)
        {
            Windows.UI.Xaml.Media.GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }
        private async void PinButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                // Prepare package images for all four tile sizes in our tile to be pinned as well as for the square30x30 logo used in the Apps view.  
                Uri square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
                //Uri wide310x150Logo = new Uri("ms-appx:///Assets/wide310x150Tile-sdk.png");
                //Uri square310x310Logo = new Uri("ms-appx:///Assets/square310x310Tile-sdk.png");
                //Uri square30x30Logo = new Uri("ms-appx:///Assets/square30x30Tile-sdk.png");

                // During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
                // These arguments should be meaningful to the application. In this sample, we'll pass in the date and time the secondary tile was pinned.
                string tileActivationArguments = logoSecondaryTileId + " WasPinnedAt=" + DateTime.Now.ToLocalTime().ToString();

                // Create a Secondary tile with all the required arguments.
                // Note the last argument specifies what size the Secondary tile should show up as by default in the Pin to start fly out.
                // It can be set to TileSize.Square150x150, TileSize.Wide310x150, or TileSize.Default.  
                // If set to TileSize.Wide310x150, then the asset for the wide size must be supplied as well.
                // TileSize.Default will default to the wide size if a wide size is provided, and to the medium size otherwise. 
                SecondaryTile secondaryTile = new SecondaryTile(logoSecondaryTileId,
                                                                "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                                                                tileActivationArguments,
                                                                square150x150Logo,
                                                                TileSize.Square150x150);

                if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent(("Windows.Phone.UI.Input.HardwareButtons"))))
                {
                   // secondaryTile.VisualElements.Wide310x150Logo = wide310x150Logo;
                    //secondaryTile.VisualElements.Square310x310Logo = square310x310Logo;
                }

                // Like the background color, the square30x30 logo is inherited from the parent application tile by default. 
                // Let's override it, just to see how that's done.
                //secondaryTile.VisualElements.Square30x30Logo = square30x30Logo;

                // The display of the secondary tile name can be controlled for each tile size.
                // The default is false.
                secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

                if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent(("Windows.Phone.UI.Input.HardwareButtons"))))
                {
                    secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;
                    secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = true;
                }

                // Specify a foreground text value.
                // The tile background color is inherited from the parent unless a separate value is specified.
                secondaryTile.VisualElements.ForegroundText = ForegroundText.Light;

                // Set this to false if roaming doesn't make sense for the secondary tile.
                // The default is true;
                secondaryTile.RoamingEnabled = false;

                // OK, the tile is created and we can now attempt to pin the tile.
                if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent(("Windows.Phone.UI.Input.HardwareButtons"))))
                {
                    // Note that the status message is updated when the async operation to pin the tile completes.
                    bool isPinned = await secondaryTile.RequestCreateForSelectionAsync(GetElementRect((FrameworkElement)sender), Windows.UI.Popups.Placement.Below);

                    if (isPinned)
                    {
                        rootPage.StatusMessage("Secondary tile successfully pinned.", Notification.StatusMessage);
                    }
                    else
                    {
                        rootPage.StatusMessage("Secondary tile not pinned.", Notification.ReadMessage);
                    }
                }
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent(("Windows.Phone.UI.Input.HardwareButtons")))
                {
                    // OK, the tile is created and we can now attempt to pin the tile.
                    // Since pinning a secondary tile on Windows Phone will exit the app and take you to the start screen, any code after 
                    // RequestCreateForSelectionAsync or RequestCreateAsync is not guaranteed to run.  For an example of how to use the OnSuspending event to do
                    // work after RequestCreateForSelectionAsync or RequestCreateAsync returns, see Scenario9_PinTileAndUpdateOnSuspend in the SecondaryTiles.WindowsPhone project.
                    await secondaryTile.RequestCreateAsync();
                }
            }
        }

        private void DisConnect_Click(object sender, RoutedEventArgs e)
        {
            object outValue;
            StreamSocket socket;
            if (!CoreApplication.Properties.TryGetValue("clientSocket", out outValue))
            {
                rootPage.StatusMessage("Please connect first.", Notification.ReadMessage);
                return;
            }else
            {
                rootPage.StatusMessage("Yes.",Notification.ReadMessage);
                socket = (StreamSocket)outValue;
                socket.Dispose();
                CoreApplication.Properties.Remove("clientSocket");
                CoreApplication.Properties.Remove("connected");
                CoreApplication.Properties.Remove("clientDataWriter");

                SendText.IsEnabled = false;
                DisConnectSocket.IsEnabled = false;
                ConnectSocket.IsEnabled = true;
            }


        }
    }
}
