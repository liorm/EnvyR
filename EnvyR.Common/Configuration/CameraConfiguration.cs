using System;

namespace EnvyR.Common.Configuration
{
    /// <summary>
    /// Holds configuration for a single camera.
    /// </summary>
    [Serializable]
    public class CameraConfiguration
    {
        public CameraConfiguration()
        {
            
        }

        /// <summary>
        /// The camera ID - That's how the camera is identified across the system.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The camera stream URL.
        /// </summary>
        public string Url { get; private set; }
    }
}
