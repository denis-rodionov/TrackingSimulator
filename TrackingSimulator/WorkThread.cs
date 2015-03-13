using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LocalizationCore;
using LogProvider;

namespace TrackingSimulator
{
    class WorkThread : IDisposable
    {
        const int STEP_TIMEOUT = 10;
        const int MODEL_TIME = 10000;

        Thread workThread;

        public bool finishRequest = false;

        public WorkThread()
        {
            workThread = new Thread(new ThreadStart(threadProc));
            workThread.IsBackground = true;
            workThread.Name = "worker";
            workThread.Start();
        }

        private void threadProc()
        {
            try
            {
                int tick = 0;
                while (true)
                {
                    tick++;
                    //if (tick % 10 == 0)
                    //    Logger.Log("Tick #" + tick, LoggingLevel.Trace);
                    Model.Instance.OnTime();

                    if (STEP_TIMEOUT != 0)
                        Thread.Sleep(STEP_TIMEOUT);

                    if (finishRequest || (MODEL_TIME != 0 && tick >= MODEL_TIME))
                        break;
                }

                Logger.Log("Model stoped!");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw;
            }
        }

        public void Dispose()
        {
            workThread.Abort();
        }

        public void Finish()
        {
            finishRequest = true;
        }
    }
}
