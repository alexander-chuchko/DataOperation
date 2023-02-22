
using System.Text.RegularExpressions;

namespace DataOperation.Helpers
{
    public class Validation
    {
        private static Regex patternDate;
        private static Regex patternPayment;
        private static Regex patternName;
        private static Regex patternAccountNumber;

        static Validation()
        {
            patternDate = new Regex(@"^[0-9]{4}-[0-9]{2}-[0-9]{2}$");
            patternPayment = new Regex(@"^[0 - 9]([.,][0 - 9]{ 1, 3 })?$");
            patternAccountNumber = new Regex(@"^[0-9]{6}$");
            patternName = new Regex(@"^[A-Z]([a-z][A-Z]?){2,15}$");
        }

        public static bool IsValidPayment(string id)
        {
            var validationResult = false;

            if (patternPayment.IsMatch(id))
            {
                validationResult = true;
            }

            return validationResult;
        }
        
        public static bool IsValidDate(string id)
        {
            var validationResult = false;

            if (patternDate.IsMatch(id))
            {
                validationResult = true;
            }

            return validationResult;
        }

        public static bool IsValidName(string id)
        {
            var validationResult = false;

            if (patternName.IsMatch(id))
            {
                validationResult = true;
            }

            return validationResult;
        }

        public static bool IsValidAccountNumber(string id)
        {
            var validationResult = false;

            if (patternAccountNumber.IsMatch(id))
            {
                validationResult = true;
            }

            return validationResult;
        }
    }
}
