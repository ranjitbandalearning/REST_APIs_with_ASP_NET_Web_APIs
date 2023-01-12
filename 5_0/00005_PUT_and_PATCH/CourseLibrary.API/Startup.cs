using AutoMapper;
using CourseLibrary.API.DataStore;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //RANJIT - This helps us to access configuration settings
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // RANJIT -  This is helpful to add services to built in dependency injection container. 
        public void ConfigureServices(IServiceCollection services)
        {
            //RANJIT - We are adding services of only controllers here as this only registers only those services
            //          which are required for building APIs like controllers, model binders, data anotations and Formatters etc.
            //services.AddControllers();
            //xmlDataContractSerializer - formatter is used for CONTENT NEGOTIATION FEATURE
            services.AddControllers(setupAction =>
            {
                //THE BELOW IS THE DEFAULT BEHAVIOUR
                //setupAction.ReturnHttpNotAcceptable = false; 
                setupAction.ReturnHttpNotAcceptable = true;
                //setupAction.OutputFormatters.Add(
                //    new XmlDataContractSerializerOutputFormatter());
            })
                //we should send our requests with application/json-patch+json as media type.
                //Let's send this, and we hit an error. 
                //That looks strange, right? Our input is as it should be. 
                //Well, the issue here is that the default JSON parsed in ASP.NET Core 3.0 is not as feature complete as something like JSON.NET.
                //It probably will get closer with future releases, but at the moment it's not there yet. So, what we can do is use JSON.NET, as most of you are probably used to.
                //let's open the Startup class. Let's scroll down a bit. What we want to do is chain AddNewtonsoftJson to the IMvcBuilder
                //The only thing I prefer to change is the contract resolver, so properties are always nicely CamelCased. To do that, simply set it to a new instance of CamelCasePropertyNamesContractResolver. That's defined in Newtonsoft.Json .Serialization.
                .AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver =
                   new CamelCasePropertyNamesContractResolver();
            })
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "https://courselibrary.com/modelvalidationproblem",
                        Title = "One or more model validation errors occurred.",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "See the errors property for details.",
                        Instance = context.HttpContext.Request.Path
                    };

                    problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

                    return new UnprocessableEntityObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });

            //LETS MAKE SURE THAT THE AUTOMAPPER SERVICES ARE REGISTERED IN THE CONTAINER
            //We are loading all profiles from all the assemblies
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //RANJIT - Dependency Injection relations are registered
            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();
            services.AddScoped<IAuthorData, AuthorData>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // RANJIT - this is called after the above method(ConfigureServices) call
        //          this is used to specify how an ASP.NET application will respond to individual HTTP request.
        //          through this we can configure the request PIPELINE 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //THE BELOW CODE IS TO RETURN SOME DEFAULT FAULT TEXT WHEN THERE IS SOME INTERNAL SERVER REQUEST ERROR 
            // I.E. FOR STATUS CODES 500
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });

            }


            //RANJIT - UseRouting, UseEndpoints, MapControllers are related to have the request ROUTE to the controller
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
