using CommonUtility.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using IAM_UI.Models;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Net.Http;
using System.Reflection;
using System.Net;
using System.Diagnostics;
using CommonUtility.SharedModels;
using IAM_UI.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.Mail;


namespace IAM_UI.Controllers
{
    public class UsersForBillingApiTrackerUIController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        private readonly ICommonService _commonService;
        private readonly string _ApiKey;
        private readonly string _MailBody;
        private readonly string BaseUrlApproval;
        private readonly IGlobalModelService _globalModelService;
        private readonly string _baseUrlGlobal;
        private readonly string _baseUrlUsersForBillingApiTrackerAPI;
        public UsersForBillingApiTrackerUIController(IConfiguration configuration, ICommonService commonService, IGlobalModelService globalModelService, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            //  _enc = new AesHmacEncryption(_configuration);
            _baseUrl = configuration["BaseUrlEmpInfo"];
            _commonService = commonService;
            _ApiKey = configuration["Apikey"];
            _MailBody = configuration["MailBody"];
            _globalModelService = globalModelService;
            _webHostEnvironment = webHostEnvironment;
            _baseUrlGlobal = configuration["BaseUrlGlobal"];
            _baseUrlUsersForBillingApiTrackerAPI = configuration["BaseUrlUsersForBillingApiTrackerAPI"];

        }




        public IActionResult Index()
        {
            return View();
        }









        //public async Task<IActionResult> SaasApiTriggerMailSend(Byte[] attachmentBytes , string attachmentFileName )
        //{

        //    try
        //    {
        //        GlobalModel gm = new GlobalModel() ;
        //        gm.TenantID = 2;

        //        string Urlparameter = "GetApiTriggerMail";

        //        ApiTriggerMailModel model = new ApiTriggerMailModel();
        //        model.TenantId = gm.TenantID;
        //        model.ApplicationId = 1;
        //        model.PurposeId = 1;
        //        List<ApiTriggerMailModel> atmm = new List<ApiTriggerMailModel>();


        //        string UrlEparameter = "FetchEmailDtls";

        //        string Body = null, encode = null;

        //        List<EmailDtls> edtls = new List<EmailDtls>();


        //        string modeldata = JsonConvert.SerializeObject(model);
        //        StringContent content = new StringContent(modeldata, Encoding.UTF8, "application/json");

        //        using (var httpclient = new HttpClient())
        //        {
        //            httpclient.DefaultRequestHeaders.Clear();
        //            httpclient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
        //            HttpResponseMessage mresponse = await httpclient.PostAsync(_baseUrlUsersForBillingApiTrackerAPI + Urlparameter, content);
        //            if (mresponse.IsSuccessStatusCode)
        //            {
        //                var data = await mresponse.Content.ReadAsStringAsync();
        //                 atmm = JsonConvert.DeserializeObject<List<ApiTriggerMailModel>>(data);
        //            }

        //            if (atmm[0].ApiTriggerMailKey >0)
        //            {
        //                EmailDtls edts = new EmailDtls();
        //                edts.EmailSetUpDtls_Key = 7; // for saas billing
        //                edts.TenantID = gm.TenantID;

        //                string jsondata = JsonConvert.SerializeObject(edts);
        //                gm.Data = jsondata;
        //                string jsonglobaldata = JsonConvert.SerializeObject(gm);
        //                StringContent Econtent = new StringContent(jsonglobaldata, Encoding.UTF8, "application/json");
        //                httpclient.DefaultRequestHeaders.Clear(); // Clears any existing headers
        //                httpclient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
        //                HttpResponseMessage response = await httpclient.PostAsync(_baseUrlGlobal + UrlEparameter, Econtent);
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var eresponse = await response.Content.ReadAsStringAsync();
        //                    edtls = JsonConvert.DeserializeObject<List<EmailDtls>>(eresponse);

        //                    Body = "";

        //                    Body = Body + edtls[0].MailBody + "<br>";




        //                    foreach (var item in atmm)
        //                    {
        //                        SendMailWithAttachment(item.SenderMail, item.SenderPassword, edtls[0].Mailsubject, item.ReceiverMail, null, null, Body, "yahoo", attachmentBytes, attachmentFileName);
        //                    }
        //                    return Json(eresponse);
        //                }
        //                else
        //                {
        //                    return BadRequest("Validation failed.");
        //                }

        //            }
        //            else
        //            {
        //                return BadRequest("mail details not found");
        //            }
                 
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        return View("Error", ex);
        //    }
        //}



        public void SendMailWithAttachment(string fromMailId, string fromPassword, string subject, string toMailId, string ccMailId, string bccMailId, string body, string mailService, byte[] attachmentBytes = null, string attachmentFileName = null)
        {
            try
            {
                // Enforce TLS 1.2
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // Determine SMTP Server
                string smtpServer = mailService.ToLower() switch
                {
                    "gmail" => "smtp.gmail.com",
                    "yahoo" => "smtp.mail.yahoo.com",
                    _ => throw new ArgumentException("Invalid mail service. Use 'gmail' or 'yahoo'.")
                };

                using (SmtpClient clientSMTP = new SmtpClient(smtpServer, 587))
                {
                    clientSMTP.EnableSsl = true;
                    clientSMTP.DeliveryMethod = SmtpDeliveryMethod.Network;
                    clientSMTP.UseDefaultCredentials = false;
                    clientSMTP.Credentials = new NetworkCredential(fromMailId, fromPassword);

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress(fromMailId);
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = true;
                        message.To.Add(toMailId);

                        if (!string.IsNullOrWhiteSpace(ccMailId))
                            message.CC.Add(ccMailId);

                        if (!string.IsNullOrWhiteSpace(bccMailId))
                            message.Bcc.Add(bccMailId);

                        // Attach PDF if provided (Keep MemoryStream outside 'using' block)
                        MemoryStream ms = null;
                        if (attachmentBytes != null && !string.IsNullOrEmpty(attachmentFileName))
                        {
                            ms = new MemoryStream(attachmentBytes);
                            message.Attachments.Add(new Attachment(ms, attachmentFileName, "application/pdf"));
                        }

                        // Send the email
                        clientSMTP.Send(message);
                        Console.WriteLine("✅ Email sent successfully.");

                        // Dispose MemoryStream after sending the email
                        ms?.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error sending email: {ex.Message}");
            }
        }





    }
}
