
using DataOperation.Interfaces;
using DataOperation.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace DataOperation.Services
{
    public class LogService : ILogService
    {
        private const int DefaultBufferSize = 4096;
        // File accessed asynchronous reading and sequentially from beginning to end.
        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

        public static string logFilePath = Path.Combine(ConfigurationManager.AppSettings["pathToFolderA"], "transactions.log"); //$@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\transactions.log";
        public static string logFilePath1 =$@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\transactions.log";
        public string LogPath => throw new System.NotImplementedException();

        public string Read()
        {
            string readTransactions = null;

            if (!File.Exists(logFilePath))
            {
                throw new System.InvalidOperationException();
            }

            using (var file = new StreamReader(logFilePath))
            {
                //To make async method
                readTransactions = file.ReadToEnd();
            }

            return readTransactions?.Length > default(int) ?
                readTransactions :
                "There are no recorded transactions";
        }

        public void Write(string logInfo, string path)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(logInfo))
            {
                string formattedString = string.Concat(logInfo, "\n");

                bool isFile = File.Exists(path);

                using (StreamWriter write = new StreamWriter(path, isFile))
                {
                    write.Write(formattedString);
                }
            }
        }

        public async Task WriteToJSONAsync(IEnumerable<Root> collection)
        {
            try
            {
                await File.WriteAllTextAsync(logFilePath, JsonConvert.SerializeObject(collection,
                    Formatting.Indented,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }

        public async Task<string> ReadAllTextAsync()
        {
            using (var sourceStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            {
                var sb = new StringBuilder();
                var buffer = new byte[0x1000];
                var numRead = 0;

                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    sb.Append(Encoding.Unicode.GetString(buffer, 0, numRead));
                }    

                return sb.ToString();
            }
        }
    }
}
