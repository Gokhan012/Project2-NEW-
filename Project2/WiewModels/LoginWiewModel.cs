using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Project2.Models;
using Project2.Pages;
using Project2.Services;


namespace Project2.WiewModels;

partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string email = "";
    [ObservableProperty]
    private string password = "";
    [RelayCommand]
    public async Task Login()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Hata", "Lütfen tüm alanları doldurun.", "Tamam");
            return;
        }else if(string.IsNullOrWhiteSpace(Password) || Password.Length < 5)
        {
            await Application.Current.MainPage.DisplayAlert("Hata", "Lütfen geçerli bir şifre girin (en az 5 karakter).", "Tamam");
            return;
        }else if(string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
        {
            await Application.Current.MainPage.DisplayAlert("Hata", "Lütfen geçerli bir e-posta adresi girin.", "Tamam");
            return;
        }
           var user = await App.Database.GetByEmailAsync(Email.Trim());
        if (user == null || user.Password != Password)
        {
            await Application.Current.MainPage.DisplayAlert("Hata", "E-posta veya şifre yanlış.", "Tamam");
            return;
        }
        else
        {
            UserSeassion.CurrentUser = user;
            var waterRecord = await App.Database.GetWaterRecordByDateAsync(DateTime.Now, user.ID);

            if (waterRecord == null)
            {
                // Bugün hiç su kaydı yoksa yeni bir tane oluştur
                waterRecord = new tblWater
                {
                    PersonId = user.ID,
                    WaterDrink = 0,
                    WaterNeeded = 2500, // Varsayılan hedef
                    Date = DateTime.Now.Date
                };
                await App.Database.SaveWaterAsync(waterRecord);
            }

            UserSeassion.CurrentlWater = waterRecord; 

            await Application.Current.MainPage.DisplayAlert("Başarılı", $"Hoşgeldin {user.Name}", "Devam");
            await Application.Current.MainPage.Navigation.PushAsync(new MainDashboardPage());
        }
    }
}
