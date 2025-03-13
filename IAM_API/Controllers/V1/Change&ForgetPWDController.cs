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

namespace IAM_API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class Change_ForgetPWDController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IDTOService _service;
        private readonly IEncryptDecrypt _encryptDecrypt;
        
        private readonly string _encryptionKey;
        private readonly string ApiKey;
        private readonly ICommonService _commonService;


        public Change_ForgetPWDController(IConfiguration configuration, IDTOService service, IEncryptDecrypt enc, ICommonService commonService)
        {
            _service = service;
            _encryptDecrypt = enc;
            _configuration = configuration;
            _commonService = commonService;

        }
        #region Forget Password
        //[HttpPost]
        //[Route("AppliedToForgetPWD")]
        //[SwaggerOperation(Tags = new[] { "Forget Password" })]
        //public async Task<int> AppliedToForgetPWD(Mail model)
        //{
        //    var parameters = new Dictionary<string, object>
        //        {
        //            {"@UserID", model.UserID},
        //            {"@PhoneNo", model.Phone_No },
        //            {"@Email",model.Email}
                                        
        //        };

        //    return await _service.AddAsync("", parameters);
        //}
        #endregion
    }
}
