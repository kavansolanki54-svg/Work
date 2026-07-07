using Callyzer.App.ViewModels;

namespace Callyzer.App.Pages;

public partial class ContactDetailPage : ContentPage
{
    public ContactDetailPage(ContactDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
