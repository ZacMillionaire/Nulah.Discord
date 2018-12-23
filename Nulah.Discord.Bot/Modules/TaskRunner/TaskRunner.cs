using Nulah.Discord.Bot.Modules.TaskRunner.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Nulah.Discord.Bot.Modules.TaskRunner {
    public class NulahTaskRunner {

        private List<TaskWrapper> _tasks { get; set; }

        private bool isPolling { get; set; } = false;

        public event EventHandler<TaskEvent> LogEvent;

        Timer t;

        public NulahTaskRunner() {
            _tasks = new List<TaskWrapper>();
        }

        /// <summary>
        /// Registers a given Periodic task to run every interval. Can be called after Start(),
        /// and any events registered this way will be added according to the current time added + interval
        /// if registered events are currently being polled.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="interval"></param>
        public void RegisterTask(PeriodicTask task, TimeSpan interval) {
            if(isPolling == false) {
                _tasks.Add(new TaskWrapper {
                    PeriodicTask = task,
                    Interval = interval
                });
            } else {
                _tasks.Add(new TaskWrapper {
                    PeriodicTask = task,
                    Interval = interval,
                    NextRun = DateTime.UtcNow + interval
                });
            }
        }
        public void RegisterTask(PeriodicTask task, TimeSpan interval, bool startImmediately) {
            // TODO: override to register a task that will have a next run of its interval, instead of immediate
            // invocation from init
            throw new NotImplementedException("Immediate task starting not yet implemented");
        }

        /// <summary>
        /// Immediately executes all tasks registered and starts the polling timer
        /// </summary>
        public void Start() {
            Start(new TimeSpan());
        }

        public void Start(TimeSpan delay) {
            isPolling = true;
            Init(delay);
            t = new Timer(new TimerCallback(Poll), null, 0, 1000);
        }

        /// <summary>
        /// Sets the next run of all tasks to be immediate
        /// </summary>
        void Init(TimeSpan delay) {
            foreach(var task in _tasks) {
                task.NextRun = DateTime.UtcNow + delay;
            }
        }

        /// <summary>
        /// Loops over all registered tasks, executing any that have a next run time in the past (the current time is greater than the next run time)
        /// and where the task is already not executing, if the task happens to be a long running execution. Running tasks that get skipped in this manner
        /// will have their next runs updated correctly to execute at their next available interval.
        /// </summary>
        /// <param name="state"></param>
        void Poll(object state) {
            foreach(var task in _tasks) {
                if(task.isExecuting == false && task.NextRun <= DateTime.UtcNow) {
                    task.Run(this);
                }
            }
        }

        /// <summary>
        /// Method for raising task event messages
        /// </summary>
        /// <param name="taskEvent"></param>
        internal void TaskStarted(TaskEvent taskEvent) {
            OnEvent(taskEvent);
        }

        /// <summary>
        /// Invokes all registered event listeners to handle the task event as required
        /// </summary>
        /// <param name="e"></param>
        void OnEvent(TaskEvent e) {
            LogEvent?.Invoke(this, e);
        }
    }
}
