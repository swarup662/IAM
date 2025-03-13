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


namespace IAM_UI.Controllers
{
    public class SaasBillingEmpController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        private readonly ICommonService _commonService;
        private readonly string _ApiKey;
        private readonly string _MailBody;
        private readonly string BaseUrlApproval;
        private readonly IGlobalModelService _globalModelService;
        public SaasBillingEmpController(IConfiguration configuration, ICommonService commonService, IGlobalModelService globalModelService, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            //  _enc = new AesHmacEncryption(_configuration);
            _baseUrl = configuration["BaseUrlEmpInfo"];
            _commonService = commonService;
            _ApiKey = configuration["Apikey"];
            _MailBody = configuration["MailBody"];
            _globalModelService = globalModelService;
            _webHostEnvironment = webHostEnvironment;
        }




        public IActionResult Index()
        {
            return View();
        }




       
        public async Task<IActionResult> PrintEmpInfo (SaasBillingEmpModel model)
        {
           string urlparameters = "PrintEmpInfo";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    string url = $"{_baseUrl}{urlparameters}";
                    string data = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", _ApiKey.Trim('{', '}'));
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);

                    
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        List<SaasBillingEmpModel> dataList = JsonConvert.DeserializeObject<List<SaasBillingEmpModel>>(responseContent);

                       if (dataList.Count > 0)
                        {
                            // Data found, generate PDF
                            FastReport.Utils.Config.WebMode = true;
                            FastReport.Report rep = new FastReport.Report();
                            string path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\RPT_Employee_information_Report.frx";
                            rep.Load(path);

                            rep.Report.RegisterData(dataList, "TBl_EmployeeInfo_ref");

                            using (MemoryStream ms = new MemoryStream())
                            {
                                if (rep.Prepare())
                                {
                                    FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport
                                    {
                                        ShowProgress = false,
                                        Subject = "Employee Information Report",
                                        Title = "Employee Information Report"
                                    };

                                    rep.Export(pdfExport, ms);
                                    ms.Position = 0;

                                    byte[] pdfBytes = ms.ToArray();
                                    string base64Pdf = Convert.ToBase64String(pdfBytes);

                                    // Return PDF as base64 string along with the message
                                    return Json(new
                                    {
                                        message = "success",
                                        pdf = base64Pdf
                                    });
                                }
                            }
                        }
                        else
                        {
                            // No data, return a "nodata" message
                            return Json(new { message = "nodata" });
                        }
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve employee data.");
                    }
                }
                catch (Exception ex)
                {
                    // Log exception or return a more detailed message if necessary
                    return Json(new { message = "error", error = ex.Message });
                }
            }

            // Add a default return statement to cover any paths that were missed
            return Json(new { message = "unknown_error" });
        }




    }
}
