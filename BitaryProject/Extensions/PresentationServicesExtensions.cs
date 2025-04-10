using Microsoft.AspNetCore.Mvc;
using Talabat.Api.Factories;

namespace BitaryProject.Extensions
{
    public static class PresentationServicesExtensions
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            services.AddControllers()
                .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);// 3shan ye2ra mn l presentaion msh mn Controller 

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ApiResponseFactory.CustomValidationErrorResponse;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CORSPolicy", builder =>
                {

                    builder.AllowAnyHeader()
                    // AllowAnyMethod =>> ay method Get ,Post ,Delete ay 7aga 
                    .AllowAnyMethod()
                    .WithOrigins("https://localhost:3000")
                    .WithOrigins("http://localhost:3000");

                    // WithOrigins =>> el Origins ely 3ayzhom yeklmoni bas 
                    // lw 3ayz URL mo3yn yeklmni ha7oto ka string fe WithOrigins()
                    //.AllowAnyOrigin()=> Ay Url yeklemni msh shart a7ded wa7ed mo3yn 

                });
            });

            return services;
        }
    }
}
