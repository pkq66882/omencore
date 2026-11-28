#define MyAppName "OmenCore"
#ifndef MyAppVersion
  #define MyAppVersion "5.0.3"
#endif
#ifndef MyPublishDir
  #define MyPublishDir "..\\publish\\win-x64"
#endif
#define MyAppPublisher "OmenCore Project"
#define MyAppExeName "OmenCore.exe"

[Setup]
AppId={{6F5B6F3F-8FAF-4FC8-A5E0-4E2C0E8F2E2B}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL="https://github.com/theantipopau/OmenCore"
AppSupportURL="https://github.com/theantipopau/OmenCore/issues"
AppUpdatesURL="https://github.com/theantipopau/OmenCore/releases"
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
SetupIconFile=..\src\OmenCoreApp\Assets\OmenCore.ico
; Branding images
WizardImageFile=wizard-large.bmp
WizardSmallImageFile=wizard-small.bmp
Compression=lzma2/ultra64
SolidCompression=yes
OutputDir=..\\artifacts
OutputBaseFilename=OmenCoreSetup-{#MyAppVersion}
PrivilegesRequired=admin
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64compatible
; Always preselect English on the language dialog (matches the stock "auto"-follow app default
; being presented in English first); users pick their language explicitly on the first page.
LanguageDetectionMethod=none
; Modern look
DisableWelcomePage=no
LicenseFile=
InfoBeforeFile=
InfoAfterFile=

; The four setup languages mirror the app's Language.Supported list (auto/en/ru/tr/zh, minus
; "auto" which is the app-side system-follow default). The second MessagesFile per language
; carries the OmenCore-specific WelcomeLabel2 override; everything else comes from the
; stock/official .isl files so ru/tr keep native-quality wizard strings.
[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl,installer-en.ism"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl,installer-ru.ism"
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl,installer-tr.ism"
Name: "chinesesimplified"; MessagesFile: "ChineseSimplified.isl,installer-zh.ism"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "startmenu"; Description: "{cm:TaskStartMenu}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "installpawnio"; Description: "{cm:TaskInstallPawnIO}"; GroupDescription: "{cm:GroupDrivers}"
Name: "autostart"; Description: "{cm:TaskAutostart}"; GroupDescription: "{cm:GroupStartup}"; Flags: unchecked

[Files]
; Self-contained app with embedded .NET runtime - no separate .NET installation needed
Source: "{#MyPublishDir}\\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; PawnIO installer (optional)
Source: "PawnIO_setup.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall; Tasks: installpawnio
; Default config
Source: "..\\config\\default_config.json"; DestDir: "{app}\\config"; Flags: ignoreversion onlyifdoesntexist

[Icons]
Name: "{autoprograms}\\{#MyAppName}"; Filename: "{app}\\{#MyAppExeName}"; WorkingDir: "{app}"
Name: "{autodesktop}\\{#MyAppName}"; Filename: "{app}\\{#MyAppExeName}"; Tasks: desktopicon; WorkingDir: "{app}"
; NOTE: Startup is now handled via Task Scheduler in the app itself (Settings > Start with Windows)
; This avoids double-startup issues and ensures elevated privileges for hardware access

[Run]
; Install PawnIO driver if bundled (PawnIO requires an explicit operation verb plus dash-style silent flag)
Filename: "{tmp}\PawnIO_setup.exe"; Parameters: "-install -silent"; StatusMsg: "{cm:StatusInstallingPawnIO}"; Flags: waituntilterminated runhidden; Tasks: installpawnio; Check: not IsPawnIOInstalled
; Create scheduled task for autostart if user selected it (runs with elevated privileges)
Filename: "schtasks"; Parameters: "/create /tn ""OmenCore"" /tr ""\""{app}\\{#MyAppExeName}\"" --minimized"" /sc onlogon /rl highest /f"; Flags: runhidden; Tasks: autostart
; Launch OmenCore with elevation (shellexec verb=runas)
Filename: "{app}\\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent shellexec runascurrentuser; Verb: runas
; In-app updater runs the installer silently and passes /RELAUNCH=1 so the new version opens again (skipifsilent above would otherwise leave the app closed)
Filename: "{app}\{#MyAppExeName}"; Flags: nowait shellexec runascurrentuser; Verb: runas; Check: ShouldRelaunchAfterUpdate

[UninstallRun]
; Stop OmenCore and HardwareWorker if running
Filename: "taskkill"; Parameters: "/F /IM OmenCore.exe"; Flags: runhidden waituntilterminated; RunOnceId: "StopOmenCore"
Filename: "taskkill"; Parameters: "/F /IM OmenCore.HardwareWorker.exe"; Flags: runhidden waituntilterminated; RunOnceId: "StopHardwareWorker"
; Hand the fans back to the BIOS. The force-kill above skips OmenCore's OnExit, which is where
; FanService.Dispose resets the EC and clears the firmware Max flag. Firmware Max is a register,
; not process state: without this the fans stay pinned after OmenCore is uninstalled. Runs
; headless (no window, exit code only) and before the files are removed below.
Filename: "{app}\{#MyAppExeName}"; Parameters: "--restore-fans"; Flags: runhidden waituntilterminated; RunOnceId: "RestoreFanControl"
; Remove scheduled task for autostart
Filename: "schtasks"; Parameters: "/delete /tn ""OmenCore"" /f"; Flags: runhidden waituntilterminated; RunOnceId: "RemoveOmenCoreTask"

[UninstallDelete]
Type: dirifempty; Name: "{app}"

[Code]
// Maps the setup language chosen on the first dialog to the app's canonical language code
// (Language.Supported: auto/en/ru/tr/zh). Written to {app}\installer-language.txt after the
// files land; App.xaml.cs adopts it only on a genuine first run, so an upgrade keeps the
// user's own language setting.
function InstallerAppLanguage: String;
var
  L: String;
begin
  L := ExpandConstant('{language}');
  if L = 'russian' then
    Result := 'ru'
  else if L = 'turkish' then
    Result := 'tr'
  else if L = 'chinesesimplified' then
    Result := 'zh'
  else
    Result := 'en';
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  SeedPath: String;
begin
  if CurStep = ssPostInstall then
  begin
    SeedPath := ExpandConstant('{app}\installer-language.txt');
    SaveStringToFile(SeedPath, InstallerAppLanguage + #13#10, False);
  end;
end;

function ShouldRelaunchAfterUpdate: Boolean;
begin
  Result := WizardSilent and (ExpandConstant('{param:RELAUNCH|0}') = '1');
end;

function IsPawnIOInstalled: Boolean;
var
  InstallPath: String;
begin
  Result := RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\PawnIO', 'InstallLocation', InstallPath);
  if not Result then
    Result := DirExists(ExpandConstant('{pf}\\PawnIO'));
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpSelectTasks then
  begin
    if IsPawnIOInstalled then
    begin
      // PawnIO already installed, uncheck the task
      WizardForm.TasksList.Checked[2] := False;
    end;
  end;
end;

function InitializeSetup(): Boolean;
begin
  Result := True;
end;

// Hands fan control back to the BIOS without starting the UI. OmenCore.exe --restore-fans logs in,
// clears the firmware Max flag and restores the Default thermal mode, then exits with 0 (restored)
// or 1 (nothing to restore). Used after a force-kill, which is the only way past a running GUI.
//
// Only an exe from 5.0.0 on knows --restore-fans. On an upgrade, {app} still holds the OLD exe: a 4.x
// build ignores the flag and opens its full window, and the wait below never returns, so the upgrade
// hangs with the old version on screen (5.0.0 regression). Hence the version gate.
procedure RestoreFanControl;
var
  ResultCode: Integer;
  ExePath: String;
  VersionMS, VersionLS: Cardinal;
begin
  ExePath := ExpandConstant('{app}\{#MyAppExeName}');
  if not FileExists(ExePath) then
  begin
    Log('Fan hand-back skipped: ' + ExePath + ' not found');
    Exit;
  end;

  if not GetVersionNumbers(ExePath, VersionMS, VersionLS) or (VersionMS < $00050000) then
  begin
    Log('Fan hand-back skipped: installed OmenCore.exe predates 5.0.0 and has no --restore-fans');
    Exit;
  end;

  if not Exec(ExePath, '--restore-fans', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    Log('Fan hand-back could not start OmenCore.exe')
  else
    Log('Fan hand-back exit code: ' + IntToStr(ResultCode));
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  ResultCode: Integer;
begin
  Result := '';
  NeedsRestart := False;
  
  // Stop OmenCore if running to prevent file locks
  Exec('taskkill', '/F /IM OmenCore.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Exec('taskkill', '/F /IM OmenCore.HardwareWorker.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  
  // Small delay to ensure processes are fully terminated
  Sleep(500);

  // An upgrade kills the same way an uninstall does, so it leaves the same firmware Max flag
  // behind: hand the fans back before the new files land on top of the old ones.
  RestoreFanControl;
end;

function InitializeUninstall(): Boolean;
begin
  Result := True;
end;

[CustomMessages]
; WelcomeLabel2 lives in per-language .ism files (installer-en/zh/ru/tr.ism), referenced as
; the second MessagesFile in [Languages], so each setup language carries its own
; OmenCore-specific welcome copy; the stock .isl files supply the standard wizard strings.
; Task-page strings below were previously hardcoded English in [Tasks]; the four language
; prefixes mirror the [Languages] names. English values are the original strings verbatim.
english.TaskStartMenu=Create Start Menu shortcut
russian.TaskStartMenu=Создать ярлык в меню «Пуск»
turkish.TaskStartMenu=Başlat menüsü kısayolu oluştur
chinesesimplified.TaskStartMenu=创建开始菜单快捷方式

english.TaskInstallPawnIO=Install PawnIO Driver (Recommended - Secure Boot compatible; enables MSR/EC access)
russian.TaskInstallPawnIO=Установить драйвер PawnIO (рекомендуется; совместимо с Secure Boot; открывает доступ к MSR/EC)
turkish.TaskInstallPawnIO=PawnIO sürücüsünü kur (önerilir - Secure Boot uyumlu; MSR/EC erişimi sağlar)
chinesesimplified.TaskInstallPawnIO=安装 PawnIO 驱动（推荐——兼容安全启动；启用 MSR/EC 访问）

english.TaskAutostart=Start OmenCore with Windows
russian.TaskAutostart=Запускать OmenCore вместе с Windows
turkish.TaskAutostart=OmenCore'u Windows ile başlat
chinesesimplified.TaskAutostart=开机自动启动 OmenCore

english.GroupDrivers=Hardware Drivers:
russian.GroupDrivers=Драйверы оборудования:
turkish.GroupDrivers=Donanım sürücüleri:
chinesesimplified.GroupDrivers=硬件驱动：

english.GroupStartup=Startup Options:
russian.GroupStartup=Параметры запуска:
turkish.GroupStartup=Başlatma seçenekleri:
chinesesimplified.GroupStartup=启动选项：

english.StatusInstallingPawnIO=Installing PawnIO driver (Secure Boot compatible)...
russian.StatusInstallingPawnIO=Установка драйвера PawnIO (совместимо с Secure Boot)...
turkish.StatusInstallingPawnIO=PawnIO sürücüsü kuruluyor (Secure Boot uyumlu)...
chinesesimplified.StatusInstallingPawnIO=正在安装 PawnIO 驱动（兼容安全启动）...
