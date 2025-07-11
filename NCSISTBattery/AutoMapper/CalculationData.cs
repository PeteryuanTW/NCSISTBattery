using NCSISTBattery.EFModel;

namespace NCSISTBattery.AutoMapper
{
    public class CalculationData
    {
        public Recipe Recipe { get; set; } = null!;
        public List<Jig> Jigs { get; set; } = new();
    }
}
