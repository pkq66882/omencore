using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using OmenCore.Localization;
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
}