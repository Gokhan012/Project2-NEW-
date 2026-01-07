using FmgLib.MauiMarkup;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Project2.Services;
using Project2.WiewModels;

namespace Project2.Pages;

public class HealthPage : ContentPage
{
    
    private Ellipse _progressCircle;
    private Label _targetGoalLabel;

    public HealthPage()
    {
        BindingContext = new HealthPageWiewModel();
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
                    CreateStatusCard(
                            title: "Kilo",
                            value: "0 kg",
                            subtitle: "Güncellemek için dokun",
                            col: 0,
                            // 1. VERİ BAĞLANTISI 
                            valueBinding: new Binding("CurrentUser.Weight", stringFormat: "{0} kg")
                            {
                                TargetNullValue = "0 kg",
                                FallbackValue = "0 kg"
                            },
                            commandPath: "KgChancherCommand"
                        ),
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
                            // KATMAN 1: Arka plandaki sabit duran soluk halka (Yol)
                            new Ellipse()
                            .Stroke(Color.FromArgb("#1A2196F3"))
                            .StrokeThickness(10),

                             // Ön plan (Dolan Bar)
                             new Ellipse()
                            .Stroke(Color.FromArgb("#2196F3"))
                            .StrokeThickness(10)
                            .Rotation(-90)
                            .CenterHorizontal()
                            .CenterVertical()
                            .Bind(Ellipse.StrokeDashArrayProperty, "SuTakibiVM.WaterStrokeDashArray"),
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
                                        new TapGestureRecognizer()
                                        .Bind(TapGestureRecognizer.CommandProperty, "SuTakibiVM.ChangeWaterGoalCommand")
                                     },

