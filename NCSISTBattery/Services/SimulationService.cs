using DevExpress.Entity.Model.Metadata;
using DevExpress.XtraPrinting.Shape.Native;
using NCSISTBattery.Data;
using NCSISTBattery.Data.EFModel;
using System.Collections.Generic;
using System.Resources;

namespace NCSISTBattery.Services
{
    public class SimulationService
    {

        private readonly IServiceScopeFactory _scopeFactory;

        public SimulationService(IServiceScopeFactory scopeFactory)
        {
            this._scopeFactory = scopeFactory;
        }
        #region db config
        private Task<List<JigConfig>> GetJigConfigs()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                return Task.FromResult(context.JigConfigs.ToList());
            }
        }
        public Task<List<BatteryConfig>> GetBatteryConfigs()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                return Task.FromResult(context.BatteryConfigs.ToList());
            }
        }
        private Task<List<BatteryRecipe>> GetBatteryRecipeByBatteryName(string name)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<NCSISTBatteryDBContext>();
                return Task.FromResult(context.BatteryRecipes.Where(x => x.BatteryName == name).ToList());
            }
        }
        #endregion



        private BatteryConfig? targetBatteryConfig;
        public BatteryConfig? TargetBattery => targetBatteryConfig;
        public bool BatterySelected => targetBatteryConfig != null;
        public async Task SetBattery(BatteryConfig? batteryConfig)
        {
			targetBatteryConfig = batteryConfig;
            batteryRecipes = await GetBatteryRecipeByBatteryName(targetBatteryConfig?.Type);
		}

		private List<BatteryRecipe> batteryRecipes = new();
		public List<BatteryRecipe> BatteryRecipes => batteryRecipes;

		private List<Jig> _jigs = new();
        public List<Jig> Jigs => _jigs;

		private List<BatteryDetail> batteryDetails = new();

        public List<BatteryDetail> BatteryDetails => batteryDetails;

		public async Task InitJigs()
        {
            _jigs.Clear();
            var jigConfigs = await GetJigConfigs();

            foreach (JigConfig jigConfig in jigConfigs)
            {
                _jigs.Add(new Jig(jigConfig));
            }
        }

        public Task RamdomAssignHeatPieceToJigs(int amountInJig, float baseLine, float error)
        {
            foreach (Jig jig in _jigs)
            {
                double heat = GenerateRandomHeat(307, 3);
                for (int i = 0; i < amountInJig; i++)
                {
                    jig.PushStack(new HeatPiece(GenerateRandomWeight(TargetBattery.MinWeight, TargetBattery.MaxWeight), heat));
                }
            }

            return Task.CompletedTask;
        }

        public double GenerateRandomHeat(double baseLine, double error)//-error to +error
        {
            double lower = baseLine - error;
            double upper = baseLine + error;

            Random random = new Random();
            return (random.NextDouble() * (upper - lower) + lower);
        }

        public double GenerateRandomWeight(double lower, double upper)
        {
            Random random = new Random();
            return (random.NextDouble() * (upper - lower) + lower);
        }

        public void InitBatteryDetails(int amount, double lower, double upper)
        {
            batteryDetails.Clear();

			for (int i = 0;i < amount; i++)
            {
                batteryDetails.Add(new BatteryDetail(i.ToString(), targetBatteryConfig, lower, upper));
			}
        }

        public Task<List<AllocationCommand>> GAHeatPieceAllocattion(int onStationAmount, int round)// List<BatteryRecipe> batteryRecipes, List<Jig> jigs, List<BatteryDetail> batteryDetails, 
		{
            int roundCount = 0;
            SimulationRoundChanged(roundCount);

			bool findRes = false;
            List<AllocationCommand> res = new();
            while (roundCount < round && !findRes)
            {
                List<Jig> jigs_tmp = _jigs.Select(x => x.Clone()).ToList();
                List<BatteryDetail> batteryDetails_tmp = batteryDetails.Select(x => x.Clone()).ToList();

                res.Clear();
                while (HasSolution(batteryRecipes, jigs_tmp, batteryDetails_tmp) && !CheckAllocationFinished(batteryDetails_tmp))
                {
					var adjustCommands = AdjustOnStation(batteryDetails_tmp, onStationAmount);
                    if (adjustCommands.Count > 0)
                    {
						res.AddRange(adjustCommands);
					}
					AllocationCommand? cmd = RandomCommand(batteryRecipes, _jigs, batteryDetails_tmp);
                    RunAllocationCommand(false, cmd, jigs_tmp, batteryDetails_tmp);
                    res.Add(cmd);
                    

					if (CheckAllocationFinished(batteryDetails_tmp))
                    {
                        findRes = true;
                        Console.WriteLine($"find solution at round {roundCount}");
                        break;
                    }

                    if (!HasSolution(batteryRecipes, jigs_tmp, batteryDetails_tmp))
                    {
						findRes = false;
						Console.WriteLine($"source empty at round {roundCount}");
						break;
					}
                }
                roundCount++;
				SimulationRoundChanged(roundCount);
			}
            if (!findRes || roundCount >= round)
            {
                res.Clear();
			}
            return Task.FromResult(res);
        }

        private void SimulationRoundChanged(int r) => SimulationRoundChangedAct?.Invoke(r);

		public Action<int>? SimulationRoundChangedAct;

        private bool HasSolution(List<BatteryRecipe> batteryRecipes, List<Jig> jigs, List<BatteryDetail> batteryDetails)
        {
            if (batteryDetails.Any(x => x.batteryStatus == BatteryStatus.Fail))
            {
                return false;
            }
            else
            {
                var demandRecipeIndexs = batteryDetails.Where(x => x.batteryStatus == BatteryStatus.NotFail).Select(x => x.currentHeatPieces);
                IEnumerable<string> resources = batteryRecipes.Where(x => demandRecipeIndexs.Contains(x.RecipeIndex)).Select(x => x.Type);
                IEnumerable<Jig> targetJigs = jigs.Where(x => !x.isEmpty && resources.Contains(x.Type));
                return targetJigs.Count()>0;
            }
        }

        private bool CheckAllocationFinished(List<BatteryDetail> batteryDetails)
        {
            return !batteryDetails.Any(x => x.batteryStatus != BatteryStatus.Success);
        }

        private List<AllocationCommand> AdjustOnStation(List<BatteryDetail> batteryDetails_tmp, int amount)
        {
            List<AllocationCommand> res = new();

            //quit if allocation success
			var finishedBattery = batteryDetails_tmp.Where(x => x.batteryStatus == BatteryStatus.Success && x.OnStation).ToList();
            finishedBattery.ForEach(x =>
            {
                x.SetOnStation(false);
                res.Add(new AllocationCommand(2, x.Name, String.Empty));
			});

			int onstationAmount = batteryDetails_tmp.Count(x => x.OnStation);
            int emptyAmount = onstationAmount - amount;

            //get on station
			if (emptyAmount < 0)
            {
                var newBatteries = batteryDetails_tmp.Where(x => x.batteryStatus == BatteryStatus.NotFail && !x.OnStation).Take(-1*emptyAmount).ToList();
                newBatteries.ForEach(x =>
                {
                    x.SetOnStation(true);
					res.Add(new AllocationCommand(1, x.Name, String.Empty));
				});
			}
            return res;
		}

        private T RandomChoose<T>(List<T> source)
        {
            Random random = new Random();
            int r = random.Next(source.Count);
            return source[r];
        }

        private AllocationCommand? RandomCommand(List<BatteryRecipe> batteryRecipes, List<Jig> jigs, List<BatteryDetail> batteryDetails)
        {
            if (!HasSolution(batteryRecipes, jigs, batteryDetails))
            {
                return null;
            }
            else
            {
                //pick battery in domand and on station
                var batteryInDomand = batteryDetails.Where(x => x.batteryStatus == BatteryStatus.NotFail && x.OnStation);
                var randomTargetBattery = RandomChoose(batteryInDomand.ToList());

                string source = batteryRecipes.FirstOrDefault(x => x.RecipeIndex == randomTargetBattery.currentHeatPieces).Type;


                IEnumerable<Jig> targetJigs = jigs.Where(x => !x.isEmpty && x.Type == source);
                var randomTargetJig = RandomChoose(targetJigs.ToList());

                return new AllocationCommand(3, randomTargetJig.Name, randomTargetBattery.Name);
            }
        }

        public bool RunAllocationCommand(bool isReal, AllocationCommand cmd, List<Jig> jigs, List<BatteryDetail> batteryDetails)
        {
            bool finalRes = true;
            switch (cmd.CommandType)
            {
                case 1:
					var onStationBattery = batteryDetails.FirstOrDefault(x => x.Name == cmd.Param_1);
                    if (onStationBattery != null)
                    {
                        onStationBattery.SetOnStation(true);
					}
					finalRes = true;
                    break;
                case 2:
					var quitStationBattery = batteryDetails.FirstOrDefault(x => x.Name == cmd.Param_1);
					if (quitStationBattery != null)
					{
						quitStationBattery.SetOnStation(false);
					}
					finalRes = true;
					break;
                case 3:
					var targetJig = jigs.FirstOrDefault(x => x.Name == cmd.Param_1);
					var targetBattery = batteryDetails.FirstOrDefault(x => x.Name == cmd.Param_2);

					if (targetJig == null || targetBattery == null)
					{
						finalRes = false;
					}
					else
					{
						HeatPiece? heatPiece = targetJig.PopStack();
						if (heatPiece == null)
						{
							finalRes = false;
						}
						else
						{
							targetBattery.PushStack(heatPiece);

							finalRes = true;
						}
					}
                    break;
                default:
					finalRes = false;
                    break;
            }
			if (isReal)
			{
				cmd.ActualRun();
			}
            return finalRes;
		}
    }
}
