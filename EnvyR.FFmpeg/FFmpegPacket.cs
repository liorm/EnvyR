using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvyR.Common.Interfaces;
using FFmpeg.AutoGen;

namespace EnvyR.FFmpeg
{
    class FFmpegPacket : IPacket, IDisposable
    {
        public void Initialize(FFmpegInputStream sourceStream)
        {
            m_sourceStream = sourceStream;
        }

        private FFmpegInputStream m_sourceStream;

        public TimeSpan Timestamp
        {
            get
            {
                if (!m_timestamp.HasValue)
                    m_timestamp = m_sourceStream.GetPacketTime(Pkt.pts);

                return m_timestamp.Value;
            }
        }

        private TimeSpan? m_timestamp;

        /// <summary>
        /// The internal packet.
        /// </summary>
        public AVPacket Pkt;

        #region IDisposable implementation

        ~FFmpegPacket()
        {
            Dispose(false);
        }

        private bool m_disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (m_disposed)
                return;
            m_disposed = true;

            unsafe
            {
                fixed (AVPacket* p = &Pkt)
                    FFmpegInvoke.av_free_packet(p);
            }
        }

        #endregion
    }
}
