
using DataOperation.Interfaces;
using DataOperation.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Configuration;
using DataOperation.Helpers;
using static System.Net.WebRequestMethods;
using Microsoft.VisualBasic;
using System.Xml.Linq;

namespace DataOperation.Services
{
    public class PaymentService
    {
        private readonly ILogService _logService;
        private readonly IMockService _mockService;
        public List<Root> roots { get; set; } = new List<Root>();

        public Report Report {get; set;} = new Report();  

        public PaymentService(ILogService logService, IMockService mockService)
        {
            _logService = logService;   
            _mockService = mockService; 
        }

        public string ReadFromLog(string path)
        {
            return _logService.Read(path);
        }

        /* Selected files*/
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

            }

            return files;
        }

        //To do string 
        public string GetPathsInvalidFiles(DirectoryInfo dir)
        {
            string pathsInvalid = string.Empty;
            
            var files = dir.GetFiles("*.*", SearchOption.AllDirectories).Where(x =>
            (x.Extension.ToLower() != ".txt") || (x.Extension.ToLower() != ".csv")).ToList();

            if (files != null && files.Count() > 0)
            {
                for (int i = 0; i < files.Count(); i++)
                {
                    pathsInvalid += string.Concat(files[i].FullName, files.Count() >=i ? '.':',');
                }
            }

            //Report.ParsedFiles = files.Count();
            //Report.InvalidFiles = GetPathsInvalidFiles(dir);

            return string.Concat('[', pathsInvalid,']');
        }
        

        //Do async method
        public void CheckOrWriteFile(IEnumerable<FileInfo> files)
        {
            int index = 1;

            foreach (var file in files)
            {
                var contentFile = _logService.Read(file.FullName);

                //Case - When the file is empty
                if ("There are no recorded payments" == contentFile)
                {
                    Report.InvalidFiles.Add(file.FullName);
                    continue;
                }
                else
                {
                    ParseLines(contentFile, file.FullName);
                    SaveFile(index);

                    if (roots.Count > 0)
                    {
                        roots.Clear();
                    }

                    index++;
                }
            }
        }

        public void ParseLines(string contentFile, string path)
        {
            int foundErrors = 0;

            string[] fileLines = contentFile.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (fileLines.Length > 0)
            {
                for (int i = 0; i < fileLines.Length; i++)
                {
                    string[] parametrs = SplitString(fileLines[i]);

                    if (parametrs.Length == 10 && CheckParametersLine(parametrs))
                    {
                        CreateModel(parametrs);
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
        }

        public void WriteReport() 
        {
            string nameFile = "meta.log";

            //DateTime dateTime = DateTime.Now;
            //string name = dateTime.ToString("dd-MM-yyyy");
            var startFolder = GetPathFolder(); //Path.Combine(ConfigurationManager.AppSettings["pathToFolderB"], name);

            CreateFolder(startFolder);

            string report = $"parsed_files: {Report.ParsedFiles}\n" +
                $"parsed_lines: {Report.ParsedLines}\n" +
                $"found_errors: {Report.FoundErrors} \n" +
                $"invalid_files: [{string.Join(", ", Report.InvalidFiles)}]";

            _logService.Write(report, Path.Combine(startFolder,nameFile));
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

        public void StartProgramm()
        {

            var files = SelectFiles();

            if (files.Count() > 0)
            {
                CheckOrWriteFile(files);
                WriteReport();
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

        public void SaveFile(int index) 
        {
            string path = GetPathFolder();
            CreateFolder(path);
            var startFolder = Path.Combine(path, $"output{index}.json");
            _logService.WriteToJSONAsync(roots, startFolder);
        }

        public void CreateModel(string[] parametrs)
        {
            if (parametrs is not null) 
            {
                //Created Root
                var root = CreateOrGetRoot(parametrs);
                //Created Service
                var service = CreateOrGetService(parametrs);
                //Created Payer
                var payer = CreatePayment(parametrs);

                service.Payers.Add(payer);
                root.Services.Add(service);

                var findedRoot = roots.Find(x => x.City == parametrs[2]);

                if (findedRoot == null)
                {
                    roots.Add(root);
                }
                string path1 = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\output.log";
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
    }
}
