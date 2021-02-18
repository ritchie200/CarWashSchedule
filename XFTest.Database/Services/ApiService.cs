using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XFTest.Core.Interfaces;
using XFTest.Core.Models;

namespace XFTest.Database.Services
{
    public class ApiService : IApiService<Visits>
    {
        ObservableCollection<Visits> items;

        //Get Visits by Data Filter
        public void GetAllVisitsbyDate(DateTime visitDate)
        {
            VisitData visitData = new VisitData();

            //Read from Json File, Get Visits Data
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ApiService)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("XFTest.Database.VisitsData.json");
            using (var reader = new System.IO.StreamReader(stream))
            {
                var jsonData = reader.ReadToEnd();
                visitData = JsonConvert.DeserializeObject<VisitData>(jsonData);
                //Filter Visits by Date Filter
                visitData.Data = new ObservableCollection<Visits>(visitData.Data.Where(x => x.StartTimeUtc.Date >= visitDate.Date && x.EndTimeUtc.Date <= visitDate.Date).ToList() as List<Visits>);

                //Set PrevHouseOwnerLatitude, PrevHouseOwnerLongitude for each Visit, Used for Distance Calculation logic from Previous Visit
                for (int i = 0; i < visitData.Data.Count; i++)
                {
                    if (i > 0)
                    {
                        visitData.Data[i].PrevHouseOwnerLatitude = visitData.Data[i - 1].HouseOwnerLatitude;
                        visitData.Data[i].PrevHouseOwnerLongitude = visitData.Data[i - 1].HouseOwnerLongitude;
                    }
                }

                //Set Filtered Visit List
                items = visitData.Data;

            }

        }

        //Get Visits based on Infinte Scrollling (Show 5 Items at a time)
        public async Task<IEnumerable<Visits>> GetItemsAsync(bool forceRefresh = false, int lastIndex = 0)
        {
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(300));
            int numberOfItemsPerPage = 5;
            return await System.Threading.Tasks.Task.FromResult(items.Skip(lastIndex).Take(numberOfItemsPerPage));
        }

    }
}
