using System.Diagnostics.CodeAnalysis;
using static InputInterceptorNS.KeyCode;
using static InputInterceptorNS.KeyState;

namespace KeyCorrect {
    [SuppressMessage("Interoperability", "CA1416")]
    internal static class KeyboardCallbackClass {
        internal static bool KeyboardCallback(ref KeyStroke keyStroke) {
            //Console.WriteLine($"{KeyStroke.Code} {KeyStroke.State} {KeyStroke.Information}");
            if (MainStatus.IgnoreAllKeyPressesButStillSendThem) {
                // allow the keypress but don't handle it
                return true;
            }
            // Check if pageUp is pressed
            if (!Console.NumberLock && keyStroke.Code == Numpad9 ||
                keyStroke is { Code: Numpad9, State: E0 or (E0 | KeyState.Up) }) {
                return HandleNumLockPress(ref keyStroke);
            }
            if (HandleNonNumLockPress(keyStroke) is { } forwardKeyStroke) {
                return forwardKeyStroke;
            }
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (MainStatus.TextToWrite.Length <= 0 && MainStatus.Active) {
                // cancel the keypress if done writing
                return false;
            }
            // allow the keypress by default
            return true;
        }

        private static bool? HandleNonNumLockPress(KeyStroke keyStroke) {
            if (KeyStrokeTriggersNextChar(keyStroke)) {
                // if the pressed key can type the next character
                // ReSharper disable once InvertIf
                if (MainStatus.Active && keyStroke.State == KeyState.Down && MainStatus.TextToWrite.Length > 0) {
                    // type the next correct character
                    TypeNextChar();
                    // cancel real keypress
                    return false;
                }
            } else if (MainStatus.Active) {
                // interception is active but the key pressed was not in standard english alphabet
                return keyStroke.Code switch {
                    LeftWindowsKey or RightWindowsKey or Alt or Tab or Control or Delete => true,
                    _ => false
                };
            }
            return null;
        }

        private static bool HandleNumLockPress(ref KeyStroke keyStroke) {
            keyStroke.State = keyStroke.State switch {
                // Fix issues with pageUp being special key
                E0 | KeyState.Up => KeyState.Up,
                E0 => KeyState.Down,
                _ => keyStroke.State
            };

            // Check if the key is released
            if (keyStroke.State != KeyState.Up) return false;
            // toggle active state
            MainStatus.Active = !MainStatus.Active;

            // Update MainStatus.TextToWrite with clipboard text right when KeyCorrect is disabled (because
            // the first character of MainStatus.TextToWrite is removed every time a character is typed)
            if (MainStatus.Active) return false;
            // First set MainStatus.TextToWrite to last stable text just to be sure
            MainStatus.TextToWrite = MainStatus.TextToWriteStable;
            // text from clipboard
            Timers.ClipboardTimer.ToRun.Invoke();

            // cancel real keypress
            return false;
        }
    }
}
