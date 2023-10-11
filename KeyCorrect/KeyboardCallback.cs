namespace KeyCorrect {
    internal static class KeyboardCallbackClass {
        internal static bool KeyboardCallback(ref KeyStroke KeyStroke) {
            //Console.WriteLine($"{KeyStroke.Code} {KeyStroke.State} {KeyStroke.Information}");
            if (MainStatus.IgnoreAllKeyPressesButStillSendThem) {
                return true;
            }
            // Check if pageUp is pressed
            if ((!Console.NumberLock && KeyStroke.Code == KeyCode.Numpad9) || (KeyStroke.Code == KeyCode.Numpad9 && (KeyStroke.State == KeyState.E0 || KeyStroke.State == (KeyState.E0 | KeyState.Up)))) {
                // Fix issues with pageUp being special key
                if (KeyStroke.State == (KeyState.E0 | KeyState.Up)) {
                    KeyStroke.State = KeyState.Up;
                } else if (KeyStroke.State == KeyState.E0) {
                    KeyStroke.State = KeyState.Down;
                }
                //Console.WriteLine($"{KeyStroke.Code} {KeyStroke.State} {KeyStroke.Information}");

                // Check if the key is released
                if (KeyStroke.State == KeyState.Up) {
                    // toggle active state
                    MainStatus.Active = !MainStatus.Active;
                    if (MainStatus.Active) {
                        // nothing needed
                    }
                }

                // cancel real keypress
                return false;
            } else {
                if (KeyCodeInAlphabet(KeyStroke)) {
                    // if the pressed key is in standard alphabet

                    if (MainStatus.Active && KeyStroke.State == KeyState.Down && MainStatus.TextToWrite.Length > 0) {
                        // type the next correct character
                        TypeNextChar();
                        // cancel real keypress
                        return false;
                    }
                } else if (MainStatus.Active) {
                    // interception is active but the key pressed was not in standard english alphabet
                    switch (KeyStroke.Code) {
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
