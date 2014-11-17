using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnvyR.Server.Services;
using Splat;

namespace EnvyR.Server.Utils
{
    /// <summary>
    /// Base class for application that implement <see cref="SynchronizationContext"/>. Needed for proper async/await handling.
    /// </summary>
    abstract class AsyncApplication
    {
        public AsyncApplication()
        {
            m_syncContext = new SingleThreadSynchronizationContext();
        }

        public void Run()
        {
            var prevCtx = SynchronizationContext.Current;
            try
            {
                // Establish the new context
                SynchronizationContext.SetSynchronizationContext(m_syncContext);

                // Init stuff.
                if (!OnInit())
                    return;

                // Start pumping.
                m_syncContext.RunOnCurrentThread();

                // Wrap up.
                OnExit();
            }
            finally { SynchronizationContext.SetSynchronizationContext(prevCtx); }
        }

        private readonly SingleThreadSynchronizationContext m_syncContext;

        /// <summary>
        /// Stop the application.
        /// </summary>
        public void Quit()
        {
            // Signal the loop to quit.
            m_syncContext.Complete();
        }

        /// <summary>
        /// Initialization function.
        /// </summary>
        protected abstract bool OnInit();

        /// <summary>
        /// Called to cleanup.
        /// </summary>
        protected virtual void OnExit()
        {
            // Noop.
        }
    }
}
