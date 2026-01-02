using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities;
using TbspRpgSettings.Settings;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class ScriptEditLinkViewModel : ViewModelBase
{
    private Script?  _script;
    private readonly string _scriptDestination;
    
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _header;
    
    private ScriptEditLinkViewModel(string scriptDestination)
    {
        _scriptDestination = scriptDestination;
    }

    public static async Task<ScriptEditLinkViewModel> CreateAsync(int? scriptId, int adventureId, string  scriptDestination)
    {
        var instance = new ScriptEditLinkViewModel(scriptDestination);
        if (scriptId != null)
        {
            instance._script = await TbspRpgDataServiceFactory.Load().ScriptsService.GetScriptById(scriptId.Value);
        }
        else
        {
            instance._script = new Script()
            {
                Name = "Empty",
                Id = 0,
                AdventureId = adventureId,
                Type = ScriptTypes.LuaScript,
                Content = ""
            };
        }
        instance.Name = instance._script.Name;
        instance.Header = $"{instance._script.Id} - {instance._script.Type}";

        return instance;
    }
    
    [RelayCommand]
    public async Task EditScriptAsync()
    {
        if (_script != null)
        {
            var scriptEditViewModel =
                await ScriptEditViewModel.CreateAsync(_script.Id, _script.AdventureId, _scriptDestination);
            WeakReferenceMessenger.Default.Send(new ScriptEditMessage(scriptEditViewModel));
        }
    }
}