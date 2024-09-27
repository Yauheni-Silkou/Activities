using System.Xml.Linq;

namespace ActivityControlApp.Models
{
    public class XmlDataWriter : DataWriter
    {
        public override void WriteData(string dirPath, User user, Action<string, string> actionIfError)
        {
            string path = $"{dirPath}\\{user.Name}.xml";
            try
            {
                XDocument xml = new(
                    new XElement("User",
                        new XElement(nameof(User.Name), user.Name),
                        new XElement(nameof(User.Status), user.Status),
                        new XElement(nameof(User.Mean), user.Mean),
                        new XElement(nameof(User.Max), user.Max),
                        new XElement(nameof(User.Min), user.Min),
                        new XElement(nameof(User.Steps), user.Steps.Select(x =>
                            new XElement(nameof(StepCounter),
                                new XElement(nameof(StepCounter.Day), x.Day),
                                new XElement(nameof(StepCounter.Count), x.Count),
                                new XElement(nameof(StepCounter.Rank), x.Rank))))));

                xml.Save(path);
            }
            catch(Exception ex)
            {
                actionIfError(path, ex.Message);
            }
        }
    }
}
