using FmgLib.MauiMarkup;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Project2.WiewModels;

namespace Project2.Pages;

public class ProfileEditPage : ContentPage
{
    // Bunlar normalde veri tabanından gelecek değerler.
    private string _existingBirthDate; 
    private string _existingGender;           
    private string _existingHeight;        
    private string _existingWeigh;

    public ProfileEditPage()
    {
        var vm = new ProfileEditViewModel();
        this.BindingContext = vm;
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
                                // Doğum Tarihi: Eğer veri varsa kilitli bir input, yoksa DatePicker gösterir
                                vm.IsBirthDateLocked
                                    ? CreateInputGroup("Doğum Tarihi", "", "", false, bindingPath: nameof(vm.BirthDate))
                                    : CreateDatePickerGroup("Doğum Tarihi", "BirthDate"),

                                // Boy ve Kilo: Her zaman güncellenebilir
                                CreateInputGroup("Boy (cm)", "Örn: 185", "", true, Keyboard.Numeric, bindingPath: nameof(vm.Height)),
                                CreateInputGroup("Kilo (kg)", "Örn: 75.5", "", true, Keyboard.Numeric, bindingPath: nameof(vm.Weight)),

                                // Şifreler: ViewModel'deki NewPassword ve ConfirmPassword alanlarına bağlı
                                CreateInputGroup("Yeni Şifre", "********", "", true, Keyboard.Default, true, bindingPath: nameof(vm.NewPassword)),
                                CreateInputGroup("Yeni Şifre Tekrar", "********", "", true, Keyboard.Default, true, bindingPath: nameof(vm.ConfirmPassword)),
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
                                    .Bind(Button.CommandProperty, nameof(vm.SaveChangesCommand)) 
                                }
                              }
                            }.Row(1),
                          }
                        };
                    }

                    private View CreateInputGroup(string labelText, string placeholder, string initialValue, bool isEnabled, Keyboard? keyboard = null, bool isPassword = false, string bindingPath = null)
                    {
                        return new VerticalStackLayout()
                        {
                            Spacing = 8,
                            Opacity = isEnabled ? 1.0 : 0.6, // Kilitli olanlar biraz daha soluk görünür
                            Children = {
                                                        new Label().Text(labelText).TextColor(isEnabled ? Colors.White : Color.FromArgb("#80FFFFFF")).FontSize(14).Margin(5, 0, 0, 0),
                                                        new Border() {
                                                            Stroke = isEnabled ? Colors.White : Color.FromArgb("#44FFFFFF"),
                                                            StrokeThickness = 1, Padding = new Thickness(10, 0),
                                                            BackgroundColor = isEnabled ? Colors.Transparent : Color.FromArgb("#1AFFFFFF"),
                                                            Content = new Entry().Placeholder(placeholder).PlaceholderColor(Colors.Gray).TextColor(Colors.White)
                                                                .IsEnabled(isEnabled).IsPassword(isPassword).Keyboard(keyboard ?? Keyboard.Default).BackgroundColor(Colors.Transparent)
                                                                .Bind(Entry.TextProperty, bindingPath) // Veri bağlama
                                                        }
                                                    }
                        };
                    }

                    private View CreateDatePickerGroup(string labelText, string bindingPath)
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
                                    .Format("dd/MM/yyyy")
                                    .BackgroundColor(Colors.Transparent)
                                    // Buraya dikkat: DateProperty'i verdiğin yola bağlar
                                    .Bind(DatePicker.DateProperty, bindingPath)
                            }
                        }
                        };
                    }
}
