global using InputInterceptorNS;
global using static KeyCorrect.Program;
global using static KeyCorrect.Util;
using System.Runtime.InteropServices;
using static KeyCorrect.KeyboardCallbackClass;

namespace KeyCorrect {
    internal static class Program {
        internal static readonly List<string> SupportedCharacters = new();
        internal static readonly List<KeyCode> KeyStrokeTriggerKeyCodes = new();
        public static void RunMainProgram() {
            Initialize.Run();

            if (InitializeDriver()) {
                // create hooks
                MainStatus.KeyboardHook = new(KeyboardCallback);

                // create live updating console text
                LiveConsole.Run();

                #region timers
                // Start a timer to get clipboard
                Timers.ClipboardTimer.Start();

                // Start a timer to get keyboard layout
                Timers.KeyboardLayoutTimer.Start();
                #endregion

                // loop until key to exit is pressed
                var key = '-';
                while (!(char.ToLower(key) == 'q' || char.ToLower(key) == 'x')) {
                    key = Console.ReadKey(true).KeyChar;
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

        internal static class MainStatus {
            internal const string Version = "1.3.0";
            internal static bool Active = false;
            private static string _textToWrite = "";

            /// <summary>
            ///     Like TextToWrite but ony updates when active is False
            /// </summary>
            internal static string TextToWriteStable = "";

            internal static KeyboardHook KeyboardHook = null!;

            internal static bool IgnoreAllKeyPressesButStillSendThem = false;

            internal static bool StopUpdatingText;

            internal static bool ShouldClearConsole;

            internal static string KeyboardLayout = "";

            internal static IntPtr HWnd;
            internal static string TextToWrite {
                set {
                    _textToWrite = value;
                    if (!Active) {
                        TextToWriteStable = value;
                    }
                }
                get => _textToWrite;
            }
        }

        #region ApiImport
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        // ReSharper disable InconsistentNaming
        public static readonly IntPtr HWND_TOPMOST = new(-1);
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;

        public const uint SWP_SHOWWINDOW = 0x0040;
        // ReSharper restore InconsistentNaming
        #endregion
    }
}
