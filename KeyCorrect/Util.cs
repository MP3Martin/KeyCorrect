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
                PressSimpleLetter = HandleCzechKeyboardLayout(CodeToPress, PressSimpleLetter, CapsLockEnabled, PressKey);
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

        private static bool HandleCzechKeyboardLayout(string CodeToPress, bool PressSimpleLetter, bool CapsLockEnabled, Action<object> PressKey) {
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
            return PressSimpleLetter;
        }

        private static object? SpecialKeysLookup(string CodeToPress, ref bool PressSimpleLetter) {
            CodeToPress = CodeToPress.ToLower();
            Action HandleNumbers = () => {
                var numberKeyCodePairs = new Dictionary<string, KeyCode> {
                    {"1", KeyCode.One},
                    {"2", KeyCode.Two},
                    {"3", KeyCode.Three},
                    {"4", KeyCode.Four},
                    {"5", KeyCode.Five},
                    {"6", KeyCode.Six},
                    {"7", KeyCode.Seven},
                    {"8", KeyCode.Eight},
                    {"9", KeyCode.Nine},
                    {"0", KeyCode.Zero},
                };
                KeyCode keyCodeNumberToPress = numberKeyCodePairs[CodeToPress];
                MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                MainStatus.KeyboardHook.SimulateKeyPress(keyCodeNumberToPress, 1);
                MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
            };
            object? SpecialKeyToPress = null;
            Dictionary<string, object> string_KeyToPress_Pairs = new() {
                { "+", 1 },
                { "ě", 2 },
                { "š", 3 },
                { "č", 4 },
                { "ř", 5 },
                { "ž", 6 },
                { "ý", 7 },
                { "á", 8 },
                { "í", 9 },
                { "é", 0 },
                {
                    "ď", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.D);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.D);
                    }
                },
                {
                    "ň", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.N);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.N);
                    }
                },
                {
                    "ó", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.O);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.O);
                    }
                },
                {
                    "ť", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.T);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.T);
                    }
                },
                {
                    "ú", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.OpenBracketBrace, 1);
                    }
                },
                {
                    "ů", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Semicolon, 1);
                    }
                },
                {
                    "\n", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Enter, 1);
                    }
                },
                {
                    "\t", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Tab, 1);
                    }
                },
                {
                    "=", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Dash, 1);
                    }
                },
                {
                    "?", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Comma, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                },
                {
                    "", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Dot, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                },
                {
                    "_", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Slash, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                },
                {
                    "!", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Apostrophe, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                },
                {
                    "\"", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Semicolon, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                },
                {
                    "(", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CloseBracketBrace, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                },
                {
                    ")", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CloseBracketBrace, 1);
                    }
                },
                {
                    "/", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.OpenBracketBrace, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                },
                {
                    "-", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Slash, 1);
                    }
                },
                {
                    "'", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Backslash, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                },
                {
                    ":", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Dot, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                }
            };

            if (string_KeyToPress_Pairs.ContainsKey(CodeToPress)) {
                SpecialKeyToPress = string_KeyToPress_Pairs[CodeToPress];
            } else if (new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" }.Contains(CodeToPress)) {
                SpecialKeyToPress = HandleNumbers;
            } else {
                PressSimpleLetter = true;
            }
            return SpecialKeyToPress;
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

        internal static System.Timers.Timer CreateClassicTimer(Action function, int interval) {
            System.Timers.Timer Timer = new();
            Timer.Elapsed += new System.Timers.ElapsedEventHandler((source, e) => {
                function();
            });
            Timer.Interval = interval;
            Timer.Enabled = true;
            return Timer;
        }

        internal class MyTimer {
            public MyTimer(Action toRun, int interval) {
                Start = () => {
                    Timer = new System.Timers.Timer();
                    Timer.Elapsed += new System.Timers.ElapsedEventHandler((source, e) => {
                        try {
                            ToRun?.Invoke();
                        } catch (ArithmeticException) { }
                    });
                    Timer.Interval = interval;
                    Timer.Enabled = true;
                };

                ToRun = toRun;
            }
            public Action Start;
            public Action ToRun;
            public System.Timers.Timer Timer = new();
            public int Interval {
                get => (int)Timer.Interval;
                set => Timer.Interval = value;
            }
        }

        internal static string FixSpecialChars(string input) {
            return input.Replace("\r\n", "\n").Replace("	", "\t").Replace("„", @"""").Replace("“", @"""");
        }
    }
}
