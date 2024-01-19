using PresentationLayer.ActionFilters;
using PresentationLayer.Middleware;
using BusinessLayer.DependencyInjectionBusinessLayer;
using DataAccessLayer.Models.Configuration;
using DataAccessLayer.DependencyInjectionDataAccesssLayer;
using BusinessLayer.Helper.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(arg =>
{
    arg.Filters.Add<ValidationActionFilter>();
});

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings")!);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.DIBusinessLayer(builder.Configuration);
builder.Services.DIDataAccessLayer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
