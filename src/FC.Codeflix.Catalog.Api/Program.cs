using FC.Codeflix.Catalog.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppConections(builder.Configuration)
    .AddUseCases()
    .AddRabbitMQ(builder.Configuration)
    .AddMessageProducer()
    .AddMessageConsumer()
    .AddStorage(builder.Configuration)
    .AddSecurity(builder.Configuration)
    .AddAndConfigureControllers()
    .AddCors(p => p.AddPolicy("CORS", builder =>
    {
        builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    }));

var app = builder.Build();
app.UseHttpLogging();
app.MigrateDatabase();
app.UseDocumentation();
//app.UseHttpsRedirection();
app.UseCors("CORS");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program { }
