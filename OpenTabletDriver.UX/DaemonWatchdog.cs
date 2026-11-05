using System;
using System.Diagnostics;
using System.IO;
using OpenTabletDriver.Interop;
using OpenTabletDriver.Plugin;

namespace OpenTabletDriver.UX
{
    public sealed class DaemonWatchdog : IDisposable
    {
        public event EventHandler? DaemonExited;

        private Process? daemonProcess;

        // Set by frontends that combine the GUI and the daemon into a single executable. When set,
        // and no standalone daemon binary is installed next to us, we restart ourselves as the backend.
        public static string? SelfHostArgument { get; set; }

        private readonly static ProcessStartInfo startInfo = SystemInterop.CurrentPlatform switch
        {
            PluginPlatform.Windows => CreateWindowsStartInfo(),
            PluginPlatform.MacOS => new ProcessStartInfo
            {
                FileName = Path.Join(AppContext.BaseDirectory, "OpenTabletDriver.Daemon")
            },
            _ => new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = Path.Join(AppContext.BaseDirectory, "OpenTabletDriver.Daemon.dll")
            }
        };

        private static ProcessStartInfo CreateWindowsStartInfo()
        {
            var standalone = Path.Join(AppContext.BaseDirectory, "OpenTabletDriver.Daemon.exe");

            if (!File.Exists(standalone) && SelfHostArgument != null && Environment.ProcessPath != null)
            {
                return new ProcessStartInfo
                {
                    FileName = Environment.ProcessPath,
                    Arguments = SelfHostArgument,
                    WorkingDirectory = AppContext.BaseDirectory,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
            }

            return new ProcessStartInfo
            {
                FileName = standalone,
                Arguments = "",
                WorkingDirectory = AppContext.BaseDirectory,
                CreateNoWindow = true
            };
        }

        public static bool CanExecute =>
            File.Exists(startInfo.FileName) ||
            File.Exists(startInfo.Arguments);

        public void Start()
        {
            this.daemonProcess = new Process()
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };
            this.daemonProcess.Exited += (_, e) =>
            {
                DaemonExited?.Invoke(this, EventArgs.Empty);
            };
            this.daemonProcess.Start();
        }

        public void Stop()
        {
            DaemonExited = null; // unregister all event handlers
            this.daemonProcess?.Kill();
            this.daemonProcess?.Dispose();
            this.daemonProcess = null;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
