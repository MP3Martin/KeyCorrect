using System.Text;
using System.Timers;
using InputInterceptorNS;
using Spectre.Console;

namespace KeyboardIntercept {
    internal class Program {
        static class MainStatus {
            public static bool active = false;
            public static string textToWrite {
                set {
                    _textToWrite = value;
                    if (!active) {
                        textToWriteStable = value;
                    }
                }
                get { return _textToWrite; }
            }
            private static string _textToWrite = "";
            /// <summary>
            /// Like textToWrite but ony updates when active is False
            /// </summary>
            public static string textToWriteStable = "";


            public static KeyboardHook keyboardHook;

            public static bool ignoreAllKeyPressesButStillSendThem = false;

        }
        [STAThread]
        public static void Main(string[] args) {
            // start of init
            Console.OutputEncoding = Encoding.UTF8;
            List<KeyCode> alphabetKeyCodes = new();

            string fixCzechKeyboardKeys(string input) {
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower() == "cs") {
                    input = input.Replace("z", "ㅁ").Replace("y", "z").Replace("ㅁ", "y");
                    input = input.Replace("Z", "ㅁ").Replace("Y", "Z").Replace("ㅁ", "Y");
                }
                return input;
            }

            bool keyCodeInAlphabet(KeyStroke keyStroke) {
                foreach (KeyCode keyCode in alphabetKeyCodes) {
                    if (keyCode == keyStroke.Code) {
                        return true;
                    }
                }
                return false;
            }

            List<int>? alphabetNums = new List<int> { 28, 57 };
            for (int i = 16; i <= 25; i++) {
                alphabetNums.Add(i);
            }
            for (int i = 30; i <= 38; i++) {
                alphabetNums.Add(i);
            }
            for (int i = 44; i <= 50; i++) {
                alphabetNums.Add(i);
            }

            // create a list of keycodes that represent the simple alphabet
            foreach (int num in alphabetNums) {
                alphabetKeyCodes.Add((KeyCode)num);
            }

            alphabetNums = null;

            List<string> alphabetCharactersAndMoreAsString = new();
            foreach (int index in Enumerable.Range(97, 122 - 97 + 1)) {
                alphabetCharactersAndMoreAsString.Add(((char)index).ToString());
            }
            foreach (int index in Enumerable.Range(65, 90 - 65 + 1)) {
                alphabetCharactersAndMoreAsString.Add(((char)index).ToString());
            }
            foreach (string symbol in new List<string> { ".", ",", " " }) {
                alphabetCharactersAndMoreAsString.Add(symbol);
            }

            // end of init

            // start of methods
            string escapeString(string str) {
                return str.EscapeMarkup().Replace("\r\n", "↵").Replace("\n", "↵").Replace("	", "⭾");
            }

            bool doesStringOnlyContainStandardLowercaseLetters(string input) {
                foreach (string character in alphabetCharactersAndMoreAsString) {
                    input = input.Replace(character, "");
                }
                return (input == "");
            }

            async void typeNextChar() {
                string codeToPress = MainStatus.textToWrite.Substring(0, 1);
                MainStatus.ignoreAllKeyPressesButStillSendThem = true;
                MainStatus.keyboardHook.SimulateInput(fixCzechKeyboardKeys(codeToPress));
                MainStatus.ignoreAllKeyPressesButStillSendThem = false;
                MainStatus.textToWrite = MainStatus.textToWrite[1..];
            }

            // end of methods

