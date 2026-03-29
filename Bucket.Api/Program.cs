using Bucket.Application.Handlers.Queries;
using Bucket.Application.Interfaces;
using Bucket.Infrastructure.Queries;
using DataModel = Bucket.Infrastructure.Data.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetPersonQuery>());
builder.Services.AddSingleton(_ => DataModel.Instance);
builder.Services.AddScoped<IPersonQuery, PersonQuery>();
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
