using System.ComponentModel.DataAnnotations;

namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}

