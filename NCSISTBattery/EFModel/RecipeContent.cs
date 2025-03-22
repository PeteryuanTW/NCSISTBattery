namespace NCSISTBattery.EFModel
{
    public class RecipeContent
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public Guid MaterialId { get; set; }
        public int Sequence { get; set; }
        public virtual Material? Material { get; set; }
        public virtual Recipe? Recipe { get; set; }
    }
}
