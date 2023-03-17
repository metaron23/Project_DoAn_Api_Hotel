using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project_DoAn_Api_Hotel.Controllers;
using Project_DoAn_Api_Hotel.Data;
using Project_DoAn_Api_Hotel.Repository.AuthenRepository;
using Project_DoAn_Api_Hotel.Repository.EmailRepository;
using Project_DoAn_Api_Hotel.Repository.TokenRepository;
using Project_DoAn_Api_Hotel.Startup;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MyDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));

builder.Services.IdentityService();
builder = builder.AuthenJWTService();
builder.Services.RepositoryService();

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors(option => option.WithOrigins("*").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
