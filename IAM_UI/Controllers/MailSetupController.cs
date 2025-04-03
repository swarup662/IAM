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
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.Mail;


namespace IAM_UI.Controllers
{
    public class MailSetupController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrlMail;
        private readonly ICommonService _commonService;
        private readonly string _ApiKey;
        private readonly string _MailBody;
        private readonly string BaseUrlApproval;
        private readonly IGlobalModelService _globalModelService;
        private readonly string _baseUrlGlobal;

        public MailSetupController(IConfiguration configuration, ICommonService commonService, IGlobalModelService globalModelService, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            //  _enc = new AesHmacEncryption(_configuration);
            _baseUrlMail = configuration["BaseUrlMail"];
            _commonService = commonService;
            _ApiKey = configuration["Apikey"];
            _MailBody = configuration["MailBody"];
            _globalModelService = globalModelService;
            _webHostEnvironment = webHostEnvironment;
            _baseUrlGlobal = configuration["BaseUrlGlobal"];


        }

        public async Task<IActionResult> Index()
        {
            try
            {
      
                var UserData = JsonConvert.DeserializeObject<UserDetail>(HttpContext.Session.GetString("UserData"));
                var model = new MailSetupModel();

                model.Tenant_ID = UserData.TenantId;


                string urlParameter = "GetData";
                string urlParameters = "GetApplicationList";

                using (var httpClient = new HttpClient())
                {
                    string url = _baseUrlMail + urlParameters;
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(url, jsonContent);

                    string url1 = _baseUrlMail + urlParameter;
                    var jsonContent1 = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage response1 = await httpClient.PostAsync(url1, jsonContent1);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContents = await response1.Content.ReadAsStringAsync();
                        List<MailSetupModel> Purposetb = JsonConvert.DeserializeObject<List<MailSetupModel>>(responseContents);
                        ViewBag.Purposetb = Purposetb;



                        string responseContent = await response.Content.ReadAsStringAsync();
                        DataSet dt = JsonConvert.DeserializeObject<DataSet>(responseContent);

                        ViewBag.Applicationtb = await _commonService.GetSelectListAsync(dt.Tables["Applicationtb"], "ApplicationId", "ApplicationName", "--Select--", "");


          
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





        public async Task<JsonResult> GetPurposeById(int ApplicationId)
        {
                var UserData = JsonConvert.DeserializeObject<UserDetail>(HttpContext.Session.GetString("UserData"));
                var model = new MailSetupModel
                {
                    Tenant_ID = UserData.TenantId,
                    Application_ID = Convert.ToInt32(UserData.AppID),
                    PurposeApplicationId = ApplicationId,
                };

                string urlparameters = "GetPurposeById";
                using (var httpClient = new HttpClient())
                {
                    string url = _baseUrlMail + urlparameters;
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(url, jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return Json(responseContent);  // ✅ Ensure JSON format
                    }
                    else
                    {
                        return Json("Error");  // ✅ Return JSON
                    }
                }
        }

        public async Task<IActionResult> EditMailSetup(int id)
        {

            var UserData = JsonConvert.DeserializeObject<UserDetail>(HttpContext.Session.GetString("UserData"));
            var model = new MailSetupModel
            {
                Tenant_ID = UserData.TenantId,
                Created_By = UserData.User_Master_Key,
                TenantMailSetupKey = id,
            };

            string urlParameters = "EditMailSetup";
            using (var httpClient = new HttpClient())
            {
                string url = _baseUrlMail + urlParameters;
                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                HttpResponseMessage response = await httpClient.PostAsync(url, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    MailSetupModel lst = JsonConvert.DeserializeObject<MailSetupModel>(data);


                    return Json(lst);
                }
                else
                {
                    return Json("Error");
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveMailSetup([FromBody] MailSetupModel model)
        {
         try {
      
                var UserData = JsonConvert.DeserializeObject<UserDetail>(HttpContext.Session.GetString("UserData"));

            model.IsCC = 0; //no cc for any mail
            model.TimeSlotMinute = 60; //fixed
            model.Tenant_ID = UserData.TenantId;
            model.Created_By = UserData.User_Master_Key;


            string urlParameters = "SaveMailSetup/";
            string jsonBody = JsonConvert.SerializeObject(model);

            using (var content = new StringContent(jsonBody, Encoding.UTF8, "application/json"))
            {
                string url = _baseUrlMail + urlParameters;

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responsedata = await response.Content.ReadAsStringAsync();

                        if (int.TryParse(responsedata, out int r))
                        {


                            if (r > 0)
                                return Json(new { status = "success" });
                            else if (r == -1)
                                return Json(new { status = "exist" });
                            else
                                return Json(new { status = "fail" });
                        }
                        else
                        {
                            return Json(new { status = "error", message = "Invalid response from server." });
                        }
                    }
                    else
                    {
                        return Json(new { status = "Invalid" });
                    }
                }
            }


             }
            catch (Exception ex)
            {
                throw ex;
            }

        }




        //For Delete function
        public async Task<IActionResult> DeleteMailSetup(int id)
        {
            var UserData = JsonConvert.DeserializeObject<UserDetail>(HttpContext.Session.GetString("UserData"));
            var model = new MailSetupModel
            {
                Tenant_ID = UserData.TenantId,
                Application_ID = Convert.ToInt32(UserData.AppID),
                Created_By = UserData.User_Master_Key,
                TenantMailSetupKey = id,
            };

            string msg = "";
            string urlParameters = "DeleteMailSetup";
            using (var httpClient = new HttpClient())
            {
                string url = _baseUrlMail + urlParameters;
                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                HttpResponseMessage response = await httpClient.PostAsync(url, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (int.TryParse(responseContent, out int apiResponse))
                    {
                        if (apiResponse > 0)
                        {
                            msg = "success";
                        }
                        else
                        {
                            msg = "Fail";
                        }

                        return Json(msg);
                    }
                    else
                    {
                        return Json("Error");
                    }
                }
                else
                {
                    return Json("Error");
                }
            }
        }





    }
}
