using OmenCore.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using OmenCore.Services;

namespace OmenCore.Controls
{
    /// <summary>
    /// Control for calibrating fan RPM curves for accurate speed control.
    /// </summary>
    public partial class FanCalibrationControl : UserControl, INotifyPropertyChanged
    {
        private readonly LoggingService _logging;
        private readonly IFanVerificationService? _fanVerifier;
        private readonly FanCalibrationStorageService _calibrationStorage;

        private CancellationTokenSource? _calibrationCts;
        private string _currentModelId = "";
        private int _selectedFanIndex = 0;
        private FanCalibrationResult? _lastCalibrationResult;

        public event PropertyChangedEventHandler? PropertyChanged;

        public FanCalibrationControl(IFanVerificationService fanVerifier, LoggingService logging)
        {
            InitializeComponent();

            _fanVerifier = fanVerifier ?? throw new ArgumentNullException(nameof(fanVerifier));
            _logging = logging ?? throw new ArgumentNullException(nameof(logging));
            _calibrationStorage = new FanCalibrationStorageService(_logging);

            Loaded += FanCalibrationControl_Loaded;
        }

        public FanCalibrationControl()
        {
            InitializeComponent();

            // Legacy constructor for XAML designer/standalone use.
            // Runtime wizard now injects dependencies directly from FanControlViewModel.
            var services = App.ServiceProvider;
            _logging = (services?.GetService(typeof(LoggingService)) as LoggingService) ?? App.Logging;
            _fanVerifier = services?.GetService(typeof(IFanVerificationService)) as IFanVerificationService;
            _calibrationStorage = new FanCalibrationStorageService(_logging);

            Loaded += FanCalibrationControl_Loaded;
        }

        private async void FanCalibrationControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_fanVerifier == null)
            {
                ModelInfoText.Text = Strings.FanCalibration_ServiceUnavailable;
                CalibrationStatusText.Text = Strings.FanCalibration_MissingVerificationService;
                CalibrationStatusText.Foreground = System.Windows.Media.Brushes.OrangeRed;
                StartCalibrationButton.IsEnabled = false;
                LoadCalibrationButton.IsEnabled = false;
                return;
            }

