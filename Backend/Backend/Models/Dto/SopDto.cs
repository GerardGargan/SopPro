using Backend.Models.DatabaseModels;

namespace Backend.Models.Dto
{
    public class SopDto : BaseDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int? DepartmentId { get; set; }
        public bool? IsApproved { get; set; }
        public SopStatus? Status { get; set; }
        public string Reference { get; set; }
        public bool? isAiGenerated { get; set; }
        public List<SopHazardDto> SopHazards { get; set; }
        public List<SopVersionDto> SopVersions { get; set; }

        public static SopDto FromSop(Sop sop)
        {
            var sopDto = new SopDto
            {
                Id = sop.Id,
                Reference = sop.Reference,
                isAiGenerated = sop.isAiGenerated,
                DepartmentId = sop.DepartmentId,
                SopVersions = sop.SopVersions?.Select(x => SopVersionDto.FromSopVersion(x)).ToList(),
            };

            return sopDto;
        }

    }
}