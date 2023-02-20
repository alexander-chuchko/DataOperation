using DataOperation.Interfaces;
using DataOperation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataOperation.Services
{
    public class MockService
    {
        public static string logFilePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\transactions.txt";
        private readonly ILogService _logService;
        public MockService(ILogService logService)
        {
            _logService = logService;
            CreatePayerFiles();
        }

        private List<string> GetPayers()
        {
            List<string> listPayers = new List<string>()
            {
                "John, Doe, “Lviv, Kleparivska 35, 4”, 500.0, 2022-27-01, 1234567, Water",
                "Mike, Wiksen, “Lviv, Kleparivska 40, 1”, 720.0, 2022-27-05, 7654321, Heat",
                "Nick, Potter, “Lviv, Gorodotska 120, 3”, 880.0, 2022-25-03, “3334444”, Parking",
                "Luke Pan,, “Lviv, Gorodotska 120, 5”, 40.0, 2022-12-07, 2222111, Gas",
                "Alex, Chuchko, “Oleksandriy, Kleparivska 35, 4”, 650.0, 2022-27-01, 1234567, Water",
            };

            return listPayers;
        }


        private void CreatePayerFiles()
        {
            foreach (var item in GetPayers())
            {
                _logService.Write(item);
            }
        }
    }
}
