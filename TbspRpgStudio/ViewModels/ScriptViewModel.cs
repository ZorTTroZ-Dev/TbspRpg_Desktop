using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class ScriptViewModel:  ViewModelBase
{
    private Script _script;
    private string _name;
    [ObservableProperty] private string _header;
    [ObservableProperty] private ViewModelBase _viewModel;

    public ScriptViewModel(Script script, bool isPrimary, bool isInclude, List<Script>? scripts)
    {
        _script = script;
        _name =  _script.Name;
        Header = _name;

        if (IsNewTab && scripts != null)
            ViewModel = new ScriptEditNewTabViewModel(scripts);
        else if (IsCopyTab)
            ViewModel = new CopyEditViewModel(Guid.Empty, "", true, true);
        else
            ViewModel = new LuaScriptEditorViewModel(_script, isPrimary);
        
        WeakReferenceMessenger.Default.Register<ScriptViewModel, ScriptModifiedMessage>(this, (w, m) =>
        {
            if (IsSpecialTab) return;
            var viewModel = (LuaScriptEditorViewModel)ViewModel;
            if (_name != viewModel.Name)
            {
                _name = viewModel.Name;
                Header = _name;
            }

            if (_script.Content != viewModel.EditorContent)
            {
                if (Header[0] != '*')
                    Header = '*' + Header;
            }
            else
            {
                if (Header.Length > 0 && Header[0] == '*')
                    Header = Header.Substring(1);
            }
        });
    }

    public bool IsSpecialTab => IsCopyTab || IsNewTab;
    
    public bool IsNewTab => _script.Name == "+++";

    public bool IsCopyTab => _script.Name == "+cpy+";

    public bool HasChanged()
    {
        if (IsSpecialTab) return false;
        var viewModel = (LuaScriptEditorViewModel)ViewModel;
        return _script.Name != viewModel.Name || _script.Content != viewModel.EditorContent;
    }

    public Script UpdatedScript {
        get
        {
            if (IsSpecialTab) return _script;
            var viewModel = (LuaScriptEditorViewModel)ViewModel;
            _script.Name = viewModel.Name;
            _script.Content = viewModel.EditorContent;
            return _script;
        }
    }

    public Script GetScript()
    {
        return _script;
    }

    public void UpdateScript(Script savedScript)
    {
        _script = savedScript;
        _name =  _script.Name;
        Header = _name;
    }

    public bool Primary
    {
        get
        {
            if (IsSpecialTab) return false;
            if (ViewModel != null)
                return ((LuaScriptEditorViewModel)ViewModel).IsPrimary;
            return false;
        }
        set
        {
            if (IsSpecialTab) return;
            if (ViewModel != null)
                ((LuaScriptEditorViewModel)ViewModel).IsPrimary = value;
        }
    }
}