using System.Timers;

namespace DataOperation.Interfaces
{
    public interface ITimerService
    {
        event ElapsedEventHandler Elapsed;
        double Interval { get; set; }
        void Start();
        void Stop();
        void Dispose();
    }
}
