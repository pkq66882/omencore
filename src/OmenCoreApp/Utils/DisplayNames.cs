using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using OmenCore.Localization;
using OmenCore.Models;
using OmenCore.Services;

namespace OmenCore.Utils
{
    /// <summary>
    /// Maps Core-side display names (RGB scenes, keyboard presets) and audio visualization
    /// modes to localized strings. Data names stay canonical English so scene/profile
    /// persistence and lookups are unaffected; only what the UI shows is translated.
    /// </summary>
    public static class DisplayNames
    {
        /// <summary>Localized GPU switch mode name for the selector and restart prompt.</summary>
        /// <summary>Localized label for a built-in fan preset name; custom preset names pass through.</summary>
        public static string FanPresetName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name ?? string.Empty;
            }

            if (FanModeNameResolver.IsAutoAlias(name)) return Strings.TrayIcon_ModeAuto;
            if (FanModeNameResolver.IsMaxAlias(name)) return Strings.TrayIcon_ModeMax;
            if (FanModeNameResolver.IsQuietAlias(name)) return Strings.TrayIcon_ModeQuiet;
            if (FanModeNameResolver.IsCustomAlias(name)) return Strings.TrayIcon_ModeCustom;

            // Built-in performance-flavoured presets (OEM presets, quick-popup curve names) still
            // need a Chinese label; Extreme keeps its own word so it does not read as "鎬ц兘".
            return name switch
            {
                "Extreme" => Strings.FanPreset_Extreme,
                "Performance" => Strings.FanPreset_Performance,
                "Turbo" => Strings.FanPreset_Performance,
                "Boost" => Strings.FanPreset_Performance,
                "Gaming" => Strings.FanPreset_Gaming,
                "Balanced" => Strings.FanPreset_Balanced,
                "Silent" => Strings.FanPreset_Silent,
                "Default" => Strings.FanPreset_Default,
                _ => name
            };
        }

        /// <summary>Localized label for a built-in performance mode name.</summary>
        public static string PerformanceModeName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name ?? string.Empty;
            }

            if (PerformanceModeNameResolver.IsQuietAlias(name)) return Strings.PerformanceMode_Quiet;
            if (PerformanceModeNameResolver.IsBalancedAlias(name)) return Strings.PerformanceMode_Balanced;
            if (PerformanceModeNameResolver.IsPerformanceAlias(name)) return Strings.PerformanceMode_Performance;
            return name;
        }
        public static string GpuSwitchName(GpuSwitchMode mode) => mode switch
        {
            GpuSwitchMode.Hybrid => Strings.GpuSwitch_ModeHybrid,
            GpuSwitchMode.Discrete => Strings.GpuSwitch_ModeDiscrete,
            GpuSwitchMode.Integrated => Strings.GpuSwitch_ModeIntegrated,
            _ => Strings.GpuSwitch_ModeUnknown
        };

        /// <summary>Localized fan name for diagnostic sentences and history rows.</summary>
        public static string FanDiagFanName(string? name) => name switch
        {
            "CPU" or "CPU Fan" => Strings.FanDiag_FanNameCpu,
            "GPU" or "GPU Fan" => Strings.FanDiag_FanNameGpu,
            "CPU Fan (est.)" => Strings.FanDiag_FanNameCpuEstimated,
            "GPU Fan (est.)" => Strings.FanDiag_FanNameGpuEstimated,
            _ => name ?? string.Empty
        };

        /// <summary>Localized RPM source name used in the diagnostic header.</summary>
        public static string FanDiagSource(string? source) => source switch
        {
            "EcDirect" => Strings.FanDiag_SourceEcDirect,
            "HardwareMonitor" => Strings.FanDiag_SourceHardwareMonitor,
            "Afterburner" => Strings.FanDiag_SourceAfterburner,
            "WmiBios" => Strings.FanDiag_SourceWmiBios,
            "Estimated" => Strings.FanDiag_SourceEstimated,
            "Unknown" => Strings.FanDiag_SourceUnknown,
            _ => source ?? string.Empty
        };

        /// <summary>Localizes Core's RpmDisplay sentence forms; unknown text passes through.</summary>
        public static string FanDiagRpm(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var m = Regex.Match(text, @"^~(\d+) RPM \(fan-level estimate\)$");
            if (m.Success) return string.Format(Strings.FanDiag_RpmEstimatedFormat, m.Groups[1].Value);
            m = Regex.Match(text, @"^firmware refused level (\d+)$");
            if (m.Success) return string.Format(Strings.FanDiag_RpmRefusedFormat, m.Groups[1].Value);
            if (text == "Physical RPM unavailable") return Strings.FanDiag_RpmUnavailable;
            m = Regex.Match(text, @"^(\d+) RPM \(source unverified\)$");
            if (m.Success) return string.Format(Strings.FanDiag_RpmUnverifiedFormat, m.Groups[1].Value);
            m = Regex.Match(text, @"^(\d+) RPM \(([^)]+)\)$");
            if (m.Success) return string.Format(Strings.FanDiag_RpmWithSourceFormat, m.Groups[1].Value, m.Groups[2].Value);
            return text;
        }

        /// <summary>Localized verification evidence token.</summary>
        public static string FanDiagEvidence(string? evidence) => evidence switch
        {
            null or "" or "None" => Strings.FanDiag_EvidenceNone,
            "RPM" => Strings.FanDiag_EvidenceRpm,
            "Level" => Strings.FanDiag_EvidenceLevel,
            "RPM+Level" => Strings.FanDiag_EvidenceRpmLevel,
            "FirmwareCeiling" => Strings.FanDiag_EvidenceFirmwareCeiling,
            _ => evidence
        };

        /// <summary>Localized verification score rating.</summary>
        public static string FanDiagRating(string? rating) => rating switch
        {
            "Excellent" => Strings.FanDiag_RatingExcellent,
            "Good" => Strings.FanDiag_RatingGood,
            "Fair" => Strings.FanDiag_RatingFair,
            "Poor" => Strings.FanDiag_RatingPoor,
            "Failed" => Strings.FanDiag_RatingFailed,
            _ => rating ?? string.Empty
        };

        /// <summary>Localizes the fixed diagnostic error sentences; dynamic detail stays as-is.</summary>
        public static string FanDiagError(string? error)
        {
            if (string.IsNullOrWhiteSpace(error)) return string.Empty;
            if (error == "No fan control backend available") return Strings.FanDiag_ErrNoBackend;
            if (error == "WMI call returned false") return Strings.FanDiag_ErrWmiFalse;
            if (error == "FanService call returned false") return Strings.FanDiag_ErrServiceFalse;
            var m = Regex.Match(error, @"^Fan verification remained inconclusive after (\d+) attempts: (.*)$", RegexOptions.Singleline);
            if (m.Success) return string.Format(Strings.FanDiag_ErrInconclusiveFormat, m.Groups[1].Value, m.Groups[2].Value);
            return error;
        }

        /// <summary>Localized automation trigger name.</summary>
        public static string AutomationTrigger(object? value) => value?.ToString() switch
        {
            "Time" => Strings.Trigger_Time,
            "Battery" => Strings.Trigger_Battery,
            "ACPower" => Strings.Trigger_AcPower,
            "Temperature" => Strings.Trigger_Temperature,
            "Idle" => Strings.Trigger_Idle,
            "Process" => Strings.Trigger_Process,
            "WiFiSSID" => Strings.Trigger_WifiSsid,
            "LidState" => Strings.Trigger_LidState,
            var s => s ?? string.Empty
        };

        /// <summary>Localizes automation rule validation messages; unknown text passes through.</summary>
        public static string AutomationError(string? error) => error switch
        {
            "Rule payload is missing." => Strings.Automation_ErrPayloadMissing,
            "Rule name is required." => Strings.Automation_ErrNameRequired,
            "At least one action is required." => Strings.Automation_ErrNeedsAction,
            "Time rules require both start and end time." => Strings.Automation_ErrTimeRange,
            "Battery rules require a threshold between 1 and 100." => Strings.Automation_ErrBatteryThreshold,
            "Battery rules require condition 'Below' or 'Above'." => Strings.Automation_ErrBatteryCondition,
            "AC power rules must specify connected or disconnected." => Strings.Automation_ErrAcState,
            "Temperature rules require a threshold between 1 and 110°C." => Strings.Automation_ErrTempThreshold,
            "Temperature rules require condition 'Above' or 'Below'." => Strings.Automation_ErrTempCondition,
            "Idle rules require a threshold between 1 and 999 minutes." => Strings.Automation_ErrIdleThreshold,
            "Process rules require an executable name." => Strings.Automation_ErrProcessName,
            "WiFi rules require an SSID." => Strings.Automation_ErrWifiSsid,
            "Lid state rules must specify open or closed." => Strings.Automation_ErrLidState,
            _ => error ?? string.Empty
        };

        /// <summary>Localized bloatware category name; unknown values pass through.</summary>
        public static string BloatwareCategory(object? value) => value?.ToString() switch
        {
            "OemSoftware" => Strings.BloatwareCategory_OemSoftware,
            "WindowsApps" => Strings.BloatwareCategory_WindowsApps,
            "Antivirus" => Strings.BloatwareCategory_Antivirus,
            "Games" => Strings.BloatwareCategory_Games,
            "Social" => Strings.BloatwareCategory_Social,
            "Communication" => Strings.BloatwareCategory_Communication,
            "Promotional" => Strings.BloatwareCategory_Promotional,
            "Telemetry" => Strings.BloatwareCategory_Telemetry,
            "Startup" => Strings.BloatwareCategory_Startup,
            "Unknown" or null => Strings.BloatwareCategory_Unknown,
            var s => s
        };

        /// <summary>Localized bloatware item type; unknown values pass through.</summary>
        public static string BloatwareType(object? value) => value?.ToString() switch
        {
            "AppxPackage" => Strings.BloatwareType_AppxPackage,
            "Win32App" => Strings.BloatwareType_Win32App,
            "StartupItem" => Strings.BloatwareType_StartupItem,
            "ScheduledTask" => Strings.BloatwareType_ScheduledTask,
            var s => s ?? string.Empty
        };

        /// <summary>Localized boost level name; unknown values pass through.</summary>
        public static string BoostLevel(string? level) => level switch
        {
            "Minimum" => Strings.GpuBoost_LevelMinimum,
            "Medium" => Strings.GpuBoost_LevelMedium,
            "Maximum" => Strings.GpuBoost_LevelMaximum,
            "Extended" => Strings.GpuBoost_LevelExtended,
            _ => level ?? string.Empty
        };

        /// <summary>
        /// Localized display names for settings dropdown tokens. The optional domain disambiguates
        /// values whose meaning differs per selector (Balanced as fan preset vs OSD density vs
        /// clean profile). Stored values stay canonical ("WmiBios", "TopRight", ...).
        /// </summary>
        public static string SettingsToken(string? token, string? domain = null) => domain switch
        {
            "FanPreset" => token switch
            {
                "" or null => Strings.Settings_NotSet,
                "Auto" => Strings.FanPreset_Auto,
                "Quiet" => Strings.FanPreset_Quiet,
                "Extreme" => Strings.FanPreset_Extreme,
                "Max" => Strings.FanPreset_Max,
                "Default" => Strings.FanPreset_Default,
                "Silent" => Strings.FanPreset_Silent,
                "Balanced" => Strings.FanPreset_Balanced,
                "Performance" => Strings.FanPreset_Performance,
                "Gaming" => Strings.FanPreset_Gaming,
                _ => token ?? string.Empty
            },
            "PerfMode" => token switch
            {
                "" or null => Strings.Settings_NotSet,
                "Silent" => Strings.PerfMode_Silent,
                "Balanced" => Strings.PerfMode_Balanced,
                "Performance" => Strings.PerfMode_Performance,
                "Turbo" => Strings.PerfMode_Turbo,
                "Comfort" => Strings.PerfMode_Comfort,
                "Quiet" => Strings.PerfMode_Quiet,
                _ => token ?? string.Empty
            },
            "MaxAutoReturn" => token switch
            {
                "Off" => Strings.MaxAutoReturn_Off,
                "Cooldown" => Strings.MaxAutoReturn_Cooldown,
                "Timer" => Strings.MaxAutoReturn_Timer,
                _ => token ?? string.Empty
            },
            "RgbSurface" => token switch
            {
                "Keyboard zones changed" => Strings.RgbSurface_KeyboardZones,
                "Per-key keys changed" => Strings.RgbSurface_PerKey,
                "Light bar changed" => Strings.RgbSurface_LightBar,
                "Single backlight changed" => Strings.RgbSurface_SingleBacklight,
                "Nothing changed" => Strings.RgbSurface_NothingChanged,
                "Different or unknown surface changed" => Strings.RgbSurface_Other,
                _ => token ?? string.Empty
            },
            "KeyboardProfile" => token switch
            {
                "Default" => Strings.KeyboardProfile_Default,
                "Game Mode" => Strings.KeyboardProfile_GameMode,
                "Ambient" => Strings.KeyboardProfile_Ambient,
                "Custom 1" => Strings.KeyboardProfile_Custom1,
                "Custom 2" => Strings.KeyboardProfile_Custom2,
                _ => token ?? string.Empty
            },
            "Peripheral" => token switch
            {
                "Default" => Strings.Peripheral_Default,
                "RGB Wave" => Strings.Peripheral_RgbWave,
                "Static Blue" => Strings.Peripheral_StaticBlue,
                "Breathing" => Strings.Peripheral_Breathing,
                "Reactive" => Strings.Peripheral_Reactive,
                _ => token ?? string.Empty
            },
            "CleanProfile" => token switch
            {
                "Aggressive" => Strings.CleanProfile_Aggressive,
                "Balanced" => Strings.CleanProfile_Balanced,
                "Conservative" => Strings.CleanProfile_Conservative,
                "OffPeakOnly" => Strings.CleanProfile_OffPeakOnly,
                "Manual" => Strings.CleanProfile_Manual,
                _ => token ?? string.Empty
            },
            "Speed" => token switch
            {
                "Slow" => Strings.Speed_Slow,
                "Medium" => Strings.Speed_Medium,
                "Fast" => Strings.Speed_Fast,
                _ => token ?? string.Empty
            },
            "BatteryCondition" => token switch
            {
                "Below" => Strings.Battery_Below,
                "Above" => Strings.Battery_Above,
                _ => token ?? string.Empty
            },
            "AcPower" => token switch
            {
                "Connected" => Strings.AcPower_Connected,
                "Disconnected" => Strings.AcPower_Disconnected,
                _ => token ?? string.Empty
            },
            "Lid" => token switch
            {
                "Closed" => Strings.Lid_Closed,
                "Open" => Strings.Lid_Open,
                _ => token ?? string.Empty
            },
            _ => token switch
            {
                "Auto" => Strings.SettingsView_BackendAuto,
                "WmiBios" => Strings.SettingsView_BackendWmiBios,
                "Wmi" => Strings.SettingsView_BackendWmi,
                "Ec" => Strings.SettingsView_BackendEc,
                "TopLeft" => Strings.Osd_PositionTopLeft,
                "TopCenter" => Strings.Osd_PositionTopCenter,
                "TopRight" => Strings.Osd_PositionTopRight,
                "BottomLeft" => Strings.Osd_PositionBottomLeft,
                "Above" => Strings.Battery_Above,
                "Below" => Strings.Battery_Below,
                "BottomCenter" => Strings.Osd_PositionBottomCenter,
                "BottomRight" => Strings.Osd_PositionBottomRight,
                "Primary" => Strings.Osd_TargetPrimary,
                "ActiveWindow" => Strings.Osd_TargetActiveWindow,
                "MouseCursor" => Strings.Osd_TargetMouseCursor,
                "Compact" => Strings.Osd_DensityCompact,
                "Balanced" => Strings.Osd_DensityBalanced,
                "Comfortable" => Strings.Osd_DensityComfortable,
                "Cyan" => Strings.Osd_AccentCyan,
                "Orange" => Strings.Osd_AccentOrange,
                "Teal" => Strings.Osd_AccentTeal,
                "Magenta" => Strings.Osd_AccentMagenta,
                _ => token ?? string.Empty
            }
        };

        /// <summary>Localized keyboard lighting apply status (Core emits canonical English).</summary>
        public static string KeyboardApplyStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status)) return Strings.KbdStatus_Unknown;
            var s = status.Trim();

            var v2Applied = Regex.Match(s, @"^Applied via V2 (.+)\. (.+)$");
            if (v2Applied.Success)
                return string.Format(Strings.KbdStatus_AppliedViaV2Format, v2Applied.Groups[1].Value, KeyboardVerification(v2Applied.Groups[2].Value));

            var v2Accepted = Regex.Match(s, @"^V2 (.+) accepted the write but did not verify: (.+)$");
            if (v2Accepted.Success)
                return string.Format(Strings.KbdStatus_V2AcceptedNotVerifiedFormat, v2Accepted.Groups[1].Value, KeyboardVerification(v2Accepted.Groups[2].Value));

            var v2Unverified = Regex.Match(s, @"^V2 (.+) did not verify: (.+)$");
            if (v2Unverified.Success)
                return string.Format(Strings.KbdStatus_V2NotVerifiedFormat, v2Unverified.Groups[1].Value, KeyboardVerification(v2Unverified.Groups[2].Value));

            if (s.StartsWith("Keyboard lighting apply failed: ", StringComparison.Ordinal))
                return string.Format(Strings.KbdStatus_ApplyFailedFormat, s.Substring("Keyboard lighting apply failed: ".Length));

            return s switch
            {
                "Applied through individual WMI zone writes." => Strings.KbdStatus_AppliedWmiZones,
                "WMI BIOS ColorTable accepted the write. Verify the intended physical surface changed; on some models this may be a light bar rather than the keyboard." => Strings.KbdStatus_ColorTableAccepted,
                "Applied via OMEN Gaming Hub proxy ColorTable fallback." => Strings.KbdStatus_AppliedOghProxy,
                "Applied via experimental EC fallback after WMI methods failed." => Strings.KbdStatus_AppliedEcFallback,
                "Experimental EC keyboard write was throttled to protect the EC." => Strings.KbdStatus_EcThrottled,
                "Applied via experimental EC keyboard register writes." => Strings.KbdStatus_AppliedEc,
                "All keyboard lighting backends failed. Colors may not be applied." => Strings.KbdStatus_AllFailed,
                _ => s
            };
        }

        private static string KeyboardVerification(string status)
        {
            if (status.Contains("Readback verified", StringComparison.OrdinalIgnoreCase)) return Strings.KbdStatus_ReadbackVerified;
            if (status.Contains("did not verify", StringComparison.OrdinalIgnoreCase) ||
                status.Contains("not verified", StringComparison.OrdinalIgnoreCase)) return Strings.KbdStatus_ReadbackNotVerified;
            return status;
        }

        /// <summary>Localized physical lighting surface reported by the keyboard backend.</summary>
        public static string KeyboardSurface(string? surface)
        {
            return surface?.Trim() switch
            {
                "HP WMI ColorTable zones (keyboard or light bar depending model wiring)" => Strings.KbdSurface_WmiColorTable,
                "HP WMI individual keyboard zones" => Strings.KbdSurface_WmiZones,
                "Keyboard zones (experimental EC)" => Strings.KbdSurface_EcZones,
                "Keyboard zones (experimental EC fallback)" => Strings.KbdSurface_EcZonesFallback,
                "OGH-proxied HP lighting zones" => Strings.KbdSurface_OghZones,
                "Unavailable" => Strings.KbdSurface_Unavailable,
                "Unknown" => Strings.KbdSurface_Unknown,
                null or "" => Strings.KbdSurface_Unknown,
                var other => other
            };
        }
        /// <summary>Localized default Corsair lighting preset names; user-created names pass through.</summary>
        public static string CorsairPreset(string? name)
        {
            return name switch
            {
                "Sync Static" => Strings.CorsairPreset_SyncStatic,
                "Sync Wave" => Strings.CorsairPreset_SyncWave,
                _ => name ?? string.Empty
            };
        }
        /// <summary>Brand label for peripheral cards; product/suite names (iCUE, G HUB) stay as-is.</summary>
        public static string Brand(string? brand)
        {
            return brand?.Trim() switch
            {
                "Corsair" => Strings.Brand_Corsair,
                "Logitech" => Strings.Brand_Logitech,
                "Razer" => Strings.Brand_Razer,
                "HP Omen" => Strings.Brand_HPOmen,
                null or "" => string.Empty,
                var other => other
            };
        }
        /// <summary>Peripheral device type (enum name to label across the Corsair/Logitech/Razer enums).</summary>
        public static string DeviceType(string? type)
        {
            return type?.Trim() switch
            {
                "Keyboard" => Strings.DeviceType_Keyboard,
                "Mouse" => Strings.DeviceType_Mouse,
                "Headset" => Strings.DeviceType_Headset,
                "MouseMat" or "Mousemat" => Strings.DeviceType_MouseMat,
                "Accessory" => Strings.DeviceType_Accessory,
                "WirelessDongle" => Strings.DeviceType_WirelessDongle,
                "Keypad" => Strings.DeviceType_Keypad,
                "ChromaLink" => Strings.DeviceType_ChromaLink,
                "Speaker" => Strings.DeviceType_Speaker,
                "Unknown" => Strings.DeviceType_Unknown,
                null or "" => Strings.DeviceType_Unknown,
                var other => other
            };
        }
        /// <summary>Localized game-library platform filter (enum name to label; brands pass through).</summary>
        public static string PlatformFilter(object? value)
        {
            return value?.ToString() switch
            {
                "All" => Strings.PlatformFilter_All,
                "Manual" => Strings.PlatformFilter_Manual,
                null => string.Empty,
                var s => s
            };
        }
        public static string Scene(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name ?? string.Empty;
            return name switch
            {
                "OMEN Red" => Strings.RgbScene_NameOmenRed,
                "Gaming" => Strings.RgbScene_NameGaming,
                "Night Mode" => Strings.RgbScene_NameNightMode,
                "Work" => Strings.RgbScene_NameWork,
                "Rainbow" => Strings.RgbScene_NameRainbow,
                "Heat Wave" => Strings.RgbScene_NameHeatWave,
                "Calm Pulse" => Strings.RgbScene_NameCalmPulse,
                "Cool Blue" => Strings.RgbScene_NameCoolBlue,
                "Ambient" => Strings.RgbScene_NameAmbient,
                "Audio Reactive" => Strings.RgbScene_NameAudioReactive,
                "Lights Off" => Strings.RgbScene_NameLightsOff,
                "Purple Haze" => Strings.RgbScene_NamePurpleHaze,
                "White" => Strings.RgbScene_NameWhite,
                "Wave Blue" => Strings.RgbScene_NameWaveBlue,
                "Wave Red" => Strings.RgbScene_NameWaveRed,
                "Breathing Green" => Strings.RgbScene_NameBreathingGreen,
                "Reactive Purple" => Strings.RgbScene_NameReactivePurple,
                "Spectrum Flow" => Strings.RgbScene_NameSpectrumFlow,
                "Cyberpunk" => Strings.RgbScene_NameCyberpunk,
                "Sunset" => Strings.RgbScene_NameSunset,
                "Forest" => Strings.RgbScene_NameForest,
                "Ocean" => Strings.RgbScene_NameOcean,
                "Lava" => Strings.RgbScene_NameLava,
                "Ice" => Strings.RgbScene_NameIce,
                "Stealth" => Strings.RgbScene_NameStealth,
                "Off" => Strings.RgbScene_NameOff,
                "None" => Strings.RgbScene_NameNone,
                _ when Regex.IsMatch(name, "^Custom Scene \\d+$")
                    => string.Format(Strings.RgbScene_NameCustomFormat, name.Substring("Custom Scene ".Length)),
                _ => name
            };
        }

        public static string AudioViz(AudioVisualizationMode mode) => mode switch
        {
            AudioVisualizationMode.Pulse => Strings.AudioViz_Pulse,
            AudioVisualizationMode.Spectrum => Strings.AudioViz_Spectrum,
            AudioVisualizationMode.VuMeter => Strings.AudioViz_VuMeter,
            AudioVisualizationMode.Wave => Strings.AudioViz_Wave,
            AudioVisualizationMode.BeatSync => Strings.AudioViz_BeatSync,
            _ => mode.ToString()
        };
        /// <summary>Maps keyboard-effect/theme/speed/direction names (and light-bar effect names)
        /// to localized display strings. Wire/data values stay canonical English.</summary>
        public static string Fx(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name ?? string.Empty;
            return name switch
            {
                "Colour cycle" => Strings.KeyboardFx_ColourCycle,
                "Starlight" => Strings.KeyboardFx_Starlight,
                "Breathing" => Strings.KeyboardFx_Breathing,
                "Ghosting" => Strings.KeyboardFx_Ghosting,
                "Ripple" => Strings.KeyboardFx_Ripple,
                "Wave" => Strings.KeyboardFx_Wave,
                "OMEN X" => Strings.KeyboardFx_OmenX,
                "Raindrop" => Strings.KeyboardFx_Raindrop,
                "Audio pulse" => Strings.KeyboardFx_AudioPulse,
                "Confetti" => Strings.KeyboardFx_Confetti,
                "Sun" => Strings.KeyboardFx_Sun,
                "Swipe" => Strings.KeyboardFx_Swipe,
                "Volcano" => Strings.KeyboardTheme_Volcano,
                "Jungle" => Strings.KeyboardTheme_Jungle,
                "Ocean" => Strings.RgbScene_NameOcean,
                "Rainbow" => Strings.RgbScene_NameRainbow,
                "Slow" => Strings.KeyboardSpeed_Slow,
                "Medium" => Strings.KeyboardSpeed_Medium,
                "Fast" => Strings.KeyboardSpeed_Fast,
                "Left to right" => Strings.KeyboardDir_LeftToRight,
                "Right to left" => Strings.KeyboardDir_RightToLeft,
                "Up" => Strings.KeyboardDir_Up,
                "Down" => Strings.KeyboardDir_Down,
                "Inward" => Strings.KeyboardDir_Inward,
                "Outward" => Strings.KeyboardDir_Outward,
                "Clockwise" => Strings.KeyboardDir_Clockwise,
                "Anticlockwise" => Strings.KeyboardDir_Anticlockwise,
                _ when Regex.IsMatch(name, @"^(\d+) custom colour\(s\)$")
                    => string.Format(Strings.DeviceLighting_CustomColorsFormat, Regex.Replace(name, @"\D", "")),
                _ => name
            };
        }

        public static string Fx(object? value) => value switch
        {
            null => string.Empty,
            string s => Fx(s),
            _ => Fx(value.ToString())
        };
    }
    /// <summary>Displays canonical English option names (speeds, directions, effects, themes)
    /// in the current UI language while keeping the underlying values unchanged.</summary>
    public class FxNameDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => DisplayNames.Fx(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
    /// <summary>Default Corsair lighting preset names shown in the preset picker.</summary>
    public class CorsairPresetNameDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => DisplayNames.CorsairPreset(value as string);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
    /// <summary>Game-library platform filter labels (All / Manually added localized).</summary>
    public class PlatformFilterDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => DisplayNames.PlatformFilter(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
    public class SceneDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => DisplayNames.Scene(value as string);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    public class AudioVizModeDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is AudioVisualizationMode mode ? DisplayNames.AudioViz(mode) : value?.ToString() ?? string.Empty;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// Maps canonical bloatware risk levels to localized badges; keeps the enum untouched in data.
    public class RiskLevelDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value?.ToString()) switch
            {
                "Low" => Strings.BloatwareManagerView_Low,
                "Medium" => Strings.BloatwareManagerView_Med,
                "High" => Strings.BloatwareManagerView_High,
                _ => value?.ToString() ?? string.Empty
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// Shows the canonical fallback model name ("Unknown OMEN") localized; real model names pass through.
    public class ModelNameDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value as string is string s && s == "Unknown OMEN"
                ? Strings.DiagnosticsView_UnknownModel
                : value?.ToString() ?? string.Empty;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// Displays the GPU switch enum localized in the selector; the enum itself is unchanged.
    public class GpuSwitchModeDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is GpuSwitchMode mode ? DisplayNames.GpuSwitchName(mode) : value?.ToString() ?? string.Empty;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// Displays canonical boost levels ("Minimum"/"Medium"/...) localized; the stored value is unchanged.
    public class GpuBoostLevelDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => DisplayNames.BoostLevel(value as string);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// Formats an integer minute value with the localized "{0} 分钟" pattern.
    public class MinutesDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is int minutes ? string.Format(Strings.SettingsView_MinutesFormat, minutes) : value?.ToString() ?? string.Empty;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// Localizes diagnostic text by domain: ConverterParameter = fan | rpm | evidence | error.
    public class DiagnosticTextDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (parameter as string) switch
        {
            "fan" => DisplayNames.FanDiagFanName(value as string),
            "rpm" => DisplayNames.FanDiagRpm(value as string),
            "evidence" => DisplayNames.FanDiagEvidence(value as string),
            "error" => DisplayNames.FanDiagError(value as string),
            _ => value?.ToString() ?? string.Empty
        };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// Displays automation trigger enums localized in the rule editor.
    public class AutomationTriggerDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => DisplayNames.AutomationTrigger(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// Localized bloatware category for the list column; the enum itself is unchanged.
    public class BloatwareCategoryDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => DisplayNames.BloatwareCategory(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// Localized bloatware item type for the list column; the enum itself is unchanged.
    public class BloatwareTypeDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => DisplayNames.BloatwareType(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// Displays settings dropdown tokens (keyboard backend / OSD position, target, density, accent) localized.
    public class SettingsTokenDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => DisplayNames.SettingsToken(value as string, parameter as string);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}