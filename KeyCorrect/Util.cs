using Spectre.Console;

namespace KeyCorrect {
    internal static class Util {
        internal static string EscapeString(string str) {
            return str.EscapeMarkup().Replace("\r\n", "↵").Replace("\n", "↵").Replace("	", "⭾");
        }

        internal static bool DoesStringOnlyContainStandardLowercaseLetters(string input) {
            foreach (string character in AlphabetCharactersAndMoreAsString) {
                input = input.Replace(character, "");
            }
            return (input == "");
        }

        internal static async void TypeNextChar() {
            string codeToPress = MainStatus.TextToWrite.Substring(0, 1);
            MainStatus.IgnoreAllKeyPressesButStillSendThem = true;
            MainStatus.KeyboardHook.SimulateInput(FixCzechKeyboardKeys(codeToPress), 0, 1);
            MainStatus.IgnoreAllKeyPressesButStillSendThem = false;
            MainStatus.TextToWrite = MainStatus.TextToWrite[1..];
        }

        internal static string FixCzechKeyboardKeys(string input) {
            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower() == "cs") {
                input = input.Replace("z", "ㅁ").Replace("y", "z").Replace("ㅁ", "y");
                input = input.Replace("Z", "ㅁ").Replace("Y", "Z").Replace("ㅁ", "Y");
            }
            return input;
        }

        internal static bool KeyCodeInAlphabet(KeyStroke keyStroke) {
            foreach (KeyCode KeyCode in AlphabetKeyCodes) {
                if (KeyCode == keyStroke.Code) {
                    return true;
                }
            }
            return false;
        }

        internal static Boolean InitializeDriver() {
            if (InputInterceptor.CheckDriverInstalled()) {
                //Console.WriteLine("Input interceptor seems to be installed.");
                if (InputInterceptor.Initialize()) {
                    //Console.WriteLine("Input interceptor successfully initialized.");
                    return true;
                }
            }
            Console.WriteLine("Input interceptor initialization failed.");
            return false;
        }

        internal static void InstallDriver() {
            Console.WriteLine("Input interceptor not installed.");
            if (InputInterceptor.CheckAdministratorRights()) {
                Console.WriteLine("Installing...");
                if (InputInterceptor.InstallDriver()) {
                    Console.WriteLine("Done! Restart your computer.");
                } else {
                    Console.WriteLine("Something has gone wrong :(");
                }
            } else {
                Console.WriteLine("Restart program with administrator rights so it will be installed.");
            }
        }
    }
}
