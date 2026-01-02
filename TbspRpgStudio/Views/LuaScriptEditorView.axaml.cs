using System;
using System.Xml;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Platform;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using TbspRpgStudio.ViewModels;

namespace TbspRpgStudio.Views;

public partial class LuaScriptEditorView : UserControl
{
    public LuaScriptEditorView()
    {
        InitializeComponent();
        IHighlightingDefinition luaHighlighting;
        using (var stream = AssetLoader.Open(new Uri("avares://TbspRpgStudio/Assets/LuaSyntax.xshd")))
        {
            using (var reader = new XmlTextReader(stream))
            {
                luaHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }
        
        ScriptEdit.SyntaxHighlighting = luaHighlighting;
        ScriptEdit.ShowLineNumbers = true;
        ScriptEdit.WordWrap = false;
        ScriptEdit.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        ScriptEdit.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        ScriptEdit.TextChanged += OnTextChanged;
        
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        var viewModel = DataContext as LuaScriptEditorViewModel;
        if (viewModel == null) return;
        ScriptEdit.Document = new TextDocument(viewModel.EditorContent);
    }
    
    private void OnTextChanged(object? sender, System.EventArgs e)
    {
        var viewModel = DataContext as LuaScriptEditorViewModel;
        if (viewModel == null) return;
        viewModel.UpdateEditorContent(ScriptEdit.Text);
    }
}