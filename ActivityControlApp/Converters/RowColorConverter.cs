using System.Windows.Data;

namespace ActivityControlApp.Converters
{
    public class RowColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var user = value as User;
            int minDiff = user.Mean - user.Min;
            int maxDiff = user.Max - user.Mean;
            if (user.Max > 1.2 * user.Mean && maxDiff > minDiff)
            {
                return "Max";
            }
            else if (user.Min < 0.8 * user.Mean && maxDiff < minDiff)
            {
                return "Min";
            }
            else if (user.Max > 1.2 * user.Mean || user.Min < 0.8 * user.Mean)
            {
                return "Mean";
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
