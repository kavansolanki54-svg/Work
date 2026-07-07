using Microsoft.Maui.Controls;
using Callyzer.App.ViewModels;

namespace Callyzer.App.Pages
{
    public partial class ExportPage : ContentPage
    {
        public ExportPage(ExportViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
