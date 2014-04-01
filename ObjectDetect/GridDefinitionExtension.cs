using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows;

namespace ObjectDetect.MarkupExtensions
{
    class GridDefinitionExtension : MarkupExtension
    {
        public string Name { get; set; }

        public GridDefinitionExtension(string name)
        {
            Name = name;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var refExt = new Reference(Name);
            var definition = refExt.ProvideValue(serviceProvider);
            if (definition is RowDefinition || definition is ColumnDefinition)
            {
                var grid = (definition as FrameworkContentElement).Parent as Grid;
                if (definition is RowDefinition)
                {
                    return grid.RowDefinitions.IndexOf(definition as RowDefinition);
                }
                else
                {
                    return grid.ColumnDefinitions.IndexOf(definition as ColumnDefinition);
                }
            }
            else
            {
                throw new Exception("Found object is neither a RowDefinition nor a ColumnDefinition");
            }
        }
    }
}
