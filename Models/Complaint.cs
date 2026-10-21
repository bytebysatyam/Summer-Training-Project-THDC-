using System.ComponentModel.DataAnnotations;

namespace WebPage.Models
{
    public class Complaint
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Subject { get; set; } = string.Empty;

        [Required, StringLength(4000)]
        public string Description { get; set; } = string.Empty;

        [EmailAddress]
        public string? UserEmail { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public string? AdminResponse { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
