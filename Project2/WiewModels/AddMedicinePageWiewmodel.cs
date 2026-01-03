using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project2.Models;
using Project2.Services;

namespace Project2.WiewModels
{
    public partial class AddMedicinePageWiewmodel : ObservableObject
    {
        // --- EKRANDAKİ GİRİŞLERE BAĞLANACAK DEĞİŞKENLER ---

        [ObservableProperty]
        private string _ilacAdi;

        [ObservableProperty]
        private string _doz;

        [ObservableProperty]
        private TimeSpan _secilenSaat = DateTime.Now.TimeOfDay;

        // --- KAYDETME KOMUTU ---
        [RelayCommand]
        public async Task Kaydet()
        {
            // 1. Kontroller
            if (string.IsNullOrWhiteSpace(IlacAdi))
            {
                await Application.Current.MainPage.DisplayAlert("Hata", "Lütfen ilaç adını giriniz.", "Tamam");
                return;
            }

            // Kullanıcı oturumu kontrolü (Çok önemli)
            if (UserSeassion.CurrentUser == null)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", "Kullanıcı oturumu yok.", "Tamam");
                return;
            }

            // 2. Dozajı Çevir
            double.TryParse(Doz, out double dozMiktari);

            // 3. Veritabanı Nesnesini Oluştur
            var yeniIlac = new tblMedicine
            {
                // BURASI EKLENDİ: İlacı şu anki kullanıcıya bağlıyoruz
                PersonID = UserSeassion.CurrentUser.ID,

                MedicineName = IlacAdi,
                MedicineDose = dozMiktari,
                MedicineTime = DateTime.Today.Add(SecilenSaat)
            };

            // 4. Veritabanına Kaydet
            // Not: App.Database.SaveMedicineAsync metodunun parametresi 'tblMedicine' olmalı.
            await App.Database.SaveMedicineAsync(yeniIlac);

            // 5. Önceki Sayfaya Dön
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}