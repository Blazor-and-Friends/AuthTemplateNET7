using AuthTemplateNET7.Shared.PublicModels;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels;
using Microsoft.EntityFrameworkCore;

namespace AuthTemplateNET7.Server.Data;

//added
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> opts) : base(opts) { }

    #region template provided entities

    #region Auth

    public DbSet<Login> Logins { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Role> Roles { get; set; }

    #endregion //auth

    public DbSet<Batch> Batches { get; set; }

    public DbSet<ContactMessage> ContactMessages { get; set; }

    public DbSet<Email> Emails { get; set; }

    public DbSet<LogItem> LogItems { get; set; }

    public DbSet<Recipient> Recipients { get; set; }

    public DbSet<SiteSetting> SiteSettings { get; set; }


    #endregion //template provided entities

    #region methods

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Recipient>().HasIndex(m => m.Address).IsUnique();

        modelBuilder.Entity<Email>().HasIndex(m => m.ToAddress).IsUnique(false);
        modelBuilder.Entity<SiteSetting>().HasIndex(m => m.Key).IsUnique();

        base.OnModelCreating(modelBuilder);
    }


    /// <summary>
    /// Wraps SaveChanges() in a try/catch. If the call throws an exception, the exception is added to the LogItems table. NOTE: If the call fails, the ChangeTracker is cleared.
    /// </summary>
    /// <param name="message">Any message you would like to include in the log item if the call fails.</param>
    /// <returns>Number of rows in the database affected if successful, -1 if exception thrown</returns>
    public int TrySave(string message = null)
    {
        try
        {
            return SaveChanges();
        }
        catch (Exception e)
        {
            ChangeTracker.Clear();

            LogItem logItem = new(e, message);
            LogItems.Add(logItem);

            try
            {
                SaveChanges();
            }
            catch { throw; }

            return -1;
        }
    }

    /// <summary>
    /// Wraps SaveChangesAsync() in a try/catch. If the call throws an exception, the exception is added to the LogItems table. NOTE: If the call fails, the ChangeTracker is cleared.
    /// </summary>
    /// <param name="message">Any message you would like to include in the log item if the call fails.</param>
    /// <returns>Number of rows in the database affected if successful, -1 if exception thrown</returns>
    public async Task<int> TrySaveAsync(string message = null)
    {
        try
        {
            return await SaveChangesAsync();
        }
        catch (Exception e)
        {
            ChangeTracker.Clear();

            LogItem logItem = new(e, message);
            LogItems.Add(logItem);

            try
            {
                await SaveChangesAsync();
            }
            catch { throw; }

            return -1;
        }
    }


    #endregion //methods
}
