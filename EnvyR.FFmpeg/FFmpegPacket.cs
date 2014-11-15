using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvyR.Common.Interfaces;

namespace EnvyR.FFmpeg
{
    class FFmpegPacket : IPacket
    {
        private TimeSpan m_timestamp;

        public TimeSpan Timestamp
        {
            get { return m_timestamp; }
        }
    }
}
