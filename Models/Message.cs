using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        // FK vers Identity
        public string? UserId { get; set; }

        public int LikeCount { get; set; }
        public IdentityUser? User { get; set; }
    }
}