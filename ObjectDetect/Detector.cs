using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using AdaBoost;
using Utilities.Random;
using System.Threading;

namespace ObjectDetect
{
    class Detector
    {
        private readonly Trainer<ImageSample> _trainer;

        internal Detector(List<FileAccess.FileEntry> fileList, int numPositive, int numNegative)
        {
            var positives = GetPositiveSamples(fileList, numPositive).ToList();
            if (numNegative <= 0) numNegative = positives.Count;
            var negatives = GetNegativeSamples(fileList, numNegative).ToList();

            _trainer = new Trainer<ImageSample>(new[] { new LBPImageLearner() }, positives, negatives);
        }

        internal async Task Train(int numLayers, CancellationToken cancellation, IProgress<Tuple<string, int>> taskAndPercentComplete)
        {
            for (var i = numLayers; i > 0; i--)
            {
                await Task.Run(() => _trainer.AddLayer(cancellation, taskAndPercentComplete));
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
            try
            {
                intIter.MoveNext();

                var i = 0;
                while (i < numNegative)
                {
                    if (fileIter.MoveNext())
                    {
                        if (fileIter.Current == null) continue;
                        Rectangle rect;
                        try
                        {
                            rect = fileIter.Current.Window.GetRectangle(intIter.Current);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            continue;
                        }
                        if (!fileIter.Current.Rectangles.Any(rect.Overlaps))
                        {
                            negatives[i] = new ImageSample(fileIter.Current.FileName, intIter.Current,
                                fileIter.Current.Window);
                            i++;
                        }
                    }
                    else
                    {
                        fileIter.Dispose();
                        fileIter = fileList.GetEnumerator();
                        if (!intIter.MoveNext()) throw new CryptographicUnexpectedOperationException("Not enough integers.");
                    }
                }
                return negatives;
            }
            finally
            {
                fileIter.Dispose();
                intIter.Dispose();
            }
        }

        internal async Task SaveData(StreamWriter writer)
        {
            await writer.WriteAsync(_trainer.Classifier.ToString());
        }

        internal static async Task<Classifier<ImageSample>> LoadData(StreamReader reader)
        {
            var retVal = new Classifier<ImageSample>();

            var line = await reader.ReadLineAsync();

            if (line.StartsWith(LBPImageLearner.UniqueIdString))
            {
                retVal.AddLayer<LBPImageLearner>(line);
            }

            return retVal;
        }
    }
}
