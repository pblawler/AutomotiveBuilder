using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AutomotiveBuilder.Utils
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal amount)
            {
                return amount.ToString("C2", culture ?? CultureInfo.CurrentCulture);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string input)
            {
                if (decimal.TryParse(input, NumberStyles.Currency, culture ?? CultureInfo.CurrentCulture, out decimal result))
                {
                    return result;
                }
            }
            return System.Windows.Data.Binding.DoNothing; // Or return a default value/handle error
        }
    }

    public class HexToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hexString && !string.IsNullOrEmpty(hexString))
            {
                try
                {
                    if (hexString.StartsWith("http"))
                    {
                        BitmapImage urlimage = new BitmapImage(new Uri(hexString));
                        return urlimage;
                    }
                    // Convert hex string to byte array
                    byte[] imageBytes = HexStringToByteArray(hexString);

                    // Create image from byte array
                    BitmapImage image = new BitmapImage();
                    using (MemoryStream stream = new MemoryStream(imageBytes))
                    {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();
                    }
                    return image;
                }
                catch
                {
                    return null; // Return null or a default image on error
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 1. Read the image file into a byte array
            byte[] imageBytes = File.ReadAllBytes(((BitmapImage)value).UriSource.OriginalString);

            // 2. Convert the byte array to a hexadecimal string
            // The Convert.ToHexString method is available in .NET 5+.
            string hexString =System.Convert.ToHexString(imageBytes);

            // For .NET versions older than 5, you can use BitConverter.ToString:
            // string hexString = BitConverter.ToString(imageBytes).Replace("-", string.Empty);

            return hexString;
        }

        private byte[] HexStringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = System.Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
