namespace TbspRpgStudio;

public class ApplicationState
{
    public double WindowWidth { get; set; }
    public double WindowHeight { get; set; }

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