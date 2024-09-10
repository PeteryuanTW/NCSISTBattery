namespace NCSISTBattery.Services
{
	public enum MyToastType
	{
		Success,
		Warning,
		Danger,
	}
	public class MyToastService
	{
		public Action<string>? ShowSuccessAct;
		public void ShowSuccess(string content) => ShowSuccessAct?.Invoke(content);

		public Action<string>? ShowWarningAct;
		public void ShowWarning(string content) => ShowWarningAct?.Invoke(content);

		public Action<string>? ShowDangerAct;
		public void ShowDanger(string content) => ShowDangerAct?.Invoke(content);


	}
}
