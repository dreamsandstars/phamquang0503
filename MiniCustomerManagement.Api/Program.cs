using MiniCustomerManagement.Api.Models;
using MiniCustomerManagement.Api.Services;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Application: {builder.Environment.ApplicationName}");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<CustomerService>();
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapGet("/swagger", () => Results.Content(
        """
        <!doctype html>
        <html>
        <head>
          <meta charset="utf-8" />
          <title>Swagger UI</title>
          <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5/swagger-ui.css" />
        </head>
        <body>
          <div id="swagger-ui"></div>
          <script src="https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
          <script>
            window.ui = SwaggerUIBundle({
              url: '/openapi/v1.json',
              dom_id: '#swagger-ui'
            });
          </script>
        </body>
        </html>
        """,
        "text/html"));
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new { message = "Mini Customer Management API is running" }));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/env", () => Results.Ok(new
{
    Environment = app.Environment.EnvironmentName
}));

app.MapGet("/config", (IConfiguration config) => Results.Ok(new
{
    AppName = config["AppSettings:AppName"],
    BaseUrl = config["AppSettings:BaseUrl"]
}));

app.MapControllers();

app.Run();
