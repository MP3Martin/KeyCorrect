using Spectre.Console;

namespace KeyCorrect {
    internal static class Util {
        internal static string EscapeString(string str) {
            return FixSpecialChars(str.EscapeMarkup()).Replace("\n", "╝").Replace("\t", "»").Replace("	", "»");
        }

        internal static bool DoesStringOnlyContainSupportedCharacters(string input) {
            input = input.ToLower();
            input = FixSpecialChars(input);
            foreach (string character in SupportedCharacters) {
                input = input.Replace(character, "");
            }
            return (input.Replace("\n", "").Replace("\r", "").Replace(Environment.NewLine, "") == "");
        }

        internal static void TypeNextChar() {
            void PressKey(object key) {
                key = key.ToString();
                MainStatus.KeyboardHook.SimulateInput(key.ToString(), 0, 1);
            }
            object? SpecialKeysLookup(string CodeToPress, ref bool PressSimpleLetter) {
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
                            MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.OpenBracketBrace, 1);
                        };
                        break;
                    case "ů":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Semicolon, 1);
                        };
                        break;
                    case "\n":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Enter, 1);
                        };
                        break;
                    case "\t":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Tab, 1);
                        };
                        break;
                    case "=":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Dash, 1);
                        };
                        break;
                    case "?":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Comma, 1); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        };
                        break;
                    case ":":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Dot, 1); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        };
                        break;
                    case "_":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Slash, 1); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        };
                        break;
                    case "!":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Apostrophe, 1); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        };
                        break;
                    case "\"":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Semicolon, 1); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        };
                        break;
                    case "(":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CloseBracketBrace, 1); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        };
                        break;
                    case ")":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CloseBracketBrace, 1);
                        };
                        break;
                    case "/":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.OpenBracketBrace, 1); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        };
                        break;
                    case "-":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Slash, 1);
                        };
                        break;
                    case "'":
                        SpecialKeyToPress = () => {
                            MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift); MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Backslash, 1); MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        };
                        break;
                    case "1" or "2" or "3" or "4" or "5" or "6" or "7" or "8" or "9" or "0":
                        SpecialKeyToPress = () => {
                            var NumberKeyCodePairs = new List<(String, KeyCode)> {
                                ("1", KeyCode.One),
                                ("2", KeyCode.Two),
                                ("3", KeyCode.Three),
                                ("4", KeyCode.Four),
                                ("5", KeyCode.Five),
                                ("6", KeyCode.Six),
                                ("7", KeyCode.Seven),
                                ("8", KeyCode.Eight),
                                ("9", KeyCode.Nine),
                                ("0", KeyCode.Zero),
                            };
                            KeyCode KeyCodeNumberToPress = NumberKeyCodePairs.First(i => i.Item1 == CodeToPress).Item2;
                            MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                            MainStatus.KeyboardHook.SimulateKeyPress(KeyCodeNumberToPress, 1);
                            MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        };
                        break;
                    default:
                        PressSimpleLetter = true;
                        break;
                }
                return SpecialKeyToPress;
            }
            int RemoveFromTextToWrite = 1;
            string CodeToPress = string.Empty;
            try {
                if (FixSpecialChars(MainStatus.TextToWrite[..2]) == "\n" || FixSpecialChars(MainStatus.TextToWrite[..2]) == "\t") {
                    RemoveFromTextToWrite = 2;
                }
            } catch (Exception) { }
            try {
                CodeToPress = MainStatus.TextToWrite[..RemoveFromTextToWrite];
            } catch (Exception) { }
            CodeToPress = FixCzechKeyboardKeys(FixSpecialChars(CodeToPress));
            bool PressSimpleLetter = false;
            MainStatus.IgnoreAllKeyPressesButStillSendThem = true;
            bool CapsLockEnabled = Console.CapsLock;
            if (IsCzechKeyboardLayout()) {
                var SpecialKeyToPress = SpecialKeysLookup(CodeToPress, ref PressSimpleLetter);
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
                PressKey(CodeToPress);
                if (CapsLockEnabled) MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CapsLock, 1);
            }
            MainStatus.IgnoreAllKeyPressesButStillSendThem = false;
            MainStatus.TextToWrite = MainStatus.TextToWrite[RemoveFromTextToWrite..];
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

        /// <summary>
        /// Returns if the keyStroke should type the next character
        /// </summary>
        internal static bool KeyStrokeTriggersNextChar(KeyStroke keyStroke) {
            foreach (KeyCode KeyCode in KeyStrokeTriggerKeyCodes) {
                if (KeyCode == keyStroke.Code) {
                    return true;
                }
            }
            return false;
        }

        internal static bool InitializeDriver() {
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

        internal static string FixSpecialChars(string input) {
            return input.Replace("\r\n", "\n").Replace("	", "\t").Replace("„", @"""").Replace("“", @"""");
        }
    }
}
