using Backend.Models.DatabaseModels;

namespace Backend.Models.Dto
{
    public class PpeDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }

        public static PpeDto FromPpe(Ppe ppe)
        {
            var ppeDto = new PpeDto
            {
                Id = ppe.Id,
                Name = ppe.Name,
                Icon = ppe.Icon
            };

            return ppeDto;
        }
    };


}