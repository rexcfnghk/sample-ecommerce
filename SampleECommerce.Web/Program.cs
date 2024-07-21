using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Formatters;
using SampleECommerce.Web.Aes;
using SampleECommerce.Web.AuthenticationHandlers;
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
AddAuthenticationToServices(serviceCollection);

var container = new Container();
serviceCollection.AddSimpleInjector(
    container,
    options =>
    {
        options.AddAspNetCore().AddControllerActivation();
    });
serviceCollection.AddTransient(
    _ => container.GetInstance<UserNamePasswordAuthenticationHandler>());

container.Register<UserNamePasswordAuthenticationHandler>();
container.RegisterSingleton<IUserSignupService, UserSignupService>();
container.RegisterSingleton<ISaltService, Random128BitsSaltService>();
container.RegisterSingleton<IPasswordEncryptionService, AesPasswordEncryptionService>();
container.RegisterSingleton<IUserRepository>(() => new SqlUserRepository(GetConnectionString(builder)));
container.RegisterDecorator<IUserRepository, CatchDuplicateSqlUserRepository>(Lifestyle.Singleton);
container.RegisterSingleton<ISerializer, DotNetJsonSerializer>();
container.RegisterDecorator<ISerializer, CatchJsonExceptionSerializer>(Lifestyle.Singleton);
container.RegisterSingleton<IJwtGenerator, MicrosoftJwtGenerator>();
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

void AddAuthenticationToServices(IServiceCollection serviceCollection1)
{
    serviceCollection1.AddAuthentication()
        .AddScheme<AuthenticationSchemeOptions,
            UserNamePasswordAuthenticationHandler>(
            "UserNamePassword",
            _ => { });

    var authenticationHandlerDescriptor = serviceCollection1.First(
        s => s.ImplementationType == typeof(UserNamePasswordAuthenticationHandler));

    serviceCollection1.Remove(authenticationHandlerDescriptor);
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
