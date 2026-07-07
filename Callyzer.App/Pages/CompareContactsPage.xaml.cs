using Microsoft.Maui.Controls;
using Callyzer.App.ViewModels;

namespace Callyzer.App.Pages
{
    public partial class CompareContactsPage : ContentPage
    {
        public CompareContactsPage(CompareContactsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
