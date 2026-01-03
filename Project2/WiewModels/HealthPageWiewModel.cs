using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project2.Models;
using Project2.Services;
using Project2.Pages;

namespace Project2.WiewModels
{
    public partial class HealthPageWiewModel : ObservableObject
    {
        // 1. Ekrana bağlanacak veri (ObservableProperty sayesinde ekran güncellenir)
        [ObservableProperty]
        private Person? _currentUser;

        // Sayfa açılınca veya geri dönülünce bu metodu çağıracağız
        public void LoadUserData()
        {
            if (UserSeassion.CurrentUser != null)
            {
                CurrentUser = UserSeassion.CurrentUser;
            }
        }

        // --- KİLO DEĞİŞTİRME KOMUTU ---
        [RelayCommand]
        public async Task KgChancher()
        {
            // 1. Güvenlik Kontrolü: Kullanıcı giriş yapmış mı?
            if (CurrentUser == null) return;

            // 2. Küçük Pencereyi Aç (DisplayPromptAsync)
            string result = await Application.Current.MainPage.DisplayPromptAsync(
                title: "Kilo Güncelle",
                message: "Güncel kilonuzu giriniz:",
                accept: "Güncelle",
                cancel: "İptal",
                placeholder: CurrentUser.Weight?.ToString() ?? "0", // Mevcut kiloyu ipucu göster
                keyboard: Keyboard.Numeric); // Sadece sayı klavyesi aç

            // 3. Kullanıcı iptale bastıysa veya boş bıraktıysa çık
            if (string.IsNullOrWhiteSpace(result)) return;

            // 4. Girilen yazıyı sayıya (double) çevirmeyi dene
            if (double.TryParse(result, out double newWeight))
            {
                // A. Hafızadaki veriyi güncelle
                CurrentUser.Weight = newWeight;

                // B. Veritabanını güncelle
                await App.Database.UpdatePersonAsync(CurrentUser);

                // C. Ekranı Yenile (Binding'i tetikler)
                // Bu satır çok önemli, yoksa ekrandaki sayı değişmez!
                OnPropertyChanged(nameof(CurrentUser));
                await Application.Current.MainPage.Navigation.PushAsync(new HealthPage());
            }
            else
            {
                // Sayı girilmediyse hata ver
                await Application.Current.MainPage.DisplayAlert("Hata", "Lütfen geçerli bir sayı giriniz.", "Tamam");
            }
        }
    }
}