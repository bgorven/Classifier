using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaBoost
{
    internal class Layer<Sample> where Sample : ISample
    {
        private ILearner<Sample> learner;
        private Configuration<ILearner<Sample>, Sample> parameters;
        internal float coefPos, coefNeg;
        internal float threshold;

        internal Layer(ILearner<Sample> learner, Configuration<ILearner<Sample>, Sample> parameters, float coefPos, float coefNeg, float threshold)
        {
            this.learner = learner;
            this.parameters = parameters;
            this.coefPos = coefPos;
            this.coefNeg = coefNeg;
            this.threshold = threshold;
        }

        internal float classify(Sample s)
        {
            learner.setSample(s);
            learner.setParams(parameters);
            return learner.classify() > threshold ? coefPos : coefNeg;
        }

        public override string ToString()
        {
            return learner + "[ " + parameters + " ] > " + threshold.ToString("n") + " ? " + coefPos + " : " + coefNeg;
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
