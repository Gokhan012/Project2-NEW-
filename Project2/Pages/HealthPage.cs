using FmgLib.MauiMarkup;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace MyAppMAUI.Pages;

public class HealthPage : ContentPage
{
    private Label _waterLabel;
    private Ellipse _progressCircle;
    private Label _targetGoalLabel;
    private double _currentWater = 0;
    private double _targetWater = 2500;
    public HealthPage()
    {
        this.BackgroundColor(Color.FromArgb("#23222E"));

        Content = new Grid()
        {
            Padding = new Thickness(20, 40, 20, 20),
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto), // 0: Üst başlık
                new RowDefinition(GridLength.Auto), // 1: Üst kartlar
                new RowDefinition(GridLength.Auto), // 2: Su takibi
                new RowDefinition(GridLength.Star), // 3: İlaç listesi (scroll)
                new RowDefinition(GridLength.Auto)  // 4: Alt nav
            },
            Children =
            {
                new Grid()
                {
                    Children =
                    {
                        new Label()
                            .Text("Sağlık")
                            .TextColor(Colors.White)
                            .FontSize(20)
                            .FontAttributes(FontAttributes.Bold)
                            .HorizontalOptions(LayoutOptions.Start)
                            .CenterVertical(),
                    }
                }.Row(0).Margin(new Thickness(0, 0, 0, 15)),

                new Grid()
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Star)
                    },
                    ColumnSpacing = 15,
                    Children =
                    {
                        CreateStatusCard("MEVCUT KİLO", "0 kg", "Veri girmek için tıkla!", 0),
                        CreateBmiCard().Column(1)
                    }
                }.Row(1),

                new VerticalStackLayout()
                {
                    Spacing = 15,
                    Margin = new Thickness(0, 25, 0, 0),
                    Children =
                    {
                        new Label()
                            .Text("Su Takibi")
                            .TextColor(Colors.White)
                            .FontSize(18)
                            .FontAttributes(FontAttributes.Bold),

                        new Grid() // Katmanlı yapı için Grid
                        {
                            HeightRequest = 170,
                            WidthRequest = 170,
                            HorizontalOptions = LayoutOptions.Center,
                            Children =
                            {
                            // KATMAN 1: Arka plandaki soluk mavi halka (Yol)
                                new Ellipse()
                                    .Stroke(Color.FromArgb("#332196F3")) // Şeffaf mavi
                                    .StrokeThickness(10)
                                    .CenterHorizontal()
                                    .CenterVertical(),

                            // KATMAN 2: Asıl Progres Bar (Mavi)
                            (_progressCircle = new Ellipse()
                            .Stroke(Color.FromArgb("#2196F3")) // Canlı Mavi
                            .StrokeThickness(10)
                            .StrokeDashArray(new double[] { 0, 100 }) // Başlangıçta boş
                            .StrokeDashOffset(0)
                            .CenterHorizontal()
                            .CenterVertical()),

                            // KATMAN 3: İçteki Beyaz Daire ve Yazılar
                            new Border()
                            {
                                BackgroundColor = Colors.White,
                                HeightRequest = 145,
                                WidthRequest = 145,
                                StrokeShape = new Ellipse(),
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,
                                GestureRecognizers =
                                {
                                    new TapGestureRecognizer
                                    {
                                        Command = new Command(async () => await ChangeWaterGoal())
                                    }
                                },

                                Content = new VerticalStackLayout()
                                {
                                    VerticalOptions = LayoutOptions.Center,
                                    InputTransparent = true,
                                    Children =
                                    {
                                        // Mevcut miktar etiketi
                                        (_waterLabel = new Label()
                                        .Text($"{_currentWater} ml / {_targetWater} ml")
                                        .TextColor(Colors.Black)
                                        .FontAttributes(FontAttributes.Bold)
                                        .CenterHorizontal()),


                                        (_targetGoalLabel = new Label()
                                        .Text($"Hedefi Güncelle")
                                        .TextColor(Colors.DimGray)
                                        .FontSize(11)
                                        .CenterHorizontal()
                                        .Margin(new Thickness(0,2,0,0)))

                                    }
                                }
                            }
                        }
                        },

                        new Grid()
                        {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Star)
                            },
                            ColumnSpacing = 10,
                            Children =
                            {
                                CreateWaterButton("+250 ml", 0),
                                CreateWaterButton("+500 ml", 1),
                                CreateWaterButton("+ Özel", 2)
                            }
                        }
                    }
                }.Row(2),

                new VerticalStackLayout()
                {
                    Spacing = 10,
                    Margin = new Thickness(0, 25, 0, 0),
                    Children =
                    {
                        new Grid()
                        {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Auto)
                            },
                            Children =
                            {
                                new Label()
                                    .Text("İlaç Takibi")
                                    .TextColor(Colors.White)
                                    .FontSize(18)
                                    .FontAttributes(FontAttributes.Bold),

                                new Label()
                                    .Text("+")
                                    .TextColor(Color.FromArgb("#00FF85"))
                                    .FontSize(30)
                                    .FontAttributes(FontAttributes.Bold)
                                    .Column(1)
                                    .GestureRecognizers(new TapGestureRecognizer()
                                    {
                                    Command = new Command(async () => await Navigation.PushAsync(new AddMedicinePage()))
                                    }),
                            }
                        },

                        new ScrollView()
                        {
                            Content = new VerticalStackLayout()
                            {
                                Spacing = 10,
                                Children =
                                {
                                    new Label()
                                        .Text("Henüz ilaç eklenmedi.\nİlaç eklemek için + butonuna dokunun.")
                                        .TextColor(Colors.Gray)
                                        .FontSize(13)
                                }
                            }
                        }
                    }
                }.Row(3),

                CreateBottomNav().Row(4)
            }
        };
    }

    // --- YARDIMCI METOTLAR ---

    private View CreateStatusCard(string title, string value, string subtitle, int col)
    {
        return new Border()
        {
            Stroke = Colors.White,
            StrokeThickness = 1,
            Padding = 12,
            Content = new VerticalStackLayout()
            {
                Spacing = 6,
                Children =
                {
                    new Label().Text(title).TextColor(Colors.White).FontSize(11).CenterHorizontal(),
                    new Label().Text(value).TextColor(Colors.White).FontSize(20).FontAttributes(FontAttributes.Bold).CenterHorizontal(),
                    new Label().Text("✎").TextColor(Color.FromArgb("#00FF85")).CenterHorizontal(),
                    new Label().Text(subtitle).TextColor(Colors.Gray).FontSize(10).CenterHorizontal().TextCenter()
                }
            }
        }.Column(col);
    }

    private View CreateBmiCard()
    {
        return new Border()
        {
            Stroke = Colors.White,
            StrokeThickness = 1,
            Padding = 12,
            Content = new VerticalStackLayout()
            {
                Spacing = 6,
                Children =
                {
                    new Label().Text("BMI SKORU").TextColor(Colors.White).FontSize(11).CenterHorizontal(),
                    new Label().Text("0").TextColor(Colors.White).FontSize(20).FontAttributes(FontAttributes.Bold).CenterHorizontal(),
                    new Label().Text("Veri girilmedi").TextColor(Colors.Gray).FontSize(10).CenterHorizontal()
                }
            }
        };
    }

    private Button CreateWaterButton(string text, int col)
    {
        var button = new Button()
            .Text(text)
            .BackgroundColor(Color.FromArgb("#00FF85"))
            .FontAttributes(FontAttributes.Bold)
            .TextColor(Colors.White)
            .FontSize(11)
            .HeightRequest(35)
            .CornerRadius(8)
            .BorderColor(Colors.White)
            .BorderWidth(1)
            .Column(col);

        button.Clicked += async (s, e) =>
        {
            if (text == "+ Özel")
            {
                await OpenSpecialWaterPopup();
            }
        };
        return button;
    }

    private View CreateBottomNav()
    {
        return new Border()
            .Stroke(Colors.White)
            .StrokeThickness(1)
            .Margin(new Thickness(-20, 0))
            .Padding(new Thickness(0, 10))
            .Content(new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star)
                },
                Children =
                {
                    CreateNavTab("🏠", "Ana Sayfa", 0)
                    .GestureRecognizers(new TapGestureRecognizer()
                    {
                        Command = new Command(async () => await Navigation.PushAsync(new MainDashboardPage()))
                    }),
                    CreateNavTab("📅", "Takvim", 1)
                    .GestureRecognizers(new TapGestureRecognizer()
                    {
                        Command = new Command(async () => await Navigation.PushAsync(new CalendarMainPage()))
                    }),
                    CreateNavTab("💰", "Bütçe", 2)
                    .GestureRecognizers(new TapGestureRecognizer()
                    {
                        Command = new Command(async () => await Navigation.PushAsync(new BudgetPage()))
                    }),
                    CreateNavTab("❤️", "Sağlık", 3, true)
                }
            });
    }

    private View CreateNavTab(string icon, string text, int col, bool isActive = false)
    {
        return new VerticalStackLayout()
        {
            Spacing = 2,
            Children =
            {
                new Label().Text(icon).FontSize(20).CenterHorizontal(),
                new Label()
                    .Text(text)
                    .TextColor(isActive ? Colors.CornflowerBlue : Colors.White)
                    .FontSize(10)
                    .CenterHorizontal()
            }
        }.Column(col);
    }

    private async Task OpenSpecialWaterPopup()
    {
        string result = await DisplayPromptAsync(
            title: "Özel Su Girişi",
            message: "İçtiğiniz miktarı mililitre (ml) cinsinden yazınız:",
            accept: "Ekle",
            cancel: "İptal",
            placeholder: "Örn: 330",
            maxLength: 4,
            keyboard: Keyboard.Numeric);

        if (!string.IsNullOrWhiteSpace(result))
        {
            await DisplayAlert("Başarılı", "Girilen su miktarı eklendi!", "Tamam");
        }
    }

    private async Task ChangeWaterGoal()
    {
        string result = await DisplayPromptAsync(
            title: "Hedef Güncelle",
            message: "Günlük su içme hedefinizi belirleyin (ml):",
            accept: "Güncelle",
            cancel: "İptal",
            placeholder: "Örn: 3000",
            initialValue: _targetWater.ToString(),
            keyboard: Keyboard.Numeric);

        if (double.TryParse(result, out double newGoal) && newGoal > 0)
        {
            _targetWater = newGoal;
            UpdateWaterUI(0); // UI'ı yeni hedefe göre tazele
        }
    }
    private void UpdateWaterUI(double addedAmount)
    {
        _currentWater += addedAmount;

        // Metinleri güncelle
        _waterLabel.Text = $"{_currentWater} ml / {_targetWater} ml";

    }
}