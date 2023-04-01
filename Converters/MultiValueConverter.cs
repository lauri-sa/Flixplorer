using System;
using System.Windows.Data;
using System.Globalization;

namespace Flixplorer.Converters
{
    /// <summary>
    /// Class that is used to convert an array of values into a single value that is passed as command parameter.
    /// Implements IMultiValueConverter interface.
    /// </summary>
    internal class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}