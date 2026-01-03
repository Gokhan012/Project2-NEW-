using FmgLib.MauiMarkup;
using Microsoft.Maui.Controls.Shapes;

namespace Project2.Pages;

public class BudgetPage : ContentPage
{
    public BudgetPage()
    {
        this.BackgroundColor(Color.FromArgb("#23222E"));

        Content = new Grid()
        {
            Padding = new Thickness(20, 40, 20, 20),
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto), // 0: Üst başlık
                new RowDefinition(GridLength.Auto), // 1: Bütçe Sınırı Kartı
                new RowDefinition(GridLength.Auto), // 2: Gelir/Gider Özet
                new RowDefinition(GridLength.Star), // 3: İşlem Geçmişi (Liste)
                new RowDefinition(GridLength.Auto)  // 4: Alt navigasyon
            },
            Children =
            {
                // 0: ÜST BAŞLIK
                new Grid()
                {
                    // Grid'i tek parça bırakıyoruz, içindeki elemanları HorizontalOptions ile dağıtıyoruz
                    Children = {
                        // 1. SOLDA: "Bütçe" Yazısı
                        new Label()
                            .Text("Bütçe")
                            .TextColor(Colors.White)
                            .FontSize(20)
                            .FontAttributes(FontAttributes.Bold)
                            .HorizontalOptions(LayoutOptions.Start)
                            .CenterVertical(),

                            // 2. TAM ORTADA: "Aralık 2025" BURADA KULLANICI TARİHİ DEĞİŞTİREBİLMELİ.(sağ ve sol oklarla vb.)
                        new Label()
                            .Text("Aralık 2025")
                            .TextColor(Colors.White)
                            .FontSize(18)
                            .HorizontalOptions(LayoutOptions.Center)
                            .CenterVertical(),

                        // 3. SAĞDA: Ekleme ikonu
                        new HorizontalStackLayout()
                        {
                            Spacing = 15,
                            Children = {
                                new Label()
                                    .Text("+")
                                    .TextColor(Color.FromArgb("#00FF85"))
                                    .FontSize(30)
                                    .FontAttributes(FontAttributes.Bold)
                                    .CenterVertical()
                                    .GestureRecognizers(new TapGestureRecognizer()
                                    {
                                        Command = new Command(async () => await Navigation.PushAsync(new AddBudgetPage()))
                                    }),
                            }
                        }
                        .HorizontalOptions(LayoutOptions.End)
                        .CenterVertical()
                        }
                }.Row(0).Margin(new Thickness(0, 0, 0, 15)),

                // 1: AYLIK BÜTÇE SINIRI KARTI
                new Border()
                {
                    Stroke = Colors.White,
                    StrokeThickness = 1,
                    Padding = 15,
                    Content = new VerticalStackLayout()
                    {
                        Spacing = 10,
                        Children =
                        {
                            new Label()
                                .Text("Aylık Bütçe Sınırı")
                                .TextColor(Colors.White)
                                .FontSize(14),

                            new ProgressBar() // KULLANICI VERİ GİRERSE BU BAR DOLMALI
                                .Progress(0)
                                .ProgressColor(Color.FromArgb("#00FF85"))
                                .HeightRequest(10),

                            new Label()
                                .HorizontalOptions(LayoutOptions.End)
                                .FormattedText(new FormattedString()
                                {
                                    Spans = // KULLANICI VERİ GİRERSE BU ALANLAR DEĞİŞMELİ
                                    {
                                        new Span().Text("Kalan : ")
                                            .TextColor(Colors.White),
                                        new Span()
                                            .Text("₺0")
                                            .TextColor(Color.FromArgb("#00FF85"))
                                            .FontAttributes(FontAttributes.Bold),
                                        new Span()
                                            .Text(" / ₺0")
                                            .TextColor(Colors.White)
                                    }
                                })
                        }
                    }
                }.Row(1).Margin(new Thickness(0, 0, 0, 20)),

                // 2: GELİR VE GİDER ÖZET KARTLARI
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
                        // Toplam Gelir. KULLANICI VERİ GİRERSE DEĞİŞMELİ
                        CreateSummaryCard("Toplam Gelir", "↑", "₺0", Color.FromArgb("#00FF85"), 0),
                        // Toplam Gider. KULLANICI VERİ GİRERSE DEĞİŞMELİ
                        CreateSummaryCard("Toplam Gider", "↓", "₺0", Color.FromArgb("#FF5252"), 1)
                    }
                }.Row(2).Margin(new Thickness(0, 0, 0, 20)),

                // 3: İŞLEM GEÇMİŞİ
                new ScrollView()
                {
                    Content = new VerticalStackLayout()
                    {
                        Spacing = 10,
                        Padding = new Thickness(0, 15),
                        Children = {
                            new Label().
                                Text("İşlem Geçmişi")
                                .TextColor(Colors.White)
                                .FontSize(18)
                                .FontAttributes(FontAttributes.Bold)
                                .Margin(new Thickness(0, 0, 0, 5)),
                            new Label()
                                .Text("Henüz bir işlem bulunmuyor.")
                                .TextColor(Colors.Gray)
                                .FontSize(14)
                                .Margin(new Thickness(0, 10, 0, 0))
                        }
                    }
                }.Row(3),

                // 3: ALT NAVİGASYON
                new Border()
                    .Stroke(Colors.White)
                    .StrokeThickness(1)
                    .Margin(new Thickness(-20, 0))
                    .Padding(new Thickness(0, 10))
                    .Content(
                        new Grid()
                        {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Star)
                            },
                            Children = {
                                CreateNavTab("🏠", "Ana Sayfa", 0)
                                .GestureRecognizers(new TapGestureRecognizer()
                                {
                                    Command = new Command(async () => await Navigation.PushAsync(new MainDashboardPage()))
                                }),
                                CreateNavTab("📅", "Takvim", 1)
                                .GestureRecognizers(new TapGestureRecognizer() // Takvim ikonuna basınca takvim ekranı açılır.
                                {
                                    Command = new Command(async () => await Navigation.PushAsync(new CalendarMainPage()))
                                }),
                                CreateNavTab("💰", "Bütçe", 2, true), // Bütçe aktif
                                CreateNavTab("❤️", "Sağlık", 3)
                                .GestureRecognizers(new TapGestureRecognizer()
                                {
                                    Command = new Command(async () => await Navigation.PushAsync(new HealthPage()))
                                }),
                            }
                        }
                    ).Row(4)
            }
        };
    }

    // Yardımcı Metot: Özet Kartları (Gelir/Gider) için
    private View CreateSummaryCard(string title, string arrow, string amount, Color themeColor, int col)
    {
        return new Border()
        {
            Stroke = Colors.White,
            StrokeThickness = 1,
            Padding = 10,
            HeightRequest = 100,
            Content = new VerticalStackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
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
                                .Text(title)
                                .TextColor(Colors.White)
                                .FontSize(14),
                            new Label()
                                .Text(arrow)
                                .TextColor(themeColor)
                                .FontSize(30)
                                .Column(1)
                        }
                    },
                    new Label()
                    .Text(amount)
                    .TextColor(themeColor)
                    .FontSize(18)
                    .FontAttributes(FontAttributes.Bold)
                    .Margin(0,10,0,0)
                }
            }
        }.Column(col);
    }

    // Yardımcı Metot: Navigasyon Sekmeleri için
    private View CreateNavTab(string icon, string text, int col, bool isActive = false) // En alt satır için icon ve yazı üretir (ana sayfa - takvim vb)
    {
        return new VerticalStackLayout()
        {
            Spacing = 2,
            Children = {

                new Label()
                    .Text(icon) // İconlar (ev - takvim vb)
                    .FontSize(20)
                    .CenterHorizontal(),

                new Label()
                    .Text(text) // Sekmenin adı (ana sayfa - takvim vb)
                    .TextColor(isActive ? Colors.CornflowerBlue : Colors.White) // Aktif yazı mavi olmayan yazı beyaz (iconlar ile aynı renk)
                    .FontSize(10)
                    .CenterHorizontal()
            }
        }.Column(col); // Hangi kolona konulacağını parametre ile belirliyoruz.
    }
}
