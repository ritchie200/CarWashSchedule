using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFTest.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void BtnCleaningList_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.CleaningList());
        }

        private async void btnCalendar_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new Calendar());
        }

    }
}