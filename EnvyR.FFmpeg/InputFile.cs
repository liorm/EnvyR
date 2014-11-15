﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnvyR.Common.Interfaces;
using EnvyR.FFmpeg.Exceptions;
using EnvyR.FFmpeg.Managed;
using EnvyR.FFmpeg.Stream;
using FFmpeg.AutoGen;
using System.IO;
using System.Collections.ObjectModel;
using AVPacket = EnvyR.FFmpeg.Managed.AVPacket;

namespace EnvyR.FFmpeg
{
    /// <summary>
    /// Represents an ffmpeg input stream.
    /// </summary>
    /// TODO: MOVE TO FFmpegInputAdapter
    class InputFile : IDisposable
    {

        /// <summary>
        /// Construct the stream for the given url.
        /// </summary>
        /// <param name="uri"></param>
        public InputFile(string uri)
        {
            if (String.IsNullOrEmpty(uri))
                throw new ArgumentNullException("uri");

            Uri = uri;
        }

        /// <summary>
        /// Request to open the file asynchroneously.
        /// </summary>
        public async Task OpenFileAsync()
        {
            m_ctx = await Task.Run(
                () =>
                {
                    unsafe
                    {
                        AVFormatContext* ctx;

                        // Open the file with FFmpeg
                        if ( FFmpegInvoke.avformat_open_input(&ctx, Uri, null, null) != 0 )
                            throw new FFmpegException("Couldn't open file");

                        if ( FFmpegInvoke.avformat_find_stream_info(ctx, null) != 0 )
                            throw new FFmpegException("Couldn't find stream info");

                        return new IntPtr(ctx);
                    }
                });

            unsafe
            {
                AVFormatContext* ctx = (AVFormatContext*)m_ctx.ToPointer();

                if (ctx->nb_streams < 1)
                    throw new FFmpegException("No streams found");

                for (int i = 0; i < ctx->nb_streams; i++)
                {
                    var stream = ctx->streams[i];

                    switch (stream->codec->codec_type)
                    {
                        case AVMediaType.AVMEDIA_TYPE_VIDEO:
                            // TODO: Implement specific stream
                            m_streams.Add(i, new CodecStream(stream));
                            break;
                        case AVMediaType.AVMEDIA_TYPE_AUDIO:
                            // TODO: Implement specific stream
                            m_streams.Add(i, new CodecStream(stream));
                            break;
                        default:
                            m_streams.Add(i, null);
                            break;
                    }
                }

                if (ctx->metadata != null)
                {
                    AVDictionaryEntry* entry = null;
                    while ((entry = FFmpegInvoke.av_dict_get(ctx->metadata, "", entry, FFmpegInvoke.AV_DICT_IGNORE_SUFFIX)) != null)
                    {
                        string key = new string((sbyte*)entry->key);
                        string value = new string((sbyte*)entry->value);

                        Console.WriteLine("Metadata: {0} == {1}", key, value);
                    }
                }

                //
                // Update duration
                //
                double duration = (ctx->duration / (double)FFmpegInvoke.AV_TIME_BASE);
                if (duration < 0)
                    duration = 0;
                Duration = TimeSpan.FromTicks((long)(duration * 1000.0 * TimeSpan.TicksPerMillisecond));
            }
        }

        /// <summary>
        /// Start the reader loop.
        /// </summary>
        public void StartRunning()
        {
            if (m_ctx == IntPtr.Zero || m_streams == null)
                throw new InvalidOperationException("File not open");

            if ( m_readerThread != null )
                throw new InvalidOperationException("Already running");

            m_readerThread = new Thread(ReaderThreadProc)
                             {
                                 IsBackground = true,
                                 Name = "InputFile Reader"
                             };
            m_stopRunning = false;
            m_readerThread.Start();
        }

        public void StopRunning()
        {
            if (m_readerThread != null)
            {
                m_stopRunning = true;
                m_readerThread.Join();
                m_readerThread = null;
            }
        }

        private IntPtr m_ctx = IntPtr.Zero;
        private Thread m_readerThread;
        private bool m_stopRunning = false;

        /// <summary>
        /// The input file name.
        /// </summary>
        public string Uri { get; private set; }

        /// <summary>
        /// Duration of the stream
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// The internal list of streams.
        /// </summary>
        public ReadOnlyCollection<CodecStream> Streams
        {
            get { return new ReadOnlyCollection<CodecStream>(m_streams.Values); }
        }
        private readonly SortedList<int, CodecStream> m_streams = new SortedList<int, CodecStream>();

        private void ReaderThreadProc()
        {
            while ( !m_stopRunning )
            {
                if ( !ReadPacket() )
                    break;
            }

            // Mark streams as finished.
            foreach (var stream in m_streams)
                stream.Value.m_subject.OnCompleted();
        }

        /// <summary>
        /// Read a single packet and forward it to the streams.
        /// </summary>
        bool ReadPacket()
        {
            unsafe
            {
                var packet = new AVPacket();

                fixed (global::FFmpeg.AutoGen.AVPacket* p = &packet.Packet)
                    if (FFmpegInvoke.av_read_frame((AVFormatContext*)m_ctx.ToPointer(), p) < 0)
                        return false;

                var dest = m_streams[packet.Packet.stream_index];
                if (dest != null)
                {
                    packet.Stream = dest;
                    dest.m_subject.OnNext(packet);
                }
                else
                    packet.Dispose();

                return true;
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
            if ( m_disposed ) 
                return;
            m_disposed = true;

            if (disposing)
            {
                StopRunning();

                foreach ( var stream in m_streams )
                    stream.Value.Dispose();
                m_streams.Clear();
            }

            unsafe
            {
                var ctx = (AVFormatContext*)m_ctx.ToPointer();
                FFmpegInvoke.avformat_close_input(&ctx);
                m_ctx = IntPtr.Zero;
            }
        }

        #endregion
    }
}
