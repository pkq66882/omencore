using System;
using OmenCore.Localization;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using OmenCore.Controls;
using OmenCore.Models;
using OmenCore.Services;
using OmenCore.Services.Diagnostics;
using OmenCore.Views;

namespace OmenCore.Utils
{
    /// <summary>
    /// Manages the system tray icon with live temperature monitoring and quick actions.
    /// </summary>
    public class TrayIconService : IDisposable
    {
        private readonly TaskbarIcon _trayIcon;
        private const string TrayRefreshTimerRegistryName = "TrayIconRefresh";
        private const int TrayRefreshIntervalMs = 2000;
        private readonly IDisposable _updateTimerSubscription;
        private readonly Action _showMainWindow;
        private readonly Action _shutdownApp;
        private readonly ImageSource? _baseIconSource;
        private readonly DisplayService _displayService;
        private readonly string _appVersion;
        private QuickPopupWindow? _quickPopup;
        private MenuItem? _cpuTempMenuItem;
        private MenuItem? _gpuTempMenuItem;
        private MenuItem? _fanModeMenuItem;
        private MenuItem? _performanceModeMenuItem;
        private MenuItem? _stayOnTopMenuItem;
        private MenuItem? _displayMenuItem;  // For updating refresh rate display
        private MenuItem? _monitoringHealthMenuItem;
        private MonitoringSample? _latestSample;
        private string _currentFanMode = "Auto";
        private string? _pendingFanModeRequest;
        private string _currentPerformanceMode = "Balanced";
        private string? _curvePresetName;
        private bool _linkFanToPerformanceMode;
        private string _monitoringHealth = "Unknown";
        private bool _disposed;
        private readonly ConfigurationService? _configService;
        private readonly NotificationService? _notificationService;
        private bool? _lastRegisteredTrayTempDisplayEnabled;
        private string? _lastTooltipText;
        private int? _lastRenderedBadgeTemperature;
        private bool? _lastRenderedTrayTempDisplayEnabled;
        
        // v2.6.1: Track fan menu items for checkmarks
        private MenuItem? _fanAutoMenuItem;
        private MenuItem? _fanMaxMenuItem;
        private MenuItem? _fanQuietMenuItem;
        private MenuItem? _perfBalancedMenuItem;
        private MenuItem? _perfPerformanceMenuItem;
        private MenuItem? _perfQuietMenuItem;
        
        // v2.7.0: GPU Power and Keyboard backlight menu items
        private MenuItem? _gpuPowerMenuItem;
        private MenuItem? _gpuPowerMinMenuItem;
        private MenuItem? _gpuPowerMedMenuItem;
        private MenuItem? _gpuPowerMaxMenuItem;
        private MenuItem? _gpuPowerExtMenuItem;
        private MenuItem? _keyboardBacklightMenuItem;
        private string _currentGpuPowerLevel = "Medium";
        private int _currentKeyboardBrightness = 3;

        // v3.0.2: Additional live status menu items
        private MenuItem? _ramMenuItem;
        private MenuItem? _fanStatusMenuItem;
        private MenuItem? _batteryMenuItem;
        
        // Throttling to prevent flicker during system events (brightness keys, etc.)
        // Use Interlocked for thread-safe access from timer callback
        private int _isUpdatingIcon = 0; // 0 = false, 1 = true (for Interlocked)
        private long _lastIconUpdateTicks = 0;
        private const int MinIconUpdateIntervalMs = 500;

        public event Action<string>? FanModeChangeRequested;
        public event Action<string>? PerformanceModeChangeRequested;
        public event Action<string>? QuickProfileChangeRequested;
        public event Action<string>? GpuPowerChangeRequested;
        public event Action<int>? KeyboardBacklightChangeRequested;
        public event Action? KeyboardBacklightToggleRequested;
        public event Action? CheckForUpdatesRequested;
        public event Action? ReportProblemRequested;
        public event Action? MemoryCleanRequested;
        
        /// <summary>
        /// Forces immediate refresh of the tray icon (e.g., when temp display setting changes).
        /// </summary>
        public void RefreshTrayIcon()
        {
            Application.Current?.Dispatcher?.BeginInvoke(() =>
            {
                UpdateTrayRefreshTimerDescription(force: true);
                UpdateTrayDisplay(null, EventArgs.Empty);
            });
        }

        public TrayIconService(TaskbarIcon trayIcon, Action showMainWindow, Action shutdownApp, ConfigurationService? configService = null, NotificationService? notificationService = null)
        {
            _trayIcon = trayIcon;
            _showMainWindow = showMainWindow;
            _shutdownApp = shutdownApp;
            _displayService = new DisplayService(App.Logging);
            _configService = configService;
            _notificationService = notificationService;
            _appVersion = AppVersionProvider.GetVersionString();

            _baseIconSource = LoadBaseIcon();
            _trayIcon.IconSource = _baseIconSource;

            InitializeContextMenu();

            // First consumer migrated onto the shared UiPollingCoordinator (see ROADMAP_v4.0.0.md
            // Phase B timer-consolidation item) instead of owning an independent DispatcherTimer.
            _updateTimerSubscription = UiPollingCoordinator.Subscribe(
                TrayRefreshTimerRegistryName,
                TimeSpan.FromMilliseconds(TrayRefreshIntervalMs),
                () => UpdateTrayDisplay(null, EventArgs.Empty));
            RegisterTrayRefreshTimer();

            UpdateTrayDisplay(null, EventArgs.Empty);
        }

