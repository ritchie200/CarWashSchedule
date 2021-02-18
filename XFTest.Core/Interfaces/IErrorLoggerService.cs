using System;
using System.Threading.Tasks;

namespace XFTest.Core.Interfaces
{
    public interface IErrorLoggerService
    {
        void SaveErrorInDatabase(string method, string page, string errorMessage);
    }
}
