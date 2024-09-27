namespace ActivityControlApp.Models
{
    public class DailyGroupData
    {
        public DailyGroupData(int day)
        {
            Day = day;
            Records = new();
        }

        public DailyGroupData(int day, List<Record> dataSet)
        {
            Day = day;
            Records = dataSet;
        }

        public int Day { get; init; }

        public List<Record> Records { get; init; }
    }
}