        /// <summary>Localized age label for the notification submenu (Core's TimeAgo is English-only).</summary>
        private static string FormatNotificationAge(DateTime timestamp)
        {
            var elapsed = DateTime.Now - timestamp;
            if (elapsed.TotalMinutes < 1) return Strings.Notify_JustNow;
            if (elapsed.TotalMinutes < 60) return string.Format(Strings.Notify_MinutesAgoFormat, (int)elapsed.TotalMinutes);
            if (elapsed.TotalHours < 24) return string.Format(Strings.Notify_HoursAgoFormat, (int)elapsed.TotalHours);
            if (elapsed.TotalDays < 7) return string.Format(Strings.Notify_DaysAgoFormat, (int)elapsed.TotalDays);
            return timestamp.ToString(Strings.Notify_AgeDatePattern);
        }
        private void InitializeContextMenu()
        {
            // Use the fully templated dark menu to avoid default system gutters/white edges.
            var contextMenu = new DarkContextMenu();
            contextMenu.MinWidth = 320;
            contextMenu.Padding = new Thickness(0, 4, 0, 4);
            var menuItemStyle = contextMenu.Resources[typeof(MenuItem)] as Style ?? new Style(typeof(MenuItem));

            // ═══ HEADER ═══
            var headerItem = new MenuItem { Header = $"OmenCore  v{_appVersion}", IsEnabled = false, FontWeight = FontWeights.Bold, FontSize = 13 };
            contextMenu.Items.Add(headerItem);
            contextMenu.Items.Add(new Separator());

            // ═══ LIVE STATUS ═══
            _cpuTempMenuItem = new MenuItem { Header = Strings.TrayIcon_CpuTempPlaceholder, IsEnabled = false };
            _cpuTempMenuItem.FontFamily = AppFonts.Monospace;
            contextMenu.Items.Add(_cpuTempMenuItem);

            _gpuTempMenuItem = new MenuItem { Header = Strings.TrayIcon_GpuTempPlaceholder, IsEnabled = false };
            _gpuTempMenuItem.FontFamily = AppFonts.Monospace;
            contextMenu.Items.Add(_gpuTempMenuItem);

            _monitoringHealthMenuItem = new MenuItem { Header = string.Format(Strings.TrayIcon_MonitorFormat, Strings.TrayIcon_HealthUnknown), IsEnabled = false };
            _monitoringHealthMenuItem.FontFamily = AppFonts.Monospace;
            contextMenu.Items.Add(_monitoringHealthMenuItem);

            _ramMenuItem = new MenuItem { Header = Strings.TrayIcon_RamPlaceholder, IsEnabled = false };
            _ramMenuItem.FontFamily = AppFonts.Monospace;
            contextMenu.Items.Add(_ramMenuItem);

            _fanStatusMenuItem = new MenuItem { Header = Strings.TrayIcon_FanPlaceholder, IsEnabled = false };
            _fanStatusMenuItem.FontFamily = AppFonts.Monospace;
            contextMenu.Items.Add(_fanStatusMenuItem);

            _batteryMenuItem = new MenuItem { Header = Strings.TrayIcon_BatteryPlaceholder, IsEnabled = false };
            _batteryMenuItem.FontFamily = AppFonts.Monospace;
            _batteryMenuItem.Visibility = Visibility.Collapsed;
            contextMenu.Items.Add(_batteryMenuItem);

            contextMenu.Items.Add(new Separator());

            // ═══ QUICK PROFILES (Combined Fan + Performance) ═══
            var quickProfileMenuItem = new MenuItem { Header = Strings.TrayIcon_QuickProfile };

            var profilePerformance = new MenuItem { Header = Strings.TrayIcon_ProfilePerformance };
            profilePerformance.Click += (s, e) => QuickProfileChangeRequested?.Invoke("Performance");
            var profileBalanced = new MenuItem { Header = Strings.TrayIcon_ProfileBalanced };
            profileBalanced.Click += (s, e) => QuickProfileChangeRequested?.Invoke("Balanced");
            var profileQuiet = new MenuItem { Header = Strings.TrayIcon_ProfileQuiet };
            profileQuiet.Click += (s, e) => QuickProfileChangeRequested?.Invoke("Quiet");
            var profileCustom = new MenuItem { Header = Strings.TrayIcon_ProfileCustom };
            profileCustom.Click += (s, e) => QuickProfileChangeRequested?.Invoke("Custom");

            quickProfileMenuItem.Items.Add(profilePerformance);
            quickProfileMenuItem.Items.Add(profileBalanced);
            quickProfileMenuItem.Items.Add(profileQuiet);
            quickProfileMenuItem.Items.Add(profileCustom);

            // Ensure submenu items use our dark MenuItem style
            quickProfileMenuItem.ItemContainerStyle = menuItemStyle;
            quickProfileMenuItem.SubmenuOpened += (s, e) =>
            {
                try
                {
                    quickProfileMenuItem.ApplyTemplate();
                    var popup = quickProfileMenuItem.Template.FindName("PART_Popup", quickProfileMenuItem) as System.Windows.Controls.Primitives.Popup;
                    if (popup?.Child != null)
                    {
                        if (popup.Child is System.Windows.Controls.Border b)
                        {
                            b.Background = contextMenu.Background;
                            if (b.Child is System.Windows.Controls.Control innerCtrl)
                                innerCtrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                        else if (popup.Child is System.Windows.Controls.Control ctrl)
                        {
                            ctrl.Background = contextMenu.Background;
                            ctrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                    }
                }
                catch (Exception ex) { App.Logging.Warn($"[Tray] Submenu style error: {ex.Message}"); }
            };

            contextMenu.Items.Add(quickProfileMenuItem);

            // Single-click Max Fan shortcut — previously only reachable via Advanced > Fan
            // Control > Max (three menu levels deep). Kept alongside that entry rather than
            // replacing it; this mirrors how OGH exposes a single-click Max option.
            var maxFanQuickItem = new MenuItem { Header = Strings.TrayIcon_MaxFan };
            maxFanQuickItem.Click += (s, e) => SetFanMode("Max");
            contextMenu.Items.Add(maxFanQuickItem);

            // Recent Notifications — surfaces toast history the user may have missed/dismissed.
            // Rebuilt from NotificationService.InAppNotifications each time the submenu opens,
            // matching the lazy-refresh pattern used by the other dynamic submenus below.
            if (_notificationService != null)
            {
                var notificationsMenuItem = new MenuItem { Header = Strings.TrayIcon_RecentNotifications };
                notificationsMenuItem.ItemContainerStyle = menuItemStyle;
                notificationsMenuItem.SubmenuOpened += (s, e) =>
                {
                    notificationsMenuItem.Items.Clear();

                    var recent = _notificationService.GetRecentNotifications(10).ToList();
                    if (recent.Count == 0)
                    {
                        notificationsMenuItem.Items.Add(new MenuItem { Header = Strings.TrayIcon_NoNotifications, IsEnabled = false });
                    }
                    else
                    {
                        foreach (var n in recent)
                        {
                            var item = new MenuItem
                            {
                                Header = $"{n.Title} — {FormatNotificationAge(n.Timestamp)}",
                                ToolTip = n.Message,
                                FontWeight = n.IsRead ? FontWeights.Normal : FontWeights.Bold
                            };
                            var id = n.Id;
                            item.Click += (s2, e2) => _notificationService.MarkAsRead(id);
                            notificationsMenuItem.Items.Add(item);
                        }

                        notificationsMenuItem.Items.Add(new Separator());
                        var clearItem = new MenuItem { Header = Strings.TrayIcon_ClearAll };
                        clearItem.Click += (s2, e2) => _notificationService.ClearAllNotifications();
                        notificationsMenuItem.Items.Add(clearItem);
                    }

                    try
                    {
                        notificationsMenuItem.ApplyTemplate();
                        var popup = notificationsMenuItem.Template.FindName("PART_Popup", notificationsMenuItem) as System.Windows.Controls.Primitives.Popup;
                        if (popup?.Child != null)
                        {
                            if (popup.Child is System.Windows.Controls.Border b)
                            {
                                b.Background = contextMenu.Background;
                                if (b.Child is System.Windows.Controls.Control innerCtrl)
                                    innerCtrl.Foreground = (Brush)contextMenu.Foreground;
                            }
                            else if (popup.Child is System.Windows.Controls.Control ctrl)
                            {
                                ctrl.Background = contextMenu.Background;
                                ctrl.Foreground = (Brush)contextMenu.Foreground;
                            }
                        }
                    }
                    catch (Exception ex) { App.Logging.Warn($"[Tray] Notifications submenu style error: {ex.Message}"); }
                };
                contextMenu.Items.Add(notificationsMenuItem);
            }

            // Report a Problem — always-visible entry point for diagnostics export, independent
            // of Lite/Advanced mode. Previously only reachable via Settings (gated behind
            // ShowAdvancedControls/non-Lite mode) or a command buried in SettingsView.xaml.
            var reportProblemItem = new MenuItem { Header = Strings.TrayIcon_ReportProblem };
            reportProblemItem.Click += (s, e) => ReportProblemRequested?.Invoke();
            contextMenu.Items.Add(reportProblemItem);

            // ═══ ADVANCED CONTROLS ═══
            var advancedMenuItem = new MenuItem { Header = Strings.TrayIcon_Advanced };

            // Fan submenu
            _fanModeMenuItem = new MenuItem { Header = Strings.TrayIcon_FanControl };
            
            _fanAutoMenuItem = new MenuItem { Header = "✓" + Strings.TrayIcon_FanAutoDesc };
            _fanAutoMenuItem.Click += (s, e) => SetFanMode("Auto");
            _fanMaxMenuItem = new MenuItem { Header = "  " + Strings.TrayIcon_FanMaxDesc };
            _fanMaxMenuItem.Click += (s, e) => SetFanMode("Max");
            _fanQuietMenuItem = new MenuItem { Header = "  " + Strings.TrayIcon_FanQuietDesc };
            _fanQuietMenuItem.Click += (s, e) => SetFanMode("Quiet");
            
            _fanModeMenuItem.Items.Add(_fanAutoMenuItem);
            _fanModeMenuItem.Items.Add(_fanMaxMenuItem);
            _fanModeMenuItem.Items.Add(_fanQuietMenuItem);
            _fanModeMenuItem.ItemContainerStyle = menuItemStyle;
            _fanModeMenuItem.SubmenuOpened += (s, e) =>
            {
                try
                {
                    _fanModeMenuItem.ApplyTemplate();
                    var popup = _fanModeMenuItem.Template.FindName("PART_Popup", _fanModeMenuItem) as System.Windows.Controls.Primitives.Popup;
                    if (popup?.Child != null)
                    {
                        if (popup.Child is System.Windows.Controls.Border b)
                        {
                            b.Background = contextMenu.Background;
                            if (b.Child is System.Windows.Controls.Control innerCtrl)
                                innerCtrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                        else if (popup.Child is System.Windows.Controls.Control ctrl)
                        {
                            ctrl.Background = contextMenu.Background;
                            ctrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                    }
                }
                catch (Exception ex) { App.Logging.Warn($"[Tray] Fan submenu style error: {ex.Message}"); }
            };
            advancedMenuItem.Items.Add(_fanModeMenuItem);

            // Performance submenu
            _performanceModeMenuItem = new MenuItem { Header = Strings.TrayIcon_PowerProfile };
            
            _perfQuietMenuItem = new MenuItem { Header = "  " + Strings.TrayIcon_PerfQuietLabel };
            _perfQuietMenuItem.Click += (s, e) => SetPerformanceMode("Quiet");
            _perfBalancedMenuItem = new MenuItem { Header = "✓" + Strings.TrayIcon_PerfBalanced };
            _perfBalancedMenuItem.Click += (s, e) => SetPerformanceMode("Balanced");
            _perfPerformanceMenuItem = new MenuItem { Header = "  " + Strings.TrayIcon_PerfPerformance };
            _perfPerformanceMenuItem.Click += (s, e) => SetPerformanceMode("Performance");
            
            _performanceModeMenuItem.Items.Add(_perfQuietMenuItem);
            _performanceModeMenuItem.Items.Add(_perfBalancedMenuItem);
            _performanceModeMenuItem.Items.Add(_perfPerformanceMenuItem);
            _performanceModeMenuItem.ItemContainerStyle = menuItemStyle;
            _performanceModeMenuItem.SubmenuOpened += (s, e) =>
            {
                try
                {
                    _performanceModeMenuItem.ApplyTemplate();
                    var popup = _performanceModeMenuItem.Template.FindName("PART_Popup", _performanceModeMenuItem) as System.Windows.Controls.Primitives.Popup;
                    if (popup?.Child != null)
                    {
                        if (popup.Child is System.Windows.Controls.Border b)
                        {
                            b.Background = contextMenu.Background;
                            if (b.Child is System.Windows.Controls.Control innerCtrl)
                                innerCtrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                        else if (popup.Child is System.Windows.Controls.Control ctrl)
                        {
                            ctrl.Background = contextMenu.Background;
                            ctrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                    }
                }
                catch (Exception ex) { App.Logging.Warn($"[Tray] Performance submenu style error: {ex.Message}"); }
            };
            advancedMenuItem.Items.Add(_performanceModeMenuItem);

            // Display submenu
            _displayMenuItem = new MenuItem { Header = Strings.TrayIcon_Display };

            var refreshHigh = new MenuItem { Header = Strings.TrayIcon_RefreshHigh };
            refreshHigh.Click += (s, e) => SetHighRefreshRate();
            var refreshLow = new MenuItem { Header = Strings.TrayIcon_RefreshLow };
            refreshLow.Click += (s, e) => SetLowRefreshRate();
            var refreshToggle = new MenuItem { Header = Strings.TrayIcon_ToggleRefresh };
            refreshToggle.Click += (s, e) => ToggleRefreshRate();
            
            _displayMenuItem.Items.Add(refreshHigh);
            _displayMenuItem.Items.Add(refreshLow);
            _displayMenuItem.Items.Add(refreshToggle);
            _displayMenuItem.Items.Add(new Separator());

            var displayOff = new MenuItem { Header = Strings.TrayIcon_TurnOffDisplay };
            displayOff.Click += (s, e) => TurnOffDisplay();
            _displayMenuItem.Items.Add(displayOff);

            _displayMenuItem.ItemContainerStyle = menuItemStyle;
            _displayMenuItem.SubmenuOpened += (s, e) =>
            {
                try
                {
                    _displayMenuItem.ApplyTemplate();
                    var popup = _displayMenuItem.Template.FindName("PART_Popup", _displayMenuItem) as System.Windows.Controls.Primitives.Popup;
                    if (popup?.Child != null)
                    {
                        if (popup.Child is System.Windows.Controls.Border b)
                        {
                            b.Background = contextMenu.Background;
                            if (b.Child is System.Windows.Controls.Control innerCtrl)
                                innerCtrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                        else if (popup.Child is System.Windows.Controls.Control ctrl)
                        {
                            ctrl.Background = contextMenu.Background;
                            ctrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                    }
                }
                catch (Exception ex) { App.Logging.Warn($"[Tray] Display submenu style error: {ex.Message}"); }
            };

            advancedMenuItem.Items.Add(_displayMenuItem);
            
            // GPU Power submenu (v2.7.0)
            _gpuPowerMenuItem = new MenuItem { Header = Strings.TrayIcon_GpuPower };
            
            _gpuPowerMinMenuItem = new MenuItem { Header = "  " + Strings.TrayIcon_GpuPowerMin };
            _gpuPowerMinMenuItem.Click += (s, e) => RequestGpuPowerChange("Minimum");
            _gpuPowerMedMenuItem = new MenuItem { Header = "✓" + Strings.TrayIcon_GpuPowerMed };
            _gpuPowerMedMenuItem.Click += (s, e) => RequestGpuPowerChange("Medium");
            _gpuPowerMaxMenuItem = new MenuItem { Header = "  " + Strings.TrayIcon_GpuPowerMax };
            _gpuPowerMaxMenuItem.Click += (s, e) => RequestGpuPowerChange("Maximum");
            // Same list as the Tuning page offers; without it a profile set to Extended showed Medium here (#142).
            _gpuPowerExtMenuItem = new MenuItem { Header = GpuPowerExtendedHeader(_currentGpuPowerLevel) };
            _gpuPowerExtMenuItem.Click += (s, e) => RequestGpuPowerChange("Extended");
            
            _gpuPowerMenuItem.Items.Add(_gpuPowerMinMenuItem);
            _gpuPowerMenuItem.Items.Add(_gpuPowerMedMenuItem);
            _gpuPowerMenuItem.Items.Add(_gpuPowerMaxMenuItem);
            _gpuPowerMenuItem.Items.Add(_gpuPowerExtMenuItem);
            _gpuPowerMenuItem.ItemContainerStyle = menuItemStyle;
            _gpuPowerMenuItem.SubmenuOpened += (s, e) =>
            {
                try
                {
                    _gpuPowerMenuItem.ApplyTemplate();
                    var popup = _gpuPowerMenuItem.Template.FindName("PART_Popup", _gpuPowerMenuItem) as System.Windows.Controls.Primitives.Popup;
                    if (popup?.Child != null)
                    {
                        if (popup.Child is System.Windows.Controls.Border b)
                        {
                            b.Background = contextMenu.Background;
                            if (b.Child is System.Windows.Controls.Control innerCtrl)
                                innerCtrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                        else if (popup.Child is System.Windows.Controls.Control ctrl)
                        {
                            ctrl.Background = contextMenu.Background;
                            ctrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                    }
                }
                catch (Exception ex) { App.Logging.Warn($"[Tray] GPU Power submenu style error: {ex.Message}"); }
            };
            advancedMenuItem.Items.Add(_gpuPowerMenuItem);
            
            // Keyboard backlight toggle (v2.7.0)
            _keyboardBacklightMenuItem = new MenuItem { Header = Strings.TrayIcon_KeyboardBacklight };
            
            var kbOff = new MenuItem { Header = "  " + Strings.TrayIcon_KbOff };
            kbOff.Click += (s, e) => RequestKeyboardBacklight(0);
            var kbLow = new MenuItem { Header = "  " + Strings.TrayIcon_KbLow };
            kbLow.Click += (s, e) => RequestKeyboardBacklight(1);
            var kbMed = new MenuItem { Header = "  " + Strings.TrayIcon_KbMedium };
            kbMed.Click += (s, e) => RequestKeyboardBacklight(2);
            var kbHigh = new MenuItem { Header = "✓" + Strings.TrayIcon_KbHigh };
            kbHigh.Click += (s, e) => RequestKeyboardBacklight(3);
            var kbToggle = new MenuItem { Header = Strings.TrayIcon_Toggle };
            kbToggle.Click += (s, e) => RequestKeyboardBacklightToggle();
            
            _keyboardBacklightMenuItem.Items.Add(kbOff);
            _keyboardBacklightMenuItem.Items.Add(kbLow);
            _keyboardBacklightMenuItem.Items.Add(kbMed);
            _keyboardBacklightMenuItem.Items.Add(kbHigh);
            _keyboardBacklightMenuItem.Items.Add(new Separator());
            _keyboardBacklightMenuItem.Items.Add(kbToggle);
            _keyboardBacklightMenuItem.ItemContainerStyle = menuItemStyle;
            _keyboardBacklightMenuItem.SubmenuOpened += (s, e) =>
            {
                try
                {
                    _keyboardBacklightMenuItem.ApplyTemplate();
                    var popup = _keyboardBacklightMenuItem.Template.FindName("PART_Popup", _keyboardBacklightMenuItem) as System.Windows.Controls.Primitives.Popup;
                    if (popup?.Child != null)
                    {
                        if (popup.Child is System.Windows.Controls.Border b)
                        {
                            b.Background = contextMenu.Background;
                            if (b.Child is System.Windows.Controls.Control innerCtrl)
                                innerCtrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                        else if (popup.Child is System.Windows.Controls.Control ctrl)
                        {
                            ctrl.Background = contextMenu.Background;
                            ctrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                    }
                }
                catch (Exception ex) { App.Logging.Warn($"[Tray] Keyboard submenu style error: {ex.Message}"); }
            };
            advancedMenuItem.Items.Add(_keyboardBacklightMenuItem);
            
            advancedMenuItem.ItemContainerStyle = menuItemStyle;
            advancedMenuItem.SubmenuOpened += (s, e) =>
            {
                try
                {
                    advancedMenuItem.ApplyTemplate();
                    var popup = advancedMenuItem.Template.FindName("PART_Popup", advancedMenuItem) as System.Windows.Controls.Primitives.Popup;
                    if (popup?.Child != null)
                    {
                        if (popup.Child is System.Windows.Controls.Border b)
                        {
                            b.Background = contextMenu.Background;
                            if (b.Child is System.Windows.Controls.Control innerCtrl)
                                innerCtrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                        else if (popup.Child is System.Windows.Controls.Control ctrl)
                        {
                            ctrl.Background = contextMenu.Background;
                            ctrl.Foreground = (Brush)contextMenu.Foreground;
                        }
                    }
                }
                catch (Exception ex) { App.Logging.Warn($"[Tray] Advanced submenu style error: {ex.Message}"); }
            };
            
            contextMenu.Items.Add(advancedMenuItem);

            contextMenu.Items.Add(new Separator());

            // ═══ ACTIONS ═══
            var showItem = new MenuItem { Header = Strings.TrayIcon_OpenDashboard };
            showItem.Click += (s, e) => _showMainWindow();
            contextMenu.Items.Add(showItem);
            
            var stayOnTopEnabled = App.Configuration.Config.StayOnTop;
            _stayOnTopMenuItem = new MenuItem
            {
                Header = stayOnTopEnabled ? Strings.TrayIcon_StayOnTopOn : Strings.TrayIcon_StayOnTopOff,
                ToolTip = stayOnTopEnabled
                    ? Strings.TrayIcon_KeepAboveOn
                    : Strings.TrayIcon_KeepAboveOff
            };
            _stayOnTopMenuItem.Click += (s, e) => ToggleStayOnTop();
            contextMenu.Items.Add(_stayOnTopMenuItem);

            contextMenu.Items.Add(new Separator());

            var checkUpdateItem = new MenuItem { Header = Strings.TrayIcon_CheckUpdates };
            checkUpdateItem.Click += (s, e) => CheckForUpdatesRequested?.Invoke();
            contextMenu.Items.Add(checkUpdateItem);

            // Own separator: Exit is the one genuinely hard-to-undo action in this menu
            // (closes the whole app, drops the tray icon) - worth a beat of visual distance
            // from "Check for Updates" right above it, same convention most tray apps use.
            contextMenu.Items.Add(new Separator());

            var exitItem = new MenuItem { Header = Strings.TrayIcon_Exit };
            exitItem.Click += (s, e) => _shutdownApp();
            contextMenu.Items.Add(exitItem);

            _trayIcon.ContextMenu = contextMenu;
        }

        public void UpdateMonitoringSample(MonitoringSample? sample)
        {
            if (sample == null)
            {
                return;
            }

            _latestSample = sample;
            
            // Also update QuickPopup if visible
            if (_quickPopup?.IsVisible == true)
            {
                _quickPopup.UpdateMonitoringSample(sample);
            }
        }

        private void UpdateTrayDisplay(object? sender, EventArgs e)
        {
            if (_disposed || _latestSample == null)
            {
                var noDataTooltip = Strings.TrayIcon_NoDataTooltip;
                if (!string.Equals(_lastTooltipText, noDataTooltip, StringComparison.Ordinal))
                {
                    _trayIcon.ToolTipText = noDataTooltip;
                    _lastTooltipText = noDataTooltip;
                }

                if (!ReferenceEquals(_trayIcon.IconSource, _baseIconSource))
                {
                    _trayIcon.IconSource = _baseIconSource;
                }

                _lastRenderedBadgeTemperature = null;
                _lastRenderedTrayTempDisplayEnabled = false;
                return;
            }

            // Throttle updates to prevent flicker during system events (brightness keys, etc.)
            // Use Interlocked for thread-safe check-and-set
            var lastTicks = Interlocked.Read(ref _lastIconUpdateTicks);
            var timeSinceLastUpdate = (DateTime.UtcNow.Ticks - lastTicks) / TimeSpan.TicksPerMillisecond;
            if (Interlocked.CompareExchange(ref _isUpdatingIcon, 1, 0) != 0 || timeSinceLastUpdate < MinIconUpdateIntervalMs)
            {
                return;
            }

            try
            {
                var cpuTemp = _latestSample.CpuTemperatureC;
                var gpuTemp = _latestSample.GpuTemperatureC;
                var cpuLoad = _latestSample.CpuLoadPercent;
                var gpuLoad = _latestSample.GpuLoadPercent;

                var memUsedGb = _latestSample.RamUsageGb;
                var memTotalGb = _latestSample.RamTotalGb;
                var memPercent = memTotalGb > 0 ? (memUsedGb * 100.0 / memTotalGb) : 0;
                
                var cpuTempStr = cpuTemp > 0 ? $"{cpuTemp:F0}°C" : "—°C";
                var gpuTempStr = gpuTemp > 0 ? $"{gpuTemp:F0}°C" : "—°C";
                
                var fan1Rpm = _latestSample.Fan1Rpm;
                var fan2Rpm = _latestSample.Fan2Rpm;
                var fanLine = fan2Rpm > 0 
                    ? string.Format(Strings.TrayIcon_FanRpmDual, fan1Rpm, fan2Rpm) 
                    : (fan1Rpm > 0 ? string.Format(Strings.TrayIcon_FanRpmSingle, fan1Rpm) : Strings.TrayIcon_FanNone);
                    
                var gpuPower = _latestSample.GpuPowerWatts;
                var gpuPowerDisplay = gpuPower > 0 ? $" · {gpuPower:F0}W" : "";
                
                var batteryPercent = _latestSample.BatteryChargePercent;
                var isAc = _latestSample.IsOnAcPower;
                var powerLine = batteryPercent > 0 || isAc
                    ? $"{batteryPercent:F0}% · {(isAc ? Strings.TrayIcon_AcPower : Strings.TrayIcon_Battery)}"
                    : "";
                
                var toolTipText = $"OmenCore v{_appVersion}\n" +
                                  $"━━━━━━━━━━━━━━━━━━\n" +
                                  string.Format(Strings.TrayIcon_TooltipCpuFormat, cpuTempStr, cpuLoad.ToString("F0")) + "\n" +
                                  string.Format(Strings.TrayIcon_TooltipGpuFormat, gpuTempStr, gpuLoad.ToString("F0"), gpuPowerDisplay) + "\n" +
                                  string.Format(Strings.TrayIcon_RamFormat, memUsedGb.ToString("F1"), memTotalGb.ToString("F1"), memPercent.ToString("F0")) + "\n" +
                                  $"{fanLine} | {MapPerfMode(_currentPerformanceMode)}\n" +
                                  string.Format(Strings.TrayIcon_TooltipFanPerfFormat, _linkFanToPerformanceMode ? Strings.TrayIcon_Linked : Strings.TrayIcon_Decoupled) + "\n" +
                                  (powerLine.Length > 0 ? $"{powerLine}\n" : "") +
                                  string.Format(Strings.TrayIcon_MonitorFormat, MapHealth(_monitoringHealth)) + "\n" +
                                  $"━━━━━━━━━━━━━━━━━━\n" +
                                  Strings.TrayIcon_LeftClickOpen;
                if (!string.Equals(_lastTooltipText, toolTipText, StringComparison.Ordinal))
                {
                    _trayIcon.ToolTipText = toolTipText;
                    _lastTooltipText = toolTipText;
                }

                SetMenuHeaderIfChanged(_cpuTempMenuItem, string.Format(Strings.TrayIcon_CpuTempFormat, cpuTempStr, cpuLoad.ToString("F0")));
                SetMenuHeaderIfChanged(_gpuTempMenuItem, string.Format(Strings.TrayIcon_GpuTempFormat, gpuTempStr, gpuLoad.ToString("F0")));
                SetMenuHeaderIfChanged(_monitoringHealthMenuItem, BuildMonitoringHealthHeaderText(_monitoringHealth));
                SetMenuHeaderIfChanged(_ramMenuItem, string.Format(Strings.TrayIcon_RamFormat, memUsedGb.ToString("F1"), memTotalGb.ToString("F1"), memPercent.ToString("F0")));

                var fanRpmStr = fan2Rpm > 0
                    ? $"CPU {fan1Rpm} · GPU {fan2Rpm} RPM"
                    : (fan1Rpm > 0 ? $"{fan1Rpm} RPM" : "—");
                SetMenuHeaderIfChanged(_fanStatusMenuItem, string.Format(Strings.TrayIcon_FanFormat, MapFanMode(_currentFanMode), fanRpmStr));

                if (_batteryMenuItem != null)
                {
                    if (batteryPercent > 0 || isAc)
                    {
                        SetMenuHeaderIfChanged(_batteryMenuItem, $"{batteryPercent:F0}% · {(isAc ? Strings.TrayIcon_Charging : Strings.TrayIcon_OnBattery)}");
                        SetMenuVisibilityIfChanged(_batteryMenuItem, Visibility.Visible);
                    }
                    else
                    {
                        SetMenuVisibilityIfChanged(_batteryMenuItem, Visibility.Collapsed);
                    }
                }

                var showTempOnTray = _configService?.Config?.Features?.TrayTempDisplayEnabled ?? true;
                UpdateTrayRefreshTimerDescription(showTempOnTray);
                if (showTempOnTray)
                {
                    var maxTemp = Math.Max(cpuTemp, gpuTemp);
                    var badgeTemperature = (int)Math.Round(maxTemp);
                    if (_lastRenderedTrayTempDisplayEnabled != true || _lastRenderedBadgeTemperature != badgeTemperature)
                    {
                           // v3.6.2: Cache miss - render state changed, requires re-render
                           RuntimeUiPerformanceCounters.RecordTrayRenderCacheMiss();
                        var badge = CreateTempIcon(maxTemp);
                        if (badge != null)
                        {
                            _trayIcon.IconSource = badge;
                            _lastRenderedBadgeTemperature = badgeTemperature;
                            _lastRenderedTrayTempDisplayEnabled = true;
                        }
                    }
                       else
                       {
                           // v3.6.2: Cache hit - render state unchanged, no re-render needed
                           RuntimeUiPerformanceCounters.RecordTrayRenderCacheHit();
                       }
                }
                else if (_lastRenderedTrayTempDisplayEnabled != false || !ReferenceEquals(_trayIcon.IconSource, _baseIconSource))
                {
                       // v3.6.2: Cache miss - render state changed (disabling or switching icon)
                       RuntimeUiPerformanceCounters.RecordTrayRenderCacheMiss();
                    _trayIcon.IconSource = _baseIconSource;
                    _lastRenderedBadgeTemperature = null;
                    _lastRenderedTrayTempDisplayEnabled = false;
                }
                   else
                   {
                       // v3.6.2: Cache hit - render state unchanged (both disabled)
                       RuntimeUiPerformanceCounters.RecordTrayRenderCacheHit();
                   }
            }
            catch (Exception ex)
            {
                App.Logging.Warn($"Failed to update tray display: {ex.Message}");
            }
            finally
            {
                Interlocked.Exchange(ref _lastIconUpdateTicks, DateTime.UtcNow.Ticks);
                Interlocked.Exchange(ref _isUpdatingIcon, 0);
            }
        }

        private void SetFanMode(string mode)
        {
            _pendingFanModeRequest = mode;
            if (_fanModeMenuItem != null)
            {
                _fanModeMenuItem.Header = BuildFanModeHeader();
            }

            FanModeChangeRequested?.Invoke(mode);
            App.Logging.Info($"Fan mode request from tray: requested={mode}, current={_currentFanMode}");
        }
        
        /// <summary>
        /// Update checkmarks on fan mode menu items (v2.6.1)
        /// </summary>
        private void UpdateFanModeCheckmarks(string activeMode)
        {
            var isAuto = FanModeNameResolver.IsAutoAlias(activeMode);
            var isMax = FanModeNameResolver.IsMaxAlias(activeMode);
            var isQuiet = FanModeNameResolver.IsQuietAlias(activeMode);

            if (_fanAutoMenuItem != null)
                _fanAutoMenuItem.Header = (isAuto ? "✓" : "  ") + Strings.TrayIcon_FanAutoDesc;
            if (_fanMaxMenuItem != null)
                _fanMaxMenuItem.Header = (isMax ? "✓" : "  ") + Strings.TrayIcon_FanMaxDesc;
            if (_fanQuietMenuItem != null)
                _fanQuietMenuItem.Header = (isQuiet ? "✓" : "  ") + Strings.TrayIcon_FanQuietDesc;
        }

        private void SetPerformanceMode(string mode)
        {
            _currentPerformanceMode = PerformanceModeNameResolver.Normalize(mode);
            if (_performanceModeMenuItem != null)
            {
                _performanceModeMenuItem.Header = BuildPerformanceModeHeaderText(_currentPerformanceMode);
            }
            
            // v2.6.1: Update checkmarks on performance menu items
            UpdatePerformanceModeCheckmarks(_currentPerformanceMode);
            
            PerformanceModeChangeRequested?.Invoke(_currentPerformanceMode);
            App.Logging.Info($"Performance mode changed from tray: {_currentPerformanceMode}");
        }
        
        /// <summary>
        /// Update checkmarks on performance mode menu items (v2.6.1)
        /// </summary>
        private void UpdatePerformanceModeCheckmarks(string activeMode)
        {
            var canonicalMode = PerformanceModeNameResolver.Normalize(activeMode);
            if (_perfBalancedMenuItem != null)
                _perfBalancedMenuItem.Header = (canonicalMode == "Balanced" ? "✓" : "  ") + Strings.TrayIcon_PerfBalanced;
            if (_perfPerformanceMenuItem != null)
                _perfPerformanceMenuItem.Header = (canonicalMode == "Performance" ? "✓" : "  ") + Strings.TrayIcon_PerfPerformance;
            if (_perfQuietMenuItem != null)
                _perfQuietMenuItem.Header = (canonicalMode == "Quiet" ? "✓" : "  ") + Strings.TrayIcon_PerfQuiet;
        }
        
        /// <summary>
        /// Request GPU power level change (v2.7.0)
        /// </summary>
        private void RequestGpuPowerChange(string level)
        {
            _currentGpuPowerLevel = level;
            UpdateGpuPowerCheckmarks(level);
            GpuPowerChangeRequested?.Invoke(level);
            App.Logging.Info($"GPU power level changed from tray: {level}");
        }
        
        /// <summary>
        /// Update GPU power level checkmarks (v2.7.0)
        /// </summary>
        private void UpdateGpuPowerCheckmarks(string activeLevel)
        {
            if (_gpuPowerMinMenuItem != null)
                _gpuPowerMinMenuItem.Header = (activeLevel == "Minimum" ? "✓" : "  ") + Strings.TrayIcon_GpuPowerMin;
            if (_gpuPowerMedMenuItem != null)
                _gpuPowerMedMenuItem.Header = (activeLevel == "Medium" ? "✓" : "  ") + Strings.TrayIcon_GpuPowerMed;
            if (_gpuPowerMaxMenuItem != null)
                _gpuPowerMaxMenuItem.Header = (activeLevel == "Maximum" ? "✓" : "  ") + Strings.TrayIcon_GpuPowerMax;
            if (_gpuPowerExtMenuItem != null)
                _gpuPowerExtMenuItem.Header = GpuPowerExtendedHeader(activeLevel);
        }

        /// <summary>Header text for the Extended tray item: ticked only when Extended is the active level.</summary>
        internal static string GpuPowerExtendedHeader(string activeLevel) =>
            (activeLevel == "Extended" ? "✓" : "  ") + Strings.TrayIcon_GpuPowerExt;
        
        /// <summary>
        /// Update GPU power level from external source (v2.7.0)
        /// </summary>
        public void SetGpuPowerLevel(string level)
        {
            _currentGpuPowerLevel = level;
            UpdateGpuPowerCheckmarks(level);
            if (_gpuPowerMenuItem != null)
            {
                _gpuPowerMenuItem.Header = string.Format(Strings.TrayIcon_GpuPowerFormat, level);
            }
        }
        
        /// <summary>
        /// Show or hide the GPU Power tray submenu based on whether the feature is supported on this hardware.
        /// Call after SystemControlViewModel.DetectGpuPowerBoost() completes — hides the submenu on HP Victus
        /// and other models where SupportsGpuPowerBoost = false, preventing confusing no-op menu items.
        /// </summary>
        public void SetGpuPowerAvailable(bool available)
        {
            Application.Current?.Dispatcher?.BeginInvoke(() =>
            {
                if (_gpuPowerMenuItem != null)
                    _gpuPowerMenuItem.Visibility = available ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        
        /// <summary>
        /// Request keyboard backlight level change (v2.7.0)
        /// </summary>
        private void RequestKeyboardBacklight(int level)
        {
            _currentKeyboardBrightness = level;
            KeyboardBacklightChangeRequested?.Invoke(level);
            App.Logging.Info($"Keyboard backlight changed from tray: level {level}");
        }
        
        /// <summary>
        /// Request keyboard backlight toggle (v2.7.0)
        /// </summary>
        private void RequestKeyboardBacklightToggle()
        {
            KeyboardBacklightToggleRequested?.Invoke();
            App.Logging.Info("Keyboard backlight toggle requested from tray");
        }
        
        /// <summary>
        /// Update keyboard brightness from external source (v2.7.0)
        /// </summary>
        public void SetKeyboardBrightness(int level)
        {
            _currentKeyboardBrightness = level;
            if (_keyboardBacklightMenuItem != null)
            {
                string levelName = level switch
                {
                    0 => Strings.TrayIcon_KbOff,
                    1 => Strings.TrayIcon_KbLow,
                    2 => Strings.TrayIcon_KbMedium,
                    _ => Strings.TrayIcon_KbHigh
                };
                _keyboardBacklightMenuItem.Header = string.Format(Strings.TrayIcon_KeyboardBacklightFormat, levelName);
            }
        }

        private string GetRefreshRateDisplay()
        {
            try
            {
                var rate = _displayService.GetCurrentRefreshRate(_displayService.ResolveLaptopPanelTarget());
                return rate > 0 ? $"{rate}Hz" : Strings.TrayIcon_Display;
            }
            catch
            {
                return Strings.TrayIcon_Display;
            }
        }

        private void SetHighRefreshRate()
        {
            if (_displayService.SetHighRefreshRate())
            {
                App.Logging.Info("✓ Switched to high refresh rate from tray");
                UpdateRefreshRateMenuItem();
            }
        }

        private void SetLowRefreshRate()
        {
            if (_displayService.SetLowRefreshRate())
            {
                App.Logging.Info("✓ Switched to low refresh rate from tray");
                UpdateRefreshRateMenuItem();
            }
        }

        private void ToggleRefreshRate()
        {
            var newRate = _displayService.ToggleRefreshRate();
            if (newRate > 0)
            {
                App.Logging.Info($"✓ Toggled refresh rate to {newRate}Hz from tray");
                UpdateRefreshRateMenuItem();
            }
        }
        
        /// <summary>
        /// Update the display menu item header to show current refresh rate.
        /// </summary>
        private void UpdateRefreshRateMenuItem()
        {
            if (_displayMenuItem == null) return;
            
            try
            {
                _displayMenuItem.Header = string.Format(Strings.TrayIcon_DisplayFormat, GetRefreshRateDisplay());
            }
            catch (Exception ex)
            {
                App.Logging.Warn($"Failed to update refresh rate menu item: {ex.Message}");
            }
        }

        private void TurnOffDisplay()
        {
            _displayService.TurnOffDisplay();
        }

        private void ToggleStayOnTop()
        {
            var newValue = !App.Configuration.Config.StayOnTop;
            App.Configuration.Config.StayOnTop = newValue;
            App.Configuration.Save(App.Configuration.Config);
            
            // Update the menu item
            if (_stayOnTopMenuItem != null)
            {
                _stayOnTopMenuItem.Header = newValue ? Strings.TrayIcon_StayOnTopOn : Strings.TrayIcon_StayOnTopOff;
                _stayOnTopMenuItem.ToolTip = newValue
                    ? Strings.TrayIcon_KeepAboveOn
                    : Strings.TrayIcon_KeepAboveOff;
            }
            
            // Notify the main window to update
            StayOnTopChanged?.Invoke(newValue);
            App.Logging.Info($"Stay on Top: {(newValue ? Strings.TrayIcon_Enabled : "Disabled")}");
        }

        public event Action<bool>? StayOnTopChanged;

        public void UpdateFanMode(string mode)
        {
            var normalizedMode = NormalizeFanModeForDisplay(mode);
            if (string.Equals(_currentFanMode, normalizedMode, StringComparison.Ordinal) && _pendingFanModeRequest == null)
            {
                return;
            }

            _currentFanMode = normalizedMode;
            _pendingFanModeRequest = null;
            Application.Current?.Dispatcher?.BeginInvoke(() =>
            {
                UpdateFanModeCheckmarks(normalizedMode);

                if (_fanModeMenuItem != null)
                {
                    _fanModeMenuItem.Header = BuildFanModeHeader();
                }

                _quickPopup?.UpdateFanMode(normalizedMode);
            });
        }

        public void UpdateCurvePresetName(string? presetName)
        {
            if (string.Equals(_curvePresetName, presetName, StringComparison.Ordinal))
            {
                return;
            }

            _curvePresetName = presetName;
            Application.Current?.Dispatcher?.BeginInvoke(() =>
            {
                _quickPopup?.UpdateCurvePresetName(presetName);
            });
        }

        // The main window calls the unapplied or firmware-default state "Default" (#199, #142). Normalizing it to Balanced for the
        // checkmarks is right, but the header must not claim Balanced is what the firmware is running.
        private bool _performanceIsFirmwareDefault;

        public void UpdatePerformanceMode(string mode)
        {
            var normalizedMode = PerformanceModeNameResolver.Normalize(mode);
            var firmwareDefault = IsFirmwareDefaultLabel(mode);
            if (string.Equals(_currentPerformanceMode, normalizedMode, StringComparison.Ordinal) &&
                firmwareDefault == _performanceIsFirmwareDefault)
            {
                return;
            }

            _performanceIsFirmwareDefault = firmwareDefault;
            _currentPerformanceMode = normalizedMode;
            Application.Current?.Dispatcher?.BeginInvoke(() =>
            {
                if (_performanceModeMenuItem != null)
                {
                    _performanceModeMenuItem.Header = _performanceIsFirmwareDefault
                        ? BuildPerformanceModeHeaderText(Strings.Tray_PerfHeaderFirmwareDefault)
                        : BuildPerformanceModeHeaderText(_currentPerformanceMode);
                }

                UpdatePerformanceModeCheckmarks(_currentPerformanceMode);

                _quickPopup?.UpdatePerformanceMode(_currentPerformanceMode);
            });
        }

        public void UpdateLinkedMode(bool linked)
        {
            if (_linkFanToPerformanceMode == linked)
            {
                return;
            }

            _linkFanToPerformanceMode = linked;
            Application.Current?.Dispatcher?.BeginInvoke(() =>
            {
                if (_fanModeMenuItem != null)
                {
                    _fanModeMenuItem.Header = BuildFanModeHeader();
                }

                if (_quickPopup != null)
                {
                    _quickPopup.UpdateLinkedMode(linked);
                }
            });
        }

        public void UpdateMonitoringHealth(MonitoringHealthStatus healthStatus)
        {
            var normalizedHealth = healthStatus switch
            {
                MonitoringHealthStatus.Healthy => "Healthy",
                MonitoringHealthStatus.Degraded => "Degraded",
                MonitoringHealthStatus.Stale => Strings.TrayIcon_HealthStale,
                _ => "Unknown"
            };

            if (string.Equals(_monitoringHealth, normalizedHealth, StringComparison.Ordinal))
            {
                return;
            }

            _monitoringHealth = normalizedHealth;

            Application.Current?.Dispatcher?.BeginInvoke(() =>
            {
                if (_monitoringHealthMenuItem != null)
                {
                    _monitoringHealthMenuItem.Header = BuildMonitoringHealthHeaderText(_monitoringHealth);
                }

                if (_quickPopup != null)
                {
                    _quickPopup.UpdateMonitoringHealth(_monitoringHealth);
                }
            });
        }

        private string BuildFanModeHeader() =>
            BuildFanModeHeaderText(_currentFanMode, _pendingFanModeRequest, _linkFanToPerformanceMode);

        private static string NormalizeFanModeForDisplay(string? mode)
        {
            if (FanModeNameResolver.IsMaxAlias(mode))
            {
                return "Max";
            }

            if (FanModeNameResolver.IsQuietAlias(mode))
            {
                return "Quiet";
            }

            if (FanModeNameResolver.IsAutoAlias(mode))
            {
                return "Auto";
            }

            if (FanModeNameResolver.IsCustomAlias(mode))
            {
                return "Custom";
            }

            return string.IsNullOrWhiteSpace(mode) ? "Auto" : mode.Trim();
        }

        /// <summary>
        /// Pure helper — separated so it can be tested without WPF dependencies.
        /// </summary>
        public static string BuildFanModeHeaderText(string currentMode, string? pendingRequest, bool linked)
        {
            var normalizedCurrentMode = string.IsNullOrWhiteSpace(currentMode) ? "Unknown" : currentMode.Trim();
            var normalizedPendingRequest = string.IsNullOrWhiteSpace(pendingRequest) ? null : pendingRequest.Trim();
            var suffix = linked ? Strings.TrayIcon_FanModeLinkedSuffix : string.Empty;
            if (normalizedPendingRequest != null &&
                !string.Equals(normalizedPendingRequest, normalizedCurrentMode, StringComparison.OrdinalIgnoreCase))
            {
                return string.Format(Strings.TrayIcon_FanModeRequestedFormat, MapFanMode(normalizedCurrentMode), suffix, MapFanMode(normalizedPendingRequest));
            }

            return string.Format(Strings.TrayIcon_FanModeFormat, MapFanMode(normalizedCurrentMode), suffix);
        }

        /// <summary>
        /// Pure helper for tray performance mode header formatting.
        /// </summary>
        internal static bool IsFirmwareDefaultLabel(string? mode) =>
            string.Equals(mode?.Trim(), "Default", StringComparison.OrdinalIgnoreCase);

        public static string BuildPerformanceModeHeaderText(string currentMode)
        {
            var normalizedCurrentMode = string.IsNullOrWhiteSpace(currentMode) ? "Unknown" : currentMode.Trim();
            return string.Format(Strings.TrayIcon_PerfFormat, MapPerfMode(normalizedCurrentMode));
        }

        /// <summary>
        /// Pure helper for tray monitoring-health header formatting.
        /// </summary>
        public static string BuildMonitoringHealthHeaderText(string monitoringHealth)
        {
            var normalizedHealth = string.IsNullOrWhiteSpace(monitoringHealth) ? "Unknown" : monitoringHealth.Trim();
            return string.Format(Strings.TrayIcon_MonitorFormat, MapHealth(normalizedHealth));
        }

        private static string MapHealth(string? health) => health switch
        {
            "Healthy" => Strings.TrayIcon_HealthHealthy,
            "Degraded" => Strings.TrayIcon_HealthDegraded,
            "Stale" => Strings.TrayIcon_HealthStale,
            "Unknown" => Strings.TrayIcon_HealthUnknown,
            _ => health ?? string.Empty
        };

        private static string MapFanMode(string? mode) => mode switch
        {
            "Auto" => Strings.TrayIcon_ModeAuto,
            "Max" => Strings.TrayIcon_ModeMax,
            "Quiet" => Strings.TrayIcon_ModeQuiet,
            "Custom" => Strings.TrayIcon_ModeCustom,
            "Curve" => Strings.TrayIcon_ModeCurve,
            _ => mode ?? string.Empty
        };

        private static string MapPerfMode(string? mode) => mode switch
        {
            "Balanced" => Strings.PerformanceMode_Balanced,
            "Performance" => Strings.PerformanceMode_Performance,
            "Quiet" => Strings.PerformanceMode_Quiet,
            "Firmware default" => Strings.TrayIcon_PerfFirmware,
            "Unknown" => Strings.TrayIcon_HealthUnknown,
            _ => mode ?? string.Empty
        };

        private static void SetMenuHeaderIfChanged(MenuItem? menuItem, string header)
        {
            if (menuItem == null)
            {
                return;
            }

            var currentHeader = menuItem.Header?.ToString();
            if (!string.Equals(currentHeader, header, StringComparison.Ordinal))
            {
                menuItem.Header = header;
            }
        }

        private static void SetMenuVisibilityIfChanged(MenuItem? menuItem, Visibility visibility)
        {
            if (menuItem != null && menuItem.Visibility != visibility)
            {
                menuItem.Visibility = visibility;
            }
        }

        /// <summary>
        /// Shows or hides the quick popup window near the tray.
        /// </summary>
        public void ToggleQuickPopup()
        {
            Application.Current?.Dispatcher?.BeginInvoke(() =>
            {
                if (_configService?.Config.QuickPopupEnabled == false)
                {
                    _showMainWindow();
                    return;
                }

                if (_quickPopup == null)
                {
                    _quickPopup = new QuickPopupWindow();
                    _quickPopup.FanModeChangeRequested += mode => FanModeChangeRequested?.Invoke(mode);
                    _quickPopup.QuickProfileChangeRequested += profile => QuickProfileChangeRequested?.Invoke(profile);
                    _quickPopup.MemoryCleanRequested += () => MemoryCleanRequested?.Invoke();
                    _quickPopup.OpenDashboardRequested += () => _showMainWindow();
                    _quickPopup.Closed += (s, e) => _quickPopup = null;
                }

                if (_quickPopup.IsVisible)
                {
                    _quickPopup.Hide();
                }
                else
                {
                    _quickPopup.ConfigureQuickAction(_configService?.Config.QuickAccessAction);
                    _quickPopup.PositionNearTray();
                    _quickPopup.UpdateFanMode(_currentFanMode);
                    _quickPopup.UpdateCurvePresetName(_curvePresetName);
                    _quickPopup.UpdatePerformanceMode(_currentPerformanceMode);
                    _quickPopup.UpdateLinkedMode(_linkFanToPerformanceMode);
                    _quickPopup.UpdateMonitoringHealth(_monitoringHealth);
                    if (_latestSample != null)
                    {
                        _quickPopup.UpdateMonitoringSample(_latestSample);
                    }
                    _quickPopup.Show();
                    _quickPopup.Activate();
                }
            });
        }

        /// <summary>
        /// Shows the quick popup window near the tray.
        /// </summary>
        public void ShowQuickPopup()
        {
            Application.Current?.Dispatcher?.BeginInvoke(() =>
            {
                if (_configService?.Config.QuickPopupEnabled == false)
                {
                    _showMainWindow();
                    return;
                }

                if (_quickPopup == null)
                {
                    _quickPopup = new QuickPopupWindow();
                    _quickPopup.FanModeChangeRequested += mode => FanModeChangeRequested?.Invoke(mode);
                    _quickPopup.QuickProfileChangeRequested += profile => QuickProfileChangeRequested?.Invoke(profile);
                    _quickPopup.MemoryCleanRequested += () => MemoryCleanRequested?.Invoke();
                    _quickPopup.OpenDashboardRequested += () => _showMainWindow();
                    _quickPopup.Closed += (s, e) => _quickPopup = null;
                }

                _quickPopup.ConfigureQuickAction(_configService?.Config.QuickAccessAction);
                _quickPopup.PositionNearTray();
                _quickPopup.UpdateFanMode(_currentFanMode);
                _quickPopup.UpdateCurvePresetName(_curvePresetName);
                _quickPopup.UpdatePerformanceMode(_currentPerformanceMode);
                _quickPopup.UpdateLinkedMode(_linkFanToPerformanceMode);
                _quickPopup.UpdateMonitoringHealth(_monitoringHealth);
                if (_latestSample != null)
                {
                    _quickPopup.UpdateMonitoringSample(_latestSample);
                }
                _quickPopup.Show();
                _quickPopup.Activate();
            });
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _updateTimerSubscription.Dispose();
                BackgroundTimerRegistry.Unregister(TrayRefreshTimerRegistryName);
                _quickPopup?.Close();
                _disposed = true;
            }
        }

        private void RegisterTrayRefreshTimer()
        {
            var showTempOnTray = _configService?.Config?.Features?.TrayTempDisplayEnabled ?? true;
            _lastRegisteredTrayTempDisplayEnabled = showTempOnTray;
            BackgroundTimerRegistry.Register(
                TrayRefreshTimerRegistryName,
                nameof(TrayIconService),
                BuildTrayRefreshTimerDescription(showTempOnTray),
                TrayRefreshIntervalMs,
                BackgroundTimerTier.Optional);
        }

        private void UpdateTrayRefreshTimerDescription(bool? showTempOnTray = null, bool force = false)
        {
            var enabled = showTempOnTray ?? (_configService?.Config?.Features?.TrayTempDisplayEnabled ?? true);
            if (!force && _lastRegisteredTrayTempDisplayEnabled == enabled)
            {
                return;
            }

            _lastRegisteredTrayTempDisplayEnabled = enabled;
            BackgroundTimerRegistry.UpdateDescription(
                TrayRefreshTimerRegistryName,
                BuildTrayRefreshTimerDescription(enabled));
        }

        private static string BuildTrayRefreshTimerDescription(bool showTempOnTray) =>
            showTempOnTray
                ? "Tray tooltip/menu refresh plus live temperature badge redraw"
                : "Tray tooltip/menu refresh with static icon";

        private ImageSource? LoadBaseIcon()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/Assets/OmenCore.ico", UriKind.Absolute);
                var bitmap = new BitmapImage(uri);
                bitmap.Freeze();
                return bitmap;
            }
            catch (Exception ex)
            {
                App.Logging.Debug($"Failed to load tray base icon from embedded assets: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Creates a dynamic tray icon showing temperature with color-coded background.
        /// Colors optimized for gaming laptops where higher temps are normal:
        /// - Blue: < 50°C (idle/cool)
        /// - Green: 50-65°C (light load)
        /// - Yellow: 65-75°C (moderate load) 
        /// - Orange: 75-85°C (gaming/heavy load - normal for laptops)
        /// - Red: 85-95°C (very hot but within spec for gaming laptops)
        /// - Magenta: > 95°C (critical - approaching thermal throttle)
        /// </summary>
        private ImageSource? CreateTempIcon(double temp)
        {
            // System tray icons are rendered at 16x16 or 32x32 depending on DPI
            // We create at 32x32 for best quality at high DPI
            const int size = 32;
            var visual = new DrawingVisual();

            using (var dc = visual.RenderOpen())
            {
                // Temperature-based background color (adjusted for gaming laptops)
                Color bgColor;
                if (temp < 50)
                    bgColor = Color.FromRgb(0, 120, 220);      // Blue - Cool/Idle
                else if (temp < 65)
                    bgColor = Color.FromRgb(0, 180, 80);       // Green - Light load
                else if (temp < 75)
                    bgColor = Color.FromRgb(220, 200, 0);      // Yellow - Moderate load
                else if (temp < 85)
                    bgColor = Color.FromRgb(255, 140, 0);      // Orange - Gaming/Heavy (normal for laptops)
                else if (temp < 95)
                    bgColor = Color.FromRgb(255, 60, 60);      // Red - Very hot
                else
                    bgColor = Color.FromRgb(200, 0, 100);      // Magenta - Critical

                // Draw colored square background (fills more of the tray space)
                var background = new SolidColorBrush(bgColor);
                dc.DrawRoundedRectangle(background, null, new Rect(0, 0, size, size), 4, 4);

                // Draw temperature text - use largest font that fits
                var text = temp.ToString("F0");
                var fontSize = temp >= 100 ? 13 : 16; // Adjusted for 32x32 icon
                // Use dark text on light backgrounds (yellow at 65-75°C is the main offender).
                // Simple luminance estimate (non-linearized) sufficient for this purpose.
                double lum = 0.2126 * (bgColor.R / 255.0) + 0.7152 * (bgColor.G / 255.0) + 0.0722 * (bgColor.B / 255.0);
                var textBrush = lum > 0.45 ? Brushes.Black : Brushes.White;

                var formatted = new FormattedText(
                    text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(AppFonts.App, FontStyles.Normal, FontWeights.ExtraBold, FontStretches.Normal),
                    fontSize,
                    textBrush,
                    1.25);

                // Center the text
                var origin = new Point((size - formatted.Width) / 2, (size - formatted.Height) / 2);
                dc.DrawText(formatted, origin);
            }

            var rtb = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);
            rtb.Freeze();
            return rtb;
        }
    }
}
