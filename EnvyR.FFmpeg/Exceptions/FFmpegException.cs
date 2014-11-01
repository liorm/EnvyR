using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvyR.FFmpeg.Exceptions
{
    class FFmpegException : Exception
    {
        public FFmpegException(string message) : 
            base(message)
        {
        }
    }
}
