using AuthTemplateNET7.Shared.PublicModels;

namespace AuthTemplateNET7.Server.Data;

public class RepoBase
{
    protected readonly DataContext dataContext;

    public RepoBase(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    /// <summary>
    /// Creates and saves a log item
    /// </summary>
    public Task CreateLogItemAsync(Exception e, string message = null)
    {
        LogItem logItem = new(e, message);
        return CreateLogItemAsync(logItem);
    }

    /// <summary>
    /// Creates and saves a log item
    /// </summary>
    public Task CreateLogItemAsync(LogItem item)
    {
        dataContext.Add(item);
        return dataContext.TrySaveAsync();
    }
}
