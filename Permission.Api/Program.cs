using Autofac;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization;
using Permission.Application.Commands;
using Permission.Application.Queries;
using Permission.Common.Domain.Interfaces.Services;
using Permission.Common.Events;
using Permission.Common.Infrastructure.EntityFramework;
using Permission.Domain.Aggregates;
using Permission.Domain.Entities;
using Permission.Domain.Interfaces;
using Permission.Domain.Interfaces.Repositories;
using Permission.Infrastructure.Config;
using Permission.Infrastructure.Consumers;
using Permission.Infrastructure.DataAccess;
using Permission.Infrastructure.Dispatchers;
using Permission.Infrastructure.Handlers;
using Permission.Infrastructure.Producers;
using Permission.Infrastructure.Repositories;
using Permission.Infrastructure.Stores;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Reflection;

namespace Permission.Api
{
    public class Program
    {
        public static IConfiguration BuiltConfiguration { get; set; }
        private static readonly string Namespace = typeof(Program).Namespace;
        private static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.',
            Namespace.LastIndexOf('.') - 1) + 1);
        public static void Main(string[] args)
        {
            GetConfiguration();

            CreateSerilogLogger(BuiltConfiguration, AppName);

            var builder = WebApplication.CreateBuilder(args);

            Log.Information("Application starting");

            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<PermissionCreatedEvent>();
            BsonClassMap.RegisterClassMap<PermissionUpdatedEvent>();
            // BsonClassMap.RegisterClassMap<PermissionGotEvent>();

            // Add services to the container.
            builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
            builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));
            Action<DbContextOptionsBuilder> configureDbContext = (o => o.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
            builder.Services.AddDbContext<DataBaseContext>(configureDbContext);
            builder.Services.AddSingleton<DataBaseContextFactory>(new DataBaseContextFactory(configureDbContext));


            // Create Database and tables for code at SQL Server
            var dataContext = builder.Services.BuildServiceProvider().GetRequiredService<DataBaseContext>();
            dataContext.Database.EnsureCreated();

            builder.Services.AddScoped<IEventStoreRepository, EventStoryRepository>();
            builder.Services.AddScoped<IEventProducer, EventProducer>();
            builder.Services.AddScoped<IEventStore, EventStore>();
            builder.Services.AddScoped<IEventSourcingHandler<PermissionAggregate>, EventSourcingHandler>();
            builder.Services.AddScoped<ICommandHandler, CommandHandler>();

            // register command handlers
            builder.Services.AddScoped(typeof(IEntityFrameworkBuilder<>), typeof(EntityFrameworkBuilder<>));
            builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
            builder.Services.AddScoped<IPermissionTypeRepository, PermissionTypeRepository>();
            var commandHandler = builder.Services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
            var dispatcher = new CommandDispatcher();
            dispatcher.RegisterHandler<RequestPermissionCommand>(commandHandler.HandleAsync);
            dispatcher.RegisterHandler<ModifyPermissionCommand>(commandHandler.HandleAsync);
            builder.Services.AddSingleton<ICommandDispatcher>(_ => dispatcher);

            builder.Services.AddScoped<IQueryHandler, QueryHandler>();
            builder.Services.AddScoped<IEventHandler, Permission.Infrastructure.Handlers.EventHandler>();
            builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
            builder.Services.AddScoped<IEventConsumer, EventConsumer>();

            // register command queries
            var queryHandler = builder.Services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
            var dispatcherQuery = new QueryDispatcher();
            dispatcherQuery.RegisterHandler<GetAllPermissionsQuery>(queryHandler.HandleAsync);
            builder.Services.AddSingleton<IQueryDispatcher<PermissionEntity>>(_ => dispatcherQuery);

            builder.Services.AddControllers();
            builder.Services.AddHostedService<ConsumerHostedService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            var app = builder.Build();

            CreateHostBuilder(args).Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }

        public static void GetConfiguration()
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string jsonConfigFile = "appsettings.Development.json";

            BuiltConfiguration = new ConfigurationBuilder()
                    .SetBasePath(directoryPath)
                    .AddJsonFile(jsonConfigFile, optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
        }

        public static void CreateSerilogLogger(IConfiguration configuration, string appName)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower()}-{DateTime.UtcNow:yyyy-MM}"
                })
               .Enrich.WithProperty("ApplicationContext", appName)
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    configuration.AddJsonFile(
                        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
                })
                .UseSerilog();
    }
}






