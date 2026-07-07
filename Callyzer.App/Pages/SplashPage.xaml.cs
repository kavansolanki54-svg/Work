using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Callyzer.App.ViewModels;

namespace Callyzer.App.Pages
{
    public partial class SplashPage : ContentPage
    {
        private readonly SplashViewModel _viewModel;
        private bool _hasInitialized;

        public SplashPage(SplashViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_hasInitialized) return;
            _hasInitialized = true;

            // Animate logo entrance
            await Task.WhenAll(
                LogoContainer.FadeTo(1, 800, Easing.CubicOut),
                LogoContainer.ScaleTo(1, 800, Easing.SpringOut)
            );

            // Start initialization logic
            await _viewModel.InitializeAppCommand.ExecuteAsync(null);
        }
    }
}
