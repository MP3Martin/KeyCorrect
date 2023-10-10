using System.Runtime.InteropServices;
using InputInterceptorNS;
using Spectre.Console;
using static KeyCorrect.Util;

namespace KeyCorrect {
    internal static class Program {
        internal static List<string> AlphabetCharactersAndMoreAsString = new();
        internal static List<KeyCode> AlphabetKeyCodes = new();

        // start of api import
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_SHOWWINDOW = 0x0040;
        // end of api import

        internal static class MainStatus {
            internal const string VERSION = "1.0.0";
            internal static bool Active = false;
            internal static string TextToWrite {
                set {
                    _textToWrite = value;
                    if (!Active) {
                        TextToWriteStable = value;
                    }
                }
                get { return _textToWrite; }
            }
            private static string _textToWrite = "";
            /// <summary>
            /// Like textToWrite but ony updates when active is False
            /// </summary>
            internal static string TextToWriteStable = "";


            internal static KeyboardHook KeyboardHook;

            internal static bool IgnoreAllKeyPressesButStillSendThem = false;

            internal static bool StopUpdatingText = false;

            internal static bool ShouldClearConsole = false;

        }
        public static void Main(string[] args) {
            Init.Run();

            if (InitializeDriver()) {
                // create hooks
                MainStatus.KeyboardHook = new KeyboardHook(KeyboardCallback);

                // create live updating console text
                Console.Clear();
                AnsiConsole.Live(new Markup("Loading..."))
                .StartAsync(async ctx => {
                    while (true) {
                        if (MainStatus.StopUpdatingText) {
                            Console.Clear();
                            return;
                        }
                        const int MAX_SHOWN_TEXT_TO_WRITE_LEN = 60;
                        string textToWrite = MainStatus.TextToWriteStable;
                        if (textToWrite.Length > MAX_SHOWN_TEXT_TO_WRITE_LEN) {
                            textToWrite = textToWrite.Substring(0, MAX_SHOWN_TEXT_TO_WRITE_LEN - 3) + "...";
                        }

                        Markup unsupportedCharWarning() {
                            if (!DoesStringOnlyContainStandardLowercaseLetters(MainStatus.TextToWriteStable)) {
                                return new Markup("[gold3]Warning:[/] [indianred1]This program only supports typing english " +
                                "letters\nand most basic punctuation marks![/]");
                            }
                            return new Markup("");
                        }

                        string textToWriteStableEscaped = EscapeString(textToWrite);
                        // calculate how much of the text was already written
                        int textWrittenLen = MainStatus.TextToWriteStable.Length - MainStatus.TextToWrite.Length;
                        string textToWriteLeftPart;
                        string textToWriteRightPart;
                        try {
                            textToWriteLeftPart = $"{textToWriteStableEscaped[..textWrittenLen]}";
                        } catch (Exception) {
                            textToWriteLeftPart = textToWriteStableEscaped;
                        }
                        try {
                            textToWriteRightPart = $"{textToWriteStableEscaped[textWrittenLen..]}";
                        } catch (Exception) {
                            textToWriteRightPart = "";
                        }

                        var panel = new Spectre.Console.Panel(new Rows(
                            new Markup($"[gold3]Intercept writing:[/] " +
                            $"[{(MainStatus.Active ? "green" : "red")}]{(MainStatus.Active ? "active" : "inactive")}[/]"),
                            new Markup($"[gold3]Text to write:[/] [#9a6f32]{textToWriteLeftPart}[/][darkorange]{textToWriteRightPart}[/]"),
                            new Markup(""),
                            unsupportedCharWarning()
                        ));
                        panel.Header = new PanelHeader("[blue]Status[/]");
                        panel.Border = BoxBorder.Rounded;
                        panel.BorderStyle = new Style(Spectre.Console.Color.DarkCyan);
                        panel.Width = MAX_SHOWN_TEXT_TO_WRITE_LEN + 20;
                        ctx.UpdateTarget(
                            new Rows(
                                panel,
                                new Markup(" "),
                                new Markup("[darkslategray2]Info: [/][lightsalmon3_1]When you enable [lightskyblue1]Intercept " +
                                $"writing[/], this program will get your clipboard contents. It will type\nthe next correct " +
                                $"letter from your clipboard when you press any letter found in standard english alphabet.[/]"),
                                new Markup(" "),
                                new Markup("[gold3]Press [magenta2]PageUp[/] to toggle [lightskyblue1]Intercept " +
                                $"writing[/].[/]"),
                                new Markup(" "),
                                new Markup("[gold3]Press [magenta2]X[/] or [magenta2]Q[/] while this " +
                                $"window is active to [lightskyblue1]Stop[/] the program.[/]")
                            )
                        );
                        ctx.Refresh();
                        await Task.Delay(10);
                    }
                });

                // Create a timer to get clipboard
                System.Timers.Timer getClipboardTimer = new System.Timers.Timer();
                getClipboardTimer.Elapsed += new System.Timers.ElapsedEventHandler((source, e) => {
                    if (MainStatus.Active) {
                        return;
                    }
                    string clipboardText = TextCopy.ClipboardService.GetText();
                    try {
                        if (clipboardText == null) {
                            clipboardText = string.Empty;
                        }
                    } catch (Exception ex) {
                        clipboardText = string.Empty;
                    }
                    MainStatus.TextToWrite = clipboardText;
                });
                getClipboardTimer.Interval = 200;
                getClipboardTimer.Enabled = true;

                // loop until key to exit is pressed
                char Key = '-';
                while (!(Char.ToLower(Key) == 'q' || Char.ToLower(Key) == 'x')) {
                    Key = Console.ReadKey(true).KeyChar;
                }
                MainStatus.KeyboardHook.Dispose();
                MainStatus.ShouldClearConsole = true;
                Console.Clear();
            } else {
                InstallDriver();
            }

            MainStatus.StopUpdatingText = true;
            Thread.Sleep(100);
            if (MainStatus.ShouldClearConsole) {
                Console.Clear();
            }
            Console.WriteLine("\nProgram ended. Press any key to close this window.");
            Console.ReadKey(true);

            bool KeyboardCallback(ref KeyStroke keyStroke) {
                //Console.WriteLine($"{keyStroke.Code} {keyStroke.State} {keyStroke.Information}");
                if (MainStatus.IgnoreAllKeyPressesButStillSendThem) {
                    return true;
                }
                // Check if pageUp is pressed
                if (keyStroke.Code == KeyCode.Numpad9 && (keyStroke.State == KeyState.E0 || keyStroke.State == (KeyState.E0 | KeyState.Up))) {
                    // Fix issues with pageUp being special key
                    if (keyStroke.State == (KeyState.E0 | KeyState.Up)) {
                        keyStroke.State = KeyState.Up;
                    } else {
                        keyStroke.State = KeyState.Down;
                    }

                    // Check if the key is released
                    if (keyStroke.State == KeyState.Up) {
                        // toggle active state
                        MainStatus.Active = !MainStatus.Active;
                        if (MainStatus.Active) {
                            // nothing needed
                        }
                    }

                    // cancel real keypress
                    return false;
                } else {
                    if (KeyCodeInAlphabet(keyStroke)) {
                        // if the pressed key is in standard alphabet

                        if (MainStatus.Active && keyStroke.State == KeyState.Down && MainStatus.TextToWrite.Length > 0) {
                            // type the next correct character
                            TypeNextChar();
                            // cancel real keypress
                            return false;
                        }
                    } else if (MainStatus.Active) {
                        // interception is active but the key pressed was not in standard english alphabet
                        switch (keyStroke.Code) {
                            case KeyCode.LeftWindowsKey or KeyCode.RightWindowsKey or KeyCode.Alt or KeyCode.Tab or
                            KeyCode.Control or KeyCode.LeftShift or KeyCode.RightShift or KeyCode.Delete:
                                return true;
                            default:
                                return false;
                        }
                    }
                }
                if (MainStatus.TextToWrite.Length <= 0 && MainStatus.Active) {
                    // cancel the keypress if done writing
                    return false;
                }
                return true;
            }


        }
    }
}
