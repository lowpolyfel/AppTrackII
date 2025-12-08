using AppTrackII.ViewModels;

namespace AppTrackII.Pages.Retrabajo;

public partial class RetrabajoPage : ContentPage
{
    public RetrabajoPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is RetrabajoViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }
}