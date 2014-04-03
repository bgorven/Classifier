//Taken from http://stackoverflow.com/questions/6447777/how-to-reference-a-row-column-defintion-in-grid-row-grid-column
//and http://stackoverflow.com/questions/19232491/wpf-markupextension-and-rowdefinition-results-in-notimplementedexception

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Util.Markup
{
    public class GridDefinitionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var definition = (DefinitionBase) value;
            var grid = (Grid) definition.Parent;

            if (definition is RowDefinition)
            {
                return grid.RowDefinitions.IndexOf(definition as RowDefinition);
            }
            if (definition is ColumnDefinition)
            {
                return grid.ColumnDefinitions.IndexOf(definition as ColumnDefinition);
            }
            
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
