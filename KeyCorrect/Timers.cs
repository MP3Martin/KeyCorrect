namespace KeyCorrect {
    internal static class Timers {
        internal class ClipboardTimer {
            internal static void Run() {
                System.Timers.Timer GetClipboardTimer = new System.Timers.Timer();
                GetClipboardTimer.Elapsed += new System.Timers.ElapsedEventHandler((source, e) => {
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
                });
                GetClipboardTimer.Interval = 200;
                GetClipboardTimer.Enabled = true;
            }

        }
    }
}
