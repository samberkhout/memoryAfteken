using memoryAfteken.Business;
using memoryAfteken.Models;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using memoryAfteken.DataAccess;
using System.Threading.Tasks;

namespace memoryAfteken.pagina;

public partial class HighScoresPage : ContentPage
{
    private readonly IGameRepository _repository;

    public HighScoresPage()
    {
        InitializeComponent();
        _repository = new GameRepository();
        LoadHighScoresAsync();
    }

    private async void LoadHighScoresAsync()
    {
        try
        {
            // Get the high scores from the repository asynchronously
            List<HighScore> highScores = await _repository.GetHighScoresAsync();

            // Bind the high scores to the ListView
            HighScoresListView.ItemsSource = highScores;
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., show an alert to the user)
            await DisplayAlert("Error", $"Failed to load high scores: {ex.Message}", "OK");
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        // Navigate back to the previous page (MainPage)
        await Navigation.PopAsync();
    }
}