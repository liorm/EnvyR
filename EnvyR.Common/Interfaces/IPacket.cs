using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvyR.Common.Interfaces
{
    /// <summary>
    /// Represents a stream packet.
    /// </summary>
    public interface IPacket
    {
        /// <summary>
        /// Timestamp for the packet.
        /// </summary>
        TimeSpan Timestamp { get; }
    }
}
