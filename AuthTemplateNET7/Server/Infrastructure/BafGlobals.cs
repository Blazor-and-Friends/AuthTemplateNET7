namespace AuthTemplateNET7.Server;

public class BafGlobals
{
    /// <summary>
    /// For differentiating environment variables for multiple websites using the same thread pool in IIS
    /// </summary>
    public string AppName { get; set; } = "AuthTemplate7";
    public DateTime AppStartTime { get; set; } = DateTime.UtcNow;
}
