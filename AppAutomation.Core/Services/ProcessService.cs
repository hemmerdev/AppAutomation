using System.ComponentModel;
using System.Diagnostics;
// This service is reponsible for communicating with the operating system to check if a process is running and to start a process.
namespace AppAutomation.Core.Services
{
    public class ProcessService
    {
        public static bool IsRunning(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName))
            {
                return false;
            }

            Process[] processes = Process.GetProcessesByName(processName);

            bool isRunning = processes.Length > 0;

            foreach (var p in processes)
            {
                p.Dispose();
            }

            return isRunning;
        }

        public static bool TryStartProcess(string executablePath, string arguments = "")
        {
            if (string.IsNullOrWhiteSpace(executablePath) ||
                !File.Exists(executablePath))
                return false;

            string fullPath = Path.GetFullPath(executablePath);

            var processInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = arguments,
                WorkingDirectory = Path.GetDirectoryName(fullPath) ?? ""
            };
            try
            {

                using var process = Process.Start(processInfo);
                return process is not null;

            }
            catch (Win32Exception)
            {
                return false;
            }
        }
    }
}
