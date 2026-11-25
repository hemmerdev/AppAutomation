using AppAutomation.Core;
using AppAutomation.WPF.Commands;
using System.Collections.ObjectModel;

namespace AppAutomation.WPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private AutomationRule _draft = new();
        private bool _isRuleFormOpen;
        private string _validationMessage = "";
        private AutomationRule? _selectedRule;
        private AutomationRule? _ruleBeingEdited;

        public ObservableCollection<AutomationRule> AutomationRules { get; }


        public AutomationRule Draft
        {
            get => _draft;
            set
            {
                if (_draft != value)
                {
                    _draft = value;
                    OnPropertyChanged();
                }
            }
        }
        public AutomationRule? SelectedRule
        {
            get => _selectedRule;
            set
            {
                if (_selectedRule != value)
                {
                    _selectedRule = value;
                    OnPropertyChanged();
                    EditRuleCommand.RaiseCanExecuteChanged();
                    DeleteRuleCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsRuleFormOpen
        {
            get => _isRuleFormOpen;
            set
            {
                if (_isRuleFormOpen != value)
                {
                    _isRuleFormOpen = value;
                    OnPropertyChanged();
                    SaveRuleCommand.RaiseCanExecuteChanged();
                    CancelRuleCommand.RaiseCanExecuteChanged();
                    AddRuleCommand.RaiseCanExecuteChanged();
                    EditRuleCommand.RaiseCanExecuteChanged();
                    DeleteRuleCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string ValidationMessage
        {
            get => _validationMessage;
            private set
            {
                if (_validationMessage != value)
                {
                    _validationMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand AddRuleCommand { get; }
        public RelayCommand SaveRuleCommand { get; }
        public RelayCommand CancelRuleCommand { get; }
        public RelayCommand EditRuleCommand { get; }
        public RelayCommand DeleteRuleCommand { get; }

        public MainWindowViewModel(ObservableCollection<AutomationRule> automationRules)
        {
            AutomationRules = automationRules;
            AddRuleCommand = new RelayCommand(
                _ => IsRuleFormOpen = true,
                _ => !IsRuleFormOpen);
            SaveRuleCommand = new RelayCommand(
                _ => SaveRule(),
                _ => IsRuleFormOpen);
            CancelRuleCommand = new RelayCommand(
                _ => CloseRuleForm(),
                _ => IsRuleFormOpen);
            EditRuleCommand = new RelayCommand(
                _ => EditRule(),
                _ => SelectedRule is not null && !IsRuleFormOpen);
            DeleteRuleCommand = new RelayCommand(
                _ => DeleteRule(),
                _ => SelectedRule is not null && !IsRuleFormOpen);
        }

        private void DeleteRule()
        {
            var rule = SelectedRule;
            if (rule is null || IsRuleFormOpen)
                return;

            AutomationRules.Remove(rule);
            SelectedRule = null;
        }

        private void EditRule()
        {
            var rule = SelectedRule;
            if (rule is null || IsRuleFormOpen)
                return;

            _ruleBeingEdited = rule;

            Draft = new AutomationRule
            {
                Name = rule.Name,
                TriggerProcessName = rule.TriggerProcessName,
                TargetProcessName = rule.TargetProcessName,
                IsEnabled = rule.IsEnabled
            };

            IsRuleFormOpen = true;
        }

        private void SaveRule()
        {
            if (string.IsNullOrWhiteSpace(Draft.Name) ||
                string.IsNullOrWhiteSpace(Draft.TriggerProcessName) ||
                string.IsNullOrWhiteSpace(Draft.TargetProcessName))
            {
                ValidationMessage = "All fields are required.";
                return;
            }

            // If _ruleBeingEdited is null, it means we are adding a new rule. Otherwise, we are editing an existing rule.
            if (_ruleBeingEdited is null)
            {
                AutomationRules.Add(Draft);
            }
            else
            {
                int i = AutomationRules.IndexOf(_ruleBeingEdited);
                AutomationRules[i] = Draft;
            }

            CloseRuleForm();
        }

        private void CloseRuleForm()
        {
            IsRuleFormOpen = false;
            _ruleBeingEdited = null;
            Draft = new AutomationRule();
            ValidationMessage = "";
        }
    }
}
