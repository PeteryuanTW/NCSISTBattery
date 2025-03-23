namespace NCSISTBattery.EFModel
{
    public partial class RecipeContent
    {
        public RecipeContent() { }
        public RecipeContent(Recipe recipe)
        {
            RecipeId = recipe.Id;
            Sequence = recipe.ContentCount;
        }
    }
}
