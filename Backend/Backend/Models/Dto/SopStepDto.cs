using Backend.Models.DatabaseModels;

namespace Backend.Models.Dto
{
    public class SopStepDto : BaseDto
    {
        public int? SopVersionId { get; set; }
        public int? Position { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public List<int> PpeIds { get; set; }
        public List<PpeDto> Ppes { get; set; }

        public static SopStepDto FromSopStep(SopStep sopStep)
        {
            var sopStepDto = new SopStepDto
            {
                Id = sopStep.Id,
                SopVersionId = sopStep.SopVersionId,
                Position = sopStep.Position,
                Text = sopStep.Text,
                Title = sopStep.Title,
                ImageUrl = sopStep.ImageUrl,
                PpeIds = sopStep.SopStepPpe?.Select(x => x.PpeId).ToList(),
            };

            return sopStepDto;
        }
    }
}