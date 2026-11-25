namespace AppAutomation.Core.Models
{
    public class AutomationRule
    {
        public string Name { get; set; } = "";
        public string TriggerProcessName { get; set; } = "";
        public string TargetProcessName { get; set; } = "";
        public bool IsEnabled { get; set; } = true;
        public string LaunchArguments { get; set; } = "";
        public string TargetExecutablePath { get; set; } = "";
    }
}
