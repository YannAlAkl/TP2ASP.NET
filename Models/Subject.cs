using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;
namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Models
{
    public class Subject

    {
        public int Id { get; set; }

        [Required]

        [StringLength(150)]

        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int ViewCount { get; set; } = 0;

        public bool IsDeleted { get; set; } = false;

        public int CategoryId { get; set; }


        public Category? Category { get; set; }

        // FK vers Identity

        public string? UserId { get; set; }

        public IdentityUser? User { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();

    }


 }
