using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using EnvyR.Server.Engine;
using EnvyR.Server.Services;
using EnvyR.Server.Utils;
using Splat;

namespace EnvyR.Server
{
    class Application : AsyncApplication, IApp
    {
        static void Main(string[] args)
        {
            var app = new Application();
            app.Run();
        }

        private readonly List<CameraFeeder> m_feeders = new List<CameraFeeder>();

        protected override bool OnInit()
        {
            // Register the app.
            Locator.CurrentMutable.RegisterConstant(this, typeof(IApp));

            // Register a logger.
            Locator.CurrentMutable.RegisterLazySingleton(() => { 
                var logger = new ConsoleLogger(); 
                logger.Start();
                return logger;
            }, typeof(ILogger));

            m_feeders.Add(CameraFeeder.Create());
            m_feeders.Add(CameraFeeder.Create());
            m_feeders.Add(CameraFeeder.Create());

            return true;
        }

        protected override void OnExit()
        {
            base.OnExit();

            var ev = AppExit;
            if (ev != null)
                ev(this, new EventArgs());
        }

        public event EventHandler AppExit;
    }
}
