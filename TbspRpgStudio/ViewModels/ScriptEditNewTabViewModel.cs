using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class ScriptEditNewTabViewModel(List<Script> scripts): ViewModelBase
{
    [ObservableProperty] private string _newScriptName = String.Empty;
    [ObservableProperty] private List<Script> _filteredScripts = scripts;
    [ObservableProperty] private Script? _selectedScript;
    [ObservableProperty] private string _scriptNameFilter = "";
    private readonly List<Script> _scripts = scripts;

    partial void OnScriptNameFilterChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            FilteredScripts = _scripts.Where(s => s.Name.ToLower().Contains(value.ToLower())).ToList();
        else
            FilteredScripts = _scripts;
    }

    [RelayCommand]
    private void AddScript()
    {
        if (SelectedScript == null) return;
        WeakReferenceMessenger.Default.Send(new ScriptEditAddScript(SelectedScript));
    }
    
    [RelayCommand]
    private void NewScript()
    {
        WeakReferenceMessenger.Default.Send(new ScriptEditAddScript(null, NewScriptName));
    }
}