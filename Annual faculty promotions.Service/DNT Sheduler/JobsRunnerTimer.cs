using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadTimer = System.Threading.Timer;

namespace Annual_faculty_promotions.Service.DNT_Sheduler
{
    internal class JobsRunnerTimer
    {
        private ThreadTimer _threadTimer; //keep it alive                              

        public void Start(long startAfter = 1000, long interval = 1000)
        {
            _threadTimer = new ThreadTimer(timerCallback, null, Timeout.Infinite, 1000);
            _threadTimer.Change(startAfter, interval);
        }

        public void Stop()
        {
            if (_threadTimer != null)
                _threadTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public Action OnTimerCallback { set; get; }

        private void timerCallback(object state)
        {
            if (OnTimerCallback != null)
                OnTimerCallback();
        }
    }
}
