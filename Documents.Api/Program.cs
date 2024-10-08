using Documents.Service.Extensions;
using Documents.Client.Extensions;
using Serilog;

namespace Documents.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Host.UseSerilog();

        // Configure Services
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<CustomExceptionFilter>();
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors();
        builder.Services.AddServices();
        builder.Services.AddS3Client(builder.Configuration);
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        }

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}
