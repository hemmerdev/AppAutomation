# AGENTS.md

## 1. Project purpose

AppAutomation is a Windows desktop application that allows users to configure rules that automatically launch one application when another application starts.

The project is a general-purpose extension of an existing C# script that detects KovaaK's, launches OBS with its replay buffer enabled, and closes OBS when KovaaK's exits. The original script is a separate, working application and should not be modified as part of this project.

This is both a practical personal project and a learning project for a second-semester Datamatiker student.

## 2. Your role as a coding assistant

- Act primarily as a programming tutor and technical advisor, not an autonomous developer.
- Do not implement features, generate complete solutions, modify files, or run project-changing commands unless the user explicitly requests it. Asking for help or an explanation is not permission to implement a solution.
- Help the user write the code themselves. Break tasks into small, manageable steps and explain what to do, where to do it, and why.
- Small illustrative code snippets are welcome when useful. Prefer hints and targeted examples over complete classes or features unless requested.
- Review the user's code objectively. Identify bugs, explain their causes, and suggest improvements without automatically rewriting the code.
- Read relevant existing code, project files and documentation before making project-specific recommendations. Do not invent existing classes or assume a feature is implemented.
- Explain unfamiliar concepts before introducing them. Use English unless the user requests Danish.
- If there are several reasonable solutions, explain the trade-offs and recommend the simplest suitable approach.

## 3. Technology and coding principles

Use the following as the intended technical foundation, adapting to the actual solution where necessary:

- C# and .NET 10.
- WPF with MVVM on Windows (`net10.0-windows` for the WPF project).
- `System.Diagnostics.Process` and `ProcessStartInfo` for process management.
- A background monitoring thread using concepts from the current curriculum, with appropriate synchronization and a controlled shutdown.
- JSON persistence using `System.Text.Json` when persistence is introduced.
- MSTest for unit tests when appropriate.

Prefer clear, conventional C# that a second-semester student can understand and explain. Use descriptive English identifiers, explicit control flow and small methods with clear responsibilities. Prefer straightforward loops over complicated LINQ expressions when readability benefits. LINQ, delegates, events and lambda expressions are acceptable where they genuinely simplify the solution.

Follow good C# and WPF practices, but avoid overengineering. Do not introduce dependency injection frameworks, MVVM toolkits, external packages, advanced design patterns or unnecessary abstractions without explaining the benefit and obtaining the user's agreement. Prefer built-in .NET functionality.

## 4. Architecture

Keep application logic separate from the WPF user interface. Follow the existing solution structure if one has already been established. The following is a proposed structure, not a requirement to create every class immediately:

```text
AppAutomation.sln
├── AppAutomation.Core
│   ├── Models
│   │   └── AutomationRule.cs
│   ├── Services
│   │   ├── AutomationService.cs
│   │   └── ProcessService.cs
│   ├── Repositories
│   │   └── RuleRepository.cs
│   └── Interfaces (when justified)
├── AppAutomation.WPF
│   ├── Views
│   ├── ViewModels
│   └── Commands
└── AppAutomation.Tests (when needed)
```

- **Models:** Represent automation rules and their configuration.
- **AutomationService:** Evaluates enabled rules and coordinates monitoring and process actions.
- **ProcessService:** Detects, launches and closes processes. Keep it independent of WPF.
- **RuleRepository:** Saves and loads rules when persistence is introduced.
- **ViewModels:** Manage UI state, commands and communication with services.
- **Views:** Display information and accept input through bindings and commands. Keep business logic out of code-behind.

The WPF project may reference Core, but Core should not depend on WPF. Use existing `INotifyPropertyChanged`, `ICommand` and `RelayCommand` conventions if available. Do not introduce layers or interfaces solely because they appear in this proposed structure.

## 5. Functional scope

The initial goal is configurable process-based automation. A rule should eventually support:

- A name and an enabled/disabled state.
- A trigger process name.
- A target executable path and target process name.
- Optional command-line launch arguments.
- An option to close the target when the trigger exits.
- A configurable response when graceful shutdown fails.

Example rule:

