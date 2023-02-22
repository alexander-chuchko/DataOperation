
using DataOperation.Interfaces;
using DataOperation.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Reflection;
using System.Collections;

namespace DataOperation.Services
{
    public class PaymentService
    {
        private readonly ILogService _logService;
        public List<Root> roots { get; set; } = new List<Root>(); 

        public PaymentService(ILogService logService)
        {
            _logService = logService;   
        }

        public string ReadFromLog()
        {
            return _logService.Read();
        }

        public void StartProgramm()
        {
            var allStrings = ReadFromLog().Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in allStrings)
            {
                var parametrs = GetParametrsFromString(item);

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
                string path = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\output.log";
                try
                {
                    var obj = JsonConvert.SerializeObject(roots);
                    Console.WriteLine(obj);
                    File.WriteAllText(path, JsonConvert.SerializeObject(roots, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
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

        public string[] GetParametrsFromString(string paymentData)
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
            //string arrayTransaction = "John, Doe, “Lviv, Kleparivska 35, 4”, 500.0, 2022-27-01, 1234567, Water";
            Payer payer = new Payer();

            /*
            {
                Name = string.Concat(dataPayment[0], ' ', dataPayment[1]),
                Payment = decimal.Parse(dataPayment[6]),
                Date = ConvertDate(dataPayment[7]),
                AccountNumber = long.Parse(dataPayment[8]),
            };*/

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
