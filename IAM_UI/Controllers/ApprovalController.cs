using CommonUtility.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using IAM_UI.Models;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Net.Http;
using System.Reflection;
using System.Net;
using System.Diagnostics;
using CommonUtility.SharedModels;
using IAM_UI.Helpers;

namespace IAM_UI.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _baseUrlGlobal;
        private readonly ICommonService _commonService;
        private readonly string _ApiKey;
        private readonly string _MailBody;
        private readonly string BaseUrlApproval;
        private readonly IGlobalModelService _globalModelService;

        // private readonly AesHmacEncryption _enc;

        public ApprovalController(IConfiguration configuration, ICommonService commonService, IGlobalModelService globalModelService)
        {
            _configuration = configuration;
          //  _enc = new AesHmacEncryption(_configuration);
            _baseUrlGlobal = configuration["BaseUrlGlobal"];
            _commonService = commonService;
            _ApiKey = configuration["Apikey"];
            _MailBody = configuration["MailBody"];
            BaseUrlApproval = configuration["BaseUrlApproval"];
            _globalModelService = globalModelService;

        }


        #region Approval
        [HttpGet]
        public async Task<IActionResult> Approval()

        {
            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
            try
            {
                string urlparameters = "FECTCH_APPROVAL_LEVEL_ONEDETAILS";

                var httpclient = new HttpClient();
                httpclient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));


                var response = await httpclient.GetAsync(BaseUrlApproval + urlparameters);
                if (response.IsSuccessStatusCode)
                {

                    var respondata = await response.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<ApprovalModel>>(respondata);

                    ViewBag.EnquiryList = list;
                    return View();
                }
                else
                {
                    //return View();
                   return View("error");
                }

            }
            catch (Exception ex)
            {

                
                throw;
            }


        }
        [HttpPost]
        public async Task<JsonResult> FetchCompanyName(int COMPANY_KEY)
        {
            string urlparameters = "FetchCompanyName";
            try
            {
                using (var httpClient = new HttpClient())

                {
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));

                    string apiurl = BaseUrlApproval + urlparameters;
                    HttpResponseMessage response = await httpClient.GetAsync(apiurl);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        return Json(responseData);
                    }
                    else
                    {
                        return Json("Error");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve CompanyList.");
            }


        }

        [HttpGet]
        public async Task<IActionResult> _ApprovalView(int ApprovalLevelKey)
        {
            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

            try
            {
                var httpclient = new HttpClient();
                httpclient.DefaultRequestHeaders.Clear();
                httpclient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                string urlparametrs2 = "FetchApplicationtype";
                string urlParameters = "FetchApplicationNames";
                string urlParameters4 = "FetchUserType";
                string urlparametrs5 = "Fetch_Operational_Mode";


                

                var response = await httpclient.GetAsync(BaseUrlApproval + urlParameters);
                var response2 = await httpclient.GetAsync(BaseUrlApproval + urlparametrs2);
                var response4 = await httpclient.GetAsync(BaseUrlApproval + urlParameters4);
                var response5 = await httpclient.GetAsync(BaseUrlApproval + urlparametrs5);

                if (response.IsSuccessStatusCode)
                {

                    var responseData = await response.Content.ReadAsStringAsync();
                     

                
                    var responsedata2 = await response2.Content.ReadAsStringAsync();
                    var responsedata4 = await response4.Content.ReadAsStringAsync();
                    var responsedata5 = await response5.Content.ReadAsStringAsync();

                    var pDataTypeList = JsonConvert.DeserializeObject<List<ApprovalModel>>(responseData);
                    var typelist = JsonConvert.DeserializeObject<List<ApprovalModel>>(responsedata2);
                    var userlist = JsonConvert.DeserializeObject<List<ApprovalModel>>(responsedata4);
                    var OperationalModeList = JsonConvert.DeserializeObject<List<ApprovalModel>>(responsedata5);

                    ViewBag.ApplicationNameList = pDataTypeList;
                    ViewBag.ApplicationTypelist = typelist;
                    ViewBag.user = userlist;
                    ViewBag.OprModeList = OperationalModeList;


                    if (!string.IsNullOrEmpty(HttpContext.Request.Query["ApprovalLevelKey"]))
                    {
                        var AppId = ApprovalLevelKey;

                        string urlParameters1 = "Getapprovalleveldetails_id/";
                     

                        using (var httpClient2 = new HttpClient())
                        

                        {
                            httpClient2.DefaultRequestHeaders.Clear();  
                            httpClient2.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                            var response11 = await httpClient2.GetAsync(BaseUrlApproval + urlParameters1+ Convert.ToInt32(AppId));
                            if (response11.IsSuccessStatusCode)
                            {

                                string data = response11.Content.ReadAsStringAsync().Result;
                                List<ApprovalModel> lst = JsonConvert.DeserializeObject<List<ApprovalModel>>(data);

                                // Return the JSON response directly as JSON data
                                var info = lst.FirstOrDefault();
                                 return Json(info);
                                
                            }
                            else
                            {
                          
                            }


                        }

                    }



                    return View();
                   // return PartialView("_ApprovalView");
                }


                else
                {
                   
                    return View("Error");
                }

            }
            catch (Exception ex)
            {

                //SERILOG
                // Log.Error(ex, "An unhandled exception occurred in _ApprovalView action");
                throw;
            }



        }

        [HttpPost]
        public async Task<JsonResult> GetModulenames(string RecType, int Application_Name_Id, int Menu_Id)
        {
            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
            try
            {
                

                string urlparameters = "FetchApplicationMainmenu/" + RecType + "/" + Application_Name_Id.ToString()+"/"+ Menu_Id.ToString();
                var httpclient = new HttpClient();
                httpclient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));

                //var dataToSerialize = new { EmployeeReportingBoss = model.EmployeeReportingBoss };

                //var content = new StringContent(JsonConvert.SerializeObject(Application_Name_Id), Encoding.UTF8, "application/json");

                var response = await httpclient.GetAsync(BaseUrlApproval + urlparameters);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var pDataTypeList = JsonConvert.DeserializeObject<List<ApprovalModel>>(responseData);

                    //var result = new { responseData };

                    return Json(pDataTypeList);
                }
                else
                {
                    //SERILOG
                    // Log.Error("Request to {BaseUrl}{urlParameters} failed with status code {StatusCode}", BaseUrlApproval, urlParameters, Response.StatusCode);
                    return Json("Error");
                }
            }
            catch (Exception ex)
            {

                //SERILOG
                // Log.Error(ex, "An unhandled exception occurred in GetModulenames action");
                throw;
            }

        }
        [HttpPost]
        public async Task<JsonResult> SaveApprovalSetupLabelOne(ApprovalModel model)
        {
            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
            try
            {
                string msg = "";
                int info;
                string urlparameters = "SaveApproval_Level_One";
                model.TenantID = Convert.ToInt32(gm.TenantID);   
                model.UserKey = Convert.ToInt32(gm.userID);

                using (var httpclient = new HttpClient())

                {
                    httpclient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    StringContent Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpclient.PostAsync(BaseUrlApproval + urlparameters, Content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responsedata = await response.Content.ReadAsStringAsync();
                        info = Convert.ToInt32(responsedata);
                        if (info > 0)
                        {
                            msg = "Success";
                        }
                        else
                        {
                            msg = "Fail";
                        }
                        var result = new { msg, info };

                        return Json(result);
                    }
                    else
                    {
                        return Json("Error");
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }


        }


        [HttpGet]
        public async Task<JsonResult> FetchdataApproval_level_two( int Approval_level_One_key)
        {
            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
            try
            {
                string urlparametrs = "FECTCH_APPROVAL_LEVEL_TWODETAILS/"+Approval_level_One_key;
                var httpclient = new HttpClient();
                httpclient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));

                var response = await httpclient.GetAsync(BaseUrlApproval + urlparametrs);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    //var result = new { responseData };

                    return Json(responseData);
                }
                else
                { //SERILOG
                  // Log.Error("Request to {BaseUrl}{urlParameters} failed with status code {StatusCode}", BaseUrlApproval, urlParameters, Response.StatusCode);
                    return Json("Error");
                }

            }
            catch (Exception ex)
            {

                //SERILOG
                //Log.Error(ex, "An unhandled exception occurred in FetchdataApproval_level_two action");

                throw;
            }



        }

        [HttpGet]
        public async Task<JsonResult> UserType()
        {
            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
            try
            {
                var httpclient = new HttpClient();
                httpclient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                string urlparametrs = "FetchUserType";
                var response = await httpclient.GetAsync(BaseUrlApproval + urlparametrs);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var UserTypeList = JsonConvert.DeserializeObject<List<ApprovalModel>>(responseData);
                    // ViewBag.UserList = UserTypeList;
                    return Json(UserTypeList);
                }
                else
                {
                    //SERILOG
                    // Log.Error("Request to {BaseUrl}{urlParameters} failed with status code {StatusCode}", BaseUrlApproval, urlParameters, Response.StatusCode);
                    return Json("Error");
                }
            }
            catch (Exception ex)
            {

                //SERILOG
                // Log.Error(ex, "An unhandled exception occurred in UserType action");
                throw;
            }

        }
        [HttpPost]
        public async Task<JsonResult> SaveApprovalSetupTwo(ApprovalModel model)
        {
            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

            model.TenantID = Convert.ToInt32(gm.TenantID);
            model.UserKey = Convert.ToInt32(gm.userID);

            try
            {
                string msg = "";
                int info;
                string urlparameters = "SaveApproval_Level_two";


                using (var httpclient = new HttpClient())

                {
                    httpclient.DefaultRequestHeaders.Clear();
                    httpclient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    StringContent Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpclient.PostAsync(BaseUrlApproval + urlparameters, Content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responsedata = await response.Content.ReadAsStringAsync();
                        info = Convert.ToInt32(responsedata);
                       
                            var result = new { id = info };
                            return Json(result);
                        
                    }
                    else
                    {
                        return Json("Error");
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        [HttpGet]
        public async Task<JsonResult> FetchdataApproval_level_three(int Approval_level_One_key,int CompanyID)
        {
            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

            try
            {
                string urlparametrs = "FetchApprovalDetails_level_three/"+ Approval_level_One_key;
                var httpclient = new HttpClient();
                httpclient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));


                //var dataToSerialize = new { Approval_level_One_key = model.Approval_level_One_key };

                //var content = new StringContent(JsonConvert.SerializeObject(dataToSerialize), Encoding.UTF8, "application/json");

                var response = await httpclient.GetAsync(BaseUrlApproval + urlparametrs+"/"+ CompanyID);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    //var result = new { responseData };

                    return Json(responseData);
                }
                else
                {
                    //SERILOG
                    // Log.Error("Request to {BaseUrl}{urlParameters} failed with status code {StatusCode}", BaseUrlApproval, urlParameters, Response.StatusCode);
                    return Json("Error");
                }

            }
            catch (Exception ex)
            {

                //SERILOG
                //Log.Error(ex, "An unhandled exception occurred in FetchdataApproval_level_three action");
                throw;
            }


        }

        [HttpGet]
        public async Task<JsonResult> GetSelectedCompany(int LevelOneId)

        {
            string urlParameters = "GetSelectedCompany/";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));

            var response = await httpClient.GetAsync(BaseUrlApproval + urlParameters + LevelOneId);

            if (response.IsSuccessStatusCode)
            {

                var responseData = await response.Content.ReadAsStringAsync();
                var pDataTypeList = JsonConvert.DeserializeObject<List<ApprovalModel>>(responseData);


                return Json(pDataTypeList);
            }
            else
            {
                // Handle the error response
                var responseData = await response.Content.ReadAsStringAsync();
                var errorData = await response.Content.ReadAsStringAsync();
                var result = new { id = errorData };
                return Json(result);
            }


        }



        [HttpPost]

        public async Task<JsonResult> GetUserDetails(string prefix,int UserType_Key,int Company_Key)
        {
            string urlparameters = "Employee/";
            List<ApprovalModel> lst = new List<ApprovalModel>();
           
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));

                //ApprovalModel model = new ApprovalModel();
                //model.Employee_Name = prefix;
                //model.Company_Key = Company_Key;
                //string jsonData = JsonConvert.SerializeObject(model);
                string url = BaseUrlApproval + urlparameters + prefix+"/"+ UserType_Key +"/"+ Company_Key;
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the API response to your desired data model or use dynamic if the structure is unknown.
                    string responseContent = await response.Content.ReadAsStringAsync();

                    lst = JsonConvert.DeserializeObject<List<ApprovalModel>>(responseContent);



                    return Json(lst);
                }
                else
                {
                    return Json("Error");
                }
            }

        }
        [HttpGet]
        public async Task<IActionResult> DeleteApprovalStep(int Approval_level_One_key,int Approval_level_Two_key)
        {
            try
            {
                string url = "DeleteApprovalStep/"+ Approval_level_One_key;
                string msg = "";
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage responseMsg = await httpClient.GetAsync(BaseUrlApproval + url +"/"+ Approval_level_Two_key);

                    if (responseMsg.IsSuccessStatusCode)
                    {
                        var responseData = await responseMsg.Content.ReadAsStringAsync();
                        if (int.TryParse(responseData, out int apiResponse))
                        {
                            if (apiResponse > 0)
                            {
                                msg = "success";
                            }
                            else
                            {
                                msg = "fail";
                            }
                            return Json(msg);
                        }
                        else
                        {
                            return Json("error");
                        }
                    }
                    else
                    {
                        var errorData = await responseMsg.Content.ReadAsStringAsync();
                        var result = new { id = errorData };
                        return Json("Error");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public async Task<JsonResult> SaveApprovalSetupThree(ApprovalModel model)
        {
            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

            model.TenantID = Convert.ToInt32(gm.TenantID);
            model.UserKey = Convert.ToInt32(gm.userID);

            try
            {
                string msg = "";
                int info;
                string urlparameters = "SaveApprovalSetupThree";


                using (var httpclient = new HttpClient())

                {
                    httpclient.DefaultRequestHeaders.Clear();
                    httpclient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    StringContent Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpclient.PostAsync(BaseUrlApproval + urlparameters, Content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responsedata = await response.Content.ReadAsStringAsync();
                        info = Convert.ToInt32(responsedata);

                        var result = new { id = info };
                        return Json(result);

                    }
                    else
                    {
                        return Json("Error");
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        [HttpGet]
        public async Task<JsonResult> Fetch_L3_Dtls(int Approval_level_One_key)
        {
            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

            try
            {
                string urlparametrs = "Fetch_L3_Dtls/" + Approval_level_One_key;
                var httpclient = new HttpClient();
                httpclient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));


                //var dataToSerialize = new { Approval_level_One_key = model.Approval_level_One_key };

                //var content = new StringContent(JsonConvert.SerializeObject(dataToSerialize), Encoding.UTF8, "application/json");

                var response = await httpclient.GetAsync(BaseUrlApproval + urlparametrs);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    //var result = new { responseData };

                    return Json(responseData);
                }
                else
                {
                    //SERILOG
                    // Log.Error("Request to {BaseUrl}{urlParameters} failed with status code {StatusCode}", BaseUrlApproval, urlParameters, Response.StatusCode);
                    return Json("Error");
                }

            }
            catch (Exception ex)
            {

                //SERILOG
                //Log.Error(ex, "An unhandled exception occurred in FetchdataApproval_level_three action");
                throw;
            }


        }

        #endregion
    }
}