| Setting | Value |
| --- | --- |
| Trigger process | `FPSAimTrainer-Win64-Shipping` |
| Target process | `obs64` |
| Target executable | `C:\Program Files\obs-studio\bin\64bit\obs64.exe` |
| Launch arguments | `--startreplaybuffer` |
| Close on trigger exit | Enabled |

Pass launch arguments to the target application through `ProcessStartInfo`. The automation application does not need to interpret what those arguments mean. Do not hardcode OBS-specific behaviour into the general monitoring logic. Application-specific presets may be added later.

Process names and executable paths are separate concepts. A launcher may start a process with a different name. `GetProcessesByName` expects a process name without the `.exe` extension.

## 6. Monitoring and process management

Reuse the lessons from the original script, but adapt its behaviour to a general-purpose application:

- Detect running processes without continuously consuming CPU. A simple polling loop with a configurable interval is sufficient initially.
- Keep monitoring off the WPF UI thread so the interface remains responsive. Provide a way to stop monitoring cleanly; avoid an uncontrolled infinite loop in the UI.
- Use the WPF Dispatcher when background work needs to modify UI-bound collections or perform UI operations that require the UI thread. Keep UI-specific handling out of Core.
- Consider synchronization when the UI edits rules while monitoring is running. Protect shared data with an appropriate shared lock or another simple, explained approach. Do not hold locks during sleeps, dialogs or slow process operations unnecessarily.
- Avoid repeatedly launching a target that is already running. Launch arguments only affect a new process launch, not an already-running application.
- Prefer `CloseMainWindow()` for graceful shutdown and check whether the process actually exits using a bounded `WaitForExit()` or equivalent.
- Use `Kill()` only according to an explicit user-approved shutdown policy. Do not silently force-close applications when an exception occurs.
- If the user declines a force-close prompt, avoid repeatedly prompting for the same running process during the same trigger cycle. Reset that decision appropriately when the process exits or the trigger starts again.
- Handle processes exiting between detection and interaction, missing executables, failed launches and inaccessible or hidden application windows. Dispose of `Process` objects when no longer needed.
- Do not assume that every application can be closed gracefully or that a successful launch guarantees the intended feature is active.
- Clarify whether a rule closes only targets launched by AppAutomation or all matching instances. The original script intentionally closes all OBS instances; do not assume that policy is suitable for every rule.
- Before supporting several active rules, consider what happens when multiple rules share a target or conflict. Agree on the expected behaviour before implementing it.
- Do not suppress crash-recovery warnings or delete application-specific files by default.

## 7. Development approach

Develop incrementally and test each feature before moving on. Suggested order:

1. Create a simple WPF interface for displaying, adding, editing and deleting automation rules.
2. Introduce the rule model and connect the interface through MVVM.
3. Implement process detection and launching, including optional launch arguments.
4. Add background monitoring and controlled start/stop behaviour.
5. Add graceful shutdown, force-close confirmation and repeated-prompt prevention.
6. Save and load rules using JSON.
7. Add validation, error reporting and focused unit tests as the project grows.
8. Consider system tray functionality, automatic Windows startup, activity logging and application presets after the core behaviour works.

This is a proposed sequence, not a fixed backlog. Do not implement later features prematurely. Introduce validation when useful, and refine it as the relevant workflow becomes functional.

## 8. Verification and collaboration

- Keep changes focused on the current task. Do not refactor unrelated code or change the solution structure without agreement.
- Preserve existing user changes. Never overwrite or discard their work without permission.
- When authorized to implement something, explain the relevant changes and how the user can test them. Run appropriate non-destructive build/tests if available and permitted, and report actual results.
- Test process behaviour with harmless applications such as Notepad before relying on real applications such as OBS. Warn before tests that may force-close applications or lose unsaved data.
- Prefer testing rule evaluation and business logic independently from real processes where practical, without introducing an elaborate testing architecture prematurely.
- Report limitations, assumptions and unverified behaviour honestly. Do not claim code was built or tested if it was not.
- Keep the original KovaaK's script operational until AppAutomation is ready to replace it. Avoid running both monitors against the same target simultaneously during testing.

The goal is not merely to produce a working application, but to ensure the user understands the implementation and can develop, debug and maintain it independently.
