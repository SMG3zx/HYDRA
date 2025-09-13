using SharpHydra.Configuration;

namespace SharpHydra.UI.TUI;

/// <summary>
/// Provides a basic text-based user interface (TUI).
/// Handles main menu and settings menu rendering.
/// </summary>
public class TUI
{
    // Reference to configuration object for displaying settings
    private Config config { get; set; }

    public TUI(Config config)
    {
        this.config = config;
    }

    /// <summary>
    /// Displays the main menu options to the console.
    /// </summary>
    public void ShowMainMenu()
    {
        Console.WriteLine($"HYDRA Version : {config.Version}");
        Console.WriteLine("Author : SMG3zx");
        Console.WriteLine($"Last Updated: {config.LastModifiedDate}");
        Console.WriteLine("=============================================");
        Console.WriteLine("HYDRA Main Menu");
        Console.WriteLine("1. Clean Profiles");
        Console.WriteLine("2. Settings");
        Console.WriteLine("3. Exit");
    }

    /// <summary>
    /// Displays the settings menu options with current configuration values.
    /// </summary>
    public void ShowSettingsMenu()
    {
        Console.WriteLine("HYDRA Settings");
        Console.WriteLine($"1. Change Client List Path: {string.Join(", ", config.ClientsPath)}");
        Console.WriteLine($"2. Change Log Location: {config.LogPath}");
        Console.WriteLine($"3. Change Skip List: {string.Join(", ", config.SkipProfiles)}");
        Console.WriteLine($"4. Change Age Setting: {config.AgeSetting}");
        Console.WriteLine($"5. Change Throttle Limit: {config.ThrottleLimit}");
        Console.WriteLine($"6. Return to Main Menu");
    }
}