using CommonUtility.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using IAM_API.Models;
using IAM_API.Controllers.V1;

namespace GlobalAppAPI.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TenantMailSetupController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly IDTOService _service;
        private readonly IEncryptDecrypt _encryptDecrypt;
        private readonly ILogger<AuthorizationController> _logger;
        private readonly string _encryptionKey;
        private readonly string ApiKey;
        private readonly ICommonService _commonService;


        public TenantMailSetupController(IConfiguration configuration, IDTOService service, ILogger<AuthorizationController> logger, IEncryptDecrypt enc, ICommonService commonService)
        {
            _service = service;
            _logger = logger;
            _encryptDecrypt = enc;
            _configuration = configuration;
            _commonService = commonService;

        }



        //Function for fetch the dropdown data 
        [HttpPost]
        [Route("GetApplicationList")]
        [SwaggerOperation(Tags = new[] { "Mail Setupy" })]
        public async Task<ActionResult> GetApplicationList( MailSetupModel model)
        {
            try
            {

                DataSet newDataSet = new DataSet();
                var parameters = new Dictionary<string, object>
                  {
                      { "@TenantId", model.Tenant_ID},
                      { "@REC_TYPE", "GET_APPLICATION_LIST" },
                      { "@ApplicationId", 0},
                      { "@TenantMailSetupKey",0}
                  };

                DataTable dt = await _service.GetDataAsync("Sp_Get_Mail_Setup", parameters);

                DataTable clonedTable = dt.Copy();
                clonedTable.TableName = "Applicationtb";
                newDataSet.Tables.Add(clonedTable);
                dt = null;

                parameters = new Dictionary<string, object>
                  {
                      { "@TenantId", model.Tenant_ID},
                     { "@REC_TYPE", "GET_PURPOSE_BY_ID" },
                    { "@ApplicationId", model.Application_ID},
                    { "@TenantMailSetupKey",0}
                  };
                dt = await _service.GetDataAsync("Sp_Get_Mail_Setup", parameters);

                clonedTable = dt.Copy();
                clonedTable.TableName = "Purposetb";
                newDataSet.Tables.Add(clonedTable);
                string dstring = JsonConvert.SerializeObject(newDataSet);
                return Ok(dstring);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        //for fetch the grid data
        [HttpPost]
        [Route("GetData")]
        [SwaggerOperation(Tags = new[] { "Mail Setupy" })]
        public async Task<List<object>> GetData( MailSetupModel model)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                     { "@TenantId", model.Tenant_ID},
                    { "@REC_TYPE", "GET_TenantMailSetup" },
                    { "@ApplicationId", 0},
                     { "@TenantMailSetupKey",0}
                };
                DataSet dataSet = await _service.GetAllDatasetAsync("Sp_Get_Mail_Setup", parameters);
                List<object> lst = new List<object>();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var info = new MailSetupModel();
                    info.TenantMailSetupKey = Convert.ToInt32(row["TenantMailSetupKey"]);
                    info.PurposeApplicationId = Convert.ToInt32(row["ApplicationId"]);
                    info.ApplicationName = row["ApplicationName"].ToString();
                    info.PurposeName = row["PurposeName"].ToString();
                    info.PurposeDescription = row["PurposeDescription"].ToString();
                    info.SenderMail = row["SenderMail"].ToString();
                    info.SenderPassword = row["SenderPassword"].ToString();
                    info.MailSubject = row["Mailsubject"].ToString();
                    info.MailBody = row["MailBody"].ToString();
                    info.IsCC = Convert.ToInt32(row["IsCC"]);
                    info.IsAttachment = Convert.ToInt32(row["IsAttachment"]);
                    info.PageLink = row["PageLink"].ToString();
                    info.Parameter1 = row["parameter1"].ToString();
                    info.Parameter2 = row["parameter2"].ToString();
                    info.Parameter3 = row["parameter3"].ToString();
                    info.TimeSlotMinute = Convert.ToInt32(row["timeslotMinute"]);

                    lst.Add(info);
                }
                return lst;

             
            }
            catch (Exception ex)
            {
                throw;
            }
        }





        //for Get Purpose List
        [HttpPost]
        [Route("GetPurposeById")]
        [SwaggerOperation(Tags = new[] { "Mail Setupy" })]
        public async Task<ActionResult> GetPurposeById([FromBody] MailSetupModel model)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                      { "@TenantId", model.Tenant_ID},
                    { "@REC_TYPE", "GET_PURPOSE_BY_ID" },
                    { "@ApplicationId", model.PurposeApplicationId},
                    { "@TenantMailSetupKey",0}
                };
                DataTable dt = await _service.GetDataAsync("Sp_Get_Mail_Setup", parameters);
                return Ok(JsonConvert.SerializeObject(dt));
            }
            catch (Exception ex)
            {
                throw;
            }
        }






        // For Edit function

        [HttpPost]
        [Route("EditMailSetup")]
        [SwaggerOperation(Tags = new[] { "Mail Setupy" })]
        public async Task<ActionResult> EditMailSetup( MailSetupModel model)
        {
            try
            {
                // Create the parameters for the stored procedure
                var parameters = new Dictionary<string, object>
              {
                      { "@TenantId", model.Tenant_ID},
                    { "@REC_TYPE", "GET_TenantMailSetup_BY_ID" },
                    { "@ApplicationId", 0},
                    { "@TenantMailSetupKey",model.TenantMailSetupKey}
              };

                // Call the service method to get the DataSet
                var dataSet = await _service.GetAllDatasetAsync("Sp_Get_Mail_Setup", parameters);

                // Check if the DataSet contains any result sets
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    // Retrieve the ServiceDesk data (first table)
                    var SenderData = dataSet.Tables[0];

                    // Retrieve the associated users data (second table)
                    var RecieverData = dataSet.Tables[1];

                    var MailSetupModel = new MailSetupModel();
                    if (SenderData.Rows.Count > 0)
                    {
                        var row = SenderData.Rows[0];
                        MailSetupModel.TenantMailSetupKey = Convert.ToInt32(row["TenantMailSetupKey"]);
                        MailSetupModel.Tenant_ID = Convert.ToInt32(row["TenantId"]);
                        MailSetupModel.Application_ID = Convert.ToInt32(row["ApplicationId"]);
                        MailSetupModel.TenantMailSetupPurposeKey = Convert.ToInt32(row["PurposeId"]);
                        MailSetupModel.TenantMailSetupKey = Convert.ToInt32(row["TenantMailSetupKey"]);
                        MailSetupModel.SenderMail = row["SenderMail"].ToString();
                        MailSetupModel.SenderPassword = row["SenderPassword"].ToString();
                        MailSetupModel.MailSubject = row["Mailsubject"].ToString();
                        MailSetupModel.MailBody = row["MailBody"].ToString();
                        MailSetupModel.IsCC = Convert.ToInt32(row["IsCC"]);
                        MailSetupModel.IsAttachment = Convert.ToInt32(row["IsAttachment"]);
                        MailSetupModel.PageLink = row["PageLink"].ToString();
                        MailSetupModel.Parameter1 = row["parameter1"].ToString();
                        MailSetupModel.Parameter2 = row["parameter2"].ToString();
                        MailSetupModel.Parameter3 = row["parameter3"].ToString();
                        MailSetupModel.TimeSlotMinute = Convert.ToInt32(row["timeslotMinute"]);
                      
                    }

                    // Generate HTML <tr> format from serviceDeskUsersData
                    string userRowsHtml = GenerateHtmlRows(RecieverData);

                    // Set the EditAllUser property with the generated HTML
                    MailSetupModel.EditAllUser = userRowsHtml;

                    // Return the ServiceDesk model as the response
                    return Ok(MailSetupModel);
                }
                else
                {
                    return NotFound(new { Message = "Mail Setup not found" });
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }

        // Generate html Rows method

        private string GenerateHtmlRows(DataTable dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return string.Empty;
            }

            var htmlBuilder = new StringBuilder();

            // Loop through each row in the DataTable
            foreach (DataRow row in dataTable.Rows)
            {
                htmlBuilder.Append("<tr>");

                // Loop through each column in the row and conditionally skip columns 4, 5, 6, and 7 (0-based index)
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    // Add "d-none" class to hide 5th, 6th, 7th, and 8th columns (0-based index)
                    if (i == 4 || i == 5 || i == 6 || i == 2)
                    {
                        htmlBuilder.Append($"<td class=\"d-none\">{row[i]}</td>");
                    }
                    else
                    {
                        htmlBuilder.Append($"<td>{row[i]}</td>");
                    }
                }

                // Append the delete button column
                htmlBuilder.Append("<td>");
                htmlBuilder.Append("<a href=\"javascript:;\" class=\"action-icon delete-row\">");
                htmlBuilder.Append("<i class=\"mdi mdi-delete\" data-toggle=\"tooltip\" title=\"Delete\" style=\"color:red\"></i>");
                htmlBuilder.Append("</a>");
                htmlBuilder.Append("</td>");

                htmlBuilder.Append("</tr>");

            }

            return htmlBuilder.ToString();
        }


        // Delete Row Functionality






        [HttpPost]
        [Route("SaveMailSetup")]
        [SwaggerOperation(Tags = new[] { "Mail Setupy" })]
        public async Task<ActionResult> SaveMailSetup( MailSetupModel model)
        {
            try
            {
                DataTable Receiver_Mail_DataTb = null;
                // Check if Maildata is null or empty before conversion
                if (model.Maildata == null || !model.Maildata.Any())
                {
                    // Create an empty DataTable with at least one column matching TVP definition
                    Receiver_Mail_DataTb = new DataTable();
                    Receiver_Mail_DataTb.Columns.Add("ReceiverMail", typeof(string)); // Add relevant column(s)
                }
                else
                {
                    Receiver_Mail_DataTb = ToDataTable(model.Maildata);
                }



                // Prepare parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                        {
                            {"@RecType", (model.TenantMailSetupKey == null || model.TenantMailSetupKey == 0) ? "INSERT" : "UPDATE"},
                            {"@TenantMailSetupKey", model.TenantMailSetupKey},
                            {"@TenantId", model.Tenant_ID},
                            {"@ApplicationId", model.PurposeApplicationId},
                            {"@SenderMail", model.SenderMail},
                            {"@SenderPassword", model.SenderPassword},
                            {"@PurposeId", model.TenantMailSetupPurposeKey},
                            {"@Created_By", model.Created_By},
                            {"@MailSubject", model.MailSubject},
                            {"@MailBody", model.MailBody},
                            {"@IsCC", model.IsCC},
                            {"@IsAttachment", model.IsAttachment}, 
                            {"@PageLink", model.PageLink},
                            {"@Parameter1", model.Parameter1},
                            {"@Parameter2", model.Parameter2},
                            {"@Parameter3", model.Parameter3},
                            {"@TimeSlotMinute", model.TimeSlotMinute}
                        };


                var tvpParameters = new Dictionary<string, (string Receiver_Mail_Tb_Type, DataTable Maildata)>
                    {
                        {"@Receiver_Mail_Data", ("dbo.Receiver_Mail_Tb_Type", Receiver_Mail_DataTb)},
                    };
                var result = await _service.Int_ProcessWithMultipleTVPsAsync("SP_CRUD_TENANT_MAIL_SETUP", parameters, tvpParameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log exception details
                var errorResponse = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return StatusCode(500, errorResponse);
            }
        }



        public DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable();

            if (items == null || !items.Any())
                return dataTable;

            var properties = typeof(T).GetProperties();

            // Add columns
            foreach (var property in properties)
            {
                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                dataTable.Columns.Add(property.Name, type);
            }

            // Preload rows (optional, to optimize adding rows)
            dataTable.BeginLoadData();

            // Add rows dynamically
            foreach (var item in items)
            {
                var row = dataTable.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item, null) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            dataTable.EndLoadData();

            return dataTable;
        }







        [HttpPost]
        [Route("DeleteMailSetup")]
        [SwaggerOperation(Tags = new[] { "Mail Setupy" })]
        public async Task<ActionResult> DeleteMailSetup( MailSetupModel model)
        {
            try
            {
                // Prepare parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                    {
                        {"@RecType", "DELETE"},
                        {"@TenantMailSetupKey", model.TenantMailSetupKey},
                        {"@TenantId", model.Tenant_ID},
                        {"@ApplicationId", model.PurposeApplicationId},
                        {"@SenderMail", model.SenderMail},
                        {"@SenderPassword", model.SenderPassword},
                        {"@PurposeId", model.TenantMailSetupPurposeKey},
                        {"@Created_By", model.Created_By},
                        {"@MailSubject", model.MailSubject},
                            {"@MailBody", model.MailBody},
                            {"@IsCC", model.IsCC},
                            {"@IsAttachment", model.IsAttachment},
                            {"@PageLink", model.PageLink},
                            {"@Parameter1", model.Parameter1},
                            {"@Parameter2", model.Parameter2},
                            {"@Parameter3", model.Parameter3},
                            {"@TimeSlotMinute", model.TimeSlotMinute}
                    };

                // Execute the stored procedure
                int result = await _service.UpdateAsync("SP_CRUD_TENANT_MAIL_SETUP", parameters);
                return Ok(result);
            }

            catch (Exception ex)
            {
                throw;
            }
        }






    }
}
