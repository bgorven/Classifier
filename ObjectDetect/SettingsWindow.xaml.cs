using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ObjectDetect
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            AddHandler(Validation.ErrorEvent, new RoutedEventHandler(CheckValidation));
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CheckValidation(object sender, RoutedEventArgs e)
        {
            OkButton.IsEnabled = true;
            foreach (var textBox in SettingsGrid.Children.OfType<TextBox>())
            {
                if (Validation.GetHasError(textBox))
                {
                    OkButton.IsEnabled = false;
                }
            }
        }
    }
}
