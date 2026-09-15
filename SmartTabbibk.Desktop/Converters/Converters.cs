using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartTabbibk.Desktop.Converters
{
    /// <summary>يحوّل نص فاضي/null إلى Collapsed، وأي نص فيه محتوى إلى Visible</summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
            string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }

    /// <summary>يحوّل bool إلى Visibility (true = Visible)</summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
            (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }

    /// <summary>يحوّل bool إلى نص عربي (نشط/معطّل) — يُستخدم مع ConverterParameter</summary>
    public class BoolToActiveTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
            (value is bool b && b) ? "نشط" : "معطّل";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
