using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace IAM_UI.Helpers
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public GlobalExceptionFilter(
            ILogger<GlobalExceptionFilter> logger,
            IHttpContextAccessor httpContextAccessor,
            ITempDataDictionaryFactory tempDataFactory)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _tempDataFactory = tempDataFactory;
        }

        public void OnException(ExceptionContext context)
            {

            if (context.ExceptionHandled)
            {
                return;
            }

            var exception = context.Exception;
            var request = context.HttpContext.Request;

            var clientIp = GetClientIpAddress(request);
            var lineNumber = ExtractLineNumber(exception.StackTrace);

            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var controllerName = actionDescriptor?.ControllerName ?? "UnknownController";
            var actionName = actionDescriptor?.ActionName ?? "UnknownAction";

            AddEmptyLineToLog();
            LogErrorToFile(clientIp, request.Method, request.Path, exception.Message, lineNumber, controllerName, actionName);

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var tempData = _tempDataFactory.GetTempData(httpContext);
                tempData["ExceptionMessage"] = context.Exception.Message; // ✅ Store error message
            }
            bool isJsonRequest = context.HttpContext.Request.Headers["Accept"].ToString().Contains("application/json");

            if (isJsonRequest || context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // ✅ If it's a JavaScript call (Fetch, AJAX, etc.), return JSON
                context.Result = new JsonResult(new { error = true, message = context.Exception.Message });
            }
            else
            {
                // ✅ Otherwise, redirect to error page
                context.Result = new RedirectToActionResult("Error", "Home", new { statusCode = 500, message = exception.Message });
            }



  
            context.ExceptionHandled = true; // Mark exception as handled
        }


        // Helper method to extract line number from the stack trace
        private string ExtractLineNumber(string stackTrace)
        {
            if (string.IsNullOrEmpty(stackTrace))
                return "Unknown"; 

            var line = stackTrace.Split(Environment.NewLine)
                .FirstOrDefault(l => l.Contains("at") && !l.Contains("Microsoft"));

            if (line != null)
            {
                var fileInfo = line.Split(':');
                if (fileInfo.Length >= 2)
                {
                    return fileInfo.Last(); // Returns line number if found (e.g., "104")
                }
            }

            return "Unknown";
        }

        // Helper method to get the client IP address from request headers
        private string GetClientIpAddress(HttpRequest request)
        {
           
      

            // Check RemoteIpAddress
            return request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
        }


        // Helper method to add an empty line before appending the new log entry
        private void AddEmptyLineToLog()
        {
            var logFileName = $"log_{DateTime.UtcNow:dd_MM_yyyy}.txt"; // Log file name with date
            var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Logs", logFileName);

            if (!File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, string.Empty); // Create the file if it doesn't exist
            }

            // Append an empty line
            File.AppendAllText(logFilePath, Environment.NewLine);
        }

        // Helper method to log error details to the file
        private void LogErrorToFile(string clientIp, string method, string path, string exceptionMessage, string lineNumber, string controllerName, string actionName)
        {
            var logFileName = $"log_{DateTime.UtcNow:dd_MM_yyyy}.txt"; // Log file name with date
            var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Logs", logFileName);

            // Create the "Logs" directory if it doesn't exist
            var logDirectory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Convert UTC to Local Time
            var localTime = DateTime.UtcNow.ToLocalTime(); // Converts UTC to local time
            var logMessage = $"{localTime:yyyy-MM-dd HH:mm:ss} [ERR] Unhandled exception occurred. IP: {clientIp}, Method: {method}, Path: {path}, Controller: {controllerName}, Action: {actionName}, Exception: {exceptionMessage}, Line: {lineNumber}";
            File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
        }

    }
}
