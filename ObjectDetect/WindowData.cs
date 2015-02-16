using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Threading;

namespace ObjectDetect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal class WindowData
    {
        private readonly MainWindow _window;
        internal WindowData(MainWindow window)
        {
            _window = window;
        }

        internal string WindowTitle { set { _window.Title = value; } }

        internal static readonly DependencyProperty RectLeftProperty = Canvas.LeftProperty;
        internal static readonly DependencyProperty RectTopProperty = Canvas.TopProperty;

        internal int FileIndex;
        internal int RectangleIndex;
        internal List<FileAccess.FileEntry> FileList;
        private const string DataFileExt = ".dat";
        private const string DataFileFilter = "datafiles (.dat)|*.dat";
        internal bool UnsavedChangesPresent;

        internal double ScaleX;
        internal double ScaleY;
        internal const double ZoomSpeed = 960;
        internal double Zoom;

        internal bool RectangleHasFocus;
        internal readonly Stack<Tuple<int, Rectangle>> UndoStack = new Stack<Tuple<int, Rectangle>>();

        internal enum Dimension {
            X, Y, Size
        }

        internal async Task Load_File(CancellationToken cancellation, IProgress<Tuple<string, int>> currentTaskAndPercentComplete)
        {
            if (UnsavedChangesPresent && !_window.Confirm_Discard_Changes()) return;

            var dialog = new OpenFileDialog
            {
                DefaultExt = DataFileExt,
                Filter = DataFileFilter
            };
            if (dialog.ShowDialog() ?? false)
            {
                FileList = await FileAccess.LoadInfo(dialog.FileName, cancellation, currentTaskAndPercentComplete);
                if (cancellation.IsCancellationRequested)
                {
                    return;
                }

                UnsavedChangesPresent = false;
                if (FileList.Any())
                {
                    FileIndex = 0;
                    RectangleIndex = 0;
                    RectangleHasFocus = false;
                    await _window.Canvas.LoadImage(this, FileIndex);
                    _window.Canvas.LoadRectangles(this, FileIndex);
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
                await FileAccess.SaveInfo(dialog.FileName, FileList);
                UnsavedChangesPresent = false;
            }
        }
    }
}
