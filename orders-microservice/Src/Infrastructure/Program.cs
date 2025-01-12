using Application.Core;
using DotNetEnv;
using MassTransit;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Order.Domain;
using System.Text.Json.Nodes;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using orders_microservice.Utils.Core.Src.Application.NotificationService;
using orders_microservice.Utils.Core.Src.Infrastructure.FireBaseNotificationsService;
using Order.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("FIREBASE-SERVICES"))
});

builder.Services.AddSingleton<MongoEventStore>();
builder.Services.AddSingleton<INotificationService, FirebaseNotificationsService>();
builder.Services.AddScoped<IEventStore, MongoEventStore>();
builder.Services.AddScoped<IdService<string>, GuidGenerator>();
builder.Services.AddScoped<IOrderRepository, MongoOrderRepository>();
builder.Services.AddScoped<IMessageBrokerService, RabbitMQService>();
builder.Services.AddScoped<ILocationService<JsonNode>, GoogleMapService>();
builder.Services.AddScoped<ISagaStateMachineService<string>, SagaStateMachineRepository>();
builder.Services.AddScoped<Logger, DotNetLogger>();
builder.Services.AddScoped<IPerformanceLogsRepository, MongoPerformanceLogsRespository>();
builder.Services.AddScoped<OrderController>();
builder.Services.AddControllers(options => {
    options.Filters.Add<GlobalExceptionFilter>();
});

var certSection = builder.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate");
certSection["Path"] = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel_CertificatesDefault_Path")!;
certSection["Password"] = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel_CertificatesDefault_Password")!;

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Order API", Version = "v1" });
});

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


builder.WebHost.ConfigureKestrel(options =>
{
     options.ListenAnyIP(5000); 
});

builder.Services.AddControllers();

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

    busConfigurator.AddConsumer<OrderRejectedConsumer>();
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

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Job v1");
    c.RoutePrefix = string.Empty;
});
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();

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
