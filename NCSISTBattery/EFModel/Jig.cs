namespace NCSISTBattery.EFModel
{
    public partial class Jig
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsOnBoard { get; set; }
        public bool IsDestination { get; set; }
    }
}
