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



            return services;
        }
    }
}
