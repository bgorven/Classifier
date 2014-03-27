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
        private static readonly DependencyProperty rectLeftProperty = System.Windows.Controls.Canvas.LeftProperty;
        private static readonly DependencyProperty rectTopProperty = System.Windows.Controls.Canvas.TopProperty;

        private int fileIndex = 0, rectangleIndex = 0;
        private List<FileAccess.FileEntry> fileList;
        private const string dataFileExt = ".dat";
        private const string dataFileFilter = "datafiles (.dat)|*.dat";
        private bool unsavedChangesPresent = false;

        private async Task Load_File()
        {
            if (!Confirm_Discard_Changes()) return;

            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = dataFileExt;
            dialog.Filter = dataFileFilter;
            if (dialog.ShowDialog() ?? false)
            {
                fileList = await FileAccess.loadInfo(dialog.FileName);
                unsavedChangesPresent = false;
                if (fileList.Count() > 0)
                {
                    fileIndex = 0;
                    rectangleIndex = 0;
                    rectangleHasFocus = false;
                    await Canvas_Load_Image(fileIndex);
                    Canvas_Load_Rectangles(fileIndex);
                }
            }
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
            Canvas_Load_Rectangles(imageIndex);

            var rect = fileList[imageIndex].Rectangles[rectIndex];
            canvas.Children.Add(initializeShape(rect, new Rectangle(), scaleX, scaleY, Brushes.RoyalBlue));
            canvas.Children.Add(initializeShape(rect, new Ellipse(), scaleX, scaleY, Brushes.RoyalBlue));

            scroller.ScrollToHorizontalOffset(Math.Max(rect.x - 32, 0) * scaleX);
            scroller.ScrollToVerticalOffset(Math.Max(rect.y - 32, 0) * scaleY);

            Title = "File " + (fileIndex + 1) + "/" + fileList.Count + ", Box " + (rectangleIndex + 1) + "/" + fileList[fileIndex].Rectangles.Count;
            rectangleHasFocus = true;
        }

        /// <summary>
        /// On-Screen size == pixel size * scale.
        /// </summary>
        double scaleX, scaleY;

        private async Task Canvas_Load_Image(int imageIndex)
        {
            var image = await Task.Run(delegate
            {
                var im = new BitmapImage(new Uri(fileList[imageIndex].FileName));
                im.Freeze();
                return im;
            });

            scaleX = image.Width / image.PixelWidth;
            scaleY = image.Height / image.PixelHeight;

            //if (image.PixelWidth != FileAccess.imageWidth || image.PixelHeight != FileAccess.imageHeight) throw new Exception();

            var bg = new ImageBrush();
            bg.ImageSource = image;

            Title = System.IO.Path.GetFileName(fileList[imageIndex].FileName) + " (" + bg.ImageSource.Width + "x" + bg.ImageSource.Height + ")";

            canvas.Background = bg;
            canvas.Width = bg.ImageSource.Width;
            canvas.Height = bg.ImageSource.Height;

            Canvas_Scale(zoom);

            Keyboard.Focus(canvas);
        }

        private void Canvas_Load_Rectangles(int imageIndex)
        {
            canvas.Children.Clear();

            foreach (var sample in fileList[imageIndex].Rectangles)
            {
                canvas.Children.Add(initializeShape(sample, new Rectangle(), scaleX, scaleY, Brushes.OrangeRed));
            }
        }

        private const double zoomSpeed = 960;
        private void Canvas_Scale(double zoom)
        {
            canvas.LayoutTransform = new ScaleTransform(Math.Exp(zoom / zoomSpeed), Math.Exp(zoom / zoomSpeed));
        }

        private async Task<bool> Canvas_Click(MouseButton ChangedButton)
        {
            bool Handled = false;
            if (ChangedButton == MouseButton.Left)
            {
                fileIndex++;
                rectangleIndex = 0;
                rectangleHasFocus = false;
                Handled = true;
            }
            else if (ChangedButton == MouseButton.Right)
            {
                fileIndex--;
                rectangleIndex = 0;
                rectangleHasFocus = false;
                Handled = true;
            }

            if (fileIndex < 0)
            {
                fileIndex = 0;
            }
            else if (fileIndex < fileList.Count)
            {
                await Canvas_Load_Image(fileIndex);
                Canvas_Load_Rectangles(fileIndex);
            }
            else
            {
                fileIndex = fileList.Count;
                canvas.Children.Clear();
                canvas.Background = Brushes.SkyBlue;
            }
            return Handled;
        }

        public System.Windows.Shapes.Shape initializeShape(rectangle rect, System.Windows.Shapes.Shape shape, double scaleX, double scaleY, Brush color)
        {
            shape.SetValue(rectLeftProperty, rect.x * scaleX);
            shape.SetValue(rectTopProperty, rect.y * scaleY);
            shape.Width = rect.w * scaleX;
            shape.Height = rect.h * scaleY;

            shape.Stroke = color;
            shape.StrokeThickness = 2;

            return shape;
        }

        private rectangle getRect(Point A, Point B)
        {
            var minX = Math.Min(A.X, B.X) / scaleX;
            var maxX = Math.Max(B.X, A.X) / scaleX;
            var minY = Math.Min(B.Y, A.Y) / scaleY;
            var maxY = Math.Max(A.Y, B.Y) / scaleY;
            return Clamp_Rectangle(new rectangle(minX, minY, maxX - minX, maxY - minY));
        }

        private void Canvas_DragIntermediate(Point dragStart, Point dragEnd)
        {
            canvas.Children.Clear();
            canvas.Children.Add(initializeShape(getRect(dragStart, dragEnd), new Rectangle(), scaleX, scaleY, Brushes.RoyalBlue));
            Title = dragStart.X + "," + dragStart.Y + " - " + getRect(dragStart, dragEnd).x + "," + getRect(dragStart, dragEnd).y;
        }

        private void Canvas_DragFinal(Point dragStart, Point dragEnd)
        {
            fileList[fileIndex].Rectangles.Insert(rectangleIndex, getRect(dragStart, dragEnd));
            Canvas_Focus_Rectangle(fileIndex, rectangleIndex);
            unsavedChangesPresent = true;
        }
        private rectangle Clamp_Rectangle(rectangle rect)
        {
            if (canvas.Background is ImageBrush && ((ImageBrush)canvas.Background).ImageSource is BitmapImage)
            {
                var image = (BitmapImage)((ImageBrush)canvas.Background).ImageSource;
                rect.x = fixed_point.Min(fixed_point.Max(rect.x, 0), image.PixelWidth - 1);
                rect.w = fixed_point.Min(rect.w, image.PixelWidth - rect.x - 1);
                rect.y = fixed_point.Min(fixed_point.Max(rect.y, 0), image.PixelHeight - 1);
                rect.h = fixed_point.Min(rect.h, image.PixelHeight - rect.y - 1);
                rect.h = rect.w = fixed_point.Min(rect.w, rect.h);
            }
            else
            {
                rect.x = fixed_point.Max(rect.x, 0);
                rect.y = fixed_point.Max(rect.y, 0);
                rect.h = rect.w = fixed_point.Min(rect.h, rect.w);
            }
            return rect;
        }

        bool rectangleHasFocus = false;
        private async Task changeRectangle(int offset) {
            rectangleIndex += offset;
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
                    rectangleIndex = fileList[fileIndex].Rectangles.Count - 1;
                }
                await Canvas_Load_Image(fileIndex);
            }
            else if (rectangleIndex >= fileList[fileIndex].Rectangles.Count)
            {
                fileIndex++;
                if (fileIndex >= fileList.Count)
                {
                    fileIndex = fileList.Count - 1;
                    rectangleIndex = fileList[fileIndex].Rectangles.Count - 1;
                }
                else
                {
                    rectangleIndex = 0;
                }
                await Canvas_Load_Image(fileIndex);
            }
        }

        private Stack<Tuple<int, rectangle>> undoStack = new Stack<Tuple<int, rectangle>>();
        private void removeRectangle(int fileIndex, int rectangleIndex)
        {
            var removed = fileList[fileIndex].Rectangles[rectangleIndex];
            undoStack.Push(Tuple.Create(fileIndex, removed));
            fileList[fileIndex].Rectangles.RemoveAt(rectangleIndex);
            rectangleHasFocus = false;
        }

        private void undoRemoveRectangle(ref int fileIndex, ref int rectangleIndex){
            if (undoStack.Count > 0)
            {
                var removed = undoStack.Pop();
                fileIndex = removed.Item1;
                rectangleIndex = fileList[fileIndex].Rectangles.Count;
                fileList[fileIndex].Rectangles.Add(removed.Item2);
            }
        }

        enum Dimension {
            x, y, size
        }

        private rectangle transformRectangle(rectangle rectangle, Dimension dimension, int amount){
            switch (dimension) {
                case Dimension.x:
                    rectangle.Left += amount;
                    break;
                case Dimension.y:
                    rectangle.Top += amount;
                    break;
                case Dimension.size:
                    rectangle.Width += amount;
                    rectangle.Height += amount;
                    break;
            }
            rectangle = Clamp_Rectangle(rectangle);
            unsavedChangesPresent = true;
            return rectangle;
        }
    }
}
