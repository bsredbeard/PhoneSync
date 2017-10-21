using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PhoneSync
{
    static class TaskExtensions
    {
        public static void SyncTo<T>(this Task<T> task, SynchronizationContext sync, Action<T> handler)
        {
            task.ContinueWith(t =>
            {
                sync.Post(state =>
                {
                    handler((T)state);
                }, t.Result);
            });
        }
    }
}
