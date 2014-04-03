using System;
using System.Collections.Generic;
using System.Linq;
using AdaBoost;

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

            _trainer = new Trainer<ImageSample>(new LBPImageLearner[] { new LBPImageLearner() }, positives, negatives);
        }

        public void Train(int numLayers)
        {
            for (; numLayers > 0; numLayers--)
            {
                _trainer.AddLayer();
            }
        } 

        internal static IEnumerable<ImageSample> GetPositiveSamples(List<FileAccess.FileEntry> fileList, int numPositive)
        {
            return (from file in fileList from rect in file.Rectangles 
                    select new ImageSample(file.FileName, file.Window.GetNearestWindow(rect), file.Window));
        }

        internal static IEnumerable<ImageSample> GetNegativeSamples(List<FileAccess.FileEntry> fileList, int numNegative)
        {
            throw new NotImplementedException();
        }
    }
}
