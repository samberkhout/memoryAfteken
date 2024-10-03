using memoryAfteken.pagina;
using Microsoft.Maui.Controls;

namespace memoryAfteken;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Use a NavigationPage to enable navigation
        MainPage = new NavigationPage(new MainPage());
    }
}