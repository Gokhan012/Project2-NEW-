using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;

namespace Project2.WiewModels;

public partial class CalendarMainPageWiew : ObservableObject
{
    [ObservableProperty]
    private DateTime _currentDate = DateTime.Now;

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Now;
    [ObservableProperty]
    private string _selectedDateText;

    [ObservableProperty]
    private string _currentMonthYearText;

    public CalendarMainPageWiew()
    {
        UpdateDateText();
        // İlk açılışta bugünün tarihini yazdıralım
        UpdateSelectedDateText(DateTime.Now);
    }

    partial void OnCurrentDateChanged(DateTime value)
    {
        UpdateDateText();
    }

    // 👇 2. YENİ EKLENEN: Seçilen gün değişince bu metot otomatik çalışır
    partial void OnSelectedDateChanged(DateTime value)
    {
        UpdateSelectedDateText(value);
    }

    private void UpdateDateText()
    {
        CurrentMonthYearText = CurrentDate.ToString("MMMM yyyy", new CultureInfo("tr-TR"));
    }

    // Seçilen tarihi yazıya çeviren yardımcı metot
    private void UpdateSelectedDateText(DateTime date)
    {
        // Format: "9 Aralık Salı"
        SelectedDateText = date.ToString("d MMMM dddd", new CultureInfo("tr-TR"));
    }

    [RelayCommand]
    public void NextMonth() => CurrentDate = CurrentDate.AddMonths(1);

    [RelayCommand]
    public void PreviousMonth() => CurrentDate = CurrentDate.AddMonths(-1);
}