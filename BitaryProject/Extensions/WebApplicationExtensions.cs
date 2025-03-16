using BitaryProject.Api.Middlewares;

namespace BitaryProject.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> SeedDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();//inject IDbInitializer

            await dbInitializer.InitializeAsync();
            await dbInitializer.InitializeIdentityAsync();

            return app;

        }
        public static WebApplication UseCustomExceptionMiddleware(this WebApplication app)
        {
            app.UseMiddleware<GlobalErrorHandlingMiddleware>();

            return app;
        }
    }
}
