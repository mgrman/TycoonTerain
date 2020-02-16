using System;
using System.Threading.Tasks;
using Votyra.Core.Logging;
using Votyra.Core.Utils;

namespace Votyra.Core.Queueing
{
    public class LastValueTaskQueue<T> : IWorkQueue<T> where T : IDisposable
    {
        private bool _stopped;
        private object _taskLock = new object();
        private bool _activeTask;
        private (bool HasValue, T Value) _queuedUpdate = (false, default);
        private TaskFactory _taskFactory;
        public event Action<T> DoWork;

        public LastValueTaskQueue()
        {
            _taskFactory = new TaskFactory();
        }

        public void QueueNew(T context)
        {
            bool startNewTask;
            lock (_taskLock)
            {
                DisposeAndSet(true, context);
                startNewTask = !_activeTask;
                _activeTask = true;
            }

            if (startNewTask)
            {
                _taskFactory.StartNew(TaskUpdate);
            }
        }

        private void TaskUpdate()
        {
            try
            {
                int counter = 0;
                while (true)
                {
                    (bool HasValue, T Value) activeContext;
                    lock (_taskLock)
                    {
                        activeContext = GetQueued();
                        _activeTask = activeContext.HasValue;
                        if (!_activeTask)
                        {
                            return;
                        }
                    }

                    try
                    {
                        DoWork?.Invoke(activeContext.Value);
                    }
                    catch (Exception ex)
                    {
                        StaticLogger.LogError($"Error in {this.GetType().GetDisplayName()}:");
                        StaticLogger.LogException(ex);
                    }
                    finally
                    {
                        try
                        {
                            activeContext.Value.Dispose();
                        }
                        catch (Exception ex)
                        {
                            StaticLogger.LogError($"Error disposing context {activeContext.GetHashCode()} in {this.GetType().GetDisplayName()}:");
                            StaticLogger.LogException(ex);
                        }
                    }

                    counter++;
                }
            }
            catch (Exception ex)
            {
                StaticLogger.LogException(ex);
            }
        }

        private void DisposeAndSet(bool hasValue, T value)
        {
            lock (_taskLock)
            {
                if (_queuedUpdate.HasValue)
                {
                    _queuedUpdate.Value?.Dispose();
                }

                _queuedUpdate = (hasValue, value);
            }
        }

        private (bool, T) GetQueued()
        {
            (bool, T) activeContext;
            lock (_taskLock)
            {
                activeContext = _queuedUpdate;
                _queuedUpdate = (false, default);
            }

            return activeContext;
        }
    }
}