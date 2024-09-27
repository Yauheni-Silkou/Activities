namespace ActivityControlApp.Models
{
    public abstract class DataWriter
    {
        public void WriteData(string dirPath, IEnumerable<User> users, Action<string, string> actionIfError)
        {
            foreach (var user in users)
            {
                WriteData(dirPath, user, actionIfError);
            }
        }
        public abstract void WriteData(string dirPath, User user, Action<string, string> actionIfError);
    }
}
