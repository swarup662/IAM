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
using System.Diagnostics.Contracts;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using IAM_UI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Web;
using IAM_UI;

namespace IAM_UI.Controllers
{
    public class ItController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerService _logger;
        private readonly string _baseUrlGlobal;
        private readonly ICommonService _commonService;
        private readonly string _ApiKey;
        private readonly string _MailBody;
        private readonly string BaseUrlAuth;
        private readonly string BaseUrlPers;
        private readonly string BaseUrlOldPassword;
        private readonly IGlobalModelService _globalModelService;

        private readonly APIResultsValue _apirelultvalues;
        private readonly string BaseUrlAllModule;

        public ItController(IConfiguration configuration, ICommonService commonService, ILoggerService logger, IGlobalModelService globalModelService, APIResultsValue apirelultvalues)
        {
            _configuration = configuration;
            _baseUrlGlobal = configuration["BaseUrlGlobal"];
            _commonService = commonService;
            _ApiKey = configuration["Apikey"];
            _MailBody = configuration["MailBody"];
            _logger = logger;
            BaseUrlAuth = configuration["BaseUrlAuth"];
            BaseUrlPers = configuration["BaseUrlPersonnel"];

            BaseUrlOldPassword = configuration["BaseUrlLogin"];
            _globalModelService = globalModelService;
            _apirelultvalues = apirelultvalues;
            BaseUrlAllModule = configuration["AllModuleGridApproval"];
        }




        public async Task<IActionResult> RoleWiseApplication()
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

                string urlParameters = "ItGetRoleWiseApplication/";
                using (var httpClient = new HttpClient())
                {
                    gm.Data = "";

                    string jsonBody = JsonConvert.SerializeObject(gm);

                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    string url = _baseUrlGlobal + urlParameters;


                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);  //API Hit From Here

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();





                        HttpContext.Session.SetString("RoleWiseApplicationeDataset", responseContent);


