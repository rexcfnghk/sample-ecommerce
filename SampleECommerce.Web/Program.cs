using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SampleECommerce.Web.Aes;
using SampleECommerce.Web.Filters;
using SampleECommerce.Web.Jwt;
using SampleECommerce.Web.Repositories;
using SampleECommerce.Web.Serializers;
using SampleECommerce.Web.Services;
using SimpleInjector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var serviceCollection = builder.Services;

serviceCollection.AddEndpointsApiExplorer();
serviceCollection.AddSwaggerGen();
serviceCollection.AddControllers(
    options =>
    {
        options.Filters.Add<HandleDuplicateUserNameExceptionFilter>();
    });

var container = new Container();
serviceCollection.AddSimpleInjector(
    container,
    options =>
    {
        options.AddAspNetCore().AddControllerActivation();
    });

container.RegisterSingleton<IUserSignupService, UserSignupService>();
container.RegisterSingleton<ISaltService, Random128BitsSaltService>();
container.RegisterSingleton<IPasswordEncryptionService, AesPasswordEncryptionService>();
container.RegisterSingleton<IUserRepository>(() => new SqlUserRepository(GetConnectionString(builder)));
container.RegisterDecorator<IUserRepository, CatchDuplicateSqlUserRepository>(Lifestyle.Singleton);
container.RegisterSingleton<ISerializer, DotNetJsonSerializer>();
container.RegisterDecorator<ISerializer, CatchJsonExceptionSerializer>(Lifestyle.Singleton);
container.RegisterSingleton<IJwtGenerator, MicrosoftJwtGenerator>();
container.RegisterSingleton<IJwtExpiryCalculator, SevenDaysExpiryCalculator>();
container.RegisterSingleton<JsonWebTokenHandler>();
RegisterJwtIssuer();
RegisterSigningCredentials();
RegisterAesKey();

serviceCollection.AddAuthentication("Bearer")
    .AddJwtBearer(
        opt =>
        {
            opt.TokenValidationParameters.ValidateAudience = false;
            opt.TokenValidationParameters.ValidateIssuer = false;
            opt.TokenValidationParameters.IssuerSigningKey =
                container.GetInstance<SigningCredentials>().Key;
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.Services.UseSimpleInjector(container);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers().WithOpenApi();

app.UseAuthentication();

container.Verify();

app.Run();
return;

string GetConnectionString(WebApplicationBuilder webApplicationBuilder)
{
    var connectionString = webApplicationBuilder.Configuration.GetConnectionString("Default");
    if (connectionString == null)
    {
        throw new InvalidOperationException(
            "Cannot retrieve connection string");
    }

    return connectionString;
}

void RegisterAesKey()
{
    var aesKey = builder.Configuration["Aes:Key"];
    if (aesKey == null)
    {
        throw new InvalidOperationException("Cannot retrieve AES Key");
    }

    var bytes = aesKey.Split(',').Select(byte.Parse).ToArray();
    container.RegisterSingleton(() => new AesKey(bytes));
}

void RegisterJwtIssuer()
{
    var issuer = builder.Configuration["Jwt:Issuer"];
    if (issuer == null)
    {
        throw new InvalidOperationException("Cannot retrieve JWT issuer");
    }
    
    container.RegisterSingleton(() => new JwtIssuer(issuer));
}

void RegisterSigningCredentials()
{
    var securityKey = builder.Configuration["Jwt:SecurityKey"];
    if (securityKey == null)
    {
        throw new InvalidOperationException("Cannot retrieve JWT security key");
    }
    
    var algorithm = builder.Configuration["Jwt:Algorithm"];
    if (algorithm == null)
    {
        throw new InvalidOperationException("Cannot retrieve JWT algorithm");
    }

    container.RegisterSingleton(
        () => new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
            algorithm));
}

