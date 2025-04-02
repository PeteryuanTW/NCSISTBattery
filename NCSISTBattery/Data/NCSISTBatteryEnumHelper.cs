using CommonLibraryP.Data;
using CommonLibraryP.MachinePKG;
using DevExpress.Blazor;

namespace NCSISTBattery.Data
{
    public static class NCSISTBatteryEnumHelper
    {
        public static IEnumerable<HeatpieceStyleWrapperClass> GetHeatpieceStylesWrapperClass()
        {
            return Enum.GetValues(typeof(ButtonRenderStyle)).OfType<ButtonRenderStyle>()
                .Select(x => new HeatpieceStyleWrapperClass(x));
        }
    }

    public class HeatpieceStyleWrapperClass : EnumWrapper
    {
        public HeatpieceStyleWrapperClass(ButtonRenderStyle renderStyle)
        {
            index = (int)renderStyle;
            displayName = renderStyle.ToString();
        }
    }
}
