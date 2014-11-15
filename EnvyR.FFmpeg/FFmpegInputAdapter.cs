using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvyR.Common;
using EnvyR.Common.Interfaces;
using EnvyR.FFmpeg.Exceptions;
using EnvyR.FFmpeg.Stream;
using FFmpeg.AutoGen;
using Splat;

namespace EnvyR.FFmpeg
{
    /// <summary>
    /// Input adapter that uses ffmpeg as the parser.
    /// </summary>
    class FFmpegInputAdapter : IInputAdapter, IEnableLogger
    {
        public FFmpegInputAdapter(string uri)
        {
            if (String.IsNullOrEmpty(uri))
                throw new ArgumentNullException("uri");

            Uri = uri;

            m_streams = new List<FFmpegInputStream>();
            Streams = m_streams.AsReadOnly();
            m_metadata = new Dictionary<string, string>();
        }

        /// <summary>
        /// The input file name.
        /// </summary>
        public string Uri { get; private set; }

        private readonly List<FFmpegInputStream> m_streams;
        private readonly Dictionary<string, string> m_metadata;

        /// <summary>
        /// The input context.
        /// </summary>
        private IntPtr m_ctx = IntPtr.Zero;

        public async Task<bool> ConnectAsyncTask()
        {
            if (m_ctx != IntPtr.Zero)
                throw new InvalidOperationException("Already connected");

            m_ctx = await Task.Run(
                () =>
                {
                    unsafe
                    {
                        AVFormatContext* ctx;

                        // Open the file with FFmpeg
                        if (FFmpegInvoke.avformat_open_input(&ctx, Uri, null, null) != 0)
                        {
                            this.Log().Debug("Couldn't open input file '{0}'", Uri);
                            return IntPtr.Zero;
                        }

                        if (FFmpegInvoke.avformat_find_stream_info(ctx, null) != 0)
                        {
                            this.Log().Debug("Couldn't find stream info for file '{0}'", Uri);
                            FFmpegInvoke.avformat_close_input(&ctx);
                            return IntPtr.Zero;
                        }

                        if (ctx->nb_streams < 1)
                        {
                            this.Log().Debug("No streams found for file '{0}'", Uri);
                            FFmpegInvoke.avformat_close_input(&ctx);
                            return IntPtr.Zero;
                        }

                        return new IntPtr(ctx);
                    }
                });

            if (m_ctx == IntPtr.Zero)
                return false;

            unsafe
            {
                // Clear default streams before initializing them.
                VideoStream = AudioStream = null;
                m_streams.Clear();

                AVFormatContext* ctx = (AVFormatContext*) m_ctx.ToPointer();

                for (int i = 0; i < ctx->nb_streams; i++)
                {
                    var stream = ctx->streams[i];

                    var ffmpegStream = new FFmpegInputStream(stream);
                    if (ffmpegStream.StreamType == StreamType.Video)
                        VideoStream = ffmpegStream;
                    else if (ffmpegStream.StreamType == StreamType.Audio)
                        AudioStream = ffmpegStream;

                    // Add the stream.
                    m_streams.Add(ffmpegStream);
                }

                //
                // Initialize metadata.
                //
                m_metadata.Clear();
                if (ctx->metadata != null)
                {
                    AVDictionaryEntry* entry = null;
                    while ((entry = FFmpegInvoke.av_dict_get(ctx->metadata, "", entry, FFmpegInvoke.AV_DICT_IGNORE_SUFFIX)) != null)
                    {
                        string key = new string((sbyte*)entry->key);
                        string value = new string((sbyte*)entry->value);

                        m_metadata[key] = value;
                    }
                }

                //
                // Update duration
                //
                double duration = (ctx->duration / (double)FFmpegInvoke.AV_TIME_BASE);
                if (duration < 0)
                    duration = 0;
                Duration = TimeSpan.FromTicks((long)(duration * 1000.0 * TimeSpan.TicksPerMillisecond));

                return true;
            }
        }

        /// <summary>
        /// The total stream duration (e.g. for discrete streams).
        /// </summary>
        public TimeSpan Duration { get; private set; }

        public bool IsConnected
        {
            get { return m_ctx != null; }
        }

        public void StartPlaying()
        {
            throw new NotImplementedException();
        }

        public void StopPlaying()
        {
            throw new NotImplementedException();
        }

        public IInputStream VideoStream { get; private set; }
        public IInputStream AudioStream { get; private set; }
        public IReadOnlyList<IInputStream> Streams { get; private set; }

        public IDictionary<string, string> Metadata
        {
            get { return m_metadata; }
        }

        #region IDisposable implementation

        ~FFmpegInputAdapter()
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

            StopPlaying();

            if (disposing)
            {
                foreach (var stream in m_streams)
                    stream.Dispose();
                m_streams.Clear();
                AudioStream = VideoStream = null;
            }

            unsafe
            {
                var ctx = (AVFormatContext*) m_ctx.ToPointer();
                if (ctx != null)
                {
                    FFmpegInvoke.avformat_close_input(&ctx);
                    m_ctx = IntPtr.Zero;
                }
            }
        }

        #endregion
    }
}
