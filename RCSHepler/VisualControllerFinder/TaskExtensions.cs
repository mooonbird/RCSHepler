using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCSHepler.VisualControllerFinder
{
    public static class TaskExtensions
    {
        public async static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            var cancelSource = new CancellationTokenSource();
            var delay = Task.Delay(timeout, cancelSource.Token);
            Task winner = await Task.WhenAny(task, delay).ConfigureAwait(false);
            if (winner == task) cancelSource.Cancel();
            else
                throw new TimeoutException();

            return await task.ConfigureAwait(false);
        }

        public async static Task WithTimeout(this Task task, TimeSpan timeout)
        {
            var cancelSource = new CancellationTokenSource();
            var delay = Task.Delay(timeout, cancelSource.Token);
            Task winner = await Task.WhenAny(task, delay).ConfigureAwait(false);
            if (winner == task) cancelSource.Cancel();
            else
                throw new TimeoutException();

            await task.ConfigureAwait(false);
        }
    }
}
