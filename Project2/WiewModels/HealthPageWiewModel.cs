using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project2.Models;
using Project2.Pages;
using Project2.Services;
using System.Collections.ObjectModel;

namespace Project2.WiewModels
{
    public partial class HealthPageWiewModel : ObservableObject
    {
        // 1. Ekrana bağlanacak veri (ObservableProperty sayesinde ekran güncellenir)
        [ObservableProperty]
        private Person? _currentUser;

        [ObservableProperty]
        private SuTakibiViewModel _suTakibiVM;

        [ObservableProperty]
        private ObservableCollection<tblMedicine> _medicineList = new ObservableCollection<tblMedicine>();

        // Sayfa açılınca veya geri dönülünce bu metodu çağıracağız
        public void LoadUserData()
        {
            if (UserSeassion.CurrentUser != null)
            {
                CurrentUser = UserSeassion.CurrentUser;
            }
        }

        public HealthPageWiewModel()
        {
            // SuTakibiWiewModel'i burada açıyoruz.
            SuTakibiVM = new SuTakibiViewModel();
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
                placeholder: CurrentUser.Weight?.ToString() ?? "CurrentUser.Weight", // Mevcut kiloyu ipucu göster
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

        // 2. İlaçları Veritabanından Çekme Metodu
        public async Task LoadMedicines()
        {
            if (CurrentUser == null) return;

            // Veritabanından kullanıcının ilaçlarını al
            // (DatabaseService içinde GetMedicinesAsync(PersonID) metodu olduğunu varsayıyoruz)
            var list = await App.Database.GetMedicinesAsync(UserSeassion.CurrentUser.ID);

            // Listeyi temizle ve yeniden doldur
            MedicineList.Clear();
            foreach (var item in list)
            {
                MedicineList.Add(item);
            }
        }

        // 3. İlaç Silme Komutu
        [RelayCommand]
        public async Task DeleteMedicine(tblMedicine med)
        {
            if (med == null) return;

            bool answer = await Application.Current.MainPage.DisplayAlert("Sil", $"{med.MedicineName} silinsin mi?", "Evet", "Hayır");
            if (answer)
            {
                await App.Database.DeleteMedicineAsync(med);
                MedicineList.Remove(med); // Ekrandan da anında sil
            }
        }

        // 4. İlaç İçildi İşaretleme (Toggle)
        [RelayCommand]
        public async Task ToggleMedicine(tblMedicine med)
        {
            if (med == null) return;

            // Eğer bugün zaten içildiyse, tarihi sıfırla (İptal et)
            if (med.LastTakenDate.Date == DateTime.Today)
            {
                med.LastTakenDate = DateTime.MinValue; // Veya eski bir tarih
            }
            else
            {
                // İçilmediyse bugünün tarihini at
                med.LastTakenDate = DateTime.Now;

                // Kullanıcıya küçük bir geri bildirim ver (Toast mesajı gibi)
                // (Burada basit alert kullanıyoruz ama istenirse kaldırılabilir)
                // await Application.Current.MainPage.DisplayAlert("Bilgi", $"{med.MedicineName} alındı.", "Tamam");
            }

            // Veritabanını güncelle
            await App.Database.SaveMedicineAsync(med);

            // Listeyi yenile ki yeşil tik görünsün/gitsin
            // (MVVM'de nesne içindeki özellik değişince UI bazen tetiklenmez, 
            // en garantisi listedeki o elemanı güncellemektir ama şimdilik Load çağırabiliriz veya 
            // ObservableObject ise otomatik olur. En basiti listeyi tazelemek.)
            await LoadMedicines();
        }
    }
}
