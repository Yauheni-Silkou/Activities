namespace ActivityControlApp.Models
{
    public class DataProvider
    {
        public List<DailyGroupData> DailyGroupDataSet { get; private set; } = new List<DailyGroupData>();

        public List<User> Users { get; private set; } = new List<User>();

        public void ReadData(IEnumerable<string> pathList, Action<string, string> actionIfError)
        {
            DailyGroupDataSet = new();

            foreach (string path in pathList)
            {
                string ext = path[(path.IndexOf('.') + 1)..].ToLower(CultureInfo.InvariantCulture);
                var dayFileResults = JsonDataReader.ReadData(path, actionIfError);
                if (dayFileResults != null)
                {
                    DailyGroupDataSet.Add(dayFileResults);
                }
            }

            Users = (from pdat in DailyGroupDataSet.SelectMany(x => x.Records, (groupData, personalData) => new { groupData.Day, Data = personalData })
                     group pdat by pdat.Data.User into user
                     select new User()
                     {
                         Name = user.Key,
                         Status = user.LastOrDefault()?.Data.Status,
                         Steps = user.Select(x => new StepCounter { Day = x.Day, Count = x.Data.Steps, Rank = x.Data.Rank })
                         .OrderBy(x => x.Day).ToList()
                     }).ToList();
        }

        public void WriteData(User user, string dirPath, DataWriter writer, Action<string, string> actionIfError)
        {
            writer.WriteData(dirPath, user, actionIfError);
        }

        public void WriteAllData(string dirPath, DataWriter writer, Action<string, string> actionIfError)
        {
            writer.WriteData(dirPath, Users, actionIfError);
        }
    }
}
