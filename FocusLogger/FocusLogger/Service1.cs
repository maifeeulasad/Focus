using System.ServiceProcess;
using System.Threading;

namespace FocusLogger
{
    public partial class Service1 : ServiceBase
    {
        Thread Worker;
        AutoResetEvent StopRequest = new AutoResetEvent(false);

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            // Start the worker thread
            Worker = new Thread(DoWork);
            Worker.Start();


        }

        protected override void OnStop()
        {
            StopRequest.Set();
            Worker.Join();
        }

        public void onDebug()
        {
            OnStart(null);
        }

        private void DoWork(object arg)
        {
            // Worker thread loop
            for (; ; )
            {

                // new FocusDetection();
                if (StopRequest.WaitOne(10000)) return;
            }
        }
    }
}
