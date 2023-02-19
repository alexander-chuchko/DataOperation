
namespace DataOperation.Interfaces
{
    internal interface ILogService
    {
        string LogPath { get; }
        void Write(string logInfo);
        string Read();
    }
}
