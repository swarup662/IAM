using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using IAM_UI.Models;
using System.Diagnostics;
using System.Reflection;
using IAM_UI.Helpers;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace IAM_UI.Helpers
{
    public class ModuleMiddleware
    {
        private readonly RequestDelegate _next;

        public ModuleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (!string.IsNullOrEmpty(path))
            {
                var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length >= 2)
                {
                    var controller = segments[0];
                    var action = segments[1];

                    if (context.Session != null && context.Session.IsAvailable)
                    {
                        var loginData = context.Session.GetString("LoginData");

                        if (!string.IsNullOrEmpty(loginData))
                        {
                            try
                            {
                                var userAuth = JsonConvert.DeserializeObject<UserAuth>(loginData);

                                if (userAuth != null)
                                {
                                    var layoutProcessor = new LayoutProcessor(userAuth);
                                    var result = layoutProcessor.ProcessLoginData(loginData);

                                    if (result?.MenuItems != null)
                                    {
                                        string approvalTag = context.Request.Query["Approve"];

                                        var routes = result.MenuItems
                                            .Where(item => item.ModuleTypeId == 800 &&
                                                        item.Controller.Equals(controller, StringComparison.OrdinalIgnoreCase) &&
                                                        item.Action.Equals(action, StringComparison.OrdinalIgnoreCase))
                                            .Select(item => new
                                            {
                                                Name = item.Action,
                                                Controller = item.Controller,
                                                Action = item.Action,
                                                ModuleId = item.ModuleId,
                                                Actions = new List<string>
                                                {
                                                    item.Add == 1 ? "Add" : null,
                                                    item.Edit == 1 ? "Edit" : null,
                                                    item.Delete == 1 ? "Delete" : null,
                                                    item.View == 1 ? "View" : null,
                                                    item.Print == 1 ? "Print" : null
                                                }.Where(act => act != null).ToArray()
                                            }).ToList();



                                        // This line checks if the URL contains the 'approve' query string. 
                                        // If so, the user is identified as an approver, meaning they can accept or reject any row. 
                                        // In this case, only the approver can view the data.
                                        // Store the actions array in Items
                                        if (!string.IsNullOrEmpty(approvalTag))
                                        {
                                            if (controller == "AdvanceUI" && action == "Index")
                                            {
                                                context.Items["Actions"] = new List<string[]> { new[] { "Approve", "View", "Edit" } };
                                                context.Items["ModuleId"] = approvalTag;
                                            }
                                            else if(controller == "EncodeDecode")
                                            {
                                                context.Items["Actions"] = new List<string[]> { new[] { "Approve", "View" } };
                                                context.Items["ModuleId"] = approvalTag;
                                            }
                                            else
                                            {
                                                context.Items["Actions"] = new List<string[]> { new[] { "Approve", "View" } };
                                                context.Items["ModuleId"] = approvalTag;
                                            }
                                            //context.Items["Actions"] = new List<string[]> { new[] { "Approve", "View" } };
                                            //context.Items["ModuleId"] = routes.FirstOrDefault()?.ModuleId.ToString();
                                            //approvalTag;
                                        }
                                        else if (controller == "EncodeDecode")
                                        {
                                            context.Items["Actions"] = new List<string[]> { new[] { "Approve", "View" } };
                                            context.Items["ModuleId"] = routes.FirstOrDefault()?.ModuleId.ToString();
                                        }
                                        else
                                        {
                                            context.Items["Actions"] = routes.Select(route => route.Actions).ToList();
                                            context.Items["ModuleId"] = routes.FirstOrDefault()?.ModuleId.ToString();
                                        }









                                        //The following code checks whether the `ModuleId` is part of an approval process.If an approval setup exists for the `ModuleId`, 
                                        //it retrieves a list of rows along with their statuses from the API.
                                        var actionsList = context.Items["Actions"] as List<string[]>;

                                        var UserData = context.Session.GetString("UserData");
                                        if (!string.IsNullOrEmpty(UserData))
                                        {
                                            var udata = JsonConvert.DeserializeObject<UserDetail>(UserData);
                                            var currentCompanyId = udata.CurrentCompanyId.ToString();
                                            if (actionsList != null && actionsList.Any())
                                            {
                                                var moduleId = context.Items["ModuleId"] as string;
                                                if (!string.IsNullOrEmpty(moduleId))
                                                {
                                                    int actionModuleId = Convert.ToInt32(moduleId);

                                                    if (result.Approval != null && result.Approval.Any())
                                                    {
                                                        var approval = result.Approval.FirstOrDefault(item => item.ModuleId == actionModuleId && item.CompanyKey == currentCompanyId);

                                                        if (approval != null)
                                                        {
                                                            context.Items["ApprovalModule"] = JsonConvert.SerializeObject(approval);

                                                            using (var httpClient = new HttpClient())
                                                            {
                                                                string urlParameters = "GET_SENDTOAPPROVAL";
                                                                var userdata = context.Session.GetString("UserData");

                                                                if (!string.IsNullOrEmpty(userdata))
                                                                {
                                                                    var emp = JsonConvert.DeserializeObject<UserDetail>(userdata);

                                                                    var model = new SendToApproval
                                                                    {
                                                                        ModuleId = actionModuleId,
                                                                        ApplicationId = emp.AppID,
                                                                        TenantId = emp.TenantId,
                                                                        CompanyId = emp.CurrentCompanyId,
                                                                        UserTypeId = emp.CurrentUserTypeId

                                                                    };

                                                                    var jsonContent = JsonConvert.SerializeObject(model);
                                                                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                                                                    // Load configuration directly within Invoke
                                                                    var configurationManager = new ConfigurationManager();
                                                                    configurationManager.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                                                                    var baseUrl = configurationManager.GetValue<string>("AllModuleGridApproval");

                                                                    HttpResponseMessage response = await httpClient.PostAsync(baseUrl + urlParameters, content);

                                                                    if (response.IsSuccessStatusCode)
                                                                    {
                                                                        var data = await response.Content.ReadAsStringAsync();
                                                                        context.Items["RowWiseApprovalDataTable"] = data;
                                                                    }

                                                                }

                                                            }
                                                        }
                                                    }
                                                }
                                            }


                                        }

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error processing login data: {ex.Message}");
                            }
                        }
                    }
                }
            }

            await _next(context);
        }
    }

    public static class ModuleMiddlewareExtensions
    {
        public static IApplicationBuilder UseModuleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ModuleMiddleware>();
        }
    }
}
