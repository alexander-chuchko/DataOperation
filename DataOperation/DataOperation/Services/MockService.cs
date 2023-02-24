using DataOperation.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace DataOperation.Services
{
    public class MockService
    {
        public const int ITEM_GENERATION_FILE = 3;
        public const int NUMBER_PAYMENTS = 3;

        private readonly ILogService _logService;
        public MockService(ILogService logService)
        {
            _logService = logService;
            StartGeneration();
        }

        private List<string> GetPayers()
        {
            return new List<string>()
            {
                "John, Doe, “Lviv, Kleparivska 35, 4”, 500.0, 2022-27-01, 1234567, Water",
                "Mike, Wiksen, “Lviv, Kleparivska 40, 1”, 720.0, 2022-27-05, 7654321, Heat",
                "Nick, Potter, “Lviv, Gorodotska 120, 3”, 880.0, 2022-25-03, “3334444”, Parking",
                "Luke Pan,, “Lviv, Gorodotska 120, 5”, 40.0, 2022-12-07, 2222111, Gas",
                "Alex, Chuchko, “Oleksandriy, Kleparivska 35, 4”, 650.0, 2022-27-01, 1234567, Water",
                "Alex, Klichko, “Kyiv, Kleparivska 35, 4”, 650.0, 2022-27-01, 1238867, Light",
                "Alex, Klichko, “Zaporozhye, Kleparivska 35, 4”, 650.0, 2022-27-01, 1238867, Parking",
                "Mike, Wiksen, “Lviv, Kleparivska 40, 1”, 720.0, 2022-27-05, 7654321, Heat",
                "Alina, Shevneva, “Zaporozhye, Gorodotska 120, 3”, 880.0, 2022-25-03, “3334444”, Parking",
                "Luke Pan,, “Lviv, Gorodotska 120, 5”, 40.0, 2022-12-07, 2222111, Gas",
                "Alex, Chuchko, “Oleksandriy, Kleparivska 35, 4”, 650.0, 2022-27-01, 1234567, Water",
            };
        }

        private void StartGeneration()
        {
            try
            {
                CreateFolder(Path.Combine(ConfigurationManager.AppSettings["pathToFolderA"]));
                CreateFolder(Path.Combine(ConfigurationManager.AppSettings["pathToFolderB"]));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error"+ ex.Message);
            }

            CreatePayerFiles(GetPayers());
        }

        private void CreatePayerFiles(IEnumerable<string>colection)
        {
            if (colection is List<string> payers)
            {
                Random random = new Random();
                string logFilePath = string.Empty;

                for (int i = 0; i < ITEM_GENERATION_FILE * NUMBER_PAYMENTS; i++)
                {
                    var index = random.Next(0, payers.Count);

                    var payer = payers[index];

                    if (i % NUMBER_PAYMENTS == 0)
                    {
                        logFilePath = Path.Combine(ConfigurationManager.AppSettings["pathToFolderA"], CreateName());
                    }

                    _logService.Write(payer, logFilePath);

                    payers.RemoveAt(index);
                }
            }
        }

        public void CreateFolder(string path)
        {
            if (!string.IsNullOrEmpty(path) &&!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public string CreateName()
        {
            Random random = new Random();
            string extension = random.Next(0, 9) % 2 == 0 ?
            ".TXT" :
            ".CSV";
            return Guid.NewGuid().ToString() + extension;
        }
    }
}
