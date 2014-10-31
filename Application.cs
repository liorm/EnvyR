﻿using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using EnvyR.Server.Engine;
using EnvyR.Server.Engine.Access;
using EnvyR.Server.Engine.Interfaces;
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

        private readonly List<ISource> m_feeders = new List<ISource>();

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

            var vs =
                new HttpAccess(
                    new Uri(
                        @"http://xperitoseu.blob.core.windows.net/asset-ce753fd0-5983-4d62-ad77-56e0e76180e6/The%20Simpsons%20Movie%20-%20Trailer.mp4?sv=2012-02-12&sr=c&si=d1fc52c2-dc8f-429b-b400-61a86c8091b0&sig=AgMNVkcsctgLxZFao7gLcQiZj4SdJhlb1onXu8AVsyU%3D&st=2014-10-31T14%3A31%3A28Z&se=2016-10-30T14%3A31%3A28Z"));
            m_feeders.Add(vs);

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
