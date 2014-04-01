using System;
using System.Collections.Generic;
using System.Linq;
using AdaBoost;

namespace Program
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
            private readonly int numFeatures;
            private Sample sample;

            private Learner(Learner other)
            {
                this.feature = other.feature;
                this.numFeatures = other.numFeatures;
                this.sample = other.sample;
            }

            public Learner(int feat)
            {
                numFeatures = feat;
            }

            public float classify()
            {
                return sample.features[feature];
            }

            public void setSample(Sample s)
            {
                sample = s;
            }

            public ILearner<Sample> withParams(string parameters)
            {
                var ret = new Learner(this);
                if (!int.TryParse(parameters, out ret.feature)) throw new ArgumentException();
                return ret;
            }

            public IEnumerable<string> getPossibleParams()
            {
                for (int f = 0; f < numFeatures; f++)
                {
                    yield return f.ToString();
                }
            }

            public int getFeature()
            {
                return feature;
            }
            
            public string getUniqueIDString()
            {
                return "Sample.Learner";
            }

            public override string ToString()
            {
                return getUniqueIDString() + " [" + feature + "]";
            }
        }
    }
}
