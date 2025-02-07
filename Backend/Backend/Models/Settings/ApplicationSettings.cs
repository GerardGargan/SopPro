namespace Backend.Models.Settings
{
    public class ApplicationSettings
    {
        public string BaseUrl { get; set; }
        public string JwtSecret { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
        public int JwtInviteExpireHours { get; set; }
        public int JwtAuthExpireDays { get; set; }
        public string PostmarkFromEmail { get; set; }
        public string PostmarkApiToken { get; set; }
        public string AzureBlobStorageContainer { get; set; }
    }
}