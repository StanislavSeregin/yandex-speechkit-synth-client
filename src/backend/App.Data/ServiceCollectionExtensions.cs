﻿using Microsoft.Extensions.DependencyInjection;

namespace App.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLiteDbContext(this IServiceCollection services)
        => services.AddScoped<ILiteDbContext, LiteDbContext>();
}
