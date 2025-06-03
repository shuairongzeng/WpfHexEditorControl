using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfHexEditor.Sample.BinaryFilesDifference
{
    public class LongToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long longValue)
            {
                return $"0x{longValue:X}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (stringValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    stringValue = stringValue.Substring(2);
                }
                if (long.TryParse(stringValue, NumberStyles.HexNumber, culture, out long result))
                {
                    return result;
                }
            }
            return value;
        }
    }
}