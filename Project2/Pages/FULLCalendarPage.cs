using FmgLib.MauiMarkup;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;

namespace Project2.Pages;

public class FULLCalendarPage : ContentPage
{
    // Projen genelinde kullandığın özel renkler
    private readonly Color _ozelMavi = Color.FromArgb("#3F63FF");
    private readonly Color _ozelYesil = Color.FromArgb("#00FF85");

    public FULLCalendarPage()
    {
        // Sayfa arka planı
        this.BackgroundColor(Color.FromArgb("#23222E"));

        Content = new Grid()
        {
            Padding = new Thickness(20, 40, 20, 0),
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto), // 0: Üst Başlık
                new RowDefinition(GridLength.Auto), // 1: Tab Seçici
                new RowDefinition(GridLength.Auto), // 2: Takvim Izgarası
                new RowDefinition(GridLength.Star), // 3: Etkinlik Listesi (9 Aralık)
                new RowDefinition(GridLength.Auto)  // 4: Alt Navigasyon
            },
            Children =
            {
                // 0: ÜST BAŞLIK (Tarih, Arama ve Ekleme)
                new Grid()
                {
                    ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                    Children = {
                        new Label().Text("Takvim - Aralık 2025").TextColor(Colors.White).FontSize(20).FontAttributes(FontAttributes.Bold).VerticalOptions(LayoutOptions.Center),
                        new HorizontalStackLayout() {
                            Spacing = 15,
                            Children = {
                                new Label().Text("🔍").FontSize(22).VerticalOptions(LayoutOptions.Center).TextColor(Colors.White),
                                new Label().Text("+").TextColor(_ozelYesil).FontSize(30).FontAttributes(FontAttributes.Bold).VerticalOptions(LayoutOptions.Center)
                            }
                        }.Column(1)
                    }
                }.Row(0).Margin(new Thickness(0, 0, 0, 15)),

                // 1: GÖRÜNÜM TABLARI
                new Grid()
                {
                    ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) },
                    ColumnSpacing = 5,
                    Children = {
                        CreateTab("Günlük", false, 0),
                        CreateTab("Haftalık", false, 1),
                        CreateTab("Aylık", true, 2)
                    }
                }.Row(1).Margin(new Thickness(0, 0, 0, 15)),

                // 2: TAKVİM BÖLÜMÜ
                new VerticalStackLayout()
                {
                    Spacing = 5,
                    Children = {
                        // Gün İsimleri Satırı
                        new Grid() {
                            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) },
                            Children = {
                                new Label().Text("Pzt").TextColor(Colors.White).FontSize(10).HorizontalOptions(LayoutOptions.Center).Column(0),
                                new Label().Text("Sal").TextColor(Colors.White).FontSize(10).HorizontalOptions(LayoutOptions.Center).Column(1),
                                new Label().Text("Çar").TextColor(Colors.White).FontSize(10).HorizontalOptions(LayoutOptions.Center).Column(2),
                                new Label().Text("Per").TextColor(Colors.White).FontSize(10).HorizontalOptions(LayoutOptions.Center).Column(3),
                                new Label().Text("Cum").TextColor(Colors.White).FontSize(10).HorizontalOptions(LayoutOptions.Center).Column(4),
                                new Label().Text("Cmt").TextColor(Colors.White).FontSize(10).HorizontalOptions(LayoutOptions.Center).Column(5),
                                new Label().Text("Paz").TextColor(Colors.White).FontSize(10).HorizontalOptions(LayoutOptions.Center).Column(6)
                            }
                        },
                        // Takvim Hücreleri
                        CreateFullCalendarGrid()
                    }
                }.Row(2),

                // 3: 9 ARALIK DETAY LİSTESİ
                new ScrollView()
                {
                    Content = new VerticalStackLayout()
                    {
                        Spacing = 10, Padding = new Thickness(0, 15),
                        Children = {
                            new Label().Text("9 Aralık Salı").TextColor(Colors.White).FontSize(18).FontAttributes(FontAttributes.Bold).Margin(new Thickness(0,0,0,5)),
                            CreateEventCard("🎓", "Ders : Matematik", "(09:00 - 12:50)"),
                            CreateEventCard("🎓", "Ders : Fizik", "(13:00 - 15:50)"),
                            CreateEventCard("🎓", "Ders : Kimya", "(16:00 - 18:50)"),
                            CreateEventCard("💊", "İlaç : Vitamin C (1000 mg)", "(09:00)", true),
                            CreateEventCard("📍", "Etkinlik : Proje Toplantısı", "(12:00 - 16:00)"),
                            CreateEventCard("🧾", "Fatura : Elektrik (₺ 200)", "(Son ödeme : Bugün)")
                        }
                    }
                }.Row(3),

                // 4: ALT NAVİGASYON
                new Border() {
                    Stroke = Colors.White, StrokeThickness = 1, Margin = new Thickness(-20, 0), Padding = new Thickness(0, 10, 0, 15),
                    Content = new Grid() {
                        ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) },
                        Children = {
                            CreateNavTab("🏠", "Ana Sayfa", 0),
                            CreateNavTab("📅", "Takvim", 1, true),
                            CreateNavTab("💰", "Bütçe", 2),
                            CreateNavTab("❤️", "Sağlık", 3)
                        }
                    }
                }.Row(4).VerticalOptions(LayoutOptions.End)
            }
        };
    }

    private View CreateFullCalendarGrid()
    {
        var grid = new Grid()
        {
            RowSpacing = 1,
            ColumnSpacing = 1,
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) },
            RowDefinitions = { new RowDefinition(70), new RowDefinition(70), new RowDefinition(70), new RowDefinition(70), new RowDefinition(70) }
        };

        for (int i = 0; i < 31; i++)
        {
            int day = i + 1;
            int row = i / 7;
            int col = i % 7;

            var cellContent = new Grid();

            // Gün Numarası
            cellContent.Children.Add(new Label()
                .Text(day.ToString()).TextColor(Colors.White).FontSize(11).Margin(3)
                .HorizontalOptions(LayoutOptions.Start).VerticalOptions(LayoutOptions.Start));

            // İkon Listesi
            var emojis = GetEmojisForDay(day);

            // İkon Konteynırı (2 Satırlı Yapı)
            var iconStack = new VerticalStackLayout() { HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.End, Margin = new Thickness(0, 0, 4, 4), Spacing = 1 };
            var row1 = new HorizontalStackLayout() { Spacing = 1, HorizontalOptions = LayoutOptions.End };
            var row2 = new HorizontalStackLayout() { Spacing = 1, HorizontalOptions = LayoutOptions.End };

            if (emojis.Count > 0)
            {
                // Satır 1: İlk 3 ikon (Boyut 13)
                for (int j = 0; j < Math.Min(emojis.Count, 3); j++)
                    row1.Children.Add(new Label().Text(emojis[j]).FontSize(13).TextColor(Colors.White));

                // Satır 2: Diğerleri veya Sayaç
                if (emojis.Count > 3 && emojis.Count <= 6)
                {
                    for (int j = 3; j < emojis.Count; j++)
                        row2.Children.Add(new Label().Text(emojis[j]).FontSize(13).TextColor(Colors.White));
                }
                else if (emojis.Count > 6)
                {
                    row2.Children.Add(new Label().Text(emojis[3]).FontSize(13).TextColor(Colors.White));
                    row2.Children.Add(new Label().Text(emojis[4]).FontSize(13).TextColor(Colors.White));
                    row2.Children.Add(new Label().Text($"+{emojis.Count - 5}").FontSize(10).TextColor(_ozelYesil).FontAttributes(FontAttributes.Bold).VerticalOptions(LayoutOptions.Center));
                }
            }

            iconStack.Children.Add(row1);
            iconStack.Children.Add(row2);
            cellContent.Children.Add(iconStack);

            grid.Children.Add(new Border()
            {
                Stroke = Colors.Gray,
                StrokeThickness = 0.5,
                BackgroundColor = (day == 9) ? _ozelMavi : Colors.Transparent,
                Content = cellContent
            }.Row(row).Column(col));
        }
        return grid;
    }

    private List<string> GetEmojisForDay(int day)
    {
        var list = new List<string>();
        if (day == 12 || day == 25) list.Add("💊");
        if (day == 16 || day == 18 || day == 20) list.Add("🎓");
        if (day == 26) list.Add("🧾");
        if (day == 28) list.Add("📍");
        if (day == 9) list.AddRange(new[] { "🎓", "📍", "💊", "🧾", "🍎", "🏃", "📚" });
        return list;
    }

    private View CreateTab(string text, bool isActive, int col) => new Border() { Stroke = Colors.White, StrokeThickness = 1, BackgroundColor = isActive ? _ozelMavi : Colors.Transparent, Content = new Label().Text(text).TextColor(Colors.White).HorizontalOptions(LayoutOptions.Center).VerticalOptions(LayoutOptions.Center) }.HeightRequest(35).Column(col);

    private View CreateEventCard(string icon, string title, string detail, bool hasCheck = false) => new Border() { Stroke = Colors.White, StrokeThickness = 1, Padding = 10, Content = new Grid() { ColumnDefinitions = { new ColumnDefinition(35), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, Children = { new Label().Text(icon).FontSize(22).HorizontalOptions(LayoutOptions.Center).VerticalOptions(LayoutOptions.Center).Column(0), new Label().Text(title).TextColor(Colors.White).FontSize(13).VerticalOptions(LayoutOptions.Center).Column(1).Margin(new Thickness(10, 0)), new HorizontalStackLayout() { Spacing = 8, Children = { new Label().Text(detail).TextColor(Colors.White).FontSize(11).VerticalOptions(LayoutOptions.Center), hasCheck ? new Label().Text("✔️").TextColor(Colors.Gray).FontSize(16).VerticalOptions(LayoutOptions.Center) : new Label() } }.Column(2).VerticalOptions(LayoutOptions.Center) } } };

    private View CreateNavTab(string icon, string text, int col, bool isActive = false) => new VerticalStackLayout() { Spacing = 2, Children = { new Label().Text(icon).TextColor(Colors.White).FontSize(20).HorizontalOptions(LayoutOptions.Center), new Label().Text(text).TextColor(isActive ? _ozelMavi : Colors.White).FontSize(10).HorizontalOptions(LayoutOptions.Center) } }.Column(col);
}
