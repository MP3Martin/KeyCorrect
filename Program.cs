using System.Text;
using System.Timers;
using InputInterceptorNS;
using Spectre.Console;

namespace KeyboardIntercept {
    internal class Program {
        static class MainStatus {
            public static bool active = false;
            public static string textToWrite = "";
        }
        [STAThread]
        public static void Main(string[] args) {
            Console.OutputEncoding = Encoding.UTF8;
            async void StartListening() {
                //Console.WriteLine("yes");
            }

            if (InitializeDriver()) {
                // create hooks
                KeyboardHook keyboardHook = new KeyboardHook(KeyboardCallback);
                //Console.WriteLine("Hooks enabled. Press any key to release.");

                char Key = '-';


                // create live updating console text
                Console.Clear();
                AnsiConsole.Live(new Markup("Loading..."))
                .StartAsync(async ctx => {
                    while (true) {
                        const int MAX_SHOWN_TEXT_TO_WRITE_LEN = 50;
                        string textToWrite = MainStatus.textToWrite;
                        if (textToWrite.Length > MAX_SHOWN_TEXT_TO_WRITE_LEN) {
                            textToWrite = textToWrite.Substring(0, MAX_SHOWN_TEXT_TO_WRITE_LEN - 3) + "...";
                        }
                        var panel = new Spectre.Console.Panel(new Rows(
                            new Markup($"[gold3]Intercept writing:[/] [{(MainStatus.active ? "green" : "red")}]{(MainStatus.active ? "active" : "inactive")}[/]"),
                            new Markup($"[gold3]Text to write:[/] [darkorange]{textToWrite.EscapeMarkup()}[/]")
                        ));
                        panel.Header = new PanelHeader("[blue]Status[/]");
                        panel.Border = BoxBorder.Rounded;
                        panel.BorderStyle = new Style(Spectre.Console.Color.DarkCyan);
                        //panel.Expand();
                        panel.Width = 70;
                        ctx.UpdateTarget(
                            new Rows(
                                panel,
                                new Markup(" "),
                                new Markup("[gold3]Press [magenta3]PageUp[/] to toggle [lightskyblue1]Intercept writing[/].[/]"),
                                new Markup(" "),
                                new Markup("[gold3]Press [magenta3]X[/] or [magenta3]Q[/] to [lightskyblue1]Stop[/] the program.[/]")
                            )
                        );
                        ctx.Refresh();
                        await Task.Delay(10);
                    }
                });

                // Create a timer to get clipboard
                System.Timers.Timer myTimer = new System.Timers.Timer();
                myTimer.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs e) => {
                    if (MainStatus.active) {
                        return;
                    }

                    string clipboardText = GetClipboardData();
                    MainStatus.textToWrite = clipboardText;
                });
                myTimer.Interval = 50;
                myTimer.Enabled = true;



                while (!(Char.ToLower(Key) == 'q' || Char.ToLower(Key) == 'x')) {
                    Key = Console.ReadKey(true).KeyChar;
                }
                keyboardHook.Dispose();
            } else {
                InstallDriver();
            }

            Console.WriteLine("Program ended.");

            void KeyboardCallback(ref KeyStroke keyStroke) {
                //Console.WriteLine($"{keyStroke.Code} {keyStroke.State} {keyStroke.Information}");
                // Check if pageUp is pressed
                if (keyStroke.Code == KeyCode.Numpad9 && (keyStroke.State == KeyState.E0 || keyStroke.State == (KeyState.E0 | KeyState.Up))) {
                    // Fix issues with pageUp being special key
                    if (keyStroke.State == (KeyState.E0 | KeyState.Up)) {
                        keyStroke.State = KeyState.Up;
                    } else {
                        keyStroke.State = KeyState.Down;
                    }

                    keyStroke.Code = KeyCode.LeftShift;
                    // Check if the key is released
                    if (keyStroke.State == KeyState.Up) {
                        MainStatus.active = !MainStatus.active;
                        if (MainStatus.active) {
                            StartListening();
                        }
                    }
                }
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
