using Coding.Challenge.Dependencies.Database;
using Coding.Challenge.Dependencies.Database.Data;
using Coding.Challenge.Dependencies.Managers;
using Coding.Challenge.Dependencies.Models;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace Coding.Challenge.API.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder webApplicationBuilder)
        {
            var serviceCollection = webApplicationBuilder.Services;

            serviceCollection.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.SerializerOptions.PropertyNamingPolicy = null;
            });
            serviceCollection.AddControllers();
            serviceCollection
                .AddEndpointsApiExplorer();

            serviceCollection.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Challenge Api", Version = "v1" });
            });

            serviceCollection
             .RegisterSlowDatabase()
             .AddDataContexts(webApplicationBuilder)
             .ConfigureServices()
             .RegisterContentsManager();
            AddConfiguration(webApplicationBuilder);
            return webApplicationBuilder;
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            return  services.AddMemoryCache();
        }

        public static void AddConfiguration(WebApplicationBuilder builder)
        {
            ApiConfiguration.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
        public static IServiceCollection AddDataContexts(this IServiceCollection services, WebApplicationBuilder builder)
        {
            return services.AddDbContext<AppDbContext>(
            x => x.UseSqlServer(ApiConfiguration.ConnectionString));

        }

        private static IServiceCollection RegisterSlowDatabase(this IServiceCollection services)
        {
            services.AddSingleton<IDatabase<Content, ContentDto>, DatabaseLenta<Content, ContentDto>>();
            services.AddSingleton<IMapper<Content, ContentDto>, ContentMapper>();
            services.AddSingleton<IDadosMockados<Content>, DadosMockados>();
            return services;
        }
        private static IServiceCollection RegisterContentsManager(this IServiceCollection services)
        {
            services.AddScoped<IContentsManager, ContentsManager>();
            services.AddScoped<IBancoSqlServer, BancoSqlServer>();
            return services;
        }

        public static WebApplicationBuilder ConfigureWebHost(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder
                .WebHost
                .ConfigureLogging(logging => { logging.ClearProviders(); });

            return webApplicationBuilder;
        }
    }
}
