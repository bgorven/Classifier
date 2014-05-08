using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Validation;

namespace ObjectDetect
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        internal SettingsWindow()
        {
            InitializeComponent();
            AddHandler(ErrorEvent, new RoutedEventHandler(CheckValidation));
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
                if (GetHasError(textBox))
                {
                    OkButton.IsEnabled = false;
                }
            }
        }
    }
}
