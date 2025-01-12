using Application.Core;
using MassTransit;
using Microsoft.IdentityModel.Tokens;
using Order.Domain;
using Order.Infrastructure;
using orders_microservice.Utils.Core.Src.Application.NotificationService;
using orders_microservice.Utils.Core.Src.Infrastructure.FireBaseNotificationsService;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Text;
using System.Text.Json.Nodes;
using System.Security.Claims;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Order.Extensions
{
    public static class ConfigureServicesExtension
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IdService<string>, GuidGenerator>();
            services.AddScoped<Logger, DotNetLogger>();
            services.AddScoped<IMessageBrokerService, RabbitMQService>();
            services.AddScoped<ILocationService<JsonNode>, GoogleMapService>();
            services.AddScoped<ISagaStateMachineService<string>, SagaStateMachineRepository>();
            services.AddSingleton<INotificationService, FirebaseNotificationsService>();
            services.AddSingleton<MongoEventStore>();
            services.AddSingleton<IEventStore, MongoEventStore>();
            services.AddSingleton<IOrderRepository, MongoOrderRepository>();
            services.AddSingleton<IPerformanceLogsRepository, MongoPerformanceLogsRespository>();
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        RoleClaimType = ClaimTypes.Role,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!))
                    };
                });
        }

        public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddSagaStateMachine<OrderStateMachine, OrderStatusStates>()
                    .MongoDbRepository<OrderStatusStates>(r =>
                    {
                        r.Connection = Environment.GetEnvironmentVariable("CONNECTION_URI");
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
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Order API", Version = "v1" });
            });
        }

        public static void UseSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order v1");
                c.RoutePrefix = string.Empty;
            });
        }

        public static void ConfigureFirebase(this IServiceCollection services)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("FIREBASE-SERVICES"))
            });
        }
    }
}
