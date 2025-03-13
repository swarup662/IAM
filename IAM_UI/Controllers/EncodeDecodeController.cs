using CommonUtility.Interface;
using IAM_UI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace IAM_UI.Controllers
{
    public class EncodeDecodeController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly ILoggerService _logger;
        private readonly string _baseUrlGlobal;
        private readonly ICommonService _commonService;
        private readonly string _ApiKey;
        private readonly string _MailBody;
        private readonly string BaseUrlAuth;

        private readonly string encryptionKey;
        private readonly string BaseUrlOldPassword;
        private readonly IGlobalModelService _globalModelService;


        private readonly IEncryptDecrypt _encodedecode;


        public EncodeDecodeController(IConfiguration configuration, ICommonService commonService, ILoggerService logger, IGlobalModelService globalModelService, APIResultsValue apirelultvalues, IEncryptDecrypt encodedecode)
        {
            _configuration = configuration;
            _baseUrlGlobal = configuration["BaseUrlGlobal"];
            _commonService = commonService;
            _ApiKey = configuration["Apikey"];
            _MailBody = configuration["MailBody"];
            _logger = logger;
            BaseUrlAuth = configuration["BaseUrlAuth"];

            BaseUrlOldPassword = configuration["BaseUrlLogin"];
            encryptionKey = configuration["encryptionKey"];
            _globalModelService = globalModelService;
     
            _encodedecode = encodedecode;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> EncryptDecrypt([FromBody] EncodeDecodeModel request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Txt))
                {
                    return BadRequest("Invalid input: Text is required.");
                }

                string result = string.Empty;

                if (request.Type == 1)
                {
                    result = await _encodedecode.EncryptAsync(request.Txt, encryptionKey);
                }
                else if (request.Type == 2)
                {
                    result = await _encodedecode.DecryptAsync(request.Txt, encryptionKey);
                }
                else
                {
                    return BadRequest("Invalid Type. Use 1 for Encrypt and 2 for Decrypt.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }


        // Define a model for better request handling
        public class EncodeDecodeModel
        {
            public string Txt { get; set; }
            public int Type { get; set; }
        }



    }
}
