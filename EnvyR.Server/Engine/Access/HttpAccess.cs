﻿using System;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Policy;
using System.Threading.Tasks;
using EnvyR.Server.Engine.Interfaces;
using EnvyR.Server.Services;
using Splat;

namespace EnvyR.Server.Engine.Access
{
    class HttpAccess : ISource, IEnableLogger
    {
        public HttpAccess(Uri sourceUri)
        {
            m_sourceUri = sourceUri;
        }

        private readonly Uri m_sourceUri;

        public IObservable<Packet> VideoStream { get; private set; }
        public IObservable<Packet> AudioStream { get; private set; }

        private readonly Subject<Packet> m_audioSubject = new Subject<Packet>();
        private readonly Subject<Packet> m_videoSubject = new Subject<Packet>();

        private bool m_started = false;

        public async void Start()
        {
            if (m_started)
                throw new InvalidOperationException("Already started");
            m_started = true;

            HttpWebRequest request = HttpWebRequest.CreateHttp(m_sourceUri);

            // Wait for a response to arrive.
            var response = await request.GetResponseAsync();
            using (response.GetResponseStream())
            {
                
            }
        }
    }
}