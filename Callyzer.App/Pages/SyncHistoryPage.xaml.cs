using Microsoft.Maui.Controls;
using Callyzer.App.ViewModels;

namespace Callyzer.App.Pages
{
    public partial class SyncHistoryPage : ContentPage
    {
        private readonly SyncHistoryViewModel _viewModel;

        public SyncHistoryPage(SyncHistoryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadHistoryCommand.ExecuteAsync(null);
        }
    }
}
