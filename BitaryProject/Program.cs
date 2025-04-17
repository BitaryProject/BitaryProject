using BitaryProject.Api.Extensions;
using BitaryProject.Extensions;
using BitaryProject.Mail;
using Services;
using Core.Services.Abstractions;

namespace BitaryProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Set startup timeout to alert if initialization is taking too long
            var startupCts = new CancellationTokenSource(TimeSpan.FromSeconds(90));
            var startupTask = Task.Run(async () => {
                try
                {
                    await StartupAsync(args);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Startup failed: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }
                    throw;
                }
            }, startupCts.Token);

            try
            {
                await startupTask;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Application startup timed out after 90 seconds");
                throw new ApplicationException("Application failed to start within the allowed time");
            }
        }

        private static async Task StartupAsync(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure logging early to capture all errors
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            
            //sec

            #region Services
            // Register essential services first
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddPresentationServices();
            builder.Services.AddCoreServices(builder.Configuration);
            
            // Register non-essential services after
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddTransient<IMailingService, MailingService>();
            #endregion

            var app = builder.Build();

            #region Pipelines
            // Skip DB seeding in production - moved to WebApplicationExtensions
            try
            {
                await app.SeedDbAsync();
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Database initialization failed but startup will continue");
            }
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();

            // Remove the simplified exception handler and replace with custom middleware
            app.UseCustomExceptionMiddleware();
            
            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader());

            // Essential middleware only
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            #endregion

            await app.RunAsync();
        }
    }
}
