
namespace DataOperation.Interfaces
{
    public interface ILogService
    {
        string LogPath { get; }
        void Write(string logInfo);
        string Read();
    }
}
