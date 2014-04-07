using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace ObjectDetect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class WindowData
    {
        private readonly MainWindow _window;
        public WindowData(MainWindow window)
        {
            _window = window;
        }

        private static readonly DependencyProperty RectLeftProperty = Canvas.LeftProperty;
        private static readonly DependencyProperty RectTopProperty = Canvas.TopProperty;

        private int _fileIndex, _rectangleIndex;
        private List<FileAccess.FileEntry> _fileList;
        private const string DataFileExt = ".dat";
        private const string DataFileFilter = "datafiles (.dat)|*.dat";
        private bool _unsavedChangesPresent;
        public List<FileAccess.FileEntry> FileList { get { return _fileList ?? new List<FileAccess.FileEntry>(); } }

        internal async Task Load_File()
        {
            if (!Confirm_Discard_Changes()) return;

            var dialog = new OpenFileDialog
            {
                DefaultExt = DataFileExt, 
                Filter = DataFileFilter
            };
            if (dialog.ShowDialog() ?? false)
            {
                _fileList = await FileAccess.LoadInfo(dialog.FileName);
                _unsavedChangesPresent = false;
                if (_fileList.Any())
                {
                    _fileIndex = 0;
                    _rectangleIndex = 0;
                    RectangleHasFocus = false;
                    await Canvas_Load_Image(_fileIndex);
                    Canvas_Load_Rectangles(_fileIndex);
                }
            }
        }

        internal async Task Save_File()
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = DataFileExt, 
                Filter = DataFileFilter
            };
            if (dialog.ShowDialog() ?? false)
            {
                await FileAccess.SaveInfo(dialog.FileName, _fileList);
                _unsavedChangesPresent = false;
            }
        }

        internal bool Confirm_Discard_Changes()
        {
            if (_unsavedChangesPresent)
            {
                var result = MessageBox.Show("Discard unsaved changes?", "Datafile Not Saved", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes) _unsavedChangesPresent = false;
            }
            return !_unsavedChangesPresent;
        }

        private void Canvas_Focus_Rectangle(int imageIndex, int rectIndex)
        {
            _window.Canvas.LayoutTransform = Transform.Identity;
            Canvas_Load_Rectangles(imageIndex);

            var rect = _fileList[imageIndex].Rectangles[rectIndex];
            _window.Canvas.Children.Add(InitializeShape(rect, new System.Windows.Shapes.Rectangle(), _scaleX, _scaleY, Brushes.RoyalBlue));
            _window.Canvas.Children.Add(InitializeShape(rect, new Ellipse(), _scaleX, _scaleY, Brushes.RoyalBlue));

            _window.Scroller.ScrollToHorizontalOffset(Math.Max(rect.X - 32, 0) * _scaleX);
            _window.Scroller.ScrollToVerticalOffset(Math.Max(rect.Y - 32, 0) * _scaleY);

            _window.Title = "File " + (_fileIndex + 1) + "/" + _fileList.Count + ", Box " + (_rectangleIndex + 1) + "/" + _fileList[_fileIndex].Rectangles.Count;
            RectangleHasFocus = true;
        }

        /// <summary>
        /// On-Screen size == pixel size * scale.
        /// </summary>
        double _scaleX, _scaleY;

        private async Task Canvas_Load_Image(int imageIndex)
        {
            var image = await Task.Run(delegate
            {
                var im = new BitmapImage(new Uri(_fileList[imageIndex].FileName));
                im.Freeze();
                return im;
            });

            _scaleX = image.Width / image.PixelWidth;
            _scaleY = image.Height / image.PixelHeight;

            //if (image.PixelWidth != FileAccess.imageWidth || image.PixelHeight != FileAccess.imageHeight) throw new Exception();

            var bg = new ImageBrush
            {
                ImageSource = image
            };

            _window.Title = Path.GetFileName(_fileList[imageIndex].FileName) + " (" + bg.ImageSource.Width + "x" + bg.ImageSource.Height + ")";

            _window.Canvas.Background = bg;
            _window.Canvas.Width = bg.ImageSource.Width;
            _window.Canvas.Height = bg.ImageSource.Height;

            Canvas_Scale(_zoom);

            Keyboard.Focus(_window.Canvas);
        }

        private void Canvas_Load_Rectangles(int imageIndex)
        {
            _window.Canvas.Children.Clear();

            foreach (var sample in _fileList[imageIndex].Rectangles)
            {
                _window.Canvas.Children.Add(InitializeShape(sample, new System.Windows.Shapes.Rectangle(), _scaleX, _scaleY, Brushes.OrangeRed));
            }
        }

        private const double ZoomSpeed = 960;

        internal void Canvas_Scale(double zoom)
        {
            _window.Canvas.LayoutTransform = new ScaleTransform(Math.Exp(zoom / ZoomSpeed), Math.Exp(zoom / ZoomSpeed));
        }

        private double _zoom = 0;
        internal void Canvas_WheelZoom(double delta)
        {
            _zoom += delta;
            Canvas_Scale(_zoom);
            RectangleHasFocus = false;
            Keyboard.Focus(_window.Canvas);
        }

        internal async Task<bool> Canvas_Click(MouseButton changedButton)
        {
            var handled = false;
            if (changedButton == MouseButton.Left)
            {
                _fileIndex++;
                _rectangleIndex = 0;
                RectangleHasFocus = false;
                handled = true;
            }
            else if (changedButton == MouseButton.Right)
            {
                _fileIndex--;
                _rectangleIndex = 0;
                RectangleHasFocus = false;
                handled = true;
            }

            if (_fileIndex < 0)
            {
                _fileIndex = 0;
            }
            else if (_fileIndex < _fileList.Count)
            {
                await Canvas_Load_Image(_fileIndex);
                Canvas_Load_Rectangles(_fileIndex);
            }
            else
            {
                _fileIndex = _fileList.Count;
                _window.Canvas.Children.Clear();
                _window.Canvas.Background = Brushes.SkyBlue;
            }
            return handled;
        }

        public Shape InitializeShape(Rectangle rect, Shape shape, double scaleX, double scaleY, Brush color)
        {
            shape.SetValue(RectLeftProperty, rect.X * scaleX);
            shape.SetValue(RectTopProperty, rect.Y * scaleY);
            shape.Width = rect.W * scaleX;
            shape.Height = rect.H * scaleY;

            shape.Stroke = color;
            shape.StrokeThickness = 2;

            return shape;
        }

        private Rectangle GetRect(Point a, Point b)
        {
            var minX = Math.Min(a.X, b.X) / _scaleX;
            var maxX = Math.Max(b.X, a.X) / _scaleX;
            var minY = Math.Min(b.Y, a.Y) / _scaleY;
            var maxY = Math.Max(a.Y, b.Y) / _scaleY;
            return Clamp_Rectangle(new Rectangle(minX, minY, maxX - minX, maxY - minY));
        }

        internal void Canvas_DragIntermediate(Point dragStart, Point dragEnd)
        {
            _window.Canvas.Children.Clear();
            _window.Canvas.Children.Add(InitializeShape(GetRect(dragStart, dragEnd), new System.Windows.Shapes.Rectangle(), _scaleX, _scaleY, Brushes.RoyalBlue));
            _window.Title = dragStart.X + "," + dragStart.Y + " - " + GetRect(dragStart, dragEnd).X + "," + GetRect(dragStart, dragEnd).Y;
        }

        internal void Canvas_DragFinal(Point dragStart, Point dragEnd)
        {
            _fileList[_fileIndex].Rectangles.Insert(_rectangleIndex, GetRect(dragStart, dragEnd));
            Canvas_Focus_Rectangle(_fileIndex, _rectangleIndex);
            _unsavedChangesPresent = true;
        }
        internal Rectangle Clamp_Rectangle(Rectangle rect)
        {
            if (_window.Canvas.Background is ImageBrush && ((ImageBrush)_window.Canvas.Background).ImageSource is BitmapImage)
            {
                var image = (BitmapImage)((ImageBrush)_window.Canvas.Background).ImageSource;
                rect.X = FixedPoint.Min(FixedPoint.Max(rect.X, 0), image.PixelWidth - 1);
                rect.W = FixedPoint.Min(rect.W, image.PixelWidth - rect.X - 1);
                rect.Y = FixedPoint.Min(FixedPoint.Max(rect.Y, 0), image.PixelHeight - 1);
                rect.H = FixedPoint.Min(rect.H, image.PixelHeight - rect.Y - 1);
                rect.H = rect.W = FixedPoint.Min(rect.W, rect.H);
            }
            else
            {
                rect.X = FixedPoint.Max(rect.X, 0);
                rect.Y = FixedPoint.Max(rect.Y, 0);
                rect.H = rect.W = FixedPoint.Min(rect.H, rect.W);
            }
            return rect;
        }

        internal bool RectangleHasFocus { get; private set; }

        internal async Task ChangeRectangle(int offset)
        {
            _rectangleIndex += offset;
            if (_rectangleIndex < 0)
            {
                _fileIndex--;
                if (_fileIndex < 0)
                {
                    _fileIndex = 0;
                    _rectangleIndex = 0;
                }
                else
                {
                    _rectangleIndex = _fileList[_fileIndex].Rectangles.Count - 1;
                }
                await Canvas_Load_Image(_fileIndex);
            }
            else if (_rectangleIndex >= _fileList[_fileIndex].Rectangles.Count)
            {
                _fileIndex++;
                if (_fileIndex >= _fileList.Count)
                {
                    _fileIndex = _fileList.Count - 1;
                    _rectangleIndex = _fileList[_fileIndex].Rectangles.Count - 1;
                }
                else
                {
                    _rectangleIndex = 0;
                }
                await Canvas_Load_Image(_fileIndex);
            }
        }

        private readonly Stack<Tuple<int, Rectangle>> _undoStack = new Stack<Tuple<int, Rectangle>>();
        internal void RemoveRectangle(int fileIndex, int rectangleIndex)
        {
            var removed = _fileList[fileIndex].Rectangles[rectangleIndex];
            _undoStack.Push(Tuple.Create(fileIndex, removed));
            _fileList[fileIndex].Rectangles.RemoveAt(rectangleIndex);
            RectangleHasFocus = false;
        }

        internal void UndoRemoveRectangle(ref int fileIndex, ref int rectangleIndex)
        {
            if (_undoStack.Count > 0)
            {
                var removed = _undoStack.Pop();
                fileIndex = removed.Item1;
                rectangleIndex = _fileList[fileIndex].Rectangles.Count;
                _fileList[fileIndex].Rectangles.Add(removed.Item2);
            }
        }

        internal enum Dimension {
            X, Y, Size
        }

        internal Rectangle TransformRectangle(Rectangle rectangle, Dimension dimension, int amount)
        {
            switch (dimension) {
                case Dimension.X:
                    rectangle.Left += amount;
                    break;
                case Dimension.Y:
                    rectangle.Top += amount;
                    break;
                case Dimension.Size:
                    rectangle.Width += amount;
                    rectangle.Height += amount;
                    break;
            }
            rectangle = Clamp_Rectangle(rectangle);
            _unsavedChangesPresent = true;
            return rectangle;
        }

        internal async void Canvas_KeyDown(object sender, KeyEventArgs e, MainWindow mainWindow)
        {
            if (mainWindow.DragLeft || mainWindow.DragRight) return;
            if (RectangleHasFocus)
            {
                var step = 2;
                if (e.IsRepeat) step = 4;
                if (e.KeyboardDevice.IsKeyDown(Key.Back))
                {
                    await ChangeRectangle(-1);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Space))
                {
                    await ChangeRectangle(+1);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Left))
                {
                    _fileList[_fileIndex].Rectangles[_rectangleIndex] = TransformRectangle(_fileList[_fileIndex].Rectangles[_rectangleIndex], Dimension.X, -step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Right))
                {
                    _fileList[_fileIndex].Rectangles[_rectangleIndex] = TransformRectangle(_fileList[_fileIndex].Rectangles[_rectangleIndex], Dimension.X, step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Up))
                {
                    _fileList[_fileIndex].Rectangles[_rectangleIndex] = TransformRectangle(_fileList[_fileIndex].Rectangles[_rectangleIndex], Dimension.Y, -step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Down))
                {
                    _fileList[_fileIndex].Rectangles[_rectangleIndex] = TransformRectangle(_fileList[_fileIndex].Rectangles[_rectangleIndex], Dimension.Y, step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.W))
                {
                    _fileList[_fileIndex].Rectangles[_rectangleIndex] = TransformRectangle(_fileList[_fileIndex].Rectangles[_rectangleIndex], Dimension.Size, step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.S))
                {
                    _fileList[_fileIndex].Rectangles[_rectangleIndex] = TransformRectangle(_fileList[_fileIndex].Rectangles[_rectangleIndex], Dimension.Size, -step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Delete) && !e.IsRepeat)
                {
                    RemoveRectangle(_fileIndex, _rectangleIndex);

                    RectangleHasFocus = false;
                    Canvas_Load_Rectangles(_fileIndex);
                    e.Handled = true;
                    return;
                }
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl) && e.KeyboardDevice.IsKeyDown(Key.Z) && !e.IsRepeat)
            {
                UndoRemoveRectangle(ref _fileIndex, ref _rectangleIndex);
                await Canvas_Load_Image(_fileIndex);
            }
            else if (!(e.Key == Key.Space || e.Key == Key.Back))
            {
                return;
            }
            _rectangleIndex = Math.Min(Math.Max(_rectangleIndex, 0), _fileList[_fileIndex].Rectangles.Count - 1);
            Canvas_Focus_Rectangle(_fileIndex, _rectangleIndex);
            e.Handled = true;
        }
    }
}
