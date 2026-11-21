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
        public static string GpuSwitchName(GpuSwitchMode mode) => mode switch
        {
            GpuSwitchMode.Hybrid => Strings.GpuSwitch_ModeHybrid,
            GpuSwitchMode.Discrete => Strings.GpuSwitch_ModeDiscrete,
            GpuSwitchMode.Integrated => Strings.GpuSwitch_ModeIntegrated,
            _ => Strings.GpuSwitch_ModeUnknown
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
                "Auto" => Strings.FanPreset_Auto,
                "Quiet" => Strings.FanPreset_Quiet,
                "Extreme" => Strings.FanPreset_Extreme,
                "Max" => Strings.FanPreset_Max,
                "Default" => Strings.FanPreset_Default,
                "Silent" => Strings.FanPreset_Silent,
                "Balanced" => Strings.FanPreset_Balanced,
                "Performance" => Strings.FanPreset_Performance,
                _ => token ?? string.Empty
            },
            "PerfMode" => token switch
            {
                "Silent" => Strings.PerfMode_Silent,
                "Balanced" => Strings.PerfMode_Balanced,
                "Performance" => Strings.PerfMode_Performance,
                "Turbo" => Strings.PerfMode_Turbo,
                "Comfort" => Strings.PerfMode_Comfort,
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

    /// Displays settings dropdown tokens (keyboard backend / OSD position, target, density, accent) localized.
    public class SettingsTokenDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => DisplayNames.SettingsToken(value as string, parameter as string);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}