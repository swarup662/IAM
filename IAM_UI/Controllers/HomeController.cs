using CommonUtility.Interface;
using CommonUtility.SharedModels;
using IAM_UI.Helpers;
using IAM_UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace IAM_UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _EncryptionKey;
        private readonly string _SessionName;
        private readonly string _LoginSessionName;
        private readonly IConfiguration _configuration;
        private readonly ILoggerService _logger;
        private readonly string _baseUrlHome;
        private readonly ICommonService _commonService;
        private readonly string _ApiKey;
   
        private readonly string _LoginUrl;
        private readonly IMemoryCache _memoryCache;
        private readonly IGlobalModelService _globalModelService;
        private readonly string BaseUrlAllModule;
        private readonly string BaseUrl;
        public HomeController(IConfiguration configuration, ICommonService commonService, ILoggerService logger, IMemoryCache memoryCache, IGlobalModelService globalModelService)
        {
            _configuration = configuration;
            _baseUrlHome = configuration["BaseUrlHome"];
            _commonService = commonService;
            _ApiKey = configuration["Apikey"];
            _logger = logger;
            _LoginUrl = configuration["Data:LoginUrl"];
            _EncryptionKey = configuration["Data:Key"];
            _memoryCache = memoryCache;
            _globalModelService = globalModelService;
            _LoginSessionName = configuration["Data:LoginSessionName"];
            _SessionName = configuration["Data:SessionName"];
            BaseUrlAllModule = configuration["AllModuleGridApproval"];
            BaseUrl = configuration["BaseUrlCompany"];
        }



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }








        [HttpPost]
        public async Task<IActionResult> ReceiveData()
        {


            // Extract OAuth header
            var oAuthHeader = Request.Headers["OAuth"].FirstOrDefault();

            if (string.IsNullOrEmpty(oAuthHeader))
            {
                return BadRequest("OAuth header missing");
            }

            // Read the request body asynchronously
            string requestBody;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();

            }

            // Get the content type of the request
            var contentType = Request.ContentType;

            if (string.IsNullOrEmpty(contentType) || !contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status415UnsupportedMediaType, "Content type not supported. Only 'application/json' is allowed.");
            }

            try
            {
                // Parse the JSON body
                var jsonData = JObject.Parse(requestBody);

                // Extract the encrypted OAuth token
                var encOAuth = jsonData["EncOAuth"]?.ToString();

                if (string.IsNullOrEmpty(encOAuth))
                {
                    return BadRequest("EncOAuth field is missing in the request");
                }

                // Decrypt the OAuth token and process as needed
                Global_Model globalModel = DecryptFromBase64(encOAuth, _EncryptionKey);

                // Extract and assign data from the JSON

                var DataModel = JObject.Parse(globalModel.Data);

                globalModel.Data = DataModel["Data"]?.ToString();

                if (globalModel.Data == oAuthHeader)
                {

                    globalModel.Token = DataModel["Token"]?.ToString();
                    globalModel.ModuleAccessData = DataModel["ModuleAccessData"]?.ToString();
                    globalModel.CompanyList = DataModel["CompanyList"]?.ToString();
                    globalModel.UserDetail = DataModel["UserDetail"]?.ToString();
                    globalModel.Approval = DataModel["ApprovalTable"]?.ToString();
                    // Set up cache options
                    var cacheKey = $"{globalModel.Data}";

                    var cacheExpiryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.UtcNow.AddMinutes(5), // Use UTC for consistency
                        Priority = CacheItemPriority.High,
                        SlidingExpiration = TimeSpan.FromSeconds(20)
                    };

                    // Store the global model in the cache
                    _memoryCache.Set(cacheKey, globalModel, cacheExpiryOptions);
                    Console.WriteLine($"Cache populated with key: {cacheKey}");
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (JsonException ex)
            {
                return BadRequest($"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private static Global_Model DecryptFromBase64(string base64Encoded, string key)
        {
            // Convert Base64 string to byte array
            byte[] encryptedBytes = Convert.FromBase64String(base64Encoded);

            // Convert the key to bytes
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            // Ensure the key length is valid for AES (16, 24, or 32 bytes)
            byte[] keyBytes32 = new byte[32];
            Array.Copy(keyBytes, keyBytes32, Math.Min(keyBytes.Length, keyBytes32.Length));

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes32;

                // Extract the IV from the beginning of the encrypted data
                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(encryptedBytes, 0, iv, 0, iv.Length);
                aes.IV = iv;

                // Get the encrypted data (after the IV)
                byte[] cipherBytes = new byte[encryptedBytes.Length - iv.Length];
                Array.Copy(encryptedBytes, iv.Length, cipherBytes, 0, cipherBytes.Length);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            // Read the decrypted string from the stream
                            string jsonString = sr.ReadToEnd();

                            // Deserialize the JSON string to GlobalModel
                            return JsonConvert.DeserializeObject<Global_Model>(jsonString);
                        }
                    }
                }
            }
        }





        public async Task<IActionResult> Dashboard()
        {


            try
            {
                var sessionData = HttpContext.Session.GetString(_LoginSessionName);
                string id = HttpContext.Request.Query["oAuth"];

                // Check if both id and sessionData are null or empty, in which case log the user out
                if (string.IsNullOrEmpty(id) && (string.IsNullOrEmpty(sessionData) || sessionData == "null"))
                {
                    return Redirect(_LoginUrl); // Redirect to login if both are null or empty
                }


                // If the id is valid (not null or empty), try to get the value from the cache
                if (!string.IsNullOrEmpty(id) && _memoryCache.TryGetValue(id, out Global_Model globalModel))
                {
                    // If the cache contains the global model, update the session with it
                    HttpContext.Session.SetString(_LoginSessionName, JsonConvert.SerializeObject(globalModel));
                }

                else if (!string.IsNullOrEmpty(sessionData) && sessionData != "null")
                {
                    // If sessionData is valid (not null or "null"), proceed with normal behavior
                    var globalModelFromSession = JsonConvert.DeserializeObject<Global_Model>(sessionData);
                    HttpContext.Session.SetString(_LoginSessionName, sessionData);
                }
                else
                {
                    // If both id and sessionData are invalid, redirect to the login page
                    return Redirect(_LoginUrl);
                }

                // Remove the item from the cache if it was used
                if (!string.IsNullOrEmpty(id))
                {
                    _memoryCache.Remove(id);
                }


                await DashboardWizards();
                // Return the dashboard view
                return View();


            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred.", ex);
            }
        }



        public async Task<IActionResult> DashboardWizards()
        {


            try
            {
                GlobalModel gm = new GlobalModel();

                string urlParameters = "Dashboard/";
                using (var httpClient = new HttpClient())
                {
                    gm.Data = "";

                    string jsonBody = JsonConvert.SerializeObject(gm);

                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    string url = _baseUrlHome + urlParameters;


                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);  //API Hit From Here

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();

                        DataSet ds = JsonConvert.DeserializeObject<DataSet>(responseContent);

                        if (ds.Tables.Contains("Users") && ds.Tables["Users"].Rows.Count > 0)
                        {
                            ViewBag.usercount = ds.Tables["Users"].Rows[0]["usercount"];
                        }
                        else
                        {
                            ViewBag.usercount = 0; // Or assign a default value
                        }



                        return View();
                    }
                    else
                    {

                        return View("Error");
                    }
                }
            }



            catch (Exception ex)
            {


                throw ex;
            }
        }






        public JsonResult ChangeCompanyUserType(string companyId, string userTypeId)
        {
            try
            {
                // Retrieve the LoginData from the session first
                var loginData = HttpContext.Session.GetString(_LoginSessionName);

                if (string.IsNullOrEmpty(loginData))
                {
                    return Json(new { message = "Login data not found in session." });
                }

                // Deserialize the login data into the GlobalModel object
                var gm = JsonConvert.DeserializeObject<Global_Model>(loginData);

                // Update the CompanyID and UserTypeId
                if (int.TryParse(companyId, out int companyIdInt))
                {
                    gm.CompanyID = companyIdInt;
                }
                else
                {
                    return Json(new { message = "Invalid companyId." });
                }

                if (int.TryParse(userTypeId, out int userTypeIdInt))
                {
                    gm.UserTypeId = userTypeIdInt;
                }
                else
                {
                    return Json(new { message = "Invalid userTypeId." });
                }

                // Serialize the updated object back into JSON
                var json = JsonConvert.SerializeObject(gm);

                // Store the updated object back in the session
                HttpContext.Session.SetString(_LoginSessionName, json);

                // Return success response with a message
                return Json(new { message = "success" });
            }
            catch (Exception ex)
            {
                // Log the error (optional)
                return Json(new { message = "An error occurred." });
            } 
        }



        [HttpPost]
        public IActionResult Logout()
        {
            try
            {
                // Check if session is configured
                if (HttpContext.Session != null)
                {
                    // Clear session data
                    HttpContext.Session.Clear();
                }

                // Check if TempData is configured
                if (TempData != null)
                {
                    // Clear TempData (if you are using it)
                    TempData.Clear();
                }

                // Return a JSON response with the redirect URL
                return Json(new { redirectUrl = _LoginUrl });
            }
            catch (Exception ex)
            {
                // Log the exception
                throw ex;
            }
        }




        //public async Task<JsonResult> ApproveReject(ApproveRejectModel model)
        //{

        //    try
        //    {
        //        string jsonses = HttpContext.Session.GetString("UserData");
        //        VM_UserLoginResponse loginresponseData = JsonConvert.DeserializeObject<VM_UserLoginResponse>(jsonses);
        //        model.UserId = loginresponseData.Employee_Master_Key;
        //        model.UserTypeId = loginresponseData.UserTypeId;
        //        model.TenantId = 2;
        //        model.ApplicationdId = Convert.ToInt32(loginresponseData.AppID);
        //        model.CompanyId = loginresponseData.Company;
        //        using (var httpClient = new HttpClient())
        //        {
        //            string urlParameters = "ApproveRejectRow";
        //            var jsonContent = JsonConvert.SerializeObject(model);

        //            // Create the content for the POST request (application/json)
        //            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //            HttpResponseMessage response = await httpClient.PostAsync(BaseUrlAllModule + urlParameters, content);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var data = await response.Content.ReadAsStringAsync();

        //                return Json(data);
        //            }
        //            else
        //            {

        //                return Json("Error");
        //            }
        //        }

        //    }

        //    catch (Exception ex)
        //    {
        //        return Json("Error");
        //    }
        //}

        //[HttpPost]
        //public async Task<List<APPROVAL>> GetHeadList(HEAD_LIST model)
        //{
        //    var httpClient = new HttpClient();
        //    VM_UserLoginResponse SessionData = new VM_UserLoginResponse();
        //    //model.EmployeeId = SessionData.Employee_Master_Key;
        //    string urlParameters = "GetHeadList";
        //    string str = JsonConvert.SerializeObject(model).ToString();

        //    var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        //    //var response = await httpClient.PostAsync(BaseUrl + urlparameters, content);
        //    var response = await httpClient.PostAsync(BaseUrl + urlParameters, content);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Deserialize the API response to your desired data model or use dynamic if the structure is unknown.
        //        var data = await response.Content.ReadAsStringAsync();

        //        List<APPROVAL> lst = JsonConvert.DeserializeObject<List<APPROVAL>>(data);



        //        return lst;

        //    }
        //    else
        //    {
        //        // Handle the API error if needed.
        //        return new List<APPROVAL>();
        //    }


        //}





        //public async Task<JsonResult> SendToApproval(SendToApproval model)
        //{
        //    try
        //    {


        //        UserDetail emp = JsonConvert.DeserializeObject<UserDetail>(HttpContext.Session.GetString("UserData"));
        //        model.OperationalMode = Convert.ToInt32(HttpContext.Session.GetString("OperationalMode"));
        //        // Use the 'emp' object here

        //        model.CompanyId = emp.CurrentCompanyId;
        //        model.UserId = emp.EMPLOYEE_MASTER_KEY;
        //        model.UserTypeId = emp.CurrentUserTypeId;
        //        model.ApplicationId = emp.AppID;
        //        model.TenantId = emp.TenantId;

        //        using (var httpClient = new HttpClient())
        //        {
        //            string urlParameters = "SendToApproval";
        //            var jsonContent = JsonConvert.SerializeObject(model);

        //            // Create the content for the POST request (application/json)
        //            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //            HttpResponseMessage response = await httpClient.PostAsync(BaseUrlAllModule + urlParameters, content);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var data = await response.Content.ReadAsStringAsync();

        //                return Json(data);
        //            }
        //            else
        //            {

        //                return Json("Error");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        throw ex;
        //    }
        //}








    }
}
