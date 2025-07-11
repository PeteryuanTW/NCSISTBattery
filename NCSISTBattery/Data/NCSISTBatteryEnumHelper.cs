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

        public static IEnumerable<WorkAreaWrapperClass> GetWorkAreaWrapperClasses()
        {
            return Enum.GetValues(typeof(WorkArea)).OfType<WorkArea>()
                .Select(x => new WorkAreaWrapperClass(x));
        }

        public static string GetWorkAreaChinese(string code)
        {
            //var a = GetHeatpieceStylesWrapperClass();
            var target = GetWorkAreaWrapperClasses().FirstOrDefault(x=>x.Index.ToString() == code);
            
            if (target is not null)
            {
                return target.DisplayName.ToString() switch
                {
                    "GantryArea" => "龍門區",
                    "PorcessingArea" => "在製區",
                    "ElectrodeHandleArea" => "極柄區",
                    _ => "未定義",
                };
            }
            else
            {
                return "未知";
            }
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

    public enum WorkArea
    {
        GantryArea,
        PorcessingArea,
        ElectrodeHandleArea,
    }

    public class WorkAreaWrapperClass : EnumWrapper
    {
        public WorkAreaWrapperClass(WorkArea workArea)
        {
            WorkArea = workArea;
            index = (int)workArea;
            displayName = workArea.ToString();
        }

        public WorkArea WorkArea { get; init; }
    }
}
