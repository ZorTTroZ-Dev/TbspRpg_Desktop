using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;
using TbspRpgProcessor;
using TbspRpgProcessor.Entities;
using TbspRpgSettings.Settings;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class ScriptEditViewModel: ViewModelBase
{
    public ObservableCollection<ScriptViewModel> Scripts { get; } = new();
    [ObservableProperty] private ScriptViewModel _selectedScript;
    private readonly string _scriptDestination;
    private readonly int _adventureId;

    private ScriptEditViewModel(string scriptDestination, int adventureId)
    {
        _scriptDestination = scriptDestination;
        _adventureId = adventureId;
        
        WeakReferenceMessenger.Default.Register<ScriptEditViewModel, ScriptEditAddScript>(this, (vm, m) =>
        {
            if (m.Script != null)
                vm.Scripts.Insert(vm.Scripts.Count - 1, new ScriptViewModel(
                    m.Script, false, false, null));
            else
                vm.Scripts.Insert(vm.Scripts.Count - 1, new ScriptViewModel(
                    NewScript(m.Name), false, false, null));
            
        });
        
        WeakReferenceMessenger.Default.Register<ScriptEditViewModel, ScriptEditNewPrimary>(this, (vm, m) => 
        {
            int newPrimaryScriptIndex = 0;
            ScriptViewModel? newPrimaryScript = null;
            // Is a name match good enough, I hope so
            int index = 0;
            foreach (var script in vm.Scripts)
            {
                if (script.IsNewTab) continue;
                if (script.GetScript().Name == m.Script.Name)
                {
                    newPrimaryScriptIndex = index;
                    newPrimaryScript = script;
                    break;
                }
                script.Primary = false;
                index++;
            }

            if (newPrimaryScript == null) return;
            if (newPrimaryScriptIndex != 0)
            {
                Scripts.RemoveAt(newPrimaryScriptIndex);
                Scripts.Insert(0, newPrimaryScript);
            }
            SelectedScript = newPrimaryScript;
        });
    }

    public static async Task<ScriptEditViewModel> CreateAsync(int scriptId, int adventureId, string scriptDestination)
    {
        var instance = new ScriptEditViewModel(scriptDestination, adventureId);
        await instance.LoadScript(scriptId, adventureId);
        return instance;
    }

    private Script NewScript(string? name = null)
    {
        return new Script()
        {
            Name = name ?? "new script",
            Id = 0,
            Type = ScriptTypes.LuaScript,
            AdventureId = _adventureId,
            Content = """
                      function run() 

                      end
                      """
        };
    }
    
    private async Task LoadScript(int scriptId, int adventureId)
    {
        var script = await TbspRpgDataServiceFactory.Load().ScriptsService.GetScriptById(scriptId) ?? NewScript();
        
        // put the scripts in a list for tabs
        Scripts.Add(new  ScriptViewModel(script, true, false, null));

        // add the new script tab
        var adventureScripts = await TbspRpgDataServiceFactory.Load().ScriptsService.GetScriptsForAdventure(adventureId);
        Scripts.Add(new ScriptViewModel(new Script()
        {
            Id = -1,
            Name = "+++",
            Content = ""
        }, false, false, adventureScripts));
    }
    
    [RelayCommand]
    private void CloseEdit()
    {
        WeakReferenceMessenger.Default.Send(new AdventureEditCancelPaneMessage());
    }

    private async Task<Script?> SaveScript()
    {
        Script? returnScript = null;
        try
        {
            var scriptProcessor = TbspRpgProcessorFactory.TbspRpgProcessor();
            // we need to get the list of script that have changed either in content or in name
            foreach (var script in Scripts)
            {
                if (script.HasChanged() || (script.Primary && script.GetScript().Id == 0))
                {
                    var scriptObj = await scriptProcessor.UpdateScript(new ScriptUpdateModel()
                    {
                        script = script.UpdatedScript
                    });
                    WeakReferenceMessenger.Default.Send(new NotificationMessage($"{scriptObj.Name} script saved", NotificationType.Success));
                    script.UpdateScript(scriptObj);
                }

                if (script.Primary)
                    returnScript = script.GetScript();
            }
        }
        catch (Exception e)
        {
            WeakReferenceMessenger.Default.Send(new NotificationMessage(e.Message, NotificationType.Error));
        }
        return returnScript;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var returnScript = await SaveScript();
        WeakReferenceMessenger.Default.Send(new AdventureEditScriptChangedMessage(
            returnScript, _scriptDestination, false));
    }
    
    [RelayCommand]
    private async Task SaveCloseAsync()
    {
        var returnScript = await SaveScript();
        WeakReferenceMessenger.Default.Send(new AdventureEditScriptChangedMessage(
            returnScript, _scriptDestination, true));
    }
}