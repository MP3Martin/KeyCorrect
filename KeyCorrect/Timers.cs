namespace KeyCorrect {
    internal static class Timers {
        internal class ClipboardTimer {
            internal static void Start() {
                CreateTimer(() => {
                    if (MainStatus.Active) {
                        return;
                    }
                    string ClipboardText = TextCopy.ClipboardService.GetText();
                    try {
                        if (ClipboardText == null) {
                            ClipboardText = string.Empty;
                        }
                    } catch (Exception ex) {
                        ClipboardText = string.Empty;
                    }
                    MainStatus.TextToWrite = ClipboardText;
                }, 200);
            }
        }

        internal class KeyboardLayoutTimer {
            internal static void Start() {
                CreateTimer(() => {
                    MainStatus.KeyboardLayout = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower();
                }, 1 * 1000);
            }
        }
    }
}
