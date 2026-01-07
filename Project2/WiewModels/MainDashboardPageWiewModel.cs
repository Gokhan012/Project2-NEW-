using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project2.Pages;
namespace Project2.WiewModels;


partial class MainDashboardPageWiewModel : ObservableObject
{
    [RelayCommand]
    public async Task GotoBillPage() { 
    if(Application.Current.MainPage is not null)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new Project2.Pages.AddBillPage());
        }
}

    [RelayCommand]
    public async Task GotoMainPage()
    {
        if (Application.Current.MainPage is not null)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Project2.Pages.MainDashboardPage());
        }
    }

    [RelayCommand]
    public async Task GotoMedicinePage()
    {
        if (Application.Current.MainPage is not null)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Project2.Pages.AddMedicinePage());
        }
    }

    [RelayCommand]
    public async Task GotoHealthPage()
    {
        if (Application.Current.MainPage is not null)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Project2.Pages.HealthPage());
        }
    }

    [RelayCommand]
    public async Task GotoCalendarPage()
    {
        if (Application.Current.MainPage is not null)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Project2.Pages.FULLCalendarPage());
        }
    }

    [RelayCommand]
    public async Task GotoBudgetPage()
    {
        if (Application.Current.MainPage is not null)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Project2.Pages.BudgetPage());
        }
    }


    [RelayCommand]
    public async Task GotoActivityPage()
    {
        if (Application.Current.MainPage is not null)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Project2.Pages.AddActivityEventPage());
        }
    }

    [RelayCommand]
    public async Task GotoAddBudgetPage()
    {
        if (Application.Current.MainPage is not null)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Project2.Pages.AddBudgetPage());
        }
    }

}