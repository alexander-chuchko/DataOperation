
using DataOperation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataOperation.Interfaces
{
    public interface ILogService
    {
        Task WriteAsync(string logInfo, string path);
        Task<string> ReadAsync(string path);
        Task<string> ReadAllTextAsync(string path);
        Task WriteToJSONAsync(IEnumerable<Root> collection, string path);
        void CreateFolder(string path);
    }
}
