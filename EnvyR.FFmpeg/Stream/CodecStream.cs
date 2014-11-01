using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using EnvyR.FFmpeg.Managed;
using FFmpeg.AutoGen;
using AVPacket = EnvyR.FFmpeg.Managed.AVPacket;
using AVStream = FFmpeg.AutoGen.AVStream;

namespace EnvyR.FFmpeg.Stream
{
    public unsafe class CodecStream : IDisposable
    {
        internal CodecStream(AVStream* avStream)
        {
            AVStream = avStream;
            m_subject = new Subject<AVPacket>();

            // Stream object should be subscribed only once.
            Stream = m_subject.Publish().RefCount();
        }

        /// <summary>
        /// The stream of packets.
        /// </summary>
        public IObservable<AVPacket> Stream { get; private set; }

        /// <summary>
        /// Internal producer for the packets.
        /// </summary>
        internal readonly Subject<AVPacket> m_subject;

        internal AVStream* AVStream { get; private set; }

        #region Implementation of IDisposable

        public void Dispose()
        {
            m_subject.Dispose();
        }

        #endregion
    }
}
