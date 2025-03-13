using Newtonsoft.Json;
using System.Text;

namespace IAM_UI.Helpers
{

    public class APIResultsValue
    {
        private readonly IConfiguration _configuration;
        private readonly string _ApiKey;

        public APIResultsValue(IConfiguration configuration)
        {
            _configuration = configuration;
            _ApiKey = configuration["Apikey"];
            
        }
        public async Task<List<CustomSelectListItem>> GetDataFromApi(string BaseUrl, string apiEndpoint)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                HttpResponseMessage response = await httpClient.GetAsync(BaseUrl + apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<CustomSelectListItem>>(data);
                }
                else
                {
                    // Handle the error if needed
                    return null; // Return null if there was an error
                }
            }
        }

        public async Task<object> GetGridData(string BaseUrl, string apiEndpoint, Type modelType)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                HttpResponseMessage response = await httpClient.GetAsync(BaseUrl + apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    // Deserialize the response dynamically based on the modelType
                    return JsonConvert.DeserializeObject(data, modelType);
                }
                else
                {
                    // Handle the error if needed
                    return null; // Return null if there was an error
                }
            }
        }

        public async Task<object> PostGridData(string BaseUrl, string apiEndpoint, object model, Type modelType)
        {
            using (var httpClient = new HttpClient())
            {
                // Serialize the model object to JSON
                var jsonContent = JsonConvert.SerializeObject(model);

                // Create the content for the POST request (application/json)
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await httpClient.PostAsync(BaseUrl + apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response dynamically based on the modelType
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject(data, modelType);
                }
                else
                {
                    // Handle the error if needed
                    return null; // Return null if there was an error
                }
            }
        }


    }

}
