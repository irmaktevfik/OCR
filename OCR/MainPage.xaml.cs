using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace OCR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IFileOpenPickerContinuable
    {
        public static MainPage Current;
        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void OpenFileBarButton_Click(object sender, RoutedEventArgs e)
        {
            //please note that PickSingleFileAsync not implemented for Windows Phone 8.1 ref http://msdn.microsoft.com/en-us/library/dn614994.aspx
            //need to implement continuation manager http://code.msdn.microsoft.com/windowsapps/File-picker-sample-9f294cba
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.PickSingleFileAndContinue();
        }

        private void ShowTextButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TextPage), bitmapImage);
        }

        private WriteableBitmap bitmapImage;
        private void CommandInvokedHandler(IUICommand command)
        {

        }

        public async void ContinueFileOpenPicker(Windows.ApplicationModel.Activation.FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count > 0)
            {
                //get the file
                var file = args.Files[0];
                //get the scaled image 
                var props = await file.GetScaledImageAsThumbnailAsync(ThumbnailMode.PicturesView);

                //read the image data 
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    //create the bitmap with the image properties
                    bitmapImage = new WriteableBitmap((int)props.OriginalWidth, (int)props.OriginalHeight);
                    await bitmapImage.SetSourceAsync(stream);

                    //set the source of the image in Homepage.xaml
                    image.Source = bitmapImage;
                }

                //set the navigation button visible
                ShowTextButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                MessageDialog dlg = new MessageDialog("File does not exists!!");
                dlg.Commands.Add(new UICommand("Close", new UICommandInvokedHandler(this.CommandInvokedHandler)));
            }
        }
    }
}
