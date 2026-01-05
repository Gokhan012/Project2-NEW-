
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project2.Models;
using Project2.Services;
using System;
using System.Threading.Tasks;

namespace Project2.WiewModels
{
    // Toolkit otomatik olarak 'Command' takılarını ekleyecektir.
    public partial class CreateProfilePageView : ObservableObject
    {
        // --- Form Alanları ---

        [ObservableProperty]
        private DateTime _birthDate = DateTime.Now.AddYears(-20); // Varsayılan 20 yıl önce

        [ObservableProperty]
        private double _weight;

        [ObservableProperty]
        private double _height;

        [ObservableProperty]
        private string _selectedGender = ""; // "K" veya "E"

        // --- Dinamik Renk Mülkleri ---

        public Color FemaleBtnColor => SelectedGender == "K" ? Colors.DeepPink : Colors.Transparent;
        public Color MaleBtnColor => SelectedGender == "E" ? Colors.DodgerBlue : Colors.Transparent;

        // --- Cinsiyet Seçim Komutları ---

        [RelayCommand]
        public void SelectFemale()
        {
            SelectedGender = "K";
            OnPropertyChanged(nameof(FemaleBtnColor));
            OnPropertyChanged(nameof(MaleBtnColor));
        }

        [RelayCommand]
        public void SelectMale()
        {
            SelectedGender = "E";
            OnPropertyChanged(nameof(FemaleBtnColor));
            OnPropertyChanged(nameof(MaleBtnColor));
        }

        // --- Kaydetme (Profil Oluştur) Komutu ---

        [RelayCommand]
        public async Task Profile()
        {
            var currentuser = UserSeassion.CurrentUser;
            if (currentuser == null) return;

            try
            {
                // 1. Yaş Hesaplama (DateTime -> int)
                int age = DateTime.Now.Year - BirthDate.Year;
                if (BirthDate > DateTime.Now.AddYears(-age)) age--;

                // 2. Verileri Modele Aktar
                currentuser.Age = age;
                currentuser.Height = Height;
                currentuser.Weight = Weight;

                // Cinsiyet Karşılığı (Örn: Erkek 1, Kadın 2)
                currentuser.Gender = SelectedGender == "E" ? 1 : (SelectedGender == "K" ? 2 : 0);

                // 3. Doğrulama (Validator)
                PersonValidator.Validate(currentuser);

                // 4. Veritabanı Güncelleme
                await App.Database.UpdatePersonAsync(currentuser);

                // 5. Başarılı Mesajı ve Navigasyon
                await Application.Current.MainPage.DisplayAlert("Başarılı", $"Profiliniz oluşturuldu! (Yaş: {age})", "Tamam");
                await Application.Current.MainPage.Navigation.PushAsync(new MainDashboardPage());
            }
            catch (ArgumentException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Uyarı", ex.Message, "Tamam");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", "Beklenmedik bir sorun oluştu.", "Tamam");
            }
        }

        // --- Atla Komutu ---

        [RelayCommand]
        public async Task SelectSkip()
        {
            bool answer = await Application.Current.MainPage.DisplayAlert("Emin misiniz?", "Profil bilgileriniz su hedefi hesaplamak için kullanılır. Atlamak istiyor musunuz?", "Evet", "Hayır");

            if (answer)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new MainDashboardPage());
            }
        }
    }
}