using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ExcelToDxfAvalonia.Extensions;

[SupportedOSPlatform("windows")]
internal static class ConsoleManager
{
    private const int SwHide = 0;
    private const int SwShow = 5;

    public static void ShowConsole() => ShowWindow(GetConsoleWindow(), SwShow);

    public static void HideConsole() => ShowWindow(GetConsoleWindow(), SwHide);

    public static bool IsConsoleVisible() => IsWindowVisible(GetConsoleWindow());

    [DllImport("kernel32.dll", EntryPoint = "AllocConsole")]
    public static extern void OpenConsole();

    [DllImport("kernel32.dll", EntryPoint = "FreeConsole")]
    public static extern void CloseConsole();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);
}
