using System.ComponentModel.DataAnnotations;

namespace NCSISTBattery.Model
{
    public class CurrentRecipeModel
    {
        [Required]
        public Guid? RecipeId { get; set; }
    }
}
