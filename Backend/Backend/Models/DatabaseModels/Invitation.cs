using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DatabaseModels
{
    /// <summary>
    /// Invitation model
    /// </summary>
    public class Invitation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public int OrganisationId { get; set; }
        [ForeignKey("OrganisationId")]
        public Organisation Organisation { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        [Required]
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public enum Status
    {
        Pending,
        Accepted,
        Revoked
    }
}
