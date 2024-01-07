namespace KeyCorrect {
    internal static class KeyboardCallbackClass {
        internal static bool KeyboardCallback(ref KeyStroke KeyStroke) {
            //Console.WriteLine($"{KeyStroke.Code} {KeyStroke.State} {KeyStroke.Information}");
            if (MainStatus.IgnoreAllKeyPressesButStillSendThem) {
                // allow the keypress but don't handle it
                return true;
            }
            // Check if pageUp is pressed
            if ((!Console.NumberLock && KeyStroke.Code == KeyCode.Numpad9) || (KeyStroke.Code == KeyCode.Numpad9 && (KeyStroke.State == KeyState.E0 || KeyStroke.State == (KeyState.E0 | KeyState.Up)))) {
                return HandleNumLockPress(ref KeyStroke);
            } else {
                if (HandleNonNumLockPress(KeyStroke) is bool forwardKeyStroke) {
                    return forwardKeyStroke;
                }
            }
            if (MainStatus.TextToWrite.Length <= 0 && MainStatus.Active) {
                // cancel the keypress if done writing
                return false;
            }
            // allow the keypress by default
            return true;
        }

        private static bool? HandleNonNumLockPress(KeyStroke KeyStroke) {
            if (KeyStrokeTriggersNextChar(KeyStroke)) {
                // if the pressed key can type the next character
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
                    KeyCode.Control or KeyCode.Delete:
                        return true;
                    default:
                        return false;
                }
            }
            return null;
        }

        private static bool HandleNumLockPress(ref KeyStroke KeyStroke) {
            // Fix issues with pageUp being special key
            if (KeyStroke.State == (KeyState.E0 | KeyState.Up)) {
                KeyStroke.State = KeyState.Up;
            } else if (KeyStroke.State == KeyState.E0) {
                KeyStroke.State = KeyState.Down;
            }

            // Check if the key is released
            if (KeyStroke.State == KeyState.Up) {
                // toggle active state
                MainStatus.Active = !MainStatus.Active;
            }

            // cancel real keypress
            return false;
        }
    }
}
