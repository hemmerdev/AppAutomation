using AppAutomation.Core.Models;

namespace AppAutomation.Core.Services
{
    public class AutomationService
    {
        public static bool TryRunRule(AutomationRule rule)
        {
            if (!rule.IsEnabled ||
                !ProcessService.IsRunning(rule.TriggerProcessName) ||
                ProcessService.IsRunning(rule.TargetProcessName))
                return false;
            else
                return ProcessService.TryStartProcess(rule.TargetExecutablePath, rule.LaunchArguments);
        }
    }
}
