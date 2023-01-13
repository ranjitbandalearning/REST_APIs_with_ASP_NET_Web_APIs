using CourseLibrary.API.DataStore;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
//we should send our requests with application/json-patch+json as media type.
//Let's send this, and we hit an error. 
//That looks strange, right? Our input is as it should be. 
//Well, the issue here is that the default JSON parsed in ASP.NET Core 3.0 is not as feature complete as something like JSON.NET.
//It probably will get closer with future releases, but at the moment it's not there yet. So, what we can do is use JSON.NET, as most of you are probably used to.
//let's open the Startup class. Let's scroll down a bit. What we want to do is chain AddNewtonsoftJson to the IMvcBuilder
//The only thing I prefer to change is the contract resolver, so properties are always nicely CamelCased. To do that, simply set it to a new instance of CamelCasePropertyNamesContractResolver. That's defined in Newtonsoft.Json .Serialization.
//.AddNewtonsoftJson(setupAction =>
//{
//    setupAction.SerializerSettings.ContractResolver =
//        new CamelCasePropertyNamesContractResolver();
//})
;
//xmlDataContractSerializer - formatter is used for CONTENT NEGOTIATION FEATURE
builder.Services.AddControllers(setupAction =>
{
    //THE BELOW IS THE DEFAULT BEHAVIOUR
    //setupAction.ReturnHttpNotAcceptable = false; 
    setupAction.ReturnHttpNotAcceptable = true;
    //setupAction.OutputFormatters.Add(
    //    new XmlDataContractSerializerOutputFormatter());
})
.AddXmlDataContractSerializerFormatters()
.AddNewtonsoftJson(setupAction =>
        setupAction.SerializerSettings.ContractResolver =
            new CamelCasePropertyNamesContractResolver()
      )
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
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//RANJIT - Dependency Injection relations are registered
builder.Services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();
builder.Services.AddSingleton<IAuthorData, AuthorData>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//RANJIT - UseRouting, UseEndpoints, MapControllers are related to have the request ROUTE to the controller
app.UseRouting();



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
