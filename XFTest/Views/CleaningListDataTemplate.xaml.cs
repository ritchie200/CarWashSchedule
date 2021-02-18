using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XFTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CleaningListDataTemplate : ContentView
    {
        public CleaningListDataTemplate()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            var state = width < 280 ? "Small" : width < 360 ? "Medium" : "Large";
            VisualStateManager.GoToState(CardlblTitle, state);
            VisualStateManager.GoToState(detailsText, state);
            VisualStateManager.GoToState(Icon1Lbl, state);
            VisualStateManager.GoToState(Icon2Lbl, state);
            VisualStateManager.GoToState(Icon3Lbl, state);
            VisualStateManager.GoToState(Icon4Lbl, state);
            VisualStateManager.GoToState(ButtonStyle, state);
        }
    }
}