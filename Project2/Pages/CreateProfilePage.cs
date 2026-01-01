using Microsoft.Maui;
using Microsoft.Maui.Controls;
using FmgLib.MauiMarkup;
using Project2.WiewModels;


namespace Project2.Pages;

public class CreateProfilePage : ContentPage // Profil oluşturma ekranı ContentPage den türeyecek çünkü kendisi bir ekran.
{
    public CreateProfilePage() // Sayfa oluşturulduğunda çalışan constructor’dır. UI elemanları burada oluşturulur.
    {
        BindingContext = new CreateProfilePageWiew();
        this.BackgroundColor(Color.FromArgb("#23222E")); // Arka plan rengi.

        Content = new Grid() // Tüm ekranı bir ızgara gibi bölerek UI elemanlarını istediğimiz yerlere yerleştirmemizi sağlar.
        {
            Padding = new Thickness(30, 60, 30, 30), // Her yönden (sol,üst,sağ,alt) 30 px boşluk bırakır.
            Children = // Gridin içine VerticalStackLayout (dikey dizilim kutusu) yerleştirmek için kullanılır.
            {
                new VerticalStackLayout() // İçine yazılan tüm elemanları üstten alta doğru dizer.
                {
                    Spacing = 15, // Alt alta dizilen elemanların arasına 15 px mesafe koyar.
                    VerticalOptions = LayoutOptions.Center, // Tüm bu dikey listeyi ekranın dikey olarak tam ortasına yerleştirir.
                    Children = // VerticalStackLayout un içine UI elemanlarını (label,buton,border) eklemek için kullanılır.
                    {
                        // PROFİL OLUŞTUR BAŞLIK
                        new Label() // Etiket
                            .Text("PROFİL OLUŞTUR") // PROFİL OLUŞTUR yazısı.
                            .TextColor(Colors.White) // Yazı beyaz renk.
                            .FontSize(30) // Yazı fontu büyük.
                            .FontAttributes(FontAttributes.Bold) // Yazıyı kalın yapar.
                            .CenterHorizontal() // Yazıyı yatayda ortalar.
                            .Margin(new Thickness(0, 0, 0, 20)), // Yazının altına 20 px extra boşluk ekler.

                        new VerticalStackLayout()
                        {
                            Spacing = 5,
                            Children =
                            {
                                new Label()
                                    .Text("Doğum Tarihi")
                                    .TextColor(Colors.White)
                                    .FontSize(14),

                                new Border()
                                    .Stroke(Colors.White)
                                    .StrokeThickness(1)
                                    .Padding(new Thickness(10, 0))
                                    .Content(
                                        new DatePicker()
                                            .TextColor(Colors.White)
                                            .HeightRequest(45)
                                            .MinimumDate(new DateTime(1950, 1, 1))
                                            .MaximumDate(DateTime.Now)
                                    )
                            }
                        },

                        // BOY (Entry)
                        CreateInputGroup("Boy (cm)"), // Boy için Label,Border,Entry içeren elemanları oluşturur.
                        
                        // KİLO (Entry)
                        CreateInputGroup("Kilo (kg)"), // Kilo için Label,Border,Entry içeren elemanları oluşturur.

                        // CİNSİYET BAŞLIĞI
                        new Label() // Etiket
                            .Text("Cinsiyet") // Cinsiyet yazısı.
                            .TextColor(Colors.White) // Yazı beyaz renk.
                            .FontSize(14) // Yazı fontu küçük.
                            .Margin(new Thickness(0,10,0,0)), // Yazının üstüne 10 px boşluk ekler.

                        // CİNSİYET BUTONLARI (Yan yana)
                        new Grid() // Grid, içine koyulan elemanları satır ve sütunlara ayırarak yerleştirmemizi sağlayan bir düzendir.
                                   // Butonları yan yana koymak için kullandık.
                        {
                            ColumnDefinitions = { // Gridi kaç sutüna böleceğimizi ve genişliğini ColumnDefinitions ile belirleriz.
                                new ColumnDefinition(GridLength.Star), // Star, kalan alanı paylaş demektir.
                                new ColumnDefinition(GridLength.Star)  // 2 sütün olduğu için 2 eşit parça olur (%50, %50).
                            },

                            ColumnSpacing = 20, // İki sütun arasına 20 piksel boşluk koyar
                            Children =  // Gridin içine UI elemanlarını (label,buton,border) eklemek için kullanılır.
                            {
                                new Button() // Buton
                                    .Text("Kadın") // Buton üzerinde Kadın yazısı.
                                    .TextColor(Colors.White) // Yazı beyaz renk.
                                    .BackgroundColor(Colors.Transparent) // Buton arka planı şeffaf.
                                    .BorderColor(Colors.White) // Butonun çerçevesi beyaz.
                                    .BorderWidth(1) // Çerçeve kalınlığı 1 px.
                                    .Column(0), // Butonu Gridin 0. sutünuna yerleştirir.(sola)

                                new Button() // Buton
                                    .Text("Erkek") // Buton üzerinde Erkek yazısı.
                                    .TextColor(Colors.White) // Yazı beyaz renk.
                                    .BackgroundColor(Colors.Transparent) // Buton arka planı şeffaf.
                                    .BorderColor(Colors.White) // Butonun çerçevesi beyaz.
                                    .BorderWidth(1) // Çerçeve kalınlığı 1 px.
                                    .Column(1) // Butonu Gridin 1. sutünuna yerleştirir.(sağa)
                            }
                        },

                        // ALT BUTONLAR (Şimdilik Atla - Devam Et)
                        new Grid()
                        {
                            ColumnDefinitions = { // Gridi kaç sutüna böleceğimizi ve genişliğini ColumnDefinitions ile belirleriz.
                                new ColumnDefinition(GridLength.Star), // Star, kalan alanı paylaş demektir.
                                new ColumnDefinition(GridLength.Star) // 2 sütün olduğu için 2 eşit parça olur (%50, %50).
                            },

                            ColumnSpacing = 20, // İki sütun arasına 20 piksel boşluk koyar
                            Margin = new Thickness(0, 30, 0, 0), // Yazının üstüne 30 px boşluk ekler.
                            Children =
                            {
                                new Button() // Buton
                                    .Text("Şimdilik Atla") // Butona Şimdilik Atla yazısı.
                                    .TextColor(Colors.White) // Yazı beyaz renk.
                                    .BackgroundColor(Colors.Gray) // Buton arka planı gri.
                                    .HeightRequest(50) // Buton yüksekliği 50 px.
                                    .BorderColor(Colors.White) // Beyaz kenarlık
                                    .BorderWidth(1) // İnce kenarlık
                                    .Column(0) // Butonu Gridin 0. sutünuna yerleştirir.(sola)
                                    .GestureRecognizers(new TapGestureRecognizer() // Label’ı buton gibi tıklanabilir yapar.
                                    {
                                    Command = new Command(async () => await Navigation.PushAsync(new MainDashboardPage()))
                                    }),


                                new Button() // Buton
                                    .Text("Devam Et") // Butona Devam Et yazısı.
                                    .TextColor(Colors.White) // Yazı beyaz renk.
                                    .BackgroundColor(Color.FromArgb("#1A00B0")) // Buton arka planı yeşil renk.
                                    .HeightRequest(50) // Buton yüksekliği 50 px.
                                    .BorderColor(Colors.White) // Beyaz kenarlık
                                    .BorderWidth(1) // İnce kenarlık
                                    .Column(1) // Butonu Gridin 1. sutünuna yerleştirir.(sağa)
                                    .GestureRecognizers(new TapGestureRecognizer() // Label’ı buton gibi tıklanabilir yapar.
                                    {
                                    Command = new Command(async () => await Navigation.PushAsync(new MainDashboardPage()))
                                    }),

                            }
                        }
                    }
                }
            }
        };
    }

