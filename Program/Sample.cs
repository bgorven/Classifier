using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdaBoost;

namespace Program
{
    struct Sample : ISample
    {
        public int[] Features;

        public Sample(int feat)
        {
            Features = new int[feat];
        }

        public IEnumerable<Learner> GetLearnerArray()
        {
            return from feat in Enumerable.Range(0, Features.Count()) select new Learner(feat);
        }

        public class Learner : ILearner<Sample>
        {
            private int _feature;
            private readonly int _numFeatures;
            private Sample _sample;

            private Learner(Learner other)
            {
                _feature = other._feature;
                _numFeatures = other._numFeatures;
                _sample = other._sample;
            }

            public Learner(int feat)
            {
                _numFeatures = feat;
            }

            public float Classify()
            {
                return _sample.Features[_feature];
            }

            public void SetSample(Sample value)
            {
                _sample = value;
            }

            public string Config { get { return _feature.ToString(CultureInfo.InvariantCulture); } }

            public ILearner<Sample> WithConfiguration(string configuration)
            {
                var ret = new Learner(this);
                if (!int.TryParse(configuration, out ret._feature)) throw new ArgumentException();
                return ret;
            }

            public IEnumerable<string> AllPossibleConfigurations()
            {
                for (var f = 0; f < _numFeatures; f++)
                {
                    yield return f.ToString(CultureInfo.InvariantCulture);
                }
            }

            public int GetFeature()
            {
                return _feature;
            }

            public string UniqueId
            {
                get { return "Sample.Learner"; }
            }

            public override string ToString()
            {
                return UniqueId + " [" + _feature + "]";
            }
        }
    }
}
