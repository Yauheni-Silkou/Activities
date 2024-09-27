namespace ActivityControlApp.Models
{
    public class CsvDataWriter : DataWriter
    {
        public override void WriteData(string dirPath, User user, Action<string, string> actionIfError)
        {
            string path = $"{dirPath}\\{user.Name}.csv";
            try
            {
                using StreamWriter stream = new(path, false, Encoding.UTF8);
                stream.WriteLine($"{nameof(User.Name)};{user.Name}");
                stream.WriteLine($"{nameof(User.Status)};{user.Status}");
                stream.WriteLine($"{nameof(User.Mean)};{user.Mean}");
                stream.WriteLine($"{nameof(User.Max)};{user.Max}");
                stream.WriteLine($"{nameof(User.Min)};{user.Min}");
                stream.WriteLine();
                stream.WriteLine($"{nameof(StepCounter.Day)};{nameof(StepCounter.Count)};{nameof(StepCounter.Rank)}");
                foreach (var st in user.Steps)
                {
                    stream.WriteLine($"{st.Day};{st.Count};{st.Rank}");
                }
            }
            catch (Exception ex)
            {
                actionIfError(path, ex.Message);
            }
        }
    }
}
