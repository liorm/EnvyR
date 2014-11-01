using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using FFmpeg.AutoGen;

namespace EnvyR.FFmpeg.Stream
{
    public class CodecStream : IDisposable
    {
        public CodecStream()
        {
            m_subject = new Subject<ManagedAVPacket>();

            // Stream object should be subscribed only once.
            Stream = m_subject.Publish().RefCount();
        }

        /// <summary>
        /// The stream of packets.
        /// </summary>
        public IObservable<ManagedAVPacket> Stream { get; private set; }

        /// <summary>
        /// Internal producer for the packets.
        /// </summary>
        internal readonly Subject<ManagedAVPacket> m_subject;

        #region Implementation of IDisposable

        public void Dispose()
        {
            m_subject.Dispose();
        }

        #endregion
    }
}
