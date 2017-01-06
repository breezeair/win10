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
            // Data source.
            List<String> itemsList = new List<string>();
            itemsList.Add("Item 1");
            itemsList.Add("Item 2");

            // Create a new flip view, add content, 
            // and add a SelectionChanged event handler.
            FlipView flipView1 = new FlipView();
            flipView1.ItemsSource = itemsList;
            flipView1.SelectionChanged += FlipView_SelectionChanged;

            // Add the flip view to a parent container in the visual tree.
            stackPanel1.Children.Add(flipView1);
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder picturesFolder = KnownFolders.PicturesLibrary;
            //StorageFolder picturesFolder = ApplicationData.Current.LocalFolder;
            //StorageFolder picturesFolder = await StorageFolder.GetFolderFromPathAsync(@"D:\");
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

            myStoryboard.Begin();
        }

        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
            Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;

            //picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
        picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                textBlock2.Text = "Picked photo: " + file.Name;
            }
            else
            {
                textBlock2.Text = "Operation cancelled.";
            }
        }
    }
}
