using PhoneSync.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PhoneSync.ViewModels
{
    public class TransferStatusToSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (TransferStatus)value;
            switch (v)
            {
                case TransferStatus.Transferred:
                    return new BitmapImage(new Uri("pack://application:,,,/transferred.png"));
                case TransferStatus.Exists:
                    return new BitmapImage(new Uri("pack://application:,,,/exists.png"));
                default:
                    return new BitmapImage(new Uri("pack://application:,,,/ignored.png")); ;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TransferStatus.Transferred;
        }
    }

    public class ImageFromEnum : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (TransferStatus)values[0];
            switch (status)
            {
                case TransferStatus.Ignored:
                    return values[1];
                case TransferStatus.Transferred:
                    return values[2];
                default:
                    return values[3];
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
