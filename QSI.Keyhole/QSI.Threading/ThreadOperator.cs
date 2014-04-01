using System;
using System.Threading;

namespace QSI.Threading
{
    /// <summary>
    /// Provides threading pause, resume, and stop controls. The requesting and receiving threads share this object.
    /// The requesting thread uses the request methods
    /// The receiving thread checks the other methods to see if it should stop or pause.
    /// </summary>
    public class ThreadOperator
    {
        private ManualResetEvent _pause;
        private ManualResetEvent _stop;

        public ThreadOperator()
        {
            _pause = new ManualResetEvent(true);
            _stop = new ManualResetEvent(false);
        }

        /// <summary>
        /// The requesting thread sets the pause. It is up the receiving thread to check WaitIfPaused
        /// </summary>
        public void RequestPause()
        {
            _pause.Reset();
        }

        /// <summary>
        /// The requesting thread 'sets' to stop. It is up the receiving thread to check IsStopRequested and respond.
        /// </summary>
        public void RequestStop()
        {
            _stop.Set();
            _pause.Set();
        }

        /// <summary>
        /// The requesting thread 'sets' to resume receiving 
        /// </summary>
        public void RequestResume()
        {
            _pause.Set();
        }

        /// <summary>
        /// The receiving thread calls this at opportune times to pause. It will not return until the requesting thread calls ReqeustResume
        /// </summary>
        public void WaitIfPaused()
        {
            _pause.WaitOne(Timeout.Infinite);
        }

        /// <summary>
        /// The receiving thread calls this at an oppotune time to stop operation. It will return immediately and true if the recieving thread is being reqeusted to stop.
        /// </summary>
        /// <returns></returns>
        public bool IsStopRequested()
        {
            return _stop.WaitOne(0);
        }
    }
}
