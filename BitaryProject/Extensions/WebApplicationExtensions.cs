using BitaryProject.Api.Middlewares;

namespace BitaryProject.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> SeedDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                
                // Skip initialization in production to reduce startup time
                if (app.Environment.IsProduction())
                {
                    logger.LogInformation("Skipping database seeding in production environment for faster startup");
                    return app;
                }
                
                // Create a cancellation token that will cancel after 60 seconds
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                
                // Execute with the cancellation token
                await Task.Run(async () => {
                    try
                    {
                        await dbInitializer.InitializeAsync();
                        await dbInitializer.InitializeIdentityAsync();
                    }
                    catch (OperationCanceledException)
                    {
                        logger.LogWarning("Database initialization was cancelled due to timeout");
                    }
                }, cts.Token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during database initialization.");
                logger.LogWarning("Continuing application startup despite database initialization error");
                
                if (ex.InnerException != null)
                {
                    logger.LogError(ex.InnerException, "Inner exception during database initialization.");
                }
            }

            return app;
        }
        public static WebApplication UseCustomExceptionMiddleware(this WebApplication app)
        {
            app.UseMiddleware<GlobalErrorHandlingMiddleware>();

            return app;
        }
    }
}
