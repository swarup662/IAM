using CommonUtility.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Text; 
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using IAM_UI.Models;
using System.Data;


using System.Dynamic;

using CommonUtility.SharedModels;

using IAM_UI.Helpers;
using System.Web;
using static IAM_UI.Controllers.EncodeDecodeController;
using System.Collections.Generic;
using System.Net.Http;


namespace IAM_UI.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerService _logger;
        private readonly EncodeDecodeController encdec;
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

        public AuthorizationController(IConfiguration configuration, ICommonService commonService, ILoggerService logger, IGlobalModelService globalModelService, APIResultsValue apirelultvalues, EncodeDecodeController EncDnc)
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
            encdec = EncDnc;
        }



        //UI-Controller
        public IActionResult Index()
        {
            return View();
        }

        //public async Task<IActionResult> RoleMaster()
        //{
        //    return View();
        //}


        #region RoleWiseApplication


        public async Task<IActionResult> RoleWiseApplication()
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

                string urlParameters = "GetRoleWiseApplication/";
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
                    
                        ViewBag.CommonGridData =  JsonConvert.DeserializeObject<DataTable>(Get_all_data["grid"]);

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

                List<string> App_user = new List<string> { application_ID,UserTypeId , CompanyId , gm.TenantID.ToString()};

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
                    List<string> App_user = new List<string> { ApplicationId.ToString(), UserTypeId.ToString() , CompanyId.ToString() , gm.TenantID.ToString()};
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


        #endregion






        #region UserCreation

        public async Task<IActionResult> UserCreation()
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

                string urlParameters = "UserCreation/";
                using (var httpClient = new HttpClient())
                {
                    gm.Data = "";

                    string jsonBody = JsonConvert.SerializeObject(gm);

                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    string url = _baseUrlGlobal + urlParameters;



                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(_baseUrlGlobal + urlParameters, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Deserialize the API response to your desired data model or use dynamic if the structure is unknown.
                        string responseContent = await response.Content.ReadAsStringAsync();

                        List<UserCreationModel> usercreationView = JsonConvert.DeserializeObject<List<UserCreationModel>>(responseContent);


                        ViewBag.UserCreation = usercreationView;

                        return View();
                    }
                    else
                    {

                        //var stackTrace = new StackTrace(true);
                        //_logger.LogError(response, stackTrace);
                        return View("Error");
                    }




                }

            }


            catch (Exception ex)
            {


                //_logger.LogError(ex);
                throw ex;
            }

        }









        public async Task<IActionResult> UserCreationView()
        {
            try
            {

           
                var Application = await ApplicationNames();
                ViewBag.Application = new SelectList(Application, "Value", "Text");


                var Gender = await FetchGender();
                ViewBag.Gender = new SelectList(Gender, "Value", "Text");


                var Category = await FetchUserCategory();
                ViewBag.Category = new SelectList(Category, "Value", "Text");

                var EmailTypes = await FetchEmailTypes();
                ViewBag.EmailTypes = new SelectList(EmailTypes, "Value", "Text");


                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

                string urlParameters = "GetUserCreations/";
                using (var httpClient = new HttpClient())
                {
                    gm.Data = "";

                    string jsonBody = JsonConvert.SerializeObject(gm);

                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    string url = _baseUrlGlobal + urlParameters;



                    return View();
                }

            }


            catch (Exception ex)
            {

                throw ex;
            }

        }
        public async Task<List<SelectListItem>> ApplicationNames()
        {
            var urlParameters = "FetchApplicationNames/";
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.GetAsync(_baseUrlGlobal + urlParameters + gm.TenantID);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var EmployeeUserType = JsonConvert.DeserializeObject<List<SelectListItem>>(responseData);



                        return EmployeeUserType;
                    }
                    else
                    {
                        // Handle the error response
                        throw new Exception("Failed to retrieve Group Head list.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<List<SelectListItem>> FetchGender()
        {
            var urlParameters = "FetchGender/";
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.GetAsync(_baseUrlGlobal + urlParameters + gm.TenantID);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var EmployeeUserType = JsonConvert.DeserializeObject<List<SelectListItem>>(responseData);



                        return EmployeeUserType;
                    }
                    else
                    {
                        // Handle the error response
                        throw new Exception("Failed to retrieve Group Head list.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<SelectListItem>> FetchEmailTypes()
        {
            var urlParameters = "FetchEmailTypes/";
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.GetAsync(_baseUrlGlobal + urlParameters + gm.TenantID);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var EmployeeUserType = JsonConvert.DeserializeObject<List<SelectListItem>>(responseData);



                        return EmployeeUserType;
                    }
                    else
                    {
                        // Handle the error response
                        throw new Exception("Failed to retrieve Group Head list.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<SelectListItem>> FetchUserCategory()
        {
            var urlParameters = "FetchUserCategory/";
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.GetAsync(_baseUrlGlobal + urlParameters + gm.TenantID);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var EmployeeUserType = JsonConvert.DeserializeObject<List<SelectListItem>>(responseData);
                       


                        return EmployeeUserType;
                    }
                    else
                    {
                        // Handle the error response
                        throw new Exception("Failed to retrieve Group Head list.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<SelectListItem>> RoleNames()
        {
            var urlParameters = "RoleNames/";
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.GetAsync(_baseUrlGlobal + urlParameters + gm.TenantID);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var EmployeeUserType = JsonConvert.DeserializeObject<List<SelectListItem>>(responseData);



                        return EmployeeUserType;
                    }
                    else
                    {
                        // Handle the error response
                        throw new Exception("Failed to retrieve Group Head list.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<JsonResult> GetUserDetailsById(int USER_MASTER_KEY)
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);




                string urlParameters = "GetUserDetailsById/";
                gm.Data = JsonConvert.SerializeObject(USER_MASTER_KEY);


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
                        var lst = JsonConvert.DeserializeObject<List<UserCreationModel>>(responseContent);

                        //   //this is for password decryption
                        if (lst != null && lst.Count > 0)
                        {
                            EncodeDecodeModel decodeModel = new EncodeDecodeModel()
                            {
                                Txt = lst[0].Password.ToString(),
                                Type = 2
                            };

                           var decodePass= await encdec.EncryptDecrypt(decodeModel);
                            lst[0].Password = (decodePass as ObjectResult)?.Value?.ToString(); ;
                        }
                         string newResponseContent = JsonConvert.SerializeObject(lst);

                        var result = new { List = newResponseContent };
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
                throw ex;
            }
        }









        public async Task<JsonResult> ComanyList(int USER_MASTER_KEY)
        {
            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
            var urlParameters = "GetAllCompany";
            UserCreationViewModel UserCreationViewModel = new UserCreationViewModel();
            UserCreationViewModel.UserID = USER_MASTER_KEY;
            UserCreationViewModel.USER_MASTER_KEY = USER_MASTER_KEY;
            using (var httpClient = new HttpClient())
            {
                string jsonData = JsonConvert.SerializeObject(UserCreationViewModel);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                HttpResponseMessage response = await httpClient.PostAsync(_baseUrlGlobal + urlParameters, content);
                if (response.IsSuccessStatusCode)
                {

                    var data = await response.Content.ReadAsStringAsync();

                    return Json(data);
                }
                else
                {

                    return Json("Error");
                }
            }
        }








        public async Task<JsonResult> SaveLabelOne([FromBody]UserCreationViewModel model)
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
                model.CreatedBy = gm.userID;
                model.TenantId = gm.TenantID;
                if (!string.IsNullOrEmpty(model.Password))
                {
                    EncodeDecodeModel decodeModel = new EncodeDecodeModel()
                    {
                        Txt = model.Password.ToString(),
                        Type = 1
                    };

                    var decodePass = await encdec.EncryptDecrypt(decodeModel);
                    model.Password = (decodePass as ObjectResult)?.Value?.ToString(); ;
                }
                else
                {
                    model.Password = "";
                }
                string mData = JsonConvert.SerializeObject(model);
                gm.Data = mData;

                //this is for password encryption
              
                var urlParameters = "SaveLebelOne";

                using (var httpClient = new HttpClient())
                {
                    string jsonData = JsonConvert.SerializeObject(gm);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(_baseUrlGlobal + urlParameters, content);
                    if (response.IsSuccessStatusCode)
                    {

                        var data = await response.Content.ReadAsStringAsync();

                        return Json(data);
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


        public async Task<JsonResult> SaveCompanyAccesss(string ItemList, string UserProfileID, string USER_MASTER_KEY , string UserID)
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

                var result = "";
                string[] arr = ItemList.Split(',');
                var urlParameters = "SaveCompanyAccess";
                UserCreationViewModel model = new UserCreationViewModel();

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                       model.CompanyIdString = ItemList;
                        model.UserProfileID = Convert.ToInt32(UserProfileID);
                        model.USER_MASTER_KEY = Convert.ToInt32(USER_MASTER_KEY);
                        model.UserID = Convert.ToInt32(USER_MASTER_KEY);
                        model.CreatedBy = Convert.ToInt32(gm.userID);
                        model.TenantId = Convert.ToInt32(gm.TenantID);

                        string mjsonData = JsonConvert.SerializeObject(model);
                        gm.Data = mjsonData;
                        string jsonData = JsonConvert.SerializeObject(gm);
                        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");



                        HttpResponseMessage response = await httpClient.PostAsync(_baseUrlGlobal + urlParameters, content);

                        if (response.IsSuccessStatusCode)
                        {
                            result = await response.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            return Json("Error");
                        }

                    }
                    return Json(result);

               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<JsonResult> empComanyList(int USER_MASTER_KEY)
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
                var urlParameters = "GetempCompany";
                UserCreationViewModel model = new UserCreationViewModel();
                model.TenantId = gm.TenantID;
                model.USER_MASTER_KEY = USER_MASTER_KEY;
                model.UserID = USER_MASTER_KEY;
                model.CreatedBy = gm.userID;
                using (var httpClient = new HttpClient())
                {
                    string mjsonData = JsonConvert.SerializeObject(model);
                    gm.Data = mjsonData;
                    string jsonData = JsonConvert.SerializeObject(gm);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(_baseUrlGlobal + urlParameters, content);
                    if (response.IsSuccessStatusCode)
                    {

                        var data = await response.Content.ReadAsStringAsync();
                        return Json(data);
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


        public async Task<JsonResult> UserTypeCheckbox(int USER_MASTER_KEY, int CompanyId)
        {
            try
            {

                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
                var urlParameters = "GetUSERUserTypeList";
                UserCreationViewModel model = new UserCreationViewModel();
                model.TenantId = gm.TenantID;
                model.USER_MASTER_KEY = USER_MASTER_KEY;
                model.UserID = USER_MASTER_KEY;
                model.empCompanyId = CompanyId;
                model.CreatedBy = gm.userID;
                using (var httpClient = new HttpClient())
                {
                    string mjsonData = JsonConvert.SerializeObject(model);
                    gm.Data = mjsonData;
                    string jsonData = JsonConvert.SerializeObject(gm);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(_baseUrlGlobal + urlParameters, content);
                    if (response.IsSuccessStatusCode)
                    {

                        var data = await response.Content.ReadAsStringAsync();

                        return Json(data);
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



        public async Task<JsonResult> SaveUserTypeMapDtls(string ItemList, string UserProfileID, string UserID ,string USER_MASTER_KEY, string CompId)
        {

            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
            var result = "";
            string[] arr = ItemList.Split(',');
            var urlParameters = "SaveUserTypeMapDtls";
            UserCreationViewModel model = new UserCreationViewModel();
            using (var httpClient = new HttpClient())
            {

                httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
              
                    model.UserTypeIdString = ItemList;
                    model.empCompanyId = Convert.ToInt32(CompId);
                    model.UserProfileID = Convert.ToInt32(UserProfileID);
                    model.USER_MASTER_KEY = Convert.ToInt32(USER_MASTER_KEY);
                    model.UserID = Convert.ToInt32(UserID);
                    model.TenantId = Convert.ToInt32(gm.TenantID);
                    model.CreatedBy = gm.userID;

                    string mjsonData = JsonConvert.SerializeObject(model);
                    gm.Data = mjsonData;
                    string jsonData = JsonConvert.SerializeObject(gm);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await httpClient.PostAsync(_baseUrlGlobal + urlParameters, content);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        return Json("Error");
                    }

               
                return Json(result);

            }
        }









        public async Task<IActionResult> GetModuleAccess_UserCreation(int CompanyId, int application_ID, int UserTypeId, int UserProfileID, int USER_MASTER_KEY)
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
                string urlParameters = "ModeuleAccess_UserCreation/";
                using (var httpClient = new HttpClient())
                {

                    List<string> App_user = new List<string> { application_ID.ToString(), UserTypeId.ToString(), CompanyId.ToString(), UserProfileID.ToString(), USER_MASTER_KEY.ToString(), gm.TenantID.ToString() };

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





        public async Task<JsonResult> UserTypeDropdown_W3(int USER_MASTER_KEY, int CompanyId)
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
                var urlParameters = "GetUSERUserTypeDropdown";
                UserCreationViewModel model = new UserCreationViewModel();
                model.USER_MASTER_KEY = USER_MASTER_KEY;
                model.empCompanyId = CompanyId;
                model.TenantId = gm.TenantID;
                using (var httpClient = new HttpClient())
                {
                    string mjsonData = JsonConvert.SerializeObject(model);
                    gm.Data = mjsonData;
                    string jsonData = JsonConvert.SerializeObject(gm);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(_baseUrlGlobal + urlParameters, content);
                    if (response.IsSuccessStatusCode)
                    {

                        var data = await response.Content.ReadAsStringAsync();

                        return Json(data);
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


        public async Task<IActionResult> SaveUserCrationView(string CompanyId, string UserTypeId, string[] arrModule, string ApplicationID, string UserProfileID, string UserID)

        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);


                var result = "";
                RoleWiseApplicationModel bModel = new RoleWiseApplicationModel();
                var urlParameters = "SaveModuleDtls";


                // Create a DataTable to hold the data
                DataTable dt = new DataTable();

                // Define the columns in the DataTable

                dt.Columns.Add("TenantId", typeof(int));//this id is hardcode later it will come from session or cache
                dt.Columns.Add("UserID", typeof(int));
                dt.Columns.Add("UserProfileID", typeof(int));
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
                            if (!arrS[0].Contains("1"))
                            {

                                string getAccess = arrS[0].ToString();

                                // Create a new DataRow
                                DataRow row = dt.NewRow();

                                // Set the values for each column
                                row["UserProfileID"] = Convert.ToInt32(UserProfileID);
                                row["UserID"] = Convert.ToInt32(UserID);
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

                        return Redirect("/Authorization/UserCreationView");

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




        [HttpPost]
        public async Task<IActionResult> CredentialsSend([FromBody] credentialsSend Cs)
        {

            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);

                string Urlparameter = "FetchEmailDtls";

                string Body = null, encode = null;

                var model = new Mail();
                var MailUserMasterModel = new MailUserMaster();
                MailUserMasterModel.USER_MASTER_KEY = Cs.USER_MASTER_KEY;
                model.Tenant_ID = gm.TenantID;
;                model.MailUserMaster = MailUserMasterModel;
                model.Application_ID = 0;
                model.TenantMailSetupKey = Cs.TenantMailSetupKey  ;

                using (var httpClient = new HttpClient())
                {
                 

                    string jsonData = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(_baseUrlGlobal + Urlparameter, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var apresponse = await response.Content.ReadAsStringAsync();
                        var mailDtls = JsonConvert.DeserializeObject<Mail>(apresponse);



                        //Body = "Hi " + mailDtls.GroupHeadName + "<br>";

                        //Body = Body + edtls[0].MailBody + "<a href='" + "" + edtls[0].PageLink + "" + "'> Click here </a> ";
                        //Body = Body + "<br>" + edtls[0].parameter1 + ": " + mailDtls.UserName + " " + edtls[0].parameter2 + ": " + mailDtls.Password;

                        //   //this is for password decryption
                        if (mailDtls.MailUserMaster.UserPassword != null && mailDtls.MailUserMaster.UserPassword  != "")
                        {
                            EncodeDecodeModel decodeModel = new EncodeDecodeModel()
                            {
                                Txt = mailDtls.MailUserMaster.UserPassword.ToString(),
                                Type = 2
                            };

                            var decodePass = await encdec.EncryptDecrypt(decodeModel);
                            mailDtls.MailUserMaster.UserPassword = (decodePass as ObjectResult)?.Value?.ToString(); ;
                        }

                        Body = _MailBody
                               .Replace("{{GroupHeadName}}", mailDtls.MailUserMaster.FullName ?? string.Empty)
                               .Replace("{{MailBody}}", mailDtls .MailBody ?? string.Empty)
                               .Replace("{{PageLink}}", mailDtls.PageLink ?? string.Empty)
                               .Replace("{{Parameter1}}", mailDtls.Parameter1 ?? string.Empty)
                               .Replace("{{UserName}}", mailDtls.MailUserMaster.Username ?? string.Empty)
                               .Replace("{{Parameter2}}", mailDtls.Parameter2 ?? string.Empty)
                               .Replace("{{Password}}", mailDtls.MailUserMaster.UserPassword ?? string.Empty)
                               .Replace("{{Pin}}", mailDtls.MailUserMaster.Pin?.ToString() == "0" ? "N/A" : mailDtls.MailUserMaster.Pin?.ToString() ?? "N/A");

                            _commonService.SendMail(mailDtls.SenderMail, mailDtls.SenderPassword, mailDtls.MailSubject, mailDtls.MailUserMaster.Email_ID, null, null, Body, mailDtls.MailUserMaster.UserMailTypeCode);
                            return Json(1);
                     
                    }
                    else
                    {
                        return BadRequest("Validation failed.");
                    }
                }


            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }




        [HttpPost]
        public async Task<IActionResult> BlockUser(BlockUserModel model)
        {
            try
            {

                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
                var urlParameters = "BlockUser";


                using (var httpClient = new HttpClient())
                {
                    string mjsonData = JsonConvert.SerializeObject(model);
                    gm.Data = mjsonData;
                    string jsonData = JsonConvert.SerializeObject(gm);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(_baseUrlGlobal + urlParameters, content);
                    if (response.IsSuccessStatusCode)
                    {

                        var data = await response.Content.ReadAsStringAsync();
                        int r = Convert.ToInt32(data);
                        if (r > 0)
                        {
                            return Ok(new { message = "success" });
                        }
                        if (r == -1)
                        {
                            return Ok(new { message = "success" });
                        }
                        else
                        {
                            return Ok(new { message = "error" });
                        }


                    }
                    else
                    {

                        return Ok(new { message = "error" });
                    }
                }



            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }





        #endregion













        #region Role Master
        [HttpGet]
        public async Task<IActionResult> RoleMaster()
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
                var ApprovalTag = HttpContext.Request.Query["Approve"].ToString();
                object result = "";

                if (ApprovalTag != "")
                {
                    var generalmodel = new GeneralModel
                    {
                        ModuleId = Convert.ToInt32(HttpContext.Items["ModuleId"] as string),
                        userId = gm.userID,
                        UserTypeId = Convert.ToInt32(gm.USER_TYPE_KEY)

                    };
                    string ApprovalGridurl = "GetModuleWiseItemList";
                    result = await _apirelultvalues.PostGridData(BaseUrlAllModule, ApprovalGridurl, generalmodel, typeof(List<RoleMasterModel>));
                }
                else {
                    string urlParameters = "GetRoleMaster/" ;
                    result = await _apirelultvalues.GetGridData(_baseUrlGlobal, urlParameters + gm.TenantID.ToString(), typeof(List<RoleMasterModel>));
                }



                if (result != null)
                {
                    ViewBag.RoleMaster = result as List<RoleMasterModel>;
                    return View();
                }
                else
                {
                    // If data is not available, return an error view
                    return View("Error");
                }

                //string urlParameters = "GetRoleMaster";   //Outside Grid Data

                //using (var httpClient = new HttpClient())

                //{
                //    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));

                //    string data = JsonConvert.SerializeObject(new { });
                //    StringContent Content = new StringContent(data, Encoding.UTF8, "application/json");

                //    HttpResponseMessage response = await httpClient.GetAsync(_baseUrlGlobal + urlParameters);   //Call


                //    if (response.IsSuccessStatusCode)
                //    {
                //        string responseContent = await response.Content.ReadAsStringAsync();
                //        List<RoleMasterModel> Role_Master = JsonConvert.DeserializeObject<List<RoleMasterModel>>(responseContent);
                //        ViewBag.RoleMaster = Role_Master;
                //        return View();
                //    }
                //    else
                //    {
                //        return View("Error");
                //    }
                //}



            }
            catch (Exception ex)
            {
                throw;

            }
        }

        //SAVE FUNCTION

        [HttpPost]
        public async Task<IActionResult> SaveRoleMaster(RoleMasterModel model)
        {
            try
            {
                GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);


                string urlParameters = "SaveRoleMaster/";
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));

                    gm.Data = JsonConvert.SerializeObject(model);

                    string jsonBody = JsonConvert.SerializeObject(gm);

                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    string url = _baseUrlGlobal + urlParameters;



                    HttpResponseMessage response = await httpClient.PostAsync(url, content);  //API Hit From Here

                    if (response.IsSuccessStatusCode)
                    {
                        //string responseContent = await responseMsg.Content.ReadAsStringAsync();
                        var responsedata = await response.Content.ReadAsStringAsync();
                        if (int.TryParse(responsedata, out int apiResponse))
                        {
                            if (apiResponse > 0)
                            {
                                TempData["MSG"] = "success";
                            }
                            else
                            {
                                TempData["MSG"] = "fail";
                            }

                        }
                        else
                        {
                            return Json("error");
                        }


                    }
                    else
                    {
                        return View("Error");

                    }
                    return Redirect("/Authorization/RoleMaster");

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //EDIT function
        public async Task<IActionResult> FetchRoleMasterID(int id)
        {
            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
            string urlParameters = "GetRoleByID/";
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                // Construct the URL with the ID parameter
                string apiUrl = _baseUrlGlobal + urlParameters + id;

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the API response to your desired data model or use dynamic if the structure is unknown.
                    var data = await response.Content.ReadAsStringAsync();
                    List<RoleMasterModel> lst = JsonConvert.DeserializeObject<List<RoleMasterModel>>(data);

                    // Return the JSON response directly as JSON data
                    var info = lst.FirstOrDefault();
                    return Json(info);
                }
                else
                {

                    // You can show an error page or return an error message.
                    return Json("Error");
                }
            }



        }

        //DELETE function
        public async Task<IActionResult> DeleteRoleByID(int id)
        {
            GlobalModel gm = _globalModelService.InitializeGlobalModel(HttpContext);
            string msg = "";
            string urlParameters = "DeleteRoleMaster/";
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                // Construct the URL with the ID parameter
                string apiUrl = _baseUrlGlobal + urlParameters + id +"/" + gm.TenantID;

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the API response to your desired data model or use dynamic if the structure is unknown.
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
                    
                    // You can show an error page or return an error message.
                    return Json("Error");
                }
            }



        }

        #endregion




        #region setview


  

        // Action to handle POST request and set ViewData
        [HttpPost]
        public IActionResult SetViewDataAction(string key, string value)
        {
           
           
            HttpContext.Session.SetString(key, value);

            // Return a response (either a redirect or the same view)





            return Json(new { success = true, message = "ViewData updated successfully." });
        }
        #endregion





  



    }
}