    // Ortak Input Grubu Metodu
    private View CreateInputGroup(string title) // Kod tekrarını azaltan metot.
    {
        return new VerticalStackLayout() // Dikey Yerleşim. İçindeki elemanları üstten alta dizer.
        {
            Spacing = 5, // VerticalStackLayout içine alt alta dizilen elemanların (Label, Entry, Button) arasına 5 px mesafe koyar.
            Children = // VerticalStackLayout un içine UI elemanlarını (label,buton,border) eklemek için kullanılır.
            {
                new Label() // Etiket
                    .Text(title) // Metoda girilen ilk parametreyi ekrana yazar.
                    .TextColor(Colors.White) // Yazı rengi beyaz.
                    .FontSize(14), // Yazı fontu küçük.

                new Border() // Çerçeve
                    .Stroke(Colors.White) // Çerçeve rengi beyaz.
                    .StrokeThickness(1) // Çerçeve kalınlığı 1
                    .Padding(new Thickness(10, 0)) // Çerçevenin içine sağdan ve soldan 10 px boşluk ekler.
                    .Content( // Çerçevenin içine eleman eklemek için
                        new Entry() // Kullanıcıdan giriş almak için çerçeve içine koyulan eleman.
                            .TextColor(Colors.White) // Yazı beyaz renk.
                            .BackgroundColor(Colors.Transparent) // Entry arka planı şeffaf.
                            .HeightRequest(45) // Entry yüksekliği 40 px.
                            .Keyboard(Keyboard.Numeric) // Boy/Kilo için sayısal klavye açar.
                    )
            }
        };
    }
}

