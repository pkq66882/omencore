using System;
using OmenCore.Localization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using OmenCore.Services;
using OmenCore.Services.SystemOptimizer;
using OmenCore.Services.SystemOptimizer.Optimizations;
using OmenCore.Utils;

namespace OmenCore.ViewModels
{
    /// <summary>
    /// ViewModel for the System Optimizer view.
    /// </summary>
    public class SystemOptimizerViewModel : INotifyPropertyChanged
    {
        private readonly LoggingService _logger;
        private readonly SystemOptimizerService _optimizerService;
        
        private bool _isLoading;
        private string _statusMessage = Strings.SystemOptimizer_StatusDone;
        private OptimizationState? _currentState;
        private bool _hasUnsavedChanges;
        private int _operationCurrentStep;
        private int _operationTotalSteps;
        private string _operationProgressLabel = string.Empty;
        private readonly DispatcherTimer _verificationTimer;
        private DateTime? _lastVerifiedAt;
        private string _verificationSummary = Strings.SystemOptimizerViewModel_VerificationPending;
        private bool _hasStateDrift;
        private OptimizationDriftSummary _lastDriftSummary = new();
        private string _driftWarningRollup = Strings.SystemOptimizerViewModel_NoDriftExplanations;
        private string _lastExportedReportPath = string.Empty;
        private OptimizationState? _expectedStateForDrift;
        private string _preflightSummary = Strings.SystemOptimizerViewModel_PreflightNotLoaded;
        private string _preflightWarningRollup = Strings.SystemOptimizerViewModel_NoPreflightWarnings;
        private bool _preflightHasHighRisk;
        private int _preflightHighRiskCount;
        
        public event PropertyChangedEventHandler? PropertyChanged;

        public SystemOptimizerViewModel(LoggingService logger)
        {
            _logger = logger;
            
            // Initialize optimizer (creates its own backup service internally)
            _optimizerService = new SystemOptimizerService(logger);
            
            // Subscribe to status updates — marshal to UI thread so StatusMessage
            // can be safely set from background threads (avoids cross-thread binding errors).
            _optimizerService.StatusChanged += status => 
            {
                DispatcherHelper.RunOnUiThread(() => HandleServiceStatus(status));
            };
            
            // Initialize collections
            PowerOptimizations = new ObservableCollection<OptimizationItem>();
            ServiceOptimizations = new ObservableCollection<OptimizationItem>();
            NetworkOptimizations = new ObservableCollection<OptimizationItem>();
            InputOptimizations = new ObservableCollection<OptimizationItem>();
            VisualOptimizations = new ObservableCollection<OptimizationItem>();
            StorageOptimizations = new ObservableCollection<OptimizationItem>();
            
            // Initialize commands
            RefreshCommand = new RelayCommand(_ => _ = RefreshStateAsync());
            ApplyGamingMaxCommand = new RelayCommand(_ => _ = ApplyGamingMaxAsync());
            ApplyBalancedCommand = new RelayCommand(_ => _ = ApplyBalancedAsync());
            RevertAllCommand = new RelayCommand(_ => _ = RevertAllAsync());
            ApplyPowerCommand = new RelayCommand(_ => _ = ApplyCategoryAsync("Power & Performance", PowerOptimizations), _ => CanApplyCategory(PowerOptimizations));
            ApplyServicesCommand = new RelayCommand(_ => _ = ApplyCategoryAsync("Windows Services", ServiceOptimizations), _ => CanApplyCategory(ServiceOptimizations));
            ApplyNetworkCommand = new RelayCommand(_ => _ = ApplyCategoryAsync("Network & Latency", NetworkOptimizations), _ => CanApplyCategory(NetworkOptimizations));
            ApplyInputCommand = new RelayCommand(_ => _ = ApplyCategoryAsync("Input & Gaming", InputOptimizations), _ => CanApplyCategory(InputOptimizations));
            ApplyVisualCommand = new RelayCommand(_ => _ = ApplyCategoryAsync("Visual Effects", VisualOptimizations), _ => CanApplyCategory(VisualOptimizations));
            ApplyStorageCommand = new RelayCommand(_ => _ = ApplyCategoryAsync("Storage & Disk", StorageOptimizations), _ => CanApplyCategory(StorageOptimizations));
            RefreshPreflightCommand = new RelayCommand(_ => _ = RefreshPreflightAsync());
            ExportOptimizationReportCommand = new RelayCommand(_ => _ = ExportOptimizationReportAsync(), _ => !IsLoading);
            OpenExportedReportCommand = new RelayCommand(_ => OpenExportedReport(), _ => CanOpenExportedReport);
            CopyExportedReportPathCommand = new RelayCommand(_ => CopyExportedReportPath(), _ => CanCopyExportedReportPath);

            _verificationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromHours(1)
            };
            _verificationTimer.Tick += async (_, _) => await RunBackgroundVerificationAsync();
            _verificationTimer.Start();
            
            // Load initial state
            _ = RefreshStateAsync();
        }

