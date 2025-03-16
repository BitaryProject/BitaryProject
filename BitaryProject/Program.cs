
using BitaryProject.Api.Extensions;
using BitaryProject.Extensions;

namespace BitaryProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //sec

            #region Services

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

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();

            #endregion
        }
    }
}
