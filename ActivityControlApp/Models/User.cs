namespace ActivityControlApp.Models
{
    public class User
    {
        public string Name { get; init; }
        public string Status { get; init; }
        public int Mean => (int)Steps.Average(x => x.Count);
        public int Max => Steps.Max(x => x.Count);
        public int Min => Steps.Min(x => x.Count);
        public List<StepCounter> Steps { get; init; } = new();
    }
}
