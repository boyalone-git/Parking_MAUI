using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace Parking_MAUI
{
    public partial class Connexion : ContentPage
    {
        public Connexion()
        {
            InitializeComponent();
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            string matricule = Matricule_text.Text;
            string password = Password_text.Text;

            try
            {
                var result = await VerifyCredentialsAsync(matricule, password);

                if (result != null && result.Status == "correct")
                {
                    // Ouvrir une nouvelle interface ou une nouvelle fenêtre ici
                    await Navigation.PushAsync(new Acceuil());
                }
                else
                {
                    await DisplayAlert("Erreur", result?.Message ?? "Erreur inconnue.", "OK");
                }
            }
            catch (HttpRequestException ex)
            {
                await DisplayAlert("Erreur de connexion", $"Erreur de connexion : {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Erreur : {ex.Message}", "OK");
            }
        }

        private async Task<ResponseModel?> VerifyCredentialsAsync(string matricule, string password)
        {
            using var client = new HttpClient();
            var request = new
            {
                matricule = matricule,
                password = password
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            try
            {
                response = await client.PostAsync("http://172.20.10.3/Apis/connection.php", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Code de statut : {response.StatusCode}. Réponse : {errorResponse}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Ajout de détails pour le débogage
                await DisplayAlert("Erreur de connexion", $"Erreur lors de la requête HTTP : {ex.Message}", "OK");
                throw;
            }

            var responseString = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ResponseModel>(responseString);
        }

    }

    public class ResponseModel
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}
