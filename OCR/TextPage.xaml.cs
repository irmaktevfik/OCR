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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WindowsPreview.Media.Ocr;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace OCR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TextPage : Page
    {
        OcrEngine oEngine = new OcrEngine(OcrLanguage.English);
        public TextPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                WriteableBitmap img = (WriteableBitmap)e.Parameter;
                LoadData(img);
            }
        }

        async void LoadData(WriteableBitmap img)
        {
            txtBlock.Text = string.Empty;
            OcrResult data = await oEngine.RecognizeAsync((uint)img.PixelHeight, (uint)img.PixelWidth, ConvertBitmapToByteArrayAsync(img));
            foreach (OcrLine item in data.Lines)
            {
                foreach (OcrWord inneritem in item.Words)
                {
                    txtBlock.Text += inneritem.Text;
                }
                txtBlock.Text += Environment.NewLine;
            }
        }

        public static byte[] ConvertBitmapToByteArrayAsync(WriteableBitmap bitmap)
        {
            using (var stream = bitmap.PixelBuffer.AsStream())
            {
                MemoryStream memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
