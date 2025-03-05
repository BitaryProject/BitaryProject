using Microsoft.AspNetCore.Mvc;

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

       
            return services;
        }
    }
}