            if (InitializeDriver()) {
                // create hooks
                MainStatus.keyboardHook = new KeyboardHook(KeyboardCallback);

                // create live updating console text
                Console.Clear();
                AnsiConsole.Live(new Markup("Loading..."))
                .StartAsync(async ctx => {
                    while (true) {
                        const int MAX_SHOWN_TEXT_TO_WRITE_LEN = 60;
                        string textToWrite = MainStatus.textToWriteStable;
                        if (textToWrite.Length > MAX_SHOWN_TEXT_TO_WRITE_LEN) {
                            textToWrite = textToWrite.Substring(0, MAX_SHOWN_TEXT_TO_WRITE_LEN - 3) + "...";
                        }

                        Markup unsupportedCharWarning() {
                            if (!doesStringOnlyContainStandardLowercaseLetters(MainStatus.textToWriteStable)) {
                                return new Markup("[gold3]Warning:[/] [indianred1]This program only supports typing english " +
                                "letters\nand most basic punctuation marks![/]");
                            }
                            return new Markup("");
                        }

                        string textToWriteStableEscaped = escapeString(textToWrite);
                        // calculate how much of the text was already written
                        int textWrittenLen = MainStatus.textToWriteStable.Length - MainStatus.textToWrite.Length;
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
                            $"[{(MainStatus.active ? "green" : "red")}]{(MainStatus.active ? "active" : "inactive")}[/]"),
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
                                new Markup("[gold3]Press [magenta2]X[/] or [magenta2]Q[/] to " +
                                $"[lightskyblue1]Stop[/] the program.[/]")
                            )
                        );
                        ctx.Refresh();
                        await Task.Delay(10);
                    }
                });

                // Create a timer to get clipboard
                System.Timers.Timer getClipboardTimer = new System.Timers.Timer();
                getClipboardTimer.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs e) => {
                    if (MainStatus.active) {
                        return;
                    }

                    string clipboardText = GetClipboardData();
                    MainStatus.textToWrite = clipboardText;
                });
                getClipboardTimer.Interval = 200;
                getClipboardTimer.Enabled = true;

                // loop until key to exit is pressed
                char Key = '-';
                while (!(Char.ToLower(Key) == 'q' || Char.ToLower(Key) == 'x')) {
                    Key = Console.ReadKey(true).KeyChar;
                }
                MainStatus.keyboardHook.Dispose();
            } else {
                InstallDriver();
            }

            Console.WriteLine("\nProgram ended.");

            bool KeyboardCallback(ref KeyStroke keyStroke) {
                //Console.WriteLine($"{keyStroke.Code} {keyStroke.State} {keyStroke.Information}");
                if (MainStatus.ignoreAllKeyPressesButStillSendThem) {
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
                        MainStatus.active = !MainStatus.active;
                        if (MainStatus.active) {
                            // nothing needed
                        }
                    }

                    // cancel real keypress
                    return false;
                } else {
                    if (keyCodeInAlphabet(keyStroke)) {
                        // if the pressed key is in standard alphabet

                        if (MainStatus.active && keyStroke.State == KeyState.Down && MainStatus.textToWrite.Length > 0) {
                            // type the next correct character
                            typeNextChar();
                            // cancel real keypress
                            return false;
                        }
                    } else if (MainStatus.active) {
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
                if (MainStatus.textToWrite.Length <= 0 && MainStatus.active) {
                    // cancel the keypress if done writing
                    return false;
                }
                return true;
            }

            Boolean InitializeDriver() {
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

            void InstallDriver() {
                Console.WriteLine("Input interceptor not installed.");
                if (InputInterceptor.CheckAdministratorRights()) {
                    Console.WriteLine("Installing...");
                    if (InputInterceptor.InstallDriver()) {
                        Console.WriteLine("Done! Restart your computer.");
                    } else {
                        Console.WriteLine("Something... gone... wrong... :(");
                    }
                } else {
                    Console.WriteLine("Restart program with administrator rights so it will be installed.");
                }
            }

            string GetClipboardData() {
                try {
                    string clipboardData = null;
                    Exception threadEx = null;
                    Thread staThread = new Thread(
                        delegate () {
                            try {
                                if (Clipboard.ContainsText(TextDataFormat.Text)) {
                                    clipboardData = Clipboard.GetText(TextDataFormat.Text);
                                } else {
                                    clipboardData = "";
                                }
                            } catch (Exception ex) {
                                threadEx = ex;
                            }
                        });
                    staThread.SetApartmentState(ApartmentState.STA);
                    staThread.Start();
                    staThread.Join();
                    return clipboardData;
                } catch (Exception exception) {
                    return string.Empty;
                }
            }
        }
    }
}
