using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdaBoost;
using Util.Random;

namespace ObjectDetect
{
    class Detector
    {
        private readonly Trainer<ImageSample> _trainer;

        public Detector(List<FileAccess.FileEntry> fileList, int numPositive, int numNegative)
        {
            var positives = GetPositiveSamples(fileList, numPositive).ToList();
            if (numNegative <= 0) numNegative = positives.Count;
            var negatives = GetNegativeSamples(fileList, numNegative).ToList();

            _trainer = new Trainer<ImageSample>(new[] { new LBPImageLearner() }, positives, negatives);
        }

        public void Train(int numLayers)
        {
            for (var i = numLayers; i > 0; i--)
            {
                _trainer.AddLayer();
            }
        }

        internal static IEnumerable<ImageSample> GetPositiveSamples(List<FileAccess.FileEntry> fileList, int numPositive)
        {
            var positives = from file in fileList
                from rect in file.Rectangles
                select new ImageSample(file.FileName, file.Window.GetNearestWindow(rect), file.Window);
            if (numPositive > 0) positives = positives.Take(numPositive);
            return positives;
        }

        internal static IEnumerable<ImageSample> GetNegativeSamples(List<FileAccess.FileEntry> fileList, int numNegative)
        {
            var negatives = new ImageSample[numNegative];
            var fileIter = fileList.GetEnumerator();
            var intIter = new PermutedSequence(fileList.Max(entry => entry.Window.NumWindows));
            intIter.MoveNext();

            var i = 0;
            while (i < numNegative)
            {
                if (fileIter.MoveNext())
                {
                    Debug.Assert(fileIter.Current != null, "fileIter.Current != null");
                    try
                    {
                        var rect = fileIter.Current.Window.GetRectangle(intIter.Current);
                        if (!fileIter.Current.Rectangles.Any(rect.Overlaps))
                        {
                            negatives[i] = new ImageSample(fileIter.Current.FileName, intIter.Current,
                                fileIter.Current.Window);
                            i++;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //continue;
                    }
                }
                else
                {
                    fileIter = fileList.GetEnumerator();
                    if (!intIter.MoveNext()) throw new Exception("Not enough integers.");
                }
            }
            return negatives;
        }

        internal async Task Write(FileStream fileStream)
        {
            using (var writer = new StreamWriter(fileStream))
            {
                await writer.WriteAsync(_trainer.Classifier.ToString());
            }

        }

        public static async Task<TrainingData> LoadData(FileStream fileStream)
        {
            var retVal = new TrainingData();
            using (var reader = new StreamReader(fileStream))
            {
                var line = await reader.ReadLineAsync();

                if (line.StartsWith(LBPImageLearner.UniqueIdString))
                {
                    retVal.Classifier.AddLayer<LBPImageLearner>(line);
                }
            }
            return retVal;
        }

        internal class TrainingData
        {
            internal Classifier<ImageSample> Classifier;
            public TrainingData()
            {
                Classifier = new Classifier<ImageSample>();
            }
        }
    }
}
