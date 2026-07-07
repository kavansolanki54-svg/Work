using Callyzer.App.ViewModels;

namespace Callyzer.App.Pages;

public partial class CallHistoryPage : ContentPage
{
    private readonly CallHistoryViewModel _viewModel;

    public CallHistoryPage(CallHistoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.CallLogs.Count == 0)
        {
            await _viewModel.LoadCallLogsCommand.ExecuteAsync(null);
        }
    }
}
