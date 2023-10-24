namespace KeyCorrect {
    internal static class Initialize {
        internal static void Run() {
            // start of window on top
            string GuidConsoleTitle = Guid.NewGuid().ToString();
            Console.Title = GuidConsoleTitle;
            Thread.Sleep(50);
            MainStatus.hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            MainStatus.hWnd = FindWindow(null, Console.Title);
            Thread.Sleep(50);

            if (MainStatus.hWnd != IntPtr.Zero) {
                // Set the console window to be always on top
                SetWindowPos(MainStatus.hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
            }
            // end of window on top

            DisableConsoleQuickEdit.Go();

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = $"KeyCorrect @ v{MainStatus.VERSION} - By MP3Martin";

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

            // create a list of keycodes that represent the simple alphabet + some special characters
            foreach (int num in AlphabetNums) {
                KeyStrokeTriggerKeyCodes.Add((KeyCode)num);
            }
            foreach (char character in new List<char> { '.', '.', '§', '/', '\'', '(', ')', ';', '\\', '?', '-', '+' }) {
                KeyStrokeTriggerKeyCodes.Add((KeyCode)character);
            }
            foreach (KeyCode keyCode in new List<KeyCode> { KeyCode.OpenBracketBrace, KeyCode.CloseBracketBrace, KeyCode.Comma, KeyCode.Dot, KeyCode.Slash }) {
                KeyStrokeTriggerKeyCodes.Add(keyCode);
            }

            AlphabetNums = null;
            // lowercase letters
            foreach (int index in Enumerable.Range(97, 122 - 97 + 1)) {
                SupportedCharacters.Add(((char)index).ToString());
            }
            // uppercase letters
            foreach (int index in Enumerable.Range(65, 90 - 65 + 1)) {
                SupportedCharacters.Add(((char)index).ToString());
            }
            foreach (string symbol in new List<string> { "\n", "\r\n", "	", "\t" }) {
                SupportedCharacters.Add(symbol);
            }
            foreach (char symbol in @" +ěščřžýáíé=úů()/""'!?:_,.-=0123456789".ToCharArray()) {
                SupportedCharacters.Add(symbol.ToString());
            }
        }
    }
}
