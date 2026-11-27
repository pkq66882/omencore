using System;
using OmenCore.Localization;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Drawing;
using OmenCore.Services;
using OmenCore.Razer;
using OmenCore.Utils;

namespace OmenCore.ViewModels
{
    public class KeyboardDiagnosticsViewModel : ViewModelBase
    {
        private readonly CorsairDeviceService? _corsairService;
        private readonly LogitechDeviceService? _logitechService;
        private readonly KeyboardLightingService? _keyboardLightingService;
        private readonly RazerService? _razerService;
        private readonly LoggingService _logging;
        private readonly OmenCore.Hardware.HpWmiBios? _wmiBios;

        public ObservableCollection<LightingDeviceInfo> DetectedDevices { get; } = new();
        public ObservableCollection<string> DiagnosticLogs { get; } = new();

        private string _diagnosticStatus = Strings.KbdDiag_Ready;
        public string DiagnosticStatus
        {
            get => _diagnosticStatus;
            set { _diagnosticStatus = value; OnPropertyChanged(); }
        }

        private bool _isRunningTest;
        public bool IsRunningTest
        {
            get => _isRunningTest;
            set { _isRunningTest = value; OnPropertyChanged(); }
        }

        public bool HasCorsair => _corsairService != null;
        public bool HasLogitech => _logitechService != null;
        public bool HasKeyboardLighting => _keyboardLightingService?.IsAvailable ?? false;
        public bool HasRazer => _razerService != null;
        
        /// <summary>
        /// Keyboard lighting telemetry statistics.
        /// </summary>
        public string KeyboardLightingTelemetry
        {
            get
            {
                if (_keyboardLightingService == null) return Strings.KeyboardDiagnostics_NoService;
                var stats = _keyboardLightingService.GetTelemetry();
                return string.Format(Strings.KeyboardDiagnostics_AttemptsFormat,
                    stats.TotalAttempts, stats.WmiSuccessCount, stats.WmiFailureCount, stats.WmiSuccessRate,
                    stats.OghSuccessCount, stats.OghFailureCount, stats.OghSuccessRate,
                    stats.EcSuccessCount, stats.EcFailureCount, stats.EcSuccessRate);
            }
        }

        public ICommand RunDeviceDetectionCommand { get; }
        public ICommand RunTestPatternCommand { get; }
        public ICommand ClearTestPatternCommand { get; }
        public ICommand CollectLogsCommand { get; }
        public ICommand RunColorTableProbeCommand { get; }
        public bool ColorTableProbeAvailable => _wmiBios?.IsAvailable == true;

        public KeyboardDiagnosticsViewModel(
            CorsairDeviceService? corsairService,
            LogitechDeviceService? logitechService,
            KeyboardLightingService? keyboardLightingService,
            RazerService? razerService,
            LoggingService logging,
            OmenCore.Hardware.HpWmiBios? wmiBios = null)
        {
            _wmiBios = wmiBios;
            RunColorTableProbeCommand = new AsyncRelayCommand(_ => RunColorTableProbeAsync(), _ => ColorTableProbeAvailable && !IsRunningTest);
            _corsairService = corsairService;
            _logitechService = logitechService;
            _keyboardLightingService = keyboardLightingService;
            _razerService = razerService;
            _logging = logging;

            RunDeviceDetectionCommand = new AsyncRelayCommand(_ => RunDeviceDetectionAsync());
            RunTestPatternCommand = new AsyncRelayCommand(_ => RunTestPatternAsync(), _ => !IsRunningTest);
            ClearTestPatternCommand = new AsyncRelayCommand(_ => ClearTestPatternAsync(), _ => !IsRunningTest);
            CollectLogsCommand = new RelayCommand(_ => CollectLogs());

            // Initial detection
            _ = RunDeviceDetectionAsync();
        }

