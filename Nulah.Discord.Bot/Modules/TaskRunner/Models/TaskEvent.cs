using System;

namespace Nulah.Discord.Bot.Modules.TaskRunner.Models {
    public class TaskEvent {
        public string Message { get; set; }
        public Guid TaskId { get; set; }
        public Guid RunId { get; set; }
        public DateTime Started_UTC { get; set; }
        public DateTime? Completed_UTC { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? NextRun_UTC { get; set; }
    }
}
