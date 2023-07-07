using Demo.Context;
using Demo.Entity;
using Demo.Repository;
using Demo.Validation;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.WebHost.UseSentry(options => options.CaptureFailedRequests = true);

Log.Logger = new LoggerConfiguration()

    .ReadFrom.Configuration(builder.Configuration)

    .CreateLogger();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext")!);
});

// Use Serilog service

builder.Host.UseSerilog((content, configurations) => configurations.ReadFrom.Configuration(content.Configuration));

builder.Services.AddHealthChecks().AddSqlServer(builder.Configuration.GetConnectionString("AppDbContext")!);

builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IRepository<Post>, PostRepository>();
builder.Services.AddScoped<IRepository<Comment>, CommentRepository>();


builder.Services.AddTransient<IValidator<User>, UserValidator>();
builder.Services.AddTransient<IValidator<Post>, PostValidator>();
builder.Services.AddTransient<IValidator<Comment>, CommentValidator>();

//builder.Services.AddApiVersioning(options =>
//{
//    options.AssumeDefaultVersionWhenUnspecified = true;
//    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
//});

builder.Services.AddMemoryCache();


builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

app.UseSentryTracing();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapHealthChecks("/api/_health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions

{

    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse

});

app.UseAuthorization();

app.MapControllers();

app.Run();
