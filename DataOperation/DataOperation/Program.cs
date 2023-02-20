using DataOperation.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataOperation
{
    public class Program
    {
        static void Main(string[] args)
        {
            //PaymentService paymentService = new PaymentService(new LogService());
            //MockService mockService = new MockService(new LogService());

            PaymentService paymentService = new PaymentService(new LogService());


            string arrayTransaction = paymentService.ReadFromLog();

            var transactions = arrayTransaction.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            var transactions1 = transactions[1].Split(new string[] { ", ", "“", "”", "'\"" }, StringSplitOptions.RemoveEmptyEntries);

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
