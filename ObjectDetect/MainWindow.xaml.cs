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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ObjectDetect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static const DependencyProperty rectLeftProperty = Canvas.LeftProperty;
        public static const DependencyProperty rectTopProperty = Canvas.TopProperty;

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_Save(object sender, RoutedEventArgs e)
        {

        }
    }
}
