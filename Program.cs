using InputInterceptorNS;

namespace KeyboardIntercept {
    internal class Program {
        static void Main(string[] args) {
            async void StartListening() {
                Console.WriteLine("start");
                await Task.Delay(3000);
                Console.WriteLine("stop");
            }

            if (InitializeDriver()) {
                KeyboardHook keyboardHook = new KeyboardHook(KeyboardCallback);
                Console.WriteLine("Hooks enabled. Press any key to release.");
                char Key = '-';
                while (!(Char.ToLower(Key) == 'q' || Char.ToLower(Key) == 'x')) {
                    Key = Console.ReadKey().KeyChar;
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
                        StartListening();
                    }
                }
            }

            Boolean InitializeDriver() {
                if (InputInterceptor.CheckDriverInstalled()) {
                    Console.WriteLine("Input interceptor seems to be installed.");
                    if (InputInterceptor.Initialize()) {
                        Console.WriteLine("Input interceptor successfully initialized.");
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
                    Console.WriteLine(
                      "Restart program with administrator rights so it will be installed.");
                }
            }
        }
    }
}
