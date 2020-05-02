using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BatchWriter
{    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int ret = 0;
            return int.TryParse((string)value, out ret) ? ret : 0;
        }
    }

    public class MultiTextTrimConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[1] == null) return values[0];

            string val = (string)values[0];
            string rem = (string)values[1];

            int index = val.IndexOf(rem);

            if (index < 0) return val;

            string r = val.Remove(index, rem.Length);
            if (r.StartsWith("\\")) r = r.Remove(0, 1);

            return r;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return new[] { value };
        }
    }
}
