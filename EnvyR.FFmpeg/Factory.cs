using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvyR.Common.Interfaces;
using FFmpeg.AutoGen;

namespace EnvyR.FFmpeg
{
    /// <summary>
    /// Main ffmpeg entry point.
    /// </summary>
    public static class Factory
    {
        static Factory()
        {
            // Initialize the library.
            FFmpegInvoke.avformat_network_init();

            // Register the codecs.
            FFmpegInvoke.av_register_all();
            FFmpegInvoke.avcodec_register_all();
#if DEBUG
            FFmpegInvoke.av_log_set_level(1000);
#endif
        }

        /// <summary>
        /// Create an input adapter for the specified url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IInputAdapter CreateAdapter(string url)
        {
            return new FFmpegInputAdapter(url);
        }
    }
}
