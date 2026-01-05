using FmgLib.MauiMarkup;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace Project2.Pages;

public class FULLMainPage : ContentPage
{
    private HorizontalStackLayout _actionButtonsPopup;

    public FULLMainPage()
    {
        this.BackgroundColor(Color.FromArgb("#23222E"));

        Content = new Grid()
        {
            Padding = new Thickness(20, 40, 20, 20),
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto), // 0: Üst Başlık
                new RowDefinition(GridLength.Auto), // 1: Yatay Özet Kartları
                new RowDefinition(GridLength.Star), // 2: Dikey Modül Listesi
                new RowDefinition(GridLength.Auto), // 3: Hızlı İşlem Butonları (+ Menüsü)
                new RowDefinition(GridLength.Auto)  // 4: Alt Navigasyon Bar
            },
            Children =
            {
                // 1. ÜST BAŞLIK (Row 0)
                new Grid()
                {
                    ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                    Children =
                    {
                        new Label()
                            .Text("Merhaba {Kullanıcı_Adi};")
                            .TextColor(Colors.White)
                            .FontSize(22)
                            .FontAttributes(FontAttributes.Bold)
                            .CenterVertical(),

                        new HorizontalStackLayout()
                        {
                            Spacing = 15,
                            Children = { new Label().Text("🔔").FontSize(22), new Label().Text("⚙️").FontSize(22) }
                        }.Column(1)
                    }
                }.Row(0).Margin(new Thickness(0,0,0,20)),

                // 2. YATAY ÖZET KARTLARI (Row 1)
                new ScrollView()
                {
                    Orientation = ScrollOrientation.Horizontal,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                    Content = new HorizontalStackLayout()
                    {
                        Spacing = 15,
                        Children =
                        {
                            CreateSummaryCard("Su Hedefi", "1500 ml / 2500 ml içildi", 0.6),
                            CreateSummaryCard("Günlük Harcama", "₺ 650 / ₺ 1000 harcandı", 0.65),
                            CreateSummaryCard("İlaç Takibi", "1 / 2 İlaç Alındı\nSıradaki: Vitamin C", 0.5)
                        }
                    }
                }.Row(1).Margin(new Thickness(0,0,0,30)),

                // 3. DİKEY MODÜL LİSTESİ (Row 2)
                new VerticalStackLayout()
                {
                    Spacing = 15,
                    Children =
                    {
                        // 1. SIRADA: Takvim - Dersler (Saatler güncellendi ve yer değiştirildi)
                        CreateModuleRow("Takvim", "Bugünkü Dersler", "09:00 - 11:50 - Nesne Yönelimli Prog.\n12:00 - 13:50 - Veri Yapıları\n14:00 - 15:50 - Ekonomi\n12:00 - 13:50 - Ders 1\n12:00 - 13:50 - Ders 2\n12:00 - 13:50 - Ders 3"),

                        // 2. SIRADA: Takvim - Etkinlikler
                        CreateModuleRow("Takvim", "Etkinlikler", "Matematik sınavına kalan gün: 7"),

                        // 3. SIRADA: Bütçe
                        CreateModuleRow("Bütçe", "Aylık Gelir / Gider", "Ayda Harcanan Para\n₺ 17000 / 30000", true)
                    }
                }.Row(2),

                // 4. HIZLI İŞLEM BUTONLARI (Row 3)
                new HorizontalStackLayout()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Spacing = 20,
                    Margin = new Thickness(0, 20),
                    Children =
                    {
                        new Border()
                        {
                            StrokeShape = new Ellipse(),
                            Stroke = Color.FromArgb("#00FF85"),
                            StrokeThickness = 3,
                            HeightRequest = 60,
                            WidthRequest = 60,
                            Content = new Label().Text("+").TextColor(Color.FromArgb("#00FF85")).FontSize(30).Center()
                        }
                        .GestureRecognizers(new TapGestureRecognizer()
                        {
                            Command = new Command(() => {
                                _actionButtonsPopup!.IsVisible = !_actionButtonsPopup.IsVisible;
                            })
                        }),

                        new HorizontalStackLayout()
                        {
                            Spacing = 15,
                            IsVisible = false,
                            Children =
                            {
                                CreateActionButton("💰", "harcama ekle"),
                                CreateActionButton("💧", "Su ekle"),
                            }
                        }.Assign(out _actionButtonsPopup)
                    }
                }.Row(3),

