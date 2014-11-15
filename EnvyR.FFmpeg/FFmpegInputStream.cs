using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using EnvyR.Common;
using EnvyR.Common.Interfaces;
using FFmpeg.AutoGen;

namespace EnvyR.FFmpeg
{
    /// <summary>
    /// Represents a stream wrapper around ffmpeg.
    /// </summary>
    unsafe class FFmpegInputStream : IDisposable, IInputStream
    {
        public FFmpegInputStream(AVStream* avStream)
        {
            m_timebase = avStream->time_base;

            switch (avStream->codec->codec_type)
            {
                case AVMediaType.AVMEDIA_TYPE_AUDIO:
                    StreamType = StreamType.Audio;
                    break;
                case AVMediaType.AVMEDIA_TYPE_VIDEO:
                    StreamType = StreamType.Video;
                    break;
                default:
                    StreamType = StreamType.Unknown;
                    break;
            }

            // Stream object should be subscribed only once.
            PacketsStream = m_subject.Publish().RefCount();
        }

        public StreamType StreamType { get; private set; }

        /// <summary>
        /// The stream of packets.
        /// </summary>
        public IObservable<IPacket> PacketsStream { get; private set; }

        /// <summary>
        /// Internal producer for the packets.
        /// </summary>
        private readonly Subject<IPacket> m_subject = new Subject<IPacket>();

        /// <summary>
        /// Push the given packet down the stream.
        /// </summary>
        /// <param name="packet"></param>
        public void SendPacket(FFmpegPacket packet)
        {
            m_subject.OnNext(packet);
        }

        /// <summary>
        /// Convert the given timestamp in timebase to TimeSpan
        /// </summary>
        public TimeSpan GetPacketTime(long ts)
        {
            return TimeSpan.FromMilliseconds((double)(ts * m_timebase.num * 1000) / m_timebase.den);
        }

        private readonly AVRational m_timebase;

        public void Dispose()
        {
            m_subject.Dispose();
        }
    }
}
