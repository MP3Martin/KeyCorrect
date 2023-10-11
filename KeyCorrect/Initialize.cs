namespace KeyCorrect {
    internal static class Initialize {
        internal static void Run() {
            // start of window on top
            string GuidConsoleTitle = Guid.NewGuid().ToString();
            Console.Title = GuidConsoleTitle;
            Thread.Sleep(50);
            IntPtr hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            hWnd = FindWindow(null, Console.Title);
            Thread.Sleep(50);

            if (hWnd != IntPtr.Zero) {
                // Set the console window to be always on top
                SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
            }
            // end of window on top

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = $"KeyCorrect @ v{MainStatus.VERSION}";

            List<int>? AlphabetNums = new List<int> { 28, 57 };
            for (int i = 16; i <= 25; i++) {
                AlphabetNums.Add(i);
            }
            for (int i = 30; i <= 38; i++) {
                AlphabetNums.Add(i);
            }
            for (int i = 44; i <= 50; i++) {
                AlphabetNums.Add(i);
            }

            // create a list of keycodes that represent the simple alphabet
            foreach (int num in AlphabetNums) {
                AlphabetKeyCodes.Add((KeyCode)num);
            }

            AlphabetNums = null;

            foreach (int index in Enumerable.Range(97, 122 - 97 + 1)) {
                AlphabetCharactersAndMoreAsString.Add(((char)index).ToString());
            }
            foreach (int index in Enumerable.Range(65, 90 - 65 + 1)) {
                AlphabetCharactersAndMoreAsString.Add(((char)index).ToString());
            }
            foreach (string symbol in new List<string> { ".", ",", " " }) {
                AlphabetCharactersAndMoreAsString.Add(symbol);
            }
        }
    }
}
