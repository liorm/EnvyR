using System;
using FFmpegInvoke = FFmpeg.AutoGen.FFmpegInvoke;

namespace EnvyR.FFmpeg.Managed
{
    /// <summary>
    /// Managed AV packet.
    /// </summary>
    public class AVPacket : IDisposable
    {
        internal AVPacket()
        {
            unsafe
            {
                fixed (global::FFmpeg.AutoGen.AVPacket* p = &Packet)
                    FFmpegInvoke.av_init_packet(p);
            }
        }

        internal AVStream Stream;

        /// <summary>
        /// The wrapped packet.
        /// </summary>
        internal global::FFmpeg.AutoGen.AVPacket Packet;

        public TimeSpan Timestamp
        {
            get
            {
                if ( !m_timestamp.HasValue )
                    m_timestamp = Stream.GetPacketTime(Packet.pts);

                return m_timestamp.Value;
            }
        }

        private TimeSpan? m_timestamp;

        #region IDisposable implementation

        ~AVPacket()
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
            if ( m_disposed ) 
                return;
            m_disposed = true;

            unsafe
            {
                fixed (global::FFmpeg.AutoGen.AVPacket* p = &Packet)
                    FFmpegInvoke.av_free_packet(p);
            }
        }

        #endregion
    }
}
