using System.Diagnostics;
using System.Text;

namespace KeyCorrect {
    internal static class Initialize {
        internal static void Run() {

            #region WindowOnTop
            var guidConsoleTitle = Guid.NewGuid().ToString();
            Console.Title = guidConsoleTitle;
            Thread.Sleep(50);
            MainStatus.HWnd = Process.GetCurrentProcess().MainWindowHandle;
#pragma warning disable CA1416
            MainStatus.HWnd = FindWindow(null, Console.Title);
#pragma warning restore CA1416
            Thread.Sleep(50);

            if (MainStatus.HWnd != IntPtr.Zero) {
                // Set the console window to be always on top
                SetWindowPos(MainStatus.HWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
            }
            #endregion

            DisableConsoleQuickEdit.Go();

            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = $"KeyCorrect @ v{MainStatus.Version} - By MP3Martin";

            #region AlphabetNums
            List<int> alphabetNums = new() { 28, 57 }; // enter, space
            for (var i = 16; i <= 25; i++) {
                alphabetNums.Add(i); // q to p
            }
            for (var i = 30; i <= 38; i++) {
                alphabetNums.Add(i); // a to l
            }
            for (var i = 44; i <= 50; i++) {
                alphabetNums.Add(i); // z to m
            }
            for (var i = 2; i <= 11; i++) {
                alphabetNums.Add(i); // numbers from 0 to 9
            }

            // create a list of keycodes that represent the simple alphabet + some special characters
            foreach (var num in alphabetNums) {
                KeyStrokeTriggerKeyCodes.Add((KeyCode)num);
            }
            foreach (var character in new List<char> { '.', '.', '§', '/', '\'', '(', ')', ';', '\\', '?', '-', '+' }) {
                KeyStrokeTriggerKeyCodes.Add((KeyCode)character);
            }
            foreach (var keyCode in new List<KeyCode> { KeyCode.OpenBracketBrace, KeyCode.CloseBracketBrace, KeyCode.Comma, KeyCode.Dot, KeyCode.Slash }) {
                KeyStrokeTriggerKeyCodes.Add(keyCode);
            }
            #endregion

            #region SupportedCharacters
            // lowercase letters
            foreach (var index in Enumerable.Range(97, 122 - 97 + 1)) {
                SupportedCharacters.Add(((char)index).ToString());
            }
            // uppercase letters
            foreach (var index in Enumerable.Range(65, 90 - 65 + 1)) {
                SupportedCharacters.Add(((char)index).ToString());
            }
            foreach (var symbol in new List<string> { "\n", "\r\n", "	", "\t" }) {
                SupportedCharacters.Add(symbol);
            }
            foreach (var symbol in @" +ěščřžďťňýáíéó=úů()/""'!?%$§:_,.-=0123456789".ToCharArray()) {
                SupportedCharacters.Add(symbol.ToString());
            }
            #endregion

        }
    }
}
