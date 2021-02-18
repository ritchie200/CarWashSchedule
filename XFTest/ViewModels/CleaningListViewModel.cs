using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Prism.Events;
using XFTest.Core.Models;
using XFTest.Core.Interfaces;
using System;
using Xamarin.Forms;
using Prism.Commands;
using XFTest.Events;
using System.Globalization;
using System.Linq;
using System.Diagnostics;

namespace XFTest.ViewModels
{
	public class CleaningListViewModel : BindableBase, INotifyPropertyChanged
    {
        #region Declare Variables
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;
        private readonly IApiService<Visits> _apiService;
        private readonly IErrorLoggerService _errorLoggerService;
        bool isBusy = false;
        public const string ScrollToPreviousLastItem = "Scroll_ToPrevious";
        private int _itemTreshold;
        private bool _isRefreshing;
        private string _lblCurrentDay;
        private int _currentMonth;
        private string _lblCurrentDayYear;
        private ObservableCollection<CalenderWeekAndDay> _listCalenderMonthDated = new ObservableCollection<CalenderWeekAndDay>();
        private ObservableCollection<Visits> _listVisits = new ObservableCollection<Visits>();
        #endregion

        #region Declare Properties
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set { SetProperty(ref _isRefreshing, value); }
        }

        public int ItemTreshold
        {
            get { return _itemTreshold; }
            set { SetProperty(ref _itemTreshold, value); }
        }

        public string lblCurrentDay
        {
            get { return _lblCurrentDay; }
            set
            {
                SetProperty(ref _lblCurrentDay, value);
            }
        }

        public ObservableCollection<CalenderWeekAndDay> listCalenderMonthDated
        {
            get { return _listCalenderMonthDated; }
            set
            {
                SetProperty(ref _listCalenderMonthDated, value);
            }
        }
        public int currentMonth
        {
            get { return _currentMonth; }
            set
            {
                SetProperty(ref _currentMonth, value);
            }
        }
        public ObservableCollection<Visits> listVisits
        {
            get { return _listVisits; }
            set
            {
                SetProperty(ref _listVisits, value);
            }
        }
        public string lblCurrentDayYear
        {
            get { return _lblCurrentDayYear; }
            set
            {
                SetProperty(ref _lblCurrentDayYear, value);
            }
        }
        #endregion

        #region Declare Commands
        public Command LoadItemsCommand { get; set; }
        public Command ItemTresholdReachedCommand { get; set; }
        public Command RefreshItemsCommand { get; set; }
        public DelegateCommand GoToMainPageCommand { get; set; }
        public DelegateCommand ShowCalenderCommand { get; set; }
        public DelegateCommand MoveMonthBackCommand { get; set; }
        public DelegateCommand MoveMonthFrontCommand { get; set; }
        public Command DaySelectedCommand { get; set; }
        public DelegateCommand HideOpenCalenderCommand { get; set; }

        #endregion

        public CleaningListViewModel( IDialogService dialogService, INavigationService navigationService, IEventAggregator eventAggregator, IApiService<Visits> apiService, IErrorLoggerService errorLoggerService)
        {
            _eventAggregator = eventAggregator;
            _apiService = apiService;
            _navigationService = navigationService;
            _errorLoggerService = errorLoggerService;

            #region Used for Visit List Logic
            listVisits = new ObservableCollection<Visits>();
            ItemTreshold = 1;
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTresholdReachedCommand = new Command(async () => await ItemsTresholdReached());
            RefreshItemsCommand = new Command(async () =>
            {
                await ExecuteLoadItemsCommand();
                IsRefreshing = false;
            });
            #endregion


            lblCurrentDay = "Today";
            currentMonth = DateTime.Now.Month;
            lblCurrentDayYear = DateTime.Now.ToString("MMM yyyy");
            listCalenderMonthDated = new ObservableCollection<CalenderWeekAndDay>(GetCalenderWeekAndDays(DateTime.Now.Year, DateTime.Now.Month) as List<CalenderWeekAndDay>);

            #region Initialize Commands
            GoToMainPageCommand = new DelegateCommand(GoToMainPage);
            ShowCalenderCommand = new DelegateCommand(ShowHideCalender);
            HideOpenCalenderCommand = new DelegateCommand(HideOpenCalenderFromPageTap);
            #endregion

            #region Initialize Calender Events
            MoveMonthBackCommand = new DelegateCommand(MoveMonthBack);
            MoveMonthFrontCommand = new DelegateCommand(MoveMonthFront);
            DaySelectedCommand = new Command((parameter) => { DaySelected(parameter); });
            #endregion

            //Set Visits for Current Day by Default
            _apiService.GetAllVisitsbyDate(DateTime.Now);
            //Get Visits Using Infinite Scrolling Logic, 5 at a time
            LoadItemsCommand.Execute(null);
        }

        /* Navigates back to Main Page  */
        private void GoToMainPage()
        {
            try
            {

               _navigationService.GoBackAsync();
               
            }
            catch (Exception ex)
            {
                _errorLoggerService.SaveErrorInDatabase("GoToMainPage", "CleaningListPage", ex.Message);
            }
        }

