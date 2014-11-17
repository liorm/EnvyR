using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnvyR.Common.Configuration;
using EnvyR.Common.Interfaces;
using Splat;

namespace EnvyR.Server.Engine
{
    /// <summary>
    /// Manages the camera. Holds the adapter along with the configuration.
    /// </summary>
    class CameraManager : IEnableLogger, IDisposable
    {
        public CameraManager(CameraConfiguration config)
        {
            m_config = config;
            LoggingContext = m_config.ToString();

            m_adapter = FFmpeg.Factory.CreateAdapter(m_config.Url);
            // TODO: Initialize the adapter according to the configuration.
        }

        public string LoggingContext { get; private set; }

        public void Start()
        {
            if (m_startedTask != null)
                throw new InvalidOperationException("Already started");

            m_startedTask = StartAsync();
        }

        private readonly CameraConfiguration m_config;
        private readonly IInputAdapter m_adapter;

        private async Task StartAsync()
        {
            if (m_disposedTokenSource.IsCancellationRequested)
                throw new ObjectDisposedException(GetType().Name);

            this.Log().Debug("Camera '{0}' starting", m_config.ToString());

            // Retry connection until cancelled.
            while (!await m_adapter.ConnectAsyncTask(m_disposedTokenSource.Token))
            {
                this.Log().Debug("Camera '{0}' failed to connect. Waiting before retry.", m_config.ToString());
                await Task.Delay(TimeSpan.FromSeconds(5), m_disposedTokenSource.Token);

                // Check for cancellation.
                m_disposedTokenSource.Token.ThrowIfCancellationRequested();
            }

            if (m_disposedTokenSource.Token.IsCancellationRequested)
            {
                this.Log().Debug("Camera '{0}' connection cancelled", m_config.ToString());
                return;
            }

            // Probably connected... start playing.
            m_adapter.StartPlaying(new SynchronizationContextScheduler(SynchronizationContext.Current));
            this.Log().Debug("Camera '{0}' started", m_config.ToString());
        }

        private readonly CancellationTokenSource m_disposedTokenSource = new CancellationTokenSource();
        private Task m_startedTask = null;

        public void Dispose()
        {
            // Cancel any pending requests.
            m_disposedTokenSource.Cancel();

            // Never started?
            if (m_startedTask == null)
                return;

            // And wait for the connection task to complete.
            if (!m_startedTask.IsCanceled)
                m_startedTask.Wait();

            // Dispose of the adapter.
            m_adapter.StopPlaying();
            m_adapter.Dispose();

            this.Log().Debug("Camera '{0}' stopped", m_config.ToString());
        }
    }
}
