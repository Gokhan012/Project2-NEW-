using FmgLib.MauiMarkup;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using MyAppMAUI.Pages;
using Project2.WiewModels;
using static Microsoft.Maui.GridLength;

namespace Project2.Pages;

public class CalendarMainPage : ContentPage
{
    private CalendarMainPageWiew _viewModel;
    private Grid _calendarGrid;

    public CalendarMainPage()
    {
        _viewModel = new CalendarMainPageWiew();
        BindingContext = _viewModel;

        this.BackgroundColor(Color.FromArgb("#23222E"));

        _viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(CalendarMainPageWiew.CurrentDate) ||
                e.PropertyName == nameof(CalendarMainPageWiew.SelectedDate))
            {
                RenderCalendar();
            }
        };

        Content = new Grid()
        {
            Padding = new Thickness(20, 40, 20, 20),
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            },
            Children =
            {
                // 0: ÜST BAŞLIK
                new Grid()
                {
                    Children = {
                        new Label().Text("Takvim").TextColor(Colors.White).FontSize(20).FontAttributes(FontAttributes.Bold)
                            .HorizontalOptions(LayoutOptions.Start).CenterVertical(),

                        new HorizontalStackLayout()
                        {
                            Spacing = 15,
                            HorizontalOptions = LayoutOptions.Center,
                            Children =
                            {
                                new Label().Text("◀").TextColor(Colors.Gray).FontSize(18).Padding(10).BackgroundColor(Colors.Transparent).CenterVertical()
                                    .GestureRecognizers(new TapGestureRecognizer().Bind(TapGestureRecognizer.CommandProperty, nameof(CalendarMainPageWiew.PreviousMonthCommand))),

                                new Label().TextColor(Colors.White).FontSize(18).CenterVertical()
                                    .Bind(Label.TextProperty, nameof(CalendarMainPageWiew.CurrentMonthYearText)),

                                new Label().Text("▶").TextColor(Colors.Gray).FontSize(18).Padding(10).BackgroundColor(Colors.Transparent).CenterVertical()
                                    .GestureRecognizers(new TapGestureRecognizer().Bind(TapGestureRecognizer.CommandProperty, nameof(CalendarMainPageWiew.NextMonthCommand))),
                            }
                        }.Column(1),

                        new HorizontalStackLayout()
                        {
                            Spacing = 15,
                            Children = {
                                new Label().Text("🔍").FontSize(22).CenterVertical(),
                                new Label().Text("+").TextColor(Color.FromArgb("#00FF85")).FontSize(30).FontAttributes(FontAttributes.Bold).CenterVertical()
                                    .GestureRecognizers(new TapGestureRecognizer().Command(new Command(async () => await Navigation.PushAsync(new AddCalendarEventPage()))))
                            }
                        }.HorizontalOptions(LayoutOptions.End).CenterVertical()
                    }
                }.Row(0).Margin(new Thickness(0, 0, 0, 15)),

                // 1: TAKVİM BÖLÜMÜ
                new VerticalStackLayout()
                {
                    Spacing = 5,
                    Children = {
                        new Grid() {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition(Star), new ColumnDefinition(Star), new ColumnDefinition(Star),
                                new ColumnDefinition(Star), new ColumnDefinition(Star), new ColumnDefinition(Star), new ColumnDefinition(Star)
                            },
                            Children = {
                                new Label().Text("Pzt").TextColor(Colors.White).FontSize(10).CenterHorizontal().Column(0),
                                new Label().Text("Sal").TextColor(Colors.White).FontSize(10).CenterHorizontal().Column(1),
                                new Label().Text("Çar").TextColor(Colors.White).FontSize(10).CenterHorizontal().Column(2),
                                new Label().Text("Per").TextColor(Colors.White).FontSize(10).CenterHorizontal().Column(3),
                                new Label().Text("Cum").TextColor(Colors.White).FontSize(10).CenterHorizontal().Column(4),
                                new Label().Text("Cmt").TextColor(Colors.White).FontSize(10).CenterHorizontal().Column(5),
                                new Label().Text("Paz").TextColor(Colors.White).FontSize(10).CenterHorizontal().Column(6)
                            }
                        },

                        new Grid()
                        {
                            RowSpacing = 5,
                            ColumnSpacing = 5,
                            ColumnDefinitions =
                            {
                                new ColumnDefinition(Star), new ColumnDefinition(Star), new ColumnDefinition(Star),
                                new ColumnDefinition(Star), new ColumnDefinition(Star), new ColumnDefinition(Star), new ColumnDefinition(Star)
                            },
                            RowDefinitions =
                            {
                                new RowDefinition(50), new RowDefinition(50), new RowDefinition(50),
                                new RowDefinition(50), new RowDefinition(50), new RowDefinition(50)
                            }
                        }.Assign(out _calendarGrid)
                    }
                }.Row(1),

                // 2: ETKİNLİK LİSTESİ (SEÇİLİ GÜN YAZISI BURADA)
                new ScrollView()
                {
                    Content = new VerticalStackLayout()
                    {
                        Spacing = 10,
                        Padding = new Thickness(0, 15),
                        Children = {
                            // 👇 GÜNCELLENDİ: Burası artık ViewModel'deki SelectedDateText'e bağlı
                            new Label()
                                .TextColor(Colors.White)
                                .FontSize(18)
                                .FontAttributes(FontAttributes.Bold)
                                .Margin(new Thickness(0, 0, 0, 5))
                                .Bind(Label.TextProperty, nameof(CalendarMainPageWiew.SelectedDateText)),

                            new Label().Text("Henüz bir etkinlik eklenmedi.").TextColor(Colors.Gray).FontSize(14).Margin(new Thickness(0, 10, 0, 0))
                        }
                    }
                }.Row(2),

                // 3: ALT NAVİGASYON
                new Border()
                    .Stroke(Colors.White).StrokeThickness(1).Margin(new Thickness(-20, 0)).Padding(new Thickness(0, 10))
                    .Content(
                        new Grid()
                        {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition(Star), new ColumnDefinition(Star),
                                new ColumnDefinition(Star), new ColumnDefinition(Star)
                            },
                            Children = {
                                CreateNavTab("🏠", "Ana Sayfa", 0).GestureRecognizers(new TapGestureRecognizer() { Command = new Command(async () => await Navigation.PushAsync(new MainDashboardPage())) }),
                                CreateNavTab("📅", "Takvim", 1, true),
                                CreateNavTab("💰", "Bütçe", 2).GestureRecognizers(new TapGestureRecognizer() { Command = new Command(async () => await Navigation.PushAsync(new BudgetPage())) }),
                                CreateNavTab("❤️", "Sağlık", 3).GestureRecognizers(new TapGestureRecognizer() { Command = new Command(async () => await Navigation.PushAsync(new HealthPage())) }),
                            }
                        }
                    ).Row(3)
            }
        };

        RenderCalendar();
    }

    private void RenderCalendar()
    {
        if (_calendarGrid == null) return;
        _calendarGrid.Children.Clear();

        DateTime currentMonth = _viewModel.CurrentDate;
        int daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
        DateTime firstDay = new DateTime(currentMonth.Year, currentMonth.Month, 1);
        int startOffset = ((int)firstDay.DayOfWeek + 6) % 7;

        int row = 0;
        int col = startOffset;

        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime dateOfCell = new DateTime(currentMonth.Year, currentMonth.Month, day);
            bool isSelected = dateOfCell.Date == _viewModel.SelectedDate.Date;
            bool isToday = dateOfCell.Date == DateTime.Today;

            // 👇 GÜNCELLENDİ: Yazı rengini ayarladık (Mor üstünde beyaz yazı iyidir)
            var cellContent = new Grid();
            cellContent.Children.Add(new Label()
                .Text(day.ToString())
                .TextColor(Colors.White) // Her zaman beyaz (Seçiliyken de okunur)
                .FontSize(12)
                .FontAttributes(isSelected ? FontAttributes.Bold : FontAttributes.None)
                .HorizontalOptions(LayoutOptions.Center).VerticalOptions(LayoutOptions.Center));

            // 👇 GÜNCELLENDİ: Arkaplan rengi Mor oldu (#9747FF)
            var border = new Border()
            {
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                Stroke = Colors.Transparent,
                BackgroundColor = isSelected ? Color.FromArgb("#9747FF") : (isToday ? Color.FromArgb("#3F63FF") : Colors.Transparent),
                Content = cellContent
            };

            var tap = new TapGestureRecognizer();
            tap.Command = new Command(() =>
            {
                _viewModel.SelectedDate = dateOfCell;
            });
            border.GestureRecognizers.Add(tap);

            _calendarGrid.Add(border, col, row);

            col++;
            if (col > 6) { col = 0; row++; }
        }
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
}