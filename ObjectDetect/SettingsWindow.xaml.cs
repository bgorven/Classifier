using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            AddHandler(System.Windows.Controls.Validation.ErrorEvent, new RoutedEventHandler(CheckValidation));
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
                if (System.Windows.Controls.Validation.GetHasError(textBox))
                {
                    OkButton.IsEnabled = false;
                }
            }
        }
    }
}
