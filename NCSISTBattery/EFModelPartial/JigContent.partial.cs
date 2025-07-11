using DevExpress.Utils.Svg;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCSISTBattery.EFModel
{
    public partial class JigContent
    {
        public bool PushToStart => StartJigId is not null && PushToStartTime is not null;
        public bool PushToDestination => DestinationJigId is not null && PushToDestinationTime is not null;

        public void InitContentToJig(Jig jig)
        {
            StartJigId = jig.Id;
            MaterialCode = jig.MaterialTypeCode;
            PushToStartTime = DateTime.Now;
        }
    }
}
