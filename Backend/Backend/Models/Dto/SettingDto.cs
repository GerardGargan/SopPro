namespace Backend.Models.Dto
{
    public class SettingDto : BaseDto
    {
        public string Type { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string ApplicationUserId { get; set; }

    }
}