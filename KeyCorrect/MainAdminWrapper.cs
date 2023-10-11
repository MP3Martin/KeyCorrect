using System.Diagnostics;
using System.Security.Principal;

namespace KeyCorrect {
    internal class MainAdminWrapper {
        // thanks to http://antscode.blogspot.com.au/2011/02/running-clickonce-application-as.html
        public static void Main(string[] args) {
            bool IsRunAsAdministrator() {
                var wi = WindowsIdentity.GetCurrent();
                var wp = new WindowsPrincipal(wi);

                return wp.IsInRole(WindowsBuiltInRole.Administrator);
            }

            if (!IsRunAsAdministrator()) {
                // It is not possible to launch a ClickOnce app as administrator directly, so instead we launch the
                // app as administrator in a new process.
                string SourcePath = Process.GetCurrentProcess().MainModule.FileName;
                if (SourcePath.EndsWith(".dll")) {
                    SourcePath = SourcePath[..(SourcePath.Length - 4)] + ".exe";
                }
                var ProcessInfo = new ProcessStartInfo(SourcePath);

                // The following properties run the new process as administrator
                ProcessInfo.UseShellExecute = true;
                ProcessInfo.Verb = "runas";

                // Start the new process
                try {
                    Process.Start(ProcessInfo);
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
        }
    }
}
