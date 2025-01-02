using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public abstract class BackgroundProcessing : IAsyncDisposable
    {
        protected static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task _backgroundTask;

        public void Start()
        {
            if (_backgroundTask != null && !_backgroundTask.IsCompleted)
            {
                throw new InvalidOperationException("Background processing is already running.");
            }

            // Start the background task on a separate thread
            _backgroundTask = Task.Run(() => ProcessInBackgroundAsync(_cancellationTokenSource.Token));
        }

        private async Task ProcessInBackgroundAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Work(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                Log.Info("Background task canceled.");
            }
            catch (Exception ex)
            {
                Log.Error("Error in background task.", ex);
            }
            finally
            {
                Log.Info("Background task stopped.");
            }
        }

        public async Task StopAsync()
        {
            _cancellationTokenSource.Cancel();

            if (_backgroundTask != null)
            {
                try
                {
                    await _backgroundTask;
                }
                catch (OperationCanceledException)
                {
                    // Expected when task is canceled.
                }
                catch (Exception ex)
                {
                    Log.Error("Exception during task shutdown.", ex);
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
            _cancellationTokenSource.Dispose();
        }

        protected abstract Task Work(CancellationToken cancellationToken);
    }
}
