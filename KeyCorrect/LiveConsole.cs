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
                    const int MAX_SHOWN_TEXT_TO_WRITE_LEN = 60;
                    string TextToWrite = MainStatus.TextToWriteStable;
                    if (TextToWrite.Length > MAX_SHOWN_TEXT_TO_WRITE_LEN) {
                        TextToWrite = TextToWrite.Substring(0, MAX_SHOWN_TEXT_TO_WRITE_LEN - 3) + "...";
                    }

                    Markup UnsupportedCharWarning() {
                        if (!DoesStringOnlyContainStandardLowercaseLetters(MainStatus.TextToWriteStable)) {
                            return new Markup("[gold3]Warning:[/] [indianred1]This program only supports typing english " +
                            "letters\nand most basic punctuation marks![/]");
                        }
                        return new Markup("");
                    }

                    string textToWriteStableEscaped = EscapeString(TextToWrite);
                    // calculate how much of the text was already written
                    int textWrittenLen = MainStatus.TextToWriteStable.Length - MainStatus.TextToWrite.Length;
                    string TextToWriteLeftPart;
                    string TextToWriteRightPart;
                    try {
                        TextToWriteLeftPart = $"{textToWriteStableEscaped[..textWrittenLen]}";
                    } catch (Exception) {
                        TextToWriteLeftPart = textToWriteStableEscaped;
                    }
                    try {
                        TextToWriteRightPart = $"{textToWriteStableEscaped[textWrittenLen..]}";
                    } catch (Exception) {
                        TextToWriteRightPart = "";
                    }

                    var panel = new Panel(new Rows(
                        new Markup($"[gold3]Intercept writing:[/] " +
                        $"[{(MainStatus.Active ? "green" : "red")}]{(MainStatus.Active ? "active" : "inactive")}[/]"),
                        new Markup($"[gold3]Text to write:[/] [#9a6f32]{TextToWriteLeftPart}[/][darkorange]{TextToWriteRightPart}[/]"),
                        new Markup(""),
                        UnsupportedCharWarning()
                    ));
                    panel.Header = new PanelHeader("[blue]Status[/]");
                    panel.Border = BoxBorder.Rounded;
                    panel.BorderStyle = new Style(Color.DarkCyan);
                    panel.Width = MAX_SHOWN_TEXT_TO_WRITE_LEN + 20;
                    ctx.UpdateTarget(
                        new Rows(
                            panel,
                            new Markup(" "),
                            new Markup("[darkslategray2]Info: [/][lightsalmon3_1]When you enable [lightskyblue1]Intercept " +
                            $"writing[/], this program will get your clipboard contents. It will type\nthe next correct " +
                            $"letter from your clipboard when you press any letter found in standard english alphabet.[/]"),
                            new Markup(" "),
                            new Markup("[gold3]Press [magenta2]PageUp[/] to toggle [lightskyblue1]Intercept " +
                            $"writing[/].[/]"),
                            new Markup(" "),
                            new Markup("[gold3]Press [magenta2]X[/] or [magenta2]Q[/] while this " +
                            $"window is active to [lightskyblue1]Stop[/] the program.[/]")
                        )
                    );
                    ctx.Refresh();
                    await Task.Delay(10);
                }
            });
        }
    }
}
