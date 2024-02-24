// Thanks to https://stackoverflow.com/a/36720802

using System.Runtime.InteropServices;

namespace KeyCorrect {
    internal static class DisableConsoleQuickEdit {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        internal static void Go() {
            var consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // Get current console mode
            if (!GetConsoleMode(consoleHandle, out var consoleMode)) {
                // ERROR: Unable to get console mode.
                return;
            }

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~ENABLE_QUICK_EDIT;

            // Set the new mode
            SetConsoleMode(consoleHandle, consoleMode);
        }

        // ReSharper disable InconsistentNaming
        private const uint ENABLE_QUICK_EDIT = 0x0040;

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        private const int STD_INPUT_HANDLE = -10;
        // ReSharper restore InconsistentNaming
    }
}
