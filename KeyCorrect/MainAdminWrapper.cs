using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;

namespace KeyCorrect {
    [SuppressMessage("Interoperability", "CA1416")]
    internal static class MainAdminWrapper {
        // thanks to http://antscode.blogspot.com.au/2011/02/running-clickonce-application-as.html
        public static void Main() {

            if (InputInterceptor.CheckDriverInstalled()) {
                // the driver is already installed
                RunMainProgram();
            } else if (!IsRunAsAdministrator()) {
                // It is not possible to launch a ClickOnce app as administrator directly, so instead we launch the
                // app as administrator in a new process.
                var sourcePath = Environment.ProcessPath;
                if (sourcePath!.EndsWith(".dll")) {
                    sourcePath = sourcePath[..^4] + ".exe";
                }

                var processInfo = new ProcessStartInfo(sourcePath) {
                    // The following properties run the new process as administrator
                    UseShellExecute = true,
                    Verb = "runas"
                };

                // Start the new process
                try {
                    Process.Start(processInfo);
                } catch (Exception ex) {
                    // The user did not allow the application to run as administrator
                    Console.WriteLine("Sorry, this application must be run as Administrator because of the keyboard driver. Error code:\n");
                    Console.WriteLine(ex);
                    Console.WriteLine("\nPress any key to exit . . . ");
                    Console.ReadKey(true);
                }

                // Shut down the current process
                Environment.Exit(0);
            } else {
                // We are running as administrator
                RunMainProgram();
            }

            return;

            bool IsRunAsAdministrator() {
                var wi = WindowsIdentity.GetCurrent();
                var wp = new WindowsPrincipal(wi);

                return wp.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}
