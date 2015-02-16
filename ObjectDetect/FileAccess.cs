using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using ObjectDetect.Properties;
using System.Threading;

namespace ObjectDetect
{
    internal static class FileAccess
    {
        internal class FileEntry
        {
            private readonly string _fileName;
            private readonly List<Rectangle> _rectangles;
            private readonly SlidingWindow _window;

            internal string FileName { get { return _fileName; } }
            internal IList<Rectangle> Rectangles { get { return _rectangles.AsReadOnly(); } }
            internal SlidingWindow Window { get { return _window;} }

            internal FileEntry(string file, List<Rectangle> rectangles, int width, int height, int startSize, int endSize, int stepSize, int offsetStepSize)
            {
                _fileName = file;
                _rectangles = rectangles;
                _window = new SlidingWindow(width, height, startSize, endSize, stepSize, offsetStepSize);
            }
        }

        //internal const int smallestWindow = 64, biggestWindow = 512, windowStep = 4, offsetStep = 6, imageWidth = 5184, imageHeight = 3456;
        internal static async Task<List<FileEntry>> LoadInfo(string dataFileName, CancellationToken cancellation, IProgress<Tuple<string, int>> currentTaskAndPercentComplete)
        {
            var fileList = new List<FileEntry>();

            try
            {
                using (var dataFile = new StreamReader(dataFileName))
                {
                    var directory = Path.GetDirectoryName(dataFileName) ?? "";
                    //SlidingWindow imageWindow = new SlidingWindow(imageWidth, imageHeight, smallestWindow, biggestWindow, windowStep, offsetStep);

                    var lineNo = -1;
                    for (var line = await dataFile.ReadLineAsync(); line != null; line = await dataFile.ReadLineAsync())
                    {
                        lineNo++;
                        var words = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                        string fileName;


                        int width, height;
                        if (words.Length < 2) continue;
                        else
                        {
                            fileName = Path.Combine(directory, words[0]);

                            if (cancellation.IsCancellationRequested)
                            {
                                return null;
                            }
                            currentTaskAndPercentComplete.Report(Tuple.Create("Opening \"" + fileName + "\"", -1));

                            try
                            {
                                using (var image = new Bitmap(fileName))
                                {
                                    width = image.Width;
                                    height = image.Height;
                                }
                            }
                            catch (FileNotFoundException)
                            {
                                var result = MessageBox.Show("\"" + words[0] + "\" not found in \"" + directory + "\"", fileName + " Not Found", MessageBoxButton.OKCancel);
                                if (result == MessageBoxResult.Cancel) return fileList;
                                continue;
                            }
                        }

                        int numSamples;
                        if (!int.TryParse(words[1], out numSamples))
                        {
                            throw new Exception("syntax error on line " + lineNo);
                        }

                        var samples = new List<Rectangle>(numSamples);

                        for (var i = 0; i < numSamples; i++)
                        {
                            const int xbase = 2, ybase = 3, wbase = 4, hbase = 5;
                            int x, y, w, h;
                            if (!(int.TryParse(words[xbase + i * 4], out x)
                                & int.TryParse(words[ybase + i * 4], out y)
                                & int.TryParse(words[wbase + i * 4], out w)
                                & int.TryParse(words[hbase + i * 4], out h)))
                            {
                                throw new Exception("syntax error on line " + lineNo + ": error reading sample number " + (i + 1));
                            }
                            samples.Add(new Rectangle(x, y, w, h));
                            //double xd, yd, wd, hd;
                            //if (imageWindow.getWindowDimensions(imageWindow.getNearestWindow(x, y, w, h), out xd, out yd, out wd, out hd))
                            //{
                            //    samples[i] = new rectangle(xd, yd, wd, hd);
                            //}
                        }

                        fileList.Add(new FileEntry(fileName, samples, width, height, Settings.Default.minRectSize, Settings.Default.maxRectSize, Settings.Default.rectSizeStep, Settings.Default.rectSlideStep));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(dataFileName + ": " + e.Message);
            }

            return fileList;
        }

        const int RectStringWidth = 20;
        internal static async Task SaveInfo(string dataFileName, List<FileEntry> fileList)
        {
            using (var dataFile = new StreamWriter(dataFileName))
            {
                var maxFilenameLength = fileList.Select(line => (Path.GetFileName(line.FileName) ?? "").Length).Max();

                var padding = (maxFilenameLength / RectStringWidth + 1) * RectStringWidth;

                foreach (var line in fileList)
                {
                    await dataFile.WriteAsync((Path.GetFileName(line.FileName) ?? "").PadRight(padding) + line.Rectangles.Count.ToString(CultureInfo.InvariantCulture).PadRight(RectStringWidth));

                    foreach (var rect in line.Rectangles)
                    {
                        await dataFile.WriteAsync((rect.Left + " " + rect.Top + " " + rect.Width + " " + rect.Height).PadRight(RectStringWidth));
                    }

                    await dataFile.WriteLineAsync();
                }
            }
        }
    }
}
