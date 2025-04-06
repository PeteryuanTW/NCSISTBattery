using NCSISTBattery.Data;

namespace NCSISTBattery.EFModel
{
    public partial class OrderCommand
    {
        public CommandStatus CommandStatus => (StartTime, FinishTime) switch
        {
            (null, null) => CommandStatus.InQueue,
            (not null, null) => CommandStatus.Processing,
            (not null, not null) => CommandStatus.Finished,
            _ => CommandStatus.Nnknown,

        };

        public string CommandTypeIcon => (CommandTypeEnum)CommandType switch
        {
            CommandTypeEnum.HorizontalMovement => "resize-width",
            CommandTypeEnum.VerticalDowmMovement => "arrow-circle-bottom",
            CommandTypeEnum.VerticalUpMovement => "arrow-circle-top",
            CommandTypeEnum.Suction => "fullscreen-exit",
            CommandTypeEnum.Release => "fullscreen-enter",
            _ => "",
        };
    }
}
