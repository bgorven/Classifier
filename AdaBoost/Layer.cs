namespace AdaBoost
{
    internal class Layer<Sample> where Sample : ISample
    {
        private readonly ILearner<Sample> learner;
        internal float coefPos, coefNeg;
        internal float threshold;

        internal Layer(ILearner<Sample> learner, float coefPos, float coefNeg, float threshold)
        {
            this.learner = learner;
            this.coefPos = coefPos;
            this.coefNeg = coefNeg;
            this.threshold = threshold;
        }

        internal float classify(Sample s)
        {
            learner.setSample(s);
            return learner.classify() > threshold ? coefPos : coefNeg;
        }

        public override string ToString()
        {
            return learner + " > " + threshold.ToString("n") + " ? " + coefPos + " : " + coefNeg;
        }

        internal void setThreshold(float threshold)
        {
            this.threshold = threshold;
        }

        internal float getThreshold()
        {
            return threshold;
        }
    }
}
