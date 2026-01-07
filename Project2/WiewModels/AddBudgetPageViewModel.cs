using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project2.Models;
using Project2.Data;
using Microsoft.Maui.Storage;
using Project2.Services; // Preferences için gerekli

namespace Project2.ViewModels
{
    public partial class AddBudgetPageViewModel : ObservableObject
    {
        [ObservableProperty] private string title;
        [ObservableProperty] private string amount;
        [ObservableProperty] private DateTime date = DateTime.Now;
        [ObservableProperty] private bool isIncome = false;
        [ObservableProperty] private bool isRecurring;
        [ObservableProperty] private string selectedCategory;

        [RelayCommand]
        void SetCategory(string category)
        {
            SelectedCategory = category;
        }

        [RelayCommand]
        void SetType(string type)
        {
            IsIncome = (type == "Gelir");
            SelectedCategory = IsIncome ? "Maaş" : "Market";
        }

        [RelayCommand]
        private async Task SaveBudget()
        {
            try
            {
                // 1. Tutar Dönüştürme Kontrolü
                if (!decimal.TryParse(Amount, out decimal decimalAmount))
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", "Geçerli bir tutar giriniz.", "Tamam");
                    return;
                }


                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
                var db = new AppDatabase(dbPath);

                // --- BÜTÇE AŞIM KONTROLÜ ---
                if (!IsIncome) // Sadece Gider eklerken kontrol et
                {
                    // 1. Kayıtlı Limiti Çek
                    decimal limit = (decimal)Preferences.Default.Get("UserBudgetLimit", 10000.0);

                    // 2. Bu ayın mevcut harcamalarını veritabanından çek
                    var allData = await db.GetAllBudgetsAsync();
                    var currentMonthExpense = allData
                        .Where(x => !x.IsIncome && x.Date.Month == Date.Month && x.Date.Year == Date.Year)
                        .Sum(x => x.Amount);

                    // 3. Hesaplama: (Şu anki + Yeni Eklenecek) Limiti aşıyor mu?
                    if (currentMonthExpense + decimalAmount > limit)
                    {
                        // Kullanıcıya Seçenek Sun
                        string action = await Application.Current.MainPage.DisplayActionSheet(
                            $"Uyarı! Aylık bütçe sınırını ({limit:C}) aşmak üzeresiniz.\nToplam Harcama: {currentMonthExpense + decimalAmount:C} olacak.",
                            "İptal",                // Vazgeç
                            null,
                            "Limiti Güncelle",      // Limiti arttırıp devam et
                            "Yine de Ekle");        // Limiti aşarak devam et

                        if (action == "İptal") return; // İşlemi durdur

                        if (action == "Limiti Güncelle")
                        {
                            // Yeni limit iste
                            string newLimitStr = await Application.Current.MainPage.DisplayPromptAsync(
                                "Limit Güncelle",
                                "Yeni bütçe sınırınızı giriniz:",
                                initialValue: limit.ToString("0"),
                                keyboard: Keyboard.Numeric);

                            if (decimal.TryParse(newLimitStr, out decimal newLimitVal))
                            {
                                // Yeni limiti kaydet
                                Preferences.Default.Set("UserBudgetLimit", (double)newLimitVal);
                                await Application.Current.MainPage.DisplayAlert("Başarılı", "Limit güncellendi, işlem kaydediliyor.", "Tamam");
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("Hata", "Geçersiz limit, işlem iptal edildi.", "Tamam");
                                return;
                            }
                        }
                        // "Yine de Ekle" seçilirse hiçbir şey yapmadan aşağı devam eder ve kaydeder.
                    }
                }
                // ----------------------------------------------------

                if (string.IsNullOrEmpty(SelectedCategory))
                {
                    SelectedCategory = "Diğer";
                }

                var newBudget = new tblBudget
                {
                    Title = this.Title,
                    Amount = decimalAmount,
                    Date = this.Date,
                    IsIncome = this.IsIncome,
                    Category = this.SelectedCategory,
                    IsRecurring = this.IsRecurring,
                    UserId = UserSeassion.CurrentUser.ID // Geçerli kullanıcı ID'si
                };

                // Kaydetme İşlemi
                await db.SaveBudgetAsync(newBudget);

                await Application.Current.MainPage.DisplayAlert("Başarılı", "İşlem kaydedildi.", "Tamam");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
    }
}