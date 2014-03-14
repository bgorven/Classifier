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

        private async void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            await Load_File();
            Keyboard.Focus(canvas);
            e.Handled = true;
        }

        private async void MenuItem_Click_Save(object sender, RoutedEventArgs e)
        {
            await Save_File();
            Keyboard.Focus(canvas);
            e.Handled = true;
        }

        private double zoom = 0;
        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += e.Delta;
            Canvas_Scale(zoom);
            rectangleHasFocus = false;
            Keyboard.Focus(canvas);
            e.Handled = true;
        }

        private Point dragStart;
        private bool dragLeft = false, dragRight = false;

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                dragStart = e.GetPosition(canvas);
            }
            Keyboard.Focus(canvas);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Canvas_DragIntermediate(dragStart, e.GetPosition(canvas));
            }
            dragLeft = e.LeftButton == MouseButtonState.Pressed;
            dragRight = e.RightButton == MouseButtonState.Pressed;
            Keyboard.Focus(canvas);
        }

        private async void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (dragLeft)
                {
                    Canvas_DragFinal(dragStart, e.GetPosition(canvas));
                }
                else
                {
                    await Canvas_Click(MouseButton.Left);
                }
                dragLeft = false;
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (!dragRight)
                {
                    await Canvas_Click(MouseButton.Right);
                }
                dragRight = false;
            }
            Keyboard.Focus(canvas);
        }

        private async void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (dragLeft || dragRight) return;
            if (rectangleHasFocus)
            {
                int step = 2;
                if (e.IsRepeat) step = 4;
                if (e.KeyboardDevice.IsKeyDown(Key.Back))
                {
                    await changeRectangle(-1);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Space))
                {
                    await changeRectangle(+1);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Left))
                {
                    fileList[fileIndex].Rectangles[rectangleIndex] = transformRectangle(fileList[fileIndex].Rectangles[rectangleIndex], Dimension.x, -step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Right))
                {
                    fileList[fileIndex].Rectangles[rectangleIndex] = transformRectangle(fileList[fileIndex].Rectangles[rectangleIndex], Dimension.x, step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Up))
                {
                    fileList[fileIndex].Rectangles[rectangleIndex] = transformRectangle(fileList[fileIndex].Rectangles[rectangleIndex], Dimension.y, -step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Down))
                {
                    fileList[fileIndex].Rectangles[rectangleIndex] = transformRectangle(fileList[fileIndex].Rectangles[rectangleIndex], Dimension.y, step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.W))
                {
                    fileList[fileIndex].Rectangles[rectangleIndex] = transformRectangle(fileList[fileIndex].Rectangles[rectangleIndex], Dimension.size, step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.S))
                {
                    fileList[fileIndex].Rectangles[rectangleIndex] = transformRectangle(fileList[fileIndex].Rectangles[rectangleIndex], Dimension.size, -step);
                }
                if (e.KeyboardDevice.IsKeyDown(Key.Delete) && !e.IsRepeat)
                {
                    removeRectangle(fileIndex, rectangleIndex);

                    rectangleHasFocus = false;
                    Canvas_Load_Rectangles(fileIndex);
                    e.Handled = true;
                    return;
                }
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl) && e.KeyboardDevice.IsKeyDown(Key.Z) && !e.IsRepeat)
            {
                undoRemoveRectangle(ref fileIndex, ref rectangleIndex);
                await Canvas_Load_Image(fileIndex);
            }
            else if (!(e.Key == Key.Space || e.Key == Key.Back))
            {
                return;
            }
            rectangleIndex = Math.Min(Math.Max(rectangleIndex, 0), fileList[fileIndex].Rectangles.Count - 1);
            Canvas_Focus_Rectangle(fileIndex, rectangleIndex);
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Confirm_Discard_Changes()) e.Cancel = true;
        }
    }
}
