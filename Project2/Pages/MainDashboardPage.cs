using FmgLib.MauiMarkup;
using System.IO;
using Microsoft.Maui.Storage;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Project2.Data;
using Project2.Models;
using Project2.Services;
using Project2.WiewModels;

namespace Project2.Pages;

public class MainDashboardPage : ContentPage
{
    private HorizontalStackLayout _actionButtonsPopup;
    private List<tblMedicine> _todaysMedicines = new List<tblMedicine>();

    public MainDashboardPage()
    {
        BindingContext = new WiewModels.MainDashboardPageWiewModel();
        this.BackgroundColor(Color.FromArgb("#23222E"));

        Content = BuildContent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            string dbPath = $"{FileSystem.AppDataDirectory}/app.db";
            var db = new AppDatabase(dbPath);

            if (UserSeassion.CurrentUser == null) return;

            int currentUserId = UserSeassion.CurrentUser.ID;
            var now = DateTime.Now;

            // --- 1. BÜTÇE (Üstteki kart için hesaplama kalıyor) ---
            var allBudgets = await db.GetAllBudgetsAsync();
            var totalExpense = allBudgets
                .Where(x => x.UserId == currentUserId &&
                            x.Date.Month == now.Month &&
                            x.Date.Year == now.Year &&
                            !x.IsIncome)
                .Sum(x => x.Amount);

            if (UserSeassion.CurrentBudget == null) UserSeassion.CurrentBudget = new tblBudget();
            UserSeassion.CurrentBudget.Amount = totalExpense;


            // --- 2. SU VERİSİ ---
            var todayWater = await db.GetWaterRecordByDateAsync(now, currentUserId);

            if (todayWater != null)
            {
                UserSeassion.CurrentlWater = todayWater;
            }
            else
            {
                UserSeassion.CurrentlWater = new tblWater
                {
                    WaterDrink = 0,
                    WaterNeeded = 2500,
                    PersonId = currentUserId,
                    Date = now
                };
            }

            // --- 3. İLAÇ VERİSİ ---
            var meds = await db.GetMedicinesAsync(currentUserId);
            _todaysMedicines = meds ?? new List<tblMedicine>();


            // --- 4. EKRANI YENİLE ---
            Content = BuildContent();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", "Veri yüklenirken hata oluştu: " + ex.Message, "Tamam");
        }
    }

    private View BuildContent()
    {
        int userId = UserSeassion.CurrentUser?.ID ?? 0;

        // --- SU HESAPLAMA ---
        double waterDrank = UserSeassion.CurrentlWater?.WaterDrink ?? 0;
        double waterTarget = UserSeassion.CurrentlWater?.WaterNeeded ?? 2500;
        double waterProgress = waterTarget > 0 ? (waterDrank / waterTarget) : 0;
        if (waterProgress > 1) waterProgress = 1;

        // --- BÜTÇE HESAPLAMA ---
        double userLimit = Preferences.Default.Get($"UserBudgetLimit_{userId}", 10000.0);
        double currentExpense = (double)(UserSeassion.CurrentBudget?.Amount ?? 0);
        double budgetProgress = userLimit > 0 ? (currentExpense / userLimit) : 0;
        if (budgetProgress > 1) budgetProgress = 1;

        // --- İLAÇ LİSTELEME ---
        string medInfoText = "";

        if (_todaysMedicines.Count == 0)
        {
            medInfoText = "İlaç Eklenmedi";
        }
        else
        {
            foreach (var med in _todaysMedicines.Take(3))
            {
                bool isTaken = med.LastTakenDate.Date == DateTime.Now.Date;
                string checkMark = isTaken ? " (✓)" : "";
                medInfoText += $"- {med.MedicineName}{checkMark}\n";
            }
            if (_todaysMedicines.Count > 3) medInfoText += "...";
        }

        double medProgress = 0;
        if (_todaysMedicines.Count > 0)
        {
            int takenMeds = _todaysMedicines.Count(m => m.LastTakenDate.Date == DateTime.Now.Date);
            medProgress = (double)takenMeds / _todaysMedicines.Count;
        }


        // --- TASARIM ---
        return new Grid()
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star), // Satır 0: İçerik
                new RowDefinition(GridLength.Auto)  // Satır 1: Alt Bar
            },
            Children =
            {
                new ScrollView()
                {
                    Content = new VerticalStackLayout()
                    {
                        Padding = new Thickness(20, 40, 20, 100), // Butonun arkada kalmaması için alt padding artırıldı
                        Spacing = 25,
                        Children =
                        {
                            new Grid()
                            {
                                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                                Children =
                                {
                                    new Label()
                                    .Text($"Merhaba {UserSeassion.CurrentUser?.Name ?? "Misafir"};")
                                    .TextColor(Colors.White)
                                    .FontSize(22)
                                    .FontAttributes(FontAttributes.Bold)
                                    .CenterVertical(),

                                    new HorizontalStackLayout()
                                    {
                                    Spacing = 15,
                                    Children = {
                                        new Label().Text("🔔").FontSize(22),
                                        new Label()
                                        .Text("⚙️")
                                        .FontSize(22)
                                        .GestureRecognizers(new TapGestureRecognizer()
                                        {
                                            Command = new Command(async () => await Navigation.PushAsync(new ProfileEditPage()))
                                        }),
                                    }
                                    }.Column(1)
                                }
                            },
                
                // 2. KARTLAR
                new ScrollView()
                {
                    Orientation = ScrollOrientation.Horizontal,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                    Content = new HorizontalStackLayout()
                    {
                        Spacing = 15,
                        Children =
                        {
                        // SU KARTI
                        CreateSummaryCard("Su Hedefi", $"{waterDrank:N0} ml\n{waterTarget:N0} ml", waterProgress),
                        
                        // BÜTÇE KARTI
                        CreateSummaryCard("Bütçe Durumu", $"{currentExpense:N0} TL / {userLimit:N0} TL", budgetProgress),
                        
                        // İLAÇ KARTI
                        CreateSummaryCard("İlaç Takibi", medInfoText.Trim(), medProgress)
                        }
                    }
                },

                // 3. LİSTE (SADECE BÜTÇE KALDIRILDI)
                new VerticalStackLayout()
                {
                    Spacing = 15,
                    Children =
                    {
                        CreateModuleRow("Takvim", "Bugünkü Dersler", "09:00 - 11:50 - Nesne Yönelimli Prog.\n12:00 - 13:50 - Veri Yapıları\n14:00 - 16:50 - İşletim Sistemleri"),
                        CreateModuleRow("Takvim", "Etkinlikler", "Matematik sınavına kalan gün: 7"),
                        // Bütçe satırı buradan silindi.
                    }
                }
            }
                    }
                }.Row(0),

                // 4. FAB BUTON
                new HorizontalStackLayout()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    Spacing = 15,
                    Margin = new Thickness(0, 0, 0, 20), // Alt barın biraz üzerinde durması için
                    Children =
                    {
                        new Border()
                        {
                            StrokeShape = new Ellipse(), Stroke = Color.FromArgb("#00FF85"), StrokeThickness = 3, HeightRequest = 60, WidthRequest = 60,
                            Content = new Label().Text("+").TextColor(Color.FromArgb("#00FF85")).FontSize(30).Center()
                        }
                        .GestureRecognizers(new TapGestureRecognizer()
                        {
                            Command = new Command(() => { if(_actionButtonsPopup != null) _actionButtonsPopup.IsVisible = !_actionButtonsPopup.IsVisible; })
                        }),

                        new HorizontalStackLayout()
                        {
                            Spacing = 15, IsVisible = false,
                            Children =
                            {
                                CreateActionButton("💰", "harcama ekle", nameof(MainDashboardPageWiewModel.GotoAddBudgetPageCommand)),
                                CreateActionButton("💧", "Su ekle", nameof(MainDashboardPageWiewModel.GotoHealthPageCommand)),
                            }
                        }.Assign(out _actionButtonsPopup)
                    }
                }.Row(0).ZIndex(1), // Row 0 içinde en üstte (overlay) durmasını sağlar

                // 5. NAVBAR
                new Border()
                {
                    Stroke = Colors.White, StrokeThickness = 1, Margin = new Thickness(10, 0, 10, 20), Padding = new Thickness(0, 10),
                    Content = new Grid()
                    {
                        ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) },
                        Children =
                        {
                            CreateNavTab("🏠", "Ana Sayfa", 0, nameof(MainDashboardPageWiewModel.GotoMainPageCommand), true),
                            CreateNavTab("📅", "Takvim", 1, nameof(MainDashboardPageWiewModel.GotoCalendarPageCommand), false),
                            CreateNavTab("💰", "Bütçe", 2, nameof(MainDashboardPageWiewModel.GotoBudgetPageCommand), false),
                            CreateNavTab("❤️", "Sağlık", 3, nameof(MainDashboardPageWiewModel.GotoHealthPageCommand), false)
                        }
                    }
                }.Row(4)
            }
        };
    }

    // YARDIMCI METOTLAR
    private View CreateSummaryCard(string title, string info, double progress)
    {
        return new Border()
        {
            WidthRequest = 220,
            HeightRequest = 130,
            StrokeShape = new RoundRectangle() { CornerRadius = 20 },
            Background = new LinearGradientBrush(new GradientStopCollection { new GradientStop(Color.FromArgb("#2F58CD"), 0), new GradientStop(Color.FromArgb("#AD49E1"), 1) }, new Point(0, 0), new Point(1, 1)),
            Content = new VerticalStackLayout()
            {
                Padding = 15,
                Spacing = 5,
                Children =
                {
                    new Label().Text("Bugünkü Özet").TextColor(Colors.White).FontSize(12),
                    new Label().Text(title).TextColor(Colors.White).FontSize(16).FontAttributes(FontAttributes.Bold),
                    new Label().Text(info).TextColor(Colors.White).FontSize(11).LineBreakMode(LineBreakMode.WordWrap),
                    new ProgressBar().Progress(progress).ProgressColor(Colors.White).Margin(new Thickness(0,10,0,0))
                }
            }
        };
    }

    private View CreateModuleRow(string title, string subTitle, string contentText, bool showProgress = false)
    {
        var layout = new VerticalStackLayout() { VerticalOptions = LayoutOptions.Center, Children = { new Label().Text(contentText).TextColor(Colors.White).FontSize(13).HorizontalTextAlignment(TextAlignment.Center) } };
        if (showProgress) layout.Children.Add(new ProgressBar().Progress(0.5).ProgressColor(Colors.Blue).Margin(new Thickness(0, 5, 0, 0)));
        return new Border() { Stroke = Colors.White, StrokeThickness = 1, StrokeShape = new RoundRectangle() { CornerRadius = 15 }, Padding = 15, Content = new Grid() { ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) }, Children = { new VerticalStackLayout() { Children = { new Label().Text(title).TextColor(Colors.White).FontSize(18).FontAttributes(FontAttributes.Bold), new Label().Text(subTitle).TextColor(Colors.Gray).FontSize(10) } }.Column(0), layout.Column(1).CenterVertical() } } };
    }

    private View CreateActionButton(string icon, string text, string commandName)
    {
        var layout = new VerticalStackLayout() { Spacing = 5, Children = { new Border() { HeightRequest = 45, WidthRequest = 45, StrokeShape = new Ellipse(), Stroke = Color.FromArgb("#00FF85"), Content = new Label().Text(icon).Center() }, new Label().Text(text).TextColor(Colors.White).FontSize(10).CenterHorizontal() } };
        if (!string.IsNullOrEmpty(commandName)) layout.GestureRecognizers.Add(new TapGestureRecognizer().Bind(TapGestureRecognizer.CommandProperty, commandName));
        return layout;
    }

    private View CreateNavTab(string icon, string text, int col, string commandName, bool isActive = false)
    {
        var color = isActive ? Colors.CornflowerBlue : Colors.White;
        var layout = new VerticalStackLayout() { Spacing = 2, Children = { new Label().Text(icon).TextColor(color).FontSize(20).CenterHorizontal(), new Label().Text(text).TextColor(color).FontSize(10).CenterHorizontal() } }.Column(col);
        if (!string.IsNullOrEmpty(commandName)) layout.GestureRecognizers.Add(new TapGestureRecognizer().Bind(TapGestureRecognizer.CommandProperty, commandName));
        return layout;
    }
}