        // ========== PROPERTIES ==========

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNotLoading)); RaiseCategoryCommandStates(); }
        }

        public bool IsNotLoading => !_isLoading;

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public bool HasUnsavedChanges
        {
            get => _hasUnsavedChanges;
            set { _hasUnsavedChanges = value; OnPropertyChanged(); }
        }

        public int OperationCurrentStep
        {
            get => _operationCurrentStep;
            private set { _operationCurrentStep = value; OnPropertyChanged(); OnPropertyChanged(nameof(OperationProgressValue)); OnPropertyChanged(nameof(OperationProgressText)); OnPropertyChanged(nameof(HasOperationProgress)); }
        }

        public int OperationTotalSteps
        {
            get => _operationTotalSteps;
            private set { _operationTotalSteps = value; OnPropertyChanged(); OnPropertyChanged(nameof(OperationProgressMaximum)); OnPropertyChanged(nameof(OperationProgressText)); OnPropertyChanged(nameof(HasOperationProgress)); }
        }

        public string OperationProgressLabel
        {
            get => _operationProgressLabel;
            private set { _operationProgressLabel = value; OnPropertyChanged(); }
        }

        public bool HasOperationProgress => OperationTotalSteps > 0;
        public double OperationProgressValue => OperationCurrentStep;
        public double OperationProgressMaximum => OperationTotalSteps > 0 ? OperationTotalSteps : 1;
        public string OperationProgressText => HasOperationProgress ? string.Format(Strings.Optimizer_StepProgressFormat, OperationCurrentStep, OperationTotalSteps) : string.Empty;

        public int ActiveOptimizationCount => _currentState?.ActiveCount ?? 0;
        public int TotalOptimizationCount => _currentState?.TotalCount ?? 0;
        public DateTime? LastVerifiedAt
        {
            get => _lastVerifiedAt;
            private set
            {
                _lastVerifiedAt = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LastVerifiedText));
            }
        }

        public string LastVerifiedText => LastVerifiedAt.HasValue
            ? string.Format(Strings.SystemOptimizerViewModel_LastVerifiedAt, LastVerifiedAt.Value)
            : Strings.SystemOptimizerViewModel_VerificationPending;

        public string VerificationSummary
        {
            get => _verificationSummary;
            private set { _verificationSummary = value; OnPropertyChanged(); }
        }

        public bool HasStateDrift
        {
            get => _hasStateDrift;
            private set { _hasStateDrift = value; OnPropertyChanged(); }
        }

        public string DriftSummaryText => _lastDriftSummary.HasDrift
            ? _lastDriftSummary.OneLinerSummary
            : Strings.SystemOptimizerViewModel_NoDriftSinceLast;

        public string DriftWarningRollup
        {
            get => _driftWarningRollup;
            private set { _driftWarningRollup = value; OnPropertyChanged(); }
        }

        public string LastExportedReportPath
        {
            get => _lastExportedReportPath;
            private set
            {
                _lastExportedReportPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasExportedReport));
                OnPropertyChanged(nameof(CanOpenExportedReport));
                OnPropertyChanged(nameof(CanCopyExportedReportPath));
            }
        }

        public bool HasExportedReport => !string.IsNullOrWhiteSpace(LastExportedReportPath);
        public bool CanOpenExportedReport => SystemOptimizerReportActionGuard.CanOpenReport(LastExportedReportPath);
        public bool CanCopyExportedReportPath => SystemOptimizerReportActionGuard.CanCopyPath(LastExportedReportPath);

        public string OptimizationSummary => 
            string.Format(Strings.SystemOptimizerViewModel_OptimizationsActive, ActiveOptimizationCount, TotalOptimizationCount);

        public string PreflightSummary
        {
            get => _preflightSummary;
            private set { _preflightSummary = value; OnPropertyChanged(); }
        }

        public string PreflightWarningRollup
        {
            get => _preflightWarningRollup;
            private set { _preflightWarningRollup = value; OnPropertyChanged(); }
        }

        public bool PreflightHasHighRisk
        {
            get => _preflightHasHighRisk;
            private set { _preflightHasHighRisk = value; OnPropertyChanged(); OnPropertyChanged(nameof(PreflightRiskBannerText)); }
        }

        public int PreflightHighRiskCount
        {
            get => _preflightHighRiskCount;
            private set { _preflightHighRiskCount = value; OnPropertyChanged(); OnPropertyChanged(nameof(PreflightRiskBannerText)); }
        }

        public string PreflightRiskBannerText => PreflightHasHighRisk
            ? string.Format(Strings.SystemOptimizerViewModel_PreflightHighRiskBanner, PreflightHighRiskCount)
            : Strings.SystemOptimizerViewModel_PreflightNoHighRisk;

        // Optimization Collections
        public ObservableCollection<OptimizationItem> PowerOptimizations { get; }
        public ObservableCollection<OptimizationItem> ServiceOptimizations { get; }
        public ObservableCollection<OptimizationItem> NetworkOptimizations { get; }
        public ObservableCollection<OptimizationItem> InputOptimizations { get; }
        public ObservableCollection<OptimizationItem> VisualOptimizations { get; }
        public ObservableCollection<OptimizationItem> StorageOptimizations { get; }

        // ========== COMMANDS ==========

        public ICommand RefreshCommand { get; }
        public ICommand ApplyGamingMaxCommand { get; }
        public ICommand ApplyBalancedCommand { get; }
        public ICommand RevertAllCommand { get; }
        public ICommand ApplyPowerCommand { get; }
        public ICommand ApplyServicesCommand { get; }
        public ICommand ApplyNetworkCommand { get; }
        public ICommand ApplyInputCommand { get; }
        public ICommand ApplyVisualCommand { get; }
        public ICommand ApplyStorageCommand { get; }
        public ICommand RefreshPreflightCommand { get; }
        public ICommand ExportOptimizationReportCommand { get; }
        public ICommand OpenExportedReportCommand { get; }
        public ICommand CopyExportedReportPathCommand { get; }

        // ========== METHODS ==========

        public async Task RefreshStateAsync(bool showOverlay = true)
        {
            try
            {
                if (showOverlay)
                {
                    IsLoading = true;
                }

                SetStatusAction(showOverlay ? Strings.SystemOptimizerViewModel_StatusScanning : Strings.SystemOptimizerViewModel_StatusRefreshing);
                
                _currentState = await _optimizerService.VerifyStateAsync();
                _expectedStateForDrift = _currentState;
                LastVerifiedAt = _currentState.LastChecked;
                VerificationSummary = Strings.SystemOptimizerViewModel_VerifiedAgainstCurrentState;
                HasStateDrift = false;
                _lastDriftSummary = new OptimizationDriftSummary();
                DriftWarningRollup = Strings.SystemOptimizerViewModel_NoDriftExplanations;
                OnPropertyChanged(nameof(DriftSummaryText));

                await RefreshPreflightAsync(showStatus: false);
                
                UpdateOptimizationCollections();
                
                OnPropertyChanged(nameof(ActiveOptimizationCount));
                OnPropertyChanged(nameof(TotalOptimizationCount));
                OnPropertyChanged(nameof(OptimizationSummary));
                
                SetStatusDone();
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to refresh optimization state: {ex.Message}");
                SetStatusFailed(ex.Message);
            }
            finally
            {
                if (showOverlay)
                {
                    IsLoading = false;
                }
            }
        }

        private void UpdateOptimizationCollections()
        {
            if (_currentState == null) return;
            
            DispatcherHelper.RunOnUiThread(() =>
            {
                // Power
                PowerOptimizations.Clear();
                PowerOptimizations.Add(new OptimizationItem
                {
                    Id = "power_ultimate_perf",
                    Name = Strings.SystemOptimizer_Item_PowerUltimatePerf_Name,
                    Description = Strings.SystemOptimizer_Item_PowerUltimatePerf_Desc,
                    IsEnabled = _currentState.Power.UltimatePerformancePlan,
                    Risk = OptimizationRisk.Low,
                    Category = "Power",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                PowerOptimizations.Add(new OptimizationItem
                {
                    Id = "power_gpu_scheduling",
                    Name = Strings.SystemOptimizer_Item_PowerGpuScheduling_Name,
                    Description = Strings.SystemOptimizer_Item_PowerGpuScheduling_Desc,
                    IsEnabled = _currentState.Power.HardwareGpuScheduling,
                    Risk = OptimizationRisk.Low,
                    Category = "Power",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                PowerOptimizations.Add(new OptimizationItem
                {
                    Id = "power_game_mode",
                    Name = Strings.SystemOptimizer_Item_PowerGameMode_Name,
                    Description = Strings.SystemOptimizer_Item_PowerGameMode_Desc,
                    IsEnabled = _currentState.Power.GameModeEnabled,
                    Risk = OptimizationRisk.Low,
                    Category = "Power",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                PowerOptimizations.Add(new OptimizationItem
                {
                    Id = "power_foreground_priority",
                    Name = Strings.SystemOptimizer_Item_PowerForegroundPriority_Name,
                    Description = Strings.SystemOptimizer_Item_PowerForegroundPriority_Desc,
                    IsEnabled = _currentState.Power.ForegroundPriority,
                    Risk = OptimizationRisk.Low,
                    Category = "Power",
                    OnToggleAsync = ToggleOptimizationAsync
                });

                // Services
                ServiceOptimizations.Clear();
                ServiceOptimizations.Add(new OptimizationItem
                {
                    Id = "service_telemetry",
                    Name = Strings.SystemOptimizer_Item_ServiceTelemetry_Name,
                    Description = Strings.SystemOptimizer_Item_ServiceTelemetry_Desc,
                    IsEnabled = _currentState.Services.TelemetryDisabled,
                    Risk = OptimizationRisk.Medium,
                    Category = "Services",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                ServiceOptimizations.Add(new OptimizationItem
                {
                    Id = "service_sysmain",
                    Name = Strings.SystemOptimizer_Item_ServiceSysmain_Name,
                    Description = Strings.SystemOptimizer_Item_ServiceSysmain_Desc,
                    IsEnabled = _currentState.Services.SysMainDisabled,
                    Risk = OptimizationRisk.Medium,
                    Category = "Services",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                ServiceOptimizations.Add(new OptimizationItem
                {
                    Id = "service_search",
                    Name = Strings.SystemOptimizer_Item_ServiceSearch_Name,
                    Description = Strings.SystemOptimizer_Item_ServiceSearch_Desc,
                    IsEnabled = _currentState.Services.SearchIndexingDisabled,
                    Risk = OptimizationRisk.Medium,
                    Warning = Strings.SystemOptimizer_Item_ServiceSearch_Warning,
                    Category = "Services",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                ServiceOptimizations.Add(new OptimizationItem
                {
                    Id = "service_diagtrack",
                    Name = Strings.SystemOptimizer_Item_ServiceDiagtrack_Name,
                    Description = Strings.SystemOptimizer_Item_ServiceDiagtrack_Desc,
                    IsEnabled = _currentState.Services.DiagTrackDisabled,
                    Risk = OptimizationRisk.Medium,
                    Category = "Services",
                    OnToggleAsync = ToggleOptimizationAsync
                });

                // Network
                NetworkOptimizations.Clear();
                NetworkOptimizations.Add(new OptimizationItem
                {
                    Id = "network_tcp_nodelay",
                    Name = Strings.SystemOptimizer_Item_NetworkTcpNodelay_Name,
                    Description = Strings.SystemOptimizer_Item_NetworkTcpNodelay_Desc,
                    IsEnabled = _currentState.Network.TcpNoDelay,
                    Risk = OptimizationRisk.Low,
                    Category = "Network",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                NetworkOptimizations.Add(new OptimizationItem
                {
                    Id = "network_tcp_ack",
                    Name = Strings.SystemOptimizer_Item_NetworkTcpAck_Name,
                    Description = Strings.SystemOptimizer_Item_NetworkTcpAck_Desc,
                    IsEnabled = _currentState.Network.TcpAckFrequency,
                    Risk = OptimizationRisk.Low,
                    Category = "Network",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                NetworkOptimizations.Add(new OptimizationItem
                {
                    Id = "network_delivery_opt",
                    Name = Strings.SystemOptimizer_Item_NetworkDeliveryOpt_Name,
                    Description = Strings.SystemOptimizer_Item_NetworkDeliveryOpt_Desc,
                    IsEnabled = _currentState.Network.DeliveryOptimizationDisabled,
                    Risk = OptimizationRisk.Low,
                    Category = "Network",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                NetworkOptimizations.Add(new OptimizationItem
                {
                    Id = "network_nagle",
                    Name = Strings.SystemOptimizer_Item_NetworkNagle_Name,
                    Description = Strings.SystemOptimizer_Item_NetworkNagle_Desc,
                    IsEnabled = _currentState.Network.NagleDisabled,
                    Risk = OptimizationRisk.Low,
                    Category = "Network",
                    OnToggleAsync = ToggleOptimizationAsync
                });

                // Input
                InputOptimizations.Clear();
                InputOptimizations.Add(new OptimizationItem
                {
                    Id = "input_mouse_accel",
                    Name = Strings.SystemOptimizer_Item_InputMouseAccel_Name,
                    Description = Strings.SystemOptimizer_Item_InputMouseAccel_Desc,
                    IsEnabled = _currentState.Input.MouseAccelerationDisabled,
                    Risk = OptimizationRisk.Low,
                    Category = "Input",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                InputOptimizations.Add(new OptimizationItem
                {
                    Id = "input_game_dvr",
                    Name = Strings.SystemOptimizer_Item_InputGameDvr_Name,
                    Description = Strings.SystemOptimizer_Item_InputGameDvr_Desc,
                    IsEnabled = _currentState.Input.GameDvrDisabled,
                    Risk = OptimizationRisk.Low,
                    Category = "Input",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                InputOptimizations.Add(new OptimizationItem
                {
                    Id = "input_game_bar",
                    Name = Strings.SystemOptimizer_Item_InputGameBar_Name,
                    Description = Strings.SystemOptimizer_Item_InputGameBar_Desc,
                    IsEnabled = _currentState.Input.GameBarDisabled,
                    Risk = OptimizationRisk.Low,
                    Category = "Input",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                InputOptimizations.Add(new OptimizationItem
                {
                    Id = "input_fullscreen_opt",
                    Name = Strings.SystemOptimizer_Item_InputFullscreenOpt_Name,
                    Description = Strings.SystemOptimizer_Item_InputFullscreenOpt_Desc,
                    IsEnabled = _currentState.Input.FullscreenOptimizationsDisabled,
                    Risk = OptimizationRisk.Low,
                    Category = "Input",
                    OnToggleAsync = ToggleOptimizationAsync
                });

                // Visual
                VisualOptimizations.Clear();
                VisualOptimizations.Add(new OptimizationItem
                {
                    Id = "visual_transparency",
                    Name = Strings.SystemOptimizer_Item_VisualTransparency_Name,
                    Description = Strings.SystemOptimizer_Item_VisualTransparency_Desc,
                    IsEnabled = _currentState.Visual.TransparencyDisabled,
                    Risk = OptimizationRisk.Low,
                    Category = "Visual",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                VisualOptimizations.Add(new OptimizationItem
                {
                    Id = "visual_animations",
                    Name = Strings.SystemOptimizer_Item_VisualAnimations_Name,
                    Description = Strings.SystemOptimizer_Item_VisualAnimations_Desc,
                    IsEnabled = _currentState.Visual.AnimationsDisabled,
                    Risk = OptimizationRisk.Low,
                    Category = "Visual",
                    OnToggleAsync = ToggleOptimizationAsync
                });

                // Storage
                StorageOptimizations.Clear();
                if (_currentState.Storage.IsSsd)
                {
                    StorageOptimizations.Add(new OptimizationItem
                    {
                        Id = "storage_trim",
                        Name = Strings.SystemOptimizer_Item_StorageTrim_Name,
                        Description = Strings.SystemOptimizer_Item_StorageTrim_Desc,
                        IsEnabled = _currentState.Storage.TrimEnabled,
                        Risk = OptimizationRisk.Low,
                        Category = "Storage",
                        OnToggleAsync = ToggleOptimizationAsync
                    });
                }
                StorageOptimizations.Add(new OptimizationItem
                {
                    Id = "storage_last_access",
                    Name = Strings.SystemOptimizer_Item_StorageLastAccess_Name,
                    Description = Strings.SystemOptimizer_Item_StorageLastAccess_Desc,
                    IsEnabled = _currentState.Storage.LastAccessDisabled,
                    Risk = OptimizationRisk.Low,
                    Category = "Storage",
                    OnToggleAsync = ToggleOptimizationAsync
                });
                StorageOptimizations.Add(new OptimizationItem
                {
                    Id = "storage_8dot3",
                    Name = Strings.SystemOptimizer_Item_Storage8dot3_Name,
                    Description = Strings.SystemOptimizer_Item_Storage8dot3_Desc,
                    IsEnabled = _currentState.Storage.ShortNamesDisabled,
                    Risk = OptimizationRisk.Low,
                    Warning = Strings.SystemOptimizer_Item_Storage8dot3_Warning,
                    Category = "Storage",
                    OnToggleAsync = ToggleOptimizationAsync
                });

                ApplyRiskAssessmentMetadata(PowerOptimizations);
                ApplyRiskAssessmentMetadata(ServiceOptimizations);
                ApplyRiskAssessmentMetadata(NetworkOptimizations);
                ApplyRiskAssessmentMetadata(InputOptimizations);
                ApplyRiskAssessmentMetadata(VisualOptimizations);
                ApplyRiskAssessmentMetadata(StorageOptimizations);
                RaiseCategoryCommandStates();
            });
        }

        private async Task ToggleOptimizationAsync(OptimizationItem item, bool desiredState)
        {
            try
            {
                if (item.IsApplying)
                {
                    return;
                }

                item.IsApplying = true;
                SetStatusAction(desiredState ? string.Format(Strings.SystemOptimizerViewModel_StatusApplying, item.Name) : string.Format(Strings.SystemOptimizerViewModel_StatusReverting, item.Name));
                
                OptimizationResult result;
                
                if (desiredState)
                {
                    result = await _optimizerService.ApplyOptimizationAsync(item.Id);
                }
                else
                {
                    result = await _optimizerService.RevertOptimizationAsync(item.Id);
                }
                
                if (result.Success)
                {
                    item.IsEnabled = desiredState;
                    SetStatusDone(string.Format(item.IsEnabled ? Strings.SystemOptimizerViewModel_ItemEnabled : Strings.SystemOptimizerViewModel_ItemDisabled, item.Name));
                    
                    if (result.RequiresReboot)
                    {
                        SetStatusDone(item.IsEnabled ? string.Format(Strings.SystemOptimizerViewModel_ItemEnabledReboot, item.Name) : string.Format(Strings.SystemOptimizerViewModel_ItemDisabled, item.Name));
                    }
                }
                else
                {
                    SetStatusFailed(result.ErrorMessage);
                }
                
                // Refresh authoritative state without blocking the whole page for a single-item toggle.
                await RefreshStateAsync(showOverlay: false);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to toggle optimization: {ex.Message}");
                SetStatusFailed(ex.Message);
            }
            finally
            {
                item.IsApplying = false;
            }
        }

        private async Task ApplyGamingMaxAsync()
        {
            var preflight = await _optimizerService.GeneratePreflightReportAsync(includeHighRisk: true);

            var msgResult = MessageBox.Show(
                string.Format(Strings.SystemOptimizerViewModel_GamingMaxBody,
                    SystemOptimizerPreflightFormatter.BuildSummary(preflight),
                    SystemOptimizerPreflightFormatter.BuildWarningRollup(preflight)),
                Strings.SystemOptimizerViewModel_GamingMaxTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            
            if (msgResult != MessageBoxResult.Yes) return;
            
            try
            {
                IsLoading = true;
                var results = await _optimizerService.ApplyGamingMaximumAsync();
                
                var successCount = results.Count(r => r.Success);
                var failCount = results.Count - successCount;
                var rebootNeeded = results.Any(r => r.RequiresReboot && r.Success);
                
                SetStatusDone(string.Format(Strings.SystemOptimizerViewModel_AppliedCount, successCount, results.Count));
                
                if (failCount > 0)
                {
                    SetStatusDone(string.Format(Strings.SystemOptimizerViewModel_AppliedCountFailed, successCount, results.Count, failCount));
                }
                
                if (rebootNeeded)
                {
                    var reboot = MessageBox.Show(
                        Strings.SystemOptimizerViewModel_RebootPromptBody,
                        Strings.SystemOptimizerViewModel_RebootPromptTitle,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                    
                    if (reboot == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start("shutdown", "/r /t 0");
                    }
                }
                
                await RefreshStateAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to apply gaming max: {ex.Message}");
                SetStatusFailed(ex.Message);
                MessageBox.Show(string.Format(Strings.SystemOptimizerViewModel_ApplyFailedBody, ex.Message),
                    Strings.CommonDialog_Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ApplyCategoryAsync(string categoryName, ObservableCollection<OptimizationItem> optimizations)
        {
            var pendingItems = optimizations.Where(item => !item.IsEnabled).ToList();
            if (!pendingItems.Any())
            {
                SetStatusDone(string.Format(Strings.Optimizer_AlreadyAppliedFormat, categoryName));
                return;
            }

            var confirmation = MessageBox.Show(
                string.Format(Strings.O2_ApplyConfirmBodyFormat, pendingItems.Count, categoryName),
                string.Format(Strings.O2_ApplyConfirmTitleFormat, categoryName),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                IsLoading = true;
                OperationTotalSteps = pendingItems.Count;

                var successCount = 0;
                var failCount = 0;

                for (var index = 0; index < pendingItems.Count; index++)
                {
                    var item = pendingItems[index];
                    OperationCurrentStep = index + 1;
                    OperationProgressLabel = string.Format(Strings.Optimizer_ApplyingItemFormat, item.Name);
                    SetStatusAction(string.Format(Strings.SystemOptimizerViewModel_StatusApplying, item.Name));

                    var result = await _optimizerService.ApplyOptimizationAsync(item.Id);
                    if (result.Success)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }

                if (failCount > 0)
                {
                    SetStatusDone(string.Format(Strings.Optimizer_AppliedCountFormat, successCount, pendingItems.Count, categoryName, failCount));
                }
                else
                {
                    SetStatusDone(string.Format(Strings.Optimizer_AppliedCategoryFormat, successCount, categoryName));
                }

                await RefreshStateAsync(showOverlay: false);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to apply {categoryName}: {ex.Message}");
                SetStatusFailed(ex.Message);
            }
            finally
            {
                ClearOperationProgress();
                IsLoading = false;
            }
        }

        private async Task ApplyBalancedAsync()
        {
            var preflight = await _optimizerService.GeneratePreflightReportAsync(includeHighRisk: false);

            var msgResult = MessageBox.Show(
                string.Format(Strings.SystemOptimizerViewModel_BalancedBody,
                    SystemOptimizerPreflightFormatter.BuildSummary(preflight),
                    SystemOptimizerPreflightFormatter.BuildWarningRollup(preflight)),
                Strings.SystemOptimizerViewModel_BalancedTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);
            
            if (msgResult != MessageBoxResult.Yes) return;
            
            try
            {
                IsLoading = true;
                var results = await _optimizerService.ApplyBalancedAsync();
                
                var successCount = results.Count(r => r.Success);
                SetStatusDone(string.Format(Strings.SystemOptimizerViewModel_AppliedCount, successCount, results.Count));
                
                await RefreshStateAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to apply balanced: {ex.Message}");
                SetStatusFailed(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RevertAllAsync()
        {
            var msgResult = MessageBox.Show(
                Strings.O2_RevertAllBody,
                Strings.O2_RevertAllTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (msgResult != MessageBoxResult.Yes) return;
            
            using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(60));
            try
            {
                IsLoading = true;
                var results = await _optimizerService.RevertAllAsync(cts.Token);
                
                var successCount = results.Count(r => r.Success);
                SetStatusDone(string.Format(Strings.Optimizer_RevertedFormat, successCount, results.Count));
                
                await RefreshStateAsync();
            }
            catch (OperationCanceledException)
            {
                _logger.Error("Revert operation timed out after 60 seconds");
                SetStatusFailed(Strings.O2_RevertTimeout);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to revert: {ex.Message}");
                SetStatusFailed(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetStatusAction(string action)
        {
            StatusMessage = $"{action.Trim().TrimEnd('.')}...";
        }

        private void SetStatusDone(string? details = null)
        {
            StatusMessage = string.IsNullOrWhiteSpace(details) ? Strings.SystemOptimizer_StatusDone : string.Format(Strings.SystemOptimizer_StatusDoneDetails, details);
            ClearOperationProgress();
        }

        private void SetStatusFailed(string? reason)
        {
            StatusMessage = string.Format(Strings.SystemOptimizer_StatusFailedDetails, reason);
            ClearOperationProgress();
        }

        private void HandleServiceStatus(string status)
        {
            UpdateOperationProgressFromStatus(status);
            StatusMessage = LocalizeOptimizerStatus(status);
        }

        /// <summary>
        /// The optimizer service reports progress as canonical English sentences (fixed text, counters,
        /// stage names). Map them here; unknown text passes through so nothing is hidden.
        /// </summary>
        internal static string LocalizeOptimizerStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return status;
            var text = status.Trim();

            var exact = text switch
            {
                "Verifying optimization state..." => Strings.Opt_StatusVerifying,
                "Checking optimization status..." => Strings.Opt_StatusChecking,
                "Creating system restore point..." => Strings.Opt_StatusRestorePoint,
                "Applying power optimizations..." => Strings.Opt_StatusApplyingPower,
                "Optimizing services..." => Strings.Opt_StatusOptimizingServices,
                "Applying network tweaks..." => Strings.Opt_StatusApplyingNetwork,
                "Optimizing input settings..." => Strings.Opt_StatusOptimizingInput,
                "Adjusting visual effects..." => Strings.Opt_StatusAdjustingVisual,
                "Configuring storage..." => Strings.Opt_StatusConfiguringStorage,
                "Creating backup..." => Strings.Opt_StatusCreatingBackup,
                "Applying recommended optimizations..." => Strings.Opt_StatusApplyingRecommended,
                "[3-6/6] Reverting network, input, visual, and storage settings in parallel..." => Strings.Opt_StatusRevertParallel,
                "Administrator privileges are required to apply optimizer changes." => Strings.Opt_ErrorAdminApply,
                "Administrator privileges are required to revert optimizer changes." => Strings.Opt_ErrorAdminRevert,
                _ => null
            };
            if (exact != null) return exact;

            var m = Regex.Match(text, @"^Unknown optimization: (.+)$");
            if (m.Success) return string.Format(Strings.Opt_ErrorUnknownItemFormat, m.Groups[1].Value);

            m = Regex.Match(text, @"^Verification complete: (\d+)/(\d+) optimizations active$");
            if (m.Success) return string.Format(Strings.Opt_StatusVerifyCompleteFormat, m.Groups[1].Value, m.Groups[2].Value);

            m = Regex.Match(text, @"^Status check complete: (\d+)/(\d+) optimizations active$");
            if (m.Success) return string.Format(Strings.Opt_StatusCheckCompleteFormat, m.Groups[1].Value, m.Groups[2].Value);

            m = Regex.Match(text, @"^Complete: (\d+)/(\d+) optimizations applied$");
            if (m.Success) return string.Format(Strings.Opt_StatusCompleteFormat, m.Groups[1].Value, m.Groups[2].Value);

            m = Regex.Match(text, @"^Revert complete: (\d+)/(\d+) settings restored$");
            if (m.Success) return string.Format(Strings.Opt_StatusRevertCompleteFormat, m.Groups[1].Value, m.Groups[2].Value);

            m = Regex.Match(text, @"^\[(\d+)/(\d+)\] Reverting (.+)\.\.\.$");
            if (m.Success)
                return string.Format(Strings.Opt_StatusRevertingFormat, m.Groups[1].Value, m.Groups[2].Value, LocalizeStageName(m.Groups[3].Value));

            m = Regex.Match(text, @"^\[(\d+)/(\d+)\] Failed to revert (.+): (.+)$");
            if (m.Success)
                return string.Format(Strings.Opt_StatusRevertFailedFormat, m.Groups[1].Value, m.Groups[2].Value, LocalizeStageName(m.Groups[3].Value), m.Groups[4].Value);

            return status;
        }

        private static string LocalizeStageName(string stage) => stage.Trim() switch
        {
            "power settings" => Strings.Opt_StagePowerSettings,
            "services" => Strings.Opt_StageServices,
            "network tweaks" => Strings.Opt_StageNetworkTweaks,
            "input settings" => Strings.Opt_StageInputSettings,
            "visual effects" => Strings.Opt_StageVisualEffects,
            "storage settings" => Strings.Opt_StageStorageSettings,
            _ => stage
        };

        private void UpdateOperationProgressFromStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return;
            }

            var match = Regex.Match(status, @"^\[(\d+)(?:-\d+)?\/(\d+)\]\s*(.+)$");
            if (match.Success &&
                int.TryParse(match.Groups[1].Value, out var currentStep) &&
                int.TryParse(match.Groups[2].Value, out var totalSteps))
            {
                OperationCurrentStep = currentStep;
                OperationTotalSteps = totalSteps;
                OperationProgressLabel = match.Groups[3].Value.Trim();
                return;
            }

            if (status.StartsWith("Revert complete", StringComparison.OrdinalIgnoreCase) ||
                status.StartsWith("Failed:", StringComparison.OrdinalIgnoreCase) ||
                status.StartsWith("Done", StringComparison.OrdinalIgnoreCase) ||
                status.StartsWith("Complete:", StringComparison.OrdinalIgnoreCase))
            {
                ClearOperationProgress();
            }
        }

        private void ClearOperationProgress()
        {
            OperationCurrentStep = 0;
            OperationTotalSteps = 0;
            OperationProgressLabel = string.Empty;
            RaiseCategoryCommandStates();
        }

        private async Task RunBackgroundVerificationAsync()
        {
            if (IsLoading || _currentState == null)
            {
                return;
            }

            try
            {
                var expectedState = _currentState;
                var verifiedState = await _optimizerService.VerifyStateAsync();
                LastVerifiedAt = verifiedState.LastChecked;

                var driftSummary = SystemOptimizerService.GetDriftExplanations(expectedState, verifiedState);
                if (!driftSummary.HasDrift)
                {
                    VerificationSummary = Strings.SystemOptimizerViewModel_VerifiedNoDrift;
                    HasStateDrift = false;
                    _lastDriftSummary = driftSummary;
                    DriftWarningRollup = Strings.SystemOptimizerViewModel_NoDriftExplanations;
                    OnPropertyChanged(nameof(DriftSummaryText));
                    return;
                }

                var autoCorrectionResults = await _optimizerService.CorrectMinorDriftAsync(expectedState, verifiedState);
                if (autoCorrectionResults.Any(result => result.Success))
                {
                    verifiedState = await _optimizerService.VerifyStateAsync();
                    LastVerifiedAt = verifiedState.LastChecked;
                    driftSummary = SystemOptimizerService.GetDriftExplanations(expectedState, verifiedState);
                }

                _currentState = verifiedState;
                _expectedStateForDrift = expectedState;
                UpdateOptimizationCollections();
                OnPropertyChanged(nameof(ActiveOptimizationCount));
                OnPropertyChanged(nameof(TotalOptimizationCount));
                OnPropertyChanged(nameof(OptimizationSummary));

                _lastDriftSummary = driftSummary;
                DriftWarningRollup = driftSummary.HasDrift
                    ? string.Join(" | ", driftSummary.DriftedItems.Take(3).Select(d => d.Explanation))
                    : Strings.SystemOptimizerViewModel_NoDriftExplanations;
                OnPropertyChanged(nameof(DriftSummaryText));

                HasStateDrift = driftSummary.HasDrift;
                VerificationSummary = HasStateDrift
                    ? string.Format(Strings.SystemOptimizerViewModel_DriftDetected, string.Join(", ", driftSummary.DriftedItems.Select(d => d.Name).Take(3)))
                    : string.Format(Strings.SystemOptimizerViewModel_VerifiedAutoCorrected, autoCorrectionResults.Count(result => result.Success));

                await RefreshPreflightAsync(showStatus: false);
            }
            catch (Exception ex)
            {
                _logger.Error($"Background optimizer verification failed: {ex.Message}");
                VerificationSummary = Strings.SystemOptimizerViewModel_VerificationFailed;
            }
        }

        private async Task RefreshPreflightAsync(bool showStatus = true)
        {
            try
            {
                if (showStatus)
                {
                    SetStatusAction(Strings.SystemOptimizerViewModel_StatusRefreshingPreflight);
                }

                var report = await _optimizerService.GeneratePreflightReportAsync(includeHighRisk: true);
                PreflightSummary = SystemOptimizerPreflightFormatter.BuildSummary(report);
                PreflightWarningRollup = SystemOptimizerPreflightFormatter.BuildWarningRollup(report);
                PreflightHasHighRisk = report.HasHighRisk;
                PreflightHighRiskCount = report.HighRiskCount;

                if (showStatus)
                {
                    SetStatusDone(Strings.Optimizer_PreflightUpdated);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to refresh preflight: {ex.Message}");
                if (showStatus)
                {
                    SetStatusFailed(string.Format(Strings.Optimizer_PreflightRefreshFailedFormat, ex.Message));
                }
            }
        }

        private async Task ExportOptimizationReportAsync()
        {
            try
            {
                SetStatusAction(Strings.SystemOptimizerViewModel_StatusExportingReport);
                var exportPath = await _optimizerService.ExportOptimizationReportAsync(_expectedStateForDrift);
                if (string.IsNullOrWhiteSpace(exportPath))
                {
                    SetStatusFailed(Strings.Optimizer_ExportReportNoPath);
                    return;
                }

                LastExportedReportPath = exportPath;
                SetStatusDone(string.Format(Strings.Optimizer_ReportExportedFormat, exportPath));
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to export optimization report: {ex.Message}");
                SetStatusFailed(string.Format(Strings.Optimizer_ReportExportFailedFormat, ex.Message));
            }
        }

        private void OpenExportedReport()
        {
            var path = SystemOptimizerReportActionGuard.NormalizePath(LastExportedReportPath);
            if (!SystemOptimizerReportActionGuard.CanOpenReport(path))
            {
                SetStatusFailed(Strings.Optimizer_ReportFileNotFound);
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path)
                {
                    UseShellExecute = true
                });
                SetStatusDone(Strings.Optimizer_ReportOpened);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to open optimization report: {ex.Message}");
                SetStatusFailed(string.Format(Strings.Optimizer_OpenReportFailedFormat, ex.Message));
            }
        }

        private void CopyExportedReportPath()
        {
            var path = SystemOptimizerReportActionGuard.NormalizePath(LastExportedReportPath);
            if (!SystemOptimizerReportActionGuard.CanCopyPath(path))
            {
                SetStatusFailed(Strings.Optimizer_NoReportPathToCopy);
                return;
            }

            try
            {
                Clipboard.SetText(path);
                SetStatusDone(Strings.Optimizer_ReportPathCopied);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to copy report path: {ex.Message}");
                SetStatusFailed(string.Format(Strings.Optimizer_CopyReportPathFailedFormat, ex.Message));
            }
        }

        private bool CanApplyCategory(ObservableCollection<OptimizationItem> optimizations)
        {
            return !IsLoading && optimizations.Any(item => !item.IsEnabled);
        }

        private void RaiseCategoryCommandStates()
        {
            (ApplyPowerCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ApplyServicesCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ApplyNetworkCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ApplyInputCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ApplyVisualCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ApplyStorageCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ExportOptimizationReportCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (OpenExportedReportCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (CopyExportedReportPathCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private static void ApplyRiskAssessmentMetadata(IEnumerable<OptimizationItem> items)
        {
            foreach (var item in items)
            {
                var details = GetRiskAssessment(item.Id);
                item.DetailedDescription = LocalizeOptimizerDetail(details.DetailedDescription);
                item.AffectedRegistryKeys = details.AffectedRegistryKeys;
                item.ServiceNamesAffected = details.ServiceNamesAffected;
                item.UndoInstructions = LocalizeOptimizerUndo(details.UndoInstructions ?? string.Empty);
            }
        }

        // Registry paths and service names stay canonical English (they are technical identifiers);
        // only the human-readable detail and undo text is localized.
        private static string LocalizeOptimizerDetail(string detail) => detail switch
        {
            "Switches the active Windows power plan to the highest-performance profile available so CPU and GPU boost behavior stays aggressive during gameplay." => Strings.OptimizeDetail_PowerUltimatePerf,
            "Enables Hardware-Accelerated GPU Scheduling so Windows can submit GPU work with lower CPU overhead on supported systems." => Strings.OptimizeDetail_PowerGpuScheduling,
            "Turns on Windows Game Mode so the OS prioritizes the active game and reduces background interference." => Strings.OptimizeDetail_PowerGameMode,
            "Raises foreground scheduling priority so the active game receives more CPU time relative to background tasks." => Strings.OptimizeDetail_PowerForegroundPriority,
            "Disables Windows telemetry policy values that control how much diagnostic data the OS is allowed to collect." => Strings.OptimizeDetail_ServiceTelemetry,
            "Stops and disables SysMain so Windows stops preloading applications in the background, which is often preferable on SSD systems." => Strings.OptimizeDetail_ServiceSysmain,
            "Stops and disables Windows Search indexing to reduce background disk and CPU usage." => Strings.OptimizeDetail_ServiceSearch,
            "Disables the Connected User Experiences and Telemetry service so background telemetry processing is reduced." => Strings.OptimizeDetail_ServiceDiagtrack,
            "Forces TCP to send packets without delayed acknowledgments, reducing latency at the cost of more frequent network traffic." => Strings.OptimizeDetail_NetworkTcpNodelay,
            "Changes TCP acknowledgment behavior so packets are acknowledged immediately instead of being batched." => Strings.OptimizeDetail_NetworkTcpAck,
            "Disables Delivery Optimization peer-to-peer update sharing so Windows Update no longer uploads update data to other machines." => Strings.OptimizeDetail_NetworkDeliveryOpt,
            "Adjusts TCP buffering behavior to minimize acknowledgment delay for latency-sensitive workloads." => Strings.OptimizeDetail_NetworkNagle,
            "Disables Enhanced Pointer Precision so mouse movement stays raw and 1:1, which is generally preferred for games." => Strings.OptimizeDetail_InputMouseAccel,
            "Disables background game recording so Game DVR stops reserving resources while games are running." => Strings.OptimizeDetail_InputGameDvr,
            "Disables Xbox Game Bar overlays and startup prompts so they do not interrupt gameplay." => Strings.OptimizeDetail_InputGameBar,
            "Tunes fullscreen optimization behavior for games by adjusting Windows GameConfigStore flags." => Strings.OptimizeDetail_InputFullscreenOpt,
            "Disables acrylic and transparency effects to reduce compositing overhead and visual distractions." => Strings.OptimizeDetail_VisualTransparency,
            "Turns off common shell animations and menu delays so the desktop feels snappier and uses fewer visual effects." => Strings.OptimizeDetail_VisualAnimations,
            "Enables the NTFS TRIM path so SSDs can reclaim deleted blocks efficiently and maintain performance over time." => Strings.OptimizeDetail_StorageTrim,
            "Disables NTFS last-access timestamp updates to reduce metadata writes on storage devices." => Strings.OptimizeDetail_StorageLastAccess,
            "Disables legacy 8.3 short filename generation, which can slightly reduce file-system bookkeeping on modern systems." => Strings.OptimizeDetail_Storage8dot3,
            _ => detail
        };

        private static string LocalizeOptimizerUndo(string undo) => undo switch
        {
            "Restore the Balanced plan with: powercfg /setactive 381b4222-f694-41f0-9685-ff5bb260df2e" => Strings.OptimizeUndo_PowerUltimatePerf,
            "Set HwSchMode back to 0 or remove it, then reboot Windows." => Strings.OptimizeUndo_PowerGpuScheduling,
            "Set AutoGameModeEnabled to 0 or remove the value." => Strings.OptimizeUndo_PowerGameMode,
            "Restore the default Win32PrioritySeparation value or remove the override." => Strings.OptimizeUndo_PowerForegroundPriority,
            "Remove the AllowTelemetry overrides to return to the Windows default policy." => Strings.OptimizeUndo_ServiceTelemetry,
            "Set the SysMain service back to Automatic and start it from Services." => Strings.OptimizeUndo_ServiceSysmain,
            "Set the Windows Search service back to Automatic and start it from Services." => Strings.OptimizeUndo_ServiceSearch,
            "Set the DiagTrack service back to Automatic and start it from Services." => Strings.OptimizeUndo_ServiceDiagtrack,
            "Remove TcpNoDelay or set it back to 0." => Strings.OptimizeUndo_NetworkTcpNodelay,
            "Remove TcpAckFrequency or restore the default behavior." => Strings.OptimizeUndo_NetworkTcpAck,
            "Remove DODownloadMode to let Windows manage Delivery Optimization normally." => Strings.OptimizeUndo_NetworkDeliveryOpt,
            "Remove TCPDelAckTicks to restore the default stack behavior." => Strings.OptimizeUndo_NetworkNagle,
            "Set MouseSpeed back to 1 and restore threshold values, or re-enable pointer precision in Windows mouse settings." => Strings.OptimizeUndo_InputMouseAccel,
            "Remove the Game DVR policy values or set them back to enabled." => Strings.OptimizeUndo_InputGameDvr,
            "Remove those Game Bar values or re-enable Xbox Game Bar in Windows Settings." => Strings.OptimizeUndo_InputGameBar,
            "Remove the GameConfigStore fullscreen values to restore Windows defaults." => Strings.OptimizeUndo_InputFullscreenOpt,
            "Set EnableTransparency back to 1 or re-enable Transparency Effects in Personalization settings." => Strings.OptimizeUndo_VisualTransparency,
            "Restore MinAnimate, MenuShowDelay, and SmoothScroll to their default values if you want the stock Windows UI feel back." => Strings.OptimizeUndo_VisualAnimations,
            "Run fsutil behavior set DisableDeleteNotify 1 to turn the override back off." => Strings.OptimizeUndo_StorageTrim,
            "Run fsutil behavior set disablelastaccess 0 to restore timestamp updates." => Strings.OptimizeUndo_StorageLastAccess,
            "Set NtfsDisable8dot3NameCreation back to 0 if older software depends on DOS-style short names." => Strings.OptimizeUndo_Storage8dot3,
            _ => undo
        };

        private static OptimizationRiskAssessment GetRiskAssessment(string optimizationId)
        {
            return optimizationId switch
            {
                "power_ultimate_perf" => new OptimizationRiskAssessment(
                    "Switches the active Windows power plan to the highest-performance profile available so CPU and GPU boost behavior stays aggressive during gameplay.",
                    Array.Empty<string>(),
                    new[] { "powercfg /setactive e9a42b02-d5df-448d-aa00-03f14749eb61", "Fallback: powercfg /setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c" },
                    "Restore the Balanced plan with: powercfg /setactive 381b4222-f694-41f0-9685-ff5bb260df2e"),
                "power_gpu_scheduling" => new OptimizationRiskAssessment(
                    "Enables Hardware-Accelerated GPU Scheduling so Windows can submit GPU work with lower CPU overhead on supported systems.",
                    new[] { @"HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers\HwSchMode" },
                    Array.Empty<string>(),
                    "Set HwSchMode back to 0 or remove it, then reboot Windows."),
                "power_game_mode" => new OptimizationRiskAssessment(
                    "Turns on Windows Game Mode so the OS prioritizes the active game and reduces background interference.",
                    new[] { @"HKCU\Software\Microsoft\GameBar\AutoGameModeEnabled" },
                    Array.Empty<string>(),
                    "Set AutoGameModeEnabled to 0 or remove the value."),
                "power_foreground_priority" => new OptimizationRiskAssessment(
                    "Raises foreground scheduling priority so the active game receives more CPU time relative to background tasks.",
                    new[] { @"HKLM\SYSTEM\CurrentControlSet\Control\PriorityControl\Win32PrioritySeparation" },
                    Array.Empty<string>(),
                    "Restore the default Win32PrioritySeparation value or remove the override."),
                "service_telemetry" => new OptimizationRiskAssessment(
                    "Disables Windows telemetry policy values that control how much diagnostic data the OS is allowed to collect.",
                    new[]
                    {
                        @"HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection\AllowTelemetry",
                        @"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection\AllowTelemetry"
                    },
                    Array.Empty<string>(),
                    "Remove the AllowTelemetry overrides to return to the Windows default policy."),
                "service_sysmain" => new OptimizationRiskAssessment(
                    "Stops and disables SysMain so Windows stops preloading applications in the background, which is often preferable on SSD systems.",
                    Array.Empty<string>(),
                    new[] { "SysMain service" },
                    "Set the SysMain service back to Automatic and start it from Services."),
                "service_search" => new OptimizationRiskAssessment(
                    "Stops and disables Windows Search indexing to reduce background disk and CPU usage.",
                    Array.Empty<string>(),
                    new[] { "WSearch service" },
                    "Set the Windows Search service back to Automatic and start it from Services."),
                "service_diagtrack" => new OptimizationRiskAssessment(
                    "Disables the Connected User Experiences and Telemetry service so background telemetry processing is reduced.",
                    Array.Empty<string>(),
                    new[] { "DiagTrack service" },
                    "Set the DiagTrack service back to Automatic and start it from Services."),
                "network_tcp_nodelay" => new OptimizationRiskAssessment(
                    "Forces TCP to send packets without delayed acknowledgments, reducing latency at the cost of more frequent network traffic.",
                    new[] { @"HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\TcpNoDelay" },
                    Array.Empty<string>(),
                    "Remove TcpNoDelay or set it back to 0."),
                "network_tcp_ack" => new OptimizationRiskAssessment(
                    "Changes TCP acknowledgment behavior so packets are acknowledged immediately instead of being batched.",
                    new[] { @"HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\TcpAckFrequency" },
                    Array.Empty<string>(),
                    "Remove TcpAckFrequency or restore the default behavior."),
                "network_delivery_opt" => new OptimizationRiskAssessment(
                    "Disables Delivery Optimization peer-to-peer update sharing so Windows Update no longer uploads update data to other machines.",
                    new[] { @"HKLM\SOFTWARE\Policies\Microsoft\Windows\DeliveryOptimization\DODownloadMode" },
                    Array.Empty<string>(),
                    "Remove DODownloadMode to let Windows manage Delivery Optimization normally."),
                "network_nagle" => new OptimizationRiskAssessment(
                    "Adjusts TCP buffering behavior to minimize acknowledgment delay for latency-sensitive workloads.",
                    new[] { @"HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\TCPDelAckTicks" },
                    Array.Empty<string>(),
                    "Remove TCPDelAckTicks to restore the default stack behavior."),
                "input_mouse_accel" => new OptimizationRiskAssessment(
                    "Disables Enhanced Pointer Precision so mouse movement stays raw and 1:1, which is generally preferred for games.",
                    new[]
                    {
                        @"HKCU\Control Panel\Mouse\MouseSpeed",
                        @"HKCU\Control Panel\Mouse\MouseThreshold1",
                        @"HKCU\Control Panel\Mouse\MouseThreshold2"
                    },
                    Array.Empty<string>(),
                    "Set MouseSpeed back to 1 and restore threshold values, or re-enable pointer precision in Windows mouse settings."),
                "input_game_dvr" => new OptimizationRiskAssessment(
                    "Disables background game recording so Game DVR stops reserving resources while games are running.",
                    new[]
                    {
                        @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR\AppCaptureEnabled",
                        @"HKCU\System\GameConfigStore\GameDVR_Enabled",
                        @"HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR\AllowGameDVR"
                    },
                    Array.Empty<string>(),
                    "Remove the Game DVR policy values or set them back to enabled."),
                "input_game_bar" => new OptimizationRiskAssessment(
                    "Disables Xbox Game Bar overlays and startup prompts so they do not interrupt gameplay.",
                    new[]
                    {
                        @"HKCU\SOFTWARE\Microsoft\GameBar\UseNexusForGameBarEnabled",
                        @"HKCU\SOFTWARE\Microsoft\GameBar\ShowStartupPanel"
                    },
                    Array.Empty<string>(),
                    "Remove those Game Bar values or re-enable Xbox Game Bar in Windows Settings."),
                "input_fullscreen_opt" => new OptimizationRiskAssessment(
                    "Tunes fullscreen optimization behavior for games by adjusting Windows GameConfigStore flags.",
                    new[]
                    {
                        @"HKCU\System\GameConfigStore\GameDVR_FSEBehaviorMode",
                        @"HKCU\System\GameConfigStore\GameDVR_HonorUserFSEBehaviorMode",
                        @"HKCU\System\GameConfigStore\GameDVR_FSEBehavior"
                    },
                    Array.Empty<string>(),
                    "Remove the GameConfigStore fullscreen values to restore Windows defaults."),
                "visual_transparency" => new OptimizationRiskAssessment(
                    "Disables acrylic and transparency effects to reduce compositing overhead and visual distractions.",
                    new[] { @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize\EnableTransparency" },
                    Array.Empty<string>(),
                    "Set EnableTransparency back to 1 or re-enable Transparency Effects in Personalization settings."),
                "visual_animations" => new OptimizationRiskAssessment(
                    "Turns off common shell animations and menu delays so the desktop feels snappier and uses fewer visual effects.",
                    new[]
                    {
                        @"HKCU\Control Panel\Desktop\WindowMetrics\MinAnimate",
                        @"HKCU\Control Panel\Desktop\MenuShowDelay",
                        @"HKCU\Control Panel\Desktop\SmoothScroll"
                    },
                    Array.Empty<string>(),
                    "Restore MinAnimate, MenuShowDelay, and SmoothScroll to their default values if you want the stock Windows UI feel back."),
                "storage_trim" => new OptimizationRiskAssessment(
                    "Enables the NTFS TRIM path so SSDs can reclaim deleted blocks efficiently and maintain performance over time.",
                    Array.Empty<string>(),
                    new[] { "fsutil behavior set DisableDeleteNotify 0" },
                    "Run fsutil behavior set DisableDeleteNotify 1 to turn the override back off."),
                "storage_last_access" => new OptimizationRiskAssessment(
                    "Disables NTFS last-access timestamp updates to reduce metadata writes on storage devices.",
                    Array.Empty<string>(),
                    new[] { "fsutil behavior set disablelastaccess 1" },
                    "Run fsutil behavior set disablelastaccess 0 to restore timestamp updates."),
                "storage_8dot3" => new OptimizationRiskAssessment(
                    "Disables legacy 8.3 short filename generation, which can slightly reduce file-system bookkeeping on modern systems.",
                    new[] { @"HKLM\SYSTEM\CurrentControlSet\Control\FileSystem\NtfsDisable8dot3NameCreation" },
                    Array.Empty<string>(),
                    "Set NtfsDisable8dot3NameCreation back to 0 if older software depends on DOS-style short names."),
                _ => new OptimizationRiskAssessment(
                    "This optimization changes Windows settings that OmenCore can also revert later through the built-in restore flow.",
                    Array.Empty<string>(),
                    Array.Empty<string>(),
                    null)
            };
        }
    }

    /// <summary>
    /// Represents a single optimization toggle item in the UI.
    /// </summary>
    public class OptimizationItem : INotifyPropertyChanged
    {
        private bool _isEnabled;
        private bool _isApplying;
        private bool _isDetailsExpanded;

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public OptimizationRisk Risk { get; set; }
        public string? Warning { get; set; }
        public string? DetailedDescription { get; set; }
        public IReadOnlyList<string> AffectedRegistryKeys { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> ServiceNamesAffected { get; set; } = Array.Empty<string>();
        public string? UndoInstructions { get; set; }
        
        public Func<OptimizationItem, bool, Task>? OnToggleAsync { get; set; }

        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(); }
        }

        public bool IsApplying
        {
            get => _isApplying;
            set { _isApplying = value; OnPropertyChanged(); }
        }

        public bool IsDetailsExpanded
        {
            get => _isDetailsExpanded;
            set { _isDetailsExpanded = value; OnPropertyChanged(); }
        }

        public string RiskText => Risk switch
        {
            OptimizationRisk.Low => Strings.SystemOptimizer_RiskLow,
            OptimizationRisk.Medium => Strings.SystemOptimizer_RiskMedium,
            OptimizationRisk.High => Strings.SystemOptimizer_RiskHigh,
            _ => Strings.SystemOptimizer_RiskUnknown
        };

        public string RiskColor => Risk switch
        {
            OptimizationRisk.Low => "#00C8C8",
            OptimizationRisk.Medium => "#FFB800",
            OptimizationRisk.High => "#FF005C",
            _ => "#808080"
        };

        public bool HasWarning => !string.IsNullOrEmpty(Warning);
        public bool HasDetailedDescription => !string.IsNullOrWhiteSpace(DetailedDescription);
        public bool HasRegistryPreview => AffectedRegistryKeys.Count > 0;
        public bool HasServicePreview => ServiceNamesAffected.Count > 0;
        public bool HasUndoInstructions => !string.IsNullOrWhiteSpace(UndoInstructions);
        public bool HasDetails => HasDetailedDescription || HasRegistryPreview || HasServicePreview || HasUndoInstructions;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task Toggle(bool desiredState)
        {
            try
            {
                if (OnToggleAsync != null)
                {
                    await OnToggleAsync(this, desiredState);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Toggle failed: {ex.Message}");
            }
        }
    }

    public sealed record OptimizationRiskAssessment(
        string DetailedDescription,
        IReadOnlyList<string> AffectedRegistryKeys,
        IReadOnlyList<string> ServiceNamesAffected,
        string? UndoInstructions);
}
