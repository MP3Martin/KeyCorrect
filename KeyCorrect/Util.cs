using InputInterceptorNS;
using Spectre.Console;
using static KeyCorrect.Program;

namespace KeyCorrect {
    internal static class Util {
        internal static string escapeString(string str) {
            return str.EscapeMarkup().Replace("\r\n", "↵").Replace("\n", "↵").Replace("	", "⭾");
        }

        internal static bool doesStringOnlyContainStandardLowercaseLetters(string input) {
            foreach (string character in alphabetCharactersAndMoreAsString) {
                input = input.Replace(character, "");
            }
            return (input == "");
        }

        internal static async void typeNextChar() {
            string codeToPress = MainStatus.textToWrite.Substring(0, 1);
            MainStatus.ignoreAllKeyPressesButStillSendThem = true;
            MainStatus.keyboardHook.SimulateInput(fixCzechKeyboardKeys(codeToPress));
            MainStatus.ignoreAllKeyPressesButStillSendThem = false;
            MainStatus.textToWrite = MainStatus.textToWrite[1..];
        }

        internal static string fixCzechKeyboardKeys(string input) {
            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower() == "cs") {
                input = input.Replace("z", "ㅁ").Replace("y", "z").Replace("ㅁ", "y");
                input = input.Replace("Z", "ㅁ").Replace("Y", "Z").Replace("ㅁ", "Y");
            }
            return input;
        }

        internal static bool keyCodeInAlphabet(KeyStroke keyStroke) {
            foreach (KeyCode keyCode in alphabetKeyCodes) {
                if (keyCode == keyStroke.Code) {
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
