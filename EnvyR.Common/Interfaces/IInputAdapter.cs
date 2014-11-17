using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EnvyR.Common.Interfaces
{
    /// <summary>
    /// Stream input adapter.
    /// </summary>
    public interface IInputAdapter : IDisposable
    {
        /// <summary>
        /// Initiate connection to the camera.
        /// </summary>
        /// <remarks>Most functions will not work until adapter is connected</remarks>
        /// <returns>success or failure</returns>
        Task<bool> ConnectAsyncTask(CancellationToken token);

        /// <summary>
        /// Return true if the adapter is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Start packets pumping.
        /// </summary>
        /// <param name="processingScheduler">Received packets should be Observed On this scheduler</param>
        void StartPlaying(IScheduler processingScheduler);

        /// <summary>
        /// Stop packets pumping.
        /// </summary>
        void StopPlaying();

        /// <summary>
        /// Returns the main video stream.
        /// </summary>
        IInputStream VideoStream { get; }

        /// <summary>
        /// Returns the main audio stream.
        /// </summary>
        IInputStream AudioStream { get; }

        /// <summary>
        /// Give access to all the internal streams.
        /// </summary>
        IReadOnlyList<IInputStream> Streams { get; }

        /// <summary>
        /// Additional metadata obtained from the camera.
        /// </summary>
        IDictionary<string, string> Metadata { get; }
    }
}
