using System;
using System.Globalization;
using System.Windows.Data;
using OmenCore.Models;
using OmenCore.Localization;

namespace OmenCore.Utils
{
    /// <summary>Shows a performance mode by the profile card's name ("Balanced", not the firmware's "Default").</summary>
    public sealed class ModeDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var n = PerformanceModeNameResolver.ToDisplayName(value as string);
            if (n == "Performance") return Strings.PerformanceMode_Performance;
            if (n == "Quiet") return Strings.PerformanceMode_Quiet;
            if (n == "Balanced") return Strings.PerformanceMode_Balanced;
            return n;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            Binding.DoNothing;
    }
}
