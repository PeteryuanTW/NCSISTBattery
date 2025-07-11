using AutoMapper;
using AutoMapper.QueryableExtensions;
using CommonLibraryP.API;
using DevExpress.Blazor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NCSISTBattery.EFModel;
using OfficeOpenXml;
using SharedMod;
using System.Net.Http.Json;



namespace NCSISTBattery.Services
{
    public class DataService
    {
        private readonly IServiceScopeFactory scopeFactory;

        private readonly APISetting apiSetting;
        public DataService(IServiceScopeFactory scopeFactory, IOptions<APISetting> apiSetting)
        {
            this.scopeFactory = scopeFactory;
            this.apiSetting = apiSetting.Value;
        }
        #region api

        public async Task<CalculationDataDTO> GetCalculationDataDTO()
        {
            var recipeRes = await GetCurrentRecipeAndContentDTO();
            var materialsRes = await GetAllMaterialDTOs();
            var jigsRes = await GetAllJigsWithContentDTO();
            return new CalculationDataDTO
            {
                RecipeDTO = recipeRes,
                MaterialDTOs = materialsRes,
                JigsDTOs = jigsRes
            };
        }

        public async Task<RecipeDTO?> GetCurrentRecipeAndContentDTO()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var recipe = await dbContext.Recipes
                    .Include(x => x.RecipeContents)
                    .ThenInclude(x => x.Material).AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IsCurrent);

                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                return mapper.Map<RecipeDTO>(recipe);
            }
        }

        public async Task<List<JigDTO>> GetAllJigsWithContentDTO()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var jigs = await GetAllJigsWithContent();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                return mapper.Map<List<JigDTO>>(jigs);
            }
        }

        public async Task<List<MaterialDTO>> GetAllMaterialDTOs()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var materialsRes = await dbContext.Materials.AsNoTracking().ToListAsync();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                return mapper.Map<List<MaterialDTO>>(materialsRes);
            }
        }

        public async Task PostCalculationDataDTO()
        {
            try
            {
                var calculationDataDTO = await GetCalculationDataDTO();
                using (HttpClient httpClient = new())
                {
                    var routing = $"{apiSetting.BaseUrl}/{apiSetting.CalculationController}";
                    var res = await httpClient.PostAsJsonAsync(routing, calculationDataDTO, (new CancellationTokenSource(TimeSpan.FromSeconds(3)).Token));
                }
            }
            catch (Exception e)
            {

            }
            
        }

        #endregion

        #region material
        public async Task<List<Material>> GetAllMaterials()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                return await dbContext.Materials.AsNoTracking().ToListAsync();
            }
        }

        public async Task<Material?> GetMaterialByCode(int code)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                return await dbContext.Materials.AsNoTracking().FirstOrDefaultAsync(x => x.TypeCode == code);
            }
        }

        public async Task<int> GetMaterialByName(string name)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var target = await dbContext.Materials.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name);
                return target is not null ? target.TypeCode : 0;
            }
        }

        public async Task<RequestResult> UpsertMaterial(Material material)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
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
                    return new(2, $"Upsert material {material.Name} success");
                }
                catch (Exception ex)
                {
                    return new(2, $"Upsert material {material.Name} fail({ex.Message})");
                }

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

        public async Task<Recipe?> GetCurrentRecipe()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                return await dbContext.Recipes.AsNoTracking().FirstOrDefaultAsync(x => x.IsCurrent);
            }
        }



        public async Task<RequestResult> SetCurrentRecipe(Guid? id)
        {

            var clearResult = await ClearCurrentRecipe();
            if (!clearResult.IsSuccess)
            {
                return clearResult;
            }
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                    var target = await dbContext.Recipes.FirstOrDefaultAsync(x => x.Id == id);
                    if (target is not null)
                    {
                        target.IsCurrent = true;
                        await dbContext.SaveChangesAsync();
                        return new(2, $"Set current recipe {target.Name} success");
                    }
                    return new(4, $"Recipe not found");
                }
                catch (Exception ex)
                {
                    return new(4, $"Set current recipe fail({ex.Message})");
                }

            }
        }

        private async Task<RequestResult> ClearCurrentRecipe()
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                    var targets = await dbContext.Recipes.Where(x => x.IsCurrent).ToListAsync();
                    if (targets is not null && targets.Count > 0)
                    {
                        foreach (var recipe in targets)
                        {
                            recipe.IsCurrent = false;
                        }

                    }
                    await dbContext.SaveChangesAsync();
                }
                return new(2, "Clear current recipe success");
            }
            catch (Exception ex)
            {
                return new(4, $"Clear current recipe fail({ex.Message})");
            }

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

        public Func<Task>? JigChangedFunc;
        private void JigChanged()
            => JigChangedFunc?.Invoke();

        public async Task<List<Jig>> GetAllJigsConfig()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                return await dbContext.Jigs.AsNoTracking().ToListAsync();
            }
        }

        public async Task<List<Jig>> GetAllJigsWithContent()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                var jigs = await dbContext.Jigs.AsNoTracking().ToListAsync();
                foreach (var jig in jigs)
                {
                    var jigContents = await dbContext.JigContents.Where(x => x.StartJigId == jig.Id).AsNoTracking().ToListAsync();
                    //var a = jigContents.Where(x => x.PushToStart && !x.PushToDestination).ToList();

                    jig.ImportContents(jigContents.Where(x => x.PushToStart && !x.PushToDestination).ToList());
                }
                return jigs;
            }
        }



        public async Task<RequestResult> UpsertJig(Jig jig)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                    var target = await dbContext.Jigs.FirstOrDefaultAsync(x => x.Id == jig.Id);
                    if (target is not null)
                    {
                        dbContext.Entry(target).CurrentValues.SetValues(jig);
                    }
                    else
                    {
                        await dbContext.Jigs.AddAsync(jig);
                    }
                    await dbContext.SaveChangesAsync();
                    return new(2, $"Upsert jig {jig.Name} success");
                }
                catch (Exception ex)
                {
                    return new(4, $"Upsert jig {jig.Name} fail({ex.Message})");
                }

            }
        }

        public async Task<RequestResult> DeleteJig(Jig jig)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                    var target = await dbContext.Jigs.FirstOrDefaultAsync(x => x.Id == jig.Id);
                    if (target is not null)
                    {
                        dbContext.Remove(target);
                        await dbContext.SaveChangesAsync();
                        return new(2, $"Delete jig {jig.Name} success");
                    }
                    return new(4, $"Jig {jig.Name} not found");
                }
                catch (Exception ex)
                {
                    return new(4, $"Delete jig {jig.Name} fail({ex.Message})");
                }

            }
        }

        public async Task<List<Jig>> GetJigsByArea(string areaCode)
        {
            var allJigs = await GetAllJigsWithContent();
            return allJigs.Where(x => x.AreaCode.ToString() == areaCode).ToList();
        }

        //update from data

        public async Task<RequestResult> UpdateJigContentToJigFromExcel(Jig jig, List<JigContent> jigContents)
        {
            var allJigs = await GetAllJigsWithContent();
            var targetJig = allJigs.FirstOrDefault(x => x.Id == jig.Id);
            if (targetJig is null)
            {
                return new(4, $"jig not found");
            }
            foreach (var jigContent in jigContents)
            {
                var targetMaterial = await GetMaterialByCode(jigContent.MaterialCode);
                if (targetMaterial is null)
                {
                    return new(4, $"material not found");
                }
                if (!jig.MaterialFree && jig.MaterialTypeCode != targetMaterial.TypeCode)
                {
                    return new(4, $"{jig.Name} and {targetMaterial.Name} type not matched");
                }
                jigContent.InitContentToJig(targetJig);
            }

            var res = await InsertJigContents(jigContents);
            JigChanged();
            return res;
        }



        #endregion

        #region jig content

        public async Task<List<JigContent>> GetJigContentFromFile(IReadOnlyList<IFileInputSelectedFile> Files)
        {
            if (Files.Count is not 1)
            {
                return new List<JigContent>();
            }
            var targetFile = Files.FirstOrDefault();
            using var stream = new System.IO.MemoryStream();
            await targetFile.OpenReadStream(targetFile.Size).CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            var result = new List<JigContent>();
            // title at first row
            for (int row = 2; row <= rowCount; row++)
            {
                var parsingRes = await GetJigContentFromRow(worksheet.Cells, row);
                if (parsingRes.MaterialCode is 0)
                {
                    throw new Exception($"parsing data material at row {row} error");
                }
                result.Add(parsingRes);
            }
            return result;
        }

        private async Task<JigContent> GetJigContentFromRow(ExcelRange excelRange, int row)
        {
            return new JigContent
            {
                Id = Guid.NewGuid(),
                Name = excelRange[row, 3].Text,
                Heat = float.TryParse(excelRange[row, 5].Text, out var heat) ? heat : 0,
                MaterialCode = await GetMaterialByName(excelRange[row, 4].Text),
                Thickness = excelRange[row, 11].Text.Split(',')
                .Select(s => double.Parse(s))
                .Average(),
                ImportOrType = false,
            };
        }

        private async Task<RequestResult> InsertJigContents(List<JigContent> jigContents)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                    await dbContext.JigContents.AddRangeAsync(jigContents);
                    await dbContext.SaveChangesAsync();
                    return new(2, $"jig contents insert success");
                }
                catch (Exception ex)
                {
                    return new(4, $"Insert jig contents fail({ex.Message})");
                }

            }
        }

        public async Task<RequestResult> UpsertJigContent(JigContent jigContent)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                    var target = await dbContext.JigContents.FirstOrDefaultAsync(x => x.Id == jigContent.Id);
                    if (target is not null)
                    {
                        dbContext.Entry(target).CurrentValues.SetValues(jigContent);
                    }
                    else
                    {
                        await dbContext.JigContents.AddAsync(jigContent);
                    }
                    await dbContext.SaveChangesAsync();
                    JigChanged();
                    return new(2, $"Upsert jig content {jigContent.Name} success");
                }
                catch (Exception ex)
                {
                    return new(4, $"Upsert jig content {jigContent.Name} fail({ex.Message})");
                }
            }
        }

        public async Task<RequestResult> RemoveJigContentBeforeStart(JigContent jigContent)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                    var target = await dbContext.JigContents.FirstOrDefaultAsync(x => x.Id == jigContent.Id);
                    if (target is not null)
                    {
                        if (target.PushToStart && !target.PushToDestination)
                        {
                            return await DeleteJigContent(target);
                        }
                    }
                    return new(4, $"Jig content {jigContent.Name} not found");
                }
                catch (Exception ex)
                {
                    return new(4, $"Delete jig content {jigContent.Name} fail({ex.Message})");
                }
            }
        }

        private async Task<RequestResult> DeleteJigContent(JigContent jigContent)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                    var target = await dbContext.JigContents.FirstOrDefaultAsync(x => x.Id == jigContent.Id);
                    if (target is not null)
                    {
                        dbContext.Remove(target);
                        await dbContext.SaveChangesAsync();
                        JigChanged();
                        return new(2, $"Delete jig content {jigContent.Name} success");
                    }
                    return new(4, $"Jig content {jigContent.Name} not found");
                }
                catch (Exception ex)
                {
                    return new(4, $"Delete jig content {jigContent.Name} fail({ex.Message})");
                }
            }
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