                // 5. ALT NAVİGASYON (Row 4)
                new Border()
                {
                    Stroke = Colors.White,
                    StrokeThickness = 1,
                    Margin = new Thickness(-20, 0),
                    Padding = new Thickness(0, 10),
                    Content = new Grid()
                    {
                        ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) },
                        Children =
                        {
                            CreateNavTab("🏠", "Ana Sayfa", 0, true),
                            CreateNavTab("📅", "Takvim", 1),
                            CreateNavTab("💰", "Bütçe", 2),
                            CreateNavTab("❤️", "Sağlık", 3)
                        }
                    }
                }.Row(4)
            }
        };
    }

    // --- YARDIMCI METOTLAR (CreateSummaryCard, CreateModuleRow, CreateActionButton, CreateNavTab) ---
    // (Önceki metot kodları burada yer alır...)

    private View CreateSummaryCard(string title, string info, double progress)
    {
        return new Border()
        {
            WidthRequest = 220,
            HeightRequest = 130,
            StrokeShape = new RoundRectangle() { CornerRadius = 20 },
            Background = new LinearGradientBrush(new GradientStopCollection {
                new GradientStop(Color.FromArgb("#2F58CD"), 0),
                new GradientStop(Color.FromArgb("#AD49E1"), 1)
            }, new Point(0, 0), new Point(1, 1)),
            Content = new VerticalStackLayout()
            {
                Padding = 15,
                Spacing = 5,
                Children =
                {
                    new Label().Text("Bugünkü Özet").TextColor(Colors.White).FontSize(12),
                    new Label().Text(title).TextColor(Colors.White).FontSize(16).FontAttributes(FontAttributes.Bold),
                    new Label().Text(info).TextColor(Colors.White).FontSize(11),
                    new ProgressBar().Progress(progress).ProgressColor(Colors.White).Margin(new Thickness(0,10,0,0))
                }
            }
        };
    }

    private View CreateModuleRow(string title, string subTitle, string contentText, bool showProgress = false)
    {
        var layout = new VerticalStackLayout()
        {
            VerticalOptions = LayoutOptions.Center,
            Children = {
                
                new Label()
                    .Text(contentText)
                    .TextColor(Colors.White)
                    .FontSize(13)
                    .HorizontalTextAlignment(TextAlignment.Center)
            }
        };

        if (showProgress)
            layout.Children.Add(new ProgressBar().Progress(0.5).ProgressColor(Colors.Blue).Margin(new Thickness(0, 5, 0, 0)));

        return new Border()
        {
            Stroke = Colors.White,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle() { CornerRadius = 15 },
            Padding = 15,
            Content = new Grid()
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) },
                Children = {
                    new VerticalStackLayout() {
                        Children = {
                            new Label().Text(title).TextColor(Colors.White).FontSize(18).FontAttributes(FontAttributes.Bold),
                            new Label().Text(subTitle).TextColor(Colors.Gray).FontSize(10)
                        }
                    }.Column(0),
                    layout.Column(1).CenterVertical()
                }
            }
        };
    }

    private View CreateActionButton(string icon, string text)
    {
        return new VerticalStackLayout()
        {
            Spacing = 5,
            Children = {
                new Border() {
                    HeightRequest = 45,
                    WidthRequest = 45,
                    StrokeShape = new Ellipse(),
                    Stroke = Color.FromArgb("#00FF85"),
                    Content = new Label().Text(icon).Center()
                },
                new Label().Text(text).TextColor(Colors.White).FontSize(10).CenterHorizontal()
            }
        };
    }

    private View CreateNavTab(string icon, string text, int col, bool isActive = false)
    {
        var color = isActive ? Colors.CornflowerBlue : Colors.White;
        return new VerticalStackLayout()
        {
            Spacing = 2,
            Children = {
                new Label().Text(icon).TextColor(color).FontSize(20).CenterHorizontal(),
                new Label().Text(text).TextColor(color).FontSize(10).CenterHorizontal()
            }
        }.Column(col);
    }
}
