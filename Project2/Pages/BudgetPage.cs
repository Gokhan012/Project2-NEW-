using FmgLib.MauiMarkup;
using Microsoft.Maui.Controls.Shapes;
using Project2.ViewModels;
using Project2.Models;
using Project2.Data;

namespace Project2.Pages;

public class BudgetPage : ContentPage
{
    private BudgetPageViewModel _viewModel;
    public BudgetPage()
    {
        _viewModel = new BudgetPageViewModel();
        BindingContext = _viewModel;

        this.BackgroundColor(Color.FromArgb("#23222E"));

        Content = new Grid()
        {
            Padding = new Thickness(20, 40, 20, 20),
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto), // 0: Üst başlık
                new RowDefinition(GridLength.Auto), // 1: Bütçe Sınırı Kartı
                new RowDefinition(GridLength.Auto), // 2: Gelir/Gider Özet
                new RowDefinition(GridLength.Star), // 3: İşlem Geçmişi (LİSTE)
                new RowDefinition(GridLength.Auto)  // 4: Alt navigasyon
            },
            Children =
            {
                // 0: ÜST BAŞLIK
                new Grid()
                {
                    Children = {
                        // 1. SOLDA: "Bütçe" Yazısı
                        new Label()
                            .Text("Bütçe")
                            .TextColor(Colors.White)
                            .FontSize(20)
                            .FontAttributes(FontAttributes.Bold)
                            .HorizontalOptions(LayoutOptions.Start)
                            .CenterVertical(),

                        // 2. TAM ORTADA: Tarih Değiştirici
                        new HorizontalStackLayout()
                        {
                            Spacing = 10,
                            HorizontalOptions = LayoutOptions.Center,
                            Children =
                            {
                                new Label().Text("◀").TextColor(Colors.Gray).FontSize(18)
                                    .GestureRecognizers(new TapGestureRecognizer().Command(_viewModel.PreviousMonthCommand)),

                                new Label()
                                    .TextColor(Colors.White)
                                    .FontSize(18)
                                    .CenterVertical()
                                    .Bind(Label.TextProperty, nameof(BudgetPageViewModel.CurrentDateText)),

                                new Label().Text("▶").TextColor(Colors.Gray).FontSize(18)
                                    .GestureRecognizers(new TapGestureRecognizer().Command(_viewModel.NextMonthCommand)),
                            }
                        }.CenterVertical(),

                        // 3. SAĞDA: Ekleme ikonu
                        new Label()
                            .Text("+")
                            .TextColor(Color.FromArgb("#00FF85"))
                            .FontSize(30)
                            .FontAttributes(FontAttributes.Bold)
                            .HorizontalOptions(LayoutOptions.End)
                            .CenterVertical()
                            .GestureRecognizers(new TapGestureRecognizer()
                            {
                                Command = new Command(async () => await Navigation.PushAsync(new AddBudgetPage()))
                            })
                    }
                }.Row(0).Margin(new Thickness(0, 0, 0, 15)),

                // 1: AYLIK BÜTÇE SINIRI KARTI
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
            // 1. DÜZELTME: Burası sadece metin olmalı, Command buraya bağlanmaz.
            new Label()
                .Text("Aylık Bütçe Sınırı")
                .TextColor(Colors.White)
                .FontSize(14),

            // Progress Bar
            new ProgressBar()
                .ProgressColor(Color.FromArgb("#00FF85"))
                .HeightRequest(10)
                .Bind(ProgressBar.ProgressProperty, nameof(BudgetPageViewModel.ProgressValue)),

            // Kalan / Toplam Yazısı
            new Label()
                .HorizontalOptions(LayoutOptions.End)
                .FormattedText(new FormattedString()
                {
                    Spans =
                    {
                        new Span().Text("Kalan : ").TextColor(Colors.White),
                        new Span()
                            .TextColor(Color.FromArgb("#00FF85"))
                            .FontAttributes(FontAttributes.Bold)
                            .Bind(Span.TextProperty, nameof(BudgetPageViewModel.BudgetUsageText))
                    }
                })
                    }
                }
            }
            .Row(1)
            .Margin(new Thickness(0, 0, 0, 20))
            // 2. DÜZELTME: Tıklama özelliği (Command) buraya, yani Border'a eklenir.
            .GestureRecognizers(new TapGestureRecognizer()
            {
                Command = _viewModel.EditBudgetLimitCommand
            }),

                // 2: GELİR VE GİDER ÖZET KARTLARI
                new Grid()
                {
                    ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) },
                    ColumnSpacing = 15,
                    Children =
                    {
                        CreateSummaryCard("Toplam Gelir", "↑", nameof(BudgetPageViewModel.TotalIncome), Color.FromArgb("#00FF85"), 0),
                        CreateSummaryCard("Toplam Gider", "↓", nameof(BudgetPageViewModel.TotalExpense), Color.FromArgb("#FF5252"), 1)
                    }
                }.Row(2).Margin(new Thickness(0, 0, 0, 20)),

                // 3: İŞLEM GEÇMİŞİ (DÜZELTİLEN KISIM: CollectionView)
                new CollectionView()
                {
                    SelectionMode = SelectionMode.None,
                    
                    // Liste Başlığı
                    Header = new Label()
                        .Text("İşlem Geçmişi")
                        .TextColor(Colors.White)
                        .FontSize(18)
                        .FontAttributes(FontAttributes.Bold)
                        .Margin(new Thickness(0, 0, 0, 10)),

                    // Veri Yoksa Görünecek Alan
                    EmptyView = new Label()
                        .Text("Henüz bir işlem bulunmuyor.")
                        .TextColor(Colors.Gray)
                        .FontSize(14)
                        .HorizontalOptions(LayoutOptions.Center)
                        .Margin(new Thickness(0, 20, 0, 0)),

                    // Satır Tasarımı
                    ItemTemplate = new DataTemplate(() =>
                    {
                        return new Border()
                        {
                            Stroke = Colors.Transparent,
                            BackgroundColor = Color.FromArgb("#2D2C39"),
                            Padding = 10,
                            Margin = new Thickness(0, 0, 0, 10),
                            Content = new Grid()
                            {
                                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                                Children =
                                {
                                    // Sol Taraf (Başlık ve Tarih) - AYNI KALSIN
                                    new VerticalStackLayout()
                                    {
                                        Children =
                                        {
                                            new Label().TextColor(Colors.White).FontSize(16).Bind(Label.TextProperty, nameof(tblBudget.Title)),
                                            new Label().TextColor(Colors.Gray).FontSize(12).Bind(Label.TextProperty, nameof(tblBudget.Date), stringFormat: "{0:dd.MM.yyyy}")
                                        }
                                    }.Column(0),

                                    // SAĞ TARAF (TUTAR) - GÜNCELLENEN KISIM
                                    new Label()
                                        .FontSize(16)
                                        .FontAttributes(FontAttributes.Bold)
                                        .CenterVertical()
                                        .Column(1)
                                        // Tutarı Yaz
                                        .Bind(Label.TextProperty, nameof(tblBudget.Amount), stringFormat: "₺{0}")
                    
                                        // Rengi Doğrudan Modelden Al (Converter Yok!)
                                        .Bind(Label.TextColorProperty, nameof(tblBudget.TransactionColor))
                                }
                            }
                        };
                    })
                }
                .Row(3) // Grid'in 3. satırına yerleş
                .Bind(CollectionView.ItemsSourceProperty, nameof(BudgetPageViewModel.Transactions)), // Veriyi bağla

                // 4: ALT NAVİGASYON
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
                                CreateNavTab("🏠", "Ana Sayfa", 0).GestureRecognizers(new TapGestureRecognizer(){ Command = new Command(async () => await Navigation.PushAsync(new MainDashboardPage())) }),
                                CreateNavTab("📅", "Takvim", 1).GestureRecognizers(new TapGestureRecognizer(){ Command = new Command(async () => await Navigation.PushAsync(new CalendarMainPage())) }),
                                CreateNavTab("💰", "Bütçe", 2, true),
                                CreateNavTab("❤️", "Sağlık", 3).GestureRecognizers(new TapGestureRecognizer(){ Command = new Command(async () => await Navigation.PushAsync(new HealthPage())) }),
                            }
                        }
                    ).Row(4)
            }
        };
    }

    private View CreateSummaryCard(string title, string arrow, string bindingPath, Color themeColor, int col)
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
                        ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                        Children =
                        {
                            new Label().Text(title).TextColor(Colors.White).FontSize(14),
                            new Label().Text(arrow).TextColor(themeColor).FontSize(30).Column(1)
                        }
                    },
                    new Label()
                    .TextColor(themeColor)
                    .FontSize(18)
                    .FontAttributes(FontAttributes.Bold)
                    .Margin(0,10,0,0)
                    .Bind(Label.TextProperty, bindingPath, stringFormat: "₺{0}")
                }
            }
        }.Column(col);
    }

    private View CreateNavTab(string icon, string text, int col, bool isActive = false)
    {
        return new VerticalStackLayout()
        {
            Spacing = 2,
            Children = {
                new Label().Text(icon).FontSize(20).CenterHorizontal(),
                new Label().Text(text).TextColor(isActive ? Colors.CornflowerBlue : Colors.White).FontSize(10).CenterHorizontal()
            }
        }.Column(col);
    }
   
    // SAYFA HER GÖRÜNDÜĞÜNDE ÇALIŞIR
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // ViewModel içindeki LoadData'yı tetikler ve veritabanını okur
        if (_viewModel != null)
        {
            await _viewModel.LoadData();
        }
    }



} // Class bitiş parantezi
