using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NCSISTBattery.EFModel
{
    public partial class OrderCommand
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int CommandType { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
    }
}
