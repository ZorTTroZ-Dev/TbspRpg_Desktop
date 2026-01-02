using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class LuaScriptEditorViewModel(Script script, bool isPrimary): ViewModelBase
{
    private Script _script = script;
    public string EditorContent = script.Content;
    [ObservableProperty] private string _name = script.Name;
    [ObservableProperty] private bool _isPrimary = isPrimary;

    public void UpdateEditorContent(string updatedEditorContent)
    {
        if (EditorContent == updatedEditorContent) return;
        EditorContent = updatedEditorContent;
        WeakReferenceMessenger.Default.Send(new ScriptModifiedMessage());
    }

    partial void OnNameChanged(string value)
    {
        WeakReferenceMessenger.Default.Send(new ScriptModifiedMessage());
    }

    partial void OnIsPrimaryChanged(bool value)
    {
        if (value)
            WeakReferenceMessenger.Default.Send(new ScriptEditNewPrimary(_script));
    }
}