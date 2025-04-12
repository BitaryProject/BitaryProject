using BitaryProject.Api.Extensions;
using BitaryProject.Extensions;
using BitaryProject.Mail;
using Services;
using Services.Abstractions;

namespace BitaryProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //sec

            #region Services
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddTransient<IMailingService , MailingService>();  
            
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddPresentationServices();
            builder.Services.AddCoreServices(builder.Configuration);

            #endregion

            var app = builder.Build();

            #region Pipelines


            await app.SeedDbAsync();
            //app.UseExceptionHandler();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"error\": \"An unexpected error occurred.\"}");
                });
            });
            //     app.UseCors("CORSPolicy");

            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();


            #endregion


            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path}");
                await next.Invoke();
            });
            try
            {
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Application failed to start.");
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