                                Content = new VerticalStackLayout()
                                {
                                    VerticalOptions = LayoutOptions.Center,
                                    InputTransparent = true,
                                    Children =
                                    {
                                        // Mevcut miktar etiketi
                                         new Label()
                                        //Burada text vardı Bind olduğu için sildim
                                        .TextColor(Colors.Black)
                                        .FontAttributes(FontAttributes.Bold)
                                        .CenterHorizontal()
                                        .Bind(Label.TextProperty, "SuTakibiVM.WaterLabelText"),


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
                                CreateWaterButton("+250 ml", 0, "250"),
                                CreateWaterButton("+500 ml", 1, "500"),
                                CreateWaterButton("+ Özel", 2, "Custom")
                            }
                        }
                    }
                }.Row(2),

                    // ... önceki satırlar (Row 0, 1, 2) aynı ...

                    new VerticalStackLayout()
                    {
                        Spacing = 10,
                        Margin = new Thickness(0, 25, 0, 0),
                        Children =
                        {
                            // BAŞLIK GRUBU
                            new Grid()
                            {
                                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                                Children =
                                {
                                    new Label().Text("İlaç Takibi").TextColor(Colors.White).FontSize(18).FontAttributes(FontAttributes.Bold),
                                    new Label().Text("+").TextColor(Color.FromArgb("#00FF85")).FontSize(30).FontAttributes(FontAttributes.Bold).Column(1)
                                        .GestureRecognizers(new TapGestureRecognizer() {
                                            Command = new Command(async () => await Navigation.PushAsync(new AddMedicinePage()))
                                        }),
                                }
                            },

                            // LİSTE GRUBU 
                           new Grid()
{
                                Children =
                                {
                                    // Liste boşken görünecek Label
                                    new Label()
                                        .Text("Henüz ilaç eklenmedi.\nİlaç eklemek için + butonuna dokunun.")
                                        .TextColor(Colors.Gray)
                                        .FontSize(13).CenterHorizontal().CenterVertical().TextCenter()
                                        .Bind(Label.IsVisibleProperty, "MedicineList.Count", convert: (int count) => count == 0),

                                    // İlaç Listesi
                                  new ScrollView()
                                    {
                                        Content = new VerticalStackLayout()
                                        {
                                            Spacing = 10,
                                        }
                                        .Bind(BindableLayout.ItemsSourceProperty, "MedicineList")
                                        // BindableLayout olduğunu açıkça belirten metot:
                                        .BindableLayoutItemTemplate(new DataTemplate(() => CreateMedicineCard()))
                                    }
                                        .Bind(ScrollView.IsVisibleProperty, "MedicineList.Count", convert: (int count) => count > 0)
                                    
                                }
                            }
                        }
                    }.Row(3), 

                    CreateBottomNav().Row(4)
            }
        };
    }

    // --- YARDIMCI METOTLAR ---

    private View CreateStatusCard(string title, string value, string subtitle, int col, BindingBase valueBinding = null, string commandPath = null)

    {

        var valuelabel = new Label().Text(value).TextColor(Colors.White).FontSize(20).FontAttributes(FontAttributes.Bold).CenterHorizontal();

        if (valueBinding != null)

        {

            valuelabel.SetBinding(Label.TextProperty, valueBinding);

        }

        var border = new Border()

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

                   valuelabel,

                    new Label().Text("✎").TextColor(Color.FromArgb("#00FF85")).CenterHorizontal(),

                    new Label().Text(subtitle).TextColor(Colors.Gray).FontSize(10).CenterHorizontal().TextCenter()

                }

            }

        };

        if (!string.IsNullOrEmpty(commandPath))

        {

            var tapGesture = new TapGestureRecognizer();

            tapGesture.SetBinding(TapGestureRecognizer.CommandProperty, commandPath);

            border.GestureRecognizers.Add(tapGesture);

        }

        return border.Column(col);

    }
    private View CreateBmiCard()
    {
        double sonuc = VkiHesaplayici.hesapla(
    UserSeassion.CurrentUser.Weight ?? 0,
    UserSeassion.CurrentUser.Height ?? 0
);
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
                    new Label().Text(sonuc.ToString("F2")).TextColor(Colors.White).FontSize(20).FontAttributes(FontAttributes.Bold).CenterHorizontal(),
                    new Label().Text(Services.VkiHesaplayici.aralikGetir(sonuc)).TextColor(Colors.Gray).FontSize(10).CenterHorizontal()
                }
            }
        };
    }

    private Button CreateWaterButton(string text, int col, string parameter)
    {
        return new Button()
            .Text(text)
            .BackgroundColor(Color.FromArgb("#00FF85"))
            .FontAttributes(FontAttributes.Bold)
            .TextColor(Colors.White)
            .FontSize(11)
            .HeightRequest(35)
            .CornerRadius(8)
            .BorderColor(Colors.White)
            .BorderWidth(1)
            .Column(col)
            // 1. Komutu Bağla: SuTakibiVM içindeki AddWaterCommand
            .Bind(Button.CommandProperty, "SuTakibiVM.AddWaterCommand")
            // 2. Parametreyi Gönder: "250" veya "Custom"
            .CommandParameter(parameter);
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
                        Command = new Command(async () => await Navigation.PushAsync(new FULLCalendarPage()))
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

    private View CreateMedicineCard()
    {
        var checkLabel = new Label { Text = "✔", TextColor = Colors.Gray, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

        // İçildi tetikleyicisi
        checkLabel.Triggers.Add(new DataTrigger(typeof(Label))
        {
            Binding = new Binding("LastTakenDate.Date"),
            Value = DateTime.Today,
            Setters = { new Setter { Property = Label.TextColorProperty, Value = Color.FromArgb("#00FF85") } }
        });

        var checkBorder = new Border { HeightRequest = 35, WidthRequest = 35, Stroke = Colors.Gray, Content = checkLabel };

        checkBorder.Triggers.Add(new DataTrigger(typeof(Border))
        {
            Binding = new Binding("LastTakenDate.Date"),
            Value = DateTime.Today,
            Setters = { new Setter { Property = Border.StrokeProperty, Value = Color.FromArgb("#00FF85") } }
        });

        return new Border
        {
            Stroke = Color.FromArgb("#33FFFFFF"),
            BackgroundColor = Color.FromArgb("#1AFFFFFF"),
            Padding = 15,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Auto) },
                Children = {
                    new VerticalStackLayout {
                        Children = {
                            new Label().TextColor(Colors.White).FontAttributes(FontAttributes.Bold).Bind(Label.TextProperty, "MedicineName"),
                            new Label().TextColor(Colors.Gray).FontSize(12).Bind(Label.TextProperty, "MedicineDose", stringFormat: "{0} mg")
                        }
                    }.Column(0).CenterVertical(),
                    new Label().TextColor(Colors.LightGray).Margin(10,0).Bind(Label.TextProperty, "MedicineTime", stringFormat: "{0:HH:mm}").Column(1).CenterVertical(),

                    checkBorder.Column(2).CenterVertical().GestureRecognizers(new TapGestureRecognizer()
                        .Bind(TapGestureRecognizer.CommandProperty, "ToggleMedicineCommand", source: this.BindingContext)//BindingContext vardı sildim
                        .Bind(TapGestureRecognizer.CommandParameterProperty, ".")),

                    new Label { Text = "🗑", TextColor = Colors.Red, FontSize = 20 }.Column(3).CenterVertical().Margin(10,0,0,0)//BindingContext vardı sildim
                        .GestureRecognizers(new TapGestureRecognizer()
                        .Bind(TapGestureRecognizer.CommandProperty, "DeleteMedicineCommand", source: this.BindingContext)
                        .Bind(TapGestureRecognizer.CommandParameterProperty, "."))          
                }
            }
        };
    }

                // Sayfa her ekrana geldiğinde çalışır
                protected override async void OnAppearing()
                {
                    base.OnAppearing();

                    if (BindingContext is HealthPageWiewModel vm)
                    {
                        // 1. Kullanıcı oturum verilerini (Kilo vb.) yükle
                        vm.LoadUserData();

                        // 2. İlaç listesini veritabanından async olarak çek
                        // await kullanarak verilerin tam yüklendiğinden emin oluyoruz
                        await vm.LoadMedicines();
                    }
                }
            }