using DevExpress.Blazor.Popup.Internal;
using Microsoft.EntityFrameworkCore;
using NCSISTBattery.EFModel;
using System;



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

        private async Task<Material?> GetRandomMaterial()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var allMaterials = await dbContext.Materials.ToListAsync();
                var materialCount = allMaterials.Count;
                if (materialCount is 0)
                {
                    return null;
                }
                Random random = new Random();
                int index = random.Next(materialCount);
                var material = allMaterials[index];
                return material;
            }
        }
        #endregion

        #region recipe

        private Recipe? currentRecipe;
        public Recipe? CurrentRecipe => currentRecipe;

        public void SetRecipe(Recipe? recipe)
        {
            currentRecipe = recipe;
        }

        public async Task<List<Recipe>> GetAllRecipes()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                return await dbContext.Recipes.Include(x => x.RecipeContents.OrderBy(x => x.Sequence)).AsNoTracking().ToListAsync();
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
                    .FirstOrDefaultAsync(x => x.Id == recipe.Id);
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

        #region jigs
        private List<Jig> jigs = new();
        public List<Jig> Jigs => jigs;

        public Func<Task>? JigChangedFunc;
        private void JigChanged()
            => JigChangedFunc?.Invoke();

        public async Task InitAllJigs()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var allJigs = await dbContext.Jigs.AsNoTracking().ToListAsync();
                foreach (var jig in allJigs)
                {
                    //start jig
                    if (!jig.IsDestination)
                    {
                        var heatPieces = await dbContext.HeatPieces.Where(x => x.StartJigId == jig.Id && !x.IsFinished && !x.IsRecord).ToListAsync();
                        jig.ImportHeatPiece(heatPieces);
                    }
                    else
                    {
                        var heatPieces = await dbContext.HeatPieces.Where(x => x.DestinationJigId == jig.Id && x.IsFinished && !x.IsRecord).ToListAsync();
                        jig.ImportHeatPiece(heatPieces);
                    }
                }
                jigs = allJigs;
            }
        }

        public async Task PushFakeHeatPieceIntiJigs(int amount = 50)
        {
            foreach (var jig in jigs)
            {
                var heatpieces = await GenerateFakeHeatPiece(amount);
                jig.ImportHeatPiece(heatpieces);
                JigChanged();
                JigChangedFunc?.Invoke();
                await Task.Delay(500);
            }
        }

        private async Task<List<HeatPiece>> GenerateFakeHeatPiece(int amount = 10)
        {
            Random r = new();
            float min = 1.0f;
            float max = 10.0f;

            var res = new List<HeatPiece>();

            for (int i = 0; i < amount; i++)
            {
                var m = await GetRandomMaterial();
                if (m is not null)
                {
                    var tmp = new HeatPiece
                    {
                        Id = Guid.NewGuid(),
                        Name = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                        Heat = (float)Math.Round(r.NextDouble() * (max - min) + min, 2),

                        MaterialId = m.Id,
                        Material = m,
                    };
                    res.Add(tmp);
                }

            }

            return res;
        }

        #endregion

        #region command
        public async Task<List<OrderCommand>> GetRuntimeCommand()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var cmds = await dbContext.OrderCommands.AsNoTracking().ToListAsync();
                return cmds.OrderBy(x => x.CommandStatus).ToList();
            }
        }

        #endregion
    }
}
