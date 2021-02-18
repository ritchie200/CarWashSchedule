using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XFTest.Core.Models
{
    public class CalenderWeekAndDay : INotifyPropertyChanged
    {
        public string Day { get; set; }
        public string WeekDay { get; set; }
        public bool IsVisible {
            get { return !Selected; }
            set
            {
                OnPropertyChanged();
            }
        }
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                IsVisible = !selected;
                OnPropertyChanged();
            }
        } 

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
