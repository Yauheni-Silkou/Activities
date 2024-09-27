using System.Windows.Data;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace ActivityControlApp.Converters
{
    public class StepsToSeriesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return null;
            }

            var user = value as User;

            var mapper = new CartesianMapper<double>()
                .X((val, index) => index)
                .Y((val) => val)
                .Fill((val, index) =>
                {
                    return val switch
                    {
                        _ when val == user.Max => Brushes.Gold,
                        _ when val == user.Min => Brushes.Red,
                        _ => Brushes.RoyalBlue,
                    };
                });

            SeriesCollection series = new(mapper);
            series.Add(new ColumnSeries
            {
                Title = "Steps",
                Values = new ChartValues<double>(user.Steps.Select(x => (double)x.Count))
            });

            return series;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
