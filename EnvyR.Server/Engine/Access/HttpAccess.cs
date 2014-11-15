using System;
using System.Net;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using EnvyR.FFmpeg;
using EnvyR.FFmpeg.Managed;
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

            //var b = new InputFile(@"C:\Users\Lior\Videos\The Simpsons Movie - Trailer.mp4");
            //var b = new InputFile(@"http://xperitoseu.blob.core.windows.net/asset-ce753fd0-5983-4d62-ad77-56e0e76180e6/The%20Simpsons%20Movie%20-%20Trailer.mp4?sv=2012-02-12&sr=c&si=d1fc52c2-dc8f-429b-b400-61a86c8091b0&sig=AgMNVkcsctgLxZFao7gLcQiZj4SdJhlb1onXu8AVsyU%3D&st=2014-10-31T14%3A31%3A28Z&se=2016-10-30T14%3A31%3A28Z");
            //var b = new InputFile(@"http://213.8.143.168/100fmAudio");

            var b = Factory.CreateAdapter(@"http://213.8.143.168/100fmAudio");

            await b.ConnectAsyncTask();

            b.Streams[0].PacketsStream.
                ObserveOn(SynchronizationContext.Current).
                Subscribe(( p ) =>
                          {
                              this.Log().Debug("Packet: {0}", p.Timestamp);
                          });

            b.StartPlaying();
        }
    }
}
