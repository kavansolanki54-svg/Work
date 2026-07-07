using Callyzer.App.ViewModels;

namespace Callyzer.App.Pages;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Initial state for animation
        this.Content.Opacity = 0;
        this.Content.TranslationY = 20;

        await _viewModel.LoadDashboardCommand.ExecuteAsync(null);

        // Entrance animation
        await Task.WhenAll(
            this.Content.FadeTo(1, 400, Easing.CubicOut),
            this.Content.TranslateTo(0, 0, 400, Easing.CubicOut)
        );
    }
}
