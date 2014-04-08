using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace ObjectDetect
{
    static class WindowCanvas
    {
        internal static void FocusRectangle(this Canvas canvas, WindowData data, int imageIndex, int rectIndex)
        {
            canvas.LayoutTransform = Transform.Identity;
            canvas.LoadRectangles(data, imageIndex);

            var rect = data.FileList[imageIndex].Rectangles[rectIndex];
            canvas.Children.Add(canvas.InitializeShape(rect, new System.Windows.Shapes.Rectangle(), data.ScaleX, data.ScaleY, Brushes.RoyalBlue));
            canvas.Children.Add(canvas.InitializeShape(rect, new Ellipse(), data.ScaleX, data.ScaleY, Brushes.RoyalBlue));

            var scroller = canvas.Parent as ScrollViewer;
            if (scroller != null)
            {
                scroller.ScrollToHorizontalOffset(Math.Max(rect.X - 32, 0) *data.ScaleX);
                scroller.ScrollToVerticalOffset(Math.Max(rect.Y - 32, 0) *data.ScaleY);
            }

            data.WindowTitle = "File " + (data.FileIndex + 1) + "/" + data.FileList.Count + ", Box " + (data.RectangleIndex + 1) + "/" + data.FileList[data.FileIndex].Rectangles.Count;
            data.RectangleHasFocus = true;
        }

        internal static async Task LoadImage(this Canvas canvas, WindowData data, int imageIndex)
        {
            var image = await Task.Run(delegate
            {
                var im = new BitmapImage(new Uri(data.FileList[imageIndex].FileName));
                im.Freeze();
                return im;
            });

            data.ScaleX = image.Width / image.PixelWidth;
            data.ScaleY = image.Height / image.PixelHeight;

            //if (image.PixelWidth != FileAccess.imageWidth || image.PixelHeight != FileAccess.imageHeight) throw new Exception();

            var bg = new ImageBrush
            {
                ImageSource = image
            };

            data.WindowTitle = Path.GetFileName(data.FileList[imageIndex].FileName) + " (" + bg.ImageSource.Width + "x" + bg.ImageSource.Height + ")";

            canvas.Background = bg;
            canvas.Width = bg.ImageSource.Width;
            canvas.Height = bg.ImageSource.Height;

            canvas.Scale(data.Zoom);

            Keyboard.Focus(canvas);
        }

        internal static void LoadRectangles(this Canvas canvas, WindowData data, int imageIndex)
        {
            canvas.Children.Clear();

            foreach (var sample in data.FileList[imageIndex].Rectangles)
            {
                canvas.Children.Add(canvas.InitializeShape(sample, new System.Windows.Shapes.Rectangle(), data.ScaleX, data.ScaleY, Brushes.OrangeRed));
            }
        }

        internal static void Scale(this Canvas canvas, double zoom)
        {
            canvas.LayoutTransform = new ScaleTransform(Math.Exp(zoom / WindowData.ZoomSpeed), Math.Exp(zoom / WindowData.ZoomSpeed));
        }

        internal static void WheelZoom(this Canvas canvas, WindowData data, double delta)
        {
            data.Zoom += delta;
            canvas.Scale(data.Zoom);
            data.RectangleHasFocus = false;
            Keyboard.Focus(canvas);
        }

        internal static async Task<bool> Click(this Canvas canvas, WindowData data, MouseButton changedButton)
        {
            var handled = false;
            if (changedButton == MouseButton.Left)
            {
                data.FileIndex++;
                data.RectangleIndex = 0;
                data.RectangleHasFocus = false;
                handled = true;
            }
            else if (changedButton == MouseButton.Right)
            {
                data.FileIndex--;
                data.RectangleIndex = 0;
                data.RectangleHasFocus = false;
                handled = true;
            }

            if (data.FileIndex < 0)
            {
                data.FileIndex = 0;
            }
            else if (data.FileIndex < data.FileList.Count)
            {
                await canvas.LoadImage(data, data.FileIndex);
                canvas.LoadRectangles(data, data.FileIndex);
            }
            else
            {
                data.FileIndex = data.FileList.Count;
                canvas.Children.Clear();
                canvas.Background = Brushes.SkyBlue;
            }
            return handled;
        }

        internal static Rectangle GetRect(this Canvas canvas, double scaleY, double scaleX, Point a, Point b)
        {
            var minX = Math.Min(a.X, b.X) / scaleX;
            var maxX = Math.Max(b.X, a.X) / scaleX;
            var minY = Math.Min(b.Y, a.Y) / scaleY;
            var maxY = Math.Max(a.Y, b.Y) / scaleY;
            return canvas.Rectangle(new Rectangle(minX, minY, maxX - minX, maxY - minY));
        }

        internal static void DragIntermediate(this Canvas canvas, WindowData data, Point dragStart, Point dragEnd)
        {
            canvas.Children.Clear();
            canvas.Children.Add(canvas.InitializeShape(canvas.GetRect(data.ScaleY, data.ScaleX, dragStart, dragEnd), new System.Windows.Shapes.Rectangle(), data.ScaleX, data.ScaleY, Brushes.RoyalBlue));
            data.WindowTitle = dragStart.X + "," + dragStart.Y + " - " + canvas.GetRect(data.ScaleY, data.ScaleX, dragStart, dragEnd).X + "," + canvas.GetRect(data.ScaleY, data.ScaleX, dragStart, dragEnd).Y;
        }

        internal static void DragFinal(this Canvas canvas, WindowData data, Point dragStart, Point dragEnd)
        {
            data.FileList[data.FileIndex].Rectangles.Insert(data.RectangleIndex, canvas.GetRect(data.ScaleY, data.ScaleX, dragStart, dragEnd));
            canvas.FocusRectangle(data, data.FileIndex, data.RectangleIndex);
            data.UnsavedChangesPresent = true;
        }

        internal static Rectangle Rectangle(this Canvas canvas, Rectangle rect)
        {
            if (canvas.Background is ImageBrush && ((ImageBrush)canvas.Background).ImageSource is BitmapImage)
            {
                var image = (BitmapImage)((ImageBrush)canvas.Background).ImageSource;
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

        internal static Rectangle TransformRectangle(this Canvas canvas, Rectangle rectangle, WindowData.Dimension dimension, int amount)
        {
            switch (dimension) {
                case WindowData.Dimension.X:
                    rectangle.Left += amount;
                    break;
                case WindowData.Dimension.Y:
                    rectangle.Top += amount;
                    break;
                case WindowData.Dimension.Size:
                    rectangle.Width += amount;
                    rectangle.Height += amount;
                    break;
            }
            rectangle = canvas.Rectangle(rectangle);
            return rectangle;
        }

        internal static async void KeyPress(this Canvas canvas, WindowData data, KeyEventArgs e, MainWindow mainWindow)
        {
            if (mainWindow.DragLeft || mainWindow.DragRight) return;
            if (data.RectangleHasFocus)
            {
                var step = 2;
                if (e.IsRepeat) step = 4;
                if (e.KeyboardDevice.IsKeyDown(Key.Back))
                {
                    await canvas.ChangeRectangle(data, -1);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Space))
                {
                    await canvas.ChangeRectangle(data, +1);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Left))
                {
                    data.FileList[data.FileIndex].Rectangles[data.RectangleIndex] = canvas.TransformRectangle(data.FileList[data.FileIndex].Rectangles[data.RectangleIndex], WindowData.Dimension.X, -step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Right))
                {
                    data.FileList[data.FileIndex].Rectangles[data.RectangleIndex] = canvas.TransformRectangle(data.FileList[data.FileIndex].Rectangles[data.RectangleIndex], WindowData.Dimension.X, step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Up))
                {
                    data.FileList[data.FileIndex].Rectangles[data.RectangleIndex] = canvas.TransformRectangle(data.FileList[data.FileIndex].Rectangles[data.RectangleIndex], WindowData.Dimension.Y, -step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Down))
                {
                    data.FileList[data.FileIndex].Rectangles[data.RectangleIndex] = canvas.TransformRectangle(data.FileList[data.FileIndex].Rectangles[data.RectangleIndex], WindowData.Dimension.Y, step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.W))
                {
                    data.FileList[data.FileIndex].Rectangles[data.RectangleIndex] = canvas.TransformRectangle(data.FileList[data.FileIndex].Rectangles[data.RectangleIndex], WindowData.Dimension.Size, step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.S))
                {
                    data.FileList[data.FileIndex].Rectangles[data.RectangleIndex] = canvas.TransformRectangle(data.FileList[data.FileIndex].Rectangles[data.RectangleIndex], WindowData.Dimension.Size, -step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Delete) && !e.IsRepeat)
                {
                    canvas.RemoveRectangle(data, data.FileIndex, data.RectangleIndex);

                    data.RectangleHasFocus = false;
                    canvas.LoadRectangles(data, data.FileIndex);
                    e.Handled = true;
                    return;
                }
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl) && e.KeyboardDevice.IsKeyDown(Key.Z) && !e.IsRepeat)
            {
                canvas.UndoRemoveRectangle(data, ref data.FileIndex, ref data.RectangleIndex);
                await canvas.LoadImage(data, data.FileIndex);
            }
            else if (!(e.Key == Key.Space || e.Key == Key.Back))
            {
                return;
            }
            data.RectangleIndex = Math.Min(Math.Max(data.RectangleIndex, 0), data.FileList[data.FileIndex].Rectangles.Count - 1);
            canvas.FocusRectangle(data, data.FileIndex, data.RectangleIndex);
            e.Handled = true;
        }

        internal static Shape InitializeShape(this Canvas canvas, Rectangle rect, Shape shape, double scaleX, double scaleY, Brush color)
        {
            shape.SetValue(WindowData.RectLeftProperty, rect.X * scaleX);
            shape.SetValue(WindowData.RectTopProperty, rect.Y * scaleY);
            shape.Width = rect.W * scaleX;
            shape.Height = rect.H * scaleY;

            shape.Stroke = color;
            shape.StrokeThickness = 2;

            return shape;
        }

        internal static async Task ChangeRectangle(this Canvas canvas, WindowData data, int offset)
        {
            data.RectangleIndex += offset;
            if (data.RectangleIndex < 0)
            {
                data.FileIndex--;
                if (data.FileIndex < 0)
                {
                    data.FileIndex = 0;
                    data.RectangleIndex = 0;
                }
                else
                {
                    data.RectangleIndex = data.FileList[data.FileIndex].Rectangles.Count - 1;
                }
                await canvas.LoadImage(data, data.FileIndex);
            }
            else if (data.RectangleIndex >= data.FileList[data.FileIndex].Rectangles.Count)
            {
                data.FileIndex++;
                if (data.FileIndex >= data.FileList.Count)
                {
                    data.FileIndex = data.FileList.Count - 1;
                    data.RectangleIndex = data.FileList[data.FileIndex].Rectangles.Count - 1;
                }
                else
                {
                    data.RectangleIndex = 0;
                }
                await canvas.LoadImage(data, data.FileIndex);
            }
        }

        internal static void RemoveRectangle(this Canvas canvas, WindowData data, int fileIndex, int rectangleIndex)
        {
            var removed = data.FileList[fileIndex].Rectangles[rectangleIndex];
            data.UndoStack.Push(Tuple.Create(fileIndex, removed));
            data.FileList[fileIndex].Rectangles.RemoveAt(rectangleIndex);
            data.RectangleHasFocus = false;
        }

        internal static void UndoRemoveRectangle(this Canvas canvas, WindowData data, ref int fileIndex, ref int rectangleIndex)
        {
            if (data.UndoStack.Count > 0)
            {
                var removed = data.UndoStack.Pop();
                fileIndex = removed.Item1;
                rectangleIndex = data.FileList[fileIndex].Rectangles.Count;
                data.FileList[fileIndex].Rectangles.Add(removed.Item2);
            }
        }

        internal static Rectangle TransformRectangle(this Canvas canvas, WindowData data, Rectangle rectangle, WindowData.Dimension dimension, int amount)
        {
            data.UnsavedChangesPresent = true;
            return canvas.TransformRectangle(rectangle, dimension, amount);
        }
    }
}
