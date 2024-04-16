using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Timer = System.Timers.Timer;

namespace KeyCorrect {
    [SuppressMessage("Interoperability", "CA1416")]
    internal static class Util {
        internal static string EscapeString(string str) {
            return FixSpecialChars(str.EscapeMarkup()).Replace("\n", "╝").Replace("\t", "»").Replace("	", "»");
        }

        internal static bool DoesStringOnlyContainSupportedCharacters(string input) {
            input = input.ToLower();
            input = FixSpecialChars(input);
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var character in SupportedCharacters) {
                input = input.Replace(character, "");
            }

            return input.Replace("\n", "").Replace("\r", "").Replace(Environment.NewLine, "") == "";
        }

        internal static void TypeNextChar() {
            var removeFromTextToWrite = 1;
            var codeToPress = string.Empty;
            try {
                if (FixSpecialChars(MainStatus.TextToWrite[..2]) == "\n" || FixSpecialChars(MainStatus.TextToWrite[..2]) == "\t") {
                    removeFromTextToWrite = 2;
                }
            } catch {
                // ignored
            }

            try {
                codeToPress = MainStatus.TextToWrite[..removeFromTextToWrite];
            } catch {
                // ignored
            }

            codeToPress = FixCzechKeyboardKeys(FixSpecialChars(codeToPress));
            var pressSimpleLetter = false;
            MainStatus.IgnoreAllKeyPressesButStillSendThem = true;
            var capsLockEnabled = Console.CapsLock;
            pressSimpleLetter = !IsCzechKeyboardLayout() || HandleCzechKeyboardLayout(codeToPress, pressSimpleLetter, capsLockEnabled, PressKey);
            if (pressSimpleLetter) {
                if (capsLockEnabled) MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CapsLock, 1);
                PressKey(codeToPress);
                if (capsLockEnabled) MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CapsLock, 1);
            }

            MainStatus.IgnoreAllKeyPressesButStillSendThem = false;
            MainStatus.TextToWrite = MainStatus.TextToWrite[removeFromTextToWrite..];
            return;
            void PressKey(object key) {
                MainStatus.KeyboardHook.SimulateInput(key.ToString(), 0, 1);
            }
        }

        private static bool HandleCzechKeyboardLayout(string codeToPress, bool pressSimpleLetter, bool capsLockEnabled, Action<object> pressKey) {
            var specialKeyToPress = SpecialKeysLookup(codeToPress, ref pressSimpleLetter);
            if (specialKeyToPress == null) return pressSimpleLetter;
            var shouldInvertCapsLock = capsLockEnabled;
            if (codeToPress.ToUpper() == codeToPress) shouldInvertCapsLock = !shouldInvertCapsLock;

            if (shouldInvertCapsLock) MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CapsLock, 1);
            if (specialKeyToPress is Action action) {
                action();
            } else {
                pressKey(specialKeyToPress);
            }

            if (shouldInvertCapsLock) MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CapsLock, 1);
            return pressSimpleLetter;
        }

        private static object? SpecialKeysLookup(string codeToPress, ref bool pressSimpleLetter) {
            codeToPress = codeToPress.ToLower();
            object? specialKeyToPress = null;
            Dictionary<string, object> stringSpecialKeyToPressPairs = new() {
                { "+", 1 },
                { "ě", 2 },
                { "š", 3 },
                { "č", 4 },
                { "ř", 5 },
                { "ž", 6 },
                { "ý", 7 },
                { "á", 8 },
                { "í", 9 },
                { "é", 0 }, {
                    "ď", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.D);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.D);
                    }
                }, {
                    "ň", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.N);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.N);
                    }
                }, {
                    "ó", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.O);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.O);
                    }
                }, {
                    "ť", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Equals);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.T);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.T);
                    }
                }, {
                    "ú", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.OpenBracketBrace, 1);
                    }
                }, {
                    "ů", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Semicolon, 1);
                    }
                }, {
                    "\n", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Enter, 1);
                    }
                }, {
                    "\t", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Tab, 1);
                    }
                }, {
                    "=", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Dash, 1);
                    }
                }, {
                    "?", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Comma, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                }, {
                    "%", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Dash, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                }, {
                    "", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Dot, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                }, {
                    "_", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Slash, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                }, {
                    "!", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Apostrophe, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                }, {
                    "\"", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Semicolon, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                }, {
                    "(", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CloseBracketBrace, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                }, {
                    ")", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.CloseBracketBrace, 1);
                    }
                }, {
                    "/", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.OpenBracketBrace, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                }, {
                    "-", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Slash, 1);
                    }
                }, {
                    "'", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Backslash, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                }, {
                    ":", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Dot, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
                    }
                }, {
                    "§", () => {
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Apostrophe, 1);
                    }
                }, {
                    "$", () => {
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Control);
                        MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.Alt);
                        MainStatus.KeyboardHook.SimulateKeyPress(KeyCode.Semicolon, 1);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Alt);
                        MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.Control);
                    }
                }
            };

            if (stringSpecialKeyToPressPairs.TryGetValue(codeToPress, out var specialKey)) {
                specialKeyToPress = specialKey;
            } else if (new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" }.Contains(codeToPress)) {
                specialKeyToPress = (Action?)HandleNumbers;
            } else {
                pressSimpleLetter = true;
            }

            return specialKeyToPress;

            void HandleNumbers() {
                var numberKeyCodePairs = new Dictionary<string, KeyCode> {
                    { "1", KeyCode.One },
                    { "2", KeyCode.Two },
                    { "3", KeyCode.Three },
                    { "4", KeyCode.Four },
                    { "5", KeyCode.Five },
                    { "6", KeyCode.Six },
                    { "7", KeyCode.Seven },
                    { "8", KeyCode.Eight },
                    { "9", KeyCode.Nine },
                    { "0", KeyCode.Zero }
                };
                var keyCodeNumberToPress = numberKeyCodePairs[codeToPress];
                MainStatus.KeyboardHook.SimulateKeyDown(KeyCode.LeftShift);
                MainStatus.KeyboardHook.SimulateKeyPress(keyCodeNumberToPress, 1);
                MainStatus.KeyboardHook.SimulateKeyUp(KeyCode.LeftShift);
            }
        }

        private static string FixCzechKeyboardKeys(string input) {
            if (!IsCzechKeyboardLayout()) return input;
            input = input.Replace("z", "ㅁ").Replace("y", "z").Replace("ㅁ", "y");
            input = input.Replace("Z", "ㅁ").Replace("Y", "Z").Replace("ㅁ", "Y");
            return input;
        }

        private static bool IsCzechKeyboardLayout() {
            return MainStatus.KeyboardLayout == "cs";
        }

        /// <summary>
        ///     Returns if the keyStroke should type the next character
        /// </summary>
        internal static bool KeyStrokeTriggersNextChar(KeyStroke keyStroke) {
            return KeyStrokeTriggerKeyCodes.Contains(keyStroke.Code);
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
                Console.WriteLine(InputInterceptor.InstallDriver() ? "Done! Restart your computer." : "Something has gone wrong :(");
            } else {
                Console.WriteLine("Restart program with administrator rights so it will be installed.");
            }
        }

        private static string FixSpecialChars(string input) {
            return input.Replace("\r\n", "\n").Replace("	", "\t").Replace("„", @"""").Replace("“", @"""");
        }

        internal class MyTimer {
            public readonly Action Start;
            public readonly Action ToRun;
            private Timer _timer = new();
            public MyTimer(Action toRun, int interval) {
                Start = () => {
                    _timer = new();
                    _timer.Elapsed += (_, _) => {
                        try {
                            ToRun?.Invoke();
                        } catch {
                            // ignored
                        }
                    };
                    _timer.Interval = interval;
                    _timer.Enabled = true;
                };

                ToRun = toRun;
            }
        }
    }
}
