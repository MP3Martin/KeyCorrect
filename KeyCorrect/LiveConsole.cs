using Spectre.Console;

namespace KeyCorrect {
    internal static class LiveConsole {
        internal static void Run() {
            Console.Clear();
            AnsiConsole.Live(new Markup("Loading..."))
                .StartAsync(async ctx => {
                    while (true) {
                        if (MainStatus.StopUpdatingText) {
                            Console.Clear();
                            return;
                        }
                        const int maxShownTextToWriteLen = 60;
                        var textToWrite = MainStatus.TextToWriteStable;
                        if (textToWrite.Length > maxShownTextToWriteLen) {
                            textToWrite = textToWrite[..(maxShownTextToWriteLen - 3)] + "...";
                        }

                        #region Warnings
                        Markup UnsupportedCharWarning() {
                            if (!DoesStringOnlyContainSupportedCharacters(MainStatus.TextToWriteStable)) {
                                return new("[gold3]Warning:[/] [indianred1]This program only supports typing english " +
                                    "letters!\nBut there is a special support for Czech QWERTZ layout.[/]");
                            }
                            return new("");
                        }

                        Markup InvisibleCharactersNote(string text) {
                            if (text.Contains('╝') || text.Contains('»')) {
                                return new("[gold3]Note:[/] [lightsalmon3_1]Symbol \"╝\" means enter and \"»\" means tab. [/]");
                            }
                            return new("");
                        }
                        #endregion

                        var textToWriteStableEscaped = EscapeString(textToWrite);
                        // calculate how much of the text was already written
                        var textWrittenLen = EscapeString(MainStatus.TextToWriteStable).Length - EscapeString(MainStatus.TextToWrite).Length;
                        string textToWriteLeftPart;
                        string textToWriteRightPart;
                        try {
                            textToWriteLeftPart = $"{textToWriteStableEscaped[..Math.Max(0, textWrittenLen)]}";
                        }
                        catch {
                            textToWriteLeftPart = textToWriteStableEscaped;
                        }
                        try {
                            textToWriteRightPart = $"{textToWriteStableEscaped[Math.Max(0, textWrittenLen)..]}";
                        }
                        catch {
                            textToWriteRightPart = "";
                        }

                        #region Panel
                        var panel = new Panel(new Rows(
                            new Markup($"[gold3]Intercept writing:[/] " +
                                $"[{(MainStatus.Active ? "green" : "red")}]{(MainStatus.Active ? "active" : "inactive")}[/]"),
                            new Markup($"[gold3]Text to write:[/] [#9a6f32]{textToWriteLeftPart}[/][darkorange]{textToWriteRightPart}[/]"),
                            new Markup(""),
                            InvisibleCharactersNote(textToWriteStableEscaped),
                            new Markup(""),
                            UnsupportedCharWarning()
                        ));
                        #endregion

                        panel.Header = new("[blue]Status[/]");
                        panel.Border = BoxBorder.Rounded;
                        panel.BorderStyle = new(Color.DarkCyan);
                        panel.Width = maxShownTextToWriteLen + 20;

                        #region Rows
                        ctx.UpdateTarget(
                            new Rows(
                                panel,
                                new Markup(" "),
                                new Markup("[darkslategray2]Info: [/][lightsalmon3_1]When you enable [lightskyblue1]Intercept " +
                                    "writing[/], this program will get your clipboard contents. It will type\nthe next correct " +
                                    "letter from your clipboard when you press any letter found in standard english alphabet.[/]"),
                                new Markup(" "),
                                new Markup("[gold3]Press [magenta2]PageUp[/] to toggle [lightskyblue1]Intercept " +
                                    "writing[/].[/]"),
                                new Markup(" "),
                                new Markup("[gold3]Press [magenta2]X[/] or [magenta2]Q[/] while this " +
                                    "window is active to [lightskyblue1]Stop[/] the program.[/]")
                            )
                        );
                        #endregion

                        ctx.Refresh();
                        await Task.Delay(10);
                    }
                });
        }
    }
}
