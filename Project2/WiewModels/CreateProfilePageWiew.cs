using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project2.Services;

namespace Project2.WiewModels
{
    partial class CreateProfilePageWiew : ObservableObject
    {
        [ObservableProperty]
        private int age;
        [ObservableProperty]
        private double weight;
        [ObservableProperty]
        private double height;
        [ObservableProperty]
        private int gender;
        [RelayCommand]
        public async Task SelectFemale()
        {
            Gender = 1;
        }

        [RelayCommand]
        public async Task SelectMale()
        { 
            Gender = 0;
        }

        [RelayCommand]
        public async Task Profile()
        {
            var currentuser = UserSeassion.CurrentUser;

            if (currentuser == null)
            {
                // ... Hata mesajı aynı kalsın ...
                return;
            }

            // -----------------------------------------------------------
            // DÜZELTME 1: Eşitleme işlemini EN BAŞA almalısın.
            // Yoksa validator "Eski veriler hatalı" deyip işlemi iptal eder.
            // -----------------------------------------------------------
            currentuser.Age = Age;
            currentuser.Height = Height;
            currentuser.Weight = Weight; // Entry bağlıysa burası dolu gelir
            currentuser.Gender = Gender;

            try
            {
                // DÜZELTME 2: Validate işlemi atamadan SONRA yapılmalı
                PersonValidator.Validate(currentuser);

                await App.Database.UpdatePersonAsync(currentuser);

                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Başarılı", "Profil oluşturuldu!", "Tamam");

                    // DÜZELTME 3: Sayfa yönlendirmesini BURADA yapmalısın
                    // (Butonun içinde değil, kayıt başarılı olunca)
                    await Application.Current.MainPage.Navigation.PushAsync(new MainDashboardPage());
                }
            }
            catch (ArgumentException ex)
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Uyarı", ex.Message, "Tamam");
            }
            catch (Exception ex)
            {
                // ... Genel hata ...
            }
        }
    } }