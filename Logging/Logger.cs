namespace SharpHydra.Logging;

public class Logger
{
    // Path where logs should be written
    private string logPath { get; set; }

    public Logger(string logPath)
    {
        this.logPath = logPath;
    }

    /// <summary>
    /// Appends a line of text to the log file.
    /// Creates the file if it does not exist.
    /// </summary>
    public void LogTextToFile(string text)
    {
        File.AppendAllText(logPath, text);
    }
}