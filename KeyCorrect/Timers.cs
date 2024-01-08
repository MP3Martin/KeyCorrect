namespace KeyCorrect {
    internal static class Timers {
        internal static readonly MyTimer ClipboardTimer = new(() => {
            if (MainStatus.Active) {
                return;
            }
            string? ClipboardText;
            try {
                ClipboardText = TextCopy.ClipboardService.GetText();
                ClipboardText ??= string.Empty;
            } catch (Exception) {
                ClipboardText = string.Empty;
            }
            MainStatus.TextToWrite = ClipboardText;
        }, 150);

        internal static readonly MyTimer KeyboardLayoutTimer = new(() => {
            MainStatus.KeyboardLayout = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower();
        }, 1000);
    }
}
