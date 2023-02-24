
using DataOperation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataOperation.Interfaces
{
    public interface ILogService
    {
        string LogPath { get; }
        void Write(string logInfo, string path);
        string Read(string path);
        Task<string> ReadAllTextAsync();
        Task WriteToJSONAsync(IEnumerable<Root> collection);
    }
}
