#define AppName "ExcelToDxfAvalonia"
#define AppExec "publish\ExcelToDxfAvalonia.exe"
#define AppVersion GetStringFileInfo(AppExec, "Assembly Version")
#define AppPublisher "yurygolub"
#define AppURL ""
#define OutputDir "deploy"
#define SourceDir "publish\*"

[Setup]
AppName={#AppName}
AppId=net.example.excel-to-dxf-avalonia
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
VersionInfoVersion={#AppVersion}
OutputDir={#OutputDir}
OutputBaseFilename={#AppName}.x64
AppPublisher={#AppPublisher}
AppCopyright=Copyright (C) {#AppPublisher} 2023
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
SetupIconFile=
DefaultGroupName={#AppName}
DefaultDirName={autopf}\{#AppName}
AllowNoIcons=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
MinVersion=10
PrivilegesRequired=lowest

[Files]
Source: {#SourceDir}; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs;

[Tasks]
Name: "desktopicon"; Description: "Create a &Desktop Icon"; GroupDescription: "Additional icons:"; Flags: unchecked

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppName}.exe"
Name: "{userdesktop}\{#AppName}"; Filename: "{app}\{#AppName}.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppName}.exe"; Description: Start Application Now; Flags: postinstall nowait skipifsilent

[UninstallDelete]
Type: dirifempty; Name: "{app}"
