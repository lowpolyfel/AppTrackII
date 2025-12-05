using AppTrackII.ViewModels;

namespace AppTrackII.Pages.Scan;

public partial class ScanPage : ContentPage
{
    public ScanPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ScanViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }
}
