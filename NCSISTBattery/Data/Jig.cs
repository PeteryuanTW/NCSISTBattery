using DevExpress.Office;
using NCSISTBattery.Data.EFModel;

namespace NCSISTBattery.Data
{
	public class Jig:ICloneable<Jig>
	{
		private string _name = String.Empty;
		public string Name => _name;

		private string _type = String.Empty;
		public string Type => _type;

		public Stack<HeatPiece> HeatPiecesStack = new();
		public int currentCount => HeatPiecesStack.Count;

		public bool isEmpty => currentCount == 0;
		public List<HeatPiece> HeatPiecesList => HeatPiecesStack.ToList();

		public Jig(JigConfig jigConfig)
		{
			_name = jigConfig.Name;
			_type = jigConfig.Type;
		}

		public void PushStack(HeatPiece heatPiece)
		{
			int currentAmount = HeatPiecesStack.Count;
			heatPiece.PushInStack(_name, currentAmount);
			HeatPiecesStack.Push(heatPiece);
		}

		public HeatPiece? PeekStack()
		{
			return HeatPiecesStack.Count > 0 ? HeatPiecesStack.Peek() : null;
		}

		public HeatPiece? PopStack()
		{
			return HeatPiecesStack.Count > 0 ? HeatPiecesStack.Pop() : null;
		}

		public Jig Clone()
		{
			Jig res = new Jig(new JigConfig { Name = _name, Type = _type });
			foreach (var heatPiece in this.HeatPiecesStack)
			{
				res.PushStack(heatPiece.Clone());
			}
			return res;
		}
	}
}
