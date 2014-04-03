using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ObjectDetect.Properties;

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

        private async void MenuItem_Dataset_Open(object sender, RoutedEventArgs e)
        {
            await Load_File();
            Keyboard.Focus(Canvas);
            e.Handled = true;
        }

        private async void MenuItem_Dataset_Save(object sender, RoutedEventArgs e)
        {
            await Save_File();
            Keyboard.Focus(Canvas);
            e.Handled = true;
        }

        private double _zoom = 0;
        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _zoom += e.Delta;
            Canvas_Scale(_zoom);
            _rectangleHasFocus = false;
            Keyboard.Focus(Canvas);
            e.Handled = true;
        }

        private Point _dragStart;
        private bool _dragLeft = false, _dragRight = false;

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _dragStart = e.GetPosition(Canvas);
            }
            Keyboard.Focus(Canvas);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Canvas_DragIntermediate(_dragStart, e.GetPosition(Canvas));
            }
            _dragLeft = e.LeftButton == MouseButtonState.Pressed;
            _dragRight = e.RightButton == MouseButtonState.Pressed;
            Keyboard.Focus(Canvas);
        }

        private async void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_dragLeft)
                {
                    Canvas_DragFinal(_dragStart, e.GetPosition(Canvas));
                }
                else
                {
                    await Canvas_Click(MouseButton.Left);
                }
                _dragLeft = false;
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (!_dragRight)
                {
                    await Canvas_Click(MouseButton.Right);
                }
                _dragRight = false;
            }
            Keyboard.Focus(Canvas);
        }

        private async void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (_dragLeft || _dragRight) return;
            if (_rectangleHasFocus)
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

                    _rectangleHasFocus = false;
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!Confirm_Discard_Changes()) e.Cancel = true;
        }

        private void MenuItem_Classifier_Train(object sender, RoutedEventArgs e)
        {
            var detector = new Detector(_fileList, 2000, 2000);
            detector.Train(2000);
        }

        private void MenuItem_Classifier_Save(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuItem_Settings_Edit(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow {Owner = this};
            var success = settingsWindow.ShowDialog() ?? false;
            if (success)
            {
                Settings.Default.Save();
            }
            else
            {
                Settings.Default.Reload();
            }
        }
    }
}
