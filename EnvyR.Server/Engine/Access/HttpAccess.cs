using System;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using EnvyR.FFmpeg;
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

            var b = new InputFile(@"C:\Users\Lior\Videos\The Simpsons Movie - Trailer.mp4");

            await b.OpenFileAsync();

            b.Streams[0].Stream.
                Delay(TimeSpan.FromSeconds(2)).
                ObserveOn(SynchronizationContext.Current).
                Subscribe(( p ) =>
                          {
                              this.Log().Debug("Packet: {0}", p.Packet.pos);
                          });

            b.StartRunning();


        }
    }
}
