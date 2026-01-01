using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project2.Services;

namespace Project2.WiewModels
{
    partial class CreateProfilePageWiew : ObservableObject
    {
        [ObservableProperty]
        private int age;
        [ObservableProperty]
        private double weight;
        [ObservableProperty]
        private double height;
        [ObservableProperty]
        private int gender;

        [RelayCommand]
        public async Task Profile()
        {
            var currentuser = UserSeassion.CurrentUser;

            if (currentuser == null)
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Hata", "Oturum bulunamadı. Lütfen tekrar giriş yapın.", "Tamam");
                return;
            }

            currentuser.Age = Age;
            currentuser.Height = Height;
            currentuser.Weight = Weight;
            currentuser.Gender = Gender;

            try
            {
                PersonValidator.Validate(currentuser);
                await App.Database.UpdatePersonAsync(currentuser);

                // C. Başarılı Mesajı ve Yönlendirme
                if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
                {
                    
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Başarılı", "Kayıt başarılı! Profil oluşturmaya yönlendiriliyorsunuz.", "Tamam");

                }
            }
            catch (ArgumentException ex)
            {
                // D. VALIDATOR HATASI YAKALAMA 🥅
                // Örneğin: "Şifre en az 1 büyük harf içermeli" hatası burada yakalanır.
                if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Uyarı", ex.Message, "Tamam");
            }
            catch (Exception ex)
            {
                // E. GENEL HATA (Veritabanı hatası vs.)
                if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Hata", "Bir sorun oluştu: " + ex.Message, "Tamam");
            }
        }
    } }