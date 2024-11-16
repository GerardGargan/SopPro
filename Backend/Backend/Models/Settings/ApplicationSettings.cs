namespace Backend.Models.Settings
{
    public class ApplicationSettings
    {
        public string JwtSecret { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
        public int JwtExpireHours { get; set; }
    }
}