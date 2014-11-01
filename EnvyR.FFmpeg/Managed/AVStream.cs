using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFmpeg.AutoGen;
using FFmpegInvoke = FFmpeg.AutoGen.FFmpegInvoke;

namespace EnvyR.FFmpeg.Managed
{
    unsafe class AVStream
    {
        public AVStream(global::FFmpeg.AutoGen.AVStream* internalStream)
        {
            m_timebase = internalStream->time_base;
        }

        private AVRational m_timebase;

        /// <summary>
        /// Convert the given timestamp in timebase to TimeSpan
        /// </summary>
        public TimeSpan GetPacketTime(long ts)
        {
            return TimeSpan.FromMilliseconds((double)( ts * m_timebase.num * 1000) / m_timebase.den);
        }
    }
}
