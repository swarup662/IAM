using CommonUtility.Interface;
using CommonUtility.SharedModels;
using IAM_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
//using Swashbuckle.Swagger.Annotations;
using System.Data;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IAM_API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDTOService _service;
        private readonly IEncryptDecrypt _encryptDecrypt;
        private readonly ILogger<AuthorizationController> _logger;
        private readonly string _encryptionKey;
        private readonly string ApiKey;
        private readonly ICommonService _commonService;

        public HomeController(IConfiguration configuration, IDTOService service, ILogger<AuthorizationController> logger, IEncryptDecrypt enc, ICommonService commonService)
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


        #region Dashboard





        [HttpPost]
        [Route("Dashboard")]
        [SwaggerOperation(Tags = new[] { "Dashboard" })]
        public async Task<ActionResult> Dashboard(GlobalModel gmodel)
        {
            try
            {
                ;
                DataSet newDataSet = new DataSet();
                //-------------Coscenter divisdion warehouse---------------------------//
                var parameters = new Dictionary<string, object>
                    {
                        { "@REC_TYPE", "" }

                    };
                DataSet dt = await _service.GetAllDatasetAsync("SP_GET_DASHBOARD", parameters);



                foreach (DataTable table in dt.Tables)
                {
                    // Clone the table (structure and data)
                    DataTable clonedTable = table.Copy();

                    // Check the table's index in the dataset and rename accordingly
                    if (dt.Tables.IndexOf(table) == 0)
                    {
                        clonedTable.TableName = "Users";
                        newDataSet.Tables.Add(clonedTable);
                    }
                   


                    
                }



                return Ok(JsonConvert.SerializeObject(newDataSet));
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



     





















    }
}
