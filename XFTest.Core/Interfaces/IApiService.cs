using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using XFTest.Core.Models;


namespace XFTest.Core.Interfaces
{
    public interface IApiService<T>
    {
        void GetAllVisitsbyDate(DateTime visitDate);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false, int lastIndex = 0);
    }
}
