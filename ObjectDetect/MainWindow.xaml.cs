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

        private int fileIndex = 0, rectangleIndex = 0;
        private List<Tuple<Uri, rectangle[]>> fileList;

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

        private void Canvas_Focus_Rectangle(int imageIndex, int rectIndex)
        {
            canvas.LayoutTransform = ScaleTransform.Identity;
            canvas.Children.Clear();

            var rect = fileList[imageIndex].Item2[rectIndex];
            canvas.Children.Add(rect.initializeShape(new Rectangle(), scaleX, scaleY));
            canvas.Children.Add(rect.initializeShape(new Ellipse(), scaleX, scaleY));

            scroller.ScrollToHorizontalOffset(Math.Max(rect.x - 32, 0) * scaleX);
            scroller.ScrollToVerticalOffset(Math.Max(rect.y - 32, 0) * scaleY);
        }

        double scaleX, scaleY;

        private async void Canvas_Load_Image(int imageIndex)
        {
            var image = await Task.Run(() =>
            {
                var im = new BitmapImage(fileList[imageIndex].Item1);
                im.Freeze();
                return im;
            });

            scaleX = 96 / image.DpiX;
            scaleY = 96 / image.DpiY;

            if (image.PixelWidth != FileAccess.imageWidth || image.PixelHeight != FileAccess.imageHeight) throw new Exception();

            var bg = new ImageBrush();
            bg.ImageSource = image;

            Title = fileList[imageIndex].Item1 + " (" + bg.ImageSource.Width + "x" + bg.ImageSource.Height + ")";

            canvas.Background = bg;
            canvas.Width = bg.ImageSource.Width;
            canvas.Height = bg.ImageSource.Height;

            Keyboard.Focus(canvas);
        }

        private void Canvas_Load_Rectangles(int imageIndex)
        {

            canvas.Children.Clear();

            foreach (var sample in fileList[imageIndex].Item2)
            {
                canvas.Children.Add(sample.initializeShape(new Rectangle(), scaleX, scaleY));
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
                Canvas_Load_Rectangles(fileIndex);
            }
            else
            {
                fileIndex = fileList.Count;
                canvas.Children.Clear();
                canvas.Background = Brushes.SkyBlue;
            }
        }

        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            switch (e.Key)
            {
                case Key.Back:
                    rectangleIndex--;
                    if (rectangleIndex < 0)
                    {
                        fileIndex--;
                        if (fileIndex < 0)
                        {
                            fileIndex = 0;
                            rectangleIndex = 0;
                        }
                        else
                        {
                            rectangleIndex = fileList[fileIndex].Item2.Length - 1;
                        }
                        Canvas_Load_Image(fileIndex);
                    }
                    break;
                case Key.Space:
                    rectangleIndex++;
                    if (rectangleIndex >= fileList[fileIndex].Item2.Length)
                    {
                        fileIndex++;
                        if (fileIndex >= fileList.Count)
                        {
                            fileIndex = fileList.Count - 1;
                            rectangleIndex = fileList[fileIndex].Item2.Length - 1;
                        }
                        else
                        {
                            rectangleIndex = 0;
                        }
                        Canvas_Load_Image(fileIndex);
                    }
                    break;
                case Key.Left:
                    fileList[fileIndex].Item2[rectangleIndex].Left--;
                    break;
                case Key.Right:
                    fileList[fileIndex].Item2[rectangleIndex].Left++;
                    break;
                case Key.Up:
                    fileList[fileIndex].Item2[rectangleIndex].Top--;
                    break;
                case Key.Down:
                    fileList[fileIndex].Item2[rectangleIndex].Top++;
                    break;
                case Key.W:
                    fileList[fileIndex].Item2[rectangleIndex].Width++;
                    fileList[fileIndex].Item2[rectangleIndex].Height++;
                    break;
                case Key.S:
                    fileList[fileIndex].Item2[rectangleIndex].Width--;
                    fileList[fileIndex].Item2[rectangleIndex].Height--;
                    break;
                default:
                    e.Handled = false;
                    break;
            }
            Canvas_Focus_Rectangle(fileIndex, rectangleIndex);
        }
    }
}
