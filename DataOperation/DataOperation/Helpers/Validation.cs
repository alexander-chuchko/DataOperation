
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DataOperation.Helpers
{
    public class Validation
    {
        private static Regex patternDate;
        private static Regex patternPayment;
        private static Regex patternName;
        private static Regex patternAccountNumber;
        private static Regex patternExtensions;
        Regex regex = new Regex(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$");

        static Validation()
        {
            patternDate = new Regex(@"[0-9]{4}-[0?[1-9]|[12]\\d|3[01]");//new Regex(@"^[0-9]{4}-[0-9]{2}-[0-9]{2}$");
            patternPayment = new Regex(@"^(\d*\.)?\d+$"); //new Regex(@"^-?[0-9]*\\.?[0-9]+$");//new Regex(@"^[0 - 9]([.,][0 - 9]{ 1, 3 })?$");
            patternAccountNumber = new Regex(@"^[0-9]{7}$");
            patternName = new Regex(@"^[A-Z]([a-z][A-Z]?){2,15}$");
            patternExtensions = new Regex(@"(\w+)\.(txt|csv)$");

        }

        private static bool IsValidExtension(string path)
        {
            var validationResult = false;

            if (patternPayment.IsMatch(path))
            {
                validationResult = true;
            }

            return validationResult;
        }

        private static bool IsValidPayment(string payment)
        {
            var validationResult = false;

            if (patternPayment.IsMatch(payment))
            {
                validationResult = true;
            }

            return validationResult;
        }
        
        private static bool IsValidDate(string date)
        {
            DateTime dt;

            return DateTime.TryParseExact(date, "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out dt);
        }

        private static bool IsValidName(string name)
        {
            var validationResult = false;

            if (patternName.IsMatch(name))
            {
                validationResult = true;
            }

            return validationResult;
        }

        private static bool IsValidAccountNumber(string accountNumber)
        {
            var validationResult = false;

            if (patternAccountNumber.IsMatch(accountNumber))
            {
                validationResult = true;
            }

            return validationResult;
        }

        public static bool CheckParametr(int indexParametr, string parametr)
        {
            bool isValid = true;

            switch (indexParametr)
            {

                case 0:
                    isValid = IsValidName(parametr);
                    break;
                case 1:
                    isValid = IsValidName(parametr);
                    break;
                case 2:
                    isValid = IsValidName(parametr);
                    break;
                case 6:
                    isValid = IsValidPayment(parametr);
                    break;

                case 7:
                    isValid = IsValidDate(parametr);
                    break;

                case 8:
                    isValid = IsValidAccountNumber(parametr);
                    break;

                case 9:
                    isValid = IsValidName(parametr);
                    break;
            }

            return isValid;
        }
    }
}
