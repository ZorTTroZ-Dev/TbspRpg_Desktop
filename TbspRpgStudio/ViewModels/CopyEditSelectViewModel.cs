using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TbspRpgDataLayer.Entities;
using TbspRpgStudio.Messages;

namespace TbspRpgStudio.ViewModels;

public partial class CopyEditSelectViewModel(List<Copy> copy, string copyDestination, bool readOnly, bool copyEnabled) : ViewModelBase
{
    private readonly List<Copy> _copy = copy;
    [ObservableProperty] private List<Copy> _filteredCopy = copy;
    [ObservableProperty] private string _copyNameFilter = "";
    [ObservableProperty] private Copy? _selectedCopy;
    [ObservableProperty] private bool _copySelected;
    [ObservableProperty] private bool _copyEnabled = copyEnabled;
    [ObservableProperty] private bool _readOnly = readOnly;

    partial void OnSelectedCopyChanged(Copy? value)
    {
        if (value != null)
            CopySelected = true;
    }

    partial void OnCopyNameFilterChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            FilteredCopy = _copy.Where(cpy => cpy.Name.ToLower().Contains(value.ToLower())).ToList();
        else
            FilteredCopy = _copy;
        CopySelected = false;
    }
    
    [RelayCommand]
    public void CancelEdit()
    {
        WeakReferenceMessenger.Default.Send(new AdventureEditCancelPaneMessage());
    }

    [RelayCommand]
    public void Save()
    {
        if (SelectedCopy != null)
            WeakReferenceMessenger.Default.Send(new AdventureEditCopyChangedMessage(SelectedCopy, copyDestination));
    }
}