using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvyR.Server.Utils
{
    static class TaskMixins
    {
        public static Task ContinueOnCurrent(this Task This, Action<Task> action)
        {
            return This.ContinueWith(action, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
