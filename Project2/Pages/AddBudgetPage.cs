using Microsoft.Maui;
using Microsoft.Maui.Controls;
using FmgLib.MauiMarkup;

namespace MyAppMAUI.Pages;

public class AddBudgetPage : ContentPage
{

    private Label _expenseInputTitleLabel;
    private VerticalStackLayout _expenseSection;
    private VerticalStackLayout _incomeSection;

    public AddBudgetPage()
    {
        this.BackgroundColor(Color.FromArgb("#23222E"));

        Content = new Grid()
        {
            Padding = new Thickness(20, 40, 20, 20),
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star),
            },
            Children =
            {
                // FORM İÇERİĞİ
                new VerticalStackLayout
                {
                    Spacing = 18,
                    Padding = new Thickness(10, 0, 10, 0),
                    Children =
                    {
                        // SAYFA BAŞLIĞI
                        new Label()
                            .Text("Gelir / Gider Ekle")
                            .TextColor(Colors.White)
                            .FontSize(24)
                            .FontAttributes(FontAttributes.Bold)
                            .Margin(new Thickness(0, 0, 0, 10)),

                        // İŞLEM TİPİ SEÇİMİ (Gelir / Gider)
                        new VerticalStackLayout()
                        {
                            Spacing = 8,
                            Children = {
                                new Label()
                                    .Text("İşlem Tipi")
                                    .TextColor(Colors.White)
                                    .FontSize(16),
                                new HorizontalStackLayout()
                                {
                                    Spacing = 25,
                                    Children = {
                                        new RadioButton()
                                        {
                                            Content = "Gelir"
                                        }
                                            .TextColor(Colors.White)
                                            .BackgroundColor(Colors.Transparent)
                                            .BorderColor(Colors.Transparent)
                                            .OnCheckedChanged(OnTransactionTypeChanged),
                                        new RadioButton()
                                        {
                                            Content = "Gider",
                                            IsChecked = true
                                        }
                                            .TextColor(Colors.White)
                                            .BackgroundColor(Colors.Transparent)
                                            .BorderColor(Colors.Transparent)
                                            .OnCheckedChanged(OnTransactionTypeChanged)
                                    }
                                }
                            }
                        },

                        new VerticalStackLayout
                            {
                                Spacing = 18,
                                IsVisible = true
                            }
                            .Assign(out _expenseSection)
                            .Children(
                                new Label()
                                    .Text("Gider Bilgileri")
                                    .TextColor(Colors.White)
                                    .FontSize(16)
                                    .FontAttributes(FontAttributes.Bold),

                                    CreateExpenseCategories(),
                                    CreateExpenseDescription(),
                                    CreateAmountInput(),
                                    CreateExpenseDate()

                            ),

                        new VerticalStackLayout
                            {
                                Spacing = 18,
                                IsVisible = false
                            }
                            .Assign(out _incomeSection)
                            .Children(
                                new Label()
                                    .Text("Gelir Bilgileri")
                                    .TextColor(Colors.White)
                                    .FontSize(16)
                                    .FontAttributes(FontAttributes.Bold),

                                    CreateIncomeCategories(),
                                    CreateSimpleDescription(),
                                    CreateAmountInput(),
                                    CreateIncomeDate(),
                                    CreateRecurringSwitch()
                            ),
                        
                        // EKLE BUTONU
                        new Button()
                            .Text("Ekle")
                            .BackgroundColor(Color.FromArgb("#00FF85"))
                            .TextColor(Colors.White)
                            .FontAttributes(FontAttributes.Bold)
                            .HeightRequest(40)
                            .WidthRequest(120)
                            .BorderColor(Colors.White)
                            .BorderWidth(1)
                            .CenterHorizontal()
                            .Margin(new Thickness(0, 40, 0, 0)),

                    }

                }.Row(0),
            }
        };

    }
    private View CreateExpenseCategories()
    {
        return new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label().Text("Kategoriler").TextColor(Colors.White),

                new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(), new ColumnDefinition(),
                        new ColumnDefinition(), new ColumnDefinition()
                    },
                    Children =
                    {
                        CreateExpenseCategory("Market", true, 0),
                        CreateExpenseCategory("Kira", false, 1),
                        CreateExpenseCategory("Fatura", false, 2),
                        CreateExpenseCategory("Diğer", false, 3)
                    }
                }
            }
        };
    }
    private View CreateExpenseCategory(string text, bool isChecked, int col)
    {
        return new RadioButton
        {
            Content = text,
            IsChecked = isChecked,
            TextColor = Colors.White,
            BackgroundColor = Colors.Transparent,
            FontSize = 12
        }
        .OnCheckedChanged(OnExpenseCategoryChanged)
        .Column(col);
    }
    private View CreateExpenseDescription()
    {
        return new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label()
                    .Text("Açıklama")
                    .TextColor(Colors.White)
                    .Assign(out _expenseInputTitleLabel),

                CreateEntry()
            }
        };
    }
    private View CreateExpenseDate()
    {
        return CreateDatePicker("Son Ödeme Tarihi");
    }

    private View CreateIncomeCategories()
    {
        return new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label().Text("Kategoriler").TextColor(Colors.White),

                new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(), new ColumnDefinition(),
                        new ColumnDefinition(), new ColumnDefinition()
                    },
                    Children =
                    {
                        CreateIncomeCategory("Maaş", true, 0),
                        CreateIncomeCategory("Kira", false, 1),
                        CreateIncomeCategory("Freelance", false,2),
                        CreateIncomeCategory("Diğer", false, 3)
                    }
                }
            }
        };
    }
    private View CreateIncomeCategory(string text, bool isChecked, int col)
    {
        return new RadioButton
        {
            Content = text,
            IsChecked = isChecked,
            TextColor = Colors.White,
            BackgroundColor = Colors.Transparent,
            FontSize = 12
        }.Column(col);
    }
    private View CreateIncomeDate()
    {
        return CreateDatePicker("Gelir Tarihi");
    }
    private View CreateRecurringSwitch()
    {
        return new HorizontalStackLayout
        {
            Spacing = 12,
            Children =
            {
                new Switch().OnColor(Color.FromArgb("#00FF85")),
                new Label()
                    .Text("Her ay otomatik tekrarlansın")
                    .TextColor(Colors.White)
                    .CenterVertical()
            }
        };
    }

    private View CreateAmountInput()
    {
        return new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label().Text("Tutar").TextColor(Colors.White),

                new Border
                {
                    Stroke = Colors.White,
                    StrokeThickness = 1,
                    Padding = new Thickness(10, 0),
                    Content = new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition(GridLength.Auto),
                            new ColumnDefinition(GridLength.Star)
                        },
                        Children =
                        {
                            new Label().Text("₺").TextColor(Colors.White).CenterVertical(),
                            new Entry()
                                .Keyboard(Keyboard.Numeric)
                                .TextColor(Colors.White)
                                .BackgroundColor(Colors.Transparent)
                                .Column(1)
                        }
                    }
                }
            }
        };
    }
    private View CreateSimpleDescription()
    {
        return new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label().Text("Açıklama").TextColor(Colors.White),
                CreateEntry()
            }
        };
    }
    private View CreateEntry()
    {
        return new Border
        {
            Stroke = Colors.White,
            StrokeThickness = 1,
            Padding = new Thickness(10, 0),
            Content = new Entry
            {
                TextColor = Colors.White,
                BackgroundColor = Colors.Transparent,
                HeightRequest = 45
            }
        };
    }
    private View CreateDatePicker(string title)
    {
        return new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label().Text(title).TextColor(Colors.White),
                new Border
                {
                    Stroke = Colors.White,
                    StrokeThickness = 1,
                    Padding = new Thickness(10, 0),
                    Content = new DatePicker
                    {
                        TextColor = Colors.White
                    }
                }
            }
        };
    }
    private void OnTransactionTypeChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value || sender is not RadioButton rb) return;

        bool isIncome = rb.Content.ToString() == "Gelir";

        _incomeSection.IsVisible = isIncome;
        _expenseSection.IsVisible = !isIncome;
    }
    private void OnExpenseCategoryChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value || sender is not RadioButton rb) return;

        _expenseInputTitleLabel.Text =
            rb.Content.ToString() == "Fatura" ? "Kurum Adı" : "Açıklama";
    }
}