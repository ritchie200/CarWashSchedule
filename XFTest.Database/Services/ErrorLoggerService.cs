using System;
using System.Threading.Tasks;
using XFTest.Core.Interfaces;

namespace XFTest.Database.Services
{
    public class ErrorLoggerService : IErrorLoggerService
    {
        public void SaveErrorInDatabase(string method, string page, string errorMessage)
        {
            //Save Error in Database , use FunctionName, PageName, ErrorMessage
        }
    }
}
