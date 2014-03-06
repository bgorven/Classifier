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

        public static readonly DependencyProperty rectLeftProperty = Canvas.LeftProperty;
        public static readonly DependencyProperty rectTopProperty = Canvas.TopProperty;

        private async void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() ?? false) {
                var fileList = await Task.Run(() => FileAccess.loadInfo(dialog.FileName));
                if (fileList.Count() == 0) return;

                var image = await Task.Run(() =>
                {
                    var im = new BitmapImage(fileList.First().Item1);
                    im.Freeze();
                    return im;
                });

                double scaleX = 96 / image.DpiX;
                double scaleY = 96 / image.DpiY;

                var bg = new ImageBrush();
                bg.ImageSource = image;

                Title = fileList.First().Item1 + " (" + bg.ImageSource.Width + "x" + bg.ImageSource.Height + ")";

                canvas.Background = bg;
                canvas.Width = bg.ImageSource.Width;
                canvas.Height = bg.ImageSource.Height;
                canvas.Children.Clear();

                foreach (ImageSample sample in fileList.First().Item2)
                {
                    var rectangle = new Rectangle();

                    rectangle.SetValue(rectLeftProperty, sample.Left * scaleX);
                    rectangle.SetValue(rectTopProperty, sample.Top * scaleY);
                    rectangle.Width = sample.Width * scaleX;
                    rectangle.Height = sample.Height * scaleY;

                    rectangle.Stroke = Brushes.AliceBlue;
                    rectangle.StrokeThickness = 3;

                    canvas.Children.Add(rectangle);
                }
            }
        }

        private void MenuItem_Click_Save(object sender, RoutedEventArgs e)
        {

        }

        private const double zoomSpeed = 960;
        private double zoom = 0;
        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += e.Delta;

            ScaleTransform scale = new ScaleTransform(Math.Exp(zoom / zoomSpeed), Math.Exp(zoom / zoomSpeed));
            canvas.LayoutTransform = scale;

            e.Handled = true;
        }
    }
}