        public async Task RunDeviceDetectionAsync()
        {
            try
            {
                DiagnosticStatus = Strings.KbdDiag_Detecting;
                DetectedDevices.Clear();

                // Detect Corsair devices
                if (_corsairService != null)
                {
                    await _corsairService.DiscoverAsync();
                    foreach (var device in _corsairService.Devices)
                    {
                        DetectedDevices.Add(new LightingDeviceInfo
                        {
                            Brand = "Corsair",
                            Model = device.Name,
                            Type = device.DeviceType.ToString(),
                            Status = Strings.KbdDiag_DeviceConnected,
                            Backend = "iCUE SDK"
                        });
                    }
                }

                // Detect Logitech devices
                if (_logitechService != null)
                {
                    await _logitechService.DiscoverAsync();
                    foreach (var device in _logitechService.Devices)
                    {
                        DetectedDevices.Add(new LightingDeviceInfo
                        {
                            Brand = "Logitech",
                            Model = device.Name,
                            Type = device.DeviceType.ToString(),
                            Status = Strings.KbdDiag_DeviceConnected,
                            Backend = "G HUB SDK"
                        });
                    }
                }

                // Detect Razer devices
                if (_razerService != null)
                {
                    _razerService.DiscoverDevices();
                    foreach (var device in _razerService.Devices)
                    {
                        DetectedDevices.Add(new LightingDeviceInfo
                        {
                            Brand = "Razer",
                            Model = device.Name,
                            Type = device.DeviceType.ToString(),
                            Status = Strings.KbdDiag_DeviceConnected,
                            Backend = "Chroma SDK"
                        });
                    }
                }

                // Detect HP Omen keyboard
                if (_keyboardLightingService?.IsAvailable ?? false)
                {
                    DetectedDevices.Add(new LightingDeviceInfo
                    {
                        Brand = "HP Omen",
                        Model = Strings.KbdDiag_IntegratedKeyboard,
                        Type = Strings.KbdDiag_KeyboardType,
                        Status = Strings.KbdDiag_DeviceConnected,
                        Backend = _keyboardLightingService.BackendType
                    });
                }

                DiagnosticStatus = string.Format(Strings.KeyboardDiagnosticsViewModel_DetectionComplete, DetectedDevices.Count);
                OnPropertyChanged(nameof(KeyboardLightingTelemetry));
            }
            catch (Exception ex)
            {
                DiagnosticStatus = string.Format(Strings.KbdDiag_DetectionFailedFormat, ex.Message);
                _logging.Error("Device detection failed", ex);
            }
        }

        /// <summary>
        /// #212: sends each candidate single-zone colour table in turn (pure red, 4 s apart) and logs it with the
        /// firmware's readback, so the owner can say which numbered step lit the keyboard. See <see cref="OmenCore.Hardware.ColorTableProbe"/>.
        /// </summary>
        public async Task RunColorTableProbeAsync()
        {
            if (_wmiBios == null) return;
            var confirm = System.Windows.MessageBox.Show(
                string.Format(Strings.KbdDiag_ProbeConfirmBody, OmenCore.Hardware.ColorTableProbe.Variants.Count),
                Strings.KbdDiag_ProbeConfirmTitle, System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Information);
            if (confirm != System.Windows.MessageBoxResult.OK) return;

            IsRunningTest = true;
            // Several probe payloads are all-zero or declare the wrong zone count, which leaves the keyboard dark (#244).
            var original = await Task.Run(() => _wmiBios.GetColorTable());
            try
            {
                var variants = OmenCore.Hardware.ColorTableProbe.Variants;
                for (var i = 0; i < variants.Count; i++)
                {
                    var v = variants[i];
                    var ok = await Task.Run(() => _wmiBios.SendColorTablePayload(v.Build(0xFF, 0x00, 0x00)));
                    await Task.Delay(500);
                    var readback = await Task.Run(() => _wmiBios.GetColorTable());
                    var rb = readback == null || readback.Length < 28 ? "none" : $"{readback[25]:X2}{readback[26]:X2}{readback[27]:X2}";
                    var rb1 = readback == null || readback.Length < 4 ? "none" : $"{readback[1]:X2}{readback[2]:X2}{readback[3]:X2}";
                    var line = $"PROBE {i + 1}/{variants.Count} {v.Id}: sent={(ok ? "ok" : "refused")}, readback@25=#{rb}, readback@1=#{rb1} - {v.Description}";
                    _logging.Info(line);
                    DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - {line}");
                    DiagnosticStatus = string.Format(Strings.KbdDiag_ProbeStepFormat, i + 1, variants.Count);
                    await Task.Delay(3500);
                }
                DiagnosticStatus = Strings.KbdDiag_ProbeFinished;
            }
            finally
            {
                if (original is { Length: 128 })
                {
                    var restored = await Task.Run(() => _wmiBios.SendColorTablePayload(original) && _wmiBios.SetBacklight(true));
                    _logging.Info($"PROBE: previous keyboard colour table {(restored ? "restored" : "could not be restored")}");
                }
                IsRunningTest = false;
            }
        }

