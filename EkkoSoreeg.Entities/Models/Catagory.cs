using System.ComponentModel.DataAnnotations;

namespace EkkoSoreeg.Entities.Models
{
    public class Catagory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(450)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
