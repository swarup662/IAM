using CommonUtility.Interface;
using CommonUtility.Repository;
using IAM_UI;
using IAM_UI.Controllers;
using IAM_UI.Helpers;
using IAM_UI.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

// Register UserAuth and LayoutProcessor
builder.Services.AddTransient<UserAuth>();
builder.Services.AddTransient<LayoutProcessor>();
builder.Services.AddTransient<HomeController>();
builder.Services.AddTransient<UsersForBillingApiTrackerUIController>();


builder.Services.AddHttpClient();
// Add services to the container
builder.Services.AddControllersWithViews();

// Register services
builder.Services.AddScoped<IEncryptDecrypt, EncryptDecryptRepository>();
builder.Services.AddScoped<ICommonService, CommonServiceRepository>();
builder.Services.AddScoped<IGlobalModelService, GlobalModelService>();
builder.Services.AddScoped<UsersForBillingApiTrackerUI>();
builder.Services.AddScoped<APIResultsValue>();
builder.Services.AddScoped<EncodeDecodeController>();

// Register custom logger service (FileLoggerRepository)
// Register custom logger service (FileLoggerRepository)
string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
builder.Services.AddScoped<ILoggerService>(provider => new FileLoggerRepository(logDirectory));

// Bind the TrustedHosts section to TrustedHostsOptions
builder.Services.Configure<TrustedHostsOptions>(builder.Configuration.GetSection("TrustedHosts"));

// Add in-memory caching services
builder.Services.AddDistributedMemoryCache(); // This is necessary for session management


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(40); // Set the session timeout here
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin() // Allows requests from any origin
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
var app = builder.Build();
    app.UseCors("AllowAll");

    // Configure the HTTP request pipeline
    if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Auth/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Session middleware must be added before UseAuthorization
app.UseSession();
app.Use(async (context, next) =>

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
            var loginUrl = builder.Configuration.GetValue<string>("Data:LoginUrl");

            // Redirect to the login page
            context.Response.Redirect(loginUrl);
            return;


        }
    }

    // Update the last access time for the session
    context.Session.SetString("LastAccessTime", DateTime.Now.ToString());

    await next();
});



app.UseModuleMiddleware();




app.UseAuthorization();

app.MapControllerRoute(
    name: "DashBoard",
    pattern: "DashBoard/{id?}",
    defaults: new { controller = "Home", action = "DashBoard" });

app.MapControllerRoute(
    name: "ReceiveData",
    pattern: "ReceiveData",
    defaults: new { controller = "Home", action = "ReceiveData" });




app.MapControllerRoute(
    name: "company",
    pattern: "company",
    defaults: new { controller = "Login", action = "SetCompany" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
