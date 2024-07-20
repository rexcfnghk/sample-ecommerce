using SampleECommerce.Web.Repositories;
using SampleECommerce.Web.Services;
using SimpleInjector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var serviceCollection = builder.Services;

serviceCollection.AddEndpointsApiExplorer();
serviceCollection.AddSwaggerGen();
serviceCollection.AddControllers();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.Services.UseSimpleInjector(container);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthentication();

app.MapControllers().WithOpenApi();

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
