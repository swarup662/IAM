using CommonUtility.Interface;
using CommonUtility.SharedModels;
using IAM_UI.Helpers;
using IAM_UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.Design;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace IAM_UI.Controllers
{

    public class LoginController : Controller
    {
        private readonly IEncryptDecrypt _encryptDecrypt;
        private readonly IConfiguration _configuration;
        private readonly string _encryptKey;
        private readonly string _baseurl;
        private readonly ICommonService _commonService;
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private readonly List<string> _trustedHosts;
        private readonly string _ApiKey;
        private readonly UsersForBillingApiTrackerUI _usersForBillingApiTrackerUI;
        private readonly UsersForBillingApiTrackerUIController _UsersForBillingApiTrackerUIController;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _baseUrlempinfo;

        public LoginController(IConfiguration configuration, IEncryptDecrypt encryptDecrypt, ICommonService commonService, IMemoryCache cache, HttpClient httpClient, IOptions<TrustedHostsOptions> options, UsersForBillingApiTrackerUI usersForBillingApiTrackerUI, UsersForBillingApiTrackerUIController UsersForBillingApiTrackerUIController, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _encryptDecrypt = encryptDecrypt;
            _encryptKey = configuration["Data:Key"];
            _baseurl = configuration["LoginUrl"];
            _commonService = commonService;
            _cache = cache;
            _httpClient = httpClient;
            _trustedHosts = options.Value.TrustedHosts;
            _ApiKey = configuration["Apikey"];
            _usersForBillingApiTrackerUI = usersForBillingApiTrackerUI;
            _UsersForBillingApiTrackerUIController = UsersForBillingApiTrackerUIController;
            _webHostEnvironment = webHostEnvironment;
            _baseUrlempinfo = configuration["BaseUrlEmpInfo"];
        }


        public IActionResult Login()
        {
            string redirectUrl = HttpContext.Request.Query["redirectUrl"];
            ViewBag.RedirectUrl = redirectUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (string.IsNullOrEmpty(loginModel.Username))
            {
                TempData["ErrorMsg"] = "No user to validate!";
                return RedirectToAction("Login", "Login");
            }
            else if (string.IsNullOrEmpty(loginModel.Password))
            {
                TempData["ErrorMsg"] = "Password not provided!";
                return RedirectToAction("Login", "Login");
            }
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Add the API key to the request headers
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));

                    string jsonBody = JsonConvert.SerializeObject(loginModel);

                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    string urlParameters = "OAuth";

                    string url = _baseurl + urlParameters;

                    HttpResponseMessage response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();

                        var responseData = JsonConvert.DeserializeObject<returnDetails>(responseContent);

                        // string decryptresponse = await _encryptDecrypt.DecryptAsync(responseData.returndata, _encryptKey);

                        var obj = JsonConvert.DeserializeObject<returndata>(responseData.returndata);


                        if (obj.returnMessage == "success")
                        {
                            //var token = obj.authToken;
                            string CompanyAccess = responseData.returnUserAuthorization;

                            string UniqueUserId = responseData.UniqueUserId;
                            string CategoryId = responseData.returnCategoryId;
                         
                            //Create A sessionData For UserID
                            HttpContext.Session.SetString("UserID", UniqueUserId);
                            HttpContext.Session.SetString("CategoryId", CategoryId);
                            HttpContext.Session.SetString("UserParentCompanModel", responseData.returnUserParentCompany);
                            // Cache the data with a unique key for each employee
                            _cache.Set($"CompanyData_{UniqueUserId}", CompanyAccess, TimeSpan.FromMinutes(30));




                            // Redirect to SetCompany action
                            return RedirectToAction("SetCompany", "Login");
                        }
                        else
                        {
                            TempData["ErrorMsg"] = obj.returnMessage;
                            return View();
                        }

                    }
                    else
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        TempData["ErrorMsg"] = $"Request failed with status code {response.StatusCode}: {errorResponse}";
                        return View();
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IActionResult> SetCompany()
        {

            var CategoryId = HttpContext.Session.GetString("CategoryId");

            ParentCompanyModel UserParentCompanyModel = JsonConvert.DeserializeObject<ParentCompanyModel>(HttpContext.Session.GetString("UserParentCompanModel"));
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
            {
                // Handle case where user is not authenticated or session expired
                return RedirectToAction("Login", "Login");
            }



            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
            {
                // Handle case where user is not authenticated or session expired

            }

            if (_cache.TryGetValue($"CompanyData_{HttpContext.Session.GetString("UserID")}", out string cachedCompanyAccess))
            {
                DataSet companyList = JsonConvert.DeserializeObject<DataSet>(cachedCompanyAccess);

                if (companyList != null && companyList.Tables.Count > 0)
                {
                    ViewBag.companyList = await _commonService.GetSelectListAsync(companyList.Tables[0], "CompanyId", "COMPANY_NAME", "--Select Company--", "0");

                    if (CategoryId == "1")
                    {

                        if (companyList != null && companyList.Tables.Count > 1)
                        {

                            var table = companyList.Tables[1];

                            if (table != null)
                            {
                                // Filter the rows based on the companyId
                                // Fetch data synchronously
                                var applicationList = table.AsEnumerable()

                                .Select(row => new
                                {
                                   
                                    name = row.Field<string>("APPLICATION_NAME"),
                                    tenantId = row.Field<long>("TenantId"),
                                    applicationId = row.Field<long>("ApplicationId"),
                                    Color = row.Field<string>("Color"),
                                    Icon = row.Field<string>("Icon")
                                })
                                .OrderByDescending(x => x.applicationId == 4)  // Ensures ApplicationId 3 appears first
                                .ThenBy(x => x.applicationId)  // Keeps the remaining items in order
                                .ToList();

                              


                                        // Encrypt data asynchronously
                                        var tasks = applicationList.Select(async app =>
                                {

                                    var encryptedData = $"{UserParentCompanyModel.CompanyId}|{app.tenantId}|{app.applicationId}";

                                    return new
                                    {
                                        name = app.name,
                                        color = app.Color,
                                        icon = app.Icon,
                                        data = await _encryptDecrypt.EncryptAsync(encryptedData, _encryptKey)
                                    };
                                });



                                // Wait for all tasks to complete
                                var resultList = await Task.WhenAll(tasks);

                                // Generate the HTML string
                                StringBuilder htmlBuilder = new StringBuilder();
                                htmlBuilder.Append("<div id='applicationsContainer'>");

                                if (resultList.Any())
                                {
                                    for (int i = 0; i < resultList.Length; i++)
                                    {
                                        if (i % 4 == 0)
                                        {
                                            htmlBuilder.Append("<div class='row app-row'>");
                                        }

                                        string formattedName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(resultList[i].name.ToLower());

                                        htmlBuilder.Append(" <div class='col-md-3 d-flex justify-content-center g-2'>");
                                        var link = "/Login/SetApplication?Key=" + resultList[i].data.ToString();
                                        var k = "/Login/SetApplication?Key=WpCOUlIxI3zQ76bSlDkVg1UqBbSAe2oYYPufO/br7hM=";
                                        htmlBuilder.Append($@"
                          
                                 <a class='card' link='{link}' 
                                    style='--bg-color: {resultList[i].color}; --bg-color-light: #f1f7ff; --text-color-hover: #e4e4e4; --box-shadow-color: rgba(220, 233, 255, 0.48);'>
                                    <div class='overlay'></div>
                                    <div class='circle'> <i class='{resultList[i].icon}'></i></div>
                                    <p class'=name'>{resultList[i].name}</p>
                                </a>
                                  ");

                                        htmlBuilder.Append(" </div>");
                                        if ((i + 1) % 4 == 0 || i == resultList.Length - 1)
                                        {
                                            htmlBuilder.Append("</div>"); // Close row div
                                        }
                                    }
                                }

                                htmlBuilder.Append("</div>");

                         


                                string a = htmlBuilder.ToString();


                                // Pass the HTML string to the View
                                ViewBag.ApplicationHtml = htmlBuilder.ToString();





                            }
                            else
                            {
                                return Json(new { error = "No application data available for the selected company." });
                            }
                        }
                        else
                        {
                            return Json(new { error = "No company data available in the dataset." });
                        }
                    }


                    else
                    {
                        ViewBag.ApplicationHtml = null;
                    }
                }
                else
                {
                    TempData["ErrorMsg"] = "No company data available.";
                }
            }
            else
            {
                TempData["ErrorMsg"] = "Company data not found in cache.";
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetApplicationsByCompany(int companyId)
        {

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
            {

                return RedirectToAction("Login", "Login");
            }

            // Try to get data from cache
            if (_cache.TryGetValue($"CompanyData_{HttpContext.Session.GetString("UserID")}", out string cachedCompanyAccess))
            {
                DataSet companyList = JsonConvert.DeserializeObject<DataSet>(cachedCompanyAccess);

                if (companyList != null && companyList.Tables.Count > 1)
                {

                    var table = companyList.Tables[1];

                    if (table != null)
                    {
                        // Filter the rows based on the companyId
                        // Fetch data synchronously
                        var applicationList = table.AsEnumerable()
                        .Where(row => row.Field<long>("CompanyId") == companyId)
                        .Select(row => new
                        {
                            name = row.Field<string>("APPLICATION_NAME"),
                            tenantId = row.Field<long>("TenantId"),
                            applicationId = row.Field<long>("ApplicationId"),
                            Color = row.Field<string>("Color"),
                            Icon = row.Field<string>("Icon")
                        })
                        .OrderByDescending(x => x.applicationId == 4)  // Ensures ApplicationId 3 appears first
                        .ThenBy(x => x.applicationId)  // Keeps the remaining items in order
                        .ToList();


                        // Encrypt data asynchronously
                        var tasks = applicationList.Select(async app =>
                            {

                                var encryptedData = $"{companyId}|{app.tenantId}|{app.applicationId}";

                                return new
                                {
                                    name = app.name,
                                    color = app.Color,
                                    icon = app.Icon,
                                    data = await _encryptDecrypt.EncryptAsync(encryptedData, _encryptKey)
                                };
                            });

                        // Wait for all tasks to complete
                        var resultList = await Task.WhenAll(tasks);

                        // Return the application list as JSON
                        return Json(new { applications = resultList });
                    }
                    else
                    {
                        return Json(new { error = "No application data available for the selected company." });
                    }
                }
                else
                {
                    return Json(new { error = "No company data available in the dataset." });
                }
            }
            else
            {

                return RedirectToAction("Login", "Login");
            }
        }
        public async Task<IActionResult> SetApplication(string Key)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
            {
                return RedirectToAction("Login", "Login");
            }
            if (_cache.TryGetValue($"CompanyData_{HttpContext.Session.GetString("UserID")}", out string cachedCompanyAccess))
            {
                DataSet companyList = JsonConvert.DeserializeObject<DataSet>(cachedCompanyAccess);

                if (companyList != null && companyList.Tables.Count > 0)
                {
                    var decryptedString = await _encryptDecrypt.DecryptAsync(Key, _encryptKey);

                    var parts = decryptedString.Split('|');
                    if (parts.Length == 3)
                    {
                        var companyId = long.Parse(parts[0]);
                        var tenantId = long.Parse(parts[1]);
                        var applicationId = long.Parse(parts[2]);

                        var applicationLink = companyList.Tables[1].AsEnumerable()
                            .Where(row => row.Field<long>("ApplicationId") == applicationId && row.Field<long>("CompanyId") == companyId)
                            .Select(row => row.Field<string>("ApplicationLink"))
                            .Distinct()
                            .FirstOrDefault();
                        DataTable targetTable = companyList.Tables[1];
                        int[] companyIds = targetTable.AsEnumerable()
                        .Where(row => row.Field<long>("ApplicationId") == applicationId)
                        .Select(row => Convert.ToInt32(row.Field<long>("CompanyId"))) // Convert CompanyId to int
                        .ToArray();

                        DataTable filteredTable = companyList.Tables[0].AsEnumerable()
                                                             .Where(row => companyIds.Contains(Convert.ToInt32(row.Field<long>("CompanyId"))))
                                                             .CopyToDataTable();


                        if (applicationLink != null)
                        {
                            string[] UserKey = HttpContext.Session.GetString("UserID").Split('-');
                            GlobalModel globalModel = new GlobalModel();

                            globalModel.ApplicationId = Convert.ToInt32(applicationId);
                            globalModel.TenantID = Convert.ToInt32(tenantId);
                            globalModel.userID = Convert.ToInt32(UserKey[0]);
                            globalModel.CompanyID = Convert.ToInt32(companyId);
                            globalModel.ModuleId = 0;
                            globalModel.Data = HttpContext.Session.GetString("UserID");
                            //Generate Token For Application
                            using (var httpClient = new HttpClient())
                            {

                                // Add the API key to the request headers
                                httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));

                                string jsonBody = JsonConvert.SerializeObject(globalModel);

                                StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                                //Post To API For Token Generation
                                string urlParameters = "OAuthToken";

                                string url = _baseurl + urlParameters;

                                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                                if (response.IsSuccessStatusCode)
                                {
                                    string responseContent = await response.Content.ReadAsStringAsync();

                                    //string dresponse = await _encryptDecrypt.DecryptAsync(responseContent, _encryptKey);

                                    globalModel = JsonConvert.DeserializeObject<GlobalModel>(responseContent);

                                }


                                var jsonData = JObject.Parse(globalModel.Data);

                                var CompanyList = JsonConvert.SerializeObject(filteredTable);

                                var Data = jsonData["Data"]?.ToString();

                                var Token = jsonData["accessToken"]?.ToString();

                                var ModuleAccessData = jsonData["ModuleAccessData"]?.ToString();

                                var UserDetail = jsonData["UserDetail"]?.ToString();
                                var ApprovalTable = jsonData["approval"]?.ToString();

                                var returndata = new { Data, Token, ModuleAccessData, CompanyList, UserDetail, ApprovalTable };

                                globalModel.Data = JsonConvert.SerializeObject(returndata);

                            }



                            //UsersForBillingAPITracker
                            if (DateTime.UtcNow.Day == 1)
                            {

                                var apiCallTrackerModel = new UsersForBillingApiTrackerUI.ApiCallTrackerSaveModel
                                {
                                    TenantId = globalModel.TenantID, // Replace with actual TenantId from your login model
                                    ApplicationId = globalModel.ApplicationId, // Replace with actual ApplicationId from your login model
                                    Month = DateTime.UtcNow.Month,
                                    Year = DateTime.UtcNow.Year

                                };


                                if (globalModel.ApplicationId == 1)// only run this when login to hrms
                                {
                                    bool userForBillingApiTriggered = false;
                                    // Call the tracker to check and update fpr user for billing
                                    userForBillingApiTriggered = await _usersForBillingApiTrackerUI.CheckAndUpdateUserForBillingTrackerAsync(apiCallTrackerModel);



                                    if (userForBillingApiTriggered)
                                    {
                                        // API was triggered; you can log or perform additional logic if needed
                                        Console.WriteLine("API triggered for inserting data into the database.");


                                        var model = new SaasBillingEmpModel
                                        {
                                            Month = DateTime.UtcNow.Month,
                                            Year = DateTime.UtcNow.Year
                                        };

                                        // Fetch required employee data from API
                                        List<SaasBillingEmpModel> dataList = await FetchEmployeeDataFromApi(model);

                                        if (dataList.Count > 0)
                                        {
                                            // Generate PDF using FastReport
                                            var (pdfBytes, fileName) = GenerateEmployeePdf(dataList);

                                            if (pdfBytes != null && pdfBytes.Length > 0)
                                            {
                                               // await _UsersForBillingApiTrackerUIController.SaasApiTriggerMailSend(pdfBytes, fileName);

                                            }
                                        }

                                    }
                                    else
                                    {
                                        // API was not triggered; same month/year as in the JSON
                                        Console.WriteLine("API was not triggered as it has already been called for this month.");
                                    }
                                }




                            }


                            bool isLocalUrl = await UrlStructure();

                            if (isLocalUrl)
                            {
                                // Ensure globalModel and configuration are not null
                                if (globalModel?.ApplicationId != null)
                                {
                                    string appId = globalModel.ApplicationId.ToString(); // Convert ApplicationId to string
                                    string key = "locallinks:" + appId;
                                    var appLink = _configuration[key]; // Fetch LoginUrl from configuration
                                    applicationLink = appLink;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid globalModel or configuration.");
                                }
                            }



                            //Token Generate 
                            string encOAuth = EncryptToBase64(globalModel, _encryptKey);
                            _httpClient.DefaultRequestHeaders.Clear();

                            if (globalModel.ApplicationId == 2) //Logistics
                            {
                                if (IsTrustedUrl(applicationLink))
                                {
                                    try
                                    {
                                        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, applicationLink + "receivedata"))
                                        {
                                            using (var httpClient = new HttpClient())
                                            {
                                                // Add the OAuth header
                                                requestMessage.Headers.Add("OAuth", HttpContext.Session.GetString("UserID"));

                                                // Create and set the content
                                                var jsonContent = new { EncOAuth = encOAuth };
                                                var jsonString = JsonConvert.SerializeObject(jsonContent);
                                                requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                                                // Send the HTTP POST request asynchronously
                                                var result = await httpClient.SendAsync(requestMessage);

                                                if (result.IsSuccessStatusCode)
                                                {
                                                    // Redirect the user to the dashboard
                                                    var userId = HttpContext.Session.GetString("UserID");
                                                    return Redirect(applicationLink + "DashBoard?OAuth=" + Uri.EscapeDataString(userId));
                                                }
                                                else
                                                {
                                                    TempData["ErrorMsg"] = "Application Not Found";
                                                    return RedirectToAction("Login", "Login");
                                                }

                                            }


                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        // Optionally notify the user
                                        TempData["ErrorMsg"] = "An unexpected error occurred. Please try again later.";
                                    }
                                }
                                else
                                {
                                    TempData["ErrorMsg"] = "This Apllication is Not Trusted App.";
                                    return RedirectToAction("Login", "Login");
                                }

                            }
                            if (globalModel.ApplicationId == 1) //Hrms
                            {



                                if (IsTrustedUrl(applicationLink))
                                {

                                    // Send the request
                                    try
                                    {
                                        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, applicationLink + "receivedata"))
                                        {
                                            using (var httpClient = new HttpClient())
                                            {
                                                // Add the OAuth header
                                                requestMessage.Headers.Add("OAuth", HttpContext.Session.GetString("UserID"));

                                                // Create and set the content
                                                var jsonContent = new { EncOAuth = encOAuth };
                                                var jsonString = JsonConvert.SerializeObject(jsonContent);
                                                requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                                                // Send the HTTP POST request asynchronously
                                                var result = await httpClient.SendAsync(requestMessage);

                                                if (result.IsSuccessStatusCode)
                                                {
                                                    // Redirect the user to the dashboard
                                                    var userId = HttpContext.Session.GetString("UserID");
                                                    return Redirect(applicationLink + "DashBoard?OAuth=" + Uri.EscapeDataString(userId));
                                                }
                                                else
                                                {
                                                    TempData["ErrorMsg"] = "Application Not Found";
                                                    return RedirectToAction("Login", "Login");
                                                }

                                            }


                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        // Optionally notify the user
                                        TempData["ErrorMsg"] = "An unexpected error occurred. Please try again later.";
                                    }
                                }
                                else
                                {
                                    TempData["ErrorMsg"] = "This Apllication is Not Trusted App.";
                                    return RedirectToAction("Login", "Login");
                                }
                            }
                            if (globalModel.ApplicationId == 3) //Inventory
                            {
                                using (var httpClient = new HttpClient())
                                {
                                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, applicationLink + "receivedata");
                                    requestMessage.Headers.Add("OAuth", encOAuth);

                                    if (IsTrustedUrl(applicationLink))
                                    {
                                        // Send the request
                                        try
                                        {
                                            // Send the HTTP POST request asynchronously
                                            var result = await httpClient.SendAsync(requestMessage);
                                            if (result.IsSuccessStatusCode)
                                            {
                                                // Redirect the user to the application link
                                                return Redirect(applicationLink + "DashBoard/" + HttpContext.Session.GetString("UserID"));
                                            }
                                            else
                                            {
                                                TempData["ErrorMsg"] = "Application Not Found";
                                                return RedirectToAction("Login", "Login");
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            // Optionally notify the user
                                            TempData["ErrorMsg"] = "An unexpected error occurred. Please try again later.";
                                        }


                                    }
                                    else
                                    {
                                        TempData["ErrorMsg"] = "This Apllication is Not Trusted App.";
                                        return RedirectToAction("Login", "Login");
                                    }
                                }

                            }


                            if (globalModel.ApplicationId == 4) //Hrms
                            {



                                if (IsTrustedUrl(applicationLink))
                                {
                                    // Send the request
                                    try
                                    {

                                        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, applicationLink + "receivedata"))
                                        {
                                            using (var httpClient = new HttpClient())
                                            {
                                                // Add the OAuth header
                                                requestMessage.Headers.Add("OAuth", HttpContext.Session.GetString("UserID"));

                                                // Create and set the content
                                                var jsonContent = new { EncOAuth = encOAuth };
                                                var jsonString = JsonConvert.SerializeObject(jsonContent);
                                                requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                                                // Send the HTTP POST request asynchronously
                                                var result = await httpClient.SendAsync(requestMessage);

                                                if (result.IsSuccessStatusCode)
                                                {
                                                    // Redirect the user to the dashboard
                                                    var userId = HttpContext.Session.GetString("UserID");
                                                    return Redirect(applicationLink + "DashBoard?OAuth=" + Uri.EscapeDataString(userId));
                                                }
                                                else
                                                {
                                                    TempData["ErrorMsg"] = "Application Not Found";
                                                    return RedirectToAction("Login", "Login");
                                                }

                                            }


                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        // Optionally notify the user
                                        TempData["ErrorMsg"] = "An unexpected error occurred. Please try again later.";
                                    }
                                }
                                else
                                {
                                    TempData["ErrorMsg"] = "This Apllication is Not Trusted App.";
                                    return RedirectToAction("Login", "Login");
                                }
                            }

                            if (globalModel.ApplicationId == 6) //Logistics-Dev
                            {
                                if (IsTrustedUrl(applicationLink))
                                {
                                    try
                                    {
                                        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, applicationLink + "receivedata"))
                                        {
                                            using (var httpClient = new HttpClient())
                                            {
                                                // Add the OAuth header
                                                requestMessage.Headers.Add("OAuth", HttpContext.Session.GetString("UserID"));

                                                // Create and set the content
                                                var jsonContent = new { EncOAuth = encOAuth };
                                                var jsonString = JsonConvert.SerializeObject(jsonContent);
                                                requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                                                // Send the HTTP POST request asynchronously
                                                var result = await httpClient.SendAsync(requestMessage);

                                                if (result.IsSuccessStatusCode)
                                                {
                                                    // Redirect the user to the dashboard
                                                    var userId = HttpContext.Session.GetString("UserID");
                                                    return Redirect(applicationLink + "DashBoard?OAuth=" + Uri.EscapeDataString(userId));
                                                }
                                                else
                                                {
                                                    TempData["ErrorMsg"] = "Application Not Found";
                                                    return RedirectToAction("Login", "Login");
                                                }

                                            }


                                        }

                                    }


                                    catch (Exception ex)
                                    {
                                        // Optionally notify the user
                                        TempData["ErrorMsg"] = "An unexpected error occurred. Please try again later.";
                                    }
                                }
                                else
                                {
                                    TempData["ErrorMsg"] = "This Apllication is Not Trusted App.";
                                    return RedirectToAction("Login", "Login");
                                }

                            }




                            if (globalModel.ApplicationId == 5) //Hrms
                            {



                                if (IsTrustedUrl(applicationLink))
                                {
                                    // Send the request
                                    try
                                    {
                                        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, applicationLink + "receivedata"))
                                        {
                                            using (var httpClient = new HttpClient())
                                            {
                                                // Add the OAuth header
                                                requestMessage.Headers.Add("OAuth", HttpContext.Session.GetString("UserID"));

                                                // Create and set the content
                                                var jsonContent = new { EncOAuth = encOAuth };
                                                var jsonString = JsonConvert.SerializeObject(jsonContent);
                                                requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                                                // Send the HTTP POST request asynchronously
                                                var result = await httpClient.SendAsync(requestMessage);

                                                if (result.IsSuccessStatusCode)
                                                {
                                                    // Redirect the user to the dashboard
                                                    var userId = HttpContext.Session.GetString("UserID");
                                                    return Redirect(applicationLink + "DashBoard?OAuth=" + Uri.EscapeDataString(userId));
                                                }
                                                else
                                                {
                                                    TempData["ErrorMsg"] = "Application Not Found";
                                                    return RedirectToAction("Login", "Login");
                                                }

                                            }


                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        // Optionally notify the user
                                        TempData["ErrorMsg"] = "An unexpected error occurred. Please try again later.";
                                    }
                                }
                                else
                                {
                                    TempData["ErrorMsg"] = "This Apllication is Not Trusted App.";
                                    return RedirectToAction("Login", "Login");
                                }
                            }
                        }
                    }
                }
                else
                {
                    TempData["ErrorMsg"] = "No company data available.";
                }
            }
            return RedirectToAction("Login", "Login");
        }

        private bool IsTrustedUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            // Allow local URLs
            if (Url.IsLocalUrl(url))
                return true;

            // Normalize the URL by removing trailing slashes
            url = url.TrimEnd('/');

            // Create a Uri object
            var uri = new Uri(url);

            // Handle default ports
            int port = uri.IsDefaultPort ? (uri.Scheme == Uri.UriSchemeHttps ? 443 : 80) : uri.Port;

            bool b = _trustedHosts.Contains(uri.Host + ":" + port);
            // Return true if the host and port are in the list of trusted hosts
            return b;
        }

        // Method to encrypt the JSON string using AES and then encode it in Base64
        private static string EncryptToBase64(object globalModel, string key)
        {
            // Step 1: Serialize the globalModel to JSON
            string jsonString = JsonConvert.SerializeObject(globalModel);

            // Convert the JSON string to bytes
            byte[] plainBytes = Encoding.UTF8.GetBytes(jsonString);

            // Convert the key to bytes
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            // Ensure the key length is valid for AES (16, 24, or 32 bytes)
            byte[] keyBytes32 = new byte[32];
            Array.Copy(keyBytes, keyBytes32, Math.Min(keyBytes.Length, keyBytes32.Length));

            // Step 2: Encrypt the byte array using AES
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes32;
                aes.GenerateIV(); // Generate a random IV for each encryption

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    // Write the IV at the beginning of the stream
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(plainBytes, 0, plainBytes.Length);
                    }

                    // Convert the encrypted data to Base64
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }


        public async Task<bool> UrlStructure()
        {
            // Check if the host is localhost
            var isLocalhost = Request.Host.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase);

            // Log the result (optional)
            Console.WriteLine($"Is Localhost: {isLocalhost}");

            return isLocalhost; // Return true if localhost, false otherwise
        }












        public async Task<List<SaasBillingEmpModel>> FetchEmployeeDataFromApi(SaasBillingEmpModel model)
        {
            string urlparameters = "PrintEmpInfo";
            using (var httpClient = new HttpClient())
            {
                try
                {
                    string url = _baseUrlempinfo + urlparameters;

                    // 🔹 Ensure API key is sent correctly
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    var jsonContent1 = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync(url, jsonContent1);


                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<SaasBillingEmpModel>>(responseContent);
                    }
                    else
                    {
                        Console.WriteLine($"Error calling API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error fetching data from API: " + ex.Message);
                }
            }
            return new List<SaasBillingEmpModel>();
        }





        public (byte[] pdfBytes, string fileName) GenerateEmployeePdf(List<SaasBillingEmpModel> dataList)
        {
            try
            {
                FastReport.Utils.Config.WebMode = true;
                FastReport.Report rep = new FastReport.Report();

                // Load the report template (no file creation, just in-memory processing)
                string path = Path.Combine(this._webHostEnvironment.WebRootPath, "Reports", "RPT_Employee_information_Report.frx");
                rep.Load(path);

                // Register data source
                rep.Report.RegisterData(dataList, "TBl_EmployeeInfo_ref");

                using (MemoryStream ms = new MemoryStream())
                {
                    // Prepare the report (no need for creating files)
                    if (rep.Prepare())
                    {
                        // Create PDF export
                        FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport
                        {
                            ShowProgress = false,  // Disable progress display
                            Subject = "Employee Information Report",  // Optional PDF subject
                            Title = "Employee Information Report"  // Optional PDF title
                        };

                        // Export directly to MemoryStream
                        rep.Export(pdfExport, ms);

                        // Generate the filename (you can customize this as needed)
                        string fileName = $"Employee_Report_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.pdf";

                        // Return the PDF bytes and the filename
                        return (ms.ToArray(), fileName);
                    }
                }

                // Return null if something goes wrong
                return (null, null);
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., to a logging system)
                Console.WriteLine($"❌ Error generating PDF: {ex.Message}");
                return (null, null);
            }
        }




    }
}
