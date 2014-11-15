using System;
using System.Collections;
using System.Collections.Generic;

namespace EnvyR.Common.Interfaces
{
    /// <summary>
    /// Represents a single input stream.
    /// </summary>
    public interface IInputStream
    {
        /// <summary>
        /// The stream type.
        /// </summary>
        StreamType StreamType { get; }

        /// <summary>
        /// The packets stream.
        /// </summary>
        IObservable<IPacket> PacketsStream { get; }
    }
}
