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

        internal static void TypeNextChar() {
            void PressKey(object key) {
                key = key.ToString();
                MainStatus.KeyboardHook.SimulateInput(key.ToString(), 0, 1);
            }
            string CodeToPress = MainStatus.TextToWrite[..1];
            CodeToPress = FixCzechKeyboardKeys(CodeToPress);
            bool PressSimpleLetter = false;
            MainStatus.IgnoreAllKeyPressesButStillSendThem = true;
            bool CapsLockEnabled = Console.CapsLock;
            if (IsCzechKeyboardLayout()) {
                object? SpecialKeyToPress = null;
                switch (CodeToPress.ToLower()) {
                    case "+":
                        SpecialKeyToPress = 1;
                        break;
                    case "ě":
                        SpecialKeyToPress = 2;
                        break;
                    case "š":
                        SpecialKeyToPress = 3;
                        break;
                    case "č":
                        SpecialKeyToPress = 4;
                        break;
                    case "ř":
                        SpecialKeyToPress = 5;
                        break;
                    case "ž":
                        SpecialKeyToPress = 6;
                        break;
                    case "ý":
                        SpecialKeyToPress = 7;
                        break;
                    case "á":
                        SpecialKeyToPress = 8;
                        break;
                    case "í":
                        SpecialKeyToPress = 9;
                        break;
                    case "é":
                        SpecialKeyToPress = 0;
                        break;
                    case "ď":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Equals); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Equals); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.D); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.D);
                        };
                        break;
                    case "ň":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Equals); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Equals); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.N); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.N);
                        };
                        break;
                    case "ó":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Equals); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Equals); MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.O); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.O);
                        };
                        break;
                    case "ť":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Equals); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Equals); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.T); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.T);
                        };
                        break;
                    case "ú":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.OpenBracketBrace);
                        };
                        break;
                    case "ů":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Semicolon);
                        };
                        break;
                    default:
                        PressSimpleLetter = true;
                        break;
                }
                if (SpecialKeyToPress != null) {
                    bool ShouldInvertCapsLock = CapsLockEnabled;
                    if (CodeToPress.ToUpper() == CodeToPress) ShouldInvertCapsLock = !ShouldInvertCapsLock;

                    if (ShouldInvertCapsLock) MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CapsLock, 1);
                    if (SpecialKeyToPress is Action) {
                        ((Action)SpecialKeyToPress)();
                    } else {
                        PressKey(SpecialKeyToPress);
                    }
                    if (ShouldInvertCapsLock) MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CapsLock, 1);
                }
            } else {
                PressSimpleLetter = true;
            }
            if (PressSimpleLetter) {
                if (CapsLockEnabled) MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CapsLock, 1);
                MainStatus.KeyboardHook.SimulateInput(CodeToPress, 0, 1);
                if (CapsLockEnabled) MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CapsLock, 1);
            }
            MainStatus.IgnoreAllKeyPressesButStillSendThem = false;
            MainStatus.TextToWrite = MainStatus.TextToWrite[1..];
        }

        internal static string FixCzechKeyboardKeys(string input) {
            if (IsCzechKeyboardLayout()) {
                input = input.Replace("z", "ㅁ").Replace("y", "z").Replace("ㅁ", "y");
                input = input.Replace("Z", "ㅁ").Replace("Y", "Z").Replace("ㅁ", "Y");
            }
            return input;
        }

        internal static bool IsCzechKeyboardLayout() {
            return MainStatus.KeyboardLayout == "cs";
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

        internal static void CreateTimer(Action function, int interval) {
            System.Timers.Timer Timer = new System.Timers.Timer();
            Timer.Elapsed += new System.Timers.ElapsedEventHandler((source, e) => {
                function();
            });
            Timer.Interval = interval;
            Timer.Enabled = true;

        }
    }
}
