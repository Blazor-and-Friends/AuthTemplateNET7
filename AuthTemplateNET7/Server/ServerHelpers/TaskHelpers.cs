namespace AuthTemplateNET7.Server.ServerHelpers;

//added

public static class TaskHelpers
{
    //courtesy https://www.meziantou.net/fire-and-forget-a-task-in-dotnet.htm

    public static void Forget(this Task task)
    {
        if(!task.IsCompleted || task.IsFaulted)
        {
            _ = forgetAwaited(task);
        }
    }

    async static Task forgetAwaited(Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch { }
    }
}
