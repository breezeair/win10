using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace App9Networking.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FilePage : Page
    {
        private StreamSocket connectedSocket = null;
        private Page1 rootPage;
        private static string MessageRec = "";
        uint Length = 102400;
        private string fileToSend;
        private string fileToGet;
        private HostName hostName;
        private string hostPort;
        private uint bufLength;
        Windows.Storage.StorageFile fileForSending;

        public FilePage()
        {
            this.InitializeComponent();
            // Data source.
            this.NavigationCacheMode = NavigationCacheMode.Required;
            fileToGet = FileName.Text;
            

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = Page1.Current;
        }

        private async void SendFile_Click(object sender, RoutedEventArgs e)
        {

            //Uri uri = new Uri("ms-appx:///assets/22.png");
            //Windows.Storage.StorageFolder storageFolder =
            // KnownFolders.PicturesLibrary;
            //Windows.Storage.StorageFile sampleFile =
            // await storageFolder.GetFileAsync("978.jpg");
            //await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            //var buffer = await Windows.Storage.FileIO.ReadBufferAsync(sampleFile);
            btnSend.IsEnabled = false;
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(fileForSending); 
            bufLength = buffer.Length;
            object outValue;
            StreamSocket connectedSocket;
            if (!CoreApplication.Properties.TryGetValue("fileSocket", out outValue))
            {
                return;
            }
            connectedSocket = (StreamSocket)outValue;
            DataWriter writer = new DataWriter(connectedSocket.OutputStream);
            writer.WriteBuffer(buffer);
            await writer.StoreAsync();
            //writer.DetachStream();
            writer.DetachBuffer();
            writer.Dispose();
            //socket.Dispose();

        }

        private async void Pickfile_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
            Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;

            //picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            fileForSending = await picker.PickSingleFileAsync();
            if (fileForSending != null)
            {
                // Application now has read/write access to the picked file
                FileUri2.Text = "Picked file: " + fileForSending.Path;
                
                btnSend.IsEnabled = true;
            }
            else
            {
                FileUri2.Text = "Operation cancelled.";
                btnSend.IsEnabled = false;

            }
        }

        private async void StartFileListener_Click(object sender, RoutedEventArgs e)
        {
            // Overriding the listener here is safe as it will be deleted once all references to it are gone.
            // However, in many cases this is a dangerous pattern to override data semi-randomly (each time user
            // clicked the button) so we block it here.
            if (CoreApplication.Properties.ContainsKey("filelistener"))
            {
                rootPage.StatusMessage(
                    "This step has already been executed. Please move to the next one.",
                    Notification.ReadMessage);
                return;
            }

            if (String.IsNullOrEmpty(PortForListener.Text))
            {
                rootPage.StatusMessage("Please provide a service name.", Notification.ReadMessage);
                return;
            }
            fileToGet = FileName.Text;
            StreamSocketListener listener = new StreamSocketListener();
            listener.ConnectionReceived += OnConnection2;

            // If necessary, tweak the listener's control options before carrying out the bind operation.
            // These options will be automatically applied to the connected StreamSockets resulting from
            // incoming connections (i.e., those passed as arguments to the ConnectionReceived event handler).
            // Refer to the StreamSocketListenerControl class' MSDN documentation for the full list of control options.
            listener.Control.KeepAlive = false;

            // Save the socket, so subsequent steps can use it.
            CoreApplication.Properties.Add("filelistener", listener);

            // Start listen operation.
            try
            {
                await listener.BindServiceNameAsync(PortForListener.Text);
                StopReceving.IsEnabled = true;
                ReceivingFile.IsEnabled = false;
                rootPage.StatusMessage("Now you can receive files", Notification.StatusMessage);

            }
            catch (Exception exception)
            {
                CoreApplication.Properties.Remove("filelistener");

                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                rootPage.StatusMessage(
                    "Start listening failed with error: " + exception.Message,
                    Notification.ReadMessage);
            }
        }


        private void CloseFileListener_Click(object sender, RoutedEventArgs e)
        {
            object outValue;


            if (CoreApplication.Properties.TryGetValue("filelistener", out outValue))
            {
                // Remove the listener from the list of application properties as we are about to close it.
                CoreApplication.Properties.Remove("filelistener");
                StreamSocketListener listener = (StreamSocketListener)outValue;

                // StreamSocketListener.Close() is exposed through the Dispose() method in C#.
                // The call below explicitly closes the socket.
                listener.Dispose();
            }
            StopReceving.IsEnabled = false;
            ReceivingFile.IsEnabled = true;
            rootPage.StatusMessage("Stop!", Notification.ReadMessage);

        }

        private async void OnConnection2(
    StreamSocketListener sender,
    StreamSocketListenerConnectionReceivedEventArgs args)
        {
            DataReader reader = new DataReader(args.Socket.InputStream);
            if (connectedSocket != null)
            {
                connectedSocket.Dispose();
                connectedSocket = null;
            }
            connectedSocket = args.Socket;

            try
            {
                while (true)
                {
                    uint actualStringLength = await reader.LoadAsync(Length);
                    var buffer = reader.ReadBuffer(actualStringLength);
                    Windows.Storage.StorageFolder storageFolder = KnownFolders.PicturesLibrary;
                    await storageFolder.CreateFileAsync(fileToGet, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                    Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync(fileToGet);
                    await Windows.Storage.FileIO.WriteBufferAsync(sampleFile, buffer);

                }

            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                StatusMessageFromAsyncThread(
                    "Read stream failed with error: " + exception.Message,
                    Notification.ReadMessage);
            }
        }


        private void StatusMessageFromAsyncThread(string strMessage, Notification type)
        {
            var ignore = Dispatcher.RunAsync(
                 CoreDispatcherPriority.Normal, () =>
                 rootPage.StatusMessage(strMessage, type));

        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (CoreApplication.Properties.ContainsKey("clientSocket"))
            {
                rootPage.StatusMessage(
                    "This step has already been executed. Please move to the next one.",
                    Notification.ReadMessage);
                return;
            }

            hostPort = PortForListener.Text;

            // By default 'HostNameForConnect' is disabled and host name validation is not required. When enabling the
            // text box validating the host name is required since it was received from an untrusted source
            // (user input). The host name is validated by catching ArgumentExceptions thrown by the HostName
            // constructor for invalid input.
            HostName hostName;
            try
            {
                hostName = new HostName("10.168.172.148");
            }
            catch (ArgumentException)
            {
                rootPage.StatusMessage("Error: Invalid host name.", Notification.ReadMessage);
                return;
            }

            StreamSocket socket = new StreamSocket();

            // If necessary, tweak the socket's control options before carrying out the connect operation.
            // Refer to the StreamSocketControl class' MSDN documentation for the full list of control options.
            socket.Control.KeepAlive = false;

            // Save the socket, so subsequent steps can use it.
            CoreApplication.Properties.Add("fileSocket", socket);
            try
            {
                rootPage.StatusMessage("Connecting to: " + hostName, Notification.StatusMessage);
                // Connect to the server (by default, the listener we created in the previous step).
                await socket.ConnectAsync(hostName, hostPort);
                rootPage.StatusMessage("Connected", Notification.StatusMessage);
                // Mark the socket as connected. Set the value to null, as we care only about the fact that the 
                // property is set.
                CoreApplication.Properties.Add("connected2", null);
                //SendText.IsEnabled = true;
                //DisConnectSocket.IsEnabled = true;
                //ConnectSocket.IsEnabled = false;
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

        private void btnDisConnect_Click(object sender, RoutedEventArgs e)
        {
            object outValue;
            StreamSocket socket;
            if (!CoreApplication.Properties.TryGetValue("fileSocket", out outValue))
            {
                rootPage.StatusMessage("Please connect first.", Notification.ReadMessage);
                return;
            }
            else
            {
                rootPage.StatusMessage("Yes.close", Notification.ReadMessage);
                socket = (StreamSocket)outValue;
                socket.Dispose();
                CoreApplication.Properties.Remove("fileSocket");
                CoreApplication.Properties.Remove("connected2");

                //SendText.IsEnabled = false;
                //DisConnectSocket.IsEnabled = false;
                //ConnectSocket.IsEnabled = true;
            }
        }
    }
}
