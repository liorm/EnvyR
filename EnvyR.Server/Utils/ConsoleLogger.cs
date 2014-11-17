using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using EnvyR.Server.Services;
using Splat;

namespace EnvyR.Server.Utils
{
    class ConsoleLogger : ILogger
    {
        class Msg
        {
            public Msg(string message)
            {
                Time = DateTimeOffset.UtcNow;
                Message = message;
            }

            public DateTimeOffset Time { get; private set; }
            public string Message { get; private set; }

        }

        private void OnAppExit(object o, EventArgs e)
        {
            Stop();
        }

        private Thread m_thread;
        private BlockingCollection<Msg> m_queue;

        private void ThreadProc()
        {
            foreach (var item in m_queue.GetConsumingEnumerable())
                Console.WriteLine("{0}: {1}", item.Time, item.Message);
        }

        public void Start()
        {
            if (m_queue != null)
                return;

            // Make sure to stop when app terminates.
            Locator.Current.GetService<IApp>().AppExit += OnAppExit;

            m_queue = new BlockingCollection<Msg>();
            m_thread = new Thread(ThreadProc);
            m_thread.Start();
        }

        public void Stop()
        {
            if (m_queue == null)
                return;

            m_queue.CompleteAdding();
            m_thread.Join();

            m_queue.Dispose();
            m_queue = null;
            m_thread = null;

            Locator.Current.GetService<IApp>().AppExit -= OnAppExit;
        }

        public void Write(string message, LogLevel logLevel)
        {
#if DEBUG
            Debug.WriteLine(message);
#endif
            m_queue.Add(new Msg(message));
        }

        public LogLevel Level { get; set; }
    }
}