using System.ComponentModel.DataAnnotations.Schema;

namespace NCSISTBattery.EFModel
{
    public partial class Jig
    {

        private Stack<JigContent> jigContent = new();
        [NotMapped]
        public List<JigContent> JigContent => jigContent.ToList();
        public void ImportContents(List<JigContent> newjigContents)
        
        {
            foreach (var newjigContent in newjigContents.OrderBy(x=>x.PushToStartTime))
            {
                jigContent.Push(newjigContent);
            }
        }

        //public void PushHeatPiece(JigContent heatPiece)
        //{
        //    if (!IsDestination)
        //    {
        //        heatPiece.StartJigId = Id;
        //    }
        //    else
        //    {
        //        heatPiece.DestinationJigId = Id;
        //    }
        //    jigContent.Push(heatPiece);
        //}

        //public JigContent? peekHeatPiece()
        //{
        //    try
        //    {
        //        return jigContent.Peek();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        //public JigContent? PopupHeatPiece()
        //{
        //    try
        //    {
        //        return jigContent.Pop();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

    }
}
