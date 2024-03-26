using eShopping.Client.Entities;
using System.Net;

namespace eShopping.Client.Data
{
    public class ErrorService : IErrorService
    {
        public bool ThrowErrors { get; set; } = false;
        private static List<ErrorData> errors = [];

        public async Task AddSmartErrorAsync(bool isError, string uri, HttpStatusCode? httpStatusCode, Exception? exception, string? debugMessage)
        {
            if (!isError)
                return;
            if (httpStatusCode is not null)
                await AddErrorAsync(uri, (int)httpStatusCode.Value, httpStatusCode.Value.ToString(), debugMessage ?? string.Empty);
            if (exception is not null)
            { 
                await AddErrorAsync(uri, exception.HResult, exception.Message.ToString(), exception.StackTrace ?? string.Empty);
                if (ThrowErrors) 
                    throw exception;
            }
        }
        public async Task AddErrorAsync(string uri,  int error, string message, string debugMessage)
        {
            errors.Add(new ErrorData(uri, error, message, debugMessage));
            await Task.CompletedTask;
        }

        public async Task<List<ErrorData>> GetAllErrorsAsync()
        {
            return await Task.FromResult(errors);
        }
        public async Task AcceptAllErrorsAsync()
        {
            errors = [];
            await Task.CompletedTask;
        }
        public async Task AcceptErrorAsync(ErrorData errorData)
        {
            errors.Remove(errorData);
            await Task.CompletedTask;
        }

    }
}
