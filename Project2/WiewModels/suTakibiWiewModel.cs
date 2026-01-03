using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.Shapes;
using Project2.Models;
using Project2.Services;

namespace Project2.WiewModels
{
    public partial class SuTakibiViewModel : ObservableObject
    {
      
        private tblWater currentwater;

        public SuTakibiViewModel()
        {
            Initialize();
        }

        // 1. Özellikler
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WaterLabelText))]
        [NotifyPropertyChangedFor(nameof(WaterStrokeDashArray))]
        private double _currentWater = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WaterLabelText))]
        [NotifyPropertyChangedFor(nameof(WaterStrokeDashArray))]
        private double _targetWater = 2500;

        [ObservableProperty]
        private bool _goalReached = false;

        // 2. Görünüm Hesaplamaları
        public string WaterLabelText => $"{CurrentWater} ml / {TargetWater} ml";

        public DoubleCollection WaterStrokeDashArray
        {
            get
            {
                double perimeter = 160 * Math.PI;
                double ratio = TargetWater == 0 ? 0 : CurrentWater / TargetWater;
                if (ratio > 1) ratio = 1;
                double filled = perimeter * ratio;
                double empty = perimeter - filled;
                return new DoubleCollection { filled, empty };
            }
        }

        private async void Initialize()
        {
            var record = await App.Database.GetFirstWaterRecordAsync();

            if (record != null)
            {
                currentwater = record;

                // --- GÜN KONTROLÜ ---
                // Eğer kayıtlı tarih bugünden farklıysa (yani eski bir günse)
                if (currentwater.Date.Date != DateTime.Now.Date)
                {
                    currentwater.WaterDrink = 0;       // Suyu sıfırla
                    currentwater.Date = DateTime.Now;  // Tarihi bugün yap

                    // Sıfırlanmış halini veritabanına kaydet
                    await App.Database.UpdateWaterAsync(currentwater);
                }

                // Değerleri ekrana yansıt
                CurrentWater = currentwater.WaterDrink;
                TargetWater = currentwater.WaterNeeded;
            }
            else
            {
                // Kayıt hiç yoksa, bugünün tarihiyle yeni oluştur
                currentwater = new tblWater
                {
                    WaterDrink = 0,
                    WaterNeeded = 2500,
                    Date = DateTime.Now // Bugünün tarihi
                };
                await App.Database.SaveWaterAsync(currentwater);
            }
        }

        // 3. Su Ekleme Komutu
        [RelayCommand]
        public async Task AddWater(string amountStr)
        {
            double amountToAdd = 0;

            if (amountStr == "Custom")
            {
                string result = await Application.Current.MainPage.DisplayPromptAsync(
                    "Özel Su Girişi", "Miktarı giriniz (ml):", "Ekle", "İptal", "330", keyboard: Keyboard.Numeric);
                if (double.TryParse(result, out double val)) amountToAdd = val;
            }
            else
            {
                double.TryParse(amountStr, out amountToAdd);
            }

            if (amountToAdd > 0)
            {
                if (GoalReached)
                {
                    await Application.Current.MainPage.DisplayAlert("Bilgi", "Günlük su hedefinize ulaştınız. Yeni su eklemek için su hedefini güncelleyiniz.", "Tamam");
                    return;
                }
                else if (!GoalReached)
                {
                    // A. EKRANI GÜNCELLE
                    CurrentWater += amountToAdd;


                    // B. VERİTABANINI GÜNCELLE (DÜZELTİLEN YER)
                    if (currentwater != null)
                    {
                        // 1. amountToAdd değil, CurrentWater (Toplam) atanmalı
                        currentwater.WaterDrink = CurrentWater;

                        // 2. new tblWater yerine mevcut nesneyi güncelliyoruz
                        await App.Database.UpdateWaterAsync(currentwater);
                    }

                    // C. KONTROL
                    await WaterGoalChecker();
                }
            }
            OnPropertyChanged(nameof(CurrentWater));
            OnPropertyChanged(nameof(WaterLabelText));
        }

        // 4. Hedef Değiştirme Komutu
        [RelayCommand]
        public async Task ChangeWaterGoal()
        {
            string result = await Application.Current.MainPage.DisplayPromptAsync(
                "Hedef Güncelle", "Günlük hedef (ml):", "Güncelle", "İptal",
                initialValue: TargetWater.ToString(), keyboard: Keyboard.Numeric);

            if (double.TryParse(result, out double newGoal) && newGoal > 0)
            {
                // 1. UI'daki Hedefi Güncelle (Ekran otomatik değişir)
                TargetWater = newGoal;

                // 2. Sayfa yönlendirmesini SİL (Buna gerek yok, hata kaynağı bu)
                // await Application.Current.MainPage.Navigation.PushAsync(new HealthPage()); <--- SİLİNDİ

                // 3. Veritabanı Nesnesini Doğru Güncelle
                if (currentwater != null)
                {
                    // HATA BURADAYDI: currentwater.WaterDrink = CurrentWater yapıyordun.
                    // DOĞRUSU: Hedefi güncellememiz lazım.
                    currentwater.WaterNeeded = TargetWater;

                    // Tarihi de güncelleyelim ki bugünün kaydı olduğu belli olsun
                    currentwater.Date = DateTime.Now;

                    // 4. Kaydet
                    await App.Database.UpdateWaterAsync(currentwater);
                }
            }
        }

        [RelayCommand]
        public async Task WaterGoalChecker()
        {
            if (CurrentWater >= TargetWater - 1)
            {
                bool answer = await Application.Current.MainPage.DisplayAlert("Tebrikler!", "Günlük su hedefinize ulaştınız. Hedefi Değiştirmek İster Misiniz?", "Evet", "Hayır");
                if (answer)
                {
                    await ChangeWaterGoal();
                    GoalReached = false;
                }
                else
                {
                    GoalReached = true;
                }
            }
        }
    }
}