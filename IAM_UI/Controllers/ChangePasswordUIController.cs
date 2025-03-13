using IAM_UI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using CommonUtility.Interface;
using CommonUtility.SharedModels;
using System.Web;
using IAM_UI;
using static IAM_UI.Controllers.EncodeDecodeController;
using System.Drawing.Imaging;
using IAM_UI.Controllers;

namespace TenantCompany.Controllers
{
    public class ChangePasswordUIController : Controller
    {
        private readonly IGlobalModelService _globalModelService;
        private readonly IConfiguration _configuration;

        private readonly string BaseUrlChangePassword;

        private readonly ICommonService _commonService;
        private readonly string _ApiKey;
        private readonly string _baseUrlGlobal;
        private readonly EncodeDecodeController encdec;
        private readonly string _MailBody;
        string urlParameters = "";
        public ChangePasswordUIController(IConfiguration configuration, ICommonService commonService, IGlobalModelService globalModelService, EncodeDecodeController EncDnc)
        {
            _configuration = configuration;


            BaseUrlChangePassword = configuration["BaseUrlChangePass"];
            _commonService = commonService;
            _ApiKey = configuration["Apikey"];
            _baseUrlGlobal = configuration["BaseUrlGlobal"];
            _MailBody = configuration["MailBody"];

            _globalModelService = globalModelService;
            encdec = EncDnc;

        }
        #region change Password
        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ChangePassword([FromBody]ChangePass dataToSend)
        {
            try
            {
             
                var usermasterkey = HttpContext.Session.GetString("UserID");
                var usermasterkeyPart = usermasterkey.Split('-')[0];
                string userID = usermasterkeyPart;

                //   //this is for password decryption
            

                if (dataToSend.Confirm_Password != null && dataToSend.Confirm_Password != "")
                {
                    EncodeDecodeModel encodeModel = new EncodeDecodeModel()
                    {
                        Txt = dataToSend.Confirm_Password.ToString(),
                        Type = 1
                    };

                    var encodePass = await encdec.EncryptDecrypt(encodeModel);
                    dataToSend.Confirm_Password = (encodePass as ObjectResult)?.Value?.ToString(); ;
                }
            

                ChangePass pass = new ChangePass();
                pass.Old_Password = dataToSend.Old_Password;
                pass.Confirm_Password = dataToSend.Confirm_Password;
                pass.User_Master_Key = Convert.ToInt32(userID);
                pass.CreatedBy = Convert.ToInt32(userID);

                if (pass.Confirm_Password=="")
                {
                    


                    string urlParameters = "ChangePasswordAPI";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    var content = new StringContent(JsonConvert.SerializeObject(pass), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(BaseUrlChangePassword + urlParameters, content);


                    if (response.IsSuccessStatusCode)
                    {


                        string responseContent = await response.Content.ReadAsStringAsync();
                        string dPass = responseContent .ToString();
                        int r = 0;
                        if (dPass != null && dPass != "")
                        {
                            EncodeDecodeModel decodeModel = new EncodeDecodeModel()
                            {
                                Txt = dPass.ToString(),
                                Type = 2
                            };

                            var docodePass = await encdec.EncryptDecrypt(decodeModel);
                            string DecodePass = (docodePass as ObjectResult)?.Value?.ToString(); 
                            if (DecodePass == dataToSend.Old_Password)
                            {
                                r = 1;
                            }

                        }



                        string json = JsonConvert.SerializeObject(r.ToString());
                        var resultList = JsonConvert.DeserializeObject<object>(json);

                        return Json(resultList);

                    }
                    else
                    {

                        var errorData = await response.Content.ReadAsStringAsync();
                        var result = new { id = errorData };
                        return Json(result);

                    }
                }
                else 
                {
                    if (pass.Old_Password != null && pass.Old_Password != "")
                    {
                        EncodeDecodeModel decodeModel = new EncodeDecodeModel()
                        {
                            Txt = pass.Old_Password.ToString(),
                            Type = 1
                        };

                        var encodePass = await encdec.EncryptDecrypt(decodeModel);
                        pass.Old_Password = (encodePass as ObjectResult)?.Value?.ToString(); ;
                    }
                    string urlparameters = "UserUpdatePassword";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    var content = new StringContent(JsonConvert.SerializeObject(pass), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(BaseUrlChangePassword + urlparameters, content);
                    if (response.IsSuccessStatusCode)
                    {

                        string responseContent = await response.Content.ReadAsStringAsync();
                        return Json(responseContent);

                    }
                    else
                    {

                        var errorData = await response.Content.ReadAsStringAsync();
                        var result = new { id = errorData };
                        return Json(result);

                    }
                }
                return Json("");
                
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public static DateTime ParseCustomDateTime(string dateTimeString)
        {
            // Split the date and time parts using the '+' separator
            string[] parts = dateTimeString.Split('+');

            if (parts.Length != 2)
                throw new FormatException("Invalid date and time format.");

            string dateString = parts[0];
            string timeString = parts[1];

            // Parse the date part
            DateTime date = DateTime.ParseExact(dateString, "dd-MM-yyyy", null);

            // Parse the time part
            TimeSpan time = TimeSpan.Parse(timeString);

            // Combine the date and time parts into a DateTime
            return date.Add(time);
        }
        [HttpPost]
        public async Task<JsonResult> AppliedToForgetPWD(Mail model)
        {

            try
            {
                string msg = "";
                int info;
                string urlparameters = "AppliedToForgetPWD";

                using (var httpclient = new HttpClient())

                {
                    httpclient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    StringContent Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpclient.PostAsync(BaseUrlChangePassword + urlparameters, Content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responsedata = await response.Content.ReadAsStringAsync();
                        info = Convert.ToInt32(responsedata);
                        if (info > 0)
                        {
                            msg = "Success";
                        }
                        else
                        {
                            msg = "Fail";
                        }
                        var result = new { msg, info };

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

                throw;
            }


        }
        #endregion


        #region Forget Password

        public IActionResult ForgetPassword()
        {

            return View();

        }


       


        [HttpPost]
        public async Task<IActionResult> CheckMail(MailUserMaster dataToSend)
        {
            try
            {




                  var r = 0;

                    string urlParameters = "CheckMail";
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    var content = new StringContent(JsonConvert.SerializeObject(dataToSend), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(BaseUrlChangePassword + urlParameters, content);


                    if (response.IsSuccessStatusCode)
                    {


                        string responseContent = await response.Content.ReadAsStringAsync();
                  

                                 r = Convert.ToInt32(responseContent);
                                    if (r > 0)
                                    {
                                       var cs = new credentialsSend();
                                            cs.ApplicationId = 0;
                                            cs.TenantMailSetupKey = 5;
                                   cs.Email_ID = dataToSend.Email_ID;
                                       int a =Convert.ToInt32( await CredentialsSend(cs));

                                            if (a == 1)
                                            {
                                                TempData["MSG"] = "successmail";
                                            }
                                            else
                                            {
                                          TempData["MSG"] = "failmail";
                                            }


                                     }

                    else
                                    {
                                        TempData["MSG"] = "failmail";
                                    }

                                    return Redirect("/ChangePasswordUI/ForgetPassword");



                    

                    }
                TempData["MSG"] = "Mail";

                return Redirect("/ChangePasswordUI/ForgetPassword");


            }
            catch (Exception ex)
            {
                throw ex;
            }


        }





        [HttpPost]
        public async Task<int> CredentialsSend([FromBody] credentialsSend Cs)
        {

            try
            {
         

                string Urlparameter = "FetchEmailDtls_ForgotPwd";

                string Body = null, encode = null;

                var model = new MailUserMaster();
                var MailUserMasterModel = new MailUserMaster();
               
                model.Email_ID = Cs.Email_ID;
               

                using (var httpClient = new HttpClient())
                {


                    string jsonData = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(BaseUrlChangePassword + Urlparameter, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var apresponse = await response.Content.ReadAsStringAsync();
                        var mailDtls = JsonConvert.DeserializeObject<Mail>(apresponse);



                        //Body = "Hi " + mailDtls.GroupHeadName + "<br>";

                        //Body = Body + edtls[0].MailBody + "<a href='" + "" + edtls[0].PageLink + "" + "'> Click here </a> ";
                        //Body = Body + "<br>" + edtls[0].parameter1 + ": " + mailDtls.UserName + " " + edtls[0].parameter2 + ": " + mailDtls.Password;

                        //   //this is for password decryption
                        if (mailDtls.MailUserMaster.UserPassword != null && mailDtls.MailUserMaster.UserPassword != "")
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
                               .Replace("{{MailBody}}", mailDtls.MailBody ?? string.Empty)
                               .Replace("{{PageLink}}", mailDtls.PageLink ?? string.Empty)
                               .Replace("{{Parameter1}}", mailDtls.Parameter1 ?? string.Empty)
                               .Replace("{{UserName}}", mailDtls.MailUserMaster.Username ?? string.Empty)
                               .Replace("{{Parameter2}}", mailDtls.Parameter2 ?? string.Empty)
                               .Replace("{{Password}}", mailDtls.MailUserMaster.UserPassword ?? string.Empty)
                               .Replace("{{Pin}}", mailDtls.MailUserMaster.Pin?.ToString() == "0" ? "N/A" : mailDtls.MailUserMaster.Pin?.ToString() ?? "N/A");

                        _commonService.SendMail(mailDtls.SenderMail, mailDtls.SenderPassword, mailDtls.MailSubject, mailDtls.MailUserMaster.Email_ID, null, null, Body, mailDtls.MailUserMaster.UserMailTypeCode);
                        return 1;

                    }
                    else
                    {
                        return 0;
                    }
                }

                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        #endregion
    }
}
