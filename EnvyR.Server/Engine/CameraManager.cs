using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvyR.Common.Configuration;

namespace EnvyR.Server.Engine
{
    /// <summary>
    /// Manages the camera. Holds the adapter along with the configuration.
    /// </summary>
    class CameraManager
    {
        public CameraManager(CameraConfiguration config)
        {
            m_config = config;

            // TODO: Initialize the adapter according to the configuration.
        }

        private readonly CameraConfiguration m_config;
    }
}
