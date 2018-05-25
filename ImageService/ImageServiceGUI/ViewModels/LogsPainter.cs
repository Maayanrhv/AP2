
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ImageService.Logging;
using System.Windows.Media;

namespace ImageServiceGUI.ViewModels
{
    /// <summary>
    /// determines the color of each log. 
    /// </summary>
    public class LogsPainter : IValueConverter
    {
        public LogsPainter() { }
        //
        // Summary:
        //     Converts a value.
        //
        // Parameters:
        //   value:
        //     The value produced by the binding source.
        //
        //   targetType:
        //     The type of the binding target property.
        //
        //   parameter:
        //     The converter parameter to use.
        //
        //   culture:
        //     The culture to use in the converter.
        //
        // Returns:
        //     A converted value. If the method returns null, the valid null value is used.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;
            if (val == MessageTypeEnum.FAIL.ToString())
            {
                return Brushes.Red;

            } else if (val == MessageTypeEnum.INFO.ToString())
            {
                return Brushes.Green;
            } 
             return Brushes.Yellow;
         }

        //
        // Summary:
        //     Converts a value.
        //
        // Parameters:
        //   value:
        //     The value that is produced by the binding target.
        //
        //   targetType:
        //     The type to convert to.
        //
        //   parameter:
        //     The converter parameter to use.
        //
        //   culture:
        //     The culture to use in the converter.
        //
        // Returns:
        //     A converted value. If the method returns null, the valid null value is used.
        public void ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

