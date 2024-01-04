using System;
using ExcelToDxfAvalonia.Models;
using ExcelToDxfAvalonia.ViewModels;
using ExcelToDxfAvalonia.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;

namespace ExcelToDxfAvalonia;

public class Startup
{
    public IConfiguration Configuration { get; } = new ConfigurationBuilder()
        .SetBasePath(Environment.CurrentDirectory)
        .AddJsonFile("appsettings.json", true, true)
        .Build();

    public IServiceCollection ConfigureServices(IServiceCollection services)
    {
        return services
            .AddSingleton<MainWindow>()
            .AddSingleton<MainViewModel>()
            .AddSingleton<MainModel>()
            .AddTransient<AboutView>()
            .AddSingleton<AboutViewModel>()
            .AddLogging(builder => builder.AddNLog(this.Configuration));
    }
}
