using CommonLibraryP.MachinePKG;

namespace NCSISTBattery.Machine
{
    public class CustomModbusMachine : ModbusTCPMachine
    {
        protected override async Task UpdateStatus()
        {
            ushort s = (await master?.ReadHoldingRegistersAsync((byte)1, (ushort)0, (ushort)1)).FirstOrDefault();
            switch (s)
            {
                case 1:
                    Idle();
                    break;
                case 2:
                    Running();
                    break;
                case 3:
                    Pause();
                    break;
                case 4:
                    SetCustomStatusCode(150);
                    break;
                default:
                    break;
            }
        }
    }
}
