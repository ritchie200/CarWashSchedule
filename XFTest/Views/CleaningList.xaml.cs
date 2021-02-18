using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using XFTest.ViewModels;
using Prism.Services.Dialogs;
using Prism.Events;
using Prism.Ioc;
using XFTest.Events;
using XFTest.Core.Models;

namespace XFTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CleaningList : ContentPage
    {

        IEventAggregator _ea;
        private App _app => (App)Xamarin.Forms.Application.Current;
        Animation animation;
        CleaningListViewModel viewModel;

        public CleaningList()
        {
            InitializeComponent();
            //Get View Model object from Binding Context
            viewModel = (CleaningListViewModel)BindingContext;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            var state = width < 280 ? "Small" : width < 360 ? "Medium" : "Large";
            VisualStateManager.GoToState(PageHeading, state);
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                _ea = _app.Container.Resolve<IEventAggregator>();

                //Subscribe Show/Hide Calender
                _ea.GetEvent<CalenderEvent>().Subscribe(ShowHideCalender);

                //Subscribe Hide Calender From Page Tap
                _ea.GetEvent<HideCalenderEvent>().Subscribe(HideOpenCalender);

                //Used for Infinite Scroll , if list is empty check again for Visits data from ApiService
                if (viewModel.listVisits.Count == 0)
                    viewModel.LoadItemsCommand.Execute(null);

            }
            catch (Exception)
            {

            }
        }

        protected override void OnDisappearing()
        {
            try
            {
                //UnSubscribe Show/Hide Calender
                _ea.GetEvent<CalenderEvent>().Unsubscribe(ShowHideCalender);
            }
            catch (Exception)
            {

            }
        }

        //Called from Page Tap only
        private async void HideOpenCalender()
        {
            if (stackLayout.Height > 0)
            {
                Action<double> callback = (input) => {
                    stackLayout.HeightRequest = input;
                };
                double startingHeight = 200;
                double endingHeight = 0;
                uint rate = 16;
                uint length = 400;
                Easing easing = Easing.CubicIn;
                stackLayout.Animate("inviss", callback, startingHeight, endingHeight, rate, length, easing);
                await System.Threading.Tasks.Task.Delay(200);
                gridHeader.IsVisible = true;
            }

        }

        //Animate Show/Hide Calender
        private async void ShowHideCalender()
        {
            if (stackLayout.Height == 0)
            {
                 Action<double> callback =  (input) => {
                     stackLayout.HeightRequest = input;
                 };
                double startingHeight = 0;
                double endingHeight = 200;
                uint rate = 16;
                uint length = 50;
                Easing easing = Easing.CubicIn;
                stackLayout.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing);
                gridHeader.IsVisible = false;
            }
            else
            {
                Action<double> callback = (input) => {
                    stackLayout.HeightRequest = input;
                };
                double startingHeight = 200;
                double endingHeight = 0;
                uint rate = 16;
                uint length = 50;
                Easing easing = Easing.CubicIn;
                stackLayout.Animate("inviss", callback, startingHeight, endingHeight, rate, length, easing);
                //await System.Threading.Tasks.Task.Delay(200);
                gridHeader.IsVisible = true;
            }
        }

    }
}