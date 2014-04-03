namespace AdaBoost
{
    internal class Layer<TSample> where TSample : ISample
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

        internal float Classify(TSample s)
        {
            _learner.SetSample(s);
            return _learner.Classify() > Threshold ? CoefPos : CoefNeg;
        }

        public override string ToString()
        {
            return _learner + " > " + Threshold.ToString("n") + " ? " + CoefPos + " : " + CoefNeg;
        }
    }
}
