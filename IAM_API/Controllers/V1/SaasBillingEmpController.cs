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
    public class SaasBillingEmpController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IDTOService _service;
        private readonly IEncryptDecrypt _encryptDecrypt;
        private readonly ILogger<AuthorizationController> _logger;
        private readonly string _encryptionKey;
        private readonly string ApiKey;
        private readonly ICommonService _commonService;


        public SaasBillingEmpController (IConfiguration configuration, IDTOService service, ILogger<AuthorizationController> logger, IEncryptDecrypt enc, ICommonService commonService)
        {
            _service = service;
            _logger = logger;
            _encryptDecrypt = enc;
            _configuration = configuration;
            _commonService = commonService;

        }





        [HttpPost]
        [Route("PrintEmpInfo")]
        [SwaggerOperation(Tags = new[] { "Employee Information" })]
        public async Task<ActionResult> PrintEmpInfo(SaasBillingEmpModel model)
        {
            try
            { 
           
                  var parameters = new Dictionary<string, object>
                {
                   { "@REC_TYPE", "EmpIn" },
                   { "@Month", model.Month ?? 0 },  // Default to 0 if null (fetch all months)
                   { "@Year", model.Year ?? 0}
                };

                // Execute stored procedure and get results
                DataTable dt = await _service.GetDataAsync("RPT_SP_GET_UsersForBilling", parameters);
                // Return the modified DataTable as JSON
                return Ok(JsonConvert.SerializeObject(dt));
            }
            catch (Exception ex)
            {
                // Log the error for server-side debugging
                var errorResponse = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }





    }
}
