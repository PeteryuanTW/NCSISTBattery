using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NCSISTBattery.EFModel
{
    public partial class RecipeContent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }
        [Required]
        public Guid? RecipeId { get; set; }
        [Required]
        public Guid? MaterialId { get; set; }
        public int Sequence { get; set; }
        public virtual Material? Material { get; set; }
        public virtual Recipe? Recipe { get; set; }
    }
}
