using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ObjectDetect
{
    public class FileAccess
    {
        public static List<Tuple<Uri, ImageSample[]>> loadInfo(string dataFileName)
        {
            List<Tuple<Uri, ImageSample[]>> fileNames = new List<Tuple<Uri, ImageSample[]>>();

            int smallestRect = int.MaxValue;
            int largestRect = int.MinValue;

            float smallestRatio = float.PositiveInfinity;
            float largestRatio = float.NegativeInfinity;

            using (var dataFile = new StreamReader(dataFileName))
            {
                Uri directory = new Uri(Path.GetDirectoryName(dataFileName) + Path.DirectorySeparatorChar);
                Uri file = null;

                int lineNo = -1;
                for (string line = dataFile.ReadLine(); line != null; line = dataFile.ReadLine())
                {
                    lineNo++;
                    var words = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length < 2) continue;
                    else
                    {
                        file = new Uri(directory, words[0]);
                        if (!System.IO.File.Exists(file.AbsolutePath))
                        {
                            var result = System.Windows.MessageBox.Show("\"" + words[0] + "\" not found in \"" + directory.AbsolutePath + "\"", file.AbsolutePath + " Not Found", System.Windows.MessageBoxButton.OKCancel);
                            if (result == System.Windows.MessageBoxResult.Cancel) return fileNames;
                        }
                    }

                    int numSamples;
                    bool success = int.TryParse(words[1], out numSamples);
                    if (!success)
                    {
                        Console.WriteLine(Path.GetFileName(dataFileName) + ": syntax error on line " + lineNo);
                        continue;
                    }

                    var samples = new ImageSample[numSamples];
                    int next = 2;
                    for (int i = 0; i < numSamples; i++)
                    {
                        int x;
                        success |= int.TryParse(words[next++], out x);
                        int y;
                        success |= int.TryParse(words[next++], out y);
                        int w;
                        success |= int.TryParse(words[next++], out w);
                        int h;
                        success |= int.TryParse(words[next++], out h);

                        if (!success)
                        {
                            Console.WriteLine(Path.GetFileName(dataFileName) + ": syntax error on line " + lineNo + ": error reading sample number " + (i + 1));
                            break;
                        }

                        var ratio = (float)w / h;
                        ratio = ratio > 1 ? ratio : 1 / ratio;

                        smallestRatio = Math.Min(ratio, smallestRatio);
                        largestRatio = Math.Max(ratio, largestRatio);

                        smallestRect = Math.Min(Math.Max(w, h), smallestRect);
                        largestRect = Math.Max(Math.Max(w, h), largestRect);

                        samples[i] = new ImageSample(file, x, y, w, h);
                    }
                    if (!success) continue;

                    fileNames.Add(new Tuple<Uri, ImageSample[]>(file, samples));
                }
            }

            List<ImageSample> positives = new List<ImageSample>();
            List<ImageSample> negatives = new List<ImageSample>();

            return fileNames;
        }
    }
}