                        Dictionary<string, string> Get_all_data = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);


                        Dictionary<string, string> CommonGridData = new Dictionary<string, string>();

                        //CommonGridData.Add("list", Get_all_data["grid"]);

                        ViewBag.CommonGridData = JsonConvert.DeserializeObject<DataTable>(Get_all_data["grid"]);

                        DataSet ds = JsonConvert.DeserializeObject<DataSet>(Get_all_data["dataset"]);

                        ViewBag.UserType = await _commonService.GetSelectListAsync(ds.Tables["UserType"], "UserTypeId", "UserTypeName", "----Select UserType-----", "0");

                        ViewBag.Application = await _commonService.GetSelectListAsync(ds.Tables["Application"], "ApplicationId", "ApplicationName", "----Select Application-----", "0");
                        ViewBag.Company = await _commonService.GetSelectListAsync(ds.Tables["Company"], "CompanyId", "CompanyName", "----Select Company-----", "0");


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







        public async Task<IActionResult> GetModuleAccess(string CompanyId, string application_ID, string UserTypeId)
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

                List<string> App_user = new List<string> { application_ID, UserTypeId, CompanyId , gm.TenantID.ToString() };

                string urlParameters = "";
                //if (Convert.ToInt32(UserTypeId) > 0 && Convert.ToInt32(UserTypeId) > 0   )
                //{


                urlParameters = "EditRoleWiseApplication/";
                gm.Data = JsonConvert.SerializeObject(App_user);
                //}
                //else {

                //     urlParameters = "GetModuleAccess/";
                //    gm.Data = JsonConvert.SerializeObject(App_user);

                //}

                using (var httpClient = new HttpClient())
                {


                    string jsonBody = JsonConvert.SerializeObject(gm);

                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    string url = _baseUrlGlobal + urlParameters;


                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);  //API Hit From Here

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return Json(responseContent);

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




        public async Task<IActionResult> SaveRoleWiseApplication(string CompanyId, string UserTypeId, string[] arrModule, string ApplicationID)

        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);


                var result = "";
                RoleWiseApplicationModel bModel = new RoleWiseApplicationModel();
                var urlParameters = "SaveRoleWiseApplication";


                // Create a DataTable to hold the data
                DataTable dt = new DataTable();

                // Define the columns in the DataTable

                dt.Columns.Add("TenantId", typeof(int));//this id is hardcode later it will come from session or cache
                dt.Columns.Add("CompanyId", typeof(int));
                dt.Columns.Add("UserTypeId", typeof(int));
                dt.Columns.Add("ModuleId", typeof(int));
                dt.Columns.Add("ActionId", typeof(int));
                dt.Columns.Add("ApplicationID", typeof(int));
                dt.Columns.Add("CreatedBy", typeof(int));//this id is hardcode later it will come from session or cache

                foreach (string str in arrModule)
                {
                    string[] arrS = str.Split('_');
                    string module = arrS[1];


                    if (!arrS[0].Contains(","))
                    {
                        if (!arrS[0].Contains("deny"))
                        {
                            string getAccess = arrS[0].ToString();

                            // Create a new DataRow
                            DataRow row = dt.NewRow();

                            // Set the values for each column
                            row["TenantId"] = Convert.ToInt32(2);
                            row["CompanyId"] = Convert.ToInt32(CompanyId);
                            row["UserTypeId"] = Convert.ToInt32(UserTypeId);
                            row["ModuleId"] = Convert.ToInt32(module);
                            row["ActionId"] = Convert.ToInt32(getAccess);
                            row["ApplicationID"] = Convert.ToInt32(ApplicationID);
                            row["CreatedBy"] = Convert.ToInt32(gm.userID);
                            // Add the row to the DataTable
                            dt.Rows.Add(row);






                        }

                    }

                }





                using (var httpClient = new HttpClient())
                {
                    gm.Data = JsonConvert.SerializeObject(dt);
                    string jsonData = JsonConvert.SerializeObject(gm);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(_baseUrlGlobal + urlParameters, content);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadAsStringAsync();
                        int r = Convert.ToInt32(result);
                        if (r > 0)
                        {
                            TempData["MSG"] = "success";

                        }
                        else if (r == -1)
                        {
                            TempData["MSG"] = "exist";

                        }
                        else
                        {
                            TempData["MSG"] = "Fail";
                        }

                        return Redirect("/Authorization/RoleWiseApplication");

                    }
                    else
                    {
                        return Json("Error");
                    }

                }




            }


            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<IActionResult> EditRoleWiseApplication(int UserTypeId, int CompanyId, int ApplicationId)
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

                string urlParameters = "EditRoleWiseApplication/";
                using (var httpClient = new HttpClient())
                {
                    List<string> App_user = new List<string> { ApplicationId.ToString(), UserTypeId.ToString(), CompanyId.ToString() , gm.TenantID.ToString() };
                    gm.Data = JsonConvert.SerializeObject(App_user);


                    string jsonBody = JsonConvert.SerializeObject(gm);

                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    string url = _baseUrlGlobal + urlParameters;



                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);  //API Hit From Here

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();


                        return Json(responseContent);

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



        public async Task<IActionResult> DeleteRoleWiseApplication(int UserTypeId, int CompanyId, int ApplicationId)
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

                string urlParameters = "DeleteRoleWiseApplication/";
                using (var httpClient = new HttpClient())
                {

                    List<string> App_user = new List<string> { ApplicationId.ToString(), UserTypeId.ToString(), CompanyId.ToString() };
                    gm.Data = JsonConvert.SerializeObject(App_user);

                    string jsonBody = JsonConvert.SerializeObject(gm);

                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    string url = _baseUrlGlobal + urlParameters;



                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);  //API Hit From Here

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        int r = JsonConvert.DeserializeObject<int>(responseContent);

                        return Json(r);

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




    }
}
