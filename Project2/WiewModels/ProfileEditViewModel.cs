using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project2.Models;
using Project2.Services;
using System;
using System.Threading.Tasks;

namespace Project2.WiewModels
{
    public partial class ProfileEditViewModel : ObservableObject
    {
        private Person _currentUser;

        [ObservableProperty] private DateTime _birthDate = DateTime.Now;
        [ObservableProperty] private double _weight;
        [ObservableProperty] private double _height;
        [ObservableProperty] private string _selectedGender = "";
        [ObservableProperty] private string _newPassword = "";
        [ObservableProperty] private string _confirmPassword = "";

        // Görsel Kilit Kontrolleri
        [ObservableProperty] private bool _isBirthDateLocked;

        public ProfileEditViewModel()
        {
            _ = Initialize();
        }

        public async Task Initialize()
        {
            _currentUser = UserSeassion.CurrentUser;
            if (_currentUser == null) return;

            // Mevcut verileri doldur
            Height = (double)_currentUser.Height;
            Weight = (double)_currentUser.Weight;

            // Cinsiyet ve Doğum Tarihi doluysa kilitliyoruz
            SelectedGender = _currentUser.Gender == 1 ? "E" : (_currentUser.Gender == 2 ? "K" : "");

            // Not: tblPerson modelinde Doğum Tarihi tutuluyorsa buraya atanmalı. 
            // Eğer tutulmuyorsa varsayılan gelir.
           
            if(_currentUser.Age > 0)
            {
                IsBirthDateLocked = true;
            }

        }


        [RelayCommand]
        public async Task SaveChanges()
        {
            try
            {
                // Şifre Kontrolü
                if (!string.IsNullOrEmpty(NewPassword) && NewPassword != ConfirmPassword)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", "Şifreler eşleşmiyor!", "Tamam");
                    return;
                }

                // Verileri güncelle
                _currentUser.Height = Height;
                _currentUser.Weight = Weight;
                if (!string.IsNullOrEmpty(NewPassword)) _currentUser.Password = NewPassword;
               
                // Yaş hesaplama (Eğer kilitli değilse yeni tarihten hesapla)
                if (!IsBirthDateLocked)
                {
                    int age = DateTime.Now.Year - BirthDate.Year;
                    if (BirthDate > DateTime.Now.AddYears(-age)) age--;
                    _currentUser.Age = age;
                }
                PersonValidator.Validate(_currentUser);
                await App.Database.UpdatePersonAsync(_currentUser);
                await Application.Current.MainPage.DisplayAlert("Başarılı", "Profiliniz güncellendi.", "Tamam");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
    }
}