namespace Nulah.Discord.Bot.Modules.TaskRunner.Models {
    public abstract class PeriodicTask {
        /// <summary>
        /// Entry point for when the task is executed. No other method will be called by the
        /// task runner, and any instance of PeriodicTask will share all properties between calls unless
        /// defaulted by implementations.
        /// </summary>
        public abstract void Execute();
    }
}
