using System;
using System.Linq;
using System.Security.Principal;
using Eto.Forms;

namespace OpenTabletDriver.UX.Wpf
{
    class Program
    {
        private const string DAEMON_ARG = "--daemon";

        [STAThread]
        public static void Main(string[] args)
        {
            // Ships as a single executable: when asked to be the driver backend, run the daemon
            // in this same process so no separate OpenTabletDriver.Daemon.exe is required.
            if (args.Contains(DAEMON_ARG))
            {
                Daemon.Program.Main(args.Where(a => a != DAEMON_ARG).ToArray()).GetAwaiter().GetResult();
                return;
            }

            DaemonWatchdog.SelfHostArgument = DAEMON_ARG;

            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(identity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                _ = new Application(Eto.Platforms.Wpf);
                MessageBox.Show("OpenTabletDriver should not be run with administrator privileges.\n" +
                    "Some features may not work as intended, such as Plugin Manager and Tablet Debugger.\n" +
                    "If you did not manually set OpenTabletDriver to run with administrator privileges please enable UAC to resolve this.\n" +
                    "If it isn't resolved after, you are using the Administrator named account which overrides UAC.", MessageBoxType.Warning);
                Eto.Platform.AllowReinitialize = true;
            }

            App.Run(Eto.Platforms.Wpf, args);
        }
    }
}
