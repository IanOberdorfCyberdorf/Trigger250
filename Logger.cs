using System;
using System.Diagnostics;
using System.IO;

public class Logger
{
    private readonly string _logFilePath;

    public Logger(string logFilePath)
    {
        _logFilePath = logFilePath;
    }

    public void Log(string message)
    {
        try
        {
            using (var writer = new StreamWriter(_logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur while logging (optional)
            Debug.WriteLine($"Logging error: {ex.Message}");
        }
    }
}