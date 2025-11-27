using System;
using System.IO;
using Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace TbspRpgStudio;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        var loggerFactory = new LoggerFactory().AddSerilog(Log.Logger);
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogInformation("Logger configured");
        
        var dbPath = configuration["Database:Path"];
        if (dbPath == null)
        {
            logger.LogError("Database path not configured");
            throw new ArgumentNullException(nameof(dbPath));
        }
        var initializeSet = bool.TryParse(configuration["Database:Initialize"], out var initializeDb);
        var connectionString = $"Data Source={dbPath};";
        if (!File.Exists(dbPath) || (initializeSet && initializeDb))
        {
            if (initializeSet && initializeDb && File.Exists(dbPath))
                File.Delete(dbPath);
            using (File.Create(dbPath)) { }
            TbspRpgDataLayer.TbspRpgDataLayer.Connect(connectionString, loggerFactory, true);
        }
        else
        {
            TbspRpgDataLayer.TbspRpgDataLayer.Connect(connectionString, loggerFactory);    
        }
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }


// Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}