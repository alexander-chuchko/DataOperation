using DataOperation.Interfaces;
using DataOperation.Models;
using DataOperation.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace DataOperation
{
    public class Program
    {

        static async Task<string> ReadFromFileAsync(string filePath, int bufferSize = 1024)
        {
            if (bufferSize < 1024)
                throw new ArgumentNullException("bufferSize");

            if (string.IsNullOrEmpty(filePath))

                throw new ArgumentNullException("filePath");

            StringBuilder readBuffer = null;

            byte[] buffer = new byte[bufferSize];

            FileStream fileStream = null;

            try
            {
                Console.WriteLine($"Begin read in method ReadFromFileAsync {filePath}");

                readBuffer = new StringBuilder();

                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read,

                FileShare.Read, bufferSize: bufferSize, useAsync: true);

                Int32 bytesRead = 0;

                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    readBuffer.Append(Encoding.Unicode.GetString(buffer, 0, bytesRead));

                }

            }
            catch
            {
                readBuffer = null;

                //Write code here to handle exceptions;
            }
            finally
            {

                if (fileStream != null)

                    fileStream.Dispose();
            }

            Console.WriteLine($"End read in method ReadFromFileAsync {filePath}");

            return readBuffer.ToString();

        }

        public static async Task<string> CheckOrReadFile(FileInfo file)
        {
                Console.WriteLine($"Work method CheckOrWriteFile {file.Name}");
                //change
                var contentFile = await ReadAsync(file.FullName);
                Console.WriteLine($"Ended work method CheckOrWriteFile {file.Name}");

            return file.Name;
            
        }

        public static string ReadFile(FileInfo file)
        {
            Console.WriteLine($"Work method CheckOrWriteFile {file.Name}");
            //change
            var contentFile = Read(file.FullName);
            Console.WriteLine($"Ended work method CheckOrWriteFile {file.Name}");

            return file.Name;

        }

        public static IEnumerable<FileInfo> SelectFiles()
        {
            string invalidFiles = string.Empty;

            IEnumerable<FileInfo> files = null;

            var startFolder = Path.Combine(ConfigurationManager.AppSettings["pathToFolderA"]);

            DirectoryInfo dir = new DirectoryInfo(startFolder);

            if (dir.Exists)
            {
                files = dir.GetFiles("*.*", SearchOption.AllDirectories).Where(x =>
                            (x.Extension.ToLower() == ".txt") || (x.Extension.ToLower() == ".csv"));

            }

            return files;
        }

        public static async Task<string> ReadAsync(string path)
        {
            string readTransactions = null;

            if (!System.IO.File.Exists(path))
            {
                throw new System.InvalidOperationException();
            }

            using (var file = new StreamReader(path))
            {
                //To make async method
                Console.WriteLine($"Work method ReadAsync {path}");
                readTransactions = await file.ReadToEndAsync();
                Console.WriteLine($"Ended work method ReadAsync {path}");
            }

            return path;
        }

        public static string Read(string path)
        {
            string readTransactions = null;

            if (!System.IO.File.Exists(path))
            {
                throw new System.InvalidOperationException();
            }

            using (var file = new StreamReader(path))
            {
                //To make async method
                Console.WriteLine($"Work method ReadAsync {path}");
                readTransactions = file.ReadToEnd();
                Console.WriteLine($"Ended work method ReadAsync {path}");
            }

            return readTransactions?.Length > default(int) ?
                readTransactions :
                "There are no recorded payments";
        }



        private static void GetAllStrings()
        {
            //Stopwatch stopwatch = Stopwatch.StartNew();
            LogService logService = new LogService();
            var res = SelectFiles();
            foreach (var file in res)
            {
                Task <string>getAllStrings = ReadAsync(file.FullName);
                getAllStrings.ContinueWith(_ =>
                {
                    var lists = getAllStrings.Result;

                    //Console.WriteLine($"File {lists} read");
                });
            }

            Console.WriteLine($"End Read!!!");
            //stopwatch.Stop();
            //Console.WriteLine(stopwatch.ElapsedMilliseconds);

        }

        private static async Task GetAllStrings1()
        {
            
            LogService logService = new LogService();
            var res = SelectFiles();

            foreach (var file in res)
            {
                //await ReadFromFileAsync(file.FullName);
                //await ReadAsync(file.FullName);
                //Console.WriteLine($"Execute: {file.Name}");
                Read(file.FullName);
            }

        }
        static async Task Main(string[] args)
        {
            /*
            Stopwatch stopwatch = Stopwatch.StartNew();

            
            List<Task<string>> tasks1 = new List<Task<string>>();

            IEnumerable<Task<string>> tasks = SelectFiles().Select(x => x.FullName).Select(ReadAsync).ToList();

            Task<string[]> allTask = Task.WhenAll(tasks);
            string[] strings = await allTask;*/


            //Read(file.FullName);
            //GetAllStrings();
            //await GetAllStrings1();

           // stopwatch.Stop();

            //Console.WriteLine($"Time spend {stopwatch.ElapsedMilliseconds}");
            /*
            Dictionary<string, Action> commands = new Dictionary<string, Action>()
            {
                { "start", (() =>{ }),
                { "stop", (() => Console.Clear()) }
            };*/

            //start/reset/stop 

            //Console.Write("");
            /*
            while (true)
            {
                string input = Console.ReadLine();
                if (commands.ContainsKey(input))
                    commands[input]();
                else
                    Console.WriteLine("Unrecognized command: " + input);
            }*/

            
           PaymentService paymentService = new PaymentService(new LogService(), new MockService(new LogService()), new TimerService());
            //MockService mockService = new MockService(new LogService());
            //mockService.GenerationMock();
           await paymentService.StartProgramm();
           Console.ReadKey();
        }
    }
}
