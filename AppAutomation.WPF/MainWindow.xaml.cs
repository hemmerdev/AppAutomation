using AppAutomation.Core;
using AppAutomation.WPF.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace AppAutomation.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var viewModel = new MainWindowViewModel(new ObservableCollection<AutomationRule>()
            {
                new AutomationRule() { Name = "Rule 1", TriggerProcessName = "ProcessA", TargetProcessName = "ProcessB", IsEnabled = true },
                new AutomationRule() { Name = "Rule 2", TriggerProcessName = "ProcessC", TargetProcessName = "ProcessD", IsEnabled = false },
            });
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}