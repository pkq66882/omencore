using System;
using OmenCore.Localization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OmenCore.Services;
using OmenCore.Services.BloatwareManager;
using OmenCore.Utils;

namespace OmenCore.ViewModels
{
    /// <summary>
    /// ViewModel for the Bloatware Manager view.
    /// Provides UI bindings for scanning, removing, and restoring bloatware.
    /// </summary>
    public class BloatwareManagerViewModel : INotifyPropertyChanged
    {
        private readonly BloatwareManagerService _service;
        private readonly LoggingService _logger;
        private bool _isScanning;
        private bool _isProcessing;
        private string _statusMessage = Strings.BloatwareManagerViewModel_InitialStatus;
        private string _filterText = "";
        private BloatwareCategory? _selectedCategory;
        private BloatwareApp? _selectedApp;
        private string _riskFilter = "All";
        private int _bulkRemoveProgress;
        private int _bulkRemoveTotal;
        private bool _isBulkRemoving;
        private int _bulkRestoreProgress;
        private int _bulkRestoreTotal;
        private bool _isBulkRestoring;
        private bool _hasRemovalResults;
        private CancellationTokenSource? _bulkOperationCancellation;
        private string? _lastReportPath;
        private bool _isRemovalPreviewVisible;
        private string _removalPreviewTitle = Strings.Bloatware_RemovalPreviewTitle;
        private string _removalPreviewSummary = "";
        private int _removalPreviewEstimatedSeconds;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<BloatwareApp> AllApps { get; } = new();
        public ObservableCollection<BloatwareApp> FilteredApps { get; } = new();
        public ObservableCollection<BloatwareCategory> Categories { get; } = new();
        public ObservableCollection<BloatwareRemovalPreviewItem> RemovalPreviewItems { get; } = new();

        public ICommand ScanCommand { get; }
        public ICommand RemoveSelectedCommand { get; }
        public ICommand RemoveAllLowRiskCommand { get; }
        public ICommand RestoreSelectedCommand { get; }
        public ICommand RestoreAllRemovedCommand { get; }
        public ICommand ExportDryRunReportCommand { get; }
        public ICommand ExportResultLogCommand { get; }
        public ICommand CancelBulkOperationCommand { get; }
        public ICommand ViewLastReportCommand { get; }
        public ICommand ConfirmRemovalPreviewCommand { get; }
        public ICommand CancelRemovalPreviewCommand { get; }
        public ICommand ToggleAllPreviewItemsCommand { get; }

        public ICommand ApplyTelemetryOnlyPresetCommand { get; }
        public ICommand ApplyOmenConflictReducerPresetCommand { get; }
        public ICommand ApplyWindowsConsumerAppsPresetCommand { get; }
        public ICommand ApplyFullStandalonePresetCommand { get; }

