using Gateway.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ===== Reverse Proxy =====
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ===== AuthN/AuthZ =====
builder.Services.AddGatewayAuthentication(builder.Configuration);
builder.Services.AddGatewayAuthorization();

// ===== Swagger UI (dropdown downstream) =====
builder.Services.AddGatewaySwagger(builder.Configuration);

var app = builder.Build();

// Production headers / forward headers (nếu chạy sau Nginx/Ingress)
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger"; // /swagger

    // Load swagger endpoints dynamically từ config
    app.ConfigureSwaggerEndpoints(options);

    // (Optional) OAuth2 with PKCE (Keycloak) - nếu bật
    app.ConfigureSwaggerOAuth(options);
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map reverse proxy
app.MapReverseProxy();

// Health for gateway itself
app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
   .AllowAnonymous();

app.Run();