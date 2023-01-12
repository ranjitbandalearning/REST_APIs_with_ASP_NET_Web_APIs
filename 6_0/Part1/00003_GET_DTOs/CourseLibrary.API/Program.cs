using CourseLibrary.API.DataStore;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//xmlDataContractSerializer - formatter is used for CONTENT NEGOTIATION FEATURE
builder.Services.AddControllers(setupAction =>
{
    //THE BELOW IS THE DEFAULT BEHAVIOUR
    //setupAction.ReturnHttpNotAcceptable = false; 
    setupAction.ReturnHttpNotAcceptable = true;
    //setupAction.OutputFormatters.Add(
    //    new XmlDataContractSerializerOutputFormatter());
}).AddXmlDataContractSerializerFormatters();

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

//RANJIT - MapControllers is required to have the request ROUTE to the controller
app.MapControllers();

app.Run();
