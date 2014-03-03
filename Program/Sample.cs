using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdaBoost;

namespace ConsoleTrainer
{
    struct Sample : ISample
    {
        public int[] features;

        public Sample(int feat)
        {
            features = new int[feat];
        }

        public IEnumerable<Learner> getLearnerArray()
        {
            return from feat in Enumerable.Range(0, features.Count()) select new Learner(feat);
        }

        public class Learner : ILearner<Sample>
        {
            private int feature;
            private Sample sample;

            public Learner(int feat)
            {
                feature = feat;
            }

            public float classify()
            {
                return sample.features[feature];
            }

            public void setSample(Sample s)
            {
                sample = s;
            }

            public void setParams(Configuration<ILearner<Sample>, Sample> parameters) { }

            public IEnumerable<Configuration<ILearner<Sample>, Sample>> getPossibleParams()
            {
                return new Configuration<ILearner<Sample>, Sample>[] { new param() };
            }

            public int getFeature()
            {
                return feature;
            }
            
            public string getUniqueIDString()
            {
                return "Sample.Learner " + feature;
            }

            public override string ToString()
            {
                return getUniqueIDString();
            }

            public struct param : Configuration<ILearner<Sample>, Sample> { }
        }
    }
}
