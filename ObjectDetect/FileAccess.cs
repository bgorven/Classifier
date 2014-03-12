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
        public static List<Tuple<Uri, rectangle[]>> loadInfo(string dataFileName)
        {
            List<Tuple<Uri, rectangle[]>> fileNames = new List<Tuple<Uri, rectangle[]>>();

            try
            {
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
                        if (!int.TryParse(words[1], out numSamples))
                        {
                            throw new Exception("syntax error on line " + lineNo);
                        }

                        var samples = new rectangle[numSamples];
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

                            samples[i] = new rectangle(x, y, w, h);
                        }

                        fileNames.Add(new Tuple<Uri, rectangle[]>(file, samples));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(dataFileName + ": " + e.Message);
            }

            return fileNames;
        }
    }
}
