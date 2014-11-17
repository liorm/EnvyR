using System;

namespace EnvyR.Common.Configuration
{
    /// <summary>
    /// Holds configuration for a single camera.
    /// </summary>
    [Serializable]
    public struct CameraConfiguration
    {
        /// <summary>
        /// Initialize a new configuration instance.
        /// </summary>
        public CameraConfiguration(string url) : this()
        {
            Id = Guid.NewGuid();
            Url = url;
        }

        /// <summary>
        /// The camera ID - That's how the camera is identified across the system.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The camera stream URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The camera display name.
        /// </summary>
        public string Name { get; set; }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Id, Name);
        }
    }
}
