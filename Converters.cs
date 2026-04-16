using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

public static class ColorExtensions
{
    // Extension method to mix two colors
    public static Color MixWith(this Color color1, Color color2, double factor)
    {
        // Average the A, R, G, and B components
        byte a = (byte)(color1.A * (1.0 - factor) + color2.A * factor);
        byte r = (byte)(color1.R * (1.0 - factor) + color2.R * factor);
        byte g = (byte)(color1.G * (1.0 - factor) + color2.G * factor);
        byte b = (byte)(color1.B * (1.0 - factor) + color2.B * factor);

        // Return the new mixed color
        return Color.FromArgb(a, r, g, b);
    }
        public static Color GreenColor = Color.FromRgb(0x33, 0xCC, 0x00);
        public static Color WhiteColor = Color.FromRgb(0xFF, 0xFF, 0xFF);
        public static Color RedColor = Color.FromRgb(0xE1, 0x25, 0x1B);
}

namespace User.CornerSpeed
{
    public class ComparisonModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ComparisonMode cm)
            {
                return cm.ToDescriptionString();
            }
            return "vs Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class SpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = System.Convert.ToDouble(value);
            string valString = val.ToString();
            string finalString = string.Empty;
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            //nfi.NumberGroupSeparator = " ";
            finalString = val.ToString("#,0.00", nfi); // "1 234 897.11"
            //finalString = finalString.Replace(".", ",");//"1 234 897,11"

            return finalString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class SpeedDeltaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = System.Convert.ToDouble(value);
            string valString = val.ToString();
            string finalString = string.Empty;
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            //nfi.NumberGroupSeparator = " ";
            finalString = val.ToString("#,0.00", nfi); // "1 234 897.11"
            //finalString = finalString.Replace(".", ",");//"1 234 897,11"
            if (val >= 0)
            {
                finalString = "+" + finalString;
            }

            return finalString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MeterDeltaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = System.Convert.ToDouble(value);
            string valString = val.ToString();
            string finalString = string.Empty;
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            //nfi.NumberGroupSeparator = " ";
            finalString = val.ToString("0.0", nfi); // "1 234 897.11"
            //finalString = finalString.Replace(".", ",");//"1 234 897,11"
            if (val >= 0)
            {
                finalString = "+" + finalString;
            }

            return finalString + "m";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeSpanFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                string positiveFormat = timeSpan.Negate().ToString(@"s\.fff");
                if (timeSpan.TotalMinutes > 1.0) { 
                    positiveFormat = timeSpan.Negate().ToString(@"m\:ss\.fff");
                }
                // Check if the TimeSpan is negative
                if (timeSpan < TimeSpan.Zero)
                {
                    // Format as positive and prepend the minus sign manually
                    // The "g" standard format specifier can work well, or a custom format
                    return "-" + positiveFormat;
                }
                return positiveFormat;
            }
            return value; // Or return a default value/error indicator
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    

    public class ValueToInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double dvalue)
            {
                return -dvalue;
            }
            return value; // Or return a default value/error indicator
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DeltaBarColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                doubleValue *= 30;
                if (doubleValue > 0)
                {
                    return new SolidColorBrush(ColorExtensions.WhiteColor.MixWith(ColorExtensions.RedColor, Math.Min(doubleValue, 1.0)));
                }
                else
                {
                    return new SolidColorBrush(ColorExtensions.WhiteColor.MixWith(ColorExtensions.GreenColor, Math.Min(-doubleValue, 1.0)));
                }
            }
            // Return a default color or null if the value is not an integer
            return new SolidColorBrush(ColorExtensions.WhiteColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeSpanDeltaFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                string positiveFormat = timeSpan.Negate().ToString(@"s\.fff");
                // Check if the TimeSpan is negative
                if (timeSpan < TimeSpan.Zero)
                {
                    // Format as positive and prepend the minus sign manually
                    // The "g" standard format specifier can work well, or a custom format
                    return "-" + positiveFormat;
                }
                return "+" + positiveFormat;
            }
            return value; // Or return a default value/error indicator
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LapTimeDeltaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                var timeSpan = TimeSpan.FromSeconds(d);
                string positiveFormat = timeSpan.Negate().ToString(@"s\.fff");
                // Check if the TimeSpan is negative
                if (timeSpan < TimeSpan.Zero)
                {
                    // Format as positive and prepend the minus sign manually
                    // The "g" standard format specifier can work well, or a custom format
                    return "-" + positiveFormat;
                }
                return "+" + positiveFormat;
            }
            return value; // Or return a default value/error indicator
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DeltaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                if (doubleValue > 0)
                {
                    return new SolidColorBrush(ColorExtensions.WhiteColor.MixWith(ColorExtensions.GreenColor, Math.Min(doubleValue, 1.0)));
                }
                else
                {
                    return new SolidColorBrush(ColorExtensions.WhiteColor.MixWith(ColorExtensions.RedColor, Math.Min(-doubleValue, 1.0)));
                }
            }
            // Return a default color or null if the value is not an integer
            return new SolidColorBrush(ColorExtensions.WhiteColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DurationDeltaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                var doubleValue = timeSpan.TotalSeconds / 0.1;
                if (doubleValue > 0)
                {
                    return new SolidColorBrush(ColorExtensions.WhiteColor.MixWith(ColorExtensions.RedColor, Math.Min(doubleValue, 1.0)));
                }
                else
                {
                    return new SolidColorBrush(ColorExtensions.WhiteColor.MixWith(ColorExtensions.GreenColor, Math.Min(-doubleValue, 1.0)));
                }
            }
            // Return a default color or null if the value is not an integer
            return new SolidColorBrush(ColorExtensions.WhiteColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
