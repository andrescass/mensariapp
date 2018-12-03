using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mensajerapp
{
    [System.Windows.Data.ValueConversion(typeof(bool), typeof(String))]
    class stateConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            bool isFinished = (bool)value;
            if (isFinished)
            {
                return "Finalizado";
            }
            else
            {
                return "En curso";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
