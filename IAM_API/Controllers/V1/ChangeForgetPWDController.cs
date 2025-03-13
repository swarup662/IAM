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
using IAM_API.Utility;
using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;

namespace IAM_API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ChangeForgetPWDController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IDTOService _service;
        private readonly IEncryptDecrypt _encryptDecrypt;
        
        private readonly string _encryptionKey;
        private readonly string ApiKey;
        private readonly ICommonService _commonService;


        public ChangeForgetPWDController(IConfiguration configuration, IDTOService service, IEncryptDecrypt enc, ICommonService commonService)
        {
            _service = service;
            _encryptDecrypt = enc;
            _configuration = configuration;
            _commonService = commonService;

        }
     
   


        #region ChangePassword


        [HttpPost]
        [Route("UserUpdatePassword")]
        [SwaggerOperation(Tags = new[] { "Forget Password" })]

        public async Task<object> UserUpdatePassword(ChangePass pass)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                      {"@TenantId",pass.TenantId},
                    {"@USER_MASTER_KEY",pass.User_Master_Key},
                       {"@Email_Id",""},
                    {"@OLDPASSWORD",pass.Old_Password},
                    { "@RECTYPE","UPDATE_OLDPASS"},
                    {"@CONFIRMPASSWORD",pass.Confirm_Password},
                    { "@CREATEDBY",pass.CreatedBy}


                };
                int dt = await _service.AddAsync("SP_UPDATE_UserProfile_PASSWORD", parameters);
                string json = JsonConvert.SerializeObject(dt);
                var resultList = JsonConvert.DeserializeObject<object>(json);

                return resultList;

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("ChangePasswordAPI")]

        public async Task<string> UserPasswordReset(ChangePass pass)
        {
            try
            {
                string dPass = "";
                var parameters = new Dictionary<string, object>
                {
                    {"@TenantId",pass.TenantId},
                    {"@USER_MASTER_KEY",pass.User_Master_Key},
                        {"@Email_Id",""},
                    {"@OLDPASSWORD",pass.Old_Password},
                    { "@RECTYPE","VALIDATE_OLDPASS"},
                    {"@CONFIRMPASSWORD","0" },
                    { "@CREATEDBY",pass.CreatedBy}


                };
                DataSet ds = await _service.GetAllDatasetAsync("SP_UPDATE_UserProfile_PASSWORD", parameters);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];

                        dPass = row["UserPwd"].ToString();
                       
                    }

                }

                  

                return dPass;

            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion











        #region ForgotPassword

        [HttpPost]
        [Route("CheckMail")]

        public async Task<int> CheckMail(MailUserMaster mail)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                      {"@TenantId",0},
                    {"@USER_MASTER_KEY",0},
                       {"@Email_Id",mail.Email_ID},
                    {"@OLDPASSWORD",0},
                    { "@RECTYPE","CHECK_MAIL"},
                    {"@CONFIRMPASSWORD",""},
                    { "@CREATEDBY",""}


                };
                int dt = await _service.AddAsync("SP_UPDATE_UserProfile_PASSWORD", parameters);

                return dt;

            }
            catch (Exception)
            {
                throw;
            }
        }








        [HttpPost]
        [Route("FetchEmailDtls_ForgotPwd")]
        [SwaggerOperation(Tags = new[] { "Forgot password" })]
        public async Task<Mail> FetchEmailDtls_ForgotPwd(MailUserMaster model)
        {

            try
            {
                Mail Mail = new Mail();
                // Create the parameters for the stored procedure
                var parameters = new Dictionary<string, object>
              {
                    { "@TenantId", 0},
                     { "@Email_ID", model.Email_ID},
                    { "@REC_TYPE", "MailSetupDetails_ForgotPwd" },
                    { "@ApplicationId", 0},
                    { "@TenantMailSetupKey",5},
                    { "@USER_MASTER_KEY",0}
              };

                // Call the service method to get the DataSet
                var dataSet = await _service.GetAllDatasetAsync("Sp_FetchEmailDtls_ForgotPwd", parameters);

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

                        Mail.ReceiverEmail = lst;

                    }
                    var UserMasterData = dataSet.Tables[2];
                    if (UserMasterData.Rows.Count > 0)
                    {
                        var urow = UserMasterData.Rows[0];
                        var MailUserMasterModel = new MailUserMaster();

                        MailUserMasterModel.USER_MASTER_KEY = Convert.ToInt32(urow["USER_MASTER_KEY"]);
                        MailUserMasterModel.Email_ID = urow["Email_ID"].ToString();
                        MailUserMasterModel.FullName = urow["FullName"].ToString();
                        MailUserMasterModel.Username = urow["Username"].ToString();
                        MailUserMasterModel.UserPassword = urow["UserPassword"].ToString();
                        MailUserMasterModel.UserMailTypeCode = urow["UserMailTypeCode"].ToString();
                        MailUserMasterModel.Pin = urow["Pin"].ToString();


                        Mail.MailUserMaster = MailUserMasterModel;

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








        #endregion












    }
}
