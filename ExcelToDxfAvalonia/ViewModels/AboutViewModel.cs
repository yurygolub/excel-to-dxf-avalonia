using System;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace ExcelToDxfAvalonia.ViewModels;

public class AboutViewModel : ViewModelBase
{
    private readonly ILogger<AboutViewModel> logger;

    public AboutViewModel(ILogger<AboutViewModel> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        try
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            var buildDate = new DateTime(2000, 1, 1)
                .AddDays(version.Build)
                .AddSeconds(version.Revision * 2);

            this.Version = version.ToString();
            this.BuildDate = buildDate.ToString();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception");
        }
    }

    public string Version { get; }

    public string BuildDate { get; }
}
