
using DataOperation.Interfaces;
using DataOperation.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Configuration;
using DataOperation.Helpers;
using System.Threading.Tasks;
using System.Timers;

namespace DataOperation.Services
{
    public class PaymentService
    {
        private readonly ILogService _logService;
        private readonly IMockService _mockService;
        private readonly ITimerService _checkTimer;
        public List<Root> roots { get; set; } = new List<Root>();

        public Report Report {get; set;} = new Report();
        public string[] LastlyState { get; set; }

        public PaymentService(ILogService logService, IMockService mockService, ITimerService checkTimer)
        {
            _logService = logService;   
            _mockService = mockService; 
            _checkTimer = checkTimer;   
            
        }

        public IEnumerable<FileInfo> SelectFiles()
        {
            string invalidFiles = string.Empty;

            IEnumerable<FileInfo> files = null;

            var startFolder = Path.Combine(ConfigurationManager.AppSettings["pathToFolderA"]);
  
            DirectoryInfo dir = new DirectoryInfo(startFolder);

            if (dir.Exists)
            {
                files = dir.GetFiles("*.*", SearchOption.AllDirectories).Where(x =>
                            (x.Extension.ToLower() == ".txt") || (x.Extension.ToLower() == ".csv"));
                if ()
                {
                    LastlyState = new string[files.Count()];
                    LastlyState = files.Select(x => x.FullName).ToArray();
                }
            }

            return files;
        }

        private void StartOrStopTimer(bool value)
        {
            if(value)
            {
                _checkTimer.Interval = 1000;
                _checkTimer.Start();
            }
            else if(value)
            {
                _checkTimer.Stop();
            }
        }

        public async Task<string[]> ReadFile(IEnumerable<FileInfo> files)
        {
            IEnumerable<Task<string>> tasks = files.Select(x => x.FullName).Select(_logService.ReadAsync).ToList();
            Task<string[]> allTask = Task.WhenAll(tasks);
            string[] strings = await allTask;
            
            return strings;
        }
        
        public async Task CheckOrWriteFile(string contentFile, string fullName, int index)
        {
            if ("There are no recorded payments" == contentFile)
            {
                Report.InvalidFiles.Add(fullName);
            }
            else
            {
                var parametrs = ParseAndCheckLines(contentFile, fullName);

                if (parametrs.Count() > 0)
                {
                    foreach (var parametr in parametrs)
                    {
                        CreateModel(parametr);
                    }
                }

                await SaveFile(index);

                if (roots.Count > 0)
                {
                    roots.Clear();
                }
            }
        }

        public List<string[]> ParseAndCheckLines(string contentFile, string path)
        {
            List<string[]> strings = new List<string[]>();  

            int foundErrors = 0;

            string[] fileLines = contentFile.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (fileLines.Length > 0)
            {
                for (int i = 0; i < fileLines.Length; i++)
                {
                    string[] parametrs = SplitString(fileLines[i]);

                    if (parametrs.Length == 10 && CheckParametersLine(parametrs))
                    {
                        strings.Add(parametrs);
                    }
                    else
                    {
                        foundErrors++;
                    }

                    Report.ParsedLines++;
                }

                Report.FoundErrors += foundErrors;
                Report.ParsedFiles++;

                if (Report.FoundErrors > 0)
                {
                    Report.InvalidFiles.Add(path);
                }
            }

            return strings;
        }

        public async Task WriteReport() 
        {
            string nameFile = "meta.log";

            var startFolder = GetPathFolder(); 

            _logService.CreateFolder(startFolder);

            string report = $"parsed_files: {Report.ParsedFiles}\n" +
                $"parsed_lines: {Report.ParsedLines}\n" +
                $"found_errors: {Report.FoundErrors} \n" +
                $"invalid_files: [{string.Join(", ", Report.InvalidFiles)}]";


            //Change
            await _logService.WriteAsync(report, Path.Combine(startFolder,nameFile));
        }

        public void CreateFolder(string path)
        {
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public bool CheckParametersLine(string []parametrs)
        {
            bool isValid = true;  
            
            for (int i = 0; i < parametrs.Length; i++)
            {
                isValid = Validation.CheckParametr(i, parametrs[i]);

                if (!isValid)
                {
                    isValid = false;
                    break;
                }
            }

            return isValid;
        }

        public async Task StartProgramm()
        {
            var files = SelectFiles().ToList();

            if (files.Count > 0)
            {
                var parametrs = await ReadFile(files);

                for(int i =0; i < parametrs.Length; i++)
                {
                    await CheckOrWriteFile(parametrs[i], files[i].FullName, i + 1);
                }

                await WriteReport();
            }
            else
            {
                Console.WriteLine("No files");
            }
        }

        private string GetPathFolder()
        {
            string startFolder = null;

            try
            {
                DateTime dateTime = DateTime.Now;
                string name = dateTime.ToString("dd-MM-yyyy");
                startFolder = Path.Combine(ConfigurationManager.AppSettings["pathToFolderB"], name);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:"+ ex.Message);
            }


            return startFolder;
        }

        public async Task SaveFile(int index) 
        {
            string path = GetPathFolder();
            _logService.CreateFolder(path);
            var startFolder = Path.Combine(path, $"output{index}.json");
            await _logService.WriteToJSONAsync(roots, startFolder);
        }

        public void CreateModel(string[] parametrs)
        {
            if (parametrs is not null) 
            {
                var root = CreateOrGetRoot(parametrs);
                var service = CreateOrGetService(parametrs);
                var payer = CreatePayment(parametrs);

                service.Payers.Add(payer);
                service.Total += payer.Payment;

                if (!root.Services.Contains(service))
                {
                    root.Services.Add(service);
                }

                root.Total += payer.Payment;

                var findedRoot = roots.Find(x => x.City == parametrs[2]);


                if (findedRoot == null)
                {
                    roots.Add(root);
                }
            }
        }

        public string[] SplitString(string paymentData)
        {
            var parametrs = paymentData.Split(new string[] { ", ", "“", "”", " " }, StringSplitOptions.RemoveEmptyEntries);

            return parametrs;
        }

        public Payer CreatePayment(string[] dataPayment)
        {
            Payer payer = new Payer();

            try
            {
                payer.Name = $"{dataPayment[0]} {dataPayment[1]}";
                payer.Payment = decimal.Parse(dataPayment[6], CultureInfo.InvariantCulture);
                payer.Date = ConvertDate(dataPayment[7]);
                payer.AccountNumber = long.Parse(dataPayment[8]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }

            return payer;
        }

        private Service CreateOrGetService(string[] parametrs)
        {
            Service service = null;
            var findedOrCreatedService = roots.FirstOrDefault(x => x.City == parametrs[2] && x.Services.Exists(d=>d.Name == parametrs[9]));
            
            if (findedOrCreatedService == null)
            {
                service = new Service()
                {
                    Name = parametrs[9], 
                };
            }
            else
            {
                service = findedOrCreatedService.Services.FirstOrDefault(s => s.Name == parametrs[9]);
            }

            return service;
        }

        private Root CreateOrGetRoot(string[] strings)
        {   
            var findedOrCreatedRoot = roots.Find(x => x.City == strings[2]);

            if (findedOrCreatedRoot == null)
            {
                findedOrCreatedRoot = new Root()
                {
                    City = strings[2],
                };

                roots.Add(findedOrCreatedRoot);
            }

            return findedOrCreatedRoot;
        }

        private DateTime ConvertDate(string data)
        {
            DateTime date;
            DateTime.TryParseExact(data, "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out date);

            return date;
        }

        private string[] UpdateFolder()
        {

            return string[];
        }
    }
}
