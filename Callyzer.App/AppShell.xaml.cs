namespace Callyzer.App;

/// <summary>
/// Application shell — defines tab-based navigation and registers detail routes.
/// </summary>
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register non-tab routes for push navigation
        Routing.RegisterRoute("contactdetail", typeof(Pages.ContactDetailPage));
        Routing.RegisterRoute("login", typeof(Pages.LoginPage));
        Routing.RegisterRoute("synchistory", typeof(Pages.SyncHistoryPage));
        Routing.RegisterRoute("comparecontacts", typeof(Pages.CompareContactsPage));
        Routing.RegisterRoute("export", typeof(Pages.ExportPage));
        Routing.RegisterRoute("backuprestore", typeof(Pages.BackupRestorePage));
    }
}
