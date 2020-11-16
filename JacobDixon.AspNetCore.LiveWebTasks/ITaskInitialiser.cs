namespace JacobDixon.AspNetCore.LiveWebTasks
{
    public interface ITaskInitialiser
    {
        void StartFileWatchers();
        void StopFileWatchers();
    }
}