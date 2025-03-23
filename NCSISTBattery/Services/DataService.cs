using DevExpress.Blazor.Popup.Internal;
using Microsoft.EntityFrameworkCore;
using NCSISTBattery.EFModel;

namespace NCSISTBattery.Services
{
    public class DataService
    {
        private readonly IServiceScopeFactory scopeFactory;
        public DataService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        #region material
        public async Task<List<Material>> GetAllMaterials()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                return await dbContext.Materials.AsNoTracking().ToListAsync();
            }
        }

        public async Task UpsertMaterial(Material material)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var target = await dbContext.Materials.FirstOrDefaultAsync(x => x.Id == material.Id);
                if (target is not null)
                {
                    dbContext.Entry(target).CurrentValues.SetValues(material);
                }
                else
                {
                    await dbContext.Materials.AddAsync(material);
                }
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteMaterial(Material material)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var target = await dbContext.Materials.FirstOrDefaultAsync(x => x.Id == material.Id);
                if (target is not null)
                {
                    dbContext.Remove(target);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
        #endregion

        #region recipe

        public async Task<List<Recipe>> GetAllRecipes()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                return await dbContext.Recipes.Include(x=>x.RecipeContents.OrderBy(x=>x.Sequence)).AsNoTracking().ToListAsync();
            }
        }

        public async Task UpsertRecipe(Recipe recipe)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var target = await dbContext.Recipes.FirstOrDefaultAsync(x => x.Id == recipe.Id);
                if (target is not null)
                {
                    dbContext.Entry(target).CurrentValues.SetValues(recipe);
                }
                else
                {
                    await dbContext.Recipes.AddAsync(recipe);
                }
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteRecipe(Recipe recipe)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var target = await dbContext.Recipes.FirstOrDefaultAsync(x => x.Id == recipe.Id);
                if (target is not null)
                {
                    dbContext.Remove(target);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        #endregion

        #region recipe content

        public async Task UpsertRecipeContent(RecipeContent recipeContent)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var target = await dbContext.RecipeContents.FirstOrDefaultAsync(x => x.Id == recipeContent.Id);
                if (target is not null)
                {
                    dbContext.Entry(target).CurrentValues.SetValues(recipeContent);
                }
                else
                {
                    await dbContext.RecipeContents.AddAsync(recipeContent);
                }
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpsertRecipeContent(List<RecipeContent> recipeContents)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                foreach (var content in recipeContents)
                {
                    var target = await dbContext.RecipeContents.FirstOrDefaultAsync(x => x.Id == content.Id);
                    if (target is not null)
                    {
                        dbContext.Entry(target).CurrentValues.SetValues(content);
                    }
                    else
                    {
                        await dbContext.RecipeContents.AddAsync(content);
                    }
                    await dbContext.SaveChangesAsync();
                }
                
                
            }
        }

        public async Task DeleteRecipeContent(RecipeContent recipeContent)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var target = await dbContext.RecipeContents.FirstOrDefaultAsync(x => x.Id == recipeContent.Id);
                if (target is not null)
                {
                    dbContext.Remove(target);
                    await dbContext.SaveChangesAsync();
                    await ResetRecipeContentSequence(recipeContent);
                }
            }
        }

        private async Task ResetRecipeContentSequence(RecipeContent recipeContent)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var targets = dbContext.RecipeContents.Where(x => x.RecipeId == recipeContent.RecipeId).OrderBy(x => x.Sequence);
                if (targets.Count() > 0)
                {
                    int newSequence = 0;
                    foreach (var content in targets)
                    {
                        content.Sequence = newSequence;
                        newSequence++;
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task ResetRecipeContentSequence(Recipe recipe)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var target = await dbContext.Recipes.Include(x => x.RecipeContents).FirstOrDefaultAsync(x => x.Id == recipe.Id);
                if (target is not null && target.ContentCount > 0)
                {
                    int newSequence = 0;
                    foreach (var content in target.RecipeContents)
                    {
                        content.Sequence = newSequence;
                        newSequence++;
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UPdateAllRecipeContents(Recipe recipe)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var target = await dbContext.Recipes
                    .Include(x => x.RecipeContents)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(x=>x.Id == recipe.Id);
                if (target is not null)
                {
                    var newContents = target.RecipeContents;
                    //delete not exist
                    var contentsToDelete = newContents.Where(x => !recipe.RecipeContents.Any(y => y.Id == x.Id)).ToList();
                    if (contentsToDelete.Count() > 0)
                    {
                        dbContext.RecipeContents.RemoveRange(contentsToDelete);
                    }
                    foreach (var newContent in newContents)
                    {
                        var targetContent = await dbContext.RecipeContents.FirstOrDefaultAsync(x => x.Id == newContent.Id);
                        if (targetContent is not null)
                        {
                            dbContext.Entry(targetContent).CurrentValues.SetValues(newContent);
                        }
                        else
                        {
                            dbContext.RecipeContents.Add(newContent);
                        }
                    }
                    await dbContext.SaveChangesAsync();
                }

            }
        }

        #endregion
    }
}
