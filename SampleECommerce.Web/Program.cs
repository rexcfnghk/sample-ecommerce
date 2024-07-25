using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SampleECommerce.Web.Aes;
using SampleECommerce.Web.Filters;
using SampleECommerce.Web.Jwt;
using SampleECommerce.Web.Repositories;
using SampleECommerce.Web.Serializers;
using SampleECommerce.Web.Services;
using SampleECommerce.Web.ValueProviders;
using SimpleInjector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var serviceCollection = builder.Services;

// Docker-specific setup
builder.Configuration.AddKeyPerFile(
    directoryPath: "/run/secrets",
    optional: true);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/protection-keys"))
    .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
    {
        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
    });

serviceCollection.AddEndpointsApiExplorer();
serviceCollection.AddSwaggerGen();

serviceCollection.AddControllers(
    options =>
    {
        options.ValueProviderFactories.Insert(0, new JwtSubjectValueProviderFactory());
        options.Filters.Add<HandleOrderExceptionFilter>();
        options.Filters.Add<HandleDuplicateUserNameExceptionFilter>();
    });

serviceCollection.AddSingleton<JsonWebTokenHandler>();

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
container.RegisterSingleton<IOrderService, OrderService>();
container.RegisterSingleton<IOrderRepository>(() => new SqlOrderRepository(GetConnectionString(builder)));
container.RegisterDecorator<IOrderRepository, CatchContraintViolationOrderRepository>(Lifestyle.Singleton);
container.RegisterSingleton<IProductService, ProductService>();
container.RegisterSingleton<IProductRepository>(() => new SqlProductRepository(GetConnectionString(builder)));

RegisterJwtIssuer();
RegisterSigningCredentials();
RegisterAesKey();

serviceCollection.AddAuthentication("Bearer")
    .AddJwtBearer(
        opt =>
        {
            // Prevent .NET from replacing 'sub' claim into some other formats
            JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
            opt.MapInboundClaims = false;
            
            opt.TokenValidationParameters.ValidAudience =
                container.GetInstance<JwtIssuer>().Issuer;
            opt.TokenValidationParameters.ValidIssuer =
                container.GetInstance<JwtIssuer>().Issuer;
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
    var connectionString = webApplicationBuilder.Configuration.GetConnectionString(webApplicationBuilder.Environment.EnvironmentName);
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
