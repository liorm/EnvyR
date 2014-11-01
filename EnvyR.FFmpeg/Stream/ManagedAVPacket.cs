using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFmpeg.AutoGen;

namespace EnvyR.FFmpeg.Stream
{
    public class ManagedAVPacket : IDisposable
    {
        public ManagedAVPacket()
        {
            unsafe
            {
                fixed (AVPacket* p = &Packet)
                    FFmpegInvoke.av_init_packet(p);
            }
        }

        public AVPacket Packet;

        #region IDisposable implementation

        ~ManagedAVPacket()
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
                fixed (AVPacket* p = &Packet)
                    FFmpegInvoke.av_free_packet(p);
            }
        }

        #endregion
    }
}
