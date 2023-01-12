using CourseLibrary.API.DataStore;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            }).AddXmlDataContractSerializerFormatters();

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
