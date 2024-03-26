using eShopping.Client.Entities;
using System.Net;
namespace eShopping.Client.Data
{
    public interface IErrorService
    {
        bool ThrowErrors { get; set; }
        Task AddSmartErrorAsync(bool isError, string uri, HttpStatusCode? httpStatusCode, Exception? exception, string? debugMessage);
        Task AddErrorAsync(string uri, int error, string message, string debugMessage);
        Task<List<ErrorData>> GetAllErrorsAsync();
        Task AcceptAllErrorsAsync();
        Task AcceptErrorAsync(ErrorData errorData);
        //Task AcceptError();
        //Task DeleteError();
        //Task GetByIdError();
    }
}
