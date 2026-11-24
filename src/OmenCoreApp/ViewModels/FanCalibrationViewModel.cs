using OmenCore.Localization;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using OmenCore.Hardware;
using OmenCore.Services;
using OmenCore.Services.FanCalibration;
using OmenCore.Utils;

namespace OmenCore.ViewModels
{
    /// <summary>
    /// ViewModel for the fan calibration wizard UI.
    /// </summary>
    public class FanCalibrationViewModel : INotifyPropertyChanged
    {
        private readonly FanCalibrationService _calibrationService;
        private readonly LoggingService _logging;
        private CancellationTokenSource? _cts;

        private string _status = Strings.FanCalibration_StatusNotStarted;
        private int _progress = 0;
        private bool _isCalibrating = false;
        private string _calibrationInfo = "";

        public event PropertyChangedEventHandler? PropertyChanged;

        public FanCalibrationViewModel(FanCalibrationService calibrationService, LoggingService logging)
        {
            _calibrationService = calibrationService;
            _logging = logging;

            // Commands
            StartCalibrationCommand = new RelayCommand(async _ => await StartCalibrationAsync(), _ => CanStartCalibration);
            CancelCalibrationCommand = new RelayCommand(_ => CancelCalibration(), _ => IsCalibrating);

            // Subscribe to calibration events
            _calibrationService.CalibrationStepCompleted += OnCalibrationStepCompleted;
            _calibrationService.CalibrationCompleted += OnCalibrationCompleted;
            _calibrationService.CalibrationError += OnCalibrationError;

            // Initialize
            UpdateCalibrationInfo();
        }

        #region Properties

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public int Progress
        {
            get => _progress;
            set { _progress = value; OnPropertyChanged(); }
        }

        public bool IsCalibrating
        {
            get => _isCalibrating;
            set
            {
                _isCalibrating = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanStartCalibration));
            }
        }

        public bool CanStartCalibration => !_isCalibrating;

        public string CalibrationInfo
        {
            get => _calibrationInfo;
            set { _calibrationInfo = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CalibrationStepItem> CalibrationSteps { get; } = new();

        public FanCalibrationProfile? Profile => _calibrationService.ActiveProfile;

        #endregion

        #region Commands

        public ICommand StartCalibrationCommand { get; }
        public ICommand CancelCalibrationCommand { get; }

        #endregion

        #region Calibration

        private async Task StartCalibrationAsync()
        {
            IsCalibrating = true;
            Status = Strings.FanCalibration_Preparing;
            Progress = 0;
            CalibrationSteps.Clear();

            _cts = new CancellationTokenSource();

            try
            {
                Status = Strings.FanCalibration_RunningWizard;
                await _calibrationService.StartCalibrationAsync(_cts.Token);
            }
            catch (OperationCanceledException)
            {
                Status = Strings.FanCalibration_Cancelled;
            }
            catch (Exception ex)
            {
                Status = string.Format(Strings.FanCalibration_ErrorFormat, ex.Message);
                _logging.Error($"Calibration error: {ex.Message}", ex);
            }
            finally
            {
                IsCalibrating = false;
                _cts = null;
                UpdateCalibrationInfo();
            }
        }

        private void CancelCalibration()
        {
            _cts?.Cancel();
            _calibrationService.CancelCalibration();
            Status = Strings.FanCalibration_Cancelling;
        }

        private void OnCalibrationStepCompleted(object? sender, CalibrationStep step)
        {
            // Update UI on UI thread
            DispatcherHelper.RunOnUiThread(() =>
            {
                CalibrationSteps.Add(new CalibrationStepItem
                {
                    Level = step.Level,
                    Fan0Rpm = step.Fan0Rpm,
                    Fan1Rpm = step.Fan1Rpm,
                    Status = Strings.FanCalibration_CompleteCheck
                });

                Progress = _calibrationService.CalibrationProgress;
                Status = string.Format(Strings.FanCalibration_TestingLevelFormat, step.Level, Progress);
            });
        }

        private void OnCalibrationCompleted(object? sender, FanCalibrationProfile profile)
        {
            DispatcherHelper.RunOnUiThread(() =>
            {
                Status = Strings.FanCalibration_CompleteExclaim;
                Progress = 100;
                IsCalibrating = false;
                UpdateCalibrationInfo();
                OnPropertyChanged(nameof(Profile));
            });
        }

        private void OnCalibrationError(object? sender, string error)
        {
            DispatcherHelper.RunOnUiThread(() =>
            {
                Status = string.Format(Strings.FanCalibration_ErrorWarnFormat, error);
                IsCalibrating = false;
            });
        }

        private void UpdateCalibrationInfo()
        {
            var profile = _calibrationService.ActiveProfile;
            if (profile == null || !profile.IsValid)
            {
                CalibrationInfo = Strings.FC2_NoCalibration;
            }
            else
            {
                var age = DateTime.Now - profile.CalibratedAt;
                CalibrationInfo = string.Format(
                                    Strings.FC2_CalibrationInfoFormat,
                                    profile.ModelName,
                                    profile.MaxLevel,
                                    profile.MinSpinLevel,
                                    profile.Fan0MaxRpm,
                                    profile.Fan1MaxRpm,
                                    profile.CalibratedAt.ToString("g"),
                                    age.TotalDays);
            }
        }

        #endregion

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Cleanup()
        {
            _calibrationService.CalibrationStepCompleted -= OnCalibrationStepCompleted;
            _calibrationService.CalibrationCompleted -= OnCalibrationCompleted;
            _calibrationService.CalibrationError -= OnCalibrationError;
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }

    /// <summary>
    /// UI item for a calibration step.
    /// </summary>
    public class CalibrationStepItem
    {
        public int Level { get; set; }
        public int Fan0Rpm { get; set; }
        public int Fan1Rpm { get; set; }
        public string Status { get; set; } = "";
    }
}
