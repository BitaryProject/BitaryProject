namespace BitaryProject.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> SeedDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();//inject IDbInitializer

            await dbInitializer.InitializeAsync();

            return app;

        }
    }
}
