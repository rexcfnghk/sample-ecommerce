using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SampleECommerce.Web.Aes;
using SampleECommerce.Web.AuthenticationHandlers;
using SampleECommerce.Web.BasicAuthDecoders;
using SampleECommerce.Web.Constants;
using SampleECommerce.Web.Filters;
using SampleECommerce.Web.Jwt;
using SampleECommerce.Web.Repositories;
using SampleECommerce.Web.Services;
using SampleECommerce.Web.Swashbuckle;
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
serviceCollection.AddSwaggerGen(
    c =>
    {
        c.AddSecurityDefinition(name: AuthenticationSchemes.Bearer, securityScheme: new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter the Bearer Authorization string as following: `Bearer {token}`",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = AuthenticationSchemes.Bearer
        });
        
        c.AddSecurityDefinition(name: AuthenticationSchemes.Basic, securityScheme: new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Basic authentication",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = AuthenticationSchemes.Basic
        });

        c.OperationFilter<OpenApiParameterIgnoreFilter>();
        c.OperationFilter<SecurityRequirementsOperationFilter>();
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });

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

serviceCollection
    .AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationSchemes.Basic,
        _ => { })
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

CrossWireAspNetCoreDependencies();

container.RegisterSingleton<IUserSignupService, UserSignupService>();
container.RegisterSingleton<ISaltService, Random128BitsSaltService>();
container.RegisterSingleton<IPasswordEncryptionService, AesPasswordEncryptionService>();
container.RegisterSingleton(() => GetConnectionString(builder));
container.RegisterSingleton<IUserRepository, SqlUserRepository>();
container.RegisterDecorator<IUserRepository, CatchDuplicateSqlUserRepository>(Lifestyle.Singleton);
container.RegisterSingleton<IJwtGenerator, MicrosoftJwtGenerator>();
container.RegisterSingleton<IJwtExpiryCalculator, SevenDaysExpiryCalculator>();
container.RegisterSingleton<IOrderService, OrderService>();
container.RegisterSingleton<IOrderRepository, SqlOrderRepository>();
container.RegisterDecorator<IOrderRepository, CatchContraintViolationOrderRepository>(Lifestyle.Singleton);
container.RegisterSingleton<IOrderIdGenerator, GuidOrderIdGenerator>();
container.RegisterSingleton<IOrderTimeGenerator, CurrentDateTimeOffsetOrderTimeGenerator>();
container.RegisterSingleton<IProductService, ProductService>();
container.RegisterSingleton<IProductRepository, SqlProductRepository>();
container.RegisterSingleton<IBasicAuthDecoder, Base64BasicAuthDecoder>();
container.RegisterDecorator<IBasicAuthDecoder, CatchFormatExceptionBasicAuthDecoder>(Lifestyle.Singleton);
container.RegisterSingleton<BasicAuthenticationHandler>();

RegisterJwtIssuer();
RegisterSigningCredentials();
RegisterAesKey();

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

ConnectionString GetConnectionString(WebApplicationBuilder webApplicationBuilder)
{
    var connectionString = webApplicationBuilder.Configuration.GetConnectionString(webApplicationBuilder.Environment.EnvironmentName);
    if (connectionString == null)
    {
        throw new InvalidOperationException(
            "Cannot retrieve connection string");
    }
    return (ConnectionString)connectionString;
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

void CrossWireAspNetCoreDependencies()
{
    serviceCollection.AddSingleton(
        _ => container.GetInstance<BasicAuthenticationHandler>());
    serviceCollection.AddSingleton<IPasswordEncryptionService>(
        _ => container.GetInstance<IPasswordEncryptionService>());
    serviceCollection.AddSingleton<IUserRepository>(
        _ => container.GetInstance<IUserRepository>());
    serviceCollection.AddSingleton<IBasicAuthDecoder>(
        _ => container.GetInstance<IBasicAuthDecoder>());
}
