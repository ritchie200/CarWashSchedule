using Prism;
using Prism.Ioc;
using XFTest.ViewModels;
using XFTest.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Events;
using XFTest.Core.Interfaces;
using XFTest.Database.Services;
using Prism.Mvvm;
using XFTest.Core.Models;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XFTest
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        //Register Classes for Dependency Injection
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.Register<IApiService<Visits>, ApiService>();
            containerRegistry.Register<IErrorLoggerService, ErrorLoggerService>();
        }
    }
}
