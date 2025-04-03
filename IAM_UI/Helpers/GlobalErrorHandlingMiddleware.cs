
namespace IAM_UI.Helpers
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                // If the status code indicates an error (e.g., 404, 403, etc.)
                if (context.Response.StatusCode >= 400)
                {
                    string errorMessage = GetErrorMessage(context.Response.StatusCode);
                    context.Response.Redirect($"/Home/Error?statusCode={context.Response.StatusCode}&message={Uri.EscapeDataString(errorMessage)}");
                }
            }
            catch (Exception ex)
            {
              
                // Redirect to Error page with status code 500
                context.Response.Redirect($"/Home/Error?statusCode=500&message={Uri.EscapeDataString(ex.Message)}");
            }
        }

        private string GetErrorMessage(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request. Please check your input and try again.",
                401 => "Unauthorized. You do not have permission to access this page.",
                403 => "Forbidden. Access is denied.",
                404 => "Page not found. The requested resource could not be found.",
                500 => "Internal Server Error. An unexpected error occurred.",
                _ => "An unknown error occurred. Please contact support."
            };
        }
    }
}