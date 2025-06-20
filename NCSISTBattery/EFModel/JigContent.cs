using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCSISTBattery.EFModel
{
    public partial class JigContent
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public ushort MaterialCode { get; set; }
        public double Heat { get; set; }
        public double Thickness { get; set; }
        public Guid? StartJigId { get; private set; }
        public DateTime? PushToStartTime { get; private set; }
        public Guid? DestinationJigId { get; private set; }
        public DateTime? PushToDestinationTime { get; private set; }
    }
}
