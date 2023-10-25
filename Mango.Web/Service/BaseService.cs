using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ResponseDTO?> SendAsync(RequestDTO requestDTO)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
                HttpRequestMessage message = new();
                message.Headers.Add("Content-Type", "application/json");
                //token

                message.RequestUri = new Uri(requestDTO.Url);
                //if there is put or post request we have to send the data to the api for that we have to serialize the data

                if (requestDTO.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage? apiResponse = null;

                switch (requestDTO.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                apiResponse = await client.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new ResponseDTO() { Message = "Not found" };
                    case HttpStatusCode.Forbidden:
                        return new ResponseDTO() { Message = "Access Denied" };
                    case HttpStatusCode.Unauthorized:
                        return new ResponseDTO() { Message = "Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new ResponseDTO() { Message = "InternalServerError" };
                    default:
                        var apiContenet = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDTO = JsonConvert.DeserializeObject<ResponseDTO>(apiContenet);
                        return apiResponseDTO;
                }
            }
          
            catch (Exception ex)
            {
                return new ResponseDTO() { Message = ex.Message };
            }
        }
    }
}
