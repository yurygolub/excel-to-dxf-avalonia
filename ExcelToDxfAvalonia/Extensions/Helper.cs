using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;

namespace ExcelToDxfAvalonia.Extensions;

internal class Helper
{
    public static async Task OpenDialog(string title, string message)
    {
        _ = title ?? throw new ArgumentNullException(nameof(title));
        _ = message ?? throw new ArgumentNullException(nameof(message));

        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            IMsBox<ButtonResult> box = MessageBoxManager.GetMessageBoxStandard(
                title,
                message,
                ButtonEnum.Ok,
                windowStartupLocation: WindowStartupLocation.CenterOwner);

            await box.ShowWindowDialogAsync(desktop.MainWindow);
        }
    }
}