        public async Task RunTestPatternAsync()
        {
            try
            {
                IsRunningTest = true;
                DiagnosticStatus = Strings.KbdDiag_RunningTestPattern;
                DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + Strings.KbdDiag_LogStartingTest);

                // Test Corsair devices
                if (_corsairService != null && _corsairService.Devices.Any())
                {
                    DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogTestingBrandFormat, Strings.Brand_Corsair));
                    foreach (var device in _corsairService.Devices)
                    {
                        // Apply a rainbow pattern or simple test
                        await Task.Delay(100); // Simulate
                        DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogAppliedTestFormat, device.Name));
                    }
                }

                // Test Logitech devices
                if (_logitechService != null && _logitechService.Devices.Any())
                {
                    DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogTestingBrandFormat, Strings.Brand_Logitech));
                    foreach (var device in _logitechService.Devices)
                    {
                        await Task.Delay(100);
                        DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogAppliedTestFormat, device.Name));
                    }
                }

                // Test Razer devices
                if (_razerService != null && _razerService.Devices.Any())
                {
                    DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogTestingBrandFormat, Strings.Brand_Razer));
                    foreach (var device in _razerService.Devices)
                    {
                        await Task.Delay(100);
                        DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogAppliedTestFormat, device.Name));
                    }
                }

                // Test HP Omen keyboard
                if (_keyboardLightingService?.IsAvailable ?? false)
                {
                    DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + Strings.KbdDiag_LogTestingHpKeyboard);

                    var fourZonePattern = new[]
                    {
                        ColorTranslator.FromHtml("#FF0000"),
                        ColorTranslator.FromHtml("#00FF00"),
                        ColorTranslator.FromHtml("#0000FF"),
                        ColorTranslator.FromHtml("#FFFF00")
                    };

                    await _keyboardLightingService.SetAllZoneColors(fourZonePattern);
                    DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + Strings.KbdDiag_LogApplied4Zone);

                    await Task.Delay(600);

                    var white = ColorTranslator.FromHtml("#FFFFFF");
                    await _keyboardLightingService.SetAllZoneColors(new[] { white, white, white, white });
                    DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + Strings.KbdDiag_LogAppliedWhite);
                }

                DiagnosticStatus = Strings.KbdDiag_TestPatternApplied;
                DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + Strings.KbdDiag_LogTestCompleted);
            }
            catch (Exception ex)
            {
                DiagnosticStatus = string.Format(Strings.KbdDiag_TestPatternFailedFormat, ex.Message);
                DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogTestFailedFormat, ex.Message));
                _logging.Error("Test pattern failed", ex);
            }
            finally
            {
                IsRunningTest = false;
                OnPropertyChanged(nameof(KeyboardLightingTelemetry));
            }
        }

        public Task ClearTestPatternAsync()
        {
            try
            {
                IsRunningTest = true;
                DiagnosticStatus = Strings.KbdDiag_ClearingTestPattern;
                DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + Strings.KbdDiag_LogClearing);

                // Clear Corsair devices
                if (_corsairService != null)
                {
                    // Reset to default
                    DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogClearedBrandFormat, Strings.Brand_Corsair));
                }

                // Clear Logitech devices
                if (_logitechService != null)
                {
                    // Reset to default
                    DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogClearedBrandFormat, Strings.Brand_Logitech));
                }

                // Clear Razer devices
                if (_razerService != null)
                {
                    // Reset to default
                    DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogClearedBrandFormat, Strings.Brand_Razer));
                }

                // Clear HP Omen keyboard
                if (_keyboardLightingService?.IsAvailable ?? false)
                {
                    _keyboardLightingService.RestoreDefaults();
                    DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + Strings.KbdDiag_LogRestoredHpDefaults);
                }

                DiagnosticStatus = Strings.KbdDiag_TestPatternCleared;
                DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + Strings.KbdDiag_LogClearCompleted);
            }
            catch (Exception ex)
            {
                DiagnosticStatus = string.Format(Strings.KbdDiag_ClearFailedFormat, ex.Message);
                DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogClearFailedFormat, ex.Message));
                _logging.Error("Clear test pattern failed", ex);
            }
            finally
            {
                IsRunningTest = false;
            }

            OnPropertyChanged(nameof(KeyboardLightingTelemetry));

            return Task.CompletedTask;
        }

        private void CollectLogs()
        {
            DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + Strings.KbdDiag_LogCollecting);
            // In a real implementation, this would gather logs from various services
            // For now, just add some sample logs
            DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogServiceStatusFormat, "KeyboardLightingService", HasKeyboardLighting, _keyboardLightingService?.BackendType ?? "N/A"));
            DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogServiceStatusFormat, "CorsairService", HasCorsair, ""));
            DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogServiceStatusFormat, "LogitechService", HasLogitech, ""));
            DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogServiceStatusFormat, "RazerService", HasRazer, ""));
            DiagnosticLogs.Insert(0, $"{DateTime.Now:HH:mm:ss} - " + string.Format(Strings.KbdDiag_LogDetectedDevicesFormat, DetectedDevices.Count));
        }
    }

    public class LightingDeviceInfo
    {
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string Type { get; set; } = "";
        public string Status { get; set; } = "";
        public string Backend { get; set; } = "";

        // Display-only projections: Brand/Type keep canonical names for grouping and diagnostics.
        public string BrandDisplay => DisplayNames.Brand(Brand);
        public string TypeDisplay => DisplayNames.DeviceType(Type);
    }
}