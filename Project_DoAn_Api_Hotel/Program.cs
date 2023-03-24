using Microsoft.EntityFrameworkCore;
using Project_DoAn_Api_Hotel.Data;
using Project_DoAn_Api_Hotel.Repository.NotifiHub;
using Project_DoAn_Api_Hotel.Startup;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
builder.Services.AddCors();
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.SwaggerService();

builder.Services.AddDbContext<MyDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));

builder.Services.IdentityService();
builder.AuthenJWTService();
builder.Services.RepositoryService();
builder.Services.AuthorService();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSignalR().AddAzureSignalR();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseFileServer();
app.UseCors(builder =>
{
    builder.WithOrigins("http://127.0.0.1:5500", "http://localhost:4200")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials();
});
app.MapHub<ChatHub>("/hub");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.Run();
