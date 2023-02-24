
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

namespace DataOperation.Services
{
    public class PaymentService
    {
        private readonly ILogService _logService;
        public List<Root> roots { get; set; } = new List<Root>();
        //public Dictionary<string, string> Report { get; set; } = new Dictionary<string, string>();

        public Report Report {get; set;} = new Report();  

        public PaymentService(ILogService logService)
        {
            _logService = logService;   
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

            Report.ParsedFiles = files.Count();
            Report.InvalidFiles = GetPathsInvalidFiles(dir);

            return string.Concat('[', pathsInvalid,']');
        }
        

        //Do async method
        public void CheckOrWriteFile(IEnumerable<FileInfo> files)
        {
            foreach (var file in files)
            {
                var contentFile = _logService.Read(file.FullName);

                //Case - When the file is empty
                if ("There are no recorded payments" == contentFile)
                {
                    continue;
                }
                else
                {
                    CheckLines(contentFile);
                }
            }
        }

        public void CheckLines(string contentFile)
        {
            int foundErrors = 0;

            string invalidFiles = string.Empty;

            string[] fileLines = contentFile.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (fileLines.Length > 0)
            {
                for (int i = 0; i < fileLines.Length; i++)
                {
                    string [] parametrs = SplitString(fileLines[i]);

                    if (parametrs.Length == 10 && CheckParametersLine(parametrs))
                    {
                        //Занести строки валидные строки, которые необходимо парсить
                        //передавать строки на конвертацию в модел и после запись в json

                    }
                    else
                    {
                        foundErrors++;
                    }

                    Report.ParsedLines++;
                }

                Report.FoundErrors += foundErrors;
                Report.ParsedFiles++;
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

        public string SelectInvalidFiles()
        {
            return string.Empty;
        }

        public void StartProgramm1()
        {
            var files = SelectFiles();

            if (files.Count() > 0)
            {
                CheckOrWriteFile(files);
            }
            else
            {
            
            }
        }


        public void StartProgramm(string path)
        {
            var allStrings = ReadFromLog(path).Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in allStrings)
            {
                var parametrs = SplitString(item);

                //Created Root
                var root = CreateOrGetRoot(parametrs);

                //Created Service
                var service = CreateOrGetService(parametrs);

                //roots.Add();
                //Created Payer
                var payer =  CreatePayment(parametrs);

                service.Payers.Add(payer);
                root.Services.Add(service);

                var findedRoot = roots.Find(x => x.City == parametrs[2]);

                if (findedRoot == null)
                {
                    roots.Add(root);
                }
                string path1 = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\output.log";
                try
                {
                    var obj = JsonConvert.SerializeObject(roots);
                    Console.WriteLine(obj);
                    //File.WriteAllText(path1, JsonConvert.SerializeObject(roots, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
                

                /*
                service.Payers.Add(payer);
                service.Name = parametrs[9];
                service.Total = 120.0058m;

                root.City = parametrs[2];
                root.Total = 120.0058m;
                root.Services.Add(service);*/
            }

            //var transactions2 = arrayTransaction.Split(new string[] { ", ", "“", "”", " " }, StringSplitOptions.RemoveEmptyEntries);

            //var transactions = arrayTransaction.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            //var transactions1 = transactions[1].Split(new string[] { ", ", "“", "”", "'\"" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] SplitString(string paymentData)
        {
            var parametrs = paymentData.Split(new string[] { ", ", "“", "”", " " }, StringSplitOptions.RemoveEmptyEntries);

            return parametrs;
        }

        public bool ExecuteValidation(string[] paymentData)
        {
            bool isResult = false;

            if (paymentData.Length > 9)
            {
                isResult = true;    
            }


            foreach (var parametr in paymentData)
            {
                
            }   

            return false;
        }

        public Payer CreatePayment(string[] dataPayment)
        {
            Payer payer = new Payer();

            try
            {
                payer.Name = string.Concat(dataPayment[0], ' ', dataPayment[1]);
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
            int[] arrayData = new int[3];

            if (!string.IsNullOrEmpty(data))
            {
                arrayData = data.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            }

            return new DateTime(arrayData[0], arrayData[1], arrayData[2]);
        }
    }
}
