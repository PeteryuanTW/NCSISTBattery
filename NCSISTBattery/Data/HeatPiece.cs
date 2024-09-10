using DevExpress.Office;

namespace NCSISTBattery.Data
{
    public class HeatPiece:ICloneable<HeatPiece>
    {
        private double _weight = 0.0f;
        public double Weight => _weight;

        private double _heatPerGram = 0.0f;
        public double HeatPerGram => _heatPerGram;

        public double TotalHeat => _weight * _heatPerGram;

        private string _jigName = string.Empty;
        public string JigName => _jigName;

        private int _jigIndex = -1;
        public int JigIndex => _jigIndex;

        public HeatPiece(double weight, double heatPerGram)
        {
            _weight = weight;
            _heatPerGram = heatPerGram;
        }
        public void PushInStack(string jigName, int index)
        {
            _jigName = jigName;
            _jigIndex = index;
        }

		public HeatPiece Clone()
		{
            HeatPiece res = new HeatPiece(_weight, _heatPerGram);
            res.PushInStack(_jigName, _jigIndex);

            return res;
		}
	}
}
