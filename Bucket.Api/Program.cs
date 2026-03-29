using Bucket.Api.Configuration;
using Bucket.Application.Handlers.Queries.Persons;
using Bucket.Application.Queries;
using Bucket.Application.Repositories;
using Bucket.Application.Services;
using Bucket.Infrastructure.Queries;
using Bucket.Infrastructure.Reports;
using Bucket.Infrastructure.Repositories;
using DataModel = Bucket.Infrastructure.Data.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<FrontendOptions>(builder.Configuration.GetSection(FrontendOptions.SectionName));
builder.Services.AddProblemDetails();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetPersonsQuery>());
builder.Services.AddSingleton(_ => DataModel.Instance);
builder.Services.AddScoped<IPersonQuery, PersonQuery>();
builder.Services.AddScoped<IProductQuery, ProductQuery>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPurchaseQuery, PurchaseQuery>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<IRaportGenerator, RaportGenerator>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStatusCodePages();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
