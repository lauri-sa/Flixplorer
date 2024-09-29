using System;
using System.IO;

namespace Flixplorer.Helpers
{
    /// <summary>
    /// A helper class that manages the saving of error logs.
    /// </summary>
    internal class LogHelper
    {
        /// <summary>
        /// Logs the given exception to a file named ErrorLog.txt.
        /// It creates a directory for the error log file in the LocalApplicationData folder if it doesn't exist.
        /// It then writes the date and time, error message, and stack trace of the exception to the log file.
        /// </summary>
        /// <param name="ex">The exception to be logged.</param>
        public static void LogError(Exception ex)
        {
            string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MovieDatabase");

            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
            }

            using (StreamWriter writer = File.AppendText(logFilePath + "\\ErrorLog.txt"))
            {
                writer.WriteLine($"[{DateTime.Now}]\n{ex.Message}\n{ex.StackTrace}\n");
            }
        }
    }
}