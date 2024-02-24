using TextCopy;

namespace KeyCorrect {
    internal static class Timers {
        internal static readonly MyTimer ClipboardTimer = new(() => {
            if (MainStatus.Active) {
                return;
            }
            string? clipboardText;
            try {
                clipboardText = ClipboardService.GetText();
                clipboardText ??= string.Empty;
            }
            catch {
                clipboardText = string.Empty;
            }
            MainStatus.TextToWrite = clipboardText;
        }, 150);

        internal static readonly MyTimer KeyboardLayoutTimer = new(() => {
            MainStatus.KeyboardLayout = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower();
        }, 1000);
    }
}
