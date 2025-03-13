using CommonUtility.DataAccess;
using CommonUtility.Interface;
using CommonUtility.Repository;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using IAM_API.Interface; 
using IAM_API.Repository;
using Swashbuckle.AspNetCore.Annotations;
using IAM_API.Models;
using IAM_API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddSingleton<AdoDataAccess>(provider =>
    new AdoDataAccess(builder.Configuration.GetConnectionString("DBConn")));
// Register IDTOService with DTOServiceRepository
builder.Services.AddScoped<IDTOService, DTOServiceRepository>();

// Register ICommonService with CommonServiceRepository
builder.Services.AddScoped<ICommonService, CommonServiceRepository>();

// Register IEncryptDecrypt with EncryptDecryptRepository
builder.Services.AddScoped<IEncryptDecrypt, EncryptDecryptRepository>();

// Bind TokenSettings from configuration
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));

// Register ITokenService with TokenServiceRepo
builder.Services.AddScoped<ITokenService, TokenServiceRepo>();
//builder.Services.AddScoped<I_Password, PasswordService>();


// Configure API versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LoginAPI", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "LoginAPI", Version = "v2" });
    c.EnableAnnotations();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LoginAPIv1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "LoginAPIv2");
    });
}
// Use the custom API key middleware
app.UseApiKeyMiddleware();

//app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();
