using TbspRpgStudio.ViewModels;

namespace TbspRpgStudio;

public class WindowNameMap
{
    public const string ADVENTURE_EDIT_WINDOW = "AdventureEdit";
    
    public static ViewModelBase? WindowNameToViewModel(string windowName)
    {
        // I could probably do this with reflection
        switch (windowName)
        {
            case ADVENTURE_EDIT_WINDOW:
                return new AdventureEditViewModel();
        }
        return null;
    }
}