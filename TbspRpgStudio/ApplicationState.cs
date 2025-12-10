using Avalonia.Media;
using TbspRpgSettings.Settings;

namespace TbspRpgStudio;

public class ApplicationState
{
    public double WindowWidth { get; set; }
    public double WindowHeight { get; set; }
    public string Language { get; set; } = Languages.DEFAULT;
    public Color AccentColor { get; set; } = Colors.White;

    private ApplicationState()
    {
    }

    private static ApplicationState _instance { get; set; }
    public static ApplicationState Load()
    {
        _instance ??= new ApplicationState();
        return _instance;
    }
}