using Application.Core;
using DotNetEnv;
using MassTransit;
using Microsoft.IdentityModel.Tokens;
using orders_microservice.Domain.Repositories;
using orders_microservice.Infrastructure.Repositories;
using System.Text;
using orders_microservice.Utils.Core.Src.Infrastructure.SagaStateMachineService;
using orders_microservice.Utils.Core.Src.Infrastructure.SagaStateMachineService.States;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using orders_microservice.Utils.Core.Src.Application.LocationService;
using orders_microservice.Utils.Core.Src.Infrastructure.GoogleMapService;
using System.Text.Json.Nodes;
using orders_microservice.Utils.Core.Src.Application.SagaStateMachineService;
using orders_microservice.Utils.Core.Src.Infrastructure.SagaStateMachineService.Repositories;
using orders_microservice.Utils.Core.Src.Infrastructure.SagaStateMachineService.Models;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<MongoEventStore>();
builder.Services.AddScoped<IEventStore, MongoEventStore>();
builder.Services.AddScoped<IdService<string>, GuidGenerator>();
builder.Services.AddScoped<IOrderRepository, MongoOrderRepository>();
builder.Services.AddScoped<IMessageBrokerService, RabbitMQService>();
builder.Services.AddScoped<ILocationService<JsonNode>, GoogleMapService>();
builder.Services.AddScoped<ISagaStateMachineService<string>, SagaStateMachineRepository>();
builder.Services.AddControllers(options => {
    options.Filters.Add<GlobalExceptionFilter>();
});

//var certSection = builder.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate");
//certSection["Path"] = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel_CertificatesDefault_Path")!;
//certSection["Password"] = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel_CertificatesDefault_Password")!;

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = Environment.GetEnvironmentVariable("API_GATEWAY_URL")!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!))
        };
    });



builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddSagaStateMachine<OrderStateMachine, OrderStatusStates>()
        .MongoDbRepository<OrderStatusStates>(r =>
        {
            r.Connection = Environment.GetEnvironmentVariable("CONNECTION_URI"); ;
            r.DatabaseName = Environment.GetEnvironmentVariable("DATABASE_NAME")!;
            r.CollectionName = "status-events";
        });

    
    BsonClassMap.RegisterClassMap<OrderStatusStates>(cm =>
    {
        cm.AutoMap();
        cm.MapIdProperty(x => x.CorrelationId)
            .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
    });

    BsonClassMap.RegisterClassMap<MongoStates>(cm =>
    {
        cm.AutoMap();
        cm.MapIdProperty(x => x.CorrelationId)
            .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
    });

    busConfigurator.SetKebabCaseEndpointNameFormatter();
    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(Environment.GetEnvironmentVariable("RABBITMQ_URI")!), h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USERNAME")!);
            h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")!);
        });

        configurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        configurator.ConfigureEndpoints(context);
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Order API", Version = "v1" });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); 
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseSwagger(c =>
{
    c.SerializeAsV2 = true;
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order v1");
    c.RoutePrefix = string.Empty;
});

app.MapGet("api/order/health", () => Results.Ok("ok"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
