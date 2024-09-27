using Newtonsoft.Json;
using System.Text.RegularExpressions;


namespace ActivityControlApp.Models
{
    public class JsonDataReader
    {
        public static DailyGroupData ReadData(string path, Action<string, string> actionIfError)
        {
            try
            {
                int day = GetDay(path);
                if (day <= 0)
                {
                    throw new ArgumentException($"The file has wrong name. The format must be 'dayX.json', where X is a positive number.");
                }
                
                DailyGroupData data = new(day);

                string content;
                content = File.ReadAllText(path);

                dynamic result = JsonConvert.DeserializeObject(content);
                foreach (var x in result)
                {
                    data.Records.Add(new Record()
                    {
                        Rank = x.Rank,
                        User = x.User,
                        Status = x.Status,
                        Steps = x.Steps
                    });
                };

                return data;
            }
            catch (Exception ex)
            {
                actionIfError(path, ex.Message);
                return null;
            }
        }

        private static int GetDay(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            int day = 0;
            if (Regex.Match(fileName, @"day[1-9]\d*$").Success)
            {
                day = Convert.ToInt32(fileName[3..]);
            }

            return day;
        }
    }
}
