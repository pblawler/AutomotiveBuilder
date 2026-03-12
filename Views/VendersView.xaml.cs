using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace AutomotiveBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class VendersView : Window
    {
        public VendersView()
        {
            InitializeComponent();
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string url = PartURL.Text;

            if ((url == null) || (url == "")) return;
            // For .NET Core, .NET 5+, .NET 6+, etc.
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);

        }

        private void LabelVU_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string url = VenderUrl.Text;

            if ((url == null) || (url == "")) return;
            // For .NET Core, .NET 5+, .NET 6+, etc.
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);

        }

        private void Image_Drop(object sender, System.Windows.DragEventArgs e)
        {
            // 1. Handling Windows Explorer (FileDrop)
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                PartImg.Source = new BitmapImage(new Uri(files[0]));
            }
            // 2. Handling Web Browsers (Images from Chrome/Edge)
            else if (e.Data.GetDataPresent(System.Windows.DataFormats.Bitmap))
            {
                // Some browsers provide the actual Bitmap object
                PartImg.Source = (BitmapSource)e.Data.GetData(System.Windows.DataFormats.Bitmap);
            }
            // 3. Handling Web Browsers (HTML/URL)
            else if (e.Data.GetDataPresent(System.Windows.DataFormats.Text))
            {
                string url = (string)e.Data.GetData(System.Windows.DataFormats.Text);
                if (url.StartsWith("http"))
                {
                    PartImg.Source = new BitmapImage(new Uri(url));
                }
            }

            e.Handled = true; // Mark the event as handled
        }

        private void Image_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            // Check if the dragged data contains file paths
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                // Set the effect to Copy, indicating a drop is allowed
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(System.Windows.DataFormats.Bitmap))
            {
                // Also allow if the data is a bitmap directly (e.g. from a web browser)
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                // Otherwise, disable the drop
                e.Effects = System.Windows.DragDropEffects.None;
            }
            e.Handled = true; // Mark the event as handled
        }
    }
}