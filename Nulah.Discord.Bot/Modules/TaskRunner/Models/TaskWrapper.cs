using System;
using System.Threading.Tasks;

namespace Nulah.Discord.Bot.Modules.TaskRunner.Models {
    public class TaskWrapper {
        public TimeSpan Interval { get; set; }
        public DateTime NextRun { get; set; }
        public bool isExecuting { get; set; }
        public PeriodicTask PeriodicTask { get; set; }

        private readonly Guid TaskId;
        private Guid RunId { get; set; }

        public TaskWrapper() {
            TaskId = Guid.NewGuid();
        }

        public void Run(NulahTaskRunner taskRunner) {
            if(isExecuting == false) {
                RunId = Guid.NewGuid();
                isExecuting = true;

                var taskEvent = new TaskEvent {
                    Message = "Task started execution",
                    RunId = RunId,
                    TaskId = TaskId,
                    Started_UTC = DateTime.UtcNow
                };

                taskRunner.TaskStarted(taskEvent);

                Task.Run(() => {
                    PeriodicTask.Execute();
                    isExecuting = false;
                    NextRun = NextRun + Interval;

                    taskEvent.Completed_UTC = DateTime.UtcNow;
                    taskEvent.Duration = taskEvent.Completed_UTC - taskEvent.Started_UTC;
                    taskEvent.NextRun_UTC = NextRun;
                    taskEvent.Message = "Task completed";

                    taskRunner.TaskStarted(taskEvent);
                });
            }
        }
    }
}
