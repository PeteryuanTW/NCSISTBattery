using System.ComponentModel.DataAnnotations.Schema;

namespace NCSISTBattery.EFModel
{
    public partial class HeatPiece
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public float Heat { get; set; }
        public Guid? StartJigId { get; set; }
        public Guid? DestinationJigId { get; set; }
        //place into destination jig
        public bool IsFinished { get; set; }
        //place into destination jig and finish all sorting
        public bool IsRecord { get; set; }
    }
}
