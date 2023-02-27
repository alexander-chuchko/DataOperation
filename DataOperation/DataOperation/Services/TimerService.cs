using DataOperation.Interfaces;
using System.Timers;

namespace DataOperation.Services
{
    public class TimerService : ITimerService
    {
        public event ElapsedEventHandler Elapsed;
        private System.Timers.Timer aTimer;

        public TimerService()
        {
            aTimer = new System.Timers.Timer();
        }
        public double Interval
        {
            get => aTimer.Interval;
            set => aTimer.Interval = value;
        }


        public void Dispose()
        {
        }

        public void Start()
        {
            aTimer.Elapsed += FireElapsedEvent;
            aTimer.AutoReset = true;
            aTimer.Start();
        }

        public void Stop()
        {
            aTimer.Stop();
        }

        public void FireElapsedEvent(object? sender, ElapsedEventArgs e)
        {
            if (Elapsed != null)
            {
                Elapsed?.Invoke(this, null);
            }
        }
    }
}
