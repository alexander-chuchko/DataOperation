using DataOperation.Interfaces;
using DataOperation.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<string> ReadAsync(string path)
        {
            string readTransactions = null;

            if (!File.Exists(path))
            {
                throw new System.InvalidOperationException();
            }

            using (var file = new StreamReader(path))
            {
                //To make async method
                Console.WriteLine($"Begin read in method ReadAsync {path}");
                readTransactions = await file.ReadToEndAsync();
                Console.WriteLine($"End read in method ReadAsync {path}");
            }

            return readTransactions?.Length > default(int) ?
                readTransactions :
                "There are no recorded payments";
        }

        public async Task WriteAsync(string logInfo, string path)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(logInfo))
            {
                string formattedString = string.Concat(logInfo, "\n");

                bool isFile = File.Exists(path);

                using (StreamWriter write = new StreamWriter(path, isFile))
                {
                    await write.WriteAsync(formattedString);
                }
            }
        }

        public async Task WriteToJSONAsync(IEnumerable<Root> collection, string path)
        {
            try
            {
                Console.WriteLine($"Begin {path}");
                await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(collection,
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        DateFormatString = "yyyy-MM-dd",
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
                Console.WriteLine($"End {path}");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }

        public async Task<string> ReadAllTextAsync(string path)
        {
            Console.WriteLine(path);
            using (var sourceStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            {
                var sb = new StringBuilder();
                var buffer = new byte[0x1000];
                var numRead = 0;

                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    sb.Append(Encoding.Unicode.GetString(buffer, 0, numRead));
                }
                Console.WriteLine(path);
                return sb.ToString();
            }
        }

        public void CreateFolder(string path)
        {
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
