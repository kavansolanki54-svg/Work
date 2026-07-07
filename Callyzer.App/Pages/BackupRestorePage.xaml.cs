using Microsoft.Maui.Controls;
using Callyzer.App.ViewModels;

namespace Callyzer.App.Pages
{
    public partial class BackupRestorePage : ContentPage
    {
        public BackupRestorePage(BackupRestoreViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is BackupRestoreViewModel vm)
            {
                vm.LoadBackupsCommand.Execute(null);
            }
        }
    }
}
