using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TwitterCloneApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// connect to DB 
var connetionString = builder.Configuration.GetConnectionString("DefualtConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connetionString));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options =>
{
	// check Jwt token header 
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;  // work jwt not cookie 
}).AddJwtBearer(options =>  // verified key 
{
	options.SaveToken = true;
	options.RequireHttpsMetadata = false; // check if used HTTPs 
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidateIssuer = true,
		ValidIssuer = builder.Configuration["JWT:IssuerIp"],
		ValidateAudience = true,
		ValidAudience = builder.Configuration["JWT:AudienceIp"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecurityKey"]))
	};

});

builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "TwitterCloneAPI",
		Description = "Twitter Clone API",
		TermsOfService = new Uri("https://google.com"),  // Corrected URI
		Contact = new OpenApiContact
		{
			Name = "Abdelrahman",
			Email = "test@ayhaga.com"
		}
	});
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
	});
	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference=new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id="Bearer"

				},
				Name="Bearer",
				In=ParameterLocation.Header
			},
			new List<string>()
		}

	});

});





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthorization();

app.MapControllers();

app.Run();
