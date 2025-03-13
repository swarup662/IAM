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
using System.IO.Pipelines;
namespace IAM_API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UsersForBillingApiTrackerAPIController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IDTOService _service;
        private readonly IEncryptDecrypt _encryptDecrypt;
        private readonly ILogger<AuthorizationController> _logger;
        private readonly string _encryptionKey;
        private readonly string ApiKey;
        private readonly ICommonService _commonService;


        public UsersForBillingApiTrackerAPIController(IConfiguration configuration, IDTOService service, ILogger<AuthorizationController> logger, IEncryptDecrypt enc, ICommonService commonService)
        {
            _service = service;
            _logger = logger;
            _encryptDecrypt = enc;
            _configuration = configuration;
            _commonService = commonService;

        }


        [HttpPost]
        [Route("SaveUsersForBilling")]
        [SwaggerOperation(Tags = new[] { "UsersForBillingApiTrackerAPI" })]
        public async Task<IActionResult> SaveUsersForBilling( ApiCallTrackerSaveModel model)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                                   { "@TenantId", model.TenantId },
                                    { "@AplicationId", model.ApplicationId },
                                    { "@Month", model.Month },
                                    { "@Year", model.Year }
                };

                int result = await _service.AddAsync("SP_SAVE_USERS_FOR_BILLING", parameters);


                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }




        [HttpPost]
        [Route("GetApiTriggerMail")]
        [SwaggerOperation(Tags = new[] { "UsersForBillingApiTrackerAPI" })]
        public async Task<IActionResult> GetApiTriggerMail(ApiTriggerMailModel amodel)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                                   { "@TenantId", amodel.TenantId },
                                    { "@ApplicationId", amodel.ApplicationId },
                                    { "@PurposeId", amodel.PurposeId },
                                    { "@ApiTriggerMailKey", amodel.ApiTriggerMailKey }
                };

                DataSet r = await _service.GetAllDatasetAsync("Sp_Get_ApiTriggerMail", parameters);
                List<ApiTriggerMailModel> result = new List<ApiTriggerMailModel>();

                // Check if DataTable has any rows
                if (r.Tables[0].Rows.Count > 0)
                {
                    // Iterate over the DataTable rows
                    foreach (DataRow row in r.Tables[0].Rows)
                    {
                        ApiTriggerMailModel model = new ApiTriggerMailModel
                        {
                            TenantId = row.Field<int>("TenantId"),
                            ApplicationId = row.Field<int>("ApplicationId"),
                      
                            ApiTriggerMailKey = row.Field<int>("ApiTriggerMailKey"),
                            PurposeId = row.Field<int>("PurposeId"),
                            PurposeName = row.Field<string>("PurposeName"),
                            SenderMail = row.Field<string>("SenderMail"),
                            SenderPassword = row.Field<string>("SenderPassword"),
                            ReceiverMail = row.Field<string>("ReceiverMail")
                        };

                        result.Add(model);
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }








    }




    public class ApiCallTrackerSaveModel
    {
        public int TenantId { get; set; }
        public int ApplicationId { get; set; }
        public int Month { get; set; }

        public int Year { get; set; }
    }
}
