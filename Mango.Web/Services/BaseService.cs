using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        public ResponceDto responceDto { get; set; }
        public IHttpClientFactory httpClient { get; set; }

        public BaseService(IHttpClientFactory http)
        {
            responceDto = new ResponceDto();
            httpClient = http;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(true);
        }

        public async Task<T> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MangoAPI");
                var message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                client.DefaultRequestHeaders.Clear();
                if (!String.IsNullOrEmpty(apiRequest.AccessToken)) 
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.AccessToken);
                }
                if (apiRequest.Data != null) 
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage apiResponce = null;
                switch (apiRequest.ApiType) 
                {
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                apiResponce = await client.SendAsync(message);

                var apiContent = await apiResponce.Content.ReadAsStringAsync();
                var apiResponceDto = JsonConvert.DeserializeObject<T>(apiContent);
                return apiResponceDto;
            }
            catch (Exception ex) 
            {
                var dto = new ResponceDto()
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string>() { ex.Message }
                };

                var res = JsonConvert.SerializeObject(dto);
                var apiResponceDto = JsonConvert.DeserializeObject<T>(res);

                return apiResponceDto;

            }
        }
    }
}
