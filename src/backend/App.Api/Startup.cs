﻿using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using App.Data;
using App.YandexClient;
using App.Api.HostedServices;

namespace App.Api;

public class Startup
{
    public const string STATIC_FILES_PATH = "wwwroot";
    public const string YANDEX_CLIENT_CONFIGURATION_KEY = "YandexClientSettings";

    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddCors()
            .AddControllers();

        services
            .AddLogging()
#if !DEBUG
            .AddHostedService<ConsoleGuiHostedService>()
#endif
            .AddHostedService<InitAppSettingsHostedService>()
            .AddLiteDbContext()
            .AddYandexClient(_configuration, YANDEX_CLIENT_CONFIGURATION_KEY)
            .AddMediatR(typeof(Startup));
    }

    public void Configure(IApplicationBuilder app)
    {
        app
            .UseCors(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader();
            })
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

#if !DEBUG
        ConfigureEmbeddedAssets(app);
#endif
    }

#pragma warning disable IDE0051 // Remove unused private members
    private static void ConfigureEmbeddedAssets(IApplicationBuilder app)
    {
        var manifestEmbeddedFileProvider = new ManifestEmbeddedFileProvider(
            Assembly.GetExecutingAssembly(),
            STATIC_FILES_PATH);

        var staticFileOptions = new StaticFileOptions
        {
            FileProvider = manifestEmbeddedFileProvider,
            RequestPath = string.Empty,
        };

        app.UseSpaStaticFiles(staticFileOptions);
        app.UseSpa(spaBuilder =>
        {
            spaBuilder.Options.DefaultPageStaticFileOptions = staticFileOptions;
        });
    }
#pragma warning restore IDE0051 // Remove unused private members
}
