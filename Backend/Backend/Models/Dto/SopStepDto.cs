using Backend.Models.DatabaseModels;

namespace Backend.Models.Dto
{
    public class SopStepDto : BaseDto
    {
        public int? SopVersionId { get; set; }
        public int? Position { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }

        public static SopStepDto FromSopStep(SopStep sopStep)
        {
            var sopStepDto = new SopStepDto
            {
                Id = sopStep.Id,
                SopVersionId = sopStep.SopVersionId,
                Position = sopStep.Position,
                Text = sopStep.Text,
                ImageUrl = sopStep.ImageUrl
            };

            return sopStepDto;
        }
    }
}