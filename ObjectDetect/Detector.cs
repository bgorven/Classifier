using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaBoost;

namespace ObjectDetect
{
    class Detector
    {
        private AdaBoost.Trainer<ImageSample> trainer;

        public Detector(List<FileAccess.FileEntry> fileList, int numPositive, int numNegative)
        {
            var positives = getPositiveSamples(fileList, numPositive);
            if (numNegative <= 0) numNegative = positives.Count;
            var negatives = getNegativeSamples(fileList, numNegative);

            trainer = new Trainer<ImageSample>(new LBPImageLearner[] { new LBPImageLearner() }, positives, negatives);
        }

        private List<ImageSample> getPositiveSamples(List<FileAccess.FileEntry> fileList, int numPositive)
        {
            var list = new List<ImageSample>();
            foreach (var p in fileList)
            {
                var window = new SlidingWindow(p.Width, p.Height, 128, 512, 4, 7);
                foreach (var rect in p.Rectangles)
                {
                    list.Add(new ImageSample(p.FileName, window.getNearestWindow(rect), window);
                }
            }
            return list;
        }

        private List<ImageSample> getNegativeSamples(List<FileAccess.FileEntry> fileList, int numNegative)
        {
            throw new NotImplementedException();
        }
    }
}
