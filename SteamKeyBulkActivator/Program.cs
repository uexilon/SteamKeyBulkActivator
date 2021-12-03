using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
namespace SteamKeyBulkActivator
{
    class Program
    {
        static int checkboxXPosition = 830;
        static int checkboxYPosition = 527;
        static int activateXPosition = 1330;
        static int activateYPosition = 490;
        static void Main()
        {
            Console.WriteLine("Provide the seperator used to seperate your steam keys. ex: ,");
            string seperator = Console.ReadLine();
            Console.WriteLine("Provide your steam keys with , as a seperator. No spaces... ex: XXXXX-XXXXX-XXXXX,YYYYY-YYYYY-YYYYY" +
                "\nTest with one key or a test word first to adjust the click positions for the checkbox and activate button.");
            string keysString = Console.ReadLine();

            Console.WriteLine("On windows, this tool can automatically activate the key by executing two clicks.");
            Console.WriteLine("Would you like to change the click positions (Defaults are used otherwise - Checkbox (830/527) ActivateButton (1330/490)? (Y or N)");
            ConsoleKey pressedKey = Console.ReadKey().Key;
            if(pressedKey == ConsoleKey.Y)
            {
                Console.WriteLine("Choose values in the center of the checkbox/button. No sanity checks are made. Only provide integer numbers.");
                Console.WriteLine("Please provide the checkbox X position from top left of the screen in pixel");
                checkboxXPosition = int.Parse(Console.ReadLine());
                Console.WriteLine("Please provide the checkbox Y position from top left of the screen in pixel");
                checkboxXPosition = int.Parse(Console.ReadLine());
                Console.WriteLine("Please provide the activate button X Position from top left of the screen in pixel");
                activateXPosition = int.Parse(Console.ReadLine());
                Console.WriteLine("Please provide the activate button X position from top left of the screen in pixel");
                activateYPosition = int.Parse(Console.ReadLine());
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Do not move your mouse during the process (when autoclicking on windows");
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(5000);
            string[] keys = keysString.Split(seperator); 
            foreach (string key in keys)
            {
                OpenBrowser($"https://store.steampowered.com/account/registerkey?key={key}");
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Finished!");
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Lazy copied from: https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                    Thread.Sleep(2500);
                    LeftMouseClick(checkboxXPosition, checkboxYPosition);
                    Thread.Sleep(200);
                    LeftMouseClick(activateXPosition, activateYPosition);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                     Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
            
        }
        // Didn't bother to write my own implementation. Thanks goes to: https://stackoverflow.com/a/8273118
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }
    }
}
