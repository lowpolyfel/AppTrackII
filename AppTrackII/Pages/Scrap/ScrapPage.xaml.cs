using AppTrackII.ViewModels;

namespace AppTrackII.Pages.Scrap;

public partial class ScrapPage : ContentPage
{
    public ScrapPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ScrapViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }
}