            await LoadModelInformationAsync();
            UpdateCalibrationStatus();
        }

        private async Task LoadModelInformationAsync()
        {
            try
            {
                // Try to get model information from WMI
                var modelInfo = await Task.Run(() =>
                {
                    try
                    {
                        using var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
                        foreach (var obj in searcher.Get())
                        {
                            var manufacturer = obj["Manufacturer"]?.ToString() ?? "";
                            var model = obj["Model"]?.ToString() ?? "";
                            return $"{manufacturer} {model}".Trim();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logging.Warn($"[FanCalibrationControl] WMI model query failed: {ex.Message} — using fallback");
                    }
                    return "Unknown Model";
                });

                // Generate model ID from model info
                _currentModelId = GenerateModelId(modelInfo);

                ModelInfoText.Text = string.Format(Strings.FanCalibration_ModelLineFormat, modelInfo, _currentModelId);
            }
            catch (Exception ex)
            {
                _logging.Error($"Failed to load model information: {ex.Message}", ex);
                ModelInfoText.Text = Strings.FanCalibration_ModelDetectFailed;
                _currentModelId = "unknown";
            }
        }

        private void UpdateCalibrationStatus()
        {
            if (_calibrationStorage.HasCalibration(_currentModelId))
            {
                var calibration = _calibrationStorage.GetCalibration(_currentModelId);
                CalibrationStatusText.Text = string.Format(Strings.FanCalibration_AvailableFormat, calibration?.CreatedDate.ToShortDateString());
                CalibrationStatusText.Foreground = System.Windows.Media.Brushes.Green;
                LoadCalibrationButton.IsEnabled = true;
            }
            else
            {
                CalibrationStatusText.Text = Strings.FanCalibration_NoCalibrationData;
                CalibrationStatusText.Foreground = System.Windows.Media.Brushes.Orange;
                LoadCalibrationButton.IsEnabled = false;
            }
        }

        private void FanSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FanSelector.SelectedItem is ComboBoxItem item && item.Tag is string tag)
            {
                if (int.TryParse(tag, out int fanIndex))
                {
                    _selectedFanIndex = fanIndex;
                    UpdateFanStatus();
                }
            }
        }

        private void UpdateFanStatus()
        {
            if (_fanVerifier == null)
            {
                FanStatusText.Text = Strings.FanCalibration_VerificationUnavailable;
                return;
            }

            try
            {
                var (rpm, level, source) = _fanVerifier.GetCurrentFanStateWithSource(_selectedFanIndex);
                var expectedPercent = _calibrationStorage.GetCalibratedPercent(_currentModelId, _selectedFanIndex, rpm);

                FanStatusText.Text = string.Format(Strings.FanCalibration_FanStatusFormat, rpm, level, source, expectedPercent);
            }
            catch (Exception ex)
            {
                FanStatusText.Text = string.Format(Strings.FanCalibration_ReadStatusFailedFormat, ex.Message);
                _logging.Error($"Failed to update fan status: {ex.Message}", ex);
            }
        }

        private async void StartCalibrationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_fanVerifier == null || !_fanVerifier.IsAvailable)
            {
                MessageBox.Show(Strings.FanCalibration_VerificationUnavailableBody,
                              Strings.FanCalibration_CalibrationUnavailableTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                Strings.FanCalibration_StartConfirmBody,
                Strings.FanCalibration_StartConfirmTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            await StartCalibrationAsync();
        }

        private async Task StartCalibrationAsync()
        {
            if (_fanVerifier == null)
            {
                throw new InvalidOperationException("Fan verification service is unavailable.");
            }

            _calibrationCts = new CancellationTokenSource();

            try
            {
                // Update UI
                StartCalibrationButton.Visibility = Visibility.Collapsed;
                StopCalibrationButton.Visibility = Visibility.Visible;
                ProgressGroup.Visibility = Visibility.Visible;
                ResultsGroup.Visibility = Visibility.Collapsed;
                SaveCalibrationButton.Visibility = Visibility.Collapsed;

                CalibrationProgress.Value = 0;
                ProgressText.Text = Strings.FanCalibration_Starting;

                // Perform calibration
                var calibrationResult = await _fanVerifier.PerformFanCalibrationAsync(_selectedFanIndex, _calibrationCts.Token);

                // Update progress
                CalibrationProgress.Value = 100;
                ProgressText.Text = Strings.FanCalibration_Completed;

                // Show results
                await DisplayCalibrationResultsAsync(calibrationResult);
                _lastCalibrationResult = calibrationResult.Success ? calibrationResult : null;

                // Enable save button if successful
                if (calibrationResult.Success)
                {
                    SaveCalibrationButton.Visibility = Visibility.Visible;
                }

                _logging.Info($"Fan calibration completed: {calibrationResult.Success}");
            }
            catch (OperationCanceledException)
            {
                ProgressText.Text = Strings.FanCalibration_Cancelled;
                _logging.Info("Fan calibration cancelled by user");
            }
            catch (Exception ex)
            {
                ProgressText.Text = string.Format(Strings.FanCalibration_FailedDetailFormat, ex.Message);
                _logging.Error($"Fan calibration failed: {ex.Message}", ex);
                MessageBox.Show(string.Format(Strings.FanCalibration_CalibrationFailedFormat, ex.Message), Strings.FanCalibration_CalibrationErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Reset UI
                StartCalibrationButton.Visibility = Visibility.Visible;
                StopCalibrationButton.Visibility = Visibility.Collapsed;
                _calibrationCts?.Dispose();
                _calibrationCts = null;
            }
        }

        private Task DisplayCalibrationResultsAsync(FanCalibrationResult result)
        {
            ResultsGroup.Visibility = Visibility.Visible;

            if (result.Success)
            {
                ResultsSummaryText.Text = string.Format(Strings.FanCalibration_SuccessFormat, result.CalibrationPoints.Count, result.Duration.TotalSeconds.ToString("F1"));
                ResultsSummaryText.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                ResultsSummaryText.Text = string.Format(Strings.FanCalibration_IssuesFormat, result.CalibrationPoints.Count, result.CalibrationPoints.Count(p => !p.VerificationPassed));
                ResultsSummaryText.Foreground = System.Windows.Media.Brushes.Orange;
            }

            // Convert to display format
            var displayItems = result.CalibrationPoints.Select(p => new CalibrationResultDisplayItem
            {
                RequestedPercent = p.RequestedPercent,
                AppliedLevel = p.AppliedLevel,
                MeasuredRpm = p.MeasuredRpm,
                RpmRange = $"{p.RpmRangeMin}-{p.RpmRangeMax}",
                Status = p.VerificationPassed ? "✓ Pass" : "✗ Fail",
                DurationFormatted = p.Duration.TotalSeconds.ToString("F1") + "s"
            }).ToList();

            CalibrationResultsGrid.ItemsSource = displayItems;

            return Task.CompletedTask;
        }

        private void StopCalibrationButton_Click(object sender, RoutedEventArgs e)
        {
            _calibrationCts?.Cancel();
        }

        private void RestoreAutoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_fanVerifier == null)
            {
                MessageBox.Show(Strings.FanCalibration_VerificationUnavailable, Strings.FanCalibration_RestoreAutoUnavailableTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var restored = _fanVerifier.RestoreFanControlAfterCalibration();
            ProgressText.Text = restored
                ? "BIOS auto fan control restored"
                : "Could not restore BIOS auto fan control from this backend";
            ProgressGroup.Visibility = Visibility.Visible;
            UpdateFanStatus();
        }

        private void LoadCalibrationButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateCalibrationStatus();
            UpdateFanStatus();
            var calibration = _calibrationStorage.GetCalibration(_currentModelId);
            var fanCount = calibration?.FanCalibrations.Count ?? 0;
            MessageBox.Show(
                string.Format(Strings.FanCalibration_LoadedFormat, _currentModelId, fanCount),
                Strings.FanCalibration_LoadedTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private async void SaveCalibrationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_lastCalibrationResult == null || !_lastCalibrationResult.Success)
                {
                    MessageBox.Show(
                        Strings.FanCalibration_NothingToSaveBody,
                        Strings.FanCalibration_NothingToSaveTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var calibration = _calibrationStorage.CreateCalibrationFromResults(
                    _currentModelId,
                    new List<FanCalibrationResult> { _lastCalibrationResult },
                    _currentModelId);
                await _calibrationStorage.StoreCalibrationAsync(_currentModelId, calibration);
                UpdateCalibrationStatus();
                SaveCalibrationButton.Visibility = Visibility.Collapsed;

                MessageBox.Show(
                    string.Format(Strings.FanCalibration_SavedFormat, _currentModelId),
                    Strings.FanCalibration_SavedTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logging.Error($"Failed to save calibration: {ex.Message}", ex);
                MessageBox.Show(string.Format(Strings.FanCalibration_SaveFailedFormat, ex.Message), Strings.FanCalibration_SaveErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearCalibrationButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                Strings.FanCalibration_ClearConfirmBody,
                Strings.FanCalibration_ClearConfirmTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Clear calibration data
                UpdateCalibrationStatus();
                MessageBox.Show(Strings.FanCalibration_DataCleared, Strings.FanCalibration_DataClearedTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private static string GenerateModelId(string modelInfo)
        {
            return FanCalibrationStorageService.NormalizeModelId(modelInfo);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Display item for calibration results DataGrid.
    /// </summary>
    public class CalibrationResultDisplayItem
    {
        public int RequestedPercent { get; set; }
        public int AppliedLevel { get; set; }
        public int MeasuredRpm { get; set; }
        public string RpmRange { get; set; } = "";
        public string Status { get; set; } = "";
        public string DurationFormatted { get; set; } = "";
    }
}
