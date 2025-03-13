using CommonUtility.Interface;
using CommonUtility.SharedModels;
using IAM_UI;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IAM_UI.Helpers
{
    public class UsersForBillingApiTrackerUI
    {
        private readonly string _jsonFilePath;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrlUsersForBillingApiTrackerAPI;
        private readonly ICommonService _commonService;
        private readonly string _ApiKey;
        private readonly IGlobalModelService _globalModelService;

        public UsersForBillingApiTrackerUI(
            IWebHostEnvironment env,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ICommonService commonService,
            IGlobalModelService globalModelService)
        {
            _jsonFilePath = Path.Combine(env.WebRootPath, "UsersForBillingApiTracker", "UsersForBillingApiTracker.json");
            _httpClientFactory = httpClientFactory;

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_jsonFilePath));

            _configuration = configuration;
            _baseUrlUsersForBillingApiTrackerAPI = configuration["BaseUrlUsersForBillingApiTrackerAPI"];
            _commonService = commonService;
            _ApiKey = configuration["Apikey"];
            _globalModelService = globalModelService;
        }

        public async Task<bool> CheckAndUpdateUserForBillingTrackerAsync(ApiCallTrackerSaveModel model)
        {
            try
            {
            

                // Read the tracker data from the JSON file
                var tracker = await ReadApiCallTrackerAsync();

                // If the month and year match, no need to trigger the API
                if (tracker != null && tracker.Month == model.Month && tracker.Year == model.Year)
                {
                    return false; // API was not triggered
                }


                // Update the tracker with the current month and year
                tracker = new ApiCallTracker
                {
                    Month = model.Month,
                    Year = model.Year
                };
               

                // Call the API to insert data into the database
                var  result =   await TriggerDatabaseInsertApi(model);
                if (result != 0)
                {
                    await WriteApiCallTrackerAsync(tracker);
                }
                return true; // API was triggered
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<ApiCallTracker> ReadApiCallTrackerAsync()
        {
            if (!File.Exists(_jsonFilePath))
            {
                return null; // File does not exist
            }

            var jsonText = await File.ReadAllTextAsync(_jsonFilePath);
            return string.IsNullOrWhiteSpace(jsonText) ? null : JsonConvert.DeserializeObject<ApiCallTracker>(jsonText);
        }

        private async Task WriteApiCallTrackerAsync(ApiCallTracker tracker)
        {
            var jsonText = JsonConvert.SerializeObject(tracker, Formatting.Indented);
            await File.WriteAllTextAsync(_jsonFilePath, jsonText);
        }

        private async Task<int> TriggerDatabaseInsertApi(ApiCallTrackerSaveModel model)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Add API key header
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));

                    string jsonBody = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    var urlParameter = "SaveUsersForBilling/";
                    // Construct API URL
                    string url = _baseUrlUsersForBillingApiTrackerAPI + urlParameter;

                    HttpResponseMessage response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        if (int.TryParse(responseContent, out int apiResponse))
                        {
                            return apiResponse;
                        }
                    }

                    return 0; // Default return if not successful
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Model for the tracker
        public class ApiCallTracker
        {
            public int Month { get; set; }
            public int Year { get; set; }
        }

        public class ApiCallTrackerSaveModel
        {
            public int TenantId { get; set; }
            public int ApplicationId { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }
        }
    }
}
