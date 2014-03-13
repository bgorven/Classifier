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

        private int fileIndex = 0, rectangleIndex = -1;
        private List<Tuple<Uri, rectangle[]>> fileList;
        private const string dataFileExt = ".dat";
        private const string dataFileFilter = "datafiles (.dat)|*.dat";
        private bool unsavedChangesPresent = false;

        private async void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            await Load_File();
        }

        private async Task Load_File()
        {
            if (!Confirm_Discard_Changes()) return;

            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = dataFileExt;
            dialog.Filter = dataFileFilter;
            if (dialog.ShowDialog() ?? false)
            {
                fileList = await FileAccess.loadInfo(dialog.FileName);
                if (fileList.Count() > 0)
                {
                    fileIndex = 0;
                    rectangleIndex = -1;
                    Canvas_Load_Image(fileIndex);
                    Canvas_Load_Rectangles(fileIndex);
                    unsavedChangesPresent = false;
                }
            }
        }

        private async void MenuItem_Click_Save(object sender, RoutedEventArgs e)
        {
            await Save_File();
        }

        private async Task Save_File()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = dataFileExt;
            dialog.Filter = dataFileFilter;
            if (dialog.ShowDialog() ?? false)
            {
                await FileAccess.saveInfo(dialog.FileName, fileList);
                unsavedChangesPresent = false;
            }
        }

        private bool Confirm_Discard_Changes()
        {
            if (unsavedChangesPresent)
            {
                var result = MessageBox.Show("Discard unsaved changes?", "Datafile Not Saved", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes) unsavedChangesPresent = false;
            }
            return !unsavedChangesPresent;
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
            var image = await Task.Run(delegate
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

            Title = System.IO.Path.GetFileName(fileList[imageIndex].Item1.AbsoluteUri) + " (" + bg.ImageSource.Width + "x" + bg.ImageSource.Height + ")";

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
                rectangleIndex = -1;
                e.Handled = true;
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                fileIndex--;
                rectangleIndex = -1;
                e.Handled = true;
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
            int step = 2;
            if (e.IsRepeat) step = 4;
            if (e.KeyboardDevice.IsKeyDown(Key.Back))
            {
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
                e.Handled = true;
            }
            if (e.KeyboardDevice.IsKeyDown(Key.Space))
            {
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
                e.Handled = true;
            }
            if (e.KeyboardDevice.IsKeyDown(Key.Left))
            {
                fileList[fileIndex].Item2[rectangleIndex].Left -= step;
                e.Handled = true;
            }
            if (e.KeyboardDevice.IsKeyDown(Key.Right))
            {
                fileList[fileIndex].Item2[rectangleIndex].Left += step;
                e.Handled = true;
            }
            if (e.KeyboardDevice.IsKeyDown(Key.Up))
            {
                fileList[fileIndex].Item2[rectangleIndex].Top -= step;
                e.Handled = true;
            }
            if (e.KeyboardDevice.IsKeyDown(Key.Down))
            {
                fileList[fileIndex].Item2[rectangleIndex].Top += step;
                e.Handled = true;
            }
            if (e.KeyboardDevice.IsKeyDown(Key.W))
            {
                fileList[fileIndex].Item2[rectangleIndex].Width += step;
                fileList[fileIndex].Item2[rectangleIndex].Height += step;
                e.Handled = true;
            }
            if (e.KeyboardDevice.IsKeyDown(Key.S))
            {
                fileList[fileIndex].Item2[rectangleIndex].Width -= step;
                fileList[fileIndex].Item2[rectangleIndex].Height -= step;
                e.Handled = true;
            }
            Canvas_Focus_Rectangle(fileIndex, rectangleIndex);
            Title = "File " + (fileIndex + 1) + "/" + fileList.Count + ", Box " + (rectangleIndex + 1) + "/" + fileList[fileIndex].Item2.Length;
            unsavedChangesPresent = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Confirm_Discard_Changes()) e.Cancel = true;
        }
    }
}
