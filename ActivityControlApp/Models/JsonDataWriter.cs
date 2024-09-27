using Newtonsoft.Json;

namespace ActivityControlApp.Models
{
    public class JsonDataWriter : DataWriter
    {
        public override void WriteData(string dirPath, User user, Action<string, string> actionIfError)
        {
            string path = $"{dirPath}\\{user.Name}.json";
            try
            {

                var json = JsonConvert.SerializeObject(user, Formatting.Indented);
                using var stream = new StreamWriter(path, false, Encoding.UTF8);
                stream.Write(json);
            }
            catch(Exception ex)
            {
                actionIfError(path, ex.Message);
            }
        }
    }
}
