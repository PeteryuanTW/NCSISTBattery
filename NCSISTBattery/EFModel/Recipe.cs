namespace NCSISTBattery.EFModel
{
    public class Recipe
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<RecipeContent> RecipeContents { get; set; } = null!;
    }
}
