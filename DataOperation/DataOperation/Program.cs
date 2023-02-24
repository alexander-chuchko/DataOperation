using DataOperation.Models;
using DataOperation.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataOperation
{
    public class Program
    {
        private static void GetAllStrings()
        {
            LogService logService = new LogService();
            Task<string> getAllStrings = logService.ReadAllTextAsync();
            getAllStrings.ContinueWith(_ => 
            {
                var lists = getAllStrings.Result;

                Console.WriteLine("HelloWWWWWWWWWWWWWWW");
            });

        } 
        static void Main(string[] args)
        {


            //DriveInfo[] allDrives = DriveInfo.GetDrives();
            //string fileName = System.IO.Path.GetRandomFileName();
            //MockService mockService = new MockService(new LogService());

            PaymentService paymentService = new PaymentService(new LogService());
            //en-GB
            //en-US
            DateTime dateTime = DateTime.Now;
            var res = dateTime.ToString("dd-mm-yyyy");
            string name = string.Concat(dateTime.Day, '-', dateTime.Month, '-', dateTime.Year).ToString();
            paymentService.StartProgramm();
            //paymentService.StartProgramm();


            //LogService logService = new LogService();
            //GetAllStrings();

            //string arrayTransaction = "John, Doe, “Lviv, Kleparivska 35, 4”, 500.0, 2022-27-01, 1234567, Water"; //paymentService.ReadFromLog();
            //var transactions2 = arrayTransaction.Split(new string[] { ", ", "“", "”", " "}, StringSplitOptions.RemoveEmptyEntries);

            //var transactions = arrayTransaction.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            //var transactions1 = transactions[1].Split(new string[] { ", ", "“", "”", "'\"" }, StringSplitOptions.RemoveEmptyEntries);
            /*
            List<Root> roots = new List<Root>();

            foreach (var item in transactions)
            {
                var arrayWords = item.Split(new string[] { ", ", "“", "”", "'\"" }, StringSplitOptions.RemoveEmptyEntries);
                Payer payer = new Payer()
                {
                    Name = String.Concat(arrayWords[0],
                    arrayWords[1]),
                    Payment = Decimal.Parse(arrayWords[6]),
                    AccountNumber = int.Parse(arrayWords[7]),
                    Date = DateTime.Parse(arrayWords[8]),
                };
                

                Root root = new Root(); 
                root.City = arrayWords[3];

            }
            */


            //"John, Doe, “Lviv, Kleparivska 35, 4”, 500.0, 2022-27-01, 1234567, Water",

            //List<string> list = new List<string>();
            /*
            foreach (var item in transactions)
            {
                list.Add(item);
            }*/

            //string phonePattern = @"^[2-9]\d{2}-\d{3}



            Console.WriteLine("Hello World!");
        }
    }
}
