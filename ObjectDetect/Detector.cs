using System;
using System.Collections.Generic;
using System.Linq;
using AdaBoost;
using ObjectDetect.Properties;

namespace ObjectDetect
{
    class Detector
    {
        private AdaBoost.Trainer<ImageSample> trainer;

        public Detector(List<FileAccess.FileEntry> fileList, int numPositive, int numNegative)
        {
            var positives = getPositiveSamples(fileList, numPositive).ToList();
            if (numNegative <= 0) numNegative = positives.Count;
            var negatives = getNegativeSamples(fileList, numNegative).ToList();

            trainer = new Trainer<ImageSample>(new LBPImageLearner[] { new LBPImageLearner() }, positives, negatives);
        }

        public void Train(int numLayers)
        {
            for (; numLayers > 0; numLayers--)
            {
                trainer.addLayer();
            }
        } 

        internal static IEnumerable<ImageSample> getPositiveSamples(List<FileAccess.FileEntry> fileList, int numPositive)
        {
            return (from file in fileList from rect in file.Rectangles 
                    select new ImageSample(file.FileName, file.Window.getNearestWindow(rect), file.Window));
        }

        internal static IEnumerable<ImageSample> getNegativeSamples(List<FileAccess.FileEntry> fileList, int numNegative)
        {
            throw new NotImplementedException();
        }
    }
}
