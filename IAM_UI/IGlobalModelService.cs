using CommonUtility.SharedModels;
using IAM_UI.Helpers;
using IAM_UI;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using Microsoft.Extensions.Caching.Memory;

namespace IAM_UI
{
    public interface IGlobalModelService
    {
        GlobalModel InitializeGlobalModel(HttpContext context);
        Task logout(HttpContext context);
    }
}


public class GlobalModelService : IGlobalModelService
{
    private readonly IConfiguration _configuration;
    private readonly string _LoginUrl;
    private readonly IMemoryCache _memoryCache;
    public GlobalModelService(IConfiguration configuration, IMemoryCache memoryCache)
    {
        _configuration = configuration;
        _LoginUrl = configuration["Data:LoginUrl"];
        _memoryCache = memoryCache;


    }

    public GlobalModel InitializeGlobalModel(HttpContext context)
    {
        logout(context);
        var globalModel = new GlobalModel();

        var usereDetail = JsonConvert.DeserializeObject<UserDetail>(context.Session.GetString("UserData"));

        globalModel.ApplicationId = usereDetail.CurrentApplicationId;
        globalModel.TenantID = usereDetail.CurrentTenantId;
        globalModel.userID = usereDetail.USER_MASTER_KEY;
        globalModel.CompanyID = usereDetail.CurrentCompanyId;
        globalModel.USER_TYPE_KEY = usereDetail.CurrentUserTypeId;

        if (context.Items.ContainsKey("ModuleId") && context.Items["ModuleId"] != null)
        {
            // Set the ModuleId from context.Items and save it in the session
            globalModel.ModuleId = int.Parse(context.Items["ModuleId"].ToString());

            // Remove the old ModuleId from the session (destroy it)
            context.Session.Remove("ModuleId");

            context.Session.SetInt32("ModuleId", globalModel.ModuleId);
        }
        else
        {
            // Retrieve ModuleId from session, or use a default value (e.g., 0) if it's not set
            globalModel.ModuleId = context.Session.GetInt32("ModuleId") ?? 0; // default to 0 if null
        }



        return globalModel;
    }

    public async Task logout(HttpContext context)
    {

        var UserData = context.Session.GetString("UserData");
        if (string.IsNullOrEmpty(UserData))
        {
            
                // Clear the session or perform logout logic
                context.Session.Clear();

                // Redirect to login URL
                context.Response.Redirect(_LoginUrl, permanent: false);
                // Ensure to await the flush asynchronously and close the response
                await context.Response.Body.FlushAsync();
                context.Response.Body.Close();

                // Terminate execution
                return;
            
        }
        else
        {

            var lastAccessTime = context.Session.GetString("LastAccessTime");
            if (string.IsNullOrEmpty(lastAccessTime))
            {
                // Set initial last access time if it does not exist
                context.Session.SetString("LastAccessTime", DateTime.Now.ToString());
            }
            else
            {
                // Check if the session has been idle for longer than the timeout
                var timeout = TimeSpan.FromMinutes(40); // Adjust as needed
                var lastAccess = DateTime.Parse(lastAccessTime);
                var currentTime = DateTime.Now;
                var idleDuration = currentTime - lastAccess;

                if (idleDuration > timeout)
                {
                    //var sessionManager = SessionManager.GetInstance();
                    //var sessionId = context.Session.GetString("SessionId");
                    //var JobRole = context.Session.GetString("SelectedJobRole");
                    //var LoginData = context.Session.GetString("LoginData");

                    // Call your session removal function when the session times out
                    //sessionManager.RemoveSession(sessionId);
                    //sessionManager.RemoveSession(JobRole);
                    //sessionManager.RemoveSession(LoginData);

                    // Clear the session

                    context.Session.Clear();

                    // Redirect to login URL
                    context.Response.Redirect(_LoginUrl, permanent: false);
                    // Ensure to await the flush asynchronously and close the response
                    await context.Response.Body.FlushAsync();
                    context.Response.Body.Close();

                    // Terminate execution


                }
            }



        }


    }


}