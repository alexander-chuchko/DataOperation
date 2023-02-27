
using System.Threading.Tasks;

namespace DataOperation.Interfaces
{
    public interface IPaymentService
    {
        void DeleteFiles(string path);
        Task StartProgrammAsync(int index = 0);
    }
}
