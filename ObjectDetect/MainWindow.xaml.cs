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
            _data = new WindowData(this);
        }

        private readonly WindowData _data;

        internal bool DragLeft
        {
            set { _dragLeft = value; }
            get { return _dragLeft; }
        }

        internal bool DragRight
        {
            set { _dragRight = value; }
            get { return _dragRight; }
        }

        private async void MenuItem_Dataset_Open(object sender, RoutedEventArgs e)
        {
            await _data.Load_File();
            Keyboard.Focus(Canvas);
            e.Handled = true;
        }

        private async void MenuItem_Dataset_Save(object sender, RoutedEventArgs e)
        {
            await _data.Save_File();
            Keyboard.Focus(Canvas);
            e.Handled = true;
        }

        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Canvas.WheelZoom(_data, e.Delta);
            e.Handled = true;
        }

        private Point _dragStart;
        private bool _dragLeft, _dragRight;

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
                Canvas.DragIntermediate(_data, _dragStart, e.GetPosition(Canvas));
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
                    Canvas.DragFinal(_data, _dragStart, e.GetPosition(Canvas));
                }
                else
                {
                    await Canvas.Click(_data, MouseButton.Left);
                }
                _dragLeft = false;
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (!_dragRight)
                {
                    await Canvas.Click(_data, MouseButton.Right);
                }
                _dragRight = false;
            }
            Keyboard.Focus(Canvas);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_data.UnsavedChangesPresent && !Confirm_Discard_Changes()) e.Cancel = true;
        }

        private void MenuItem_Classifier_Train(object sender, RoutedEventArgs e)
        {
            var detector = new Detector(_data.FileList, Settings.Default.numPositive, Settings.Default.numNegative);
            detector.Train(Settings.Default.numLayers);
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

        internal bool Confirm_Discard_Changes()
        {
            return MessageBox.Show(this, "Discard unsaved changes?", "Data file Not Saved", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            Canvas.KeyPress(_data, e, this);
        }
    }
}
