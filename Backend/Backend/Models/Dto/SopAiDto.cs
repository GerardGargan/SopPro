namespace Backend.Models.Dto
{
    public class AiSop
    {
        public AiSopVersion SopVersion { get; set; }
    }

    public class AiSopVersion
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<AiSopStep> SopSteps { get; set; }
        public List<AiSopHazard> SopHazards { get; set; }
    }

    public class AiSopStep
    {
        public int? Position { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }

    public class AiSopHazard
    {
        public string Name { get; set; }
        public string ControlMeasure { get; set; }
        public int RiskLevel { get; set; }
    }

}