        #region Used for Calender Logic
        private void ShowHideCalender()
        {
            try
            {
              
                _eventAggregator.GetEvent<CalenderEvent>().Publish();
            }
            catch (Exception ex)
            {
                _errorLoggerService.SaveErrorInDatabase("ShowHideCalender", "CleaningListPage", ex.Message);
            }
        }
        private void HideOpenCalenderFromPageTap()
        {
            try
            {

                _eventAggregator.GetEvent<HideCalenderEvent>().Publish();
            }
            catch (Exception ex)
            {
                _errorLoggerService.SaveErrorInDatabase("HideOpenCalenderFromPageTap", "CleaningListPage", ex.Message);
            }
        }
      
        /* Move Month Back  */
        private void MoveMonthBack()
        {
            try
            {
                if (currentMonth == 1)
                    return;
                else
                {
                    currentMonth -= 1;
                    listCalenderMonthDated = new ObservableCollection<CalenderWeekAndDay>(GetCalenderWeekAndDays(DateTime.Now.Year, currentMonth) as List<CalenderWeekAndDay>);
                    DateTimeFormatInfo mfi = new DateTimeFormatInfo();
                    string strMonth = mfi.GetMonthName(currentMonth).Substring(0, 3);
                    lblCurrentDayYear = strMonth + " " + DateTime.Now.ToString("yyyy"); 
                }

            }
            catch (Exception ex)
            {
                _errorLoggerService.SaveErrorInDatabase("MoveMonthBack", "CleaningListPage", ex.Message);
            }
        }

        /* Move Month Front  */
        private void MoveMonthFront()
        {
            
            try
            {

                if (currentMonth == 12)
                    return;
                else
                {
                    currentMonth += 1;
                    listCalenderMonthDated = new ObservableCollection<CalenderWeekAndDay>(GetCalenderWeekAndDays(DateTime.Now.Year, currentMonth) as List<CalenderWeekAndDay>);
                    DateTimeFormatInfo mfi = new DateTimeFormatInfo();
                    string strMonth = mfi.GetMonthName(currentMonth).Substring(0, 3);
                    lblCurrentDayYear = strMonth + " " + DateTime.Now.ToString("yyyy");
                }

            }
            catch (Exception ex)
            {
                _errorLoggerService.SaveErrorInDatabase("MoveMonthFront", "CleaningListPage", ex.Message);
            }
        }

        /* Day Selected  */
        private void DaySelected(object parameter)
        {
            try
            {
                //Hide Calender
                ShowHideCalender();
                List<CalenderWeekAndDay> listCalenderDays = new List<CalenderWeekAndDay>();
                var day = Convert.ToString(parameter);

                //Logic for selecting/deSelecting dates in calender
                var item = listCalenderMonthDated.FirstOrDefault(i => i.Day == day);
                if (item != null)
                {
                    item.Selected = true;
                }
                listCalenderMonthDated.Where(i => i.Day != day).ToList().ForEach(d =>
                {
                    d.Selected = false;
                });

                DateTimeFormatInfo mfi = new DateTimeFormatInfo();
                lblCurrentDay = day+" "+ mfi.GetMonthName(currentMonth).Substring(0, 3) + " " + DateTime.Now.ToString("yyyy");

                //Set Visits for Day Selected
                _apiService.GetAllVisitsbyDate(new DateTime(DateTime.Now.Year, currentMonth, Convert.ToInt32(day)));
                //Get Visits Using Infinite Scrolling Logic, 5 at a time
                LoadItemsCommand.Execute(null);

            }
            catch (Exception ex)
            {
                _errorLoggerService.SaveErrorInDatabase("DaySelected", "CleaningListPage", ex.Message);
            }
        }

        //Get Calender Week And Days 
        private List<CalenderWeekAndDay> GetCalenderWeekAndDays(int year, int month)
        {
            List<CalenderWeekAndDay> calenderWeekAndDays = new List<CalenderWeekAndDay>();
            var listAllDays = AllDatesInMonth(year, month);
            foreach(DateTime dateValue in listAllDays)
            {
                CalenderWeekAndDay calenderWeekAndDay = new CalenderWeekAndDay();
                calenderWeekAndDay.Day = dateValue.ToString("dd");
                calenderWeekAndDay.Selected = dateValue.Date == DateTime.Now.Date ? true : false;
                calenderWeekAndDay.WeekDay = dateValue.DayOfWeek.ToString().Substring(0, 3);
                calenderWeekAndDays.Add(calenderWeekAndDay);
            }

            return calenderWeekAndDays;
        }

        //All Dates In Month
        public static IEnumerable<DateTime> AllDatesInMonth(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= days; day++)
            {
                yield return new DateTime(year, month, day);
            }
        }

        #endregion

        #region Used for Infinite Scroll Logic
        async System.Threading.Tasks.Task ItemsTresholdReached()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var visits = await _apiService.GetItemsAsync(true, listVisits.Count);

                var previousLastVisit = listVisits.Last();
                foreach (var visit in visits)
                {
                    listVisits.Add(visit);
                }

                if (visits.Count() == 0)
                {
                    ItemTreshold = -1;
                    return;
                }
            }
            catch (Exception ex)
            {
                _errorLoggerService.SaveErrorInDatabase("ItemsTresholdReached", "CleaningListPage", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async System.Threading.Tasks.Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                ItemTreshold = 1;
                listVisits.Clear();
                var visits = await _apiService.GetItemsAsync(true);
                foreach (var visit in visits)
                {
                    listVisits.Add(visit);
                }
            }
            catch (Exception ex)
            {
                _errorLoggerService.SaveErrorInDatabase("ExecuteLoadItemsCommand", "CleaningListPage", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
