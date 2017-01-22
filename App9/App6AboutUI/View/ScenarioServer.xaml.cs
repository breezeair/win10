using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Core;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App9Networking.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScenarioServer : Page
    {
        private Page1 rootPage;
        
        private static bool optedIn = false;

        private static string MessageRec = "";

        private StreamSocket connectedSocket = null;

        public ScenarioServer()
        {
            this.InitializeComponent();

            // I want this page to be always cached so that we don't have to add logic to save/restore state for the checkbox.
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = Page1.Current;
        }



        private async void StartListener_Click(object sender, RoutedEventArgs e)
        {
            // Overriding the listener here is safe as it will be deleted once all references to it are gone.
            // However, in many cases this is a dangerous pattern to override data semi-randomly (each time user
            // clicked the button) so we block it here.
            if (CoreApplication.Properties.ContainsKey("listener"))
            {
                rootPage.StatusMessage(
                    "This step has already been executed. Please move to the next one.",
                    Notification.ReadMessage);
                return;
            }

            if (String.IsNullOrEmpty(ServiceNameForListener.Text))
            {
                rootPage.StatusMessage("Please provide a service name.", Notification.ReadMessage);
                return;
            }

            CoreApplication.Properties.Remove("serverAddress");
            CoreApplication.Properties.Remove("adapter");



            StreamSocketListener listener = new StreamSocketListener();
            listener.ConnectionReceived += OnConnection;

            // If necessary, tweak the listener's control options before carrying out the bind operation.
            // These options will be automatically applied to the connected StreamSockets resulting from
            // incoming connections (i.e., those passed as arguments to the ConnectionReceived event handler).
            // Refer to the StreamSocketListenerControl class' MSDN documentation for the full list of control options.
            listener.Control.KeepAlive = false;

            // Save the socket, so subsequent steps can use it.
            CoreApplication.Properties.Add("listener", listener);

            // Start listen operation.
            try
            {
                    await listener.BindServiceNameAsync(ServiceNameForListener.Text);
                    rootPage.StatusMessage("Listening", Notification.StatusMessage);
                
            }
            catch (Exception exception)
            {
                CoreApplication.Properties.Remove("listener");

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

        /// <summary>
        /// Invoked once a connection is accepted by StreamSocketListener.
        /// </summary>
        /// <param name="sender">The listener that accepted the connection.</param>
        /// <param name="args">Parameters associated with the accepted connection.</param>
        private async void OnConnection(
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
                    // Read first 4 bytes (length of the subsequent string).
                    uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint))
                    {
                        // The underlying socket was closed before we were able to read the whole data.
                        return;
                    }

                    // Read the string.
                    uint stringLength = reader.ReadUInt32();
                    uint actualStringLength = await reader.LoadAsync(stringLength);
                    if (stringLength != actualStringLength)
                    {
                        // The underlying socket was closed before we were able to read the whole data.
                        return;
                    }
                    // Display the string on the screen. The event is invoked on a non-UI thread, so we need to marshal
                    // the text back to the UI thread.
                    StatusMessageFromAsyncThread(
                        String.Format("Received Message: \"{0}\"", reader.ReadString(actualStringLength)),
                        Notification.StatusMessage);
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



        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataWriter writer = new DataWriter(connectedSocket.OutputStream);
                writer.WriteBytes(Encoding.UTF8.GetBytes(SendMessageTextBox.Text));
                SendMessageTextBox.Text = "";
                await writer.StoreAsync();
                writer.DetachStream();
                writer.Dispose();
            }
            catch (Exception exception)
            {
                rootPage.StatusMessage(exception.Message, Notification.ReadMessage);
            }
        }


        private void StatusMessageFromAsyncThread(string strMessage, Notification type)
        {
            var ignore = Dispatcher.RunAsync(
                 CoreDispatcherPriority.Normal, () =>
                 rootPage.StatusMessage(strMessage, type));

            //tbMessageReceived.Text = strMessage;
            var ignore2 = Dispatcher.RunAsync(
                 CoreDispatcherPriority.Normal, () =>
                 ReceivingMessage(strMessage));
        }

        private void ReceivingMessage(string strMessage)
        {
            MessageRec += strMessage + "\n";
            tbMessageReceived.Text = MessageRec;
        }

        private void CloseSockets_Click(object sender, RoutedEventArgs e)
        {
            object outValue;
            if (CoreApplication.Properties.TryGetValue("clientDataWriter", out outValue))
            {
                // Remove the data writer from the list of application properties as we are about to close it.
                CoreApplication.Properties.Remove("clientDataWriter");
                DataWriter dataWriter = (DataWriter)outValue;

                // To reuse the socket with another data writer, the application must detach the stream from the
                // current writer before disposing it. This is added for completeness, as this sample closes the socket
                // in the very next block.
                dataWriter.DetachStream();
                dataWriter.Dispose();
            }

            if (CoreApplication.Properties.TryGetValue("clientSocket", out outValue))
            {
                // Remove the socket from the list of application properties as we are about to close it.
                CoreApplication.Properties.Remove("clientSocket");
                StreamSocket socket = (StreamSocket)outValue;

                // StreamSocket.Close() is exposed through the Dispose() method in C#.
                // The call below explicitly closes the socket.
                socket.Dispose();
            }

            if (CoreApplication.Properties.TryGetValue("listener", out outValue))
            {
                // Remove the listener from the list of application properties as we are about to close it.
                CoreApplication.Properties.Remove("listener");
                StreamSocketListener listener = (StreamSocketListener)outValue;

                // StreamSocketListener.Close() is exposed through the Dispose() method in C#.
                // The call below explicitly closes the socket.
                listener.Dispose();
            }

            CoreApplication.Properties.Remove("connected");

            rootPage.StatusMessage("Socket and listener closed", Notification.StatusMessage);
        }
    }
}