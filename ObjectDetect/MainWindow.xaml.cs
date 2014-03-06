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

        private readonly DependencyProperty rectLeftProperty = Canvas.LeftProperty;
        private readonly DependencyProperty rectTopProperty = Canvas.TopProperty;

        private int fileIndex;
        private List<Tuple<Uri, ImageSample[]>> fileList;

        private async void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() ?? false) {
                fileList = await Task.Run(() => FileAccess.loadInfo(dialog.FileName));
                if (fileList.Count() == 0) return;

                fileIndex = 0;
                Canvas_Load_Image(fileIndex);
            }
        }

        private async void Canvas_Load_Image(int index)
        {
            var image = await Task.Run(() =>
            {
                var im = new BitmapImage(fileList[index].Item1);
                im.Freeze();
                return im;
            });

            double scaleX = 96 / image.DpiX;
            double scaleY = 96 / image.DpiY;

            var bg = new ImageBrush();
            bg.ImageSource = image;

            Title = fileList[index].Item1 + " (" + bg.ImageSource.Width + "x" + bg.ImageSource.Height + ")";

            canvas.Background = bg;
            canvas.Width = bg.ImageSource.Width;
            canvas.Height = bg.ImageSource.Height;
            canvas.Children.Clear();

            foreach (ImageSample sample in fileList[index].Item2)
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

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                fileIndex++;
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                fileIndex--;
            }

            if (fileIndex < 0)
            {
                fileIndex = 0;
            }
            else if (fileIndex < fileList.Count)
            {
                Canvas_Load_Image(fileIndex);
            }
            else
            {
                fileIndex = fileList.Count;
                canvas.Children.Clear();
                canvas.Background = Brushes.SkyBlue;
            }
        }
    }
}
