
using DataOperation.Interfaces;

namespace DataOperation.Services
{
    public class PaymentService
    {
        private readonly ILogService _logService;

        public PaymentService(ILogService logService)
        {
            _logService = logService;   
        }

        public string ReadFromLog()
        {
            return _logService.Read();
        }
    }
}
