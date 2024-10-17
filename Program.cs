using CompRoomAuthApi.Contexts;
using CompRoomAuthApi.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
	options.Password.RequireDigit = false; // Nem kötelezõ számjegy
	options.Password.RequireLowercase = false; // Nem kötelezõ kisbetû
	options.Password.RequireUppercase = false; // Nem kötelezõ nagybetû
	options.Password.RequireNonAlphanumeric = false; // Nem kötelezõ különleges karakter
	options.Password.RequiredLength = 8; // Minimális jelszóhossz (pl.: 6 karakter)
	options.Password.RequiredUniqueChars = 0; // Minimális egyedi karakterek száma
})
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddSignInManager()
	.AddRoles<IdentityRole>()
	.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateIssuerSigningKey = true,
		ValidateLifetime = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
	};
});

builder.Services.AddSwaggerGen(option => {

	option.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Enter JWT Token",
		Name = "Authorization",
		Scheme = "Bearer",
		BearerFormat = "JWT",
		Type = SecuritySchemeType.Http
	});

	option.AddSecurityRequirement(new OpenApiSecurityRequirement()
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "BearerAuth"
				}
			},
			Array.Empty<string>()
		}
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
