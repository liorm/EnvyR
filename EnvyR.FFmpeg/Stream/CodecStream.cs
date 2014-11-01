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
    /// <summary>
    /// Base class for a codec.
    /// </summary>
    public unsafe class CodecStream : IDisposable
    {
        internal CodecStream(AVStream* avStream)
        {
            m_timebase = avStream->time_base;
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

        /// <summary>
        /// Convert the given timestamp in timebase to TimeSpan
        /// </summary>
        internal TimeSpan GetPacketTime(long ts)
        {
            return TimeSpan.FromMilliseconds((double)(ts * m_timebase.num * 1000) / m_timebase.den);
        }
        private readonly AVRational m_timebase;

        #region Implementation of IDisposable

        public void Dispose()
        {
            m_subject.Dispose();
        }

        #endregion
    }
}
