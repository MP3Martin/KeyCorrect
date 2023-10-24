global using InputInterceptorNS;
global using static KeyCorrect.Program;
global using static KeyCorrect.Util;
using System.Runtime.InteropServices;
using static KeyCorrect.KeyboardCallbackClass;

namespace KeyCorrect {
    internal static class Program {
        internal static List<string> SupportedCharacters = new();
        internal static List<KeyCode> KeyStrokeTriggerKeyCodes = new();

        // start of api import
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_SHOWWINDOW = 0x0040;
        // end of api import

        internal static class MainStatus {
            internal const string VERSION = "1.2.4";
            internal static bool Active = false;
            internal static string TextToWrite {
                set {
                    _textToWrite = value;
                    if (!Active) {
                        TextToWriteStable = value;
                    }
                }
                get { return _textToWrite; }
            }
            private static string _textToWrite = "";
            /// <summary>
            /// Like textToWrite but ony updates when active is False
            /// </summary>
            internal static string TextToWriteStable = "";


            internal static KeyboardHook KeyboardHook;

            internal static bool IgnoreAllKeyPressesButStillSendThem = false;

            internal static bool StopUpdatingText = false;

            internal static bool ShouldClearConsole = false;

            internal static string KeyboardLayout = "";

            internal static IntPtr hWnd;

        }
        public static void RunMainProgram() {
            Initialize.Run();

            if (InitializeDriver()) {
                // create hooks
                MainStatus.KeyboardHook = new KeyboardHook(KeyboardCallback);

                // create live updating console text
                LiveConsole.Run();

                // Create a timer to get clipboard
                Timers.ClipboardTimer.Start();

                // Create a timer to get keyboard layout
                Timers.KeyboardLayoutTimer.Start();

                // loop until key to exit is pressed
                char Key = '-';
                while (!(Char.ToLower(Key) == 'q' || Char.ToLower(Key) == 'x')) {
                    Key = Console.ReadKey(true).KeyChar;
                }
                MainStatus.KeyboardHook.Dispose();
                MainStatus.ShouldClearConsole = true;
                Console.Clear();
            } else {
                InstallDriver();
            }

            MainStatus.StopUpdatingText = true;
            Thread.Sleep(100);
            if (MainStatus.ShouldClearConsole) {
                Console.Clear();
            }
            Console.WriteLine("\nProgram ended. Press any key to close this window.");
            Console.ReadKey(true);
        }
    }
}
