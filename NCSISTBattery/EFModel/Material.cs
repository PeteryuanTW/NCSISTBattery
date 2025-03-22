namespace NCSISTBattery.EFModel
{
    public class Material
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;
        public virtual ICollection<RecipeContent> RecipeContents { get; set; } = null!;
    }
}
