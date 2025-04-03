using System.ComponentModel.DataAnnotations.Schema;

namespace NCSISTBattery.EFModel
{
    public partial class Jig
    {

        private Stack<HeatPiece> jigContent = new();
        [NotMapped]
        public List<HeatPiece> JigContent => jigContent.ToList();

        public void ImportHeatPiece(List<HeatPiece> heatPieces, bool reverse = false)
        {
            if (reverse)
            {
                heatPieces.Reverse();
            }
            foreach (var heatPiece in heatPieces)
            {
                if (IsDestination)
                {
                    heatPiece.DestinationJigId = Id;
                }
                else
                {
                    heatPiece.StartJigId = Id;
                }
                jigContent.Push(heatPiece);
            }
        }

        public void PushHeatPiece(HeatPiece heatPiece)
        {
            if (!IsOnBoard)
            {
                return;
            }
            if (!IsDestination)
            {
                heatPiece.StartJigId = Id;
            }
            else
            {
                heatPiece.DestinationJigId = Id;
            }
            jigContent.Push(heatPiece);
        }

        public HeatPiece? peekHeatPiece()
        {
            try
            {
                return jigContent.Peek();
            }
            catch
            {
                return null;
            }
        }

        public HeatPiece? PopupHeatPiece()
        {
            try
            {
                return jigContent.Pop();
            }
            catch
            {
                return null;
            }
        }

    }
}
