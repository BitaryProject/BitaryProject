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

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseCors("CORSPolicy");
            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();

            #endregion


            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path}");
                await next.Invoke();
            });
            app.Use((context, next) =>
            {
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
                    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                    context.Response.StatusCode = 200;
                    return Task.CompletedTask;
                }
                return next();
            });
        }
    }
}
