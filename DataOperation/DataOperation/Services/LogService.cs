
using DataOperation.Interfaces;
using System.IO;
using System.Reflection;

namespace DataOperation.Services
{
    public class LogService : ILogService
    {
        public static string logFilePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\transactions.log";
        public string LogPath => throw new System.NotImplementedException();

        public string Read()
        {
            string? readTransactions = null;

            if (!File.Exists(logFilePath))
            {
                throw new System.InvalidOperationException();
            }

            using (var file = new StreamReader(logFilePath))
            {
                readTransactions = file.ReadToEnd();
            }

            return readTransactions?.Length > default(int) ?
                readTransactions :
                "There are no recorded transactions";
        }

        public void Write(string logInfo)
        {
            if (!string.IsNullOrEmpty(logFilePath) && !string.IsNullOrEmpty(logInfo))
            {
                string? formattedString = string.Concat(logInfo, "\n");

                bool isFile = File.Exists(logFilePath);

                using (StreamWriter write = new StreamWriter(logFilePath, isFile))
                {
                    write.Write(formattedString);
                }
            }
        }
    }
}
