using System.ComponentModel.DataAnnotations;

namespace NCSISTBattery.EFModel
{
    public partial class Jig
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public ushort MaterialTypeCode { get; set; }

        private bool materialFree;
        public bool MaterialFree
        {
            get
            {
                return materialFree;
            }
            set
            {
                if (value)
                {
                    MaterialTypeCode = 0;
                }
                materialFree = value;
            }
        }

        public ushort AreaCode { get; set; }
        [Range(0, 12)]
        public ushort Row { get; set; }
        [Range(1, 12)]
        public ushort RowSpan { get; set; }
        [Range(0, 12)]
        public ushort ColumnIndex { get; set; }
        [Range(1, 12)]
        public ushort ColumnSpan { get; set; }

    }
}
