using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvyR.Server.Services
{
    interface IApp
    {
        /// <summary>
        /// Call to terminate the application.
        /// </summary>
        void Quit();

        /// <summary>
        /// Called when application exits.
        /// </summary>
        event EventHandler AppExit;
    }
}
