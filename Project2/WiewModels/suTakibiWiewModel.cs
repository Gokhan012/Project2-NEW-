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

        public string WaterLabelText => $"{CurrentWater} ml / {TargetWater} ml";

        public SuTakibiViewModel()
        {
            _ = Initialize();
        }

        // ELİPSİN PÜRÜZSÜZ DOLMASI İÇİN MATEMATİK
        public DoubleCollection WaterStrokeDashArray
        {
            get
            {
                double ratio = TargetWater <= 0 ? 0 : CurrentWater / TargetWater;
                if (ratio > 1) ratio = 1;
                if (ratio < 0) ratio = 0;

                double totalLength = 1000;
                double filled = ratio * totalLength;
                double empty = totalLength - filled;

                return new DoubleCollection { filled, empty };
            }
        }

        public async Task Initialize()
        {
            var currentPerson = UserSeassion.CurrentUser;
            if (currentPerson == null) return;

            var record = await App.Database.GetWaterRecordByDateAsync(DateTime.Now, currentPerson.ID);

            if (record != null)
            {
                currentwater = record;
            }
            else
            {
                var lastRecord = await App.Database.GetLatestWaterRecordAsync(currentPerson.ID);
                double lastTarget = lastRecord?.WaterNeeded ?? 2500;

                currentwater = new tblWater
                {
                    PersonId = UserSeassion.CurrentUser.ID,
                    WaterDrink = 0,
                    WaterNeeded = lastTarget,
                    Date = DateTime.Now.Date
                };
                await App.Database.SaveWaterAsync(currentwater);
            }

            MainThread.BeginInvokeOnMainThread(() => {
                CurrentWater = currentwater.WaterDrink;
                TargetWater = currentwater.WaterNeeded;
                GoalReached = CurrentWater >= TargetWater;
            });
        }

        [RelayCommand]
        public async Task AddWater(string amountStr)
        {
            double amountToAdd = 0;
            if (amountStr == "Custom")
            {
                string result = await Application.Current.MainPage.DisplayPromptAsync(
                    "Özel Giriş", "Miktar (ml):", "Ekle", "İptal", "330", keyboard: Keyboard.Numeric);
                if (double.TryParse(result, out double val)) amountToAdd = val;
            }
            else double.TryParse(amountStr, out amountToAdd);

            if (amountToAdd > 0)
            {
                if (!GoalReached)
                {
                    CurrentWater += amountToAdd;
                    currentwater.WaterDrink = CurrentWater;
                    await App.Database.UpdateWaterAsync(currentwater);
                }else
                {
                    await Application.Current.MainPage.DisplayAlert("Bilgi", "Zaten hedefinize ulaştınız!", "Tamam");
                }
                await WaterGoalChecker();
            }
        }

        [RelayCommand]
        public async Task ChangeWaterGoal()
        {
            string result = await Application.Current.MainPage.DisplayPromptAsync(
                "Hedef Güncelle", "Hedef (ml):", "Güncelle", "İptal",
                initialValue: TargetWater.ToString(), keyboard: Keyboard.Numeric);

            if (double.TryParse(result, out double newGoal) && newGoal > 0)
            {
                TargetWater = newGoal;
                currentwater.WaterNeeded = TargetWater;
                await App.Database.UpdateWaterAsync(currentwater);
                GoalReached = CurrentWater >= TargetWater;
            }
        }

        public async Task WaterGoalChecker()
        {
            if (CurrentWater >= TargetWater && !GoalReached)
            {
                GoalReached = true;
                await Application.Current.MainPage.DisplayAlert("Tebrikler!", "Hedefinize ulaştınız!", "Tamam");
            }
            else if (CurrentWater < TargetWater) GoalReached = false;
        }
    }
}