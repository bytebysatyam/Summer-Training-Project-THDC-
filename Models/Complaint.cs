using System.ComponentModel.DataAnnotations;

namespace WebPage.Models
{
    public class Complaint
    {
        public int Id { get; set; }

        [Required]
        public string? Subject { get; set; }

        [Required]
        public string? Description { get; set; }

        public string? UserEmail { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;  // ✅ Default value
        public string? AdminResponse { get; set; }
        public string Status { get; set; }
    }

}
