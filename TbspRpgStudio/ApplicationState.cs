using Avalonia.Media;
using TbspRpgDataLayer.Entities;
using TbspRpgSettings.Settings;

namespace TbspRpgStudio;

public class ApplicationState
{
    public double WindowWidth { get; set; }
    public double WindowHeight { get; set; }
    public Language Language { get; set; }
    public Color AccentColor { get; set; } = Colors.White;
    public Color AlternateColor { get; set; } = Colors.White;

    private ApplicationState()
    {
        var dataLayer = TbspRpgDataLayer.TbspRpgDataServiceFactory.Load();
        Language = dataLayer.LanguagesService.GetDefaultLanguage();
    }

    private static ApplicationState _instance { get; set; }
    public static ApplicationState Load()
    {
        _instance ??= new ApplicationState();
        return _instance;
    }
}