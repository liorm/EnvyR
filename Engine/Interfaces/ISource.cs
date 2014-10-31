using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvyR.Server.Engine.Interfaces
{
    class Packet
    {
        
    }

    interface IVideoSource
    {
        // TODO: Move from ISource
    }

    interface IAudioSource
    {
        // TODO: Move from ISource
    }

    interface ISource : IVideoSource, IAudioSource
    {
        IObservable<Packet> VideoStream { get; }
        IObservable<Packet> AudioStream { get; }

        void Start();
    }
}

