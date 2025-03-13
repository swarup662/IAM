using IAM_API.Models;
using CommonUtility.Interface; 
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Security.Claims; 
using System.Security.Cryptography; 
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using IAM_API.Interface;
using CommonUtility.SharedModels;

namespace IAM_API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IEncryptDecrypt _encryptDecrypt;
        private readonly IConfiguration _configuration;
        private readonly string _encryptKey;
        private readonly string passEncryptionKey;
        private readonly IDTOService _service;
        private readonly ICommonService _commonService;
        private readonly ITokenService _tokenService;
        public RSA PrivateKey { get; private set; }
        public RSA publicKey { get; private set; }
        string returndata="";

        public AuthController(IConfiguration configuration, IEncryptDecrypt encryptDecrypt, IDTOService service, ICommonService commonService, ITokenService tokenService)
        {
            _configuration = configuration;
            _encryptDecrypt = encryptDecrypt;
            _encryptKey = configuration["Data:Key"];
            passEncryptionKey = configuration["passEncryptionKey"];
            _service = service;
            _commonService = commonService;
            _tokenService = tokenService;
        }
        [HttpPost("OAuth")]
        public async Task<IActionResult> GetLogin(LoginModel loginModel)
        {
            Login login=new Login();

            if (loginModel == null)
            {
                return BadRequest("Login model cannot be null.");
            }
            //var pass = await _encryptDecrypt.DecryptAsync(loginModel.Password, passEncryptionKey);
            //loginModel.Password = pass;
            string userName = loginModel.Username;
            string password = loginModel.Password;

             
           
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Username and password must be provided.");
            }

            try
            {
                 
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    return BadRequest("Username or Password is missing.");
                }

                // Check user credentials
                var parameters = new Dictionary<string, object>
                {
                    { "@GET_REC_TYPE", "CHECK_USER" },
                    { "@USER_ID", 0 },
                    { "@USER_NAME", userName },
                    { "@USER_PASSWORD","" },
                    { "@LOCK_STATUS", 0 }
                };

                DataTable checkUsername = await _service.GetDataAsync("SP_UNI_USER", parameters);
                // Check if the username is invalid
                if (checkUsername == null || checkUsername.Rows.Count == 0)
                {
                    var wrongUser = new
                    {
                        returnMessage = "User login failed. Please check your credentials and try again.",
                        authToken = "",
                        //refreshToken = refreshToken
                    };

                    string encwrongUser =  JsonConvert.SerializeObject(wrongUser) ;

                    return Ok(new
                    {
                        returndata = encwrongUser
                    });
                }

                var list = await _commonService.ConvertDataTableToModelList<Login>(checkUsername);

                login = list.FirstOrDefault();
              ;

                // password checking
           

                // Check user credentials
                var parameters1 = new Dictionary<string, object>
                {
                    { "@GET_REC_TYPE", "CHECK_USER" },
                    { "@USER_ID", 0 },
                    { "@USER_NAME", userName },
                    { "@USER_PASSWORD",""  },
                    { "@LOCK_STATUS", 0 }
                };

                DataTable checkPassword = await _service.GetDataAsync("SP_UNI_USER", parameters);

               
                string pass = await _encryptDecrypt.DecryptAsync(login.Password, passEncryptionKey);
                if (pass != password)
                {
                    // Set Lock Status to 0 for an incorrect login due to wrong password
                    login.LockedStatus = 0;
                        var Lockparam = new Dictionary<string, object>
                        {
                            { "@GET_REC_TYPE", "VALID_USER" },
                            { "@USER_ID", login.User_Key.ToString() },
                            { "@USER_NAME", login.UserName },
                            { "@USER_PASSWORD", "" },
                            { "@LOCK_STATUS", login.LockedStatus.ToString() }
                        };

                    await _service.AddAsync("SP_UNI_USER", Lockparam);
                    var wrongpassword = new
                    {
                        returnMessage = "User login failed. Please check your password and try again.",
                        authToken = "",
                        //refreshToken = refreshToken
                    };

                    string encwrongpassword = JsonConvert.SerializeObject(wrongpassword);

                    return Ok(new
                    {
                        returndata = encwrongpassword 
                    });
                }

                if (!string.IsNullOrEmpty(login.msg))
                {
                    // Set Lock Status to 2 for a correct login but already locked
                    login.LockedStatus = 2;
                    var Lockparam = new Dictionary<string, object>
                        {
                            { "@GET_REC_TYPE", "VALID_USER" },
                            { "@USER_ID", login.User_Key.ToString() },
                            { "@USER_NAME", login.UserName },
                            { "@USER_PASSWORD", "" },
                            { "@LOCK_STATUS", login.LockedStatus.ToString() }
                        };

                    await _service.AddAsync("SP_UNI_USER", Lockparam);
                    var Dbmassage = new
                    {
                        returnMessage = "User login failed. " + login.msg,
                        authToken = "",
                        //refreshToken = refreshToken
                    };

                    string encDbmassage = JsonConvert.SerializeObject(Dbmassage);

                    return Ok(new
                    {
                        returndata = encDbmassage
                    });
                     
                }

                

                // Set Lock Status to 1 for a correct login
                login.LockedStatus = 1;
                var param = new Dictionary<string, object>
                {
                    { "@GET_REC_TYPE", "VALID_USER" },
                    { "@USER_ID", login.User_Key.ToString() },
                    { "@USER_NAME", login.UserName },
                    { "@USER_PASSWORD", "" },
                    { "@LOCK_STATUS", login.LockedStatus.ToString() }
                };

                await _service.AddAsync("SP_UNI_USER", param);

                string UserIdwithGuid = $"{login.User_Key}-{Guid.NewGuid()}";

                // Prepare the response
                var responseContent = new
                {
                    returnMessage = "success"
                    //authToken = accessToken,
                    //refreshToken = refreshToken
                };

                string encryptedResponse = JsonConvert.SerializeObject(responseContent);

                //Company Access For Login User
                Dictionary<string, object> CompanyList;



                if (login.UserCatagoryId == 1)
                {

                     CompanyList = new Dictionary<string, object>
                {

                    { "@GET_REC_TYPE", "USER_ACCESS" },
                    { "@TenantId", login.TenantId },
                    { "@USER_ID", login.User_Key.ToString() },
                    { "@CATAGORY_ID", login.UserCatagoryId.ToString() },
                    {"@APPLICATION_ID" ,"4  "}
                };
                }
                else
                {

                    CompanyList = new Dictionary<string, object>
                {

                    { "@GET_REC_TYPE", "USER_ACCESS" },
                    { "@TenantId", login.TenantId },
                    { "@USER_ID", login.User_Key.ToString() },
                    { "@CATAGORY_ID", login.UserCatagoryId.ToString() },
                    {"@APPLICATION_ID" ,"0"}
                };
                }

                DataSet ds = await _service.GetAllDatasetAsync("SP_UNI_COMPANY_ACCESS_USER_WISE_GLOBAL", CompanyList);
                
                if(ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    var company = new
                    {
                        returnMessage = "This User Have Not any Company Access",
                        authToken = "",
                        //refreshToken = refreshToken
                    };

                    string enccompany = JsonConvert.SerializeObject(company);

                    return Ok(new
                    {
                        returndata = enccompany
                    });
                }
                string encryptedUserAuthorization = JsonConvert.SerializeObject(ds);

            var  ParentCompany = new Dictionary<string, object>
                 {

                     { "REC_TYPE", "USER_ACCESS" },
                     { "@TenantId", login.TenantId }
                     
                 };

                DataSet pds = await _service.GetAllDatasetAsync("SP_GET_PARENTCOMPANYID", ParentCompany);
                if (pds == null || ds.Tables[0].Rows.Count == 0)
                {
                    var parentcompany = new
                    {
                        returnMessage = "This User Have Not Any Panent Company",
                        authToken = "",
                        //refreshToken = refreshToken
                    };

                    string pnccompany = JsonConvert.SerializeObject(parentcompany);

                    return Ok(new
                    {
                        returndata = pnccompany
                    });
                }
                //var UserParentCompanyModel = await _commonService.ConvertDataTableToModelList<ParentCompanyModel>(pds.Tables[0]);

                List<ParentCompanyModel> UserParentCompanyModellst = new List<ParentCompanyModel>();

                foreach (DataRow item in pds.Tables[0].Rows)
                {
                    var info = new ParentCompanyModel
                    {
                        TenantId = Convert.ToInt64(item["TenantId"]),
                        CompanyId = Convert.ToInt64(item["CompanyId"]), 
                        CompanyName = item["CompanyName"].ToString(),
                      
                    };
                    UserParentCompanyModellst.Add(info);
                }
                string UserParentCompany = JsonConvert.SerializeObject(UserParentCompanyModellst.FirstOrDefault());

                return Ok(new
                {
                    returndata = encryptedResponse,
                    returnUserAuthorization= encryptedUserAuthorization,
                    UniqueUserId= UserIdwithGuid,
                    returnCategoryId = login.UserCatagoryId.ToString(),
                    returnUserParentCompany = UserParentCompany,
                    token_type = "Bearer",
                    expires_in = 3600
                });
            }
            catch (Exception ex)
            {
                // Capture detailed error information
                var errorDetails = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Source = ex.Source,
                    // Optionally include other relevant details
                };
                // Log the exception if needed
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "An error occurred while processing your request.",
                    details = errorDetails
                });
            }
        }
        [HttpPost]
        [Route("OAuthToken")]
        public async Task<IActionResult> TokenGenerate(GlobalModel globalModel)
        {
            if (globalModel == null)
            {
                return BadRequest("Login model cannot be null.");
            }

            //GlobalModel globalModel = JsonConvert.DeserializeObject<GlobalModel>(await _encryptDecrypt.DecryptAsync(encKey, _encryptKey));
            //Fatch User Details
            // Check user credentials
            var parameters = new Dictionary<string, object>
            {
                { "@GET_REC_TYPE", "VALID_USER_TOKEN" },
                { "@USER_ID", globalModel.userID.ToString() },
                { "@USER_NAME", "" },
                { "@USER_PASSWORD", "" },
                { "@LOCK_STATUS", 0 }
            };

            DataTable UserDetails = await _service.GetDataAsync("SP_UNI_USER", parameters);
            var UserList = await _commonService.ConvertDataTableToModelList<Login>(UserDetails);
            Login login = UserList.FirstOrDefault();
            // Fetch tenant credentials
            var parameter = new Dictionary<string, object>
                {
                    { "@GET_REC_TYPE", "GET_TENANT_CREDENTIALS" },
                    { "@USER_ID",globalModel.userID.ToString() },
                    { "@USER_NAME", "" },
                    { "@USER_PASSWORD", "" },
                    { "@LOCK_STATUS", 0 }
                };

            DataTable tenantCredentials = await _service.GetDataAsync("SP_UNI_USER", parameter);
            var listTenant = await _commonService.ConvertDataTableToModelList<TenantAuthSetting>(tenantCredentials);
            TenantAuthSetting tenantAuthSetting = listTenant.FirstOrDefault();

            if (tenantAuthSetting == null)
            {
                var wrongMsg = new
                {
                    returnMessage = "Tenant Credentials Not Found",
                    authToken = "",
                    //refreshToken = refreshToken
                };

                string encwrongMsg = JsonConvert.SerializeObject(wrongMsg);

                return Ok(new
                {
                    returndata = encwrongMsg
                });
            }
            //// Generate JWT Access Token
            //RSA privateKey = RSA.Create();

            //privateKey.ImportFromPem(tenantAuthSetting.PrivateKey);

            ////

            //var claims = new[]
            //{
            //        new Claim(JwtRegisteredClaimNames.Sub,globalModel.Data ),
            //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //        new Claim(JwtRegisteredClaimNames.Iss, tenantAuthSetting.Issuer), // Issuer 
            //        new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(60).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // Expiration
            //        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // Issued At
            //        new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // Not Before 
            //        new Claim("Employee_Key", globalModel.userID.ToString()),
            //        new Claim("UserName", login.UserName),
            //        new Claim("UserCatagoryId", login.UserCatagoryId.ToString()),
            //        new Claim("TenantID", tenantAuthSetting.TenantID.ToString()),
            //        new Claim("ApplicationId",globalModel.ApplicationId.ToString()),
            //        new Claim("CompanyId",globalModel.CompanyID.ToString())
            //};

            //var signingCredentials = new SigningCredentials(new RsaSecurityKey(privateKey), SecurityAlgorithms.RsaSha256);

            //var jwt = new JwtSecurityToken(
            //    issuer: tenantAuthSetting.Issuer,
            //    claims: claims,
            //    notBefore: DateTime.UtcNow,
            //    expires: DateTime.UtcNow.AddMinutes(60),
            //    signingCredentials: signingCredentials
            //);

            var accessToken = "";

            //Module Access For Login User
            var CompanyList = new Dictionary<string, object>
                {
               
                    { "@GET_REC_TYPE", "ROLE_ACCESS" },
                      { "@TenantId", login.TenantId },
                    { "@USER_ID", login.User_Key.ToString() },
                    { "@CATAGORY_ID", login.UserCatagoryId.ToString() },
                    {"@APPLICATION_ID" ,globalModel.ApplicationId.ToString()}
                };

            DataSet ds = await _service.GetAllDatasetAsync("SP_UNI_COMPANY_ACCESS_USER_WISE_GLOBAL", CompanyList);
            
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                var company = new
                {
                    returnMessage = "This User Have Not any Company Access",
                    authToken = "",
                    //refreshToken = refreshToken
                };

                string enccompany = JsonConvert.SerializeObject(company);

                return Ok(new
                {
                    returndata = enccompany
                });
            }
            //string encryptedUserAuthorization = await _encryptDecrypt.EncryptAsync(JsonConvert.SerializeObject(ds), _encryptKey);
            var ModuleAccessData = JsonConvert.SerializeObject(ds);

            //User Details For Login User
            var User = new Dictionary<string, object>
                {
              
                    { "@GET_REC_TYPE", "USER_DETAIL" },
                      { "@TenantId", login.TenantId },
                    { "@USER_ID", login.User_Key.ToString() },
                    { "@CATAGORY_ID", login.UserCatagoryId.ToString() },
                    {"@APPLICATION_ID" ,globalModel.ApplicationId.ToString()}
                };

            DataTable dt = await _service.GetDataAsync("SP_UNI_COMPANY_ACCESS_USER_WISE_GLOBAL", User);

            if (dt == null || dt.Rows.Count == 0)
            {
                var company = new
                {
                    returnMessage = "User Details Are Missed Out ",
                    authToken = "",
                    //refreshToken = refreshToken
                };

                string enccompany = JsonConvert.SerializeObject(company);

                return Ok(new
                {
                    returndata = enccompany
                });
            }
            var UserDetail = JsonConvert.SerializeObject(dt);

            //var refreshToken = _tokenService.GenerateRefreshToken(tenantAuthSetting.TenantID.ToString());
            var Data = globalModel.Data;

            var Approval = new Dictionary<string, object>
                {
                
                    { "@GET_REC_TYPE", "APPROVAL" },
                     { "@TenantId",login.TenantId },
                    { "@USER_ID", login.User_Key.ToString() },
                    { "@CATAGORY_ID", login.UserCatagoryId.ToString() },
                    {"@APPLICATION_ID" ,globalModel.ApplicationId.ToString()}
                };

            DataTable adt = await _service.GetDataAsync("SP_UNI_COMPANY_ACCESS_USER_WISE_GLOBAL", Approval);
            List<ApprovalList> ApprovalList = await _commonService.ConvertDataTableToModelList<ApprovalList>(adt); 
            var approval = JsonConvert.SerializeObject(ApprovalList);
            var returnMassage = new {Data,accessToken, ModuleAccessData,UserDetail, approval };
            globalModel.Data = JsonConvert.SerializeObject(returnMassage);

            return Ok(JsonConvert.SerializeObject(globalModel)  );
        }
    }
}
