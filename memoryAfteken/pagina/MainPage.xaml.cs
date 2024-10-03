using memoryAfteken.pagina;
using Microsoft.Maui.Controls;

namespace memoryAfteken.pagina;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnStartGameButtonClicked(object sender, EventArgs e)
    {
        // Determine the number of pairs based on the selected difficulty
        int numberOfPairs = DifficultyPicker.SelectedIndex switch
        {
            0 => 4,  // Easy
            1 => 8,  // Medium
            2 => 12, // Hard
            _ => 8   // Default to Medium if none is selected
        };

        // Navigate to the GamePage with the selected difficulty
        await Navigation.PushAsync(new GamePage(numberOfPairs));
    }

    private async void OnViewHighScoresClicked(object sender, EventArgs e)
    {
        // Navigate to the HighScoresPage
        await Navigation.PushAsync(new HighScoresPage());
    }
}