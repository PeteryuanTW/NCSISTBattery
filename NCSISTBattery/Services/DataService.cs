using NCSISTBattery.Data.EFModel;

namespace NCSISTBattery.Services
{
	public class DataService
	{
		private readonly IServiceScopeFactory _scopeFactory;

		public DataService(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		#region battery material
		public Task<List<string>> GetAllBatteryMaterials()
		{
			using (var scope = _scopeFactory.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
				return Task.FromResult(context.BatteryMaterials.Select(x=>x.Name).ToList());
			}
		}
		#endregion

		#region battery config
		public Task<List<BatteryConfig>> GetAllBatteryConfigs()
		{
			using (var scope = _scopeFactory.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
				return Task.FromResult(context.BatteryConfigs.ToList());
			}
		}
		public Task<List<BatteryRecipe>> GetAllBatteryRecipeByName(string name)
		{
			using (var scope = _scopeFactory.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
				return Task.FromResult(context.BatteryRecipes.Where(x=>x.BatteryName == name).OrderByDescending(x=>x.RecipeIndex).ToList());
			}
		}
		#endregion
	}
}
