using CommonUtility.Interface;
using CommonUtility.SharedModels;
using IAM_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.ComponentModel.Design;

//using Swashbuckle.Swagger.Annotations;
using System.Data;
using System.Dynamic;
using System.IO.Pipelines;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IAM_API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDTOService _service;
        private readonly IEncryptDecrypt _encryptDecrypt;
        private readonly ILogger<AuthorizationController> _logger;
        private readonly string _encryptionKey;
        private readonly string ApiKey;
        private readonly ICommonService _commonService;

        public AuthorizationController(IConfiguration configuration, IDTOService service, ILogger<AuthorizationController> logger, IEncryptDecrypt enc, ICommonService commonService)
        {
            _service = service;
            _logger = logger;
            _encryptDecrypt = enc;
            _configuration = configuration;
            _commonService = commonService;

        }







        #region SerializeModel
        [HttpGet]
        [Route("GetAvailableModels")]
        [SwaggerOperation(Tags = new[] { "SerializeModel" })]
        public IActionResult GetAvailableModels()
        {
            // Dynamically list all models in the "Models" folder (you can hardcode this if necessary)
            var modelNames = typeof(AuthorizationController).Assembly.GetTypes()
                .Where(t => t.Namespace == "GlobalAppAPI.Models" && t.IsClass)
                .Select(t => t.Name)
                .ToList();

            return Ok(modelNames);  // Will return the list of model names dynamically
        }



        [HttpPost]
        [Route("GenerateJson")]
        [SwaggerOperation(Tags = new[] { "SerializeModel" })]
        public IActionResult GenerateJson([FromBody] List<string> modelNames)
        {
            if (modelNames == null || !modelNames.Any())
            {
                return BadRequest("Model names cannot be null or empty.");
            }

            // Handle the case for a single model
            if (modelNames.Count == 1)
            {
                var modelName = modelNames.First();

                // Reflectively create an instance of the model
                var modelType = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(t => t.Name == modelName && t.Namespace == "GlobalAppAPI.Models");

                if (modelType == null)
                {
                    return NotFound($"Model '{modelName}' not found.");
                }

                var modelInstance = Activator.CreateInstance(modelType);
                var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                var propertyDict = new ExpandoObject() as IDictionary<string, object>;

                foreach (var prop in properties)
                {
                    var value = prop.GetValue(modelInstance, null);
                    propertyDict.Add(prop.Name, value);
                }

                // Serialize directly to JSON without wrapping in another object
                string jsonSerialized = JsonConvert.SerializeObject(propertyDict);
                return Ok(jsonSerialized); // Return flat JSON for single model
            }

            // Handle the case for multiple models
            var combinedModelDict = new ExpandoObject() as IDictionary<string, object>;

            foreach (var modelName in modelNames)
            {
                // Reflectively create an instance of the model
                var modelType = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(t => t.Name == modelName && t.Namespace == "GlobalAppAPI.Models");

                if (modelType == null)
                {
                    return NotFound($"Model '{modelName}' not found.");
                }

                var modelInstance = Activator.CreateInstance(modelType);
                var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                var propertyDict = new ExpandoObject() as IDictionary<string, object>;

                foreach (var prop in properties)
                {
                    var value = prop.GetValue(modelInstance, null);
                    propertyDict.Add(prop.Name, value);
                }

                // Add the properties of the current model to the combined dictionary
                combinedModelDict.Add(modelName, propertyDict);
            }

            // Serialize the combined result into JSON format using Newtonsoft.Json
            string combinedJsonSerialized = JsonConvert.SerializeObject(combinedModelDict);
            return Ok(combinedJsonSerialized); // Return combined JSON for multiple models
        }



        [HttpPost]
        [Route("EscapeJson")]
        [SwaggerOperation(Tags = new[] { "SerializeModel" })]

        public ActionResult<string> EscapeJson([FromBody] JsonElement jsonElement)
        {
            // Convert the JsonElement back to a JSON string
            string jsonString = jsonElement.ToString();

            // Escape the JSON string
            string escapedJson = jsonString.Replace("\"", "\\\"")
                                           .Replace("\n", "\\n")
                                           .Replace("\r", "\\r");

            // Return the escaped JSON string
            return Ok(escapedJson);
        }

        #endregion


        #region RoleWiseApplication





        [HttpPost]
        [Route("GetRoleWiseApplication")]
        [SwaggerOperation(Tags = new[] { "RoleWiseApplication" })]
        public async Task<ActionResult> GetRoleWiseApplication(GlobalModel gmodel)
        {
            try
            {
                //JobProfileModel jrModel  = JsonConvert.DeserializeObject<JobProfileModel>(gmodel.Data);
                DataSet newDataSet = new DataSet();
                //-------------Coscenter divisdion warehouse---------------------------//
                var parameters = new Dictionary<string, object>
                    {
                        { "@REC_TYPE", "" },
                         { "@TenantId", gmodel.TenantID }

                    };
                DataSet dt = await _service.GetAllDatasetAsync("SP_GET_RoleWiseApplication", parameters);



                foreach (DataTable table in dt.Tables)
                {
                    // Clone the table (structure and data)
                    DataTable clonedTable = table.Copy();

                    // Check the table's index in the dataset and rename accordingly
                    if (dt.Tables.IndexOf(table) == 1)
                    {
                        clonedTable.TableName = "UserType";
                        newDataSet.Tables.Add(clonedTable);
                    }
                    else if (dt.Tables.IndexOf(table) == 2)
                    {
                        clonedTable.TableName = "Application";
                        newDataSet.Tables.Add(clonedTable);
                    }
                    else if (dt.Tables.IndexOf(table) == 3)
                    {
                        clonedTable.TableName = "Company";
                        newDataSet.Tables.Add(clonedTable);
                    }


                    // Add the cloned and renamed table to the new dataset

                }



                Dictionary<string, string> Get_all_data = new Dictionary<string, string>();

                Get_all_data.Add("grid", JsonConvert.SerializeObject(dt.Tables[0]));
                if (dt.Tables.Count > 1)
                {
                    Get_all_data.Add("dataset", JsonConvert.SerializeObject(newDataSet));
                }
                return Ok(JsonConvert.SerializeObject(Get_all_data));
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    ex.Message,
                    ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost]
        [Route("GetAllUserActions")]
        [SwaggerOperation(Tags = new[] { "RoleWiseApplication" })]
        public async Task<ActionResult> GetAllUserActions()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {


                };
                DataSet dt = await _service.GetAllDatasetAsync("SP_FETCH_USER_ACTIONS_ACCESS_RoleWiseApplication", parameters);
                return Ok(JsonConvert.SerializeObject(dt));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("GetModuleAccess")]
        [SwaggerOperation(Tags = new[] { "RoleWiseApplication" })]
        public async Task<ActionResult> GetModuleAccess(GlobalModel gmodel)
        {


            string ii = "";
            int moduleTypeid = 0;
            int count = 0;
            try
            {
                List<string> App_User = JsonConvert.DeserializeObject<List<string>>(gmodel.Data);

                var parameters = new Dictionary<string, object>
                        {
                            { "@TenantId",  App_User[3]},
                            { "@id",  App_User[0]}
                        };

                DataSet ds = await _service.GetAllDatasetAsync("SP_GET_Authorization_SETUP_MAIN_MENU_RoleWiseApplication", parameters);
                List<object> types = new List<object>();

                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    moduleTypeid = Convert.ToInt32(item["ModuleTypeId"]);

                    if (moduleTypeid == 100)
                    {
                        ii += "<tr><th style='text-align:left;background-color: honeydew;font-size: medium;color: black;' colspan='9'>" + item["ModuleName"].ToString() + "</th></tr>";
                    }

                    if (moduleTypeid == 800)
                    {
                        count++;
                        ii += "<tr>";
                        ii += "<td align='center' width='6%'><input type='hidden' id='hdn_User_Module_" + Convert.ToInt32(item["ModuleId"]) + "' value='" + Convert.ToInt32(item["ModuleId"]) + "' />" + count + "</td>";
                        ii += "<td align='center' width='10%'>" + item["ModuleName"].ToString() + "</td>";

                        // Get the ActionResult from GetAllUserActions
                        var actionResult = await GetAllUserActions();

                        // Cast ActionResult to OkObjectResult and extract the JSON string
                        var okResult = actionResult as OkObjectResult;
                        if (okResult != null)
                        {
                            var jsonResponse = okResult.Value.ToString(); // Extract the JSON string

                            // Deserialize the JSON string back into a DataSet
                            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(jsonResponse);

                            foreach (DataRow itemchk in dataSet.Tables[0].Rows)
                            {
                                string actionName = itemchk["ActionName"].ToString();
                                string className = "";

                                // Determine the class based on the action name
                                switch (actionName)
                                {
                                    case "All":
                                        className = "all";
                                        break;
                                    case "Add":
                                        className = "add";
                                        break;
                                    case "Edit":
                                        className = "edit";
                                        break;
                                    case "Delete":
                                        className = "delete";
                                        break;
                                    case "Print":
                                        className = "print";
                                        break;
                                    case "View":
                                        className = "view";
                                        break;
                                }
                                ii += "<td align='center' width='8%'>";
                                ii += "<input type='checkbox' id='" + Convert.ToInt32(itemchk["ActionId"]) + "_" + Convert.ToInt32(item["ModuleId"]) + "' class='" + className + "'   />&nbsp;&nbsp;&nbsp;";
                                ii += "</td>";
                            }
                        }

                        ii += "<td align='center' width='8%'>";
                        ii += "<input type='checkbox' id='deny_" + Convert.ToInt32(item["ModuleId"]) + "' class='deny' checked >&nbsp;&nbsp;&nbsp;";
                        ii += "</td>";
                        ii += "</tr>";
                    }
                }

                // Create a sample DataTable
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Module", typeof(string));
                dataTable.Columns.Add("UserTypeID", typeof(int));
                dataTable.Columns.Add("ApplicationId", typeof(int));



                dataTable.Rows.Add(ii, 0, 0);


                return Ok(JsonConvert.SerializeObject(dataTable));
            }
            catch (Exception ex)
            {
                throw;
            }
        }






        [HttpPost]
        [Route("SaveRoleWiseApplication")]
        [SwaggerOperation(Tags = new[] { "RoleWiseApplication" })]
        public async Task<ActionResult> SaveRoleWiseApplication(GlobalModel gmodel)
        {
            try
            {
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(gmodel.Data);

                int result = await _service.AddWithTVPAsync("SP_BulK_Save_RoleWiseApplication", "@Data", "ACL_UserType_Module_Map_TYPE_TABLE", dt);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }


        [HttpPost]
        [Route("DeleteRoleWiseApplication")]
        [SwaggerOperation(Tags = new[] { "RoleWiseApplication" })]
        public async Task<ActionResult> DeleteRoleWiseApplication(GlobalModel gmodel)
        {
            try
            {
                List<string> App_User = JsonConvert.DeserializeObject<List<string>>(gmodel.Data);
                var parameters = new Dictionary<string, object>
                    {

                         { "@ApplicationId",  App_User[0]},
                    { "@UserTypeId", App_User[1]},
                    { "@CompanyId",  App_User[2]}

                    };
                int r = await _service.AddAsync("SP_Delete_RoleWiseApplication", parameters);
                return Ok(r);
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }


        [HttpPost]
        [Route("EditRoleWiseApplication")]
        [SwaggerOperation(Tags = new[] { "RoleWiseApplication" })]
        public async Task<ActionResult> EditRoleWiseApplication(GlobalModel gmodel)
        {
            try
            {
                DataSet newDataSet = new DataSet();
                List<string> App_User = JsonConvert.DeserializeObject<List<string>>(gmodel.Data);

                var parameters = new Dictionary<string, object>
                    {
                        { "@TenantId", App_User[3] },
                         { "@UserTypeId", App_User[1] },
                         { "@ApplicationId",App_User[0]},
                          { "@CompanyId",App_User[2]}


                    };



                DataSet ds = await _service.GetAllDatasetAsync("SP_GET_USERTYPE_RoleWiseApplication", parameters);
                var UserTypeDatatable = ds.Tables[0];
                string ii = "";
                int count = 0;

                // Adding the dynamic rows
                ii += "<div style='overflow-y: auto; max-height: 300px; position: relative;'>";
                ii += "<table id='example11' class='table table-bordered' style='width: 100%; border: 1px solid #000;'>"; // Set width to 100%
                ii += "<thead style='position: sticky; top: 0; z-index: 100; background-color:rgb(119 153 179); color: white; border: 1px solid #000;'>"; // Dark background for better contrast
                ii += "<tr style='text-align:center'>";
                ii += "<th style='width:10%; border: 1px solid #000;'>Sr. No.</th>"; // Added border
                ii += "<th style='width:20%; border: 1px solid #000;'>Module Name</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>Full Access</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>Add</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>Edit</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>Delete</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>Print</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>View</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>Deny Access</th>"; // Added border
                ii += "</tr>";
                ii += "</thead>";
                ii += "<tbody id='ModuleTable'>";

                foreach (DataRow item in ds.Tables[1].Rows)
                {
                    int moduleTypeId = Convert.ToInt32(item["ModuleTypeId"]);
                    int moduleId = Convert.ToInt32(item["ModuleId"]);
                    int moduleParentId = Convert.ToInt32(item["ModuleParentId"]);
                    string moduleName = item["ModuleName"].ToString();

                    // Main Menu
                    if (moduleTypeId == 100)
                    {
                        ii += $"<tr data-id='{moduleId}' data-parent-id='{moduleParentId}' class='module-level-100'>";
                        ii += $"<th colspan='9' style='text-align:left;background-color:rgb(221, 232, 240);font-size:small;color:black;padding:0px'>";
                        ii += $"<button type='button' class='btn btn-link toggle-button' data-target='module-parent-{moduleId}' aria-expanded='false'>+</button>";
                        ii += $"{moduleName}</th></tr>";
                    }

                    // Submenu levels (200 to 700)
                    if (moduleTypeId >= 200 && moduleTypeId <= 700)
                    {
                        ii += $"<tr data-id='{moduleId}' data-parent-id='{moduleParentId}' class='module-level-{moduleTypeId} module-parent-{moduleParentId} submenu' style='display:none;'>";
                        ii += $"<td colspan='9' style='padding:0px;'>";
                        ii += $"<button type='button' class='btn btn-link toggle-button' data-target='module-parent-{moduleId}' aria-expanded='false'>+</button>";
                        ii += $"{moduleName}</td></tr>";
                    }

                    // Form Level (ModuleTypeId == 800)
                    if (moduleTypeId == 800)
                    {
                        count++;
                        ii += $"<tr data-id='{moduleId}' data-parent-id='{moduleParentId}' class='module-level-800 module-parent-{moduleParentId} module-parent-{moduleId}' style='display:none'>";
                        ii += $"<td align='center' style='width:10%'><input type='hidden' id='hdn_User_Module_{moduleId}' value='{moduleId}' />{count}</td>";
                        ii += $"<td style='width:20%' align='center'>{moduleName}</td>";

                        var actionResult = await GetAllUserActions();
                        var okResult = actionResult as OkObjectResult;
                        var jsonResponse = okResult.Value.ToString(); // Extract the JSON string

                        // Deserialize the JSON string back into a DataSet
                        DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(jsonResponse);
                        foreach (DataRow itemchk in dataSet.Tables[0].Rows)
                        {
                            string actionName = itemchk["ActionName"].ToString().ToLower();
                            string className = "";

                            switch (actionName)
                            {
                                case "all": className = "all"; break;
                                case "add": className = "add"; break;
                                case "edit": className = "edit"; break;
                                case "delete": className = "delete"; break;
                                case "print": className = "print"; break;
                                case "view": className = "view"; break;
                            }
                            bool conditionMet = false;

                            foreach (DataRow row in UserTypeDatatable.Rows)
                            {
                                int ActionId = Convert.ToInt32(row["ActionId"]);
                                int ModuleId = Convert.ToInt32(row["ModuleId"]);

                                if (ModuleId == moduleId && ActionId == Convert.ToInt32(itemchk["ActionId"]))
                                {
                                    conditionMet = true;
                                    ii += $"<td align='center' style='width:10%'><input type='checkbox' id='{ActionId}_{moduleId}' class='{className}' checked /></td>";
                                    break;
                                }
                            }

                            if (!conditionMet)
                            {
                                ii += $"<td align='center' style='width:5%'><input type='checkbox' id='{Convert.ToInt32(itemchk["ActionId"])}_{moduleId}' class='{className}' /></td>";
                            }
                        }

                        ii += $"<td align='center' style='width:5%'><input type='checkbox' id='deny_{moduleId}' class='deny' checked /></td>";
                        ii += "</tr>";
                    }
                }

                // Closing tags for dynamic table and scrollable div
                ii += "</tbody>";
                ii += "</table>";
                ii += "</div>";


                // Create a sample DataTable
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Module", typeof(string));
                dataTable.Columns.Add("CompanyId", typeof(int));
                dataTable.Columns.Add("UserTypeID", typeof(int));
                dataTable.Columns.Add("ApplicationId", typeof(int));


                if (ds.Tables[0].Rows.Count == 0)
                {
                    dataTable.Rows.Add(ii, Convert.ToInt32(App_User[2]), Convert.ToInt32(App_User[1]), 0);
                }
                else
                {
                    foreach (DataRow row in UserTypeDatatable.Rows)
                    {
                        // Add sample data to the DataTable
                        dataTable.Rows.Add(ii, Convert.ToInt32(row["CompanyId"]), Convert.ToInt32(row["UserTypeId"]), Convert.ToInt32(row["ApplicationId"]));
                        break;

                    }
                }

                return Ok(JsonConvert.SerializeObject(dataTable));
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    ex.Message,
                    ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }




        #endregion



        #region UserTypeAccessSetting


        [HttpPost]
        [Route("ItGetRoleWiseApplication")]
        [SwaggerOperation(Tags = new[] { "It Admin" })]
        public async Task<ActionResult> ItGetRoleWiseApplication(GlobalModel gmodel)
        {
            try
            {
                //JobProfileModel jrModel  = JsonConvert.DeserializeObject<JobProfileModel>(gmodel.Data);
                DataSet newDataSet = new DataSet();
                //-------------Coscenter divisdion warehouse---------------------------//
                var parameters = new Dictionary<string, object>
              {

                   { "@REC_TYPE", "" },
                   { "@TenantId", gmodel.TenantID }
              };
                DataSet dt = await _service.GetAllDatasetAsync("SP_GET_RoleWiseApplication_UserTypeAccessSetting", parameters);



                foreach (DataTable table in dt.Tables)
                {
                    // Clone the table (structure and data)
                    DataTable clonedTable = table.Copy();

                    // Check the table's index in the dataset and rename accordingly
                    if (dt.Tables.IndexOf(table) == 1)
                    {
                        clonedTable.TableName = "UserType";
                        newDataSet.Tables.Add(clonedTable);
                    }
                    else if (dt.Tables.IndexOf(table) == 2)
                    {
                        clonedTable.TableName = "Application";
                        newDataSet.Tables.Add(clonedTable);
                    }
                    else if (dt.Tables.IndexOf(table) == 3)
                    {
                        clonedTable.TableName = "Company";
                        newDataSet.Tables.Add(clonedTable);
                    }


                    // Add the cloned and renamed table to the new dataset

                }



                Dictionary<string, string> Get_all_data = new Dictionary<string, string>();

                Get_all_data.Add("grid", JsonConvert.SerializeObject(dt.Tables[0]));
                if (dt.Tables.Count > 1)
                {
                    Get_all_data.Add("dataset", JsonConvert.SerializeObject(newDataSet));
                }
                return Ok(JsonConvert.SerializeObject(Get_all_data));
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    ex.Message,
                    ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }

        #endregion







        #region UserCreation


        [HttpPost]
        [Route("UserCreation")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]
        public async Task<IEnumerable<UserCreationModel>> UserCreation()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                        {"@REC_TYPE", "ALL"},
                         {"@USER_MASTER_KEY", 0},
                          {"@UserProfileID", 0},
                        {"@UserTypeId", 0}
                };

                DataSet ds = await _service.GetAllDatasetAsync("SP_GET_USER_CREATION", parameters);


                List<UserCreationModel> lst = new List<UserCreationModel>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var mdl = new UserCreationModel
                        {
                            TenantID = row["TenantID"] as int? ?? default,
                            USER_MASTER_KEY = row["USER_MASTER_KEY"] as int? ?? default,
                            UserProfileID = row["UserProfileID"] as int? ?? default,
                            UserTypeId = row["UserTypeId"] as int? ?? default,
                            UserCategoryId = row["UserCategoryId"] as int? ?? default,
                            UserCategoryName = row["UserCategoryName"]?.ToString() ?? string.Empty,
                            UserID = row["UserID"] as int? ?? default,
                            AuthBlock = row["AuthBlock"] as int? ?? default,
                            CreatedBy = row["CreatedBy"] as int? ?? default,
                            StatusKey = row["StatusKey"] as int? ?? default,
                            Pin = row["Pin"] as int? ?? default,
                            UserName = row["UserName"]?.ToString() ?? string.Empty,
                            Password = row["Password"]?.ToString() ?? string.Empty,
                            FirstName = row["FirstName"]?.ToString() ?? string.Empty,
                            MiddleName = row["MiddleName"]?.ToString() ?? string.Empty,
                            FullName = row["FullName"]?.ToString() ?? string.Empty,
                            LastName = row["LastName"]?.ToString() ?? string.Empty,
                            Mobile_No = row["Mobile_No"]?.ToString() ?? string.Empty,
                            Email_ID = row["Email_ID"]?.ToString() ?? string.Empty,
                            EmailTypeId = row["EmailTypeId"] as int? ?? default,
                            EmailTypeName = row["Email_ID"]?.ToString() ?? string.Empty,
                            EmailTypeCode = row["Email_ID"]?.ToString() ?? string.Empty,
                            GenderName = row["GenderName"]?.ToString() ?? string.Empty,
                            Gender = row["Gender"] as int? ?? default,
                            CurrentAddress = row["CurrentAddress"]?.ToString() ?? string.Empty,
                            PermanentAddress = row["PermanentAddress"]?.ToString() ?? string.Empty,
                            Aadhar_no = row["Aadhar_no"]?.ToString() ?? string.Empty,
                            DOB = row["DOB"] as DateTime? ?? default,
                            CreatedDate = row["CreatedDate"] as DateTime? ?? default,
                            IsAcceptedTerms = row["IsAcceptedTerms"] as int? ?? default
                        };

                        lst.Add(mdl);
                    }
                }

                return lst;


            }
            catch
            {
                throw;
            }
        }









        [HttpPost]
        [Route("GetAllCompany")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]
        public async Task<string> GetAllCompany(UserCreationViewModel UserCreationViewModel)
        {
            string ii = "";
            try
            {

                var userID = UserCreationViewModel.USER_MASTER_KEY;
                var USER_MASTER_KEY = UserCreationViewModel.USER_MASTER_KEY;
                DataSet dataSet = await AllCompanyList();



              
                DataSet eds = await GetUserById(userID);

              



                int ec = 0;
                ii += "<ul class='list-group' id='ItemList'>";
                foreach (DataRow item in dataSet.Tables[0].Rows)
                {
                    var CompanyId = Convert.ToInt32(item["CompanyId"]).ToString();
                    int Info = await UserWiseCompany(CompanyId, userID);

                   



                    ii += "<li class='list-group-item'>";
                    ii += "<div class='checkbox'>";
                    if (Info == 1)
                    {

                            ii += "<input type='checkbox' id='" + CompanyId + "' checked  />&nbsp;&nbsp;&nbsp;";
                       
                    }
                    else
                    {
                        ii += "<input type='checkbox' id='" + CompanyId + "'  />&nbsp;&nbsp;&nbsp;";
                    }

                    if (Info == 1)
                    {

                        if (ec == 1)
                        {
                            ii += "<label style='color: black;' for='" + CompanyId + "'>" + item["COMPANY_NAME"].ToString() + "</label>";
                        }
                        else
                        {
                            ii += "<label  for='" + CompanyId + "'>" + item["COMPANY_NAME"].ToString() + "</label>";
                        }

                    }
                    else
                    {
                        ii += "<label for='" + CompanyId + "'>" + item["COMPANY_NAME"].ToString() + "</label>";
                    }
                    ii += "</div>";
                    ii += "</li>";
                }
                ii += "</ul>";
                return ii;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("AllCompanyList")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]
        private async Task<DataSet> AllCompanyList()
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                return await _service.GetAllDatasetAsync("SP_FATCH_COMPANIES", parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("UserWiseCompany")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]
        private async Task<int> UserWiseCompany(string companyId, int? USER_MASTER_KEY)
        {
            try
            {
                var parameters = new Dictionary<string, object>
        { 
            {"@CompanyID", companyId.ToString()},
            {"@USER_MASTER_KEY", USER_MASTER_KEY.ToString()}
        };

                int r = await _service.AddAsync("SP_FATCH_USERWISECOMPANIES_USERCREATION", parameters);



                return r;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("GetUserById")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]
        public Task<DataSet> GetUserById(int? USER_MASTER_KEY)
        {
            try
            {

                var parameters = new Dictionary<string, object>
                {
                    {"@REC_TYPE", "BYID_UC_W1"},
                         {"@USER_MASTER_KEY",USER_MASTER_KEY },
                          {"@UserProfileID", 0},
                        {"@UserTypeId", 0}

                };
                return _service.GetAllDatasetAsync("SP_GET_USER_CREATION", parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        [HttpPost]
        [Route("GetUserDetailsById")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]
        public async Task<List<UserCreationModel>> GetUserDetailsById(GlobalModel gm)
        {
            try
            {
                int USER_MASTER_KEY = string.IsNullOrEmpty(gm?.Data?.ToString()) ? 0 : Convert.ToInt32(gm.Data);

                DataSet ds = await GetUserById(USER_MASTER_KEY);
                List<UserCreationModel> lst = new List<UserCreationModel>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var mdl = new UserCreationModel
                        {
                            TenantID = row["TenantID"] as int? ?? default,
                            USER_MASTER_KEY = row["USER_MASTER_KEY"] as int? ?? default,
                            UserProfileID = row["UserProfileID"] as int? ?? default,
                            UserTypeId = row["UserTypeId"] as int? ?? default,
                            UserCategoryId = row["UserCategoryId"] as int? ?? default,
                            UserCategoryName = row["UserCategoryName"]?.ToString() ?? string.Empty,
                            UserID = row["UserID"] as int? ?? default,
                            AuthBlock = row["AuthBlock"] as int? ?? default,
                            CreatedBy = row["CreatedBy"] as int? ?? default,
                            StatusKey = row["StatusKey"] as int? ?? default,
                            Pin = row["Pin"] as int? ?? default,
                            UserName = row["UserName"]?.ToString() ?? string.Empty,
                            Password = row["Password"]?.ToString() ?? string.Empty,
                            FirstName = row["FirstName"]?.ToString() ?? string.Empty,
                            MiddleName = row["MiddleName"]?.ToString() ?? string.Empty,
                            FullName = row["FullName"]?.ToString() ?? string.Empty,
                            LastName = row["LastName"]?.ToString() ?? string.Empty,
                            Mobile_No = row["Mobile_No"]?.ToString() ?? string.Empty,
                            Email_ID = row["Email_ID"]?.ToString() ?? string.Empty,
                            EmailTypeId = row["EmailTypeId"] as int? ?? default,
                            EmailTypeName = row["Email_ID"]?.ToString() ?? string.Empty,
                            EmailTypeCode = row["Email_ID"]?.ToString() ?? string.Empty,
                            Gender = row["Gender"] as int? ?? default,
                            GenderName = row["GenderName"]?.ToString() ?? string.Empty,
                            CurrentAddress = row["CurrentAddress"]?.ToString() ?? string.Empty,
                            PermanentAddress = row["PermanentAddress"]?.ToString() ?? string.Empty,
                            Aadhar_no = row["Aadhar_no"]?.ToString() ?? string.Empty,
                            DOB = row["DOB"] as DateTime? ?? default,
                            CreatedDate = row["CreatedDate"] as DateTime? ?? default,
                            IsAcceptedTerms = row["IsAcceptedTerms"] as int? ?? default,
                              User_Unique_ID  = row["User_Unique_ID"]?.ToString() ?? string.Empty
                        };

                        lst.Add(mdl);
                    }
                }

                return lst;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        [Route("FetchApplicationNames/{TenantId}")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]

        public async Task<List<SelectListItem>> FetchApplicationNames(int TenantId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@REC_TYPE", "ALL"},
                     { "@TenantId", TenantId}

                };

                DataSet DS = await _service.GetAllDatasetAsync("SP_FETCH_All_APPLICATION", parameters);
                List<SelectListItem> types = new List<SelectListItem>();

                types.Add(new SelectListItem { Text = "-Select-", Value = "" });
                foreach (DataRow dr in DS.Tables[0].Rows)
                {
                    types.Add(new SelectListItem { Text = dr["ApplicationName"].ToString(), Value = dr["ApplicationId"].ToString() });
                }
                return types;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [HttpGet]
        [Route("RoleNames/{TenantId}")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]

        public async Task<List<SelectListItem>> RoleNames(int TenantId)
        {
            try
            {

                var parameters = new Dictionary<string, object>
                {
                    { "@REC_TYPE","GET"},
                     { "@TenantId", TenantId}

                };
                DataSet DS = await _service.GetAllDatasetAsync("SP_GET_ACL_UserType_M", parameters);

                List<SelectListItem> types = new List<SelectListItem>();

                types.Add(new SelectListItem { Text = "-Select-", Value = "0" });
                foreach (DataRow dr in DS.Tables[0].Rows)
                {
                    types.Add(new SelectListItem { Text = dr["UserTypeName"].ToString(), Value = dr["UserTypeId"].ToString() });
                }
                return types;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        [HttpGet]
        [Route("FetchGender/{TenantId}")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]

        public async Task<List<SelectListItem>> FetchGender(int TenantId)
        {
            try
            {

                var parameters = new Dictionary<string, object>
                {
                    { "@REC_TYPE","GET"},
                     { "@TenantId", TenantId}

                };
                DataSet DS = await _service.GetAllDatasetAsync("SP_GET_GENDER", parameters);

                List<SelectListItem> types = new List<SelectListItem>();

                types.Add(new SelectListItem { Text = "-Select-", Value = "" });
                foreach (DataRow dr in DS.Tables[0].Rows)
                {
                    types.Add(new SelectListItem { Text = dr["GenderName"].ToString(), Value = dr["GenderId"].ToString() });
                }
                return types;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



        [HttpGet]
        [Route("FetchEmailTypes/{TenantId}")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]

        public async Task<List<SelectListItem>> FetchEmailTypes(int TenantId)
        {
            try
            {

                var parameters = new Dictionary<string, object>
                {
                    { "@REC_TYPE","ALL"},
                     { "@TenantId", TenantId}

                };
                DataSet DS = await _service.GetAllDatasetAsync("SP_FETCH_All_EMAIL_TYPES", parameters);

                List<SelectListItem> types = new List<SelectListItem>();

                types.Add(new SelectListItem { Text = "-Select-", Value = "" });
                foreach (DataRow dr in DS.Tables[0].Rows)
                {
                    types.Add(new SelectListItem { Text = dr["EmailTypeName"].ToString(), Value = dr["EmailTypeId"].ToString() });
                }
                return types;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }




        [HttpGet]
        [Route("FetchUserCategory/{TenantId}")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]

        public async Task<List<SelectListItem>> FetchUserCategory(int TenantId)
        {
            try
            {

                var parameters = new Dictionary<string, object>
                {
                    { "@REC_TYPE","GET"},
                     { "@TenantId", TenantId}

                };
                DataSet DS = await _service.GetAllDatasetAsync("SP_GET_USER_CATEGORY", parameters);

                List<SelectListItem> types = new List<SelectListItem>();

                types.Add(new SelectListItem { Text = "-Select-", Value = "" });
                foreach (DataRow dr in DS.Tables[0].Rows)
                {
                    types.Add(new SelectListItem { Text = dr["UserCategoryName"].ToString(), Value = dr["UserCateGoryId"].ToString() });
                }
                return types;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



        [HttpPost]
        [Route("SaveLebelOne")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]
        public async Task<int> SaveLebelOne(GlobalModel gmodel)
        {

            try
            {
                UserCreationViewModel model = JsonConvert.DeserializeObject<UserCreationViewModel>(gmodel.Data);
                var parameters = new Dictionary<string, object>
                {
                    {"@TenantId", model.TenantId},
                    {"@UserID", model.UserID},
                    {"@USER_MASTER_KEY", model.USER_MASTER_KEY},  // Output parameter
                    {"@UserProfileID", model.UserProfileID},
                    {"@User_Unique_ID", model.User_Unique_ID},
                    {"@UserCategoryId", model.UserCategoryId},
                    {"@UserCategoryName", model.UserCategoryName},
                    {"@FirstName", model.FirstName},
                    {"@MiddleName", model.MiddleName},
                    {"@LastName", model.LastName},
                    {"@FullName", model.FullName},
                    {"@CurrentAddress", model.CurrentAddress},
                    {"@PermanentAddress", model.PermanentAddress},
                    {"@Gender", model.Gender},
                    {"@GenderName", model.GenderName},
                    {"@UserName", model.UserName},
                    {"@Password", model.Password},
                    {"@Pin", model.Pin},
                    {"@DOB", model.DOB},
                    {"@Mobile_No", model.Mobile_No},
                    {"@Email_ID", model.Email_ID},
                     {"@EmailTypeId", model.EmailTypeId},
                    {"@Aadhar_no", model.Aadhar_no},
                    {"@IsAcceptedTerms", model.IsAcceptedTerms},
                    {"@StatusKey", model.StatusKey},
                    {"@CreatedBy", model.CreatedBy},
                    {"@CreatedDate", model.CreatedDate}
                };

                                // Execute stored procedure
                 var r = await _service.AddAsync("SP_SAVE_UserProfile_UserMaster", parameters);
                  return r;
                            }
            catch (Exception ex)
            {
                throw;
            }




         }




        [HttpPost]
        [Route("SaveCompanyAccess")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]

        public async Task<int> SaveCompanyAccess(GlobalModel gmodel)
        {
            try
            {
                UserCreationViewModel model = JsonConvert.DeserializeObject<UserCreationViewModel>(gmodel.Data);

                var parameters = new Dictionary<string, object>
                    {
                            { "@TenantId", model.TenantId.ToString()},
                            { "@USER_MASTER_KEY", model.USER_MASTER_KEY.ToString()},
                            { "@UserProfileID", model.UserProfileID.ToString()},
                            { "@CompanyIdString", model.CompanyIdString.ToString()},
                            { "@CreatedBy", model.CreatedBy.ToString()}



                    };

                int r = await _service.AddAsync("SP_SAVE_USER_COMPANYACCESS", parameters);
                return r;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        [Route("GetempCompany")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]

        public async Task<string> GetempCompany(GlobalModel gmodel)
        {
            try
            {
                UserCreationViewModel model = JsonConvert.DeserializeObject<UserCreationViewModel>(gmodel.Data);

                string dropdown = "";

                var parameters = new Dictionary<string, object>
                {
                    { "@USER_MASTER_KEY", model.USER_MASTER_KEY.ToString()}



                };
                DataSet dataSet = await _service.GetAllDatasetAsync("Sp_Get_userWiseCompanyList", parameters);

                List<SelectListItem> Company = new List<SelectListItem>();
                dropdown = "";
                dropdown += $"<option value=''>{"--Select Company--"}</option>";
                foreach (DataRow item in dataSet.Tables[0].Rows)
                {
                    dropdown += $"<option value='{Convert.ToInt32(item["CompanyId"])}'>{item["COMPANY_NAME"].ToString()}</option>";
                }
                dropdown += "";
                return dropdown;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        [Route("GetUSERUserTypeList")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]
        public async Task<string> GetUSERUserTypeList(GlobalModel gmodel)
        {
            try
            {
                UserCreationViewModel model = JsonConvert.DeserializeObject<UserCreationViewModel>(gmodel.Data);
                string ii = "";



                var parameters = new Dictionary<string, object>
                {
                    { "@REC_TYPE","GET"},
                     { "@TenantId",model.TenantId},
                     { "@CompanyId", model.empCompanyId.ToString()}

                };
                DataSet dataSet = await _service.GetAllDatasetAsync("SP_GET_ACL_UserType_UserCreation_Wizard2", parameters);

                ii += "<ul class='list-group' id='ItemUserTypeList'>";
                foreach (DataRow item in dataSet.Tables[0].Rows)
                {
                    var TypeId = Convert.ToInt32(item["UserTypeId"]).ToString();

                    model.UserTypeId = Convert.ToInt32(TypeId);


                    var parameters1 = new Dictionary<string, object>
                    {
                         { "@TenantId", model.TenantId.ToString()},
                         { "@USER_MASTER_KEY", model.USER_MASTER_KEY.ToString()},
                        { "@CompanyId", model.empCompanyId.ToString()},
                        { "@UserTypeId", model.UserTypeId.ToString()}

                    };
                    int info = await _service.AddAsync("SP_GET_UserTypeEmpCompWise_usercreation", parameters1);


                    ii += "<li class='list-group-item'>";
                    ii += "<div class='checkbox'>";
                    if (info == 1)
                    {
                        ii += "<input saved='saved' type='checkbox' class='checkbox-item data-typeid' value='" + TypeId + "' checked />&nbsp;&nbsp;&nbsp;";


                    }
                    else
                    {
                        ii += "<input type='checkbox' class='checkbox-item data-typeid' value=" + TypeId + "  />&nbsp;&nbsp;&nbsp;";
                    }

                    ii += "<label for=" + TypeId + " >" + item["UserTypeName"].ToString() + "</ label > ";
                    ii += "</div>";
                    ii += "</li>";
                }
                ii += "</ul>";
                return ii;



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        [Route("SaveUserTypeMapDtls")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]
        public async Task<int> SaveUserTypeMapDtls(GlobalModel gmodel)
        {

            try
            {
                UserCreationViewModel model = JsonConvert.DeserializeObject<UserCreationViewModel>(gmodel.Data);

                var parameters = new Dictionary<string, object>
                {
                       { "@TenantID", model.TenantId.ToString()},
                         { "@USER_MASTER_KEY", model.USER_MASTER_KEY.ToString()},
                        { "@UserProfileID", model.UserProfileID.ToString()},
                         { "@CompanyId", model.empCompanyId.ToString()},
                        { "@UserTypeIdString", model.UserTypeIdString.ToString()},
                        { "@CreatedBy", model.CreatedBy.ToString()}


                };

                int r = await _service.AddAsync("SP_Save_UserProfile_Access_Map_Dtls1", parameters);
                return r;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        [HttpPost]
        [Route("ModeuleAccess_UserCreation")]
        [SwaggerOperation(Tags = new[] { "RoleWiseApplication" })]
        public async Task<ActionResult> ModeuleAccess_UserCreation(GlobalModel gmodel)
        {
            try
            {
                DataSet newDataSet = new DataSet();
                List<string> App_User = JsonConvert.DeserializeObject<List<string>>(gmodel.Data);

                var parameters = new Dictionary<string, object>
                    {
                         { "@TenantId", App_User[5] },
                         { "@UserTypeId", App_User[1] },
                         { "@ApplicationId",App_User[0]},
                          { "@UserProfileID",App_User[3]},
                         { "@USER_MASTER_KEY",App_User[4]},
                          { "@CompanyId",App_User[2]}


                    };



                DataSet ds = await _service.GetAllDatasetAsync("SP_GET_MODILE_ACCESS_UserCreation", parameters);
                var UserTypeDatatable = ds.Tables[3];
                string ii = "";
                int count = 0;

                // Adding the dynamic rows
                ii += "<div style=' position: relative;'>";
                ii += "<table id='example11' class='table table-bordered' style=' border: 1px solid #000;'>"; // Set width to 100%
                ii += "<thead style='position: sticky; top: 0; z-index: 100; background-color:rgb(119 153 179); color: white; border: 1px solid #000;'>"; // Dark background for better contrast
                ii += "<tr style='text-align:center'>";
                ii += "<th style='width:10%; border: 1px solid #000;'>Sr. No.</th>"; // Added border
                ii += "<th style='width:20%; border: 1px solid #000;'>Module Name</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>Full Access</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>Add</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>Edit</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>Delete</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>Print</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>View</th>"; // Added border
                ii += "<th style='width:5%; border: 1px solid #000;'>Deny Access</th>"; // Added border
                ii += "</tr>";
                ii += "</thead>";
                ii += "<tbody id='ModuleTable'>";

                foreach (DataRow item in ds.Tables[1].Rows)
                {
                    int moduleTypeId = Convert.ToInt32(item["ModuleTypeId"]);
                    int moduleId = Convert.ToInt32(item["ModuleId"]);
                    int moduleParentId = Convert.ToInt32(item["ModuleParentId"]);
                    string moduleName = item["ModuleName"].ToString();

                    // Main Menu
                    if (moduleTypeId == 100)
                    {
                        ii += $"<tr data-id='{moduleId}' data-parent-id='{moduleParentId}' class='module-level-100'>";
                        ii += $"<th colspan='9' style='text-align:left;background-color:rgb(221, 232, 240);font-size:small;color:black;padding:0px'>";
                        ii += $"<button type='button' class='btn btn-link toggle-button' data-target='module-parent-{moduleId}' aria-expanded='false'>+</button>";
                        ii += $"{moduleName}</th></tr>";
                    }

                    // Submenu levels (200 to 700)
                    if (moduleTypeId >= 200 && moduleTypeId <= 700)
                    {
                        ii += $"<tr data-id='{moduleId}' data-parent-id='{moduleParentId}' class='module-level-{moduleTypeId} module-parent-{moduleParentId} submenu' style='display:none;'>";
                        ii += $"<td colspan='9' style='padding:0px;'>";
                        ii += $"<button type='button' class='btn btn-link toggle-button' data-target='module-parent-{moduleId}' aria-expanded='false'>+</button>";
                        ii += $"{moduleName}</td></tr>";
                    }

                    // Form Level (ModuleTypeId == 800)
                    if (moduleTypeId == 800)
                    {
                        count++;
                        ii += $"<tr data-id='{moduleId}' data-parent-id='{moduleParentId}' class='module-level-800 module-parent-{moduleParentId} module-parent-{moduleId}' style='display:none'>";
                        ii += $"<td align='center' style='width:10%'><input type='hidden' id='hdn_User_Module_{moduleId}' value='{moduleId}' />{count}</td>";
                        ii += $"<td style='width:20%' align='center'>{moduleName}</td>";

                        var actionResult = await GetAllUserActions();
                        var okResult = actionResult as OkObjectResult;
                        var jsonResponse = okResult.Value.ToString(); // Extract the JSON string

                        // Deserialize the JSON string back into a DataSet
                        DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(jsonResponse);
                        foreach (DataRow itemchk in dataSet.Tables[0].Rows)
                        {
                            string actionName = itemchk["ActionName"].ToString().ToLower();
                            string className = "";

                            switch (actionName)
                            {
                                case "all": className = "all"; break;
                                case "add": className = "add"; break;
                                case "edit": className = "edit"; break;
                                case "delete": className = "delete"; break;
                                case "print": className = "print"; break;
                                case "view": className = "view"; break;
                            }
                            bool conditionMet = false;

                            foreach (DataRow row in UserTypeDatatable.Rows)
                            {
                                int ActionId = Convert.ToInt32(row["ActionId"]);
                                int ModuleId = Convert.ToInt32(row["ModuleId"]);

                                if (ModuleId == moduleId && ActionId == Convert.ToInt32(itemchk["ActionId"]))
                                {
                                    conditionMet = true;
                                    ii += $"<td align='center'  style='width:10%'><input saved='saved' type='checkbox' id='{ActionId}_{moduleId}' class='{className}' checked /></td>";
                                    break;
                                }
                            }

                            if (!conditionMet)
                            {
                                ii += $"<td align='center'  style='width:5%'><input type='checkbox' id='{Convert.ToInt32(itemchk["ActionId"])}_{moduleId}' class='{className}' /></td>";
                            }
                        }

                        ii += $"<td align='center' style='width:5%'><input saved='saved' type='checkbox' id='deny_{moduleId}' class='deny' checked /></td>";
                        ii += "</tr>";
                    }
                }

                // Closing tags for dynamic table and scrollable div
                ii += "</tbody>";
                ii += "</table>";
                ii += "</div>";


                // Create a sample DataTable
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Module", typeof(string));
                dataTable.Columns.Add("CompanyId", typeof(int));
                dataTable.Columns.Add("UserTypeID", typeof(int));
                dataTable.Columns.Add("ApplicationId", typeof(int));


                if (ds.Tables[0].Rows.Count == 0)
                {
                    dataTable.Rows.Add(ii, Convert.ToInt32(App_User[2]), Convert.ToInt32(App_User[1]), 0);
                }
                else
                {
                    foreach (DataRow row in UserTypeDatatable.Rows)
                    {
                        // Add sample data to the DataTable
                        dataTable.Rows.Add(ii, Convert.ToInt32(row["CompanyId"]), Convert.ToInt32(row["UserTypeId"]), Convert.ToInt32(row["ApplicationId"]));
                        break;

                    }
                }

                return Ok(JsonConvert.SerializeObject(dataTable));
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    ex.Message,
                    ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost]
        [Route("GetUSERUserTypeDropdown")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]
        public async Task<string> GetUSERUserTypeDropdown(GlobalModel gmodel)
        {
            try
            {
                UserCreationViewModel model = JsonConvert.DeserializeObject<UserCreationViewModel>(gmodel.Data);
                string dropdownHtml = "";

                // Fetch User Types
                var parameters = new Dictionary<string, object>
        {
            { "@REC_TYPE", "GET" },
             { "@TenantId", model.TenantId}
        };

                DataSet dataSet = await _service.GetAllDatasetAsync("SP_GET_ACL_UserType_M", parameters);

                // Start the dropdown


                // Add a default "Select" option
                dropdownHtml += "<option value=''>Select User Type</option>";

                // Loop through the results and check the info value
                foreach (DataRow item in dataSet.Tables[0].Rows)
                {
                    var TypeId = Convert.ToInt32(item["UserTypeId"]).ToString();
                    model.UserTypeId = Convert.ToInt32(TypeId);

                    // Prepare parameters for the second stored procedure
                    var parameters1 = new Dictionary<string, object>
                        {
                            { "@TenantId", model.TenantId.ToString() },
                            { "@USER_MASTER_KEY", model.USER_MASTER_KEY.ToString() },
                            { "@CompanyId", model.empCompanyId.ToString() },
                            { "@UserTypeId", model.UserTypeId.ToString() }
                        };

                    int info = await _service.AddAsync("SP_GET_UserTypeEmpCompWise_usercreation", parameters1);

                    // If info = 1, create an option in the dropdown
                    if (info == 1)
                    {
                        dropdownHtml += "<option value='" + TypeId + "'>" + item["UserTypeName"].ToString() + "</option>";
                    }
                }

                // Close the dropdown


                // Return the HTML for the dropdown
                return dropdownHtml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        [HttpPost]
        [Route("SaveModuleDtls")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]
        public async Task<ActionResult> SaveModuleDtls(GlobalModel gmodel)
        {
            try
            {
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(gmodel.Data);

                int result = await _service.AddWithTVPAsync("SP_BulK_Save_UserProfile_Access_Map_Dtls2", "@Data", "UserProfile_Access_Map_Dtls2_Table_Type", dt);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }






        [HttpPost]
        [Route("FetchEmailDtls")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]
        public async Task<Mail> FetchEmailDtls(Mail model)
        {
         
            try
            {
                Mail Mail = new Mail();
                // Create the parameters for the stored procedure
                var parameters = new Dictionary<string, object>
              {
                    { "@TenantId", model.Tenant_ID},
                    { "@REC_TYPE", "MailSetupDetails_UserCreation" },
                    { "@ApplicationId", 0},
                    { "@TenantMailSetupKey",model.TenantMailSetupKey},
                    { "@USER_MASTER_KEY",model.MailUserMaster.USER_MASTER_KEY}
              };

                // Call the service method to get the DataSet
                var dataSet = await _service.GetAllDatasetAsync("Sp_FetchEmailDtls", parameters);

                // Check if the DataSet contains any result sets
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    // Retrieve the ServiceDesk data (first table)
                    var SenderData = dataSet.Tables[0];

                    // Retrieve the associated users data (second table)
                    var RecieverData = dataSet.Tables[1];

                   
                    if (SenderData.Rows.Count > 0)
                    {
                        var row = SenderData.Rows[0];
                        Mail.TenantMailSetupKey = Convert.ToInt32(row["TenantMailSetupKey"]);
                        Mail.Tenant_ID = Convert.ToInt32(row["TenantId"]);
                        Mail.Application_ID = Convert.ToInt32(row["ApplicationId"]);
                        Mail.TenantMailSetupPurposeKey = Convert.ToInt32(row["PurposeId"]);
                        Mail.TenantMailSetupKey = Convert.ToInt32(row["TenantMailSetupKey"]);
                        Mail.SenderMail = row["SenderMail"].ToString();
                        Mail.SenderPassword = row["SenderPassword"].ToString();
                        Mail.MailSubject = row["Mailsubject"].ToString();
                        Mail.MailBody = row["MailBody"].ToString();
                        Mail.IsCC = Convert.ToInt32(row["IsCC"]);
                        Mail.IsAttachment = Convert.ToInt32(row["IsAttachment"]);
                        Mail.PageLink = row["PageLink"].ToString();
                        Mail.Parameter1 = row["parameter1"].ToString();
                        Mail.Parameter2 = row["parameter2"].ToString();
                        Mail.Parameter3 = row["parameter3"].ToString();
                        Mail.TimeSlotMinute = Convert.ToInt32(row["timeslotMinute"]);

                    }


                    var ReceiverData = dataSet.Tables[1];
                    if (ReceiverData.Rows.Count > 0)
                    {

                      
                        var lst = new List<ReceiverEmail>();
                        foreach (DataRow row in ReceiverData.Rows)
                        {
                            var mdl = new ReceiverEmail
                            {
                                TenantMailSetupKey = row["TenantMailSetupKey"] as int? ?? default,
                                TenantMailSetupDtlsKey = row["TenantMailSetupDtlsKey"] as int? ?? default,
                                ReceiverMail = row["ReceiverMail"]?.ToString() ?? string.Empty,

                            };

                            lst.Add(mdl);
                        }

                        Mail.ReceiverEmail= lst;

                    }
                    var UserMasterData = dataSet.Tables[2];
                    if (UserMasterData.Rows.Count > 0)
                    {
                        var urow = UserMasterData.Rows[0];
                        var MailUserMasterModel = new MailUserMaster();

                        MailUserMasterModel.USER_MASTER_KEY =Convert.ToInt32(urow["USER_MASTER_KEY"]);
                        MailUserMasterModel.Email_ID = urow["Email_ID"].ToString();
                        MailUserMasterModel.FullName = urow["FullName"].ToString();
                        MailUserMasterModel.Username = urow["Username"].ToString();
                        MailUserMasterModel.UserPassword = urow["UserPassword"].ToString();
                        MailUserMasterModel.UserMailTypeCode = urow["UserMailTypeCode"].ToString();
                        MailUserMasterModel.Pin = urow["Pin"].ToString();


                        Mail.MailUserMaster=MailUserMasterModel;

                    }


                }
                    // Return the ServiceDesk model as the response
                    return Mail;



            }
            catch
            {
                throw;
            }
        }




        [HttpPost]
        [Route("BlockUser")]
        [SwaggerOperation(Tags = new[] { "User Creation" })]
        public async Task<ActionResult> BlockUser(GlobalModel gmodel)
        {
            try
            {
                // var gm = gmodel;
                var model = JsonConvert.DeserializeObject<BlockUserModel>(gmodel.Data);

                var parameters = new Dictionary<string, object>
                {
                       { "@TenantID", gmodel.TenantID},
                       { "@ApplicationId", gmodel.ApplicationId },
                       { "@CompanyId", gmodel.CompanyID },
                       { "@userID", gmodel.userID },

                       { "@Key", model.Key },
                       { "@USER_MASTER_KEY", model.UserID }

                 };
                var result = await _service.AddAsync("SP_BLOCK_USER", parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }





        #endregion


        #region RoleMaster



        [HttpGet]
        [Route("GetRoleMaster/{TenantId}")]
        [SwaggerOperation(Tags = new[] { "Role Master" })]
        public async Task<ActionResult> GetRoleMaster(int TenantId) //Populating data in the grid
        {
            try
            {


                var parameters = new Dictionary<string, object>
                    {
                        { "@REC_TYPE", "All"},
                          { "@TenantId",TenantId },//parameters as provided in the respective SP
                        { "@UserTypeId", 0},


            };
                DataTable dt = await _service.GetDataAsync("SP_GET_ROLE_MASTER", parameters);
                return Ok(JsonConvert.SerializeObject(dt));
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }

        //SAVE function

        [HttpPost]
        [Route("SaveRoleMaster")]
        [SwaggerOperation(Tags = new[] { "Role Master" })]
        public async Task<ActionResult> SaveRoleMaster(GlobalModel gmodel)
        {
            try
            {
                RoleMasterModel ppModel = JsonConvert.DeserializeObject<RoleMasterModel>(gmodel.Data);


                var parameters = new Dictionary<string, object>
                                    {
                                         { "@REC_TYPE",ppModel.UserTypeId == 0 ? "INSERT" : "UPDATE" },
                                        { "@TenantId", gmodel.TenantID },
                                        { "@UserTypeId", ppModel.UserTypeId },
                                        { "@UserTypeCode", ppModel.UserTypeCode},
                                        { "@UserTypeName", ppModel.UserTypeName },
                                        { "@UserTypeDescription", ppModel.UserTypeDescription },
                                        { "@LevelUpto", ppModel.LevelUpto },
                                        { "@AutoInApproval", ppModel.AutoInApproval },
                                        { "@Hierarchy ", ppModel.Hierarchy },
                                        { "@CreatedBy", gmodel.userID },


                };


                //}
                int result = await _service.AddAsync("SP_CRUD_ROLE_MASTER", parameters);

                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }


        //EDIT function

        [HttpGet]
        [Route("GetRoleByID/{id}")]
        [SwaggerOperation(Tags = new[] { "Role Master" })]
        public async Task<ActionResult> GetRoleByID(int id) //Populating individual data by id
        {
            try
            {


                var parameters = new Dictionary<string, object>
                    {
                        { "@REC_TYPE", "By_Id"},
                          { "@TenantId", 0},
                        { "@UserTypeId", id},


            };
                DataTable dt = await _service.GetDataAsync("SP_GET_ROLE_MASTER", parameters);
                return Ok(JsonConvert.SerializeObject(dt));
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }

        //DELETE function

        [HttpGet]
        [Route("DeleteRoleMaster/{id}/{TenantId}")]
        [SwaggerOperation(Tags = new[] { "DeleteRoleMaster" })]
        public async Task<ActionResult> DeleteRoleByID(int id, int TenantId)   //Delete function
        {
            try
            {

                var parameters = new Dictionary<string, object>
                                    {
                                         { "@REC_TYPE", "DELETE" },
                                        { "@UserTypeId", id },
                                        { "@TenantId", TenantId },
                                        { "@UserTypeCode", "" },
                                        { "@UserTypeName", "" },
                                        { "@UserTypeDescription", "" },
                                           { "@LevelUpto", "" },
                                         { "@AutoInApproval", ""},
                                          { "@Hierarchy ", 0.00 },
                                     
                                        { "@CreatedBy", 0 },

                };


                //}
                int result = await _service.UpdateAsync("SP_CRUD_ROLE_MASTER", parameters);

                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }
        #endregion







    





    }
}
