using System.Data;
using System.Diagnostics;

namespace NCSISTBattery.Data
{
	// type 1: go station param_1, type 2: quit station param_1, type 3: move form param_1 to param_2
	public class AllocationCommand
	{
		private bool processed;
		public bool Processed => processed;

		private int commandType;
		public int CommandType => commandType;


		private string param_1;
		public string Param_1 => param_1;
		private string param_2;
		public string Param_2 => param_2;

		public AllocationCommand(int type, string param1, string param2)
		{
			processed = false;
			commandType = type;
			param_1 = param1;
			param_2 = param2;

		}

		public void ActualRun()
		{
			processed = true;
		}
    }
}
