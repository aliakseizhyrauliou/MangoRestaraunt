using Mango.Web.Models;

namespace Mango.Web.Services.IServices
{
    public interface IBaseService : IDisposable
    {
        ResponceDto responceDto { get; set; }

        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