        public bool IsScanning
        {
            get => _isScanning;
            set { _isScanning = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanInteract)); RaiseCommandStates(); }
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set { _isProcessing = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanInteract)); RaiseCommandStates(); }
        }

        public bool CanInteract => !IsScanning && !IsProcessing;

        public bool IsRemovalPreviewVisible
        {
            get => _isRemovalPreviewVisible;
            private set
            {
                _isRemovalPreviewVisible = value;
                OnPropertyChanged();
                RaiseCommandStates();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public string FilterText
        {
            get => _filterText;
            set { _filterText = value; OnPropertyChanged(); ApplyFilter(); }
        }

        public BloatwareCategory? SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); ApplyFilter(); }
        }

        public BloatwareApp? SelectedApp
        {
            get => _selectedApp;
            set
            {
                _selectedApp = value;
                OnPropertyChanged();
                // Must raise CanExecuteChanged on the commands (not just PropertyChanged on
                // the computed bool) so WPF re-evaluates button IsEnabled via ICommand.CanExecute.
                RaiseCommandStates();
            }
        }

        public bool CanRemoveSelected => SelectedApp != null && !SelectedApp.IsRemoved && CanInteract;
        public bool CanRestoreSelected => SelectedApp != null && SelectedApp.IsRemoved && SelectedApp.CanRestore && CanInteract;
        public bool CanExportDryRun => AllApps.Count > 0 && CanInteract;

        public int TotalCount => AllApps.Count;
        public int RemovedCount => AllApps.Count(a => a.IsRemoved);
        public int LowRiskCount => AllApps.Count(a => a.RemovalRisk == RemovalRisk.Low && !a.IsRemoved);

        public string RiskFilter
        {
            get => _riskFilter;
            set
            {
                _riskFilter = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRiskAll));
                OnPropertyChanged(nameof(IsRiskLow));
                OnPropertyChanged(nameof(IsRiskMedium));
                OnPropertyChanged(nameof(IsRiskHigh));
                ApplyFilter();
            }
        }

        public bool IsRiskAll    { get => _riskFilter == "All";    set { if (value) RiskFilter = "All"; } }
        public bool IsRiskLow    { get => _riskFilter == "Low";    set { if (value) RiskFilter = "Low"; } }
        public bool IsRiskMedium { get => _riskFilter == "Medium"; set { if (value) RiskFilter = "Medium"; } }
        public bool IsRiskHigh   { get => _riskFilter == "High";   set { if (value) RiskFilter = "High"; } }

        public bool IsBulkRemoving
        {
            get => _isBulkRemoving;
            set { _isBulkRemoving = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanCancelBulkOperation)); RaiseCommandStates(); }
        }

        public int BulkRemoveProgress
        {
            get => _bulkRemoveProgress;
            set { _bulkRemoveProgress = value; OnPropertyChanged(); }
        }

        public int BulkRemoveTotal
        {
            get => _bulkRemoveTotal;
            set { _bulkRemoveTotal = value; OnPropertyChanged(); }
        }

        public bool IsBulkRestoring
        {
            get => _isBulkRestoring;
            set { _isBulkRestoring = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanCancelBulkOperation)); RaiseCommandStates(); }
        }

        public int BulkRestoreProgress
        {
            get => _bulkRestoreProgress;
            set { _bulkRestoreProgress = value; OnPropertyChanged(); }
        }

        public int BulkRestoreTotal
        {
            get => _bulkRestoreTotal;
            set { _bulkRestoreTotal = value; OnPropertyChanged(); }
        }

        /// <summary>True after any bulk removal has completed, enabling the Export Log button.</summary>
        public bool HasRemovalResults
        {
            get => _hasRemovalResults;
            set { _hasRemovalResults = value; OnPropertyChanged(); }
        }

        public string? LastReportPath
        {
            get => _lastReportPath;
            private set
            {
                _lastReportPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasLastReport));
                RaiseCommandStates();
            }
        }

        public bool HasLastReport => !string.IsNullOrWhiteSpace(LastReportPath) && File.Exists(LastReportPath);

        public string RemovalPreviewTitle
        {
            get => _removalPreviewTitle;
            private set { _removalPreviewTitle = value; OnPropertyChanged(); }
        }

        public string RemovalPreviewSummary
        {
            get => _removalPreviewSummary;
            private set { _removalPreviewSummary = value; OnPropertyChanged(); }
        }

        public string RemovalPreviewEstimatedText => _removalPreviewEstimatedSeconds <= 0
            ? Strings.BloatwareManagerViewModel_EstimatedUnderMin
            : string.Format(Strings.BloatwareManagerViewModel_EstimatedAboutFormat, _removalPreviewEstimatedSeconds / 60 + 1);

        public bool HasRemovalPreviewSelection => RemovalPreviewItems.Any(item => item.IsSelected);

        public bool CanCancelBulkOperation => IsBulkRemoving || IsBulkRestoring;

        public string BulkRemoveProgressText => BulkRemoveTotal > 0
            ? string.Format(Strings.BloatwareManagerViewModel_ProgressDoneFormat, BulkRemoveProgress, BulkRemoveTotal)
            : Strings.BloatwareManagerViewModel_Waiting;

        public string BulkRestoreProgressText => BulkRestoreTotal > 0
            ? string.Format(Strings.BloatwareManagerViewModel_ProgressDoneFormat, BulkRestoreProgress, BulkRestoreTotal)
            : Strings.BloatwareManagerViewModel_Waiting;

        public BloatwareManagerViewModel(LoggingService logger)
        {
            _logger = logger;
            _service = new BloatwareManagerService(logger);

            // Initialize commands
            ScanCommand = new RelayCommand(async _ => await ScanAsync(), _ => CanInteract);
            RemoveSelectedCommand = new RelayCommand(async _ => await PrepareSelectedRemovalPreviewAsync(), _ => CanRemoveSelected);
            RemoveAllLowRiskCommand = new RelayCommand(async _ => await PrepareLowRiskRemovalPreviewAsync(), _ => LowRiskCount > 0 && CanInteract);
            RestoreSelectedCommand = new RelayCommand(async _ => await RestoreSelectedAsync(), _ => CanRestoreSelected);
            RestoreAllRemovedCommand = new RelayCommand(async _ => await RestoreAllRemovedAsync(), _ => RemovedCount > 0 && CanInteract);
            ExportDryRunReportCommand = new RelayCommand(_ => ExportDryRunReport(), _ => CanExportDryRun);
            ExportResultLogCommand = new RelayCommand(_ => ExportResultLog(), _ => HasRemovalResults);
            CancelBulkOperationCommand = new RelayCommand(_ => CancelBulkOperation(), _ => CanCancelBulkOperation);
            ViewLastReportCommand = new RelayCommand(_ => ViewLastReport(), _ => HasLastReport);
            ConfirmRemovalPreviewCommand = new RelayCommand(async _ => await ConfirmRemovalPreviewAsync(), _ => IsRemovalPreviewVisible && HasRemovalPreviewSelection && CanInteract);
            CancelRemovalPreviewCommand = new RelayCommand(_ => CancelRemovalPreview(), _ => IsRemovalPreviewVisible && CanInteract);
            ToggleAllPreviewItemsCommand = new RelayCommand(_ => ToggleAllPreviewItems(), _ => IsRemovalPreviewVisible && RemovalPreviewItems.Count > 0 && CanInteract);

            // Initialize preset commands
            ApplyTelemetryOnlyPresetCommand = new RelayCommand(_ => ApplyTelemetryOnlyPreset(), _ => AllApps.Count > 0 && CanInteract);
            ApplyOmenConflictReducerPresetCommand = new RelayCommand(_ => ApplyOmenConflictReducerPreset(), _ => AllApps.Count > 0 && CanInteract);
            ApplyWindowsConsumerAppsPresetCommand = new RelayCommand(_ => ApplyWindowsConsumerAppsPreset(), _ => AllApps.Count > 0 && CanInteract);
            ApplyFullStandalonePresetCommand = new RelayCommand(_ => ApplyFullStandalonePreset(), _ => AllApps.Count > 0 && CanInteract);

            // Initialize categories
            foreach (var cat in Enum.GetValues<BloatwareCategory>().Where(c => c != BloatwareCategory.Unknown))
            {
                Categories.Add(cat);
            }

            // Subscribe to service events (Core-side status strings are mapped to the catalogs)
            _service.StatusChanged += status => DispatcherHelper.RunOnUiThread(() =>
            {
                var localized = status switch
                {
                    "Scanning for bloatware..." => Strings.BloatwareManagerViewModel_Scanning,
                    _ => status
                };
                StatusMessage = localized;
            });
            _service.AppRemoved += app => DispatcherHelper.RunOnUiThread(UpdateCounts);
            _service.AppRestored += app => DispatcherHelper.RunOnUiThread(UpdateCounts);

            // Warn immediately if not running as admin
            if (!BloatwareManagerService.IsRunningAsAdmin)
            {
                SetStatusFailed(Strings.BloatwareManagerViewModel_NotAdminWarn);
            }
        }

        public async Task ScanAsync()
        {
            if (IsScanning) return;

            try
            {
                IsScanning = true;
                SetStatusAction(Strings.BloatwareManagerViewModel_Scanning);
                AllApps.Clear();
                FilteredApps.Clear();

                var apps = await _service.ScanForBloatwareAsync();

                foreach (var app in apps)
                {
                    app.Description = LocalizeBloatDescription(app.Description);
                    AllApps.Add(app);
                }

                ApplyFilter();
                UpdateCounts();
                SetStatusDone(string.Format(Strings.BloatwareManagerViewModel_ScanCompletedFormat, AllApps.Count));
            }
            catch (Exception ex)
            {
                _logger.Error($"Bloatware scan failed: {ex.Message}");
                SetStatusFailed(string.Format(Strings.BloatwareManagerViewModel_ScanFailedFormat, ex.Message));
            }
            finally
            {
                IsScanning = false;
            }
        }

        private async Task PrepareSelectedRemovalPreviewAsync()
        {
            if (!BloatwareManagerService.IsRunningAsAdmin)
            {
                SetStatusFailed(Strings.BloatwareManagerViewModel_CannotRemoveNotAdmin);
                return;
            }

            if (SelectedApp == null || IsProcessing)
            {
                return;
            }

            BuildRemovalPreview(Strings.BloatwareManagerViewModel_RemoveSelectedItem, new List<BloatwareApp> { SelectedApp });
            await Task.CompletedTask;
        }

        private async Task ExecuteSingleRemovalAsync(BloatwareApp app)
        {
            try
            {
                IsProcessing = true;
                SetStatusAction(string.Format(Strings.BloatwareManagerViewModel_RemovingFormat, app.Name));
                await _service.RemoveAppAsync(app);
                HasRemovalResults = true;
                UpdateCounts();
                ApplyFilter();
                var outcomeMessage = BuildSingleRemovalOutcomeMessage(app);
                if (app.LastRemovalStatus == RemovalStatus.Failed)
                {
                    SetStatusFailed(outcomeMessage);
                }
                else
                {
                    SetStatusDone(outcomeMessage);
                }

                GenerateAndStoreSessionReport();
            }
            finally
            {
                IsProcessing = false;
                OnPropertyChanged(nameof(CanRemoveSelected));
                OnPropertyChanged(nameof(CanRestoreSelected));
            }
        }

        private async Task PrepareLowRiskRemovalPreviewAsync()
        {
            if (!BloatwareManagerService.IsRunningAsAdmin)
            {
                SetStatusFailed(Strings.BloatwareManagerViewModel_CannotRemoveNotAdmin);
                return;
            }

            if (IsProcessing) return;

            var lowRiskApps = AllApps.Where(a => a.RemovalRisk == RemovalRisk.Low && !a.IsRemoved).ToList();
            if (!lowRiskApps.Any())
            {
                SetStatusDone(Strings.BloatwareManagerViewModel_NoLowRisk);
                return;
            }

            BuildRemovalPreview(Strings.BloatwareManagerViewModel_RemoveLowRisk, lowRiskApps);
            await Task.CompletedTask;
        }

        private async Task ExecuteBulkRemovalAsync(List<BloatwareApp> appsToRemove)
        {
            try
            {
                IsProcessing = true;
                IsBulkRemoving = true;
                BulkRemoveTotal = appsToRemove.Count;
                BulkRemoveProgress = 0;
                OnPropertyChanged(nameof(BulkRemoveProgressText));
                using var cancellation = BeginBulkOperation();
                var bulkResult = await _service.RemoveAppsWithRollbackAsync(
                    appsToRemove,
                    (index, total, app) => Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
                    {
                        BulkRemoveProgress = index;
                        OnPropertyChanged(nameof(BulkRemoveProgressText));
                        SetStatusAction(string.Format(Strings.BloatwareManagerViewModel_RemovingProgressFormat, index, total, app.Name));
                    })),
                    cancellation.Token);

                if (bulkResult.Canceled)
                {
                    var canceledAt = bulkResult.CanceledAt?.Name ?? Strings.BloatwareManagerViewModel_TheCurrentItem;
                    SetStatusFailed(
                        string.Format(Strings.BloatwareManagerViewModel_BulkCanceledFormat, canceledAt, bulkResult.Succeeded.Count, bulkResult.Skipped.Count));
                }
                else if (bulkResult.Completed)
                {
                    SetStatusDone(BuildBulkRemovalCompletionMessage(bulkResult));
                }
                else
                {
                    var failedName = bulkResult.FailedAt?.Name ?? Strings.BloatwareManagerViewModel_UnknownItem;
                    var failedDetail = string.IsNullOrWhiteSpace(bulkResult.FailedAt?.LastRemovalDetail)
                        ? string.IsNullOrWhiteSpace(bulkResult.FailedAt?.LastFailureReason)
                            ? Strings.BloatwareManagerViewModel_NoFailureDetail
                            : bulkResult.FailedAt!.LastFailureReason!
                        : bulkResult.FailedAt!.LastRemovalDetail!;
                    SetStatusFailed(
                        string.Format(Strings.BloatwareManagerViewModel_BulkStoppedFormat, failedName, failedDetail,
                            bulkResult.RollbackSucceeded.Count, bulkResult.RollbackFailed.Count, bulkResult.RollbackSkipped.Count));
                }

                HasRemovalResults = true;
                UpdateCounts();
                ApplyFilter();
                GenerateAndStoreSessionReport();
            }
            finally
            {
                EndBulkOperation();
                IsProcessing = false;
                IsBulkRemoving = false;
                BulkRemoveProgress = 0;
                BulkRemoveTotal = 0;
                OnPropertyChanged(nameof(BulkRemoveProgressText));
            }
        }

        private static string BuildSingleRemovalOutcomeMessage(BloatwareApp app)
        {
            var detail = string.IsNullOrWhiteSpace(app.LastRemovalDetail)
                ? app.LastFailureReason
                : app.LastRemovalDetail;

            return app.LastRemovalStatus switch
            {
                RemovalStatus.VerifiedSuccess or RemovalStatus.Succeeded => string.IsNullOrWhiteSpace(detail)
                    ? string.Format(Strings.BloatwareManagerViewModel_RemovedFormat, app.Name)
                    : string.Format(Strings.BloatwareManagerViewModel_RemovedDetailFormat, app.Name, detail),
                RemovalStatus.Skipped => string.IsNullOrWhiteSpace(detail)
                    ? string.Format(Strings.BloatwareManagerViewModel_SkippedNoChange, app.Name)
                    : string.Format(Strings.BloatwareManagerViewModel_SkippedDetailFormat, app.Name, detail),
                RemovalStatus.Failed => string.IsNullOrWhiteSpace(detail)
                    ? string.Format(Strings.BloatwareManagerViewModel_RemoveFailedFormat, app.Name)
                    : string.Format(Strings.BloatwareManagerViewModel_RemoveFailedDetailFormat, app.Name, detail),
                RemovalStatus.Pending => string.Format(Strings.BloatwareManagerViewModel_RemovePendingFormat, app.Name),
                _ => string.IsNullOrWhiteSpace(detail)
                    ? string.Format(Strings.BloatwareManagerViewModel_RemoveUnknownFormat, app.Name)
                    : string.Format(Strings.BloatwareManagerViewModel_RemoveCompletedDetailFormat, app.Name, detail)
            };
        }

        private static string BuildBulkRemovalCompletionMessage(BulkRemovalResult result)
        {
            var summary = string.Format(Strings.BloatwareManagerViewModel_BulkCompleteFormat, result.Succeeded.Count, result.Skipped.Count, result.RequestedTotal);

            var firstSkippedWithDetail = result.Skipped
                .FirstOrDefault(app => !string.IsNullOrWhiteSpace(app.LastRemovalDetail));
            if (firstSkippedWithDetail != null)
            {
                summary += string.Format(Strings.BloatwareManagerViewModel_BulkSkipExampleFormat, firstSkippedWithDetail.Name, firstSkippedWithDetail.LastRemovalDetail);
            }

            return summary;
        }

        private void BuildRemovalPreview(string title, List<BloatwareApp> candidates)
        {
            RemovalPreviewItems.Clear();

            foreach (var app in candidates)
            {
                var item = new BloatwareRemovalPreviewItem(
                    app,
                    EstimateRemovalSeconds(app),
                    GetDependencyHint(app));

                item.PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName == nameof(BloatwareRemovalPreviewItem.IsSelected))
                    {
                        UpdateRemovalPreviewSummary();
                    }
                };

                RemovalPreviewItems.Add(item);
            }

            RemovalPreviewTitle = title;
            IsRemovalPreviewVisible = true;
            UpdateRemovalPreviewSummary();
        }


        private static string LocalizeBloatDescription(string? description) => description switch
        {
            "3D/Mixed Reality app - rarely used" => Strings.BloatDesc_3dMixedReality,
            "App startup - disable if not needed immediately" => Strings.BloatDesc_AppStartup,
            "Cortana assistant startup" => Strings.BloatDesc_CortanaStartup,
            "Dolby audio enhancement - may affect audio quality" => Strings.BloatDesc_DolbyAudio,
            "Groove Music / Movies & TV app" => Strings.BloatDesc_GrooveMusic,
            "HP OMEN software - conflict-sensitive until OmenCore fan, RGB, and hotkey control are verified" => Strings.BloatDesc_HpOmenSoftware,
            "HP audio enhancement software" => Strings.BloatDesc_HpAudioEnhancement,
            "HP diagnostics/telemetry tool - safe to remove" => Strings.BloatDesc_HpDiagnostics,
            "HP scheduled task" => Strings.BloatDesc_HpScheduledTask,
            "HP security software - can conflict with third-party AV" => Strings.BloatDesc_HpSecurity,
            "HP software startup item" => Strings.BloatDesc_HpStartupItem,
            "HP system control software - OmenCore replaces this" => Strings.BloatDesc_HpSystemControl,
            "Microsoft Bing news/weather app" => Strings.BloatDesc_BingNewsWeather,
            "Microsoft Solitaire with ads" => Strings.BloatDesc_Solitaire,
            "Microsoft Teams startup - disable if not using" => Strings.BloatDesc_TeamsStartup,
            "Microsoft video editor" => Strings.BloatDesc_VideoEditor,
            "OMEN lighting/control component - remove only after OmenCore keyboard lighting and OMEN key behavior are verified" => Strings.BloatDesc_OmenLighting,
            "OneDrive scheduled sync task" => Strings.BloatDesc_OnedriveSyncTask,
            "OneDrive startup - disable if not using cloud sync" => Strings.BloatDesc_OnedriveStartup,
            "Phone Link app" => Strings.BloatDesc_PhoneLink,
            "Pre-installed casual game" => Strings.BloatDesc_CasualGame,
            "Promotional app shortcut" => Strings.BloatDesc_PromoShortcut,
            "Skype messaging app" => Strings.BloatDesc_Skype,
            "Social media app" => Strings.BloatDesc_SocialMedia,
            "Telemetry data collection task" => Strings.BloatDesc_TelemetryTask,
            "Trial antivirus - can impact gaming performance" => Strings.BloatDesc_TrialAntivirus,
            "Windows People app" => Strings.BloatDesc_PeopleApp,
            "Windows tips and help app" => Strings.BloatDesc_TipsHelp,
            "Xbox app - keep if you use Xbox Game Pass" => Strings.BloatDesc_XboxApp,
            _ => description ?? string.Empty
        };
        private void UpdateRemovalPreviewSummary()
        {
            var selectedCount = RemovalPreviewItems.Count(item => item.IsSelected);
            var totalCount = RemovalPreviewItems.Count;
            _removalPreviewEstimatedSeconds = RemovalPreviewItems
                .Where(item => item.IsSelected)
                .Sum(item => item.EstimatedSeconds);

            var selectedApps = RemovalPreviewItems
                .Where(item => item.IsSelected)
                .Select(item => item.App)
                .ToList();
            var conflictSensitiveCount = selectedApps.Count(app => app.ConflictSensitive || app.RequiresOmenCoreValidation);

            RemovalPreviewSummary = string.Format(Strings.BloatwareManagerViewModel_PreviewSelectedFormat, selectedCount, totalCount);
            if (conflictSensitiveCount > 0)
            {
                RemovalPreviewSummary += string.Format(Strings.BloatwareManagerViewModel_PreviewConflictFormat, conflictSensitiveCount);
            }

            OnPropertyChanged(nameof(RemovalPreviewEstimatedText));
            OnPropertyChanged(nameof(HasRemovalPreviewSelection));
            RaiseCommandStates();
        }

        private async Task ConfirmRemovalPreviewAsync()
        {
            if (!IsRemovalPreviewVisible || IsProcessing)
            {
                return;
            }

            var selectedApps = RemovalPreviewItems
                .Where(item => item.IsSelected)
                .Select(item => item.App)
                .ToList();

            if (!selectedApps.Any())
            {
                SetStatusFailed(Strings.Bloatware_NoAppsSelected);
                return;
            }

            if (selectedApps.Any(app => app.RemovalRisk >= RemovalRisk.Medium))
            {
                var confirmation = MessageBox.Show(
                    Strings.Bloatware_RiskyConfirmBody,
                    Strings.Bloatware_RiskyConfirmTitle,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmation != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            var canProceed = await EnsureRestorePointBeforeRemovalAsync(selectedApps.Count);
            if (!canProceed)
            {
                return;
            }

            CancelRemovalPreview();

            if (selectedApps.Count == 1)
            {
                await ExecuteSingleRemovalAsync(selectedApps[0]);
                return;
            }

            await ExecuteBulkRemovalAsync(selectedApps);
        }

        private async Task<bool> EnsureRestorePointBeforeRemovalAsync(int selectedCount)
        {
            if (!BloatwareManagerService.IsRunningAsAdmin)
            {
                return true;
            }

            var createRestorePoint = MessageBox.Show(
                string.Format(Strings.Bloatware_RestorePointAskFormat, selectedCount),
                Strings.Bloatware_RestorePointAskTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (createRestorePoint != MessageBoxResult.Yes)
            {
                SetStatusAction(Strings.BloatwareManagerViewModel_RestoreSkipped);
                return true;
            }

            SetStatusAction(Strings.BloatwareManagerViewModel_CreatingRestorePoint);
            var restorePointResult = await _service.EnsurePreRemovalRestorePointAsync(selectedCount);
            if (restorePointResult.Success)
            {
                var createdAtText = restorePointResult.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? Strings.BloatwareManagerViewModel_RestoreNow;
                var prefix = restorePointResult.ReusedExisting ? Strings.BloatwareManagerViewModel_RestoreReused : Strings.BloatwareManagerViewModel_RestoreCreated;
                SetStatusDone($"{prefix} ({createdAtText})");
                return true;
            }

            var continueWithoutRestorePoint = MessageBox.Show(
                string.Format(Strings.Bloatware_RestorePointFailedFormat, restorePointResult.Message),
                Strings.Bloatware_RestorePointFailedTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (continueWithoutRestorePoint == MessageBoxResult.Yes)
            {
                SetStatusFailed(Strings.Bloatware_ProceedWithoutRestore);
                return true;
            }

            SetStatusFailed(Strings.Bloatware_CanceledNoRestore);
            return false;
        }

        private void CancelRemovalPreview()
        {
            IsRemovalPreviewVisible = false;
            RemovalPreviewItems.Clear();
            RemovalPreviewSummary = string.Empty;
            _removalPreviewEstimatedSeconds = 0;
            OnPropertyChanged(nameof(RemovalPreviewEstimatedText));
            OnPropertyChanged(nameof(HasRemovalPreviewSelection));
            RaiseCommandStates();
        }

        private void ToggleAllPreviewItems()
        {
            if (!RemovalPreviewItems.Any())
            {
                return;
            }

            var shouldSelectAll = RemovalPreviewItems.Any(item => !item.IsSelected);
            foreach (var item in RemovalPreviewItems)
            {
                item.IsSelected = shouldSelectAll;
            }

            UpdateRemovalPreviewSummary();
        }

        private int EstimateRemovalSeconds(BloatwareApp app)
        {
            return app.Type switch
            {
                BloatwareType.Win32App => 20,
                BloatwareType.AppxPackage => 8,
                BloatwareType.StartupItem => 3,
                BloatwareType.ScheduledTask => 3,
                _ => 8
            };
        }

        private string GetDependencyHint(BloatwareApp app)
        {
            if (app.DependencyTags.Any())
            {
                return string.Join("; ", app.DependencyTags);
            }

            if (!string.IsNullOrWhiteSpace(app.DependencyNotes))
            {
                return app.DependencyNotes;
            }

            var relatedCount = AllApps.Count(candidate =>
                !ReferenceEquals(candidate, app)
                && !candidate.IsRemoved
                && !string.IsNullOrWhiteSpace(candidate.Publisher)
                && candidate.Publisher.Equals(app.Publisher, StringComparison.OrdinalIgnoreCase));

            if (relatedCount <= 0)
            {
                return "No obvious linked packages detected";
            }

            return $"Potentially related packages from same publisher: {relatedCount}";
        }

        private async Task RestoreSelectedAsync()
        {
            if (SelectedApp == null || !SelectedApp.CanRestore || IsProcessing) return;

            try
            {
                IsProcessing = true;
                SetStatusAction(string.Format(Strings.Bloatware_RestoreActionItemFormat, SelectedApp.Name));
                await _service.RestoreAppAsync(SelectedApp);
                UpdateCounts();
                SetStatusDone(string.Format(Strings.Bloatware_RestoreDoneItemFormat, SelectedApp.Name));
            }
            finally
            {
                IsProcessing = false;
                OnPropertyChanged(nameof(CanRemoveSelected));
                OnPropertyChanged(nameof(CanRestoreSelected));
            }
        }

        private async Task RestoreAllRemovedAsync()
        {
            if (IsProcessing) return;

            var removedApps = AllApps.Where(a => a.IsRemoved && a.CanRestore).ToList();
            if (!removedApps.Any()) return;

            var result = MessageBox.Show(
                string.Format(Strings.Bloatware_RestoreAllFormat, removedApps.Count),
                Strings.Bloatware_RestoreAllTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                IsProcessing = true;
                IsBulkRestoring = true;
                BulkRestoreTotal = removedApps.Count;
                BulkRestoreProgress = 0;
                OnPropertyChanged(nameof(BulkRestoreProgressText));
                using var cancellation = BeginBulkOperation();
                var restoreResult = await _service.RestoreAppsAsync(
                    removedApps,
                    (index, total, app) => Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
                    {
                        BulkRestoreProgress = index;
                        OnPropertyChanged(nameof(BulkRestoreProgressText));
                        SetStatusAction(string.Format(Strings.Bloatware_RestoreProgressFormat, index, total, app.Name));
                    })),
                    cancellation.Token);

                if (restoreResult.Canceled)
                {
                    var canceledAt = restoreResult.CanceledAt?.Name ?? "the current item";
                    SetStatusFailed(
                        $"Bulk restore canceled at {canceledAt}. {restoreResult.Succeeded.Count} completed, {restoreResult.Skipped.Count} skipped.");
                }
                else if (restoreResult.Failed.Count > 0)
                {
                    SetStatusFailed(
                        $"Bulk restore finished with {restoreResult.Failed.Count} failures. Restored {restoreResult.Succeeded.Count}/{restoreResult.RequestedTotal} items.");
                }
                else
                {
                    SetStatusDone(string.Format(Strings.Bloatware_RestoredCountFormat, restoreResult.Succeeded.Count));
                }

                UpdateCounts();
                GenerateAndStoreSessionReport();
            }
            finally
            {
                EndBulkOperation();
                IsProcessing = false;
                IsBulkRestoring = false;
                BulkRestoreProgress = 0;
                BulkRestoreTotal = 0;
                OnPropertyChanged(nameof(BulkRestoreProgressText));
            }
        }

        private void ApplyFilter()
        {
            FilteredApps.Clear();

            var filtered = AllApps.AsEnumerable();

            // Filter by category
            if (SelectedCategory.HasValue)
            {
                filtered = filtered.Where(a => a.Category == SelectedCategory.Value);
            }

            // Filter by risk level
            if (_riskFilter != "All")
            {
                filtered = _riskFilter switch
                {
                    "Low"    => filtered.Where(a => a.RemovalRisk == RemovalRisk.Low),
                    "Medium" => filtered.Where(a => a.RemovalRisk == RemovalRisk.Medium),
                    "High"   => filtered.Where(a => a.RemovalRisk == RemovalRisk.High),
                    _        => filtered
                };
            }

            // Filter by text
            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                var search = FilterText.ToLowerInvariant();
                filtered = filtered.Where(a =>
                    a.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.Publisher.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var app in filtered)
            {
                FilteredApps.Add(app);
            }

            OnPropertyChanged(nameof(FilteredApps));
        }

        private void UpdateCounts()
        {
            OnPropertyChanged(nameof(TotalCount));
            OnPropertyChanged(nameof(RemovedCount));
            OnPropertyChanged(nameof(LowRiskCount));
            OnPropertyChanged(nameof(CanExportDryRun));
            RaiseCommandStates();
        }

        private void ExportDryRunReport()
        {
            var path = _service.ExportDryRunReport(AllApps);
            if (path == null)
            {
                SetStatusFailed(Strings.Bloatware_ExportNothingToScan);
                return;
            }

            LastReportPath = path;
            SetStatusDone(string.Format(Strings.Bloatware_DryRunExportedFormat, path));
            try { Process.Start("explorer.exe", $"/select,\"{path}\""); }
            catch { /* non-fatal; file is still created */ }
        }

        private void ExportResultLog()
        {
            var path = _service.ExportRemovalLog(AllApps);
            if (path == null)
            {
                SetStatusFailed(Strings.Bloatware_ExportNoRemovalLog);
                return;
            }

            LastReportPath = path;
            SetStatusDone(string.Format(Strings.Bloatware_LogExportedFormat, path));
            try { Process.Start("explorer.exe", $"/select,\"{path}\""); }
            catch { /* non-fatal; file is still created */ }
        }

        private void GenerateAndStoreSessionReport()
        {
            var path = _service.ExportRemovalLog(AllApps);
            if (!string.IsNullOrWhiteSpace(path))
            {
                LastReportPath = path;
            }
        }

        private void ViewLastReport()
        {
            if (!HasLastReport || string.IsNullOrWhiteSpace(LastReportPath))
            {
                SetStatusFailed(Strings.Bloatware_NoReport);
                return;
            }

            try
            {
                Process.Start("explorer.exe", LastReportPath);
            }
            catch (Exception ex)
            {
                SetStatusFailed(string.Format(Strings.Bloatware_OpenReportFailedFormat, ex.Message));
            }
        }

        private void SetStatusAction(string action)
        {
            StatusMessage = $"{action.Trim().TrimEnd('.')}...";
        }

        private void SetStatusDone(string details)
        {
            StatusMessage = Strings.BloatwareManagerViewModel_StatusDonePrefix + details;
        }

        private void SetStatusFailed(string reason)
        {
            StatusMessage = Strings.BloatwareManagerViewModel_StatusFailedPrefix + reason;
        }

        private CancellationTokenSource BeginBulkOperation()
        {
            EndBulkOperation();
            _bulkOperationCancellation = new CancellationTokenSource();
            OnPropertyChanged(nameof(CanCancelBulkOperation));
            RaiseCommandStates();
            return _bulkOperationCancellation;
        }

        private void EndBulkOperation()
        {
            _bulkOperationCancellation?.Dispose();
            _bulkOperationCancellation = null;
            OnPropertyChanged(nameof(CanCancelBulkOperation));
            RaiseCommandStates();
        }

        private void CancelBulkOperation()
        {
            if (!CanCancelBulkOperation)
            {
                return;
            }

            _bulkOperationCancellation?.Cancel();
            SetStatusAction(Strings.BloatwareManagerViewModel_CancelRequested);
        }

        private void RaiseCommandStates()
        {
            (ScanCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (RemoveSelectedCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (RemoveAllLowRiskCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (RestoreSelectedCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (RestoreAllRemovedCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ExportDryRunReportCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ExportResultLogCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (CancelBulkOperationCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ViewLastReportCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ConfirmRemovalPreviewCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (CancelRemovalPreviewCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ToggleAllPreviewItemsCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ApplyTelemetryOnlyPresetCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ApplyOmenConflictReducerPresetCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ApplyWindowsConsumerAppsPresetCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ApplyFullStandalonePresetCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private void ApplyTelemetryOnlyPreset()
        {
            var toRemove = AllApps
                .Where(a => !a.IsRemoved && a.Category == BloatwareCategory.Telemetry)
                .ToList();
            if (toRemove.Count == 0)
            {
                SetStatusDone(Strings.Bloatware_NoTelemetryPackages);
                return;
            }
            BuildRemovalPreview("Telemetry Only Preset", toRemove);
        }

        private void ApplyOmenConflictReducerPreset()
        {
            var toRemove = AllApps
                .Where(a => !a.IsRemoved && a.Category == BloatwareCategory.OemSoftware &&
                    (a.Name.Contains("OMEN", StringComparison.OrdinalIgnoreCase) ||
                     a.Name.Contains("Gaming Hub", StringComparison.OrdinalIgnoreCase) ||
                     a.Name.Contains("OMEN Hub", StringComparison.OrdinalIgnoreCase) ||
                     a.Name.Contains("Light Studio", StringComparison.OrdinalIgnoreCase) ||
                     a.Name.Contains("OmenCap", StringComparison.OrdinalIgnoreCase) ||
                     a.Name.Contains("OmenInstall", StringComparison.OrdinalIgnoreCase)))
                .ToList();
            if (toRemove.Count == 0)
            {
                SetStatusDone(Strings.Bloatware_NoConflictingPackages);
                return;
            }
            BuildRemovalPreview("OMEN Conflict Reducer Preset", toRemove);
        }

        private void ApplyWindowsConsumerAppsPreset()
        {
            var toRemove = AllApps
                .Where(a => !a.IsRemoved && a.Category == BloatwareCategory.WindowsApps)
                .ToList();
            if (toRemove.Count == 0)
            {
                SetStatusDone(Strings.Bloatware_NoConsumerApps);
                return;
            }
            BuildRemovalPreview("Windows Consumer Apps Preset", toRemove);
        }

        private void ApplyFullStandalonePreset()
        {
            var toRemove = AllApps
                .Where(a => !a.IsRemoved &&
                    (a.Category == BloatwareCategory.Telemetry ||
                     a.Category == BloatwareCategory.WindowsApps ||
                     (a.Category == BloatwareCategory.OemSoftware && 
                      (a.Name.Contains("OMEN", StringComparison.OrdinalIgnoreCase) ||
                       a.Name.Contains("Gaming Hub", StringComparison.OrdinalIgnoreCase) ||
                       a.Name.Contains("Light Studio", StringComparison.OrdinalIgnoreCase) ||
                       a.Name.Contains("OmenCap", StringComparison.OrdinalIgnoreCase)))))
                .ToList();
            if (toRemove.Count == 0)
            {
                SetStatusDone(Strings.Bloatware_NoFullStandaloneMatches);
                return;
            }
            BuildRemovalPreview("Full Standalone Prep Preset", toRemove);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public sealed class BloatwareRemovalPreviewItem : INotifyPropertyChanged
    {
        private bool _isSelected = true;

        public BloatwareApp App { get; }
        public string Name => App.Name;
        public RemovalRisk Risk => App.RemovalRisk;
        public int EstimatedSeconds { get; }
        public string EstimatedText => EstimatedSeconds <= 5 ? "< 5s" : $"~{EstimatedSeconds}s";
        public string DependencyHint { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                {
                    return;
                }

                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        public BloatwareRemovalPreviewItem(BloatwareApp app, int estimatedSeconds, string dependencyHint)
        {
            App = app;
            EstimatedSeconds = estimatedSeconds;
            DependencyHint = dependencyHint;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
