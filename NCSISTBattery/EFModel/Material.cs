using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NCSISTBattery.EFModel
{
    public class Material
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        [Range(1, int.MaxValue)]
        public int TypeCode { get; set; }
        [Required]
        public int StyleIndex { get; set; }
        public virtual ICollection<RecipeContent> RecipeContents { get; set; } = null!;
        
    }
}
