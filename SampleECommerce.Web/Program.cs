using SimpleInjector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var serviceCollection = builder.Services;

serviceCollection.AddEndpointsApiExplorer();
serviceCollection.AddSwaggerGen();
serviceCollection.AddControllers();

var container = new SimpleInjector.Container();
serviceCollection.AddSimpleInjector(
    container,
    options =>
    {
        options.AddAspNetCore().AddControllerActivation();
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

//app.UseAuthentication();

app.MapControllers().WithOpenApi();

container.Verify();

app.Run();
