using OmenCore.Localization;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using OmenCore.Hardware;
using OmenCore.Models;
using OmenCore.Services;
using OmenCore.Utils;

namespace OmenCore.ViewModels
{
    /// <summary>
    /// Four small, mutually-independent maintenance features extracted out of
    /// SystemControlViewModel: GPU mode switching (Hybrid/Discrete/Integrated), display panel
    /// overdrive, the OMEN Gaming Hub cleanup wizard, and manual system-restore-point creation.
    /// None of them touch the fan/EC/undervolt/GPU-OC write paths or the tuning-safety rollback
    /// system that ties those together, which is what makes this a clean, low-risk slice to pull
    /// out on its own.
    /// </summary>
    public class SystemMaintenanceViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly LoggingService _logging;
        private readonly GpuSwitchService _gpuSwitchService;
        private readonly OmenGamingHubCleanupService _cleanupService;
        private readonly SystemRestoreService _restoreService;
        private readonly HpWmiBios? _wmiBios;

        public event PropertyChangedEventHandler? PropertyChanged;

        #region GPU Mode Switching

        private string _currentGpuMode = Strings.GpuSwitch_Detecting;
        public string CurrentGpuMode
        {
            get => _currentGpuMode;
            private set
            {
                if (_currentGpuMode != value)
                {
                    _currentGpuMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool GpuModeSwitchingSupported => _gpuSwitchService.IsSupported;

        public bool GpuModeSwitchingUnsupported => !GpuModeSwitchingSupported;

        public string GpuModeSwitchingStatusText => GpuModeSwitchingSupported
            ? Strings.GpuSwitch_Available
            : string.Format(Strings.GpuSwitch_UnavailableFormat, LocalizeUnsupportedReason(_gpuSwitchService.UnsupportedReason));

        public ObservableCollection<GpuSwitchMode> GpuSwitchModes { get; } = new();
        public GpuSwitchMode? SelectedGpuMode { get; set; }
        public ICommand SwitchGpuModeCommand { get; }

        #endregion

        #region Display Overdrive

        private bool _displayOverdriveEnabled;
        public bool DisplayOverdriveEnabled
        {
            get => _displayOverdriveEnabled;
            set
            {
                if (_displayOverdriveEnabled != value)
                {
                    _displayOverdriveEnabled = value;
                    OnPropertyChanged(nameof(DisplayOverdriveEnabled));
                    // Apply immediately when toggled
                    _ = SetDisplayOverdriveAsync(value);
                }
            }
        }

        private bool _displayOverdriveSupported;
        public bool DisplayOverdriveSupported
        {
            get => _displayOverdriveSupported;
            private set
            {
                if (_displayOverdriveSupported != value)
                {
                    _displayOverdriveSupported = value;
                    OnPropertyChanged(nameof(DisplayOverdriveSupported));
                }
            }
        }

        #endregion

        #region OMEN Cleanup

        public ObservableCollection<string> OmenCleanupSteps { get; } = new();
        public bool CleanupUninstallApp { get; set; } = true;
        public bool CleanupRemoveServices { get; set; } = true;
        public bool CleanupRegistryEntries { get; set; } = false;
        public bool CleanupRemoveLegacyInstallers { get; set; } = true;
        public bool CleanupRemoveFiles { get; set; } = true;
        public bool CleanupKillProcesses { get; set; } = true;
        public string CleanupStatusText => CleanupStatus;
        public ICommand RunCleanupCommand { get; }

        // Reversible alternative to the cleanup above: stops and disables HP analytics services, remembers
        // each original start type, and can put everything back.
        private readonly HpTelemetryServiceControl _telemetryServices =
            new(System.IO.Path.Combine(SessionSentinel.DefaultDirectory(), "hp-services-backup.json"));
        public ICommand DisableHpTelemetryCommand { get; }
        public ICommand RestoreHpTelemetryCommand { get; }

        private string _hpTelemetryStatus = "";
        public string HpTelemetryStatus
        {
            get => _hpTelemetryStatus;
            private set { if (_hpTelemetryStatus != value) { _hpTelemetryStatus = value; OnPropertyChanged(); } }
        }

        private void RefreshHpTelemetryStatus()
        {
            try
            {
                var states = _telemetryServices.Query();
                if (states.Count == 0) { HpTelemetryStatus = Strings.Telemetry_NoServices; return; }
                var off = states.Count(s => s.StartType == HpTelemetryServiceControl.Disabled);
                HpTelemetryStatus = string.Format(Strings.Telemetry_SummaryFormat, states.Count, off) +
                                    (states.Any(s => s.Managed) ? Strings.Telemetry_RestoreAvailable : Strings.Telemetry_Period);
            }
            catch (Exception ex)
            {
                _logging.Warn($"HP telemetry status failed: {ex.Message}");
                HpTelemetryStatus = Strings.Telemetry_ReadFailed;
            }
        }

        private Task RunHpTelemetryAsync(bool disable) => Task.Run(() =>
        {
            try
            {
                var result = disable ? _telemetryServices.DisableAll() : _telemetryServices.RestoreAll();
                _logging.Info($"HP telemetry services: {result.Describe(disable ? "disabled" : "restored")}");
                System.Windows.Application.Current?.Dispatcher.Invoke(() =>
                {
                    RefreshHpTelemetryStatus();
                    HpTelemetryStatus += " " + DescribeTelemetryResult(result, disable);
                });
            }
            catch (Exception ex)
            {
                _logging.Warn($"HP telemetry change failed: {ex.Message}");
                System.Windows.Application.Current?.Dispatcher.Invoke(() => HpTelemetryStatus = Strings.Telemetry_FailedPrefix + ex.Message);
            }
        });

        private bool _cleanupInProgress;
        public bool CleanupInProgress
        {
            get => _cleanupInProgress;
            private set
            {
                if (_cleanupInProgress != value)
                {
                    _cleanupInProgress = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasCleanupSteps));
                    (RunCleanupCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _cleanupComplete;
        public bool CleanupComplete
        {
            get => _cleanupComplete;
            private set
            {
                if (_cleanupComplete != value)
                {
                    _cleanupComplete = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasCleanupSteps => OmenCleanupSteps.Count > 0;

        private string _cleanupStatus = Strings.Cleanup_StatusNotChecked;
        public string CleanupStatus
        {
            get => _cleanupStatus;
            private set
            {
                if (_cleanupStatus != value)
                {
                    _cleanupStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Restore Point

        public ICommand CreateRestorePointCommand { get; }

        #endregion

        // Mirrors ViewModelBase.ExecuteWithLoadingAsync (this class doesn't inherit
        // ViewModelBase, matching the newer sub-VM convention established for UpdateViewModel /
        // GpuClampViewModel) so the three async operations below keep the exact busy-state
        // behavior they had on SystemControlViewModel, even though nothing currently binds to it.
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsNotLoading));
                }
            }
        }

        public bool IsNotLoading => !IsLoading;

        private string _loadingMessage = Strings.Common_Loading;
        public string LoadingMessage
        {
            get => _loadingMessage;
            set
            {
                if (_loadingMessage != value)
                {
                    _loadingMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private async Task ExecuteWithLoadingAsync(Func<Task> operation, string? loadingMessage = null)
        {
            try
            {
                if (loadingMessage != null)
                {
                    LoadingMessage = loadingMessage;
                }
                IsLoading = true;
                await operation();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public SystemMaintenanceViewModel(
            GpuSwitchService gpuSwitchService,
            OmenGamingHubCleanupService cleanupService,
            SystemRestoreService restoreService,
            LoggingService logging,
            HpWmiBios? wmiBios = null)
        {
            _gpuSwitchService = gpuSwitchService;
            _cleanupService = cleanupService;
            _restoreService = restoreService;
            _logging = logging;
            _wmiBios = wmiBios;

            // CanExecute, not a silent log line inside the handler: GitHub #215 clicked Apply on a board whose
            // BIOS doesn't expose GPU mode switching and saw nothing happen at all.
            SwitchGpuModeCommand = new AsyncRelayCommand(_ => SwitchGpuModeAsync(), _ => GpuModeSwitchingSupported);
            RunCleanupCommand = new AsyncRelayCommand(_ => RunCleanupAsync(), _ => !CleanupInProgress);
            DisableHpTelemetryCommand = new AsyncRelayCommand(_ => RunHpTelemetryAsync(true));
            RestoreHpTelemetryCommand = new AsyncRelayCommand(_ => RunHpTelemetryAsync(false));
            RefreshHpTelemetryStatus();
            CreateRestorePointCommand = new AsyncRelayCommand(_ => CreateRestorePointAsync());

            GpuSwitchModes.Add(GpuSwitchMode.Hybrid);
            GpuSwitchModes.Add(GpuSwitchMode.Discrete);
            GpuSwitchModes.Add(GpuSwitchMode.Integrated);

            DetectGpuMode();
            DetectDisplayOverdrive();
        }

        private void DetectGpuMode()
        {
            try
            {
                var mode = _gpuSwitchService.DetectCurrentMode();
                CurrentGpuMode = ModeName(mode);
                _logging.Info($"Detected GPU mode: {CurrentGpuMode}");
            }
            catch (Exception ex)
            {
                _logging.ErrorWithContext(
                    component: "SystemMaintenanceViewModel",
                    operation: "DetectGpuMode",
                    message: "Failed to detect GPU mode",
                    ex: ex);
                CurrentGpuMode = Strings.GpuSwitch_ModeDetectionFailed;
            }
        }

        private async Task SwitchGpuModeAsync()
        {
            if (SelectedGpuMode == null)
            {
                _logging.Warn("No GPU mode selected");
                return;
            }

            // Check if GPU mode switching is supported BEFORE attempting
            if (!_gpuSwitchService.IsSupported)
            {
                var reason = _gpuSwitchService.UnsupportedReason;
                _logging.Info($"GPU mode switching not available: {reason}");
                return;
            }

            await ExecuteWithLoadingAsync(() => {
                var targetMode = SelectedGpuMode.Value;
                _logging.Info($"⚡ Attempting to switch GPU mode to: {targetMode}");

                var success = _gpuSwitchService.Switch(targetMode);

                if (success)
                {
                    _logging.Info($"✓ GPU mode switch initiated. System restart required to apply changes.");

                    // Show restart prompt
                    var result = System.Windows.MessageBox.Show(
                        string.Format(Strings.GpuSwitch_RestartBodyFormat, ModeName(targetMode)),
                        Strings.GpuSwitch_RestartTitle,
                        System.Windows.MessageBoxButton.YesNo,
                        System.Windows.MessageBoxImage.Question);

                    if (result == System.Windows.MessageBoxResult.Yes)
                    {
                        // Restart the system
                        System.Diagnostics.Process.Start("shutdown", "/r /t 0");
                    }
                }
                else
                {
                    _logging.Error("✗ GPU mode switch failed");
                    System.Windows.MessageBox.Show(
                        Strings.GpuSwitch_FailedBody,
                        Strings.GpuSwitch_FailedTitle,
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }

                return Task.CompletedTask;
            });
        }

        private async Task SetDisplayOverdriveAsync(bool enabled)
        {
            if (_wmiBios == null) return;

            await Task.Run(() =>
            {
                try
                {
                    var success = _wmiBios.SetDisplayOverdrive(enabled);
                    if (!success)
                    {
                        _logging.Warn($"Display overdrive toggle failed");
                        // Revert UI
                        _displayOverdriveEnabled = !enabled;
                        System.Windows.Application.Current?.Dispatcher?.BeginInvoke(() => OnPropertyChanged(nameof(DisplayOverdriveEnabled)));
                    }
                }
                catch (Exception ex)
                {
                    _logging.Error($"Display overdrive error: {ex.Message}", ex);
                    _displayOverdriveEnabled = !enabled;
                    System.Windows.Application.Current?.Dispatcher?.BeginInvoke(() => OnPropertyChanged(nameof(DisplayOverdriveEnabled)));
                }
            });
        }

        private void DetectDisplayOverdrive()
        {
            if (_wmiBios == null || !_wmiBios.IsAvailable) return;

            try
            {
                var status = _wmiBios.GetDisplayOverdrive();
                if (status.HasValue)
                {
                    DisplayOverdriveSupported = true;
                    _displayOverdriveEnabled = status.Value;
                    OnPropertyChanged(nameof(DisplayOverdriveEnabled));
                    _logging.Info($"✓ Display overdrive supported. Current: {(status.Value ? "enabled" : "disabled")}");
                }
                else
                {
                    DisplayOverdriveSupported = false;
                    _logging.Info("Display overdrive not supported on this model");
                }
            }
            catch (Exception ex)
            {
                DisplayOverdriveSupported = false;
                _logging.Warn($"Display overdrive detection failed: {ex.Message}");
            }
        }

        private async Task RunCleanupAsync()
        {
            OmenCleanupSteps.Clear();
            _cleanupService.StepCompleted += OnStepCompleted;

            try
            {
                await ExecuteWithLoadingAsync(async () =>
                {
                    var options = new OmenCleanupOptions
                    {
                        RemoveStorePackage = CleanupUninstallApp,
                        RemoveServicesAndTasks = CleanupRemoveServices,
                        RemoveRegistryTraces = CleanupRegistryEntries,
                        RemoveLegacyInstallers = CleanupRemoveLegacyInstallers,
                        RemoveResidualFiles = CleanupRemoveFiles,
                        KillRunningProcesses = CleanupKillProcesses,
                        DryRun = false,
                        PreserveFirewallRules = true
                    };

                    var result = await _cleanupService.CleanupAsync(options);
                    CleanupStatus = result.Success ? Strings.Cleanup_Complete : Strings.Cleanup_Failed;
                    CleanupComplete = result.Success;
                }, Strings.Cleanup_Running);
            }
            finally
            {
                _cleanupService.StepCompleted -= OnStepCompleted;
                CleanupInProgress = false;
            }
        }

        private void OnStepCompleted(string step)
        {
            System.Windows.Application.Current?.Dispatcher?.BeginInvoke(() =>
            {
                var localized = LocalizeCleanupStep(step);
                OmenCleanupSteps.Add(localized);
                CleanupStatus = localized;
                OnPropertyChanged(nameof(HasCleanupSteps));
            });
        }

        private async Task CreateRestorePointAsync()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                _logging.Info("Creating system restore point...");
                var result = await _restoreService.CreateRestorePointAsync("OmenCore - Before System Changes");

                if (result.Success)
                {
                    _logging.Info($"✓ System restore point created successfully (Sequence: {result.SequenceNumber})");
                }
                else
                {
                    _logging.Error($"✗ Failed to create restore point: {result.Message}");
                }
            }, Strings.Cleanup_CreatingRestorePoint);
        }

        public void Dispose()
        {
            // Nothing persistent to unhook: _cleanupService.StepCompleted is subscribed and
            // unsubscribed entirely within RunCleanupAsync's own try/finally.
        }

        /// <summary>Localized display name for the GPU switch selector and restart prompt.</summary>
        private static string ModeName(GpuSwitchMode mode) => mode switch
        {
            GpuSwitchMode.Hybrid => Strings.GpuSwitch_ModeHybrid,
            GpuSwitchMode.Discrete => Strings.GpuSwitch_ModeDiscrete,
            GpuSwitchMode.Integrated => Strings.GpuSwitch_ModeIntegrated,
            _ => Strings.GpuSwitch_ModeUnknown
        };

        /// <summary>Maps Core's canonical English unsupported reasons (GpuSwitchService) to UI text.</summary>
        private static string LocalizeUnsupportedReason(string? reason)
        {
            if (string.IsNullOrEmpty(reason)) return string.Empty;
            if (reason == "Could not detect system information") return Strings.GpuSwitch_ReasonNoSysInfo;
            if (reason == "GPU mode switching only supported on HP systems") return Strings.GpuSwitch_ReasonNotHp;
            const string omenPrefix = "GPU mode switching only supported on HP OMEN models (detected: ";
            if (reason.StartsWith(omenPrefix, StringComparison.Ordinal))
                return string.Format(Strings.GpuSwitch_ReasonNotOmenFormat, reason.Substring(omenPrefix.Length).TrimEnd(')'));
            if (reason == "HP BIOS does not support GPU mode switching via WMI") return Strings.GpuSwitch_ReasonBiosNoWmi;
            const string errorPrefix = "Error checking GPU mode support: ";
            if (reason.StartsWith(errorPrefix, StringComparison.Ordinal))
                return string.Format(Strings.GpuSwitch_ReasonErrorFormat, reason.Substring(errorPrefix.Length));
            return reason;
        }

        /// <summary>Builds the localized telemetry result line Core's Result.Describe would print.</summary>
        private static string DescribeTelemetryResult(HpTelemetryServiceControl.Result result, bool disable)
        {
            if (result.Failed > 0)
                return string.Format(Strings.Telemetry_ResultFailedFormat,
                    disable ? Strings.Telemetry_VerbDisabled : Strings.Telemetry_VerbRestored,
                    result.Changed, result.Failed);
            if (result.Changed == 0) return Strings.Telemetry_NothingToChange;
            return string.Format(disable ? Strings.Telemetry_ResultDisabledFormat : Strings.Telemetry_ResultRestoredFormat, result.Changed);
        }

        /// <summary>Keeps Core's "[n/m] " progress prefix and localizes the step message itself.</summary>
        private static string LocalizeCleanupStep(string step)
        {
            var close = step.IndexOf("] ", StringComparison.Ordinal);
            if (!step.StartsWith("[", StringComparison.Ordinal) || close < 0) return step;
            var prefix = step.Substring(0, close + 2);
            var message = step.Substring(close + 2);
            var localized = message switch
            {
                "Killed running OMEN processes" => Strings.Cleanup_StepKilledProcesses,
                "Removing Store packages (this may take a minute)..." => Strings.Cleanup_StepStorePackages,
                "Running legacy uninstallers (this may take a few minutes)..." => Strings.Cleanup_StepLegacyUninstallers,
                "Cleaning up services and scheduled tasks..." => Strings.Cleanup_StepServicesTasks,
                "⚠ OmenCap detected in DriverStore - see log for removal instructions" => Strings.Cleanup_StepOmenCap,
                "Firewall rules preserved per user preference" => Strings.Cleanup_StepFirewallPreserved,
                "✓ Cleanup completed successfully" => Strings.Cleanup_StepCompleted,
                "⚠ Cleanup cancelled" => Strings.Cleanup_StepCancelled,
                _ => message
            };
            return prefix + localized;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
