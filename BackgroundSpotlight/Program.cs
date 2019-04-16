using System.Runtime.InteropServices;
using System.Threading;

namespace BackgroundSpotlight
{
	internal static class Program
	{
		private static string _currentSpotlightPath;

		private const int SetWallpaper = 20;
		private const int UpdateIniFile = 0x01;
		private const int SendWinIniChange = 0x02;

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

		private static void ChangeWallpaper(string pathToImage)
		{
			SystemParametersInfo(SetWallpaper, 0, pathToImage, UpdateIniFile | SendWinIniChange);
		}
		private static void UpdateDesktop()
		{
			ChangeWallpaper(_currentSpotlightPath);
		}

		// Show output only if in a command prompt
		[DllImport("kernel32.dll")]
		private static extern void AttachConsole(int dwProcessId);

		private static void Main()
		{
			AttachConsole(-1);


			while (true)
			{
				// Run every minute
				var latestCurrentImage = FindImage.FindCurrentImage();
				if (_currentSpotlightPath != latestCurrentImage)
				{
					_currentSpotlightPath = latestCurrentImage;
					UpdateDesktop();
				}
				// Sleep process 1 minute
				Thread.Sleep(60 * 1000);
			}
		}
	}
}
