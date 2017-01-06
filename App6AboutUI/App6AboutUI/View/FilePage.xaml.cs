using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class FilePage : Page
    {
        public FilePage()
        {
            this.InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder picturesFolder = KnownFolders.PicturesLibrary;
            StringBuilder outputText = new StringBuilder();

            IReadOnlyList<StorageFile> fileList =
                await picturesFolder.GetFilesAsync();

            outputText.AppendLine("Files:");
            foreach (StorageFile file in fileList)
            {
                outputText.Append(file.Name + "\n");
            }

            IReadOnlyList<StorageFolder> folderList =
                await picturesFolder.GetFoldersAsync();

            outputText.AppendLine("Folders:");
            foreach (StorageFolder folder in folderList)
            {
                outputText.Append(folder.DisplayName + "\n");
            }
            string ss = outputText.ToString();
            textBlock1.Text = ss;
        }
    }
}
