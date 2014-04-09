using System;
using System.Diagnostics;
using System.Globalization;
using Utilities.Collections;

namespace AdaBoost
{
    public class Layer<TSample> where TSample : ISample
    {
        private readonly ILearner<TSample> _learner;
        internal float CoefPos, CoefNeg;
        internal float Threshold;

        internal Layer(ILearner<TSample> learner, float coefPos, float coefNeg, float threshold)
        {
            _learner = learner;
            CoefPos = coefPos;
            CoefNeg = coefNeg;
            Threshold = threshold;
        }

        public Layer(ILearner<TSample> learner, string configuration)
        {
            if (learner == null) throw new ArgumentNullException("learner");
            if (configuration == null) throw new ArgumentNullException("configuration");

            configuration = configuration.Substring(_learner.UniqueId.Length + 2);
            _learner = learner.WithConfiguration(configuration.Substring(0, configuration.LastIndexOf('>')));
            configuration.Substring(configuration.LastIndexOf('>') + 1)
                .Split(new[] {'?', ':'})
                .Unpack(out Threshold, out CoefPos, out CoefNeg, float.Parse);
        }

        public override string ToString()
        {
            return _learner.UniqueId + ": " + _learner.Config + "> " + Threshold.ToString("R", CultureInfo.InvariantCulture) + " ? " + CoefPos.ToString("R", CultureInfo.InvariantCulture) + " : " + CoefNeg.ToString("R", CultureInfo.InvariantCulture);
        }

        internal float Classify(TSample s)
        {
            _learner.SetSample(s);
            return _learner.Classify() > Threshold ? CoefPos : CoefNeg;
        }
    }
}
