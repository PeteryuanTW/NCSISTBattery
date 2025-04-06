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
    public enum CommandTypeEnum
    {
        HorizontalMovement = 0,
        VerticalDowmMovement = 1,
        VerticalUpMovement = 2,
        Suction = 3,
        Release = 4,
    }
    public enum CommandStatus
    {
        Nnknown = -1,
        Processing = 0,
        InQueue = 1,
        Finished = 2,
    }
}
