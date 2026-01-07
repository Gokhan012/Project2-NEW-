using Microsoft.Maui;
using Microsoft.Maui.Controls;
using FmgLib.MauiMarkup;
using Project2.ViewModels; // ViewModel namespace'i

namespace Project2.Pages;

public class AddBillPage : ContentPage
{
    public AddBillPage()
    {
        // 1. ViewModel Bağlantısı
        var vm = new AddBillPageViewModel();
        BindingContext = vm;

        this.BackgroundColor(Color.FromArgb("#23222E"));

        Content = new Grid()
        {
            Padding = new Thickness(20, 40, 20, 20),
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star), // 0: Form içeriği (Kalan alanı kaplar)
                new RowDefinition(GridLength.Auto)  // 1: Alt Navigasyon (İçeriği kadar)
            },
            Children =
            {
                // FORM İÇERİĞİ
                new VerticalStackLayout()
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

                        // İŞLEM TİPİ SEÇİMİ (Gelir / Gider) DEĞİŞTİRİLEMEZ
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
                                    InputTransparent = true,
                                    Children = {
                                        new RadioButton()
                                        {
                                            Content = "Gelir"
                                        }
                                            .TextColor(Colors.White)
                                            .BackgroundColor(Colors.Transparent)
                                            .BorderColor(Colors.Transparent),
                                        new RadioButton()
                                        {
                                            Content = "Gider",
                                            IsChecked = true
                                        }
                                            .TextColor(Colors.White)
                                            .BackgroundColor(Colors.Transparent)
                                            .BorderColor(Colors.Transparent)
                                    }
                                }
                            }
                        },

                        // KATEGORİLER (Market, Kira, Fatura, Diğer) DEĞİŞTİRİLEMEZ
                        new VerticalStackLayout()
                        {
                            Spacing = 8,
                            Children = {
                                new Label()
                                    .Text("Kategoriler")
                                    .TextColor(Colors.White)
                                    .FontSize(16),
                                new Grid()
                                {
                                    ColumnDefinitions = {
                                        new ColumnDefinition(GridLength.Auto),
                                        new ColumnDefinition(GridLength.Auto),
                                        new ColumnDefinition(GridLength.Auto),
                                        new ColumnDefinition(GridLength.Auto)
                                    },
                                    ColumnSpacing = 10,
                                    InputTransparent = true,
                                    Children = {
                                        CreateCategoryRadioButton("Market", false, 0),
                                        CreateCategoryRadioButton("Kira", false, 1),
                                        CreateCategoryRadioButton("Fatura", true, 2),
                                        CreateCategoryRadioButton("Diğer", false, 3)
                                    }
                                }
                            }
                        },


                        // KURUM ADI (Title'a bağlandı)
                        CreateInputGroup("Kurum Adı", nameof(AddBillPageViewModel.Title)),
                        
                        // TUTAR ALANI (Amount'a bağlandı)
                        CreateAmountInput(nameof(AddBillPageViewModel.Amount)),

                        // SON ÖDEME TARİHİ (DueDate'e bağlandı)
                        CreateDateInput(nameof(AddBillPageViewModel.DueDate)),

                        // EKLE BUTONU (SaveBillCommand'e bağlandı)
                        new Button()
                            .Text("Kaydet")
                            .BackgroundColor(Color.FromArgb("#00FF85"))
                            .TextColor(Colors.White)
                            .FontAttributes(FontAttributes.Bold)
                            .HeightRequest(40)
                            .WidthRequest(120)
                            .BorderColor(Colors.White)
                            .BorderWidth(1)
                            .CenterHorizontal()
                            .Margin(0, 40, 0, 0)
                            .Bind(Button.CommandProperty, nameof(AddBillPageViewModel.SaveBillCommand))
                    }
                }.Row(0),
            }
        };
    }

    // --- YARDIMCI METOTLAR ---

    // Kategori Seçici (ViewModel'e bağlanır)
    private View CreateCategoryRadioButton(string text, bool isChecked, int col)
    {
        return new RadioButton
        {
            Content = text,
            IsChecked = isChecked,
            TextColor = Colors.White,
            BackgroundColor = Colors.Transparent,
            GroupName = "BillCategoryGroup"
        }
        .Column(col)
        .OnCheckedChanged((sender, e) =>
        {
            if (e.Value && BindingContext is AddBillPageViewModel vm)
            {
                vm.SetCategoryCommand.Execute(text);
            }
        });
    }

    // Metin Giriş Grubu (Binding Ekli)
    private View CreateInputGroup(string title, string bindingPath)
    {
        return new VerticalStackLayout()
        {
            Spacing = 8,
            Children = {
                new Label().Text(title).TextColor(Colors.White).FontSize(14),
                new Border()
                    .Stroke(Colors.White).StrokeThickness(1).BackgroundColor(Colors.Transparent).Padding(new Thickness(10, 0))
                    .Content(
                        new Entry().TextColor(Colors.White).BackgroundColor(Colors.Transparent).HeightRequest(45)
                        .Bind(Entry.TextProperty, bindingPath) // BAĞLANTI
                    )
            }
        };
    }

    // Tutar Giriş Alanı (Binding Ekli)
    private View CreateAmountInput(string bindingPath)
    {
        return new VerticalStackLayout()
        {
            Spacing = 8,
            Children = {
                new Label().Text("Tutar").TextColor(Colors.White).FontSize(14),
                new Border()
                    .Stroke(Colors.White).StrokeThickness(1).Padding(new Thickness(10, 0))
                    .Content(
                        new Grid()
                        {
                            ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) },
                            Children = {
                                new Label().Text("₺").TextColor(Colors.White).CenterVertical(),
                                new Entry().Keyboard(Keyboard.Numeric).TextColor(Colors.White).BackgroundColor(Colors.Transparent).HeightRequest(45).Column(1)
                                .Bind(Entry.TextProperty, bindingPath) // BAĞLANTI
                            }
                        }
                    )
            }
        };
    }

    // Tarih Giriş Alanı (Binding Ekli)
    private View CreateDateInput(string bindingPath)
    {
        return new VerticalStackLayout()
        {
            Spacing = 8,
            Children = {
                new Label().Text("Son Ödeme Tarihi").TextColor(Colors.White).FontSize(14),
                new Border()
                    .Stroke(Colors.White).StrokeThickness(1).HeightRequest(45).Padding(new Thickness(10, 0))
                    .Content(
                        new Grid()
                        {
                            Children = {
                                new DatePicker().Format("dd.MM.yyyy").TextColor(Colors.White).BackgroundColor(Colors.Transparent).CenterVertical()
                                .Bind(DatePicker.DateProperty, bindingPath) // BAĞLANTI
                            }
                        }
                    )
            }
        };
    }
}