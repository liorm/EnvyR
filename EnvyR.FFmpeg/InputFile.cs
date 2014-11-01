using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EnvyR.FFmpeg.Exceptions;
using EnvyR.FFmpeg.Stream;
using FFmpeg.AutoGen;
using System.IO;
using System.Collections.ObjectModel;

namespace EnvyR.FFmpeg
{
    /// <summary>
    /// Represents an ffmpeg input file.
    /// </summary>
    public unsafe class InputFile : IDisposable
    {
        static InputFile()
        {
            // Register the codecs.
            FFmpegInvoke.av_register_all();
            FFmpegInvoke.avcodec_register_all();
#if DEBUG
            FFmpegInvoke.av_log_set_level(1000);
#endif
        }

        public InputFile(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            Filename = filename;
        }

        /// <summary>
        /// Request to open the file asynchroneously.
        /// </summary>
        public void OpenFile()
        {
            AVFormatContext* ctx;
            // Open the file with FFmpeg
            if ( FFmpegInvoke.avformat_open_input(&ctx, Filename, null, null) != 0 )
                throw new FFmpegException("Couldn't open file");

            if ( FFmpegInvoke.avformat_find_stream_info(ctx, null) != 0 )
                throw new FFmpegException("Couldn't find stream info");

            m_ctx = ctx;

            if (m_ctx->nb_streams < 1)
                throw new FFmpegException("No streams found");
        }

        AVFormatContext* m_ctx;

        /// <summary>
        /// The input file name.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Duration of the stream
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                double duration = (m_ctx->duration / (double)FFmpegInvoke.AV_TIME_BASE);
                if (duration < 0)
                    duration = 0;
                return TimeSpan.FromTicks((long)(duration * 1000.0 * TimeSpan.TicksPerMillisecond));
            }
        }

        #region IDisposable implementation

        ~InputFile()
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
            if (!m_disposed)
            {
                m_disposed = true;

                fixed (AVFormatContext** ctxAddr = &m_ctx)
                {
                    FFmpegInvoke.avformat_close_input(ctxAddr);
                }
            }
        }

        #endregion
    }
}