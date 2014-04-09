//Taken from http://stackoverflow.com/questions/6447777/how-to-reference-a-row-column-defintion-in-grid-row-grid-column
//and http://stackoverflow.com/questions/19232491/wpf-markupextension-and-rowdefinition-results-in-notimplementedexception

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utilities.Markup
{
    public class GridDefinitionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            for (var def = value as RowDefinition; def != null;)
            {
                var grid = (Grid) def.Parent;
                return grid.RowDefinitions.IndexOf(def);
            }
            for (var def = value as ColumnDefinition; def != null; )
            {
                var grid = (Grid)def.Parent;
                return grid.ColumnDefinitions.IndexOf(def);
            }
            
            throw new ArgumentException("value should be a RowDefinition or ColumnDefinition.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
