namespace JacobDixon.AspNetCore.LiveWebTasks.Tasks
{
    public interface ITaskFactory
    {
        void Deregister(global::System.String name);
        ITask GetTask(global::System.String name, FileWatcherOptions options);
        void Register(global::System.String name, Type type);
    }
}