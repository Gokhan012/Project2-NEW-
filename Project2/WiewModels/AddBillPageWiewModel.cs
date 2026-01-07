using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project2.Models;
using Project2.Data;     // <-- AppDatabase İÇİN BU GEREKLİ
using Project2.Services; // <-- BudgetValidator buradaysa kalsın, yoksa silebilirsin
using System.Collections.ObjectModel;

namespace Project2.ViewModels
{
    public partial class AddBillPageViewModel : ObservableObject
    {
        // View'daki Entry'lerin bağlandığı özellikler
        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string amount;

        [ObservableProperty]
        private DateTime dueDate = DateTime.Now;

        [ObservableProperty]
        private string selectedCategory = "Fatura";

        [RelayCommand]
        void SetCategory(string category)
        {
            SelectedCategory = category;
        }

        // KAYDETME KOMUTU
        [RelayCommand]
        private async Task SaveBill()
        {
            try
            {
                // 1. Tutar Dönüşümü
                if (!decimal.TryParse(Amount, out decimal decimalAmount))
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", "Lütfen geçerli bir tutar giriniz.", "Tamam");
                    return;
                }

                // 2. Model Oluşturma
                var newBill = new tblBill
                {
                    Title = this.Title,
                    Amount = (double)decimalAmount, // AppDatabase double/decimal uyumuna dikkat et
                    DueDate = this.DueDate,
                    Category = this.SelectedCategory,
                    IsPaid = false,
                    UserId = UserSeassion.CurrentUser.ID // Kullanıcı ID'si ekleniyor
                };

                // 3. Validasyon (Eğer Validator sınıfın duruyorsa)
                BudgetValidator.Validate(newBill);

                // 4. Veritabanına Kayıt (GÜNCELLENEN KISIM)
                // BillService YERİNE AppDatabase KULLANIYORUZ
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
                var db = new AppDatabase(dbPath);

                // AppDatabase içindeki SaveBillAsync metodunu çağırıyoruz
                await db.SaveBillAsync(newBill);

                // 5. Başarılı Mesajı ve Geri Dönüş
                await Application.Current.MainPage.DisplayAlert("Başarılı", "Fatura başarıyla eklendi.", "Tamam");

                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", $"Bir hata oluştu: {ex.Message}", "Tamam");
            }
        }
    }
}