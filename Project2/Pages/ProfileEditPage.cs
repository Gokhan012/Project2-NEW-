using FmgLib.MauiMarkup;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Project2.Pages;

public class ProfileEditPage : ContentPage
{
    // Bunlar normalde veri tabanından gelecek değerler.
    private string _existingBirthDate = "15.05.1998"; // Örnek: DOLU VERİ DÜZENLENEMEZ
    private string _existingGender = "";           // Örnek: Boş veri (düzenlenebilir)
    private string _existingHeight = "180";        // Örnek: DOLU VERİ DÜZENLENEBİLİR
    private string _existingWeight = "75";

    public ProfileEditPage()
    {
        this.BackgroundColor(Color.FromArgb("#23222E"));

        // Veri var mı kontrolü (Mantık: Değişken boş değilse kilitli olacak)
        // Sadece doğum günü ve cinsiyet var ise kilitli olacak.
        bool isBirthDateLocked = !string.IsNullOrEmpty(_existingBirthDate);
        bool isGenderLocked = !string.IsNullOrEmpty(_existingGender);

        Content = new Grid()
        {
            Padding = new Thickness(20, 40, 20, 20),
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto), // 0: Başlık
                new RowDefinition(GridLength.Star), // 1: Form Alanları
            },
            Children =
            {
                // Üst Başlık
                new Grid()
                {
                    ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) },
                    Children =
                    {
                        new Label()
                            .Text("Profili Düzenle")
                            .TextColor(Colors.White)
                            .FontSize(24)
                            .FontAttributes(FontAttributes.Bold)
                            .Column(1)
                    }
                }.Row(0).Margin(0, 0, 0, 30),

                // Form Alanları
                new ScrollView()
                {
                    Content = new VerticalStackLayout()
                    {
                        Spacing = 20,
                        Children =
                        {
                            // Doğum Tarihi ve Cinsiyet: Önceden girildiyse kilitli (IsEnabled false)
                            // Doğum tarihi eğer kilitli ise (veri varsa) eski CreateInputGroup, yoksa yeni Takvim metodu çalışır
                            isBirthDateLocked
                                ? CreateInputGroup("Doğum Tarihi", "", _existingBirthDate, false)
                                : CreateDatePickerGroup("Doğum Tarihi", true),

                            CreateInputGroup("Cinsiyet", "Örn: Erkek / Kadın", _existingGender, !isGenderLocked),
                            
                            // Boy ve Kilo: Her zaman güncellenebilir (IsEnabled true)
                            CreateInputGroup("Boy (cm)", "Örn: 185", _existingHeight, true, Keyboard.Numeric),
                            CreateInputGroup("Kilo (kg)", "Örn: 75.5", _existingWeight, true, Keyboard.Numeric),
                            
                            // Şifre: Güncellenebilir. ŞİFRE EŞLEŞİYOR MU? KONTROL LAZIM.
                            CreateInputGroup("Yeni Şifre", "********", "", true, Keyboard.Default, true),
                            CreateInputGroup("Yeni Şifre Tekrar", "********", "", true, Keyboard.Default, true),

                            new Button()
                                .Text("Değişiklikleri Kaydet")
                                .BackgroundColor(Color.FromArgb("#00FF85"))
                                .TextColor(Colors.White)
                                .FontAttributes(FontAttributes.Bold)
                                .CornerRadius(12)
                                .HeightRequest(45)
                                .WidthRequest(200)
                                .HorizontalOptions(LayoutOptions.Center)
                                .Margin(0, 30, 0, 20)
                        }
                    }
                }.Row(1),
            }
        };
    }

    private View CreateInputGroup(string labelText, string placeholder, string initialValue, bool isEnabled, Keyboard? keyboard = null, bool isPassword = false)
    {
        return new VerticalStackLayout()
        {
            Spacing = 8,
            Opacity = isEnabled ? 1.0 : 0.6, // Kilitli olanlar biraz daha soluk görünür
            Children =
            {
                new Label()
                    .Text(labelText)
                    .TextColor(isEnabled ? Colors.White : Color.FromArgb("#80FFFFFF")) // Kilitliyse yazı rengi değişir
                    .FontSize(14)
                    .Margin(5, 0, 0, 0),

                new Border()
                {
                    Stroke = isEnabled ? Colors.White : Color.FromArgb("#44FFFFFF"), // Kilitliyse kenarlık rengi değişir
                    StrokeThickness = 1,
                    Padding = new Thickness(10, 0),
                    BackgroundColor = isEnabled ? Colors.Transparent : Color.FromArgb("#1AFFFFFF"), // Kilitliyse hafif gri dolgu
                    Content = new Entry()
                        .Text(initialValue)
                        .Placeholder(placeholder)
                        .PlaceholderColor(Colors.Gray)
                        .TextColor(Colors.White)
                        .IsEnabled(isEnabled) // Kilit mekanizması
                        .IsPassword(isPassword) // Şifre gizleme
                        .Keyboard(keyboard ?? Keyboard.Default)
                        .BackgroundColor(Colors.Transparent)
                }
            }
        };
    }

    private View CreateDatePickerGroup(string labelText, bool isEnabled)
    {
        return new VerticalStackLayout()
        {
            Spacing = 8,
            Children =
        {
            new Label().Text(labelText).TextColor(Colors.White).FontSize(14).Margin(5, 0, 0, 0),
            new Border()
            {
                Stroke = Colors.White,
                StrokeThickness = 1,
                Padding = new Thickness(10, 0),
                Content = new DatePicker()
                    .TextColor(Colors.White)
                    .MinimumDate(new DateTime(1950, 1, 1))
                    .MaximumDate(DateTime.Now)
                    .BackgroundColor(Colors.Transparent)
            }
        }
        };
    }
}
