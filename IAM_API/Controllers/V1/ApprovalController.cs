using CommonUtility.Interface;
using CommonUtility.SharedModels;
using IAM_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IAM_API.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.ComponentModel.Design;
namespace IAM_API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ApprovalController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IDTOService _service;
        private readonly IEncryptDecrypt _encryptDecrypt;
        private readonly ILogger<AuthorizationController> _logger;
        private readonly string _encryptionKey;
        private readonly string ApiKey;
        private readonly ICommonService _commonService;


        public ApprovalController(IConfiguration configuration, IDTOService service, ILogger<AuthorizationController> logger, IEncryptDecrypt enc, ICommonService commonService)
        {
            _service = service;
            _logger = logger;
            _encryptDecrypt = enc;
            _configuration = configuration;
            _commonService = commonService;

        }


        [HttpGet]
        [Route("FECTCH_APPROVAL_LEVEL_ONEDETAILS")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<List<ApprovalSetupAPIModel>> GetAPPROVALLEVELONEDETAILS()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {

                };

                DataSet ds = await _service.GetAllDatasetAsync("SP_FETCH_APPROVAL_LEVEL_ONE_DETAILS", parameters);
                List<ApprovalSetupAPIModel> lst = new List<ApprovalSetupAPIModel>();

                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    var info = new ApprovalSetupAPIModel
                    {
                        Approval_level_One_key = Convert.ToInt32(item["APPROVAL_LVL1_KEY"]),
                        ApprovalTypeDesc = item["ApprovalTypeDesc"].ToString(),
                        ModuleName = item["ModuleName"].ToString(),
                        COMPANY_NAME = item["COMPANY_NAME"].ToString()
                    };
                    lst.Add(info);
                }

                return lst;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("FetchApplicationtype")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<List<object>> GetApplicationType()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {

                };
                DataSet ds = await _service.GetAllDatasetAsync("SP_FETCH_APPROVALSETUP_TYPE", parameters);
                List<object> types = new List<object>();

                foreach (DataRow item in ds.Tables[0].Rows)
                {

                    types.Add(
                        new { value = Convert.ToInt32(item["APPROVAL_TYPE_KEY"]), text = item["ApprovalTypeDesc"].ToString() }
                        );

                }

                return types;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        [HttpGet]
        [Route("FetchApplicationNames")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<List<object>> GetApplicationNames()
        {

            try
            {
                var parameters = new Dictionary<string, object>
                {

                };
                DataSet ds = await _service.GetAllDatasetAsync("SP_FETCH_APPROVAL_SETUP", parameters);
                List<object> types = new List<object>();
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    types.Add(
                        new { value = Convert.ToInt32(item["ApplicationId"]), text = item["ApplicationName"].ToString() }
                        );


                }

                return types;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        [HttpGet]
        [Route("FetchCompanyName")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<string> GetCompanyNames()
        {
            string dropdown = "<select>";
            dropdown += $"<option value='{0}'>{"--Select--"}</option>";
            try
            {
                string ii = "";
                var parameters = new Dictionary<string, object>
                {

                };
                DataSet ds = await _service.GetAllDatasetAsync("SP_FETCH_APPROVAL_SETUP_GET_COMPANIES", parameters);
                ii += "<ul class='list-group' id='ItemList'>";
                foreach (DataRow item in ds.Tables[0].Rows)
                {

                    ii += "<li class='list-group-item'>";
                    ii += "<div class='checkbox'>";
                    if (Convert.ToInt32(item["CompanyId"]) == 100) {

                        ii += "<input type='checkbox' id='" + Convert.ToInt32(item["CompanyId"]) + "' name='checkbox' value='" + Convert.ToInt32(item["CompanyId"]) + "' class='checkCompany' onchange='toggleCompanySelection()' />&nbsp;&nbsp;&nbsp;";
                    }
                    else
                    {
                        ii += "<input type='checkbox' id='" + Convert.ToInt32(item["CompanyId"]) + "' name='checkbox' value='" + Convert.ToInt32(item["CompanyId"]) + "' class='checkCompany' onchange='updateSelectAll()' />&nbsp;&nbsp;&nbsp;";

                    }

                    ii += "<label for='" + Convert.ToInt32(item["CompanyId"]) + "'>" + item["COMPANY_NAME"].ToString() + "</label>";
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



        [HttpGet]
        [Route("FetchUserType")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<List<object>> GetUserType()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@REC_TYPE","All"},
                    { "@UserTypeId",0}
                };
                DataSet ds = await _service.GetAllDatasetAsync("SP_GET_ROLE_MASTER", parameters);
                List<object> types = new List<object>();

                foreach (DataRow item in ds.Tables[0].Rows)
                {

                    types.Add(
                        new { value = Convert.ToInt32(item["UserTypeId"]), text = item["UserTypeName"].ToString() }
                        );


                }


                return types;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        [HttpGet]
        [Route("Fetch_Operational_Mode")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<List<object>> Fetch_Operational_Mode()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@REC_TYPE","All"},
                    { "@Operational_ModeId",0}
                };
                DataSet ds = await _service.GetAllDatasetAsync("SP_GET_OperationalMode", parameters);
                List<object> types = new List<object>();

                foreach (DataRow item in ds.Tables[0].Rows)
                {

                    types.Add(
                        new { value = Convert.ToInt32(item["ActionId"]), text = item["ActionName"].ToString() }
                        );
                  

                }


                return types;
            }
            catch (Exception ex)
            {

                throw;
            }

        }



        [HttpGet]
        [Route("Getapprovalleveldetails_id/{id}")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<ActionResult<IEnumerable<ApprovalSetupAPIModel>>> GetApprovalSetupDetails(int id)
        {
            try
            {
                DataTable dataTable = await GetApprovalSetupById(id);
                List<ApprovalSetupAPIModel> approvalList = new List<ApprovalSetupAPIModel>();

                foreach (DataRow row in dataTable.Rows)
                {
                    var info = new ApprovalSetupAPIModel
                    {
                        Approval_level_One_key = Convert.ToInt32(row["APPROVAL_LVL1_KEY"]),
                        Application_Name_Id = Convert.ToInt32(row["ApplicationId"]),
                        Application_Main_Menu_Id = Convert.ToInt32(row["ModuleParentId"]),
                        Application_Module_Id = Convert.ToInt32(row["ModuleId"]),
                        COMPANY_KEY = Convert.ToString(row["COMPANY_KEY"]),
                        Approval_Type = Convert.ToInt32(row["APPROVAL_TYPE_KEY"]),
                        Approval_Path_setup_type = Convert.ToInt32(row["PATH_SETUP_TYPE"]),    // rdb
                        Approval_choose_ = Convert.ToInt32(row["APPROVER_OPTION"]),
                        OprMode_Id = Convert.ToInt32(row["OPERATIONAL_MODE"]),
                    };

                    approvalList.Add(info);
                    approvalList.Add(info);
                }

                return Ok(approvalList);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetApprovalSetupById")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<DataTable> GetApprovalSetupById(int id)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@ID", id }
                };

                return await _service.GetDataAsync("SP_FETCH_APPROVAL_LEVEL_ONE_BY_ID", parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("FetchApplicationMainmenu/{RecType}/{Application_Name_Id}/{Menu_Id}")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<List<object>> GetApplicationMainMenu(string RecType, int Application_Name_Id, int Menu_Id)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@RecType", RecType},
                    { "@Application_Id", Application_Name_Id.ToString() },
                    { "@Menu_Id",Menu_Id.ToString()}
                };

                DataSet ds = await _service.GetAllDatasetAsync("SP_GET_APPROVAL_SETUP_MAIN_MENU", parameters);
                List<object> types = new List<object>();

                foreach (DataRow item in ds.Tables[0].Rows)
                {

                    types.Add(
                        new { value = Convert.ToInt32(item["ModuleId"]), text = item["ModuleName"].ToString() }
                        );


                }


                return types;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        [HttpPost]
        [Route("SaveApproval_Level_One")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<int> SaveApprovalSetup_Level_one(ApprovalSetupAPIModel model)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"@APPROVAL_LVL1_KEY", model.Approval_level_One_key.ToString()},
                    {"@APPROVAL_TYPE_KEY", model.Approval_Type.ToString() },
                    {"@PATH_SETUP_TYPE",model.Approval_Path_setup_type.ToString()},  // rdb //Approval_Path_setup_type
                    {"@APPROVER_OPTION ",model.Approval_choose_.ToString()},         // rdb //Approval_choose_
                    {"@CompanyKey",model.COMPANY_KEY},
                    {"@TenantId",model.TenantID},
                    {"@USER_ID",model.UserKey },
                    {"@ApplicationId",model.Application_Name_Id.ToString() },
                    {"@OperationalModeID",model.OprMode_Id.ToString() },
                    {"@ModuleId",model.Application_Module_Id.ToString()}
                };

            return await _service.AddAsync("SP_APPROVAL_L1_SAVE_EDIT_NEW", parameters);
        }

        [HttpGet]
        [Route("FECTCH_APPROVAL_LEVEL_TWODETAILS/{Approval_level_One_key}")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<string> GetApproval_Details_two(int Approval_level_One_key)
        {
            string ii = "";
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Rec_Type","STEP2" },
                    {"@Label_ONE_KEY", Approval_level_One_key.ToString() },
                    {"@Label_TWO_KEY", "" },
                     {"@companyKey",0}
                };


                DataSet ds = await _service.GetAllDatasetAsync("SP_FETCH_DETAILS_APPROVAL_SETUP_THREE", parameters);
                List<object> types = new List<object>();

                int rowCount = ds.Tables[0].Rows.Count; // Get total rows
                int currentIndex = 0; // Track the current index

                foreach (DataRow item in ds.Tables[0].Rows)
                {


                    ii += "<tr>";
                    //ii += "<td style='display:none'>" + Convert.ToInt32(item["APPROVAL_LVL1_KEY"]) + "</td>";
                    //ii += "<td style='display:none'>" + Convert.ToInt32(item["APPROVAL_LVL2_KEY"]) + "</td>";
                    //    ii += "<td style='display:none'>" + Convert.ToInt32(item["APPROVAL_User_Type_Key"]) + "</td>";
                    ii += "<td>" + Convert.ToInt32(item["STEP_NO"]) + "</td>";
                    ii += "<td>" + item["USER_TYPE_DESC"].ToString() + "</td>";
                    ii += "<td>";

                    // Add delete button only for the last row
                    if (currentIndex == rowCount - 1)
                    {
                        ii += "<a href='javascript:;' class='action-icon'>";
                        ii += "<i class='mdi mdi-delete' onclick='DeleteApprovalStep(" +
                              Convert.ToInt32(item["APPROVAL_LVL1_KEY"]) + "," +
                              Convert.ToInt32(item["APPROVAL_LVL2_KEY"]) +
                              ")' data-toggle='tooltip' title='Delete' style='color: red'></i>";
                    }

                    ii += "</td>";
                    ii += "</tr>";

                    currentIndex++;
                }
                return ii;


            }
            catch (Exception ex)
            {

                throw;
            }



        }

        [HttpPost]
        [Route("SaveApproval_Level_two")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<int> SaveApprovalSetupLevelTwo(ApprovalSetupAPIModel model)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"@APPROVAL_LVL2_KEY",model.Approval_level_two_key.ToString() },
                    {"@APPROVAL_LVL1_KEY",model.Approval_level_One_key.ToString() },
                    {"@STEP_NO",model.StepNo},
                    { "@UserType_key",model.arr_opt},
                      {"@TenantId",model.TenantID},
                    {"@USER_ID",model.UserKey }

                };

            return await _service.AddAsync("SP_APPROVAL_L2_SAVE_EDIT_NEW", parameters);
        }
        [HttpGet]
        [Route("GetSelectedCompany/{LevelOneId}")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task <List<object>> GetSelectedCompany(int LevelOneId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {  {"@Rec_Type","GET_SELECTED_COMPANY" },
                    {"@Label_ONE_KEY", LevelOneId.ToString() },
                      {"@Label_TWO_KEY", "" },
                     {"@companyKey",0}
                };
                DataSet DS = await _service.GetAllDatasetAsync("SP_FETCH_DETAILS_APPROVAL_SETUP_THREE", parameters);
                List<object> types = new List<object>();
                foreach (DataRow item in DS.Tables[0].Rows)
                {
                    types.Add(

                        new
                        {
                            value = Convert.ToInt32(item["CompanyId"]),
                            text = item["COMPANY_NAME"].ToString()

                        }
                        );


                }

                return types;



            }
            catch (Exception ex)
            {

                throw;
            }
        }



        [HttpGet]
        [Route("FetchApprovalDetails_level_three/{Approval_level_One_key}/{CompanyID}")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<string> GetApproval_Details_Setup_three(int Approval_level_One_key,int CompanyID)
        {
            string ii = "";
            try
            {
                var parameters = new Dictionary<string, object>
                {  {"@Rec_Type","STEP3" },
                    {"@Label_ONE_KEY", Approval_level_One_key.ToString() },
                                       {"@Label_TWO_KEY", "" },
                    {"@companyKey",CompanyID.ToString()}
                };

                DataSet ds = await _service.GetAllDatasetAsync("SP_FETCH_DETAILS_APPROVAL_SETUP_THREE", parameters);
                List<object> types = new List<object>();

                foreach (DataRow item in ds.Tables[0].Rows)
                {


                    ii += "<tr>";
             
                    ii += "<td>" + Convert.ToInt32(item["STEP_NO"]) + "</td>";
                    ii += "<td>" + item["USER_TYPE_DESC"].ToString() + "</td>";
                    ii += "<td><input type=\"search\" class=\"form-control\" placeholder=\"Search Employee Name\" id=\"searchemployee" + Convert.ToInt32(item["APPROVAL_User_Type_Key"])+ Convert.ToInt32(item["STEP_NO"]) + "\" autocomplete=\"off\" onclick='srch(" + Convert.ToInt32(item["APPROVAL_User_Type_Key"])+Convert.ToInt32(item["STEP_NO"]) +","+ Convert.ToInt32(item["APPROVAL_User_Type_Key"]) +","+ Convert.ToInt32(item["CompanyId"]) + ")'/></td>";
                    // added 2day
                    ii += "<td style='display:none'><input type='hidden' id='employee_master_key_" + Convert.ToInt32(item["APPROVAL_User_Type_Key"])+Convert.ToInt32(item["STEP_NO"]) + "'/></td>";
                    ii += "<td><button type='button' class='btn btn-primary save-btn' style='display:none;' " +
                           "id='save-btn-" + Convert.ToInt32(item["APPROVAL_User_Type_Key"]) + Convert.ToInt32(item["STEP_NO"]) + "' " +
                           "onclick='SaveApproval_Lavel3(" + Convert.ToInt32(item["APPROVAL_LVL1_KEY"]) + "," + Convert.ToInt32(item["APPROVAL_LVL2_KEY"]) + "," + Convert.ToInt32(item["APPROVAL_User_Type_Key"])+ Convert.ToInt32(item["STEP_NO"]) + ")'>" +
                           "Save</button></td>";


                    ii += "</td>";
                    ii += "</tr>";


                }
                return ii;


            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet]
        [Route("DeleteApprovalStep/{Approval_level_One_key}/{Approval_level_Two_key}")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]

        public async Task<int> DeleteApprovalStep(int Approval_level_One_key, int Approval_level_Two_key)
        {
            try
            {

                var parameters = new Dictionary<string, object>
                {  {"@Rec_Type","DELETE_STEP" },
                   {"@Label_ONE_KEY", Approval_level_One_key.ToString() },
                   {"@Label_TWO_KEY", Approval_level_Two_key.ToString() },
                     {"@companyKey",0}
                };

                return await _service.DeleteAsync("SP_FETCH_DETAILS_APPROVAL_SETUP_THREE", parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("Employee/{Prefix}/{UserType_Key}/{Company_Key}")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<IEnumerable<ApprovalSetupAPIModel>> GetUserDetails(String Prefix,int UserType_Key,int Company_Key)
        {
            try
            {
                DataSet dataSet = await GetEmployee(Prefix, UserType_Key, Company_Key);
                List<ApprovalSetupAPIModel> lst = new List<ApprovalSetupAPIModel>();
                if (dataSet != null)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        var info = new ApprovalSetupAPIModel();
                     
                        info.employee_master_key = Convert.ToInt32(row["EMPLOYEE_MASTER_KEY"]);
                        info.Employee_Name = row["Employee_Name"].ToString();

                        lst.Add(info);
                    }

                }
                return lst;
            }
            catch
            {
                throw;
            }
        }
        [HttpGet]
        [Route("GetEmployee")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<DataSet> GetEmployee(string Prefix, int UserType_Key, int Company_Key)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {  {"@rectype","ApproverName"},
                    {"@Prefix",Prefix},
                    {"@UserType_Key",UserType_Key},
                    {"@Company_Key",Company_Key}
                };
             
                return await _service.GetAllDatasetAsync("SP_GET_EMPLOYEE_FOR_APPROVAL_SETUP", parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("SaveApprovalSetupThree")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<int> SaveApprovalSetupThree(ApprovalSetupAPIModel model)
        {
              var parameters = new Dictionary<string, object>
                {
                    {"@APPROVAL_LVL2_KEY",model.Approval_level_two_key.ToString() },
                    {"@APPROVAL_LVL1_KEY",model.Approval_level_One_key.ToString() },
                    {"@Company_Key",model.COMPANY_KEY },
                    {"@APPROVAL_LVL3_KEY",model.Approval_level_three_key.ToString() },
                    {"@EMPLOYEE_MASTER_KEY",model.employee_master_key },
                    {"@TenantId",model.TenantID},
                    {"@Ent_User_Key",model.UserKey }
                };

            return await _service.AddAsync("SP_APPROVAL_L3_SAVE_EDIT", parameters);
        }

        [HttpGet]
        [Route("Fetch_L3_Dtls/{Approval_level_One_key}")]
        [SwaggerOperation(Tags = new[] { "Approval Setup" })]
        public async Task<string> Fetch_L3_Dtls(int Approval_level_One_key)
        {
            string ii = "";
            try
            {
                var parameters = new Dictionary<string, object>
                {  {"@Rec_Type","STEP3_AccessDtls" },
                    {"@Label_ONE_KEY", Approval_level_One_key.ToString() },
                    {"@Label_TWO_KEY", "" },
                    {"@companyKey",""}
                };

                DataSet ds = await _service.GetAllDatasetAsync("SP_FETCH_DETAILS_APPROVAL_SETUP_THREE", parameters);
                List<object> types = new List<object>();

                foreach (DataRow item in ds.Tables[0].Rows)
                {


                    ii += "<tr>";
                 
                       ii += "<td>" + item["COMPANY_NAME"].ToString() + "</td>";
                       ii += "<td>" + Convert.ToInt32(item["STEP_NO"]) + "</td>";
                       ii += "<td>" + item["USER_TYPE_DESC"].ToString() + "</td>";
                       ii += "<td>" + item["Employee_Name"].ToString() + "</td>";

                    ii += "</tr>";


                }
                return ii;


            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
