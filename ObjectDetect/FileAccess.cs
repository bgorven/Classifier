using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace ObjectDetect
{
    public class FileAccess
    {
        public class FileEntry
        {
            public readonly string FileName;
            public readonly List<Rectangle> Rectangles;
            public readonly SlidingWindow Window;

            public FileEntry(string file, List<Rectangle> rectangles, int width, int height, int startSize, int endSize, int stepSize, int offsetStepSize)
            {
                FileName = file;
                Rectangles = rectangles;
                Window = new SlidingWindow(width, height, startSize, endSize, stepSize, offsetStepSize);
            }
        }

        //public const int smallestWindow = 64, biggestWindow = 512, windowStep = 4, offsetStep = 6, imageWidth = 5184, imageHeight = 3456;
        public static async Task<List<FileEntry>> LoadInfo(string dataFileName)
        {
            List<FileEntry> fileList = new List<FileEntry>();

            try
            {
                using (var dataFile = new StreamReader(dataFileName))
                {
                    string directory = Path.GetDirectoryName(dataFileName) ?? "";
                    //SlidingWindow imageWindow = new SlidingWindow(imageWidth, imageHeight, smallestWindow, biggestWindow, windowStep, offsetStep);

                    int lineNo = -1;
                    for (string line = await dataFile.ReadLineAsync(); line != null; line = await dataFile.ReadLineAsync())
                    {
                        lineNo++;
                        var words = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                        string fileName;
                        int width, height;
                        if (words.Length < 2) continue;
                        else
                        {
                            fileName = Path.Combine(directory, words[0]);
                            try
                            {
                                using (var image = new System.Drawing.Bitmap(fileName))
                                {
                                    width = image.Width;
                                    height = image.Height;
                                }
                            }
                            catch (FileNotFoundException)
                            {
                                var result = System.Windows.MessageBox.Show("\"" + words[0] + "\" not found in \"" + directory + "\"", fileName + " Not Found", System.Windows.MessageBoxButton.OKCancel);
                                if (result == System.Windows.MessageBoxResult.Cancel) return fileList;
                                continue;
                            }
                        }

                        int numSamples;
                        if (!int.TryParse(words[1], out numSamples))
                        {
                            throw new Exception("syntax error on line " + lineNo);
                        }

                        var samples = new List<Rectangle>(numSamples);

                        for (int i = 0; i < numSamples; i++)
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

                        fileList.Add(new FileEntry(fileName, samples, width, height, Properties.Settings.Default.minRectSize, Properties.Settings.Default.maxRectSize, Properties.Settings.Default.rectSizeStep, Properties.Settings.Default.rectSlideStep));
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
        public static async Task SaveInfo(string dataFileName, List<FileEntry> fileList)
        {
            using (var dataFile = new StreamWriter(dataFileName))
            {
                int maxFilenameLength = fileList.Select(line => (Path.GetFileName(line.FileName) ?? "").Length).Max();

                int padding = (maxFilenameLength / RectStringWidth + 1) * RectStringWidth;

                foreach (var line in fileList)
                {
                    await dataFile.WriteAsync((Path.GetFileName(line.FileName) ?? "").PadRight(padding) + line.Rectangles.Count.ToString().PadRight(RectStringWidth));

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
