using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NCSISTBattery.EFModel
{
    public class Material
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Type { get; set; } = null!;
        public virtual ICollection<RecipeContent> RecipeContents { get; set; } = null!;
    }
}
