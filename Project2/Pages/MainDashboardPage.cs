using FmgLib.MauiMarkup;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Project2.Models;
using Project2.Services;
using Project2.WiewModels;

namespace Project2.Pages;

public class MainDashboardPage : ContentPage
{
    private HorizontalStackLayout _actionButtonsPopup; // + butonuna basınca açılan küçük buton grubunu tutar

    public MainDashboardPage()
    {
        BindingContext = new WiewModels.MainDashboardPageWiewModel();
        this.BackgroundColor(Color.FromArgb("#23222E"));

        Content = new Grid()
        {
            Padding = new Thickness(20, 40, 20, 20),
            RowDefinitions = // Sayfayı yatayda 4 satıra böldük
            {
                new RowDefinition(GridLength.Auto), // 0: Üst başlık
                new RowDefinition(GridLength.Star), // 1: Boş içerik alanı
                new RowDefinition(GridLength.Auto), // 2: + Menüsü
                new RowDefinition(GridLength.Auto)  // 3: Alt navigasyon        // (AUTO) içeriği kadar yer , // (STAR) kalan tüm alan
            },
            Children =
            {
                // 0: ÜST BAŞLIK
                new Grid()
                {
                    ColumnDefinitions = // Üst başlık satırını da 2 kolona böldük
                    {
                        new ColumnDefinition(GridLength.Star), // sol tarafa gelen yazı (kalan tüm alan)
                        new ColumnDefinition(GridLength.Auto) // sağ tarafa gelen bildirim - ayarlar ikonu (içerik kadar alan)
                    },

                    Children =
                    {
                        new Label() 
                            .Text($"Merhaba {UserSeassion.CurrentUser.Name};") // Karşılama yazısı. Varsayılan olarak sol kolana yazılır
                            .TextColor(Colors.White)
                            .FontSize(22)
                            .FontAttributes(FontAttributes.Bold)
                            .CenterVertical(),
                            
                        new HorizontalStackLayout()
                        {
                            Spacing = 15,
                            Children =
                            {
                                new Label()
                                    .Text("🔔") // Bildirim iconu
                                    .FontSize(22),

                                new Label()
                                    .Text("⚙️") // Ayarlar iconu
                                    .FontSize(22)
                            }
                        }.Column(1) // Bildirim ve ayarlar iconu sağ kolona yerleşti.
                    }
                }.Row(0).Margin(new Thickness(0,0,0,20)), // Karşılama yazısını ve iconları ilk satıra yerleşir, alta 20 px boşluk ekle.

                // 1. BOŞ EKRAN GÖRÜNÜMÜ
                new VerticalStackLayout()
                {
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 20,
                    Children =
                    {
                        new Label()
                            .Text("📝")
                            .FontSize(80)
                            .CenterHorizontal(),

                        new Label()
                            .Text("Takip asistanın henüz boş.\nVeri ekleyerek asistanını canlandır!")
                            .TextColor(Colors.Gray)
                            .FontSize(18)
                            .HorizontalTextAlignment(TextAlignment.Center) // Yazının kendisini yatayda ortalar 
                            .Margin(new Thickness(20, 0))
                    }
                }.Row(1), // 2.satıra yerleştir.

                // 3. HIZLI İŞLEM BUTONLARI
                new HorizontalStackLayout()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Spacing = 15,
                    Margin = new Thickness(0, 20),
                    Children =
                    {
                        // + BUTONU
                        new Border()
                        {
                            StrokeShape = new Ellipse() // Daire şeklinde çerçeve
                        }
                        .Stroke(Color.FromArgb("#00FF85")) // Yeşil kenarlık
                        .StrokeThickness(3) // 3px kalınlık
                        .HeightRequest(60) // Yükseklik 60px
                        .WidthRequest(60) // Genişlik 60px
                        .Content(
                            new Label()
                                .Text("+") // daire içine + yazımı
                                .TextColor(Color.FromArgb("#00FF85")) // kenarlık ile aynı renk + sembolü
                                .FontSize(30)
                                .Center()
                        )

                        .GestureRecognizers(new TapGestureRecognizer() // + butonunu tıklanabilir yaptık
                        {
                            Command = new Command(() => {
                                _actionButtonsPopup!.IsVisible = !_actionButtonsPopup.IsVisible; // Butona tıklandığında eğer buton gizli ise açar
                            })                                                                  // Açık ise butonu gizler
                        }),

                        new HorizontalStackLayout() // + butonu içindeki elemanları tutan menü
                        {
                            Spacing = 15,
                            IsVisible = false, // Sayfa ilk açıldığında + menüsü kapalı tıklayınca açılaca
                            Children =
                            {
                                CreateActionButton("💰", "harcama ekle",nameof(MainDashboardPageWiewModel.GotoBillPageCommand)),
                                CreateActionButton("💧", "Su ekle",nameof(MainDashboardPageWiewModel.GotoMedicinePageCommand)),
                            } 
                        }.Assign(out _actionButtonsPopup) // Menü (HorizontalStackLayout) artık _actionButtonsPopup değişkenine bağlandı
                    }
                }.Row(2), // + butonunu 3. satıra yerleştirdik

                // 4. ALT NAVİGASYON
                new Border()
                    .Stroke(Colors.White)
                    .StrokeThickness(1)
                    .Margin(new Thickness(-20, 0)) // Alt çerçeve kenarlara yapışması için -20 (çerçevenin dış boşluğu)
                    .Padding(new Thickness(0, 10)) // Alt çerçeve ile içindeki grid arası mesafe (çerçevenin dış boşluğu) üst-alt 10px
                    .Content(
                        new Grid()
                        {
                            ColumnDefinitions = // Gridi 4 sütuna böldük her sütun alanı eşit
                            {
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Star)
                            },
                            Children =
                            {
                                CreateNavTab("🏠", "Ana Sayfa", 0,nameof(MainDashboardPageWiewModel.GotoMainPageCommand),true), // Bir alt navigasyon sekmesi oluşturur
                                CreateNavTab("📅", "Takvim", 1,nameof(MainDashboardPageWiewModel.GotoCalendarPageCommand),false),          // İkon - Sekme ismi - Sütun yeri - Sekme aktif mi?
                                CreateNavTab("💰", "Bütçe", 2,nameof(MainDashboardPageWiewModel.GotoBillPageCommand), false),
                                CreateNavTab("❤️", "Sağlık", 3,nameof(MainDashboardPageWiewModel.GotoMedicinePageCommand), false)
                            }
                        }
                    ).Row(3) // 4.satıra yerleştir
            }
        };
    }
    // 3. Parametre olarak 'commandPath' ekledik
    private View CreateActionButton(string icon, string text, string commandPath)
    {
        // 1. Önce senin tasarımını bir değişkene atayalım
        var layout = new VerticalStackLayout()
        {
            Spacing = 5,
            Children = {
            new Border()
            {
                StrokeShape = new Ellipse()
            }
            .Stroke(Color.FromArgb("#00FF85"))
            .HeightRequest(45)
            .WidthRequest(45)
            .Content(
                new Label()
                    .Text(icon)
                    .Center()
            ),
            new Label()
                .Text(text)
                .TextColor(Colors.White)
                .FontSize(10)
                .CenterHorizontal()
        }
        };

        // 2. Şimdi bu tasarıma "Tıklama Yeteneği" (Gesture) ekleyelim
        var tapGesture = new TapGestureRecognizer();

        // Command'i buraya bağlıyoruz (Button.CommandProperty DEĞİL, TapGestureRecognizer.CommandProperty)
        tapGesture.Bind(TapGestureRecognizer.CommandProperty, commandPath);

        // Oluşturduğumuz bu yeteneği tasarıma ekliyoruz
        layout.GestureRecognizers.Add(tapGesture);

        return layout;
    }

    // Parametrelerin arasına 'string commandPath' ekledik
    private View CreateNavTab(string icon, string text, int col, string commandPath, bool isActive = false)
    {
        var layout = new VerticalStackLayout()
        {
            Spacing = 2,
            BackgroundColor = Colors.Transparent, // Tıklama alanı oluşsun diye
            Children = {
            new Label()
                .Text(icon)
                .TextColor(isActive ? Colors.CornflowerBlue : Colors.White)
                .FontSize(20)
                .CenterHorizontal()
                .InputTransparent(true), // 👈 EKLENECEK 1: Tıklamayı arkaya geçir

            new Label()
                .Text(text)
                .TextColor(isActive ? Colors.CornflowerBlue : Colors.White)
                .FontSize(10)
                .CenterHorizontal()
                .InputTransparent(true) // 👈 EKLENECEK 2: Tıklamayı arkaya geçir
        }
        };

        if (!string.IsNullOrEmpty(commandPath))
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Bind(TapGestureRecognizer.CommandProperty, commandPath);
            layout.GestureRecognizers.Add(tapGesture);
        }

        return layout.Column(col);
    }
}