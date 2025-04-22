using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Talabat.Api.Factories;
using System.Reflection;

namespace BitaryProject.Extensions
{
    public static class PresentationServicesExtensions
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            // Only use controllers from the Presentation assembly and exclude any that conflict with BitaryProject
            services.AddControllers()
                .ConfigureApplicationPartManager(manager => 
                {
                    // Get all controller types from Presentation assembly
                    var presentationAssembly = typeof(Presentation.AssemblyReference).Assembly;
                    var mainAssembly = Assembly.GetExecutingAssembly(); // BitaryProject assembly
                    
                    // Create a feature provider that excludes controllers that exist in BitaryProject
                    manager.FeatureProviders.Add(new ExcludeControllerFeatureProvider(mainAssembly));
                    
                    // Add Presentation assembly
                    manager.ApplicationParts.Add(new AssemblyPart(presentationAssembly));
                });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
            
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

    // Custom feature provider that excludes controllers that exist in the specified assembly
    public class ExcludeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly Assembly _assemblyToExcludeControllers;

        public ExcludeControllerFeatureProvider(Assembly assemblyToExcludeControllers)
        {
            _assemblyToExcludeControllers = assemblyToExcludeControllers;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            // Get all controller type names in the specified assembly
            var controllerTypesInMainAssembly = _assemblyToExcludeControllers.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Controller"))
                .Select(t => t.Name)
                .ToHashSet();

            // Remove controllers from the feature if they have the same name as a controller in the main assembly
            var controllerTypes = feature.Controllers.ToList();
            foreach (var controller in controllerTypes)
            {
                if (controllerTypesInMainAssembly.Contains(controller.Name))
                {
                    feature.Controllers.Remove(controller);
                }
            }
        }
    }
}
