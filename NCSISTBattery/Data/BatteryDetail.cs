using DevExpress.Entity.Model.Metadata;
using DevExpress.Office;
using NCSISTBattery.Data.EFModel;

namespace NCSISTBattery.Data
{
	public enum HeatStatus
	{
		TooLow,
		Appropriate,
		TooHigh,
	}
	public enum HeatPieceAmountStatus
	{
		TooLow,
		Appropriate,
		TooHigh,
	}
	public enum BatteryStatus
	{
		NotFail,
		Success,
		Fail,
	}

	public class BatteryDetail:ICloneable<BatteryDetail>
	{
		private string name;
		public string Name => name;
		public readonly BatteryConfig batteryConfig;
		public readonly double lowerBound;
		public readonly double upperBound;

		private bool onStation;
		public bool OnStation => onStation;

		public HeatStatus heatStatus => GetHeatStatus();
		public HeatPieceAmountStatus heatPieceAmountStatus => GetHeatPieceAmountStatus();
		public BatteryStatus batteryStatus => GetBatteryStatus();

		private Stack<HeatPiece> HeatPiecesStack;
		public int currentHeatPieces => HeatPiecesStack.Count;
		public double currentHeat => HeatPiecesStack.Sum(x=>x.TotalHeat);


		public List<HeatPiece> HeatPiecesList => HeatPiecesStack == null ? new List<HeatPiece>() : HeatPiecesStack.ToList();
		public BatteryDetail(string name, BatteryConfig batteryConfig,  double lower, double upper)
		{
			this.name = name;
			this.batteryConfig = batteryConfig;
			this.lowerBound = lower;
			this.upperBound = upper;
			HeatPiecesStack = new();
		}

		private HeatStatus GetHeatStatus()
		{
			if (currentHeat < lowerBound)
			{
				return HeatStatus.TooLow;
			}
			else if (currentHeat > upperBound)
			{
				return HeatStatus.TooHigh;
			}
			else
			{
				return HeatStatus.Appropriate;
			}
		}

		private HeatPieceAmountStatus GetHeatPieceAmountStatus()
		{
			if (currentHeatPieces < batteryConfig.LimitedPieces)
			{
				return HeatPieceAmountStatus.TooLow;
			}
			else if (currentHeatPieces > batteryConfig.LimitedPieces)
			{
				return HeatPieceAmountStatus.TooHigh;
			}
			else
			{
				return HeatPieceAmountStatus.Appropriate;
			}
		}

		private BatteryStatus GetBatteryStatus()
		{
			if (heatStatus == HeatStatus.TooLow && heatPieceAmountStatus == HeatPieceAmountStatus.TooLow)
			{
				return BatteryStatus.NotFail;
			}
			else if (heatStatus == HeatStatus.Appropriate && heatPieceAmountStatus == HeatPieceAmountStatus.Appropriate)
			{
				return BatteryStatus.Success;
			}
			else
			{
				return BatteryStatus.Fail;
			}
		}

		public void SetOnStation(bool b)
		{
			onStation = b;
		}

		public void PushStack(HeatPiece heatPiece)
		{
			int currentAmount = HeatPiecesStack.Count;
			heatPiece.PushInStack(heatPiece.JigName, currentAmount);
			HeatPiecesStack.Push(heatPiece);
		}

		public BatteryDetail Clone()
		{
			BatteryDetail res = new BatteryDetail(name, batteryConfig, lowerBound, upperBound);
			res.SetOnStation(onStation);
			foreach (var heatPiece in HeatPiecesStack)
			{
				res.PushStack(heatPiece);
			}
			return res;
		}
	}
}
