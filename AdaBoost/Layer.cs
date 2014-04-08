using Util.Collections;

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

        public Layer(ILearner<TSample> learner, string config)
        {
            config = config.Substring(_learner.UniqueId.Length + 2);
            _learner = learner.WithParams(config.Substring(0, config.LastIndexOf('>')));
            config.Substring(config.LastIndexOf('>') + 1)
                .Split(new[] {'?', ':'})
                .Unpack(out Threshold, out CoefPos, out CoefNeg, float.Parse);
        }

        public override string ToString()
        {
            return _learner.UniqueId + ": " + _learner.Params + "> " + Threshold.ToString("R") + " ? " + CoefPos.ToString("R") + " : " + CoefNeg.ToString("R");
        }

        internal float Classify(TSample s)
        {
            _learner.Sample = s;
            return _learner.Classify() > Threshold ? CoefPos : CoefNeg;
        }
    }
}
