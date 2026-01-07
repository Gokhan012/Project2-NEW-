using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using Project2.Data; // AppDatabase'e erişim için
using Project2.Models;
using Project2.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;//(Preferences için şart)
namespace Project2.ViewModels;

public partial class BudgetPageViewModel : ObservableObject
{
    [ObservableProperty] private DateTime currentDate;
    [ObservableProperty] private string currentDateText;
    [ObservableProperty] private decimal totalLimit = 10000;
    [ObservableProperty] private decimal totalIncome;
    [ObservableProperty] private decimal totalExpense;
    [ObservableProperty] private decimal remainingAmount;
    [ObservableProperty] private string budgetUsageText;
    [ObservableProperty] private double progressValue;
    [ObservableProperty] private bool hasTransactions;

    public ObservableCollection<tblBudget> Transactions { get; set; } = new();

    public BudgetPageViewModel()
    {
        CurrentDate = DateTime.Now;
        UpdateDateText();
    
        int currentUserId = UserSeassion.CurrentUser.ID;
      
        string dynamicKey = $"UserBudgetLimit_{currentUserId}";

        double savedLimit = Preferences.Default.Get(dynamicKey, 10000.0);

        TotalLimit = (decimal)savedLimit;
    }

    [RelayCommand]
    public async Task LoadData()
    {
        try
        {
            // 1. Veritabanı Bağlantısı
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            var db = new AppDatabase(dbPath);

            // 2. Verileri Çek
            var allData = await db.GetAllBudgetsAsync();

            // 3. AKTİF KULLANICIYI AL
            int currentUserId = UserSeassion.CurrentUser.ID; // Giriş yapan kullanıcının ID'si

            // 4. Hem Tarihe Hem KULLANICIYA Göre Filtrele
            var filteredData = allData
                .Where(x => x.UserId == currentUserId && // <--- KRİTİK NOKTA BURASI
                            x.Date.Month == CurrentDate.Month &&
                            x.Date.Year == CurrentDate.Year)
                .OrderByDescending(x => x.Date)
                .ToList();

            Transactions.Clear();
            foreach (var item in filteredData)
            {
                Transactions.Add(item);
            }

            CalculateBudget();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }

    [RelayCommand]
    async Task NextMonth()
    {
        CurrentDate = CurrentDate.AddMonths(1);
        UpdateDateText();
        await LoadData();
    }

    [RelayCommand]
    private async Task EditBudgetLimit()
    {
        // Ekrana veri giriş kutusu açar
        string result = await Application.Current.MainPage.DisplayPromptAsync(
            "Bütçe Limiti",
            "Yeni aylık bütçe sınırınızı giriniz:",
            initialValue: TotalLimit.ToString("0"),
            keyboard: Keyboard.Numeric);

        // Eğer kullanıcı bir sayı girip Tamam'a bastıysa
        if (!string.IsNullOrWhiteSpace(result) && decimal.TryParse(result, out decimal newLimit))
        {
            if (newLimit < 0) return; // Negatif sayı girilirse işlemi iptal et

            // 1. Yeni değeri ata
            TotalLimit = newLimit;

            int currentUserId = UserSeassion.CurrentUser.ID;
            string dynamicKey = $"UserBudgetLimit_{currentUserId}";

            Preferences.Default.Set(dynamicKey, (double)TotalLimit);

            CalculateBudget();
        }
    }

    [RelayCommand]
    async Task PreviousMonth()
    {
        CurrentDate = CurrentDate.AddMonths(-1);
        UpdateDateText();
        await LoadData();
    }

    void UpdateDateText()
    {
        CurrentDateText = CurrentDate.ToString("MMMM yyyy", new System.Globalization.CultureInfo("tr-TR"));

    }

    void CalculateBudget()
    {
        // 1. Toplam Gelir ve Gideri Hesapla
        TotalIncome = Transactions.Where(x => x.IsIncome).Sum(x => x.Amount);
        TotalExpense = Transactions.Where(x => !x.IsIncome).Sum(x => x.Amount);

        // Toplam Kullanılabilir Para = ( Koyulan Sınır + Kazandığın Gelirler)
        decimal totalAvailableMoney = TotalLimit + TotalIncome;

        // Kalan Para = Toplam Kullanılabilir Para - Harcamalar
        RemainingAmount = totalAvailableMoney - TotalExpense;

        // Ekranda görünecek yazı: "Kalan / (Limit + Gelir)" şeklinde güncellendi
        BudgetUsageText = $"₺{RemainingAmount} / ₺{totalAvailableMoney}";

        // Progress Bar (Doluluk Çubuğu) Hesabı
        // Artık paydada sadece Limit değil, (Limit + Gelir) var.
        if (totalAvailableMoney > 0)
        {
            ProgressValue = (double)(TotalExpense / totalAvailableMoney);
        }
        else
        {
            ProgressValue = 0;
        }

        // Çubuk %100'ü geçerse patlamasın diye sınırla
        if (ProgressValue > 1) ProgressValue = 1;

        HasTransactions = Transactions.Count > 0;
    